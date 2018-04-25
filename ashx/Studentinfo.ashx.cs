using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using VocationalProject_Bll;
using VocationalProject_Models;

namespace VocationalProject.ashx
{
    /// <summary>
    /// Studentinfo 的摘要说明
    /// </summary>
    public class Studentinfo : IHttpHandler
    {
        tb_UserInfo userinfo = new tb_UserInfo();
        CommonBll commonbll = new CommonBll();
        public void ProcessRequest(HttpContext context)
        {
            var UserNo = context.Request["UID"];
            string action = context.Request["action"];
            string Uname = context.Request["name"];
            string Pwd = context.Request["Pwd"];
            string img = context.Request["img"];
            string diyImg = context.Request["diyImg"];
            string sex = context.Request["sex"];
            string phone = context.Request["phone"];
            string email = "";
            if (context.Request["email"] != null)
            {
                email = context.Request["email"];
            }
            string QQ = context.Request["QQ"];
            string WX = context.Request["WX"];
            string File_Send = "";
            try
            {
                switch (action)
                {
                    #region 学生信息   
                    //学生信息修改
                    case "updateinfo":
                        if (diyImg == "1")
                        {
                            File_Send = context.Request["ImgUrl"];
                            userinfo.UserPic = File_Send;
                            SqlParameter[] pars = new SqlParameter[]{};
                            commonbll.UpdateInfo("tb_UserInfo","UserPic='" + userinfo.UserPic + "'", " and UserNo='" + UserNo + "'", pars);
                            context.Response.Write(File_Send);
                        }
                        if (Uname.Length > 0)
                        {
                            userinfo.UserName = Uname;
                            SqlParameter[] pars = new SqlParameter[] { };
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserName='" + userinfo.UserName + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        if (Pwd.Length > 0)
                        {
                            userinfo.UserPwd = Pwd;
                            SqlParameter[] pars = new SqlParameter[] { };
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserPwd='" + userinfo.UserPwd + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        if (WX.Length > 0)
                        {
                            userinfo.UserWX = WX;
                            SqlParameter[] pars = new SqlParameter[] { };
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserWX='" + userinfo.UserWX + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        if (QQ.Length > 0)
                        {
                            userinfo.UserQQ = QQ;
                            SqlParameter[] pars = new SqlParameter[] { };
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserQQ='" + userinfo.UserQQ + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        if (phone.Length > 0)
                        {
                            userinfo.UserPhone = phone;
                            SqlParameter[] pars = new SqlParameter[] { };
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserPhone='" + userinfo.UserPhone + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        if (email.Length > 0)
                        {
                            userinfo.UserEmail = email;
                            SqlParameter[] pars = new SqlParameter[] { };
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserEmail='" + userinfo.UserEmail + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        if (sex.Length > 0)
                        {
                            userinfo.UserSex = sex;
                            SqlParameter[] pars = new SqlParameter[] { };
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserSex='" + userinfo.UserSex + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        break;
                    case "loadinfo":
                        //类型：全部订单、已支付、未支付           
                        DataTable dt = commonbll.GetListDatatable("UserNo,UserPwd,UserName,UserPic,(select top 1 Jurisdiction from tb_School where Id=UserSchoolId) as Jurisdiction", "tb_UserInfo", " and UserNo='" + UserNo + "'");
                        
                        context.Response.Write(JsonConvert.SerializeObject(dt));
                        break;
                    #endregion


                }

            }
            catch
            {
                return;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}