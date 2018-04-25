using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocationalProject_Bll;

namespace VocationalProject.Controllers
{
    public class StudentController : BaseController
    {
        /***************************************************************
       FileName:管理员信息修改
       Copyright（c）2017-金融教育在线技术开发部
       Author:柯思金
       Create Date:2017-4-15
       ******************************************************************/
        // GET: /Admin/Personalsetting/
        CommonBll commonbll = new CommonBll();
        //string UId = "1";        //用户ID
        //string UserType = "2";   //1.管理员 2.教师 3.学生 4.裁判
        //string UserNo = "T00001"; //登录帐号
        public ActionResult Index()
        {
            var m = Base_UserInfo;

            DataTable checkcount = commonbll.GetListDatatable("*", "tb_UserInfo,tb_Team,tb_School"
          , " and tb_School.Id=tb_UserInfo.UserSchoolId and tb_UserInfo.UserClassId=tb_Team.Id and tb_UserInfo.UId=" + m.UId);
            if (checkcount != null && checkcount.Rows.Count > 0)
            {
                ViewData["AccountNo"] = checkcount.Rows[0]["UserNo"];
                ViewData["AccountPwd"] = checkcount.Rows[0]["UserPwd"];
                ViewData["Name"] = checkcount.Rows[0]["UserName"];
                ViewData["Sex"] = checkcount.Rows[0]["UserSex"];
                ViewData["BirthDate"] = Convert.ToDateTime(checkcount.Rows[0]["Custom1"]).ToString("yyyy-MM-dd HH:mm");
                ViewData["Age"] = checkcount.Rows[0]["UserYear"];
                ViewData["JoinBankDate"] = Convert.ToDateTime(checkcount.Rows[0]["AddTime"]).ToString("yyyy-MM-dd HH:mm");
                

                ViewData["UCode"] = checkcount.Rows[0]["StudentNo"];
                ViewData["Education"] = checkcount.Rows[0]["Custom2"];
                ViewData["Phone"] = checkcount.Rows[0]["UserPhone"];
                ViewData["TelPhone"] = checkcount.Rows[0]["Custom3"];
                ViewData["Email"] = checkcount.Rows[0]["UserEmail"];
                ViewData["PictureUrl"] = checkcount.Rows[0]["UserPic"];


                ViewData["SubBankId"]= checkcount.Rows[0]["Name"].ToString();
                ViewData["PositionId"] = checkcount.Rows[0]["TeamName"].ToString();

            }
            return View();
        }
        /// <summary>
        /// 初始化学生信息
        /// </summary>
        /// <returns></returns>
         public string loadinfo() 
        {
            DataTable dt = commonbll.GetListDatatable("*", "tb_UserInfo", " and UserNo='" + UserNo + "'");
            return JsonConvert.SerializeObject(dt);
        }
         
         public string SaveUpdate() 
         {
             string diyImg = Request["diyImg"];
             string diyImgS = Request["diyImgS"];
             var UserId = Base_UserInfo.UId;
             var Name = Request["Name"];//姓名
             var Sex = Request["Sex"];//性别
             var AccountPwd = Request["pwd"];//登录密码 
             var BirthDate = Request["BirthDate"];//出生年月
             //个人信息
             var Education = Request["Education"];//学历
             var Phone = Request["Phone"];//手机号码
             var TelPhone = Request["TelPhone"];//办公电话
             var Email = Request["Email"];//电子邮件

             string set1 = @"UserName=@Name,
                            UserSex=@Sex,
                            UserPwd=@AccountPwd,
                            Custom1=@BirthDate,
                            Custom2=@Education,
                            UserPhone=@Phone,
                            Custom3=@TelPhone,
                            UserEmail=@Email
";
             SqlParameter[] pars1 = new SqlParameter[] 
                {
                    new SqlParameter("@Id",UserId),
                    new SqlParameter("@Name",Name),
                    new SqlParameter("@Sex",Sex),
                    new SqlParameter("@AccountPwd",AccountPwd),
                    new SqlParameter("@BirthDate",BirthDate),
                    new SqlParameter("@Education",Education),
                    new SqlParameter("@Phone",Phone),
                    new SqlParameter("@TelPhone",TelPhone),
                    new SqlParameter("@Email",Email)
                };
             var resultcount = commonbll.UpdateInfo("tb_UserInfo", set1, " and UId=@Id", pars1);
             var File_Send = "";//头像
             if (diyImg == "1" && diyImgS=="1")//有修改图片
             {
                 HttpPostedFileBase hpFile = Request.Files["upload-file"];
                 string filePath = "/img";
                 //获得上传上来的文件名称  
                 string fileName = System.IO.Path.GetFileName(hpFile.FileName);
                 File_Send = "/img/" + fileName;

                 filePath = Server.MapPath(filePath + "/" + fileName);
                 //将上传来的 文件数据 保存在 对应的 物理路径上  
                 hpFile.SaveAs(filePath);
                 string set = "UserPic=@PictureUrl";
                 SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@Id",UserId),
                    new SqlParameter("@PictureUrl",File_Send),
                };
                 var resultcount1 = commonbll.UpdateInfo("tb_UserInfo", set, " and UId=@Id", pars);
                 var m = Base_UserInfo;
                 m.UserPic = File_Send;

                 Session["UserInfo"] = m;
             }
             return resultcount.ToString();
         }
         /// <summary>
         /// session学生信息修改信息
         /// </summary>
         /// <returns></returns>
         public string updateinfo()
         {
             if (Request["data"] != "1")
             {
                 var m = Base_UserInfo;
                 m.UserPic = Request["data"];
               
                 Session["UserInfo"] = m;

             }
             if (Request["data1"] != "")
             {
                 var m = Base_UserInfo;
                 m.UserName = Request["data1"];

                 Session["UserInfo"] = m;
             }
             return "aa";
         }
    }
}
