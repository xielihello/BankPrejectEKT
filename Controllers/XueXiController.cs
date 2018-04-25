using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocationalProject_Bll;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using VocationalProject_Models;
using System.IO;
using VocationalProject_DBUtility;
using System.Data.SqlClient;
using VocationalProject.Areas.Admin.Models;
using VocationalProject_Dal;
using VocationalProject.Controllers;
using VocationalProject.Models;
using System.Web.SessionState;
using System.Collections;
using VocationalProject_DBUtility.Sql;

namespace VocationalProject.Controllers
{
    /***************************************************************
   FileName:学习中心
   Copyright（c）2017-金融教育在线技术开发部
   Author:袁学
   Create Date:2017-8-29
   ******************************************************************/

    public class XueXiController : BaseController, IRequiresSessionState
    {
        //
        // GET: /XueXi/
        CommonBll commonbll = new CommonBll();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Browse()
        {
            return View();
        }

        public ActionResult Pay(int? kcId)
        {
            var m = new CourseManagementModel();
            DataTable data = commonbll.GetListDatatable("*", "ykt_CourseManagement", " and Id=" + kcId);
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

        public string GetList()
        {

            string wheres = " and Jurisdiction=1 and IsDelete=1";//启用and 未删除
            //我属于的班级 有哪几个教师Id
            var teacherId = commonbll.GetListSclar("TeacherId", "tb_Team", " and Id="+UserClassId);
            var useradminuid = commonbll.GetListSclar("UId", "tb_UserInfo", " and UserNo='admin' and UserType=1");//管理员
            if (teacherId.Trim().Length > 0)
            {
                wheres += " and (UploadName in (" + teacherId + ") or UploadName=" + useradminuid + ")";
            }
            else {

                wheres += " and UploadName=" + useradminuid;
            }
            if (Request["Read"] != null && Request["Read"].ToString() != "0")//是否学习
            {
                if (Request["Read"].ToString() == "1")//已学习
                {
                    wheres += " and Id in (select CourseId from ykt_CoureselRead where UserId=" + UId + ")";
                }
                if (Request["Read"].ToString() == "2")//未学习
                {
                    wheres += " and Id not in (select CourseId from ykt_CoureselRead where UserId=" + UId + ")";
                }
            }

            //课件名称
            if (Request["CourseName"] != null && Request["CourseName"].ToString().Length > 0)
            {
                wheres += " and CourseName like '%" + Request["CourseName"] + "%'";
            }


            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "Id desc"; //排序必须填写
            m.strFld = "(select COUNT(*) from ykt_CoureselRead where UserId=" + UId + " and CourseId=ykt_CourseManagement.Id) as RowsCount,* ";
            m.tab = "ykt_CourseManagement";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));

        }

        public string Read()
        {
            //校验是否已存在
            var count = commonbll.GetRecordCount("ykt_CoureselRead", " and CourseId=" + Request["kcId"] + " and UserId=" + UId);
            if (count == 0)
            {
                SqlHelper.ExecuteNonQuery("insert into ykt_CoureselRead(CourseId,UserId,AddTime) values('" + Request["kcId"] + "','" + UId + "','" + DateTime.Now + "')");
            }
            return "1";
        }
        /// <summary>
        /// 浏览
        /// </summary>
        /// <returns></returns>
        public string DataBind()
        {
            StringBuilder sb = new StringBuilder();
            string kcId = Request["kcId"];
            DataTable data = commonbll.GetListDatatable("PictureUrl", "ykt_PptTransferMap", " and CourseId=" + kcId);
            //if (data.Rows.Count > 0)
            //{
            //    for (int i = 0; i < data.Rows[0]["PictureUrl"].ToString().Split('|').Length; i++)
            //    {
            //        sb.Append("<img src='" + data.Rows[0]["PictureUrl"].ToString().Split('|')[i] + "' />");
            //    }
            //}
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// 查询课程名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string CourseName()
        {
            string Id = Request["kcId"];
            return JsonConvert.SerializeObject(commonbll.GetListSclar("CourseName", "ykt_CourseManagement", " and Id=" + Id));
        }

        /// <summary>
        /// 记录当前播放进度
        /// </summary>
        /// <returns></returns>
        public void TimeInterval()
        {
            string CourseId = Request["CourseId"];
            string CurrentTime = Request["CurrentTime"];
            //已有进度
            int Ct = commonbll.GetRecordCount("ykt_PlaybackProgress", " and CourseId=" + CourseId + " and UserId=" + UId);
            if (Ct > 0)
            {
                //更新进度
                string value = "PlaybackProgress=@PlaybackProgress";
                SqlParameter[] para = new SqlParameter[]{
                    new SqlParameter("@PlaybackProgress",CurrentTime),
                };
                commonbll.UpdateInfo("ykt_PlaybackProgress", value, " and CourseId=" + CourseId + " and UserId=" + UId + "", para);
            }
            else
            {
                //插入进度
                string list = "[PlaybackProgress],[CourseId],[UserId],[Spare1],[Spare2],[Spare3]";
                string value = "@PlaybackProgress,@CourseId,@UserId,@Spare1,@Spare2,@Spare3";
                SqlParameter[] para = new SqlParameter[]{
                    new SqlParameter("@PlaybackProgress",CurrentTime),
                    new SqlParameter("@CourseId",CourseId),
                    new SqlParameter("@UserId",UId),
                    new SqlParameter("@Spare1",null),
                    new SqlParameter("@Spare2",null),
                    new SqlParameter("@Spare3",null),
                };
                commonbll.Add("ykt_PlaybackProgress", list, value, para);
            }
        }

        /// <summary>
        /// 读取播放进度
        /// </summary>
        /// <returns></returns>
        public string TimeIntervalSelect()
        {
            string CourseId = Request["CourseId"];
            return commonbll.GetListSclar("PlaybackProgress", "ykt_PlaybackProgress", " and CourseId=" + CourseId + " and UserId=" + UId + "");
        }
    }
}
