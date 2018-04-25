using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VocationalProject_Bll;
using VocationalProject_Models;

namespace VocationalProject.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        CommonBll commonbll = new CommonBll();
        public ActionResult Index()
        {
            ////返回记住账号密码
            //string username = "";
            //string IsBer = "";
            //string PassPwd = "";
            //HttpCookie cookies = Request.Cookies["UserPasswordsModel"];
            //if (cookies != null && cookies.HasKeys)
            //{
            //    Encoding utf8 = Encoding.UTF8;
            //    username = HttpUtility.UrlDecode(cookies["UserName"], utf8);
            //    IsBer = cookies["IsBer"];
            //    if (cookies["IsBer"] == "1")
            //    {
            //        PassPwd = cookies["PassWord"];
            //    }
            //}
            //ViewData["username"] = username;
            //ViewData["IsBer"] = IsBer;
            //ViewData["password"] = PassPwd;

            return View();
        }
        public string AjaxLogin(string LoginName, string UserPwd, string isSaveCookie)
        {
            string sql = " select * from tb_UserInfo where UserNo=@UserNo and State <>0";
            SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@UserNo",LoginName), 

                     };
            var tb = commonbll.GetListParaDatatable(sql, pars);

            if (tb == null || tb.Rows.Count <= 0)
            {
                return "-1#";//用户不存在
            }
            var m = tb.Rows[0];
            if (m["State"].ToString() == "0")
            {
                return "-2#";//账号已删除
            }
            if (m["State"].ToString() == "2")
            {
                return "-3#";//账号已冻结、或未启用
            }
            if (m["UserPwd"].ToString() != UserPwd)
            {
                return "-4#";//密码不正确  
            }
            string UserType = m["UserType"].ToString();
            //将用户登录信息保存到Cookie中3天，用来来做大裁判扣分用 
            if (Convert.ToInt32(m["UserType"].ToString()) == 3)
            {
                string Student = "StudentId";
                HttpCookie cookie = new HttpCookie(Student, m["UId"].ToString());
                //保存cookie到硬盘3天
                cookie.Expires = DateTime.Now.AddDays(3);
                //讲cookie加到响应的报文中
                Response.Cookies.Add(cookie);
            }
            Session["UserType"] = m["UserType"].ToString();////管理员 1 管理员、2 教师、3 学生、4 裁判
            var UserSchoolId = m["UserSchoolId"].ToString();
            if (string.IsNullOrEmpty(m["UserSchoolId"].ToString()))
            {
                UserSchoolId = "0";
            }
            var tbSchool = commonbll.GetListDatatable("*", "tb_School", " and Id=" + UserSchoolId);
            string Jurisdiction="";
            if (tbSchool != null && tbSchool.Rows.Count > 0)
            {
                Jurisdiction = tbSchool.Rows[0]["Jurisdiction"].ToString();
            }
            Session["Jurisdiction"] = Jurisdiction;////货币知识 1 、手工点钞 2 、复核报表 3 、单据录入 4
            #region 记录用户登录
            tb_UserInfo u = new tb_UserInfo();
            u.UId = Convert.ToInt32(m["UId"].ToString());
            u.UserNo = LoginName;
            u.UserPwd = UserPwd;
            u.UserName = m["UserName"].ToString();
            u.UserPic = m["UserPic"].ToString();
            u.UserSex = m["UserSex"].ToString();
            u.UserType = Convert.ToInt32(m["UserType"].ToString());
            u.UserSchoolId = Convert.ToInt32(!string.IsNullOrEmpty(m["UserSchoolId"].ToString()) ? m["UserSchoolId"].ToString() : "0");
            u.UserClassId = Convert.ToInt32(!string.IsNullOrEmpty(m["UserClassId"].ToString()) ? m["UserClassId"].ToString() : "0"); 
            Session["UserInfo"] = u;
            #endregion

            Session.Timeout = 120;

            //账号密码记住存入cookie
            HttpCookie usercookie = new HttpCookie("UserPasswordsModel");
            usercookie.Values.Add("UserName", HttpUtility.UrlEncode(LoginName));
            usercookie.Values.Add("PassWord", UserPwd);
            usercookie.Values.Add("IsBer", isSaveCookie);
            Response.AppendCookie(usercookie);

            return UserType + "#" + Jurisdiction;

        }

    }
}
