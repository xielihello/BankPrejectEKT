using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VocationalProject.Areas.Admin.Models;
using VocationalProject.Controllers;
using VocationalProject_Bll;
using VocationalProject_Dal;
using VocationalProject_DBUtility;
using VocationalProject_DBUtility.Sql;
using VocationalProject_Models;

namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
      FileName:教师管理
      Copyright（c）2017-金融教育在线技术开发部
      Author:唐
      Create Date:2017-3-30
     ******************************************************************/
    public class TeacherManagementController : BaseController
    {
        //
        // GET: /Admin/TeacherManagement/

        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判

        CommonBll commonbll = new CommonBll();
        UserInfoBll userinfobll = new UserInfoBll();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 读取教师列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStudentList()
        {

            string wheres = "  and UserType=2 and State<>0 ";//角色2 状态1 and State=1  
            if (!string.IsNullOrEmpty(Request["SchooName"]))//学院下拉框
            {
                wheres += " and c.Name like '%" + Request["SchooName"].ToString() + "%'";
            }

            //教师信息 账号 姓名 学号 查询
            if (!string.IsNullOrEmpty(Request["TeacherName"]))
            {
                wheres += " and ( a.UserNo like '%" + Request["TeacherName"] + "%' or a.UserName like '%" + Request["TeacherName"] + "%' or a.StudentNo like '%" + Request["TeacherName"] + "%') ";
            }
            //DataTable dt = commonbll.GetListDatatable(" a.*,[Name] as SchoolName",
            //    @"tb_UserInfo a left join  tb_School c on a.UserSchoolId=c.Id", wheres);


            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "Uid desc"; //排序必须填写
            m.strFld = " a.*,[Name] as SchoolName ";
            m.tab = "tb_UserInfo a left join  tb_School c on a.UserSchoolId=c.Id";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);


            StringBuilder sb = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    sb.Append("<tr>");
                    var idx = 0;
                    if (Request["page"] != "undefined" && Request["page"] != null)
                    {
                        idx = Convert.ToInt32(Request["page"]);
                        idx = idx - 1;
                    }


                    sb.Append("<td>" + ((idx * Convert.ToInt32(Request["PageSize"])) + i + 1) + "</td>");

                    sb.Append("<td>");
                    sb.Append("<input type=\"checkbox\" value=\"" + dt.Rows[i]["UId"] + "\"  class=\"i-checks\" name=\"input[]\"/>");
                    //院校
                    sb.Append("</td>");

                    //教师账号
                    sb.Append("<td>");
                    sb.Append(dt.Rows[i]["SchoolName"]);
                    //教师账号
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserNo"]);
                    //登录密码
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserPwd"]);
                    //教师姓名
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserName"]);
                    //性别
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserSex"]);
                    //移动电话
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserPhone"]);
                    //个人邮箱
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserEmail"]);
                    //操作状态
                    sb.Append("</td><td>");
                    sb.Append("<select class=\"inline\"  onchange=\"EdtiState(" + dt.Rows[i]["UId"] + ",this.value)\">");
                    if (dt.Rows[i]["State"].ToString() == "1")
                    {
                        sb.Append("<option value=\"1\" selected=\"selected\">可操作</option>");
                        sb.Append("<option value=\"2\">禁用</option>");
                    }
                    else
                    {
                        sb.Append("<option value=\"1\">可操作</option>");
                        sb.Append("<option value=\"2\" selected=\"selected\">禁用</option>");
                    }
                    sb.Append("</select>");
                    //操作
                    sb.Append("</td><td>");
                    sb.Append("<a href=\"javascript:void(0);\" onclick=\"EditStudent('" + dt.Rows[i]["UId"] + "')\" class=\" btn-primary btn-sm\"><i class=\"fa fa-pencil m-r-xxs\"></i>编辑 </a>");

                    sb.Append("</td></tr>");

                }
            }
            return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, sb.ToString());
            //return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, sb.ToString());
        }
        /// <summary>
        /// 读取单行信息
        /// </summary>
        /// <returns></returns>
        public string GetStudentById()
        {
            DataTable dt = commonbll.GetListDatatable(" a.*,[Name] as SchoolName,c.Id as SchoolId",
              @"tb_UserInfo a 
                 inner join  tb_School c on a.UserSchoolId=c.Id ", " and UId=" + Request["UId"]);

            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public string Edit()
        {

            var StuUId = Request["UId"];
            var EditSchoolId = Request["EditSchoolId"];
            var EditName = Request["EditName"];
            var EditPwd = Request["EditPwd"];
            var EditPhone = Request["EditPhone"];
            var EditEmail = Request["EditEmail"];
            var EditSex = Request["EditSex"];

            //先查询出之前的教师名称
            var allname = commonbll.GetListSclar("UserName", "tb_UserInfo", " and UId=" + StuUId);

            string set = "UserSchoolId=@UserSchoolId,UserName=@UserName,UserPwd=@UserPwd,UserSex=@UserSex,UserEmail=@UserEmail,UserPhone=@UserPhone,Operator=@Operator";
            SqlParameter[] pars = new SqlParameter[] 
            {
                new SqlParameter("@UserSchoolId",EditSchoolId), 
                new SqlParameter("@UserName",EditName),
                new SqlParameter("@UserPwd",EditPwd),
                new SqlParameter("@UserSex",EditSex),
                new SqlParameter("@UserPhone",EditPhone),
                new SqlParameter("@UserEmail",EditEmail),
                new SqlParameter("@Operator",UId),
                new SqlParameter("@UId",StuUId)
            };
            var count = commonbll.UpdateInfo("tb_UserInfo", set, " and UId=@UId", pars);

            //同步更新班级表的名称
            //查询现有的存在的
            DataTable dt = commonbll.GetListDatatable("*", "tb_Team", " and ([TeacherId] like '%" + StuUId + ",%' or [TeacherId] like '%," + StuUId + "%' or [TeacherId]='" + StuUId + "')");
            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var str = dt.Rows[i]["Custom2"].ToString();
                    var Id = dt.Rows[i]["Id"].ToString();
                    var newname = str.Replace(allname, EditName);
                    SqlHelper.ExecuteNonQuery("update tb_Team set Custom2='" + newname + "' where Id=" + Id);
                }
            }

            return count.ToString();

        }
        /// <summary>
        ///读取学院信息
        /// </summary>
        /// <returns></returns>
        public string GetSchool()
        {
            DataTable dt = commonbll.GetListDatatable("Id,Name", "tb_School", "");
            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 读取班级信息
        /// </summary>
        /// <returns></returns>
        public string GetTeam()
        {
            string wheres = "";
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and SchoolId='" + Request["SchooId"].ToString() + "'";
            }

            DataTable dt = commonbll.GetListDatatable("Id,TeamName", "tb_Team", wheres);
            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 删除教师
        /// </summary>
        /// <returns></returns>
        public string DelStudent()
        {
            try
            {
                var Ids = Request["Ids"];
                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@Operator",UId),
                    new SqlParameter("@State","0")
                };
                //修改状态 操作人
                commonbll.UpdateInfo("tb_UserInfo", " State=@State,Operator=@Operator", " and UId in(" + Ids + ")", pars);

                var idsstr = Ids.Split(',');
                for (var i = 0; i < idsstr.Length; i++)
                {
                    if (idsstr[i].Length > 0)
                    {
                        PulTeacherClass(idsstr[i]);
                    }
                }

                return "1";
            }
            catch
            {
                return "99";
            }

        }


        /// <summary>
        /// 初始化教师密码
        /// </summary>
        /// <returns></returns>
        public string InitializationStudentPwd()
        {
            try
            {
                var Ids = Request["Ids"];

                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@Operator",UId),
                    new SqlParameter("@UserPwd","dysoft6666")
                };
                //修改状态 操作人
                commonbll.UpdateInfo("tb_UserInfo", " UserPwd=@UserPwd,Operator=@Operator", " and UId in(" + Ids + ")", pars);

                return "1";
            }
            catch
            {
                return "99";
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public string Add()
        {
            var AddSchoolId = Request["AddSchoolId"];
            var AddTeacherName = Request["AddTeacherName"];
            var AddPhone = Request["AddPhone"];
            var AddEmail = Request["AddEmail"];
            var AddSex = Request["AddSex"];

            if (AddPhone.Length > 0)
            {
                //验证手机号是否存在
                var checkcount = commonbll.GetRecordCount("tb_UserInfo", " and State=1 and UserType=2 and UserPhone='" + AddPhone + "'");
                if (checkcount > 0)
                {
                    return "88";
                }
            }


            #region 教师账号生成规则
            string Teller_No = "";
            DataTable dt = commonbll.GetListDatatable("UserNo", "tb_UserInfo", " and UserType=2  order by UserNo desc");
            if (dt.Rows.Count > 0)
            {
                string sp = dt.Rows[0]["UserNo"].ToString();
                if (Convert.ToInt32(sp.Substring(1)) < 9)
                {
                    Teller_No = "T0000" + (Convert.ToInt32(sp.Substring(1)) + 1);
                }
                if (Convert.ToInt32(sp.Substring(1)) > 8 && Convert.ToInt32(sp.Substring(1)) < 99)
                {
                    Teller_No = "T000" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
                if (Convert.ToInt32(sp.Substring(1)) > 98 && Convert.ToInt32(sp.Substring(1)) < 999)
                {
                    Teller_No = "T00" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
                if (Convert.ToInt32(sp.Substring(1)) > 998 && Convert.ToInt32(sp.Substring(1)) < 9999)
                {
                    Teller_No = "T0" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }

                if (Convert.ToInt32(sp.Substring(1)) > 9998)
                {
                    Teller_No = "T" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
            }
            else
            {
                Teller_No = "T00001";
            }
            #endregion

            tb_UserInfo userinfo = new tb_UserInfo();
            userinfo.UserNo = Teller_No;
            userinfo.UserPwd = "dysoft6666";
            userinfo.UserName = AddTeacherName;
            userinfo.UserType = 2;
            userinfo.UserSchoolId = int.Parse(AddSchoolId);
            userinfo.UserClassId = 0;// int.Parse(AddTeamId);
            userinfo.StudentNo = null;//AddStudentNo;
            userinfo.State = 1;
            userinfo.UserEmail = AddEmail;
            userinfo.UserPhone = AddPhone;
            userinfo.UserSex = AddSex;
            userinfo.Operator = UId;
            userinfo.AddOperator = UId;
            userinfo.AddTime = DateTime.Now;

            var count = userinfobll.Add(userinfo);
            return count.ToString();
        }


        /// <summary>
        /// 批量新增
        /// </summary>
        /// <returns></returns>
        public string BatchAdd()
        {
            var BatchAddSchoolId = Request["BatchAddSchoolId"];
            var BatchAddNameNum = Request["BatchAddNameNum"];
            var num = 0;
            for (var i = 0; i < int.Parse(BatchAddNameNum); i++)
            {

                #region 教师账号生成规则
                string UserNo = "";
                DataTable dt = commonbll.GetListDatatable("UserNo", "tb_UserInfo", " and UserType=2  order by UserNo desc");
                if (dt.Rows.Count > 0)
                {
                    string sp = dt.Rows[0]["UserNo"].ToString();
                    if (Convert.ToInt32(sp.Substring(1)) < 9)
                    {
                        UserNo = "T0000" + (Convert.ToInt32(sp.Substring(1)) + 1);
                    }
                    if (Convert.ToInt32(sp.Substring(1)) > 8 && Convert.ToInt32(sp.Substring(1)) < 99)
                    {
                        UserNo = "T000" + (Convert.ToInt32(sp.Substring(1)) + 1);

                    }
                    if (Convert.ToInt32(sp.Substring(1)) > 98 && Convert.ToInt32(sp.Substring(1)) < 999)
                    {
                        UserNo = "T00" + (Convert.ToInt32(sp.Substring(1)) + 1);

                    }
                    if (Convert.ToInt32(sp.Substring(1)) > 998 && Convert.ToInt32(sp.Substring(1)) < 9999)
                    {
                        UserNo = "T0" + (Convert.ToInt32(sp.Substring(1)) + 1);

                    }

                    if (Convert.ToInt32(sp.Substring(1)) > 9998)
                    {
                        UserNo = "T" + (Convert.ToInt32(sp.Substring(1)) + 1);

                    }
                }
                else
                {
                    UserNo = "T00001";
                }
                #endregion
                tb_UserInfo userinfo = new tb_UserInfo();
                userinfo.UserNo = UserNo;
                userinfo.UserSchoolId = int.Parse(BatchAddSchoolId);
                userinfo.UserPwd = "dysoft6666";
                userinfo.UserType = 2;
                userinfo.State = 1;
                userinfo.Operator = UId;
                userinfo.AddOperator = UId;
                userinfo.AddTime = DateTime.Now;
                var count = userinfobll.Add(userinfo);
                num++;
            }

            return num.ToString();

        }
        /// <summary>
        /// 账户状态修改
        /// </summary>
        /// <returns></returns>
        public string EdtiState()
        {
            try
            {
                var StuUId = Request["UId"];

                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@Operator",UId),
                    new SqlParameter("@State",Request["value"]),
                    new SqlParameter("@UId",StuUId),
                };
                //修改状态 操作人
                commonbll.UpdateInfo("tb_UserInfo", " State=@State,Operator=@Operator", " and UId=@UId", pars);
                return "1";
            }
            catch
            {
                return "99";
            }
        }


        /// <summary>
        /// 导出
        /// </summary>
        public string Export()
        {
            string wheres = "  and UserType=2 and State<>0 ";//角色2 状态1 and State=1
            if (!string.IsNullOrEmpty(Request["SchooName"]))//学院下拉框
            {
                wheres += " and c.Name like '%" + Request["SchooName"].ToString() + "%'";
            }

            //教师信息 账号 姓名 学号 查询
            if (!string.IsNullOrEmpty(Request["TeacherName"]))
            {
                wheres += " and ( a.UserNo like '%" + Request["TeacherName"] + "%' or a.UserName like '%" + Request["TeacherName"] + "%' or a.StudentNo like '%" + Request["TeacherName"] + "%') ";
            }
            wheres += " order by UId desc";
            DataTable dt = commonbll.GetListDatatable(" [Name] as SchoolName,UserNo,UserPwd,UserName,UserSex,UserPhone,UserEmail,CASE State WHEN 1 THEN '可操作' ELSE '禁用' END",
                @"tb_UserInfo a left join  tb_School c on a.UserSchoolId=c.Id", wheres);


            //            DataTable dt = commonbll.GetListDatatable("[Name] as SchoolName, TeamName,StudentNo,UserName,UserNo,UserPwd,b.Custom2 as TeachaerName",
            //                @"tb_UserInfo a inner join tb_Team b on a.UserClassId=b.Id
            //                 inner join  tb_School c on c.Id=b.Id", wheres);

            string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "分行管理员信息";
            string filename = "/Export/" + ExcelName + ".xlsx";

            OfficeHelper officeHp = new OfficeHelper();
            var Result = officeHp.DtToExcel(dt, "分行管理员信息表", new string[] { "分行", "分行管理员账号", "登陆密码", "分行管理员姓名", "性别", "移动电话", "个人邮箱", "账户状态" }, "分行管理员信息", ExcelName);
            //if (Result.Equals("SUCCESS"))
            //{
            //    Response.Write(filename);
            //}
            //else { Response.Write(Result); }


            var json = new object[] {
                        new{
                            filename=filename,
                        }
                    };
            return JsonConvert.SerializeObject(json);
        }


        //公用方法，教师在编辑和删除的时候,替换掉名称
        public void PulTeacherClass(string teacherId)
        {
            string wheres = " and (TeacherId like '" + teacherId + ",%' or  TeacherId like '%," + teacherId + ",%' or TeacherId like '%," + teacherId + "' or TeacherId like '" + teacherId + "')";
            //查询出存在教师的班级
            DataTable dt = commonbll.GetListDatatable("Id,TeacherId", "tb_Team", wheres);
            //更新名称
            //首先获取
            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var teaid = dt.Rows[i]["TeacherId"].ToString();
                    var teaname = "";
                    var teastrid = "";
                    if (teaid.Length > 0)
                    {
                        DataTable mdt = commonbll.GetListDatatable("UId,UserName", "tb_UserInfo", " and UserType=2 and State<>0 and UId in (" + teaid + ")");
                        if (mdt.Rows.Count > 0)
                        {
                            for (var m = 0; m < mdt.Rows.Count; m++)
                            {
                                teaname += mdt.Rows[m]["UserName"] + ",";//拼接用户名
                                teastrid += mdt.Rows[m]["UId"] + ",";
                            }



                        }
                        //
                        if (teaname.Length > 0)
                        {
                            teaname = teaname.Substring(0, teaname.Length - 1);//去掉逗号
                            teastrid = teastrid.Substring(0, teastrid.Length - 1);
                        }


                        //更新
                        SqlHelper.ExecuteNonQuery("update tb_Team set Custom2='" + teaname + "',TeacherId='" + teastrid + "' where Id=" + dt.Rows[i]["Id"]);


                    }

                }

            }

        }



    }

}
