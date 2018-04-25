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
    public class StatisticalAnalysisController : BaseController
    {
        /***************************************************************
      FileName:成绩统计分析
      Copyright（c）2017-金融教育在线技术开发部
      Author:唐
      Create Date:2017-4-25
      ******************************************************************/

        CommonBll commonbll = new CommonBll();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 成绩统计分析查询
        /// </summary>
        /// <param name="Type">查询类型：1 平均分、2 通过率</param>
        /// <param name="chkstr">类型集合：1 货币知识、2 手工点钞、3 复核报表、4 单据录入</param>
        /// <param name="MinTxt">最小值</param>
        /// <param name="MaxTxt">最大值</param>
        /// <returns></returns>
        public string GetList(int Type, string chkstr, int MinTxt = 0, int MaxTxt = 999)
        {

            
            string wheres = "";//最外层条件
            string whrersTo = " and UserType=3 and State=1";//里层条件
            if (UserType == "2")//教师 自己带的学生 
            {
                whrersTo += " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            }

            //平均值

            //所有赛项的平均分  =  总分数/练习次数
            string sqlScore = @"( case when (select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_MId=a.UId)>0 then 
                    (select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_MId=a.UId) /(select ISNULL(COUNT(1),0) from tb_ExaminationResult  where ER_MId=a.UId)
                        else 0 end ) as Result";
            string lenstr = "";

            //所有赛项的练习次数
            string lenstrs = "(select ISNULL(COUNT(1),0) from tb_ExaminationResult   where ER_MId=a.UId)";
            if (!string.IsNullOrEmpty(chkstr))
            {

                //赛项的平均分 类型集合：1 货币知识、2 手工点钞、3 复核报表、4 单据录入
                sqlScore = @"( case when (select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_MId=a.UId and ER_Type in(" + chkstr + @"))>0 then 
                    (select ISNULL(sum(ER_Score),0) from tb_ExaminationResult where ER_MId=a.UId and ER_Type in(" + chkstr + "))/(select ISNULL(COUNT(1),0) from tb_ExaminationResult where ER_MId=a.UId and ER_Type in(" + chkstr + "))  else 0 end ) as Result";
               
                //赛项的练习次数 类型集合：1 货币知识、2 手工点钞、3 复核报表、4 单据录入
                lenstrs = "(select ISNULL(COUNT(1),0) from tb_ExaminationResult   where ER_MId=a.UId and  ER_Type in(" + chkstr + "))";
                var zu = chkstr.Split(',');
                //赛项时长
                if (zu.Length > 0)
                {
                    
                    for (int i = 0; i < zu.Length; i++)
                    {
                        if (zu[i] == "1")
                        {
                            lenstr += "(select ISNULL(SUM(f.E_Whenlong),0) from [dbo].tb_HB_Examination f left join  [dbo].tb_ExaminationResult e on(f.EId=e.ER_EId) where  e.ER_MId=a.UId and ER_Type in(1))" + "+";
                        }
                        else if (zu[i] == "2")
                        {
                            lenstr += "(select ISNULL(SUM(f.LongTime),0) from [dbo].tb_Manual_Examination f left join  [dbo].tb_ExaminationResult e on(f.ID=e.ER_EId) where  e.ER_MId=a.UId and ER_Type in(2))" + "+";
                        }
                        else if (zu[i] == "3")
                        {
                            lenstr += "(select ISNULL(SUM(f.ExaminationLength),0) from [dbo].[tb_FH_Examination] f left join  [dbo].tb_ExaminationResult e on(f.Id=e.ER_EId) where  e.ER_MId=a.UId and ER_Type in(3))" + "+";
                        }
                        else
                        {
                            lenstr += "(select ISNULL(SUM(f.TimeLong),0) from [dbo].tb_Bill_Exam f left join  [dbo].tb_ExaminationResult e on(f.ID=e.ER_EId) where  e.ER_MId=a.UId and ER_Type in(4))" + "+";
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(lenstr))
            {
                lenstr = lenstr.Substring(0, lenstr.Length - 1);

            }
            else
            {
                //赛项总时长
                lenstr += "(select ISNULL(SUM(f.E_Whenlong),0) from [dbo].tb_HB_Examination f left join  [dbo].tb_ExaminationResult e on(f.EId=e.ER_EId) where  e.ER_MId=a.UId and ER_Type in(1))" + "+";
                lenstr += "(select ISNULL(SUM(f.LongTime),0) from [dbo].tb_Manual_Examination f left join  [dbo].tb_ExaminationResult e on(f.ID=e.ER_EId) where  e.ER_MId=a.UId and ER_Type in(2))" + "+";
                lenstr += "(select ISNULL(SUM(f.ExaminationLength),0) from [dbo].[tb_FH_Examination] f left join  [dbo].tb_ExaminationResult e on(f.Id=e.ER_EId) where  e.ER_MId=a.UId and ER_Type in(3))" + "+";
                lenstr += "(select ISNULL(SUM(f.TimeLong),0) from [dbo].tb_Bill_Exam f left join  [dbo].tb_ExaminationResult e on(f.ID=e.ER_EId) where  e.ER_MId=a.UId and ER_Type in(4))";

                
            }
            //平均时长  =  赛项时长/练习次数
            lenstr = "( case when (" + lenstr + ")>0 then ((" + lenstr + ") / " + lenstrs + ") else 0 end ) as LongTime";

            if (Type == 2)//通过率
            {
                lenstr = " 0 as LongTime";

                sqlScore = "";

                if (!string.IsNullOrEmpty(chkstr))
                {
                    var zu = chkstr.Split(',');
                    if (zu.Length > 0)
                    {
                        for (int i = 0; i < zu.Length; i++)
                        {
                            if (zu[i] == "1")
                            {
                                sqlScore += "CONVERT (decimal(19,2),(select COUNT(1) from tb_ExaminationResult e where  e.ER_MId=a.UId and   ER_Type=1 and (case when E.ER_Score>0 then (E.ER_Score/(select ISNULL(SUM(t.EP_Score),0) from tb_HB_ExaminationPapers t where t.EP_PId=e.ER_PId)) else 0 end)>=0.6))" + "+";
                            }
                            else if (zu[i] == "2")
                            {
                                sqlScore += "CONVERT (decimal(19,2),(select COUNT(1) from tb_ExaminationResult e where  e.ER_MId=a.UId and  ER_Type=2 and (case when E.ER_Score>0 then (E.ER_Score/(select ISNULL(SUM(t.Score),0) from tb_Manual_Counting t where t.ID=e.ER_PId)) else 0 end)>=0.6))" + "+";
                            }
                            else if (zu[i] == "3")
                            {
                                sqlScore += "CONVERT (decimal(19,2),(select COUNT(1) from tb_ExaminationResult e where  e.ER_MId=a.UId and ER_Type=3 and (case when E.ER_Score>0 then (E.ER_Score/(select ISNULL(SUM(t.TestsFraction),0) from tb_FH_RelationTopic t where t.TestPaperID=e.ER_PId)) else 0 end)>=0.6))" + "+";
                            }
                            else
                            {
                                sqlScore += "CONVERT (decimal(19,2),(select COUNT(1) from tb_ExaminationResult e where e.ER_MId=a.UId and  ER_Type=4 and (case when E.ER_Score>0 then (E.ER_Score/(select ISNULL(SUM(t.Score),0) from tb_Bill_TestPaperEven t where t.PaperID=e.ER_PId)) else 0 end)>=0.6))" + "+";
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sqlScore))
                {
                    sqlScore = sqlScore.Substring(0, sqlScore.Length - 1);

                }
                else
                {
                    sqlScore += "CONVERT (decimal(19,2),(select COUNT(1) from tb_ExaminationResult e where  e.ER_MId=a.UId and   ER_Type=1 and (case when E.ER_Score>0 then (E.ER_Score/(select ISNULL(SUM(t.EP_Score),0) from tb_HB_ExaminationPapers t where t.EP_PId=e.ER_PId)) else 0 end)>=0.6))" + "+";
                    sqlScore += "CONVERT (decimal(19,2),(select COUNT(1) from tb_ExaminationResult e where  e.ER_MId=a.UId and  ER_Type=2 and (case when E.ER_Score>0 then (E.ER_Score/(select ISNULL(SUM(t.Score),0) from tb_Manual_Counting t where t.ID=e.ER_PId)) else 0 end)>=0.6))" + "+";
                    sqlScore += "CONVERT (decimal(19,2),(select COUNT(1) from tb_ExaminationResult e where  e.ER_MId=a.UId and ER_Type=3 and (case when E.ER_Score>0 then (E.ER_Score/(select ISNULL(SUM(t.TestsFraction),0) from tb_FH_RelationTopic t where t.TestPaperID=e.ER_PId)) else 0 end)>=0.6))" + "+";
                    sqlScore += "CONVERT (decimal(19,2),(select COUNT(1) from tb_ExaminationResult e where e.ER_MId=a.UId and  ER_Type=4 and (case when E.ER_Score>0 then (E.ER_Score/(select ISNULL(SUM(t.Score),0) from tb_Bill_TestPaperEven t where t.PaperID=e.ER_PId)) else 0 end)>=0.6))";
                }

                sqlScore = "( case when (" + sqlScore + ")>0 then (CONVERT (decimal(19,2),(" + sqlScore + ")) / CONVERT (decimal(19,2)," + lenstrs + "))*100 else 0 end ) as Result";

            }
            
            //分数区间------------------只有排序能在最外层
            
                wheres += " and Result between " + MinTxt + " and " + MaxTxt;

                string list = "*";
                string table = @"(
select Name,TeamName,b.Custom2,UserNo,UserName," + lenstr + "," + sqlScore + @" from tb_UserInfo a 
inner join tb_Team b on b.Id=a.UserClassId
inner join tb_School c on c.Id=b.SchoolId
where 1=1  " + whrersTo + ") t";

                DataTable dt = commonbll.GetListDatatable(list, table, wheres + " order by UserNo ");//加排序 
                return JsonConvert.SerializeObject(dt);
        }

    }
}
