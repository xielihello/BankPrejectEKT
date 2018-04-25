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
using VocationalProject_DBUtility;


namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
    FileName:个人总成绩
    Copyright（c）2017-金融教育在线技术开发部
    Author:袁学
    Create Date:2017-4-18
    ******************************************************************/

    public class PersonalTotalScoreController : BaseController
    {
        //
        // GET: /Admin/PersonalTotalScore/
        CommonBll commonbll = new CommonBll();
        public ActionResult Index()
        {
            ViewData["UserType"] = UserType;
            return View();
        }

        /// <summary>
        /// 列表数据
        /// </summary>
        /// <returns></returns>
        public string GetList() {
            string wheres = "";//最外层条件
            string whrersTo = " and UserType=3 and State=1";//里层条件
            if (UserType == "2")//教师 自己带的学生 
            {
                whrersTo += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            }

            //查询条件
           
            //院校
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                whrersTo += " and c.Id='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                whrersTo += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }
            ///////////////
          

            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                whrersTo += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }

            //分数区间------------------只有排序能在最外层
            if (Request["ScoreQJ"] != null && Request["ScoreQJ"].ToString().Length > 0)
            {
                var ScoreQJ = Request["ScoreQJ"].ToString();
                wheres += " and Toscores between " + ScoreQJ.Split('-')[0] + " and " + ScoreQJ.Split('-')[1];
            }

            var hbzs_selectId = Request["hbzs_selectId"];
            var sgdc_selectId = Request["sgdc_selectId"];
            var fhbb_selectId = Request["fhbb_selectId"];
            var djlr_selectId = Request["djlr_selectId"];

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = Request["descby"]; //排序必须填写
            m.strFld = "*";
            m.tab = @"(
select Name,TeamName,b.Custom2,UserNo,UserName,StudentNo,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + hbzs_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=1)  as hbzs,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + sgdc_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=2)  as sgdc,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + fhbb_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=3)  as fhbb,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + djlr_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=4)  as djlr,
(
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + hbzs_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=1)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + sgdc_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=2)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + fhbb_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=3)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + djlr_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=4)
) as Toscores
from tb_UserInfo a 
inner join tb_Team b on b.Id=a.UserClassId
inner join tb_School c on c.Id=b.SchoolId
where 1=1  " + whrersTo + ") t";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));

        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public string ExportTotalScore() {
            string wheres = "";//最外层条件
            string whrersTo = " and UserType=3 and State=1";//里层条件
            if (UserType == "2")//教师 自己带的学生 
            {
                whrersTo += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            }

            //查询条件

            //院校
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                whrersTo += " and c.Id='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                whrersTo += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }
            ///////////////


            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                whrersTo += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }

            //分数区间------------------只有排序能在最外层
            if (Request["ScoreQJ"] != null && Request["ScoreQJ"].ToString().Length > 0)
            {
                var ScoreQJ = Request["ScoreQJ"].ToString();
                wheres += " and Toscores between " + ScoreQJ.Split('-')[0] + " and " + ScoreQJ.Split('-')[1];
            }

            var hbzs_selectId = Request["hbzs_selectId"];
            var sgdc_selectId = Request["sgdc_selectId"];
            var fhbb_selectId = Request["fhbb_selectId"];
            var djlr_selectId = Request["djlr_selectId"];


            if (UserType == "1")//管理员
            {
                string list = "*";
                string table = @"(
select Name,TeamName,b.Custom2,UserNo,UserName,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + hbzs_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=1)  as hbzs,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + sgdc_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=2)  as sgdc,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + fhbb_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=3)  as fhbb,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + djlr_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=4)  as djlr,
(
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + hbzs_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=1)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + sgdc_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=2)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + fhbb_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=3)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + djlr_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=4)
) as Toscores
from tb_UserInfo a 
inner join tb_Team b on b.Id=a.UserClassId
inner join tb_School c on c.Id=b.SchoolId
where 1=1  " + whrersTo + ") t";

                DataTable dt = commonbll.GetListDatatable(list, table, wheres + " order by " + Request["descby"]);//加排序

                string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "个人总成绩";
                string filename = "/Export/" + ExcelName + ".xlsx";

                OfficeHelper officeHp = new OfficeHelper();
                var Result = officeHp.DtToExcel(dt, "个人总成绩", new string[] { "所属院校", "所属班级", "班级教师", "学生账号", "学生姓名", "货币知识", "手工点钞", "复核报表", "单据录入", "总分" }, "个人总成绩", ExcelName);

                var json = new object[] {
                        new{
                            filename=filename,
                        }
                    };
                return JsonConvert.SerializeObject(json);
            }
            else {
                string list = "*";
                string table = @"(
select TeamName,UserNo,UserName,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + hbzs_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=1)  as hbzs,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + sgdc_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=2)  as sgdc,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + fhbb_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=3)  as fhbb,
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + djlr_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=4)  as djlr,
(
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + hbzs_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=1)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + sgdc_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=2)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + fhbb_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=3)+
(select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_EId=" + djlr_selectId + @" and ER_MId=UId and ER_State=0 and ER_Type=4)
) as Toscores
from tb_UserInfo a 
inner join tb_Team b on b.Id=a.UserClassId
inner join tb_School c on c.Id=b.SchoolId
where 1=1  " + whrersTo + ") t";

                DataTable dt = commonbll.GetListDatatable(list, table, wheres + " order by " + Request["descby"]);//加排序

                string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "个人总成绩";
                string filename = "/Export/" + ExcelName + ".xlsx";

                OfficeHelper officeHp = new OfficeHelper();
                var Result = officeHp.DtToExcel(dt, "个人总成绩", new string[] {  "所属班级",  "学生账号", "学生姓名", "货币知识", "手工点钞", "复核报表", "单据录入", "总分" }, "个人总成绩", ExcelName);

                var json = new object[] {
                        new{
                            filename=filename,
                        }
                    };
                return JsonConvert.SerializeObject(json);
            
            }
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
            if (UserType == "2")//教师 自己的班级 
            {
                wheres += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            }
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and SchoolId='" + Request["SchooId"].ToString() + "'";
            }

            DataTable dt = commonbll.GetListDatatable("Id,TeamName", "tb_Team", wheres);
            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 货币知识
        /// </summary>
        /// <returns></returns>
        public string GetHBZS()
        {
            string wheres = " and E_IsState=1";//要求激活的
            if (UserType == "2")//教师 设置的考试 
            {
                wheres += " and E_AddOperator=" + UId;
            }
            if (Request["sjly"] != null && Request["sjly"].ToString().Length > 0)//有输入试卷来源
            {
                wheres += " and UR_Custom2 like '%" + Request["sjly"].ToString() + "%'";
            }

            DataTable dt = commonbll.GetListDatatable("EId,E_Name", "tb_HB_Examination", wheres);
            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 手工点钞
        /// </summary>
        /// <returns></returns>
        public string GetSGDC()
        {
            string wheres = " and ExaminationState=1";//要求激活的
            if (UserType == "2")//教师 设置的考试 
            {
                wheres += " and a.AddOperator=" + UId;
            }
            if (Request["sjly"] != null && Request["sjly"].ToString().Length > 0)//有输入试卷来源
            {
                wheres += " and UserNo like '%" + Request["sjly"].ToString() + "%'";
            }

            DataTable dt = commonbll.GetListDatatable("ID,ExaminationName", "tb_Manual_Examination a inner join tb_UserInfo  b on b.UId=a.AddOperator", wheres);
            return JsonConvert.SerializeObject(dt);
        }


        /// <summary>
        /// 复核报表
        /// </summary>
        /// <returns></returns>
        public string GetFHBB()
        {
            string wheres = " and Isactivation=1";//要求激活的
            if (UserType == "2")//教师 设置的考试 
            {
                wheres += " and a.AddOperator=" + UId;
            }
            if (Request["sjly"] != null && Request["sjly"].ToString().Length > 0)//有输入试卷来源
            {
                wheres += " and UserNo like '%" + Request["sjly"].ToString() + "%'";
            }

            DataTable dt = commonbll.GetListDatatable("Id,ExaminationName", "tb_FH_Examination a inner join tb_UserInfo  b on b.UId=a.AddOperator", wheres);
            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 单据录入
        /// </summary>
        /// <returns></returns>
        public string GetDJlR()
        {
            string wheres = " and Bill_Spare1=1";//要求激活的
            if (UserType == "2")//教师 设置的考试 
            {
                wheres += " and a.AddOperator=" + UId;
            }
            if (Request["sjly"] != null && Request["sjly"].ToString().Length > 0)//有输入试卷来源
            {
                wheres += " and UserNo like '%" + Request["sjly"].ToString() + "%'";
            }

            DataTable dt = commonbll.GetListDatatable("ID,ExamName", "tb_Bill_Exam a inner join tb_UserInfo  b on b.UId=a.AddOperator", wheres);
            return JsonConvert.SerializeObject(dt);
        }

    }
}
