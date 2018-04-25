using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VocationalProject_DBUtility;
using VocationalProject.Areas.Admin.Models;
using VocationalProject.Controllers;
using VocationalProject.Models;
using VocationalProject_Bll;
using VocationalProject_Dal;
using VocationalProject.App_Start;

namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
    FileName:课程管理
    Copyright（c）2017-金融教育在线技术开发部
    Author:fly
    Create Date:2017-8-30
    ******************************************************************/
    public class CourseManagementController : BaseController
    {
        //
        // GET: /Admin/CourseManagement/
        CommonBll commBll = new CommonBll();
        public ActionResult CourseManagement()
        {
            return View();
        }

        public ActionResult CourseList()
        {
            return View();
        }

        public ActionResult EditCourseManagement(int? Id)
        {
            DataTable data = commBll.GetListDatatable("*", "ykt_CourseManagement", " and Id=" + Id);
            var m = new CourseManagementModel();
            if (data.Rows.Count > 0)
            {
                m.Id = Convert.ToInt32(data.Rows[0]["Id"]);
                m.CourseName = data.Rows[0]["CourseName"].ToString();
                m.UploadName = Convert.ToInt32(data.Rows[0]["UploadName"]);
                m.UploadTime = Convert.ToDateTime(data.Rows[0]["UploadTime"]);
                m.CourseUrl = data.Rows[0]["CourseUrl"].ToString();
                m.CourseCoverUrl = data.Rows[0]["CourseCoverUrl"].ToString();
                m.Format = data.Rows[0]["Format"].ToString();
                m.Jurisdiction = Convert.ToInt32(data.Rows[0]["Jurisdiction"]);
                m.FileSize = Convert.ToInt32(data.Rows[0]["FileSize"]);
                m.IsDelete = Convert.ToInt32(data.Rows[0]["IsDelete"]);
            }
            return View(m);
        }

        /// <summary>
        /// 添加课程
        /// </summary>
        /// <returns></returns>
        public string Upload()
        {
            double FileSize = 0;
            string filePath1 = string.Empty;
            string filePath = string.Empty;
            string fileType = string.Empty;
            string url1 = string.Empty;
            string url = string.Empty;
            string imgName = string.Empty;
            string imgUrl = "/img/Map";
            string Id = Request["Id"];
            string CourseName = Request["CourseName"];
            string use = Request["use"];
            HttpPostedFileBase hpFile = Request.Files["upfile"];
            HttpPostedFileBase hpFile1 = Request.Files["upimg"];
            if (Id == null)
            {
                int Count = commBll.GetRecordCount("ykt_CourseManagement", " and CourseName='" + CourseName + "' and IsDelete=1");
                if (Count > 0)
                {
                    return "999";
                }
            }
            //上传图片不是NULL
            if (hpFile1 != null)
            {
                //获取后缀名
                string fileType1 = System.IO.Path.GetExtension(hpFile1.FileName);
                if (fileType1.ToLower() != ".gif" && fileType1.ToLower() != ".jpg" &&
                    fileType1.ToLower() != ".jpeg" && fileType1.ToLower() != ".bmp" &&
                    fileType1.ToLower() != ".png")
                {
                    return "-1";
                }
                url1 = ResourceConversionURL(hpFile1.FileName);
                filePath1 = Server.MapPath(url1);
                hpFile1.SaveAs(filePath1);
            }
            //上传文件
            if (hpFile != null)
            {
                //获取文件后缀名
                fileType = System.IO.Path.GetExtension(hpFile.FileName);
                string FileName = Path.GetFileName(hpFile.FileName).Replace(fileType, "").Replace(".", "") + fileType;
                if (fileType.ToLower() != ".mp4" && fileType.ToLower() != ".mp3" && fileType.ToLower() != ".ppt" && fileType.ToLower() != ".doc")
                {
                    return "-1";
                }
                url = ResourceConversionURL(FileName);
                filePath = Server.MapPath(url);
                hpFile.SaveAs(filePath);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(Server.MapPath(url));
                //获取文件大小
                FileSize = System.Math.Ceiling(fileInfo.Length / 1024.0);
                fileType = fileType.Substring(1, fileType.Length - 1).ToLower();
            }
            LicenseHelper.ModifyInMemory.ActivateMemoryPatching();
            if (fileType == "pptx" || fileType == "ppt")
            {
                PictureConversion pc = new PictureConversion();
                imgName = pc.ConvertToImage_PPT(filePath, Server.MapPath(imgUrl), 0, 0, 145, fileType, imgUrl);
                if (imgName == "888")
                {
                    return imgName;
                }
            }
            if (use == null) { use = "2"; }
            if (Id == null)
            {
                string list = @"[CourseName]
           ,[UploadName]
           ,[UploadTime]
           ,[CourseUrl]
           ,[CourseCoverUrl]
           ,[Format]
           ,[Jurisdiction]
           ,[FileSize]
           ,[IsDelete]
           ,[Spare1]
           ,[Spare2]
           ,[Spare3]";
                string value = @"@CourseName
           ,@UploadName
           ,@UploadTime
           ,@CourseUrl
           ,@CourseCoverUrl
           ,@Format
           ,@Jurisdiction
           ,@FileSize
           ,@IsDelete
           ,@Spare1
           ,@Spare2
           ,@Spare3";
                SqlParameter[] para = new SqlParameter[]{
                new SqlParameter("@CourseName",CourseName),
                new SqlParameter("@UploadName",UId),
                new SqlParameter("@UploadTime",DateTime.Now.ToString()),
                new SqlParameter("@CourseUrl",url),
                new SqlParameter("@CourseCoverUrl",url1),
                new SqlParameter("@Format",fileType),
                new SqlParameter("@Jurisdiction",use),
                new SqlParameter("@FileSize",FileSize),
                new SqlParameter("@IsDelete",1),
                new SqlParameter("@Spare1",null),
                new SqlParameter("@Spare2",null),
                new SqlParameter("@Spare3",null)
            };
                Id = commBll.AddIdentity("ykt_CourseManagement", list, value, para).ToString();
            }
            else
            {
                DataTable data = commBll.GetListDatatable("*", "ykt_CourseManagement", " and Id=" + Id);
                if (url == "")
                {
                    url = data.Rows[0]["CourseUrl"].ToString();
                    filePath = Server.MapPath(url);
                }
                if (url1 == "")
                {
                    url1 = data.Rows[0]["CourseCoverUrl"].ToString();
                }
                if (fileType == "")
                {
                    fileType = data.Rows[0]["Format"].ToString();
                }
                if (FileSize == 0)
                {
                    FileSize = Convert.ToDouble(data.Rows[0]["FileSize"]);
                }
                string Ulist = @"[CourseName] = @CourseName,
                                 [UploadName] = @UploadName,
                                 [UploadTime] = @UploadTime,
                                 [CourseUrl] = @CourseUrl,
                                 [CourseCoverUrl] = @CourseCoverUrl,
                                 [Format] = @Format,
                                 [Jurisdiction] =@Jurisdiction,
                                 [FileSize] = @FileSize";
                SqlParameter[] para = new SqlParameter[]{
                new SqlParameter("@CourseName",CourseName),
                new SqlParameter("@UploadName",UId),
                new SqlParameter("@UploadTime",DateTime.Now.ToString()),
                new SqlParameter("@CourseUrl",url),
                new SqlParameter("@CourseCoverUrl",url1),
                new SqlParameter("@Format",fileType),
                new SqlParameter("@Jurisdiction",use),
                new SqlParameter("@FileSize",FileSize),
                };
                commBll.UpdateInfo("ykt_CourseManagement", Ulist, " and Id=" + Id, para);
            }

            if (fileType == "docx" || fileType == "doc")
            {
                //进入线程执行转换图片的方法
                UploaderCalculators.Add(filePath, fileType, Id.ToString(), UId.ToString(), Server.MapPath(imgUrl), imgUrl);
            }
            if (imgName != null && imgName != "" && imgName != "888")
            {
                string listtwo = @"[CourseId],[UserId],[PictureUrl],[Spare1],[Spare2],[Spare3]";
                string vlauetwo = "@CourseId,@UserId,@PictureUrl,@Spare1,@Spare2,@Spare3";
                SqlParameter[] parstwo = new SqlParameter[] 
                                                        {
                                                            new SqlParameter("@CourseId",Id),
                                                            new SqlParameter("@UserId",UId),
                                                            new SqlParameter("@PictureUrl",imgName),
                                                            new SqlParameter("@Spare1",null),
                                                            new SqlParameter("@Spare2",null),
                                                            new SqlParameter("@Spare3",null)
                                                        };
                Convert.ToInt32(commBll.AddIdentity("ykt_PptTransferMap", listtwo, vlauetwo, parstwo));
            }
            return Id.ToString();
        }

        /// <summary>
        /// 课程列表
        /// </summary>
        /// <returns></returns>
        public JsonResult CourseDataBind()
        {
            string where = string.Empty;
            string Jurisdiction = string.Empty;
            StringBuilder sb = new StringBuilder();
            string Name = Request["Name"];
            if (Name != null && Name != "")
            {
                where = " and CourseName like '%" + Name + "%'";
            }
            if (UserType == "2")
            {
                where += " and UploadName=" + UId;
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "Id desc"; //排序必须填写
            m.strFld = "(select UserNo from tb_UserInfo where UId=UploadName) as UserNo ,* ";
            m.tab = "ykt_CourseManagement";
            m.strWhere = " and IsDelete=1" + where;
            int PageCount = 0;//总数
            var data = Pager.GetList(m, ref PageCount);
            for (int i = 0; i < data.Rows.Count; i++)
            {
                string CourseName = data.Rows[i]["CourseName"].ToString();
                if (CourseName.Length >= 15)
                {
                    CourseName = CourseName.Substring(0, 14) + "......";
                }
                if (Convert.ToInt32(data.Rows[i]["Jurisdiction"]) == 1)
                {
                    Jurisdiction = "启用";
                }
                else
                {
                    Jurisdiction = "禁用";
                }
                var idx = 0;
                if (Request["page"] != "undefined" && Request["page"] != null)
                {
                    idx = Convert.ToInt32(Request["page"]);
                    idx = idx - 1;
                }
                sb.Append(@"<tr>
                        <td>" + ((idx * Convert.ToInt32(Request["PageSize"])) + i + 1) + @"</td>
                        <td><input type='checkbox' name='check' class='i-checks' value='" + data.Rows[i]["Id"] + @"'></td>
                        <td title='" + data.Rows[i]["CourseName"] + "'>" + CourseName + @"</td>
                        <td>" + data.Rows[i]["UserNo"] + @"</td>
                        <td>" + data.Rows[i]["UploadTime"] + @"</td>
                        <td>" + Jurisdiction + @"</td>
                        <td><button onclick='Edit(" + data.Rows[i]["Id"] + ")' type='button' class='btn btn-primary btn-sm'><span class='bold'>编辑</span></button></td></tr>");
            }
            return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, sb.ToString());
        }
        /// <summary>
        /// 课程启用and禁用
        /// </summary>
        /// <returns></returns>
        public string CourseEnable()
        {
            string Id = Request["Id"];
            string Jurisdiction = Request["Jurisdiction"];
            int Count = commBll.GetRecordCount("ykt_CourseManagement", " and Jurisdiction=" + Jurisdiction + " and Id in(" + Id + ")");
            if (Count > 0)
            {
                return JsonConvert.SerializeObject("999");
            }
            SqlParameter[] para = new SqlParameter[] { }; ;
            return JsonConvert.SerializeObject(commBll.UpdateInfo("ykt_CourseManagement", "Jurisdiction=" + Jurisdiction + "", " and Id in(" + Id + ")", para));
        }

        /// <summary>
        /// 课程删除
        /// </summary>
        /// <returns></returns>
        public string IsDelete()
        {
            string Id = Request["Id"];
            SqlParameter[] para = new SqlParameter[] { }; ;
            return JsonConvert.SerializeObject(commBll.UpdateInfo("ykt_CourseManagement", "IsDelete=0", " and Id in(" + Id + ")", para));
        }

        /// <summary>
        /// 资源路径转换
        /// </summary>
        /// <param name="resname">文件名称</param>
        /// <returns>返回路径</returns>
        public string ResourceConversionURL(string resname)
        {
            //文件夹名称
            string wjjname = "";
            //后缀
            string hz = resname.Split('.')[1];
            hz = hz.ToLower();//转小写
            if (hz == "doc" || hz == "docx")
            {
                wjjname = "word";//文件夹名称
            }
            if (hz == "xls" || hz == "xlsx")
            {
                wjjname = "excel";
            }
            if (hz == "ppt" || hz == "pptx")
            {
                wjjname = "ppt";
            }
            //图片
            if (hz == "gif" || hz == "bmp" || hz == "jpg" || hz == "jpeg"
                || hz == "tif" || hz == "png" || hz == "pcx")
            {
                wjjname = "img";
            }
            if (hz == "txt")
            {
                wjjname = "txt";
            }
            if (hz == "pdf")
            {
                wjjname = "pdf";
            }
            //视频
            if (hz == "swf" || hz == "mp3" || hz == "mp4" || hz == "rm"
                || hz == "rmvb" || hz == "wmv" || hz == "avi"
                 || hz == "3gp" || hz == "mkv" || hz == "flv")
            {
                wjjname = "video";
            }
            return "/Resources/" + wjjname + "/" + resname;
        }
    }
}
