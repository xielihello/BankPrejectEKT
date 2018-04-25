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

namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
    FileName:学生管理
    Copyright（c）2017-金融教育在线技术开发部
    Author:袁学
    Create Date:2017-3-29
   ******************************************************************/
    public class StudentManagementController : BaseController
    {
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判

        CommonBll commonbll = new CommonBll();
        UserInfoBll userinfobll = new UserInfoBll();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 读取学生列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList()
        {
            string wheres = " and State in(1,2) and UserType=3";//角色3 状态1
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and b.SchoolId='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }

            if (Request["TeacherName"] != null && Request["TeacherName"].ToString().Length > 0)//教师名称
            {

                wheres += " and b.Custom2 like '%" + Request["TeacherName"].ToString() + "%'";
            }
            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                wheres += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "UId desc"; //排序必须填写
            m.strFld = " a.*,TeamName,[Name] as SchoolName,TeacherId,b.Custom2 as TeachaerName ";
            m.tab = "tb_UserInfo a inner join tb_Team b on a.UserClassId=b.Id inner join  tb_School c on c.Id=b.SchoolId";
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
                    sb.Append("<td><input type=\"checkbox\" value=\"" + dt.Rows[i]["UId"] + "\"  class=\"i-checks\" name=\"input[]\"/>");
                    //院校
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["SchoolName"]);
                    //班级名称
                    sb.Append("</td><td>");
                    var TeamName = dt.Rows[i]["TeamName"].ToString();
                    if (TeamName.Length > 20)
                    {
                        TeamName = TeamName.Substring(0, 18) + "...";
                    }

                    sb.Append("<span title='" + dt.Rows[i]["TeamName"].ToString() + "'>" + TeamName + "</span>");
           
                   
                    //学生编号
                    sb.Append("</td><td>");
                    var StudentNo = dt.Rows[i]["StudentNo"].ToString();
                    if (StudentNo.Length > 20)
                    {
                        StudentNo = StudentNo.Substring(0, 17) + "...";
                    }

                    sb.Append("<span title='" + dt.Rows[i]["StudentNo"].ToString() + "'>" + StudentNo + "</span>");
           

               
                    //学生姓名
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserName"]);
                    //学生账号
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserNo"]);
                    //登录密码
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserPwd"]);
                    //所属教师
                    sb.Append("</td><td>");
                    //查询教师
                    var TeachaerName = dt.Rows[i]["TeachaerName"].ToString();
                    if (TeachaerName.Length > 15)//长度过长...
                    {
                        TeachaerName = TeachaerName.Substring(0, 12) + "...";
                    }
                    sb.Append(TeachaerName);

                    //////////////////////20170331新加//////////////////////////////
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
                    ///////////////////////////////////////////////////
                    //操作
                    sb.Append("</td><td>");
                    sb.Append("<a href=\"javascript:void(0);\" onclick=\"EditStudent('" + dt.Rows[i]["UId"] + "')\" class=\" btn-primary btn-sm\"><i class=\"fa fa-pencil m-r-xxs\"></i>编辑 </a>");

                    sb.Append("</td></tr>");

                }
            }

            return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, sb.ToString());
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
        /// 删除学生
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
                return "1";
            }
            catch
            {
                return "99";
            }

        }


        /// <summary>
        /// 初始化学生密码
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
                    new SqlParameter("@UserPwd","dy8888")
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
            var AddTeamId = Request["AddTeamId"];
            var AddStudentNo = Request["AddStudentNo"];
            var AddStudentName = Request["AddStudentName"];

            //验证学号是否存在
            var checkcount = commonbll.GetRecordCount("tb_UserInfo", " and State=1 and UserType=3 and StudentNo='" + AddStudentNo + "' and UserSchoolId='" + AddSchoolId + "'");
            if (checkcount > 0)
            {
                return "88";
            }


            #region 学生账号生成规则
            string Teller_No = "";
            DataTable dt = commonbll.GetListDatatable("UserNo", "tb_UserInfo", " and UserType=3  order by UserNo desc");
            if (dt.Rows.Count > 0)
            {
                string sp = dt.Rows[0]["UserNo"].ToString();
                if (Convert.ToInt32(sp.Substring(1)) < 9)
                {
                    Teller_No = "s0000" + (Convert.ToInt32(sp.Substring(1)) + 1);
                }
                if (Convert.ToInt32(sp.Substring(1)) > 8 && Convert.ToInt32(sp.Substring(1)) < 99)
                {
                    Teller_No = "s000" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
                if (Convert.ToInt32(sp.Substring(1)) > 98 && Convert.ToInt32(sp.Substring(1)) < 999)
                {
                    Teller_No = "s00" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
                if (Convert.ToInt32(sp.Substring(1)) > 998 && Convert.ToInt32(sp.Substring(1)) < 9999)
                {
                    Teller_No = "s0" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }

                if (Convert.ToInt32(sp.Substring(1)) > 9998)
                {
                    Teller_No = "s" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
            }
            else
            {
                Teller_No = "s00001";
            }
            #endregion

            tb_UserInfo userinfo = new tb_UserInfo();
            userinfo.UserNo = Teller_No;
            userinfo.UserPwd = "dy8888";
            userinfo.UserName = AddStudentName;
            userinfo.UserType = 3;
            userinfo.UserSchoolId = int.Parse(AddSchoolId);
            userinfo.UserClassId = int.Parse(AddTeamId);
            userinfo.StudentNo = AddStudentNo;
            userinfo.State = 1;
            userinfo.Operator = UId;
            userinfo.AddOperator = UId;
            userinfo.AddTime = DateTime.Now;
            userinfo.Custom1 = DateTime.Now;
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
            var BatchAddTeamId = Request["BatchAddTeamId"];
            long BatchS = long.Parse(Request["BatchS"]);//开始编号
            long BatchE = long.Parse(Request["BatchE"]);//结束编号
            var BatchAddStudentNum = Request["BatchAddStudentNum"];
            var num = 0;
            for (var i = BatchS; i <= BatchE; i++)
            {

                //跳过编号存在的
                var checkcount = commonbll.GetRecordCount("tb_UserInfo", " and State=1 and UserType=3 and StudentNo='" + i + "' and UserSchoolId='" + BatchAddSchoolId + "'");
                if (checkcount == 0)
                {
                    #region 学生账号生成规则
                    string Teller_No = "";
                    DataTable dt = commonbll.GetListDatatable("UserNo", "tb_UserInfo", " and UserType=3  order by UserNo desc");
                    if (dt.Rows.Count > 0)
                    {
                        string sp = dt.Rows[0]["UserNo"].ToString();
                        if (Convert.ToInt32(sp.Substring(1)) < 9)
                        {
                            Teller_No = "s0000" + (Convert.ToInt32(sp.Substring(1)) + 1);
                        }
                        if (Convert.ToInt32(sp.Substring(1)) > 8 && Convert.ToInt32(sp.Substring(1)) < 99)
                        {
                            Teller_No = "s000" + (Convert.ToInt32(sp.Substring(1)) + 1);

                        }
                        if (Convert.ToInt32(sp.Substring(1)) > 98 && Convert.ToInt32(sp.Substring(1)) < 999)
                        {
                            Teller_No = "s00" + (Convert.ToInt32(sp.Substring(1)) + 1);

                        }
                        if (Convert.ToInt32(sp.Substring(1)) > 998 && Convert.ToInt32(sp.Substring(1)) < 9999)
                        {
                            Teller_No = "s0" + (Convert.ToInt32(sp.Substring(1)) + 1);

                        }

                        if (Convert.ToInt32(sp.Substring(1)) > 9998)
                        {
                            Teller_No = "s" + (Convert.ToInt32(sp.Substring(1)) + 1);

                        }
                    }
                    else
                    {
                        Teller_No = "s00001";
                    }
                    #endregion

                    tb_UserInfo userinfo = new tb_UserInfo();
                    userinfo.UserNo = Teller_No;
                    userinfo.UserPwd = "dy8888";
                    userinfo.UserType = 3;
                    userinfo.UserSchoolId = int.Parse(BatchAddSchoolId);
                    userinfo.UserClassId = int.Parse(BatchAddTeamId);
                    userinfo.StudentNo = i.ToString();
                    userinfo.State = 1;
                    userinfo.Operator = UId;
                    userinfo.AddOperator = UId;
                    userinfo.AddTime = DateTime.Now;
                    userinfo.Custom1 = DateTime.Now;
                    var count = userinfobll.Add(userinfo);
                    num++;
                }
            }

            return num.ToString();

        }


        /// <summary>
        /// 导出
        /// </summary>
        public string Export()
        {
            string wheres = " and State=1 and UserType=3";//角色3 状态1
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and b.SchoolId='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }

            if (Request["TeacherName"] != null && Request["TeacherName"].ToString().Length > 0)//教师名称
            {

                wheres += " and b.Custom2 like '%" + Request["TeacherName"].ToString() + "%'";
            }
            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                wheres += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }


            DataTable dt = commonbll.GetListDatatable("[Name] as SchoolName, TeamName,StudentNo,UserName,UserNo,UserPwd,b.Custom2 as TeachaerName",
                @"tb_UserInfo a inner join tb_Team b on a.UserClassId=b.Id
                 inner join  tb_School c on c.Id=b.SchoolId", wheres + " order by UId desc");

            string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "员工信息";
            string filename = "/Export/" + ExcelName + ".xlsx";

            OfficeHelper officeHp = new OfficeHelper();
            var Result = officeHp.DtToExcel(dt, "员工信息表", new string[] { "分行", "支行名称", "员工编号", "员工姓名", "员工账号", "登录密码", "所属管理员" }, "员工信息", ExcelName);

            var json = new object[] {
                        new{
                            filename=filename,
                        }
                    };
            return JsonConvert.SerializeObject(json);
        }


        /// <summary>
        /// 读取单行信息
        /// </summary>
        /// <returns></returns>
        public string GetStudentById()
        {
            DataTable dt = commonbll.GetListDatatable(" a.*,TeamName,[Name] as SchoolName,TeacherId,b.Custom2 as TeachaerName,c.Id as SchoolId",
              @"tb_UserInfo a inner join tb_Team b on a.UserClassId=b.Id
                 inner join  tb_School c on c.Id=b.SchoolId", " and UId=" + Request["UId"]);

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
            var EditTeamId = Request["EditTeamId"];
            var EditStudentName = Request["EditStudentName"];
            var EditStudenPwd = Request["EditStudenPwd"];

            string set = "UserSchoolId=@UserSchoolId,UserClassId=@UserClassId,UserName=@UserName,UserPwd=@UserPwd,Operator=@Operator";
            SqlParameter[] pars = new SqlParameter[] 
            {
                new SqlParameter("@UserSchoolId",EditSchoolId),
                new SqlParameter("@UserClassId",EditTeamId),
                new SqlParameter("@UserName",EditStudentName),
                new SqlParameter("@UserPwd",EditStudenPwd),
                new SqlParameter("@Operator",UId),
                new SqlParameter("@UId",StuUId)
            };
            var count = commonbll.UpdateInfo("tb_UserInfo", set, " and UId=@UId", pars);
            return count.ToString();

        }
    }
}
