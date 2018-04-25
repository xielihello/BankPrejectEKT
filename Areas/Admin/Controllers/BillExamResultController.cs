﻿using System;
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
   FileName:单据录入 考试成绩
   Copyright（c）2017-金融教育在线技术开发部
   Author:袁学
   Create Date:2017-4-17
   ******************************************************************/
    public class BillExamResultController : BaseController
    {
        //
        // GET: /Admin/BillExamResult/
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
        public string GetList()
        {

            //教师名下的班级
            string wheres = "and ER_Type=4  and ER_State=0";//单据录入 3 最新的一次做题
            if (UserType == "2")//教师 自己带的学生 
            {
                wheres += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            }

            //查询条件
            //考试名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {

                wheres += " and ExamName like '%" + Request["E_Name"].ToString() + "%'";
            }
            //考试类型 模式
            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")
            {
                wheres += " and ExamPattern='" + Request["E_Type"].ToString() + "'";
            }
            //院校
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and f.Id='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }
            ///////////////
            if (Request["P_Kind"] != null && Request["P_Kind"].ToString() != "0")//试卷属性
            {
                wheres += " and c.Kind='" + Request["P_Kind"].ToString() + "'";
            }

            if (Request["P_Custom2"] != null && Request["P_Custom2"].ToString().Length > 0)//试卷来源
            {
                wheres += " and b.AddOperator in (select UId from tb_UserInfo where 1=1 and (UserName like '%" + Request["P_Custom2"].ToString() + "%' or UserNo like '%" + Request["P_Custom2"].ToString() + "%'))";

            }
            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                wheres += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }

            //分数区间
            if (Request["ScoreQJ"] != null && Request["ScoreQJ"].ToString().Length > 0)
            {
                var ScoreQJ = Request["ScoreQJ"].ToString();
                wheres += " and ER_Score between " + ScoreQJ.Split('-')[0] + " and " + ScoreQJ.Split('-')[1];
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = Request["descby"]; //排序必须填写
            m.strFld = @" ERId,ER_EId,ER_PId,ER_MId,ExamName as E_Name,
(case c.Kind when 1 then '系统试卷'  when 2 then '教师试卷' end) as  P_Kind,
(select UserName from tb_UserInfo where UId=b.AddOperator) as AUserName,
(select UserNo from tb_UserInfo where UId=b.AddOperator) as UR_Custom2,
(case ExamPattern when 2 then '考试模式'  when 1 then '练习模式' end) as E_Type,
UserNo,UserName,Name,TeamName,e.Custom2,
(select SUM(Score) from tb_Bill_TestPaperEven where PaperID=c.Id) as Score,ER_Score,ER_AddTime";
            m.tab = @"tb_ExaminationResult a
inner join tb_Bill_Exam b on a.ER_EId=b.ID
inner join tb_Bill_TestPaper c on c.ID=b.PaperID
inner join tb_UserInfo d on d.UId=a.ER_MId
inner join tb_Team e on e.Id=d.UserClassId
inner join tb_School f on f.Id=e.SchoolId";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));
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

        //导出
        public string ExportExamResult()
        {

            //教师名下的班级
            string wheres = "and ER_Type=4  and ER_State=0";//单据录入 3 最新的一次做题
            if (UserType == "2")//教师 自己带的学生 
            {
                wheres += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            }

            //查询条件
            //考试名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {

                wheres += " and ExamName like '%" + Request["E_Name"].ToString() + "%'";
            }
            //考试类型 模式
            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")
            {
                wheres += " and ExamPattern='" + Request["E_Type"].ToString() + "'";
            }
            //院校
            if (Request["SchooId"] != null && Request["SchooId"].ToString() != "0")//学院下拉框
            {
                wheres += " and f.Id='" + Request["SchooId"].ToString() + "'";
            }
            if (Request["TeamId"] != null && Request["TeamId"].ToString() != "0")//班级下拉框
            {
                wheres += " and UserClassId='" + Request["TeamId"].ToString() + "'";
            }
            ///////////////
            if (Request["P_Kind"] != null && Request["P_Kind"].ToString() != "0")//试卷属性
            {
                wheres += " and c.Kind='" + Request["P_Kind"].ToString() + "'";
            }

            if (Request["P_Custom2"] != null && Request["P_Custom2"].ToString().Length > 0)//试卷来源
            {
                wheres += " and b.AddOperator in (select UId from tb_UserInfo where 1=1 and (UserName like '%" + Request["P_Custom2"].ToString() + "%' or UserNo like '%" + Request["P_Custom2"].ToString() + "%'))";

            }
            //学生信息 账号 姓名 学号 查询
            if (Request["StudentInfo"] != null && Request["StudentInfo"].ToString().Length > 0)
            {
                wheres += " and ( UserNo like '%" + Request["StudentInfo"] + "%' or UserName like '%" + Request["StudentInfo"] + "%' or StudentNo like '%" + Request["StudentInfo"] + "%') ";
            }

            //分数区间
            if (Request["ScoreQJ"] != null && Request["ScoreQJ"].ToString().Length > 0)
            {
                var ScoreQJ = Request["ScoreQJ"].ToString();
                wheres += " and ER_Score between " + ScoreQJ.Split('-')[0] + " and " + ScoreQJ.Split('-')[1];
            }

            if (UserType == "1")//管理员
            {

                string list = @"ExamName as E_Name,
(case c.Kind when 1 then '系统试卷'  when 2 then '教师试卷' end) as  P_Kind,
ISNULL((select UserName from tb_UserInfo where UId=b.AddOperator),(select UserNo from tb_UserInfo where UId=b.AddOperator)) as AUserName,
(case ExamPattern when 2 then '考试模式'  when 1 then '练习模式' end) as E_Type,
UserNo,UserName,Name,TeamName,e.Custom2,
(select SUM(Score) from tb_Bill_TestPaperEven where PaperID=c.Id) as Score,ER_Score,ER_AddTime";
                string table = @"tb_ExaminationResult a
inner join tb_Bill_Exam b on a.ER_EId=b.ID
inner join tb_Bill_TestPaper c on c.ID=b.PaperID
inner join tb_UserInfo d on d.UId=a.ER_MId
inner join tb_Team e on e.Id=d.UserClassId
inner join tb_School f on f.Id=e.SchoolId";

                DataTable dt = commonbll.GetListDatatable(list, table, wheres + " order by " + Request["descby"]);//加排序

                string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "单据录入成绩";
                string filename = "/Export/" + ExcelName + ".xlsx";

                OfficeHelper officeHp = new OfficeHelper();
                var Result = officeHp.DtToExcel(dt, "单据录入成绩", new string[] { "考试名称", "试卷属性", "试卷来源", "考试类型", "学生账号", "学生姓名", "所属院校", "所属班级", "班级教师", "试卷总分", "考试得分", "提交时间" }, "单据录入成绩", ExcelName);

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
                string list = @"ExamName as E_Name,(case ExamPattern when 2 then '考试模式'  when 1 then '练习模式' end) as E_Type,
UserNo,UserName,TeamName,
(select SUM(Score) from tb_Bill_TestPaperEven where PaperID=c.Id) as Score,ER_Score,ER_AddTime";
                string table = @"tb_ExaminationResult a
inner join tb_Bill_Exam b on a.ER_EId=b.ID
inner join tb_Bill_TestPaper c on c.ID=b.PaperID
inner join tb_UserInfo d on d.UId=a.ER_MId
inner join tb_Team e on e.Id=d.UserClassId
inner join tb_School f on f.Id=e.SchoolId";

                DataTable dt = commonbll.GetListDatatable(list, table, wheres + " order by " + Request["descby"]);//加排序

                string ExcelName = DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.Millisecond + "单据录入成绩";
                string filename = "/Export/" + ExcelName + ".xlsx";

                OfficeHelper officeHp = new OfficeHelper();
                var Result = officeHp.DtToExcel(dt, "单据录入成绩", new string[] { "考试名称", "考试类型", "学生账号", "学生姓名", "所属班级", "试卷总分", "考试得分", "提交时间" }, "单据录入成绩", ExcelName);

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
