using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocationalProject.Controllers;
using VocationalProject_Bll;

namespace VocationalProject.Areas.Admin.Controllers
{
    public class PersonalsettingController : BaseController
    {
        /***************************************************************
        FileName:管理员信息修改
        Copyright（c）2017-金融教育在线技术开发部
        Author:柯思金
        Create Date:2017-4-10
        ******************************************************************/
        // GET: /Admin/Personalsetting/
        CommonBll commBll = new CommonBll();
        //string UId = "1";        //用户ID
        //string UserType = "2";   //1.管理员 2.教师 3.学生 4.裁判
        //string UserNo = "T00001"; //登录帐号
        public ActionResult Index()
        {
            ViewData["UserType"] = UserType;

           

            return View();
        }
        /// <summary>
        /// 查询个人资料
        /// </summary>
        /// <returns></returns>
        public string loadinfo()
        {
            if (UserType == "1")
            {
                DataTable dt = commBll.GetListDatatable("UserNo,UserPwd,UserName,UserPic", "tb_UserInfo", " and UserNo='" + UserNo + "'");
                return JsonConvert.SerializeObject(dt);
            }
            else 
            {
                DataTable dt = commBll.GetListDatatable("*", "tb_UserInfo", " and UserNo='" + UserNo + "'");
                return JsonConvert.SerializeObject(dt);
            }
        }
        /// <summary>
        /// 教师修改信息
        /// </summary>
        /// <returns></returns>
        public string updateinfo1() 
        {
            if (Request["data"] != "1")
            {
                var m = Base_UserInfo;
                m.UserPic = Request["data"];
                Session["UserInfo"] = m;
                
            }
            if (Request["name"].Length > 0)
            {
                var m = Base_UserInfo;
                m.UserName = Request["name"];
                Session["UserInfo"] = m;
            }
            return "aa";
        }
        /// <summary>
        /// 管理员修改信息
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
            if (Request["name"].Length > 0)
            {
                var m = Base_UserInfo;
                m.UserName = Request["name"];
                Session["UserInfo"] = m;
            }
            return "aa";
        }
        /// <summary>
        /// 公共修改密码
        /// </summary>
        /// <returns></returns>
        public string UpdateP()
        {
            return "aa";
        }
    }
}
