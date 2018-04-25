using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocationalProject.Areas.Admin.Models;
using VocationalProject_Dal;
using VocationalProject.Controllers;
using VocationalProject_Bll;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using VocationalProject_DBUtility.Sql;
using System.Collections;
using System.Text;
using VocationalProject_DBUtility;


namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
    FileName:货币知识 教师端学员管理
    Copyright（c）2017-金融教育在线技术开发部
    Author:袁学
    Create Date:2017-4-11
    ******************************************************************/

    public class T_StudentManageController : BaseController
    {
        //
        // GET: /Admin/StudentManage/
        //string UId = "2";//用户ID
        //string UserType = "2";//1管理员2教师3学生4裁判
        //string UserNo = "T00001";//登录账号
        //string TeacherSchoolId = "1";//教师属于的院校
        CommonBll commonbll = new CommonBll();

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
            //教师名下的班级
            wheres += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and UserClassId='" + Request["TeamId"].ToString() + "'";
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
            m.strFld = " a.*,TeamName";
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


                    //学生编号
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["StudentNo"]);
                    //学生姓名
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserName"]);
                    //性别
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserSex"]);
                    //班级名称
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["TeamName"]);
                    //学生账号
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserNo"]);
                    //登录密码
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["UserPwd"]);


                    sb.Append("</td></tr>");

                }
            }

            return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, sb.ToString());
        }

        /// <summary>
        /// 获取教师的班级
        /// </summary>
        /// <returns></returns>
        public string GetListTeam()
        {
            string wheres = " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            DataTable dt = commonbll.GetListDatatable("*", "tb_Team", wheres);

            return JsonConvert.SerializeObject(dt);
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
        /// 导出
        /// </summary>
        public string Export()
        {
            string wheres = " and State in(1,2) and UserType=3";//角色3 状态1
            //教师名下的班级
            wheres += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }

            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                wheres += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }


            DataTable dt = commonbll.GetListDatatable("StudentNo,UserName,UserSex,TeamName,UserNo,UserPwd",
                @"tb_UserInfo a inner join tb_Team b on a.UserClassId=b.Id
                 inner join  tb_School c on c.Id=b.SchoolId", wheres + " order by UId desc");

            string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "员工信息管理";
            string filename = "/Export/" + ExcelName + ".xlsx";

            OfficeHelper officeHp = new OfficeHelper();
            var Result = officeHp.DtToExcel(dt, "员工信息管理", new string[] { "员工编号", "员工姓名", "性别", "支行名称", "员工账号", "登录密码" }, "员工信息管理", ExcelName);

            var json = new object[] {
                        new{
                            filename=filename,
                        }
                    };
            return JsonConvert.SerializeObject(json);
        }

    }
}
