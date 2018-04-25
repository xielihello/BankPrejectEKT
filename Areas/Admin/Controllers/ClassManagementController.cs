using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VocationalProject.Areas.Admin.Models;
using VocationalProject.Controllers;
using VocationalProject_Bll;
using VocationalProject_Dal;
using VocationalProject_Models;

namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
    FileName:班级管理
    Copyright（c）2017-金融教育在线技术开发部
    Author:邵世铨
    Create Date:2017-3-29
   ******************************************************************/
    public class ClassManagementController : BaseController
    {
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        TeamBll tbll = new TeamBll();
        CommonBll commBll = new CommonBll();
        public ActionResult ClassManagement()
        {
            return View();
        }

        /// <summary>
        /// 读取学生列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList()
        {
            string wheres = "";//角色3 状态1
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and t.SchoolId='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and t.Id='" + Request["TeamId"].ToString() + "'";
            }

            if (Request["TeacherName"] != null && Request["TeacherName"].ToString().Length > 0)//教师名称
            {

                wheres += " and t.Custom2 like '%" + Request["TeacherName"].ToString() + "%'";
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "t.Id desc"; //排序必须填写
            m.strFld = " t.*,s.Name";
            m.tab = "tb_Team t left join  tb_School s on s.Id=t.SchoolId";
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
                    sb.Append("<td><input type=\"checkbox\" value=\"" + dt.Rows[i]["Id"] + "\"  class=\"i-checks\" name=\"input[]\"/>");
                    string Eid = dt.Rows[i]["Id"].ToString();
                    //院校名称
                    sb.Append("</td><td>");
                    sb.Append(dt.Rows[i]["Name"]);
                    //班级编号
                    sb.Append("</td><td>");
                    var TeamCode = dt.Rows[i]["TeamCode"].ToString();
                    if (TeamCode.Length>20)
                    {
                        TeamCode = TeamCode.Substring(0, 18) + "...";
                    }
                    sb.Append("<span title='" + dt.Rows[i]["TeamCode"].ToString() + "'>" + TeamCode + "</span>");
                    //班级名称
                    sb.Append("</td><td>");
                    var TeamName = dt.Rows[i]["TeamName"].ToString();
                    if (TeamName.Length > 20)
                    {
                        TeamName = TeamName.Substring(0, 18) + "...";
                    }

                    sb.Append("<span title='" + dt.Rows[i]["TeamName"].ToString() + "'>" + TeamName + "</span>");
                    //班级教师
                    sb.Append("</td><td>");
                    var Custom2 = dt.Rows[i]["Custom2"].ToString();
                    if (Custom2.Length > 15)
                    {
                        Custom2 = Custom2.Substring(0, 14) + "...";
                    }


                    sb.Append("<span title='" + dt.Rows[i]["Custom2"].ToString() + "'>" + Custom2 + "</span>");
                    //操作
                    sb.Append("</td>");
                    //操作
                    sb.Append("<td>");
                    sb.Append(" <a href=\"JavaScript:void(0)\" onclick=\"Edit(" + Eid + ")\" class=\" btn-primary btn-sm\"><i class=\"fa fa-pencil m-r-xxs\"></i>编辑 </a>");
                    sb.Append("</td>");
                    sb.Append("</tr>");

                }
            }

            return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, sb.ToString());
        }
        /// <summary>
        /// 查询所有院校
        /// </summary>
        /// <returns></returns>
        public string SelectSchool()
        {
            StringBuilder SB = new StringBuilder();
            DataTable Dt = commBll.GetListDatatable("id,Name", "tb_School", "");
            SB.Append("<option value=\"0\">请选择分行名称</option>");
            if (Dt.Rows.Count > 0)
            {
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    SB.Append("<option value=" + Dt.Rows[i]["id"] + ">" + Dt.Rows[i]["Name"] + "</option>");
                }
            }
            return SB.ToString();
        }
        /// <summary>
        /// 查询班级
        /// </summary>
        /// <returns></returns>
        public string SelectClass(int Schoolid)
        {
            StringBuilder SB = new StringBuilder();
            DataTable Dt = new DataTable();
            if (Schoolid == 0 || Schoolid == null)
            {
                Dt = commBll.GetListDatatable("*", "tb_Team", "");
            }
            else
            {
                Dt = commBll.GetListDatatable("*", "tb_Team", " and SchoolId=" + Schoolid + "");
            }

            SB.Append("<option value=\"0\">请选择支行名称</option>");
            if (Dt.Rows.Count > 0)
            {
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    SB.Append("<option value=" + Dt.Rows[i]["Id"] + ">" + Dt.Rows[i]["TeamName"] + "</option>");
                }
            }
            return SB.ToString();
        }
        /// <summary>
        /// 查找教师
        /// </summary>
        /// <param name="SchoolId">院校ID</param>
        /// <returns></returns>
        public string selectTeacher(int SchoolId, string Teactid)
        {
            if (Teactid == "")
            {
                Teactid = "0";
            }
            StringBuilder SB = new StringBuilder();
            DataTable Dt = commBll.GetListDatatable("UId,UserName", "tb_UserInfo", " and UserType=2 and UserSchoolId=" + SchoolId + " and UId not in (" + Teactid + ") and UserName!='' and State=1");
            if (Dt.Rows.Count > 0)
            {
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    if (Dt.Rows[i]["UserName"].ToString() != "" || Dt.Rows[i]["UserName"].ToString() != null)
                    {
                        SB.Append("<option value=" + Dt.Rows[i]["UId"] + ">" + Dt.Rows[i]["UserName"] + "</option>");
                    }
                }
            }
            return SB.ToString();

        }
        /// <summary>
        /// 新增时查找教师
        /// </summary>
        /// <param name="SchoolId"></param>
        /// <param name="Teactid"></param>
        /// <returns></returns>
        public string AddselectTeacher(int SchoolId, string Teactid)
        {
            if (Teactid == "")
            {
                Teactid = "0";
            }
            StringBuilder SB = new StringBuilder();
            DataTable Dt = commBll.GetListDatatable("UId,UserName", "tb_UserInfo", " and UserType=2 and UserSchoolId=" + SchoolId + " and UserName!='' and State=1");
            if (Dt.Rows.Count > 0)
            {
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    if (Dt.Rows[i]["UserName"].ToString() != "" || Dt.Rows[i]["UserName"].ToString() != null)
                    {
                        SB.Append("<option value=" + Dt.Rows[i]["UId"] + ">" + Dt.Rows[i]["UserName"] + "</option>");
                    }
                }
            }
            return SB.ToString();

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id">班级ID</param>
        /// <returns></returns>
        public int CollegeDele(string Id)
        {
            //校验班级下是否存在学生
            var checkcount = commBll.GetRecordCount("tb_UserInfo", " and State=1 and UserType=3 and UserClassId in (" + Id + ")");
            if (checkcount > 0)
            {
                return 88;
            }
            try
            {
                commBll.DeleteInfo("tb_Team", " and Id in(" + Id + ")");
                return 1;
            }
            catch
            {
                return 99;
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="Id">班级ID</param>
        /// <returns></returns>
        public string CollegeEdit(string Id)
        {
            DataTable data = commBll.GetListDatatable("*", "tb_Team", " and Id=" + Id + "");

            return JsonConvert.SerializeObject(data);
        }
        /// <summary>
        /// 查询教师
        /// </summary>
        /// <param name="Teacherid">教师ID</param>
        /// <returns></returns>
        public string SelectEditTeacher(string Teactid)
        {
            string[] id = Teactid.Split(',');

            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < id.Length; i++)
            {
                if (id[i] != "")
                {
                    DataTable Dt = commBll.GetListDatatable("UId,UserName", "tb_UserInfo", "and UId=" + id[i] + " and State=1");
                    if (Dt.Rows.Count > 0)
                    {
                        SB.Append("<option value=" + Dt.Rows[0]["UId"] + ">" + Dt.Rows[0]["UserName"] + "</option>");
                    }
                }
            }
            return SB.ToString();
        }
        /// <summary>
        /// 查找
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Query(string SchooIdName, string TeamIdName, string TeacherName)
        {
            StringBuilder sb = new StringBuilder();

            if (SchooIdName != "")
            {
                sb.Append("and T.SchoolId=" + SchooIdName + "");
            }
            if (TeamIdName != "" || TeamIdName != "0")
            {
                sb.Append("and T.TeamName='" + TeamIdName + "'");
            }
            if (TeacherName != "")
            {
                sb.Append("and T.Custom2 like '%" + TeacherName + "%'");
            }

            DataTable data = commBll.GetListDatatable("T.*,S.Name", "tb_Team T left join tb_School S on T.SchoolId=S.Id", sb.ToString());
            return View("ClassManagement", data);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="CollegeCode">班级编号</param>
        /// <param name="CollegeName">班级名称</param>
        /// <param name="Teacheid">教师ID</param>
        /// <param name="SchooId">院校ID</param>
        /// <returns></returns>
        public string Submit(string CollegeCode, string CollegeName, string Teacheid, string SchooId)
        {
            int Count = commBll.GetRecordCount("tb_Team", " and SchoolId =" + SchooId + " and TeamCode='" + CollegeCode + "'");
            if (Count > 0)
            {
                return "-1";
            }
            else
            {
                int CountTwo = commBll.GetRecordCount("tb_Team", " and SchoolId =" + SchooId + " and TeamName='" + CollegeName + "'");
                if (CountTwo > 0)
                {
                    return "-2";
                }
                else
                {
                    string Tid = "";
                    string TeacherName = "";
                    if (Teacheid == "Null,")
                    {
                        Tid = "";
                        TeacherName = "";
                    }
                    else
                    {
                        string[] StrTeacheid = Teacheid.Split(',');

                        for (int i = 0; i < StrTeacheid.Length; i++)
                        {
                            if (StrTeacheid[i] != "Null" && StrTeacheid[i] != "")
                            {
                                DataTable Dt = commBll.GetListDatatable("UserName", "tb_UserInfo", "and UId=" + StrTeacheid[i] + "");
                                if (Dt.Rows.Count > 0)
                                {
                                    if (i + 2 < StrTeacheid.Length)
                                    {
                                        TeacherName += Dt.Rows[0]["UserName"] + ",";
                                    }
                                    else
                                    {

                                        TeacherName += Dt.Rows[0]["UserName"];
                                    }

                                }
                                if (i + 2 != StrTeacheid.Length)
                                {
                                    Tid += StrTeacheid[i] + ",";
                                }
                                else
                                {
                                    Tid += StrTeacheid[i];
                                }
                            }
                        }
                    }
                    return tbll.AddClass(CollegeCode, CollegeName, Tid, SchooId, UId, TeacherName).ToString();

                }
            }
        }
        /// <summary>
        /// 班级编辑
        /// </summary>
        /// <param name="CollegeCode">班级编号</param>
        /// <param name="CollegeName">班级名称</param>
        /// <param name="Teacheid">教师ID</param>
        /// <param name="SchooId">院校ID</param>
        /// <param name="EId">班级ID</param>
        /// <returns></returns>
        public string EditSubmit(string CollegeCode, string CollegeName, string Teacheid, string SchooId, string EId)
        {
            int Count = commBll.GetRecordCount("tb_Team", " and SchoolId =" + SchooId + " and TeamCode='" + CollegeCode + "' and Id!=" + EId + "");
            if (Count > 0)
            {
                return "-1";
            }
            else
            {
                int CountTwo = commBll.GetRecordCount("tb_Team", " and SchoolId =" + SchooId + " and TeamName='" + CollegeName + "'and Id!=" + EId + "");
                if (CountTwo > 0)
                {
                    return "-2";
                }
                else
                {
                    string Tid = "";
                    string TeacherName = "";
                    if (Teacheid == "Null,")
                    {
                        Tid = "";
                        TeacherName = "";
                    }
                    else
                    {
                        string[] StrTeacheid = Teacheid.Split(',');

                        for (int i = 0; i < StrTeacheid.Length; i++)
                        {
                            if (StrTeacheid[i] != "Null" && StrTeacheid[i] != "")
                            {
                                DataTable Dt = commBll.GetListDatatable("UserName", "tb_UserInfo", "and UId=" + StrTeacheid[i] + "");
                                if (Dt.Rows.Count > 0)
                                {
                                    if (i + 2 < StrTeacheid.Length)
                                    {
                                        TeacherName += Dt.Rows[0]["UserName"] + ",";
                                    }
                                    else
                                    {

                                        TeacherName += Dt.Rows[0]["UserName"];
                                    }

                                }
                                if (i + 2 != StrTeacheid.Length)
                                {
                                    Tid += StrTeacheid[i] + ",";
                                }
                                else
                                {
                                    Tid += StrTeacheid[i];
                                }
                            }
                        }
                    }
                    tb_Team team = new tb_Team();
                    team.Id = Convert.ToInt32(EId);
                    team.TeamCode = CollegeCode;
                    team.TeamName = CollegeName;
                    team.SchoolId = Convert.ToInt32(SchooId);
                    team.TeacherId = Tid;
                    team.Operator = UId;
                    team.Custom2 = TeacherName;
                    return tbll.EditEditBll(team).ToString();
                }
            }
        }
    }
}
