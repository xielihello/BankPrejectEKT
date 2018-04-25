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
using VocationalProject.Models;
using VocationalProject_Bll;
using VocationalProject_Dal;
using VocationalProject_DBUtility;

namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
    FileName:手工点钞成绩管理
    Copyright（c）2017-金融教育在线技术开发部
    Author:柯思金
    Create Date:2017-4-17
    ******************************************************************/
    public class SG_achievementController : BaseController
    {
        TeamBll tbll = new TeamBll();
        CommonBll commBll = new CommonBll();
        public ActionResult Index()
        {
            ViewData["UserType"] = UserType;
            return View();
        }
        /// <summary>
        /// 列表数据
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {

            //教师名下的班级
            string wheres = @" and a.UId=b.StudenID and a.UserClassId=d.Id and d.SchoolId=c.Id  
and f.ID=e.Taskid and b.ExaminationID=e.ID and e.ExaminationState=1 and b.Spare1=0";//手工点钞成绩 最新的一次做题
            if (UserType == "2")//教师 自己带的学生 
            {
                wheres += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            }

            //查询条件
            //考试名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {

                wheres += " and ExaminationName like '%" + Request["E_Name"].ToString() + "%'";
            }
            //考试类型 模式
            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")
            {
                if (Request["E_Type"] != "3")
                {
                    wheres += " and Pattern='" + Request["E_Type"].ToString() + "'";
                }

            }
            //院校
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and c.Id='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }
            ///////////////
            if (Request["P_Kind"] != null && Request["P_Kind"].ToString() != "0")//试卷属性
            {
                wheres += " and Kind='" + Request["P_Kind"].ToString() + "'";
            }
            
            
            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                wheres += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or a.UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }

            //分数区间
            if (Request["ScoreQJ"] != null && Request["ScoreQJ"].ToString().Length > 0)
            {
                var ScoreQJ = Request["ScoreQJ"].ToString();
                wheres += " and TotalScore between " + ScoreQJ.Split('-')[0] + " and " + ScoreQJ.Split('-')[1];
            }
            if (Request["P_Custom2"] != null && Request["P_Custom2"].ToString().Length > 0)//试卷来源
            {
                wheres += "and f.AddOperator in ( select  UId from tb_UserInfo where (UserNo like '%" + Request["P_Custom2"] + "%' or UserName like '%" + Request["P_Custom2"] + "%') and UserType in (1,2))";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = Request["descby"]; //排序必须填写
            m.strFld = @" b.TaskScore,b.RefereePoints,b.TotalScore,a.UserName,a.UserNo,c.Name,d.TeamName,d.Custom2,b.CommitTime,e.ExaminationName, Score,b.StudenID,b.ID,
(case e.Kind when 1 then '系统试卷'  when 2 then '教师试卷' end) as  Kind,
( select UserNo from tb_UserInfo where UId=e.AddOperator) as AddOperator,
( select UserName from tb_UserInfo where UId=e.AddOperator) as AddOperator1,e.ID as IDS,
(case Pattern when 1 then '练习模式'  when 2 then '考试模式' when 3 then '竞赛模式' end) as Pattern";
            
            m.tab = @"tb_UserInfo a,tb_Manual_ExaminationAnswer b,tb_Team d,tb_School c,tb_Manual_Examination e,tb_Manual_Counting f";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));
        }
        /// <summary>
        /// 查询所有院校
        /// </summary>
        /// <returns></returns>
        public string SelectSchool()
        {
            StringBuilder SB = new StringBuilder();
            DataTable Dt = commBll.GetListDatatable("id,Name", "tb_School", "");
            SB.Append("<option value=\"0\">请选择院校名称</option>");
            if (Dt.Rows.Count > 0)
            {
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    SB.Append("<option value='" + Dt.Rows[i]["id"] + "'>" + Dt.Rows[i]["Name"] + "</option>");
                }
            }
            return JsonConvert.SerializeObject(Dt);
        }
        /// <summary>
        /// 查询班级
        /// </summary>
        /// <returns></returns>
        public string SelectClass(int Schoolid)
        {
            StringBuilder SB = new StringBuilder();
            DataTable Dt = new DataTable();
            if (Schoolid == 0)
            {
                Dt = commBll.GetListDatatable("*", "tb_Team", "");
            }
            else
            {
                Dt = commBll.GetListDatatable("*", "tb_Team", " and SchoolId=" + Schoolid + "");
            }

            SB.Append("<option value=\"0\">请选择班级名称</option>");
            if (Dt.Rows.Count > 0)
            {
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    SB.Append("<option value='" + Dt.Rows[i]["TeamName"] + "'>" + Dt.Rows[i]["TeamName"] + "</option>");
                }
            }
            return JsonConvert.SerializeObject(Dt);
        }
        public string update()
        {
            // Request["id"]
            SqlParameter[] pars ={
                               
                                };
            //根据考试id和用户id查询出最新的一条的 考试总表分值
            var Eid = "0";
            var Mid = "0";
            DataTable dt = commBll.GetListDatatable("StudenID,ExaminationID", "tb_Manual_ExaminationAnswer", " and ID=" + Request["id"]);
            if (dt.Rows.Count > 0)
            {
                Eid = dt.Rows[0]["ExaminationID"].ToString();
                Mid = dt.Rows[0]["StudenID"].ToString();

                //查询最新的Id
                var ERId = commBll.GetListSclar("top 1 ERId", "tb_ExaminationResult", " and [ER_Type]=2  and [ER_State]=0 and ER_EId=" + Eid + " and ER_MId=" + Mid + " order by ERId desc");
                commBll.UpdateInfo("tb_ExaminationResult", " ER_Score='" + Request["yuan"] + "'", " and ERId=" + ERId, pars);
            }

            int count = commBll.UpdateInfo("tb_Manual_ExaminationAnswer", "RefereePoints=" + Request["fenzhi"] + ",TotalScore=" + Request["yuan"], " and ID=" + Request["id"], pars);
            return JsonConvert.SerializeObject(count);
        }
        //导出
        public string ExportExamResult()
        {

            //教师名下的班级
            string wheres = @" and a.UId=b.StudenID and a.UserClassId=d.Id and d.SchoolId=c.Id  
and f.ID=e.Taskid and b.ExaminationID=e.ID ";//货币知识成绩 最新的一次做题
            if (UserType == "2")//教师 自己带的学生 
            {
                wheres += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            }

            //查询条件
            //考试名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {

                wheres += " and ExaminationName like '%" + Request["E_Name"].ToString() + "%'";
            }
            //考试类型 模式
            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")
            {
                if (Request["E_Type"] != "3")
                {
                    wheres += " and Pattern='" + Request["E_Type"].ToString() + "'";
                }

            }
            //院校
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and c.Id='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }
            ///////////////
            if (Request["P_Kind"] != null && Request["P_Kind"].ToString() != "0")//试卷属性
            {
                wheres += " and Kind='" + Request["P_Kind"].ToString() + "'";
            }

            
            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                wheres += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or a.UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }

            //分数区间
            if (Request["ScoreQJ"] != null && Request["ScoreQJ"].ToString().Length > 0)
            {
                var ScoreQJ = Request["ScoreQJ"].ToString();
                wheres += " and TotalScore between " + ScoreQJ.Split('-')[0] + " and " + ScoreQJ.Split('-')[1];
            }
            if (Request["P_Custom2"] != null && Request["P_Custom2"].ToString().Length > 0)//试卷来源
            {
                wheres += "and f.AddOperator in ( select  UId from tb_UserInfo where (UserNo like '%" + Request["P_Custom2"] + "%' or UserName like '%" + Request["P_Custom2"] + "%') and UserType in (1,2))";
            }
            if (UserType == "1")//管理员
            {
                string list = @" e.ExaminationName,(case e.Kind when 1 then '系统试卷'  when 2 then '教师试卷' end) as  Kind,
( select UserNo from tb_UserInfo where UId=e.AddOperator) as AddOperator,(case Pattern when 1 then '练习模式'  when 2 then '考试模式' when 3 then '竞赛模式' end) as Pattern,a.UserNo,a.UserName,c.Name,d.TeamName,d.Custom2 Score,b.TaskScore,b.TotalScore,b.CommitTime";
                string table = @"tb_UserInfo a,tb_Manual_ExaminationAnswer b,tb_Team d,tb_School c,tb_Manual_Examination e,tb_Manual_Counting f";

                DataTable dt = commBll.GetListDatatable(list, table, wheres + " order by " + Request["descby"]);//加排序

                string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "手工点钞成绩";
                string filename = "/Export/" + ExcelName + ".xlsx";

                OfficeHelper officeHp = new OfficeHelper();
                var Result = officeHp.DtToExcel(dt, "手工点钞成绩", new string[] { "考试名称", "试卷属性", "试卷来源", "考试类型", "学生账号", "学生姓名", "所属院校", "所属班级", "班级教师", "试卷总分", "考试得分", "提交时间" }, "手工点钞成绩", ExcelName);

                var json = new object[] {
                        new{
                            filename=filename,
                        }
                    };
                return JsonConvert.SerializeObject(json);
            }
            else
            {
                //教师
                string list = @"e.ExaminationName,(case Pattern when 1 then '考试模式'  when 2 then '练习模式' when 3 then '竞赛模式' end) as Pattern,a.UserNo,a.UserName,d.TeamName, Score,b.TaskScore,b.TotalScore,b.CommitTime";
                string table = @"tb_UserInfo a,tb_Manual_ExaminationAnswer b,tb_Team d,tb_School c,tb_Manual_Examination e,tb_Manual_Counting f";
                DataTable dt = commBll.GetListDatatable(list, table, wheres + " order by " + Request["descby"]);//加排序
                string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "手工点钞成绩";
                string filename = "/Export/" + ExcelName + ".xlsx";
                OfficeHelper officeHp = new OfficeHelper();
                var Result = officeHp.DtToExcel(dt, "手工点钞成绩", new string[] { "考试名称", "考试类型", "学生账号", "学生姓名", "所属班级", "试卷总分", "考试得分", "提交时间" }, "手工点钞成绩", ExcelName);

                var json = new object[] {
                        new{
                            filename=filename,
                        }
                    };
                return JsonConvert.SerializeObject(json);
            }
        }
    }
}
