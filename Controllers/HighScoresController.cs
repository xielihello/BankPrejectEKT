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

namespace VocationalProject.Controllers
{
    /***************************************************************
   FileName:学生端 分数排行榜
   Copyright（c）2017-金融教育在线技术开发部
   Author:袁学
   Create Date:2017-4-24
   ******************************************************************/
    public class HighScoresController : BaseController
    {
        //
        // GET: /HighScores/
        CommonBll commonbll = new CommonBll();

        public ActionResult Index()
        {
            return View();
        }

        public string GetPH()
        {

            var EId = SqlHelper.ExecuteSclar("select top 1 ER_EId from tb_ExaminationResult a inner join tb_HB_Examination c on c.EId=a.ER_EId where E_Type=1 and E_IsState=1 and ER_Type=1 and ER_State=0 and ER_MId=" + UId + "  order by ER_AddTime desc");

            //排名 姓名  用时 分值 

            DataTable dt = SqlHelper.ExecuteDataTable(@"select top 10 EId,E_Name,ER_Score,UserNo,UserName,ER_Custom3,
datediff(s,'00:00:00',ER_Custom3) as timescore
 from tb_ExaminationResult a 
inner join tb_UserInfo b on a.ER_MId=b.UId
inner join tb_HB_Examination c on c.EId=a.ER_EId
where  EId=" + EId + " order by ER_Score desc,ER_Custom3 asc");//ER_Type=1 and ER_State=0 and E_Type=1 and

            StringBuilder sb = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                sb.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
                sb.Append("<thead><tr   style=\"height:30px; line-height:30px; text-align:center;font-weight:800;\"><td width=\"25%\">排名</td><td width=\"25%\">姓名</td><td width=\"25%\">用时</td><td width=\"25%\">分值</td></tr></thead>");
                sb.Append("<tbody>");
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    sb.Append("<tr><td width=\"25%\"><div class=\"td_right\"><span class=\"rank_num\">" + (i + 1) + "</span> </div> </td>");

                    if (dt.Rows[i]["UserName"] == null || dt.Rows[i]["UserName"].ToString().Length == 0)
                    {
                        sb.Append("<td width=\"25%\"><div class=\"td_right\">" + dt.Rows[i]["UserNo"] + "</div></td>");
                    }
                    else
                    {
                        sb.Append("<td width=\"25%\"><div class=\"td_right\">" + dt.Rows[i]["UserName"] + "</div></td>");
                    }

                    sb.Append(" <td width=\"25%\"><div class=\"td_right\">" + dt.Rows[i]["ER_Custom3"] + "</div></td>");
                    sb.Append(" <td width=\"25%\"><div >" + dt.Rows[i]["ER_Score"] + "</div>");
                }
                sb.Append("</tbody>");
            }
            return sb.ToString();

        }
        /// <summary>
        /// 获取排行榜数据  除去手工的 其他三项 仅仅类型不同
        /// </summary>
        /// <returns></returns>
        public string GetPH_1()
        {
            var Type = Request["Type"];
            //
            string Typetxt = "";
            if (Type == "1")
            {
                Typetxt = "理论知识";
            }
            if (Type == "2")
            {
                Typetxt = "手工点钞";
            }
            if (Type == "3")
            {
                Typetxt = "复核报表";
            }
            if (Type == "4")
            {
                Typetxt = "单据录入";
            }
            //1.
            var list = @"*, convert(varchar(8),dateadd(ss,timescore,108),108) as ER_Custom3";
            var table = @"(
select ER_MId,UserName,UserNo,StudentNo,sum(ER_Score) as ER_Score,sum(datediff(s,'00:00:00',ER_Custom3)) as timescore
from tb_ExaminationResult a inner join tb_UserInfo b on a.ER_MId=b.UId
where 1=1 and ER_Type=" + Type + " and ER_State=0 and UserClassId=" + UserClassId + " group by  ER_MId,UserName,UserNo,StudentNo) t";
            var where = " ";
            var order = " order by ER_Score desc,timescore asc,StudentNo desc";
            //排序 分值 ， 用时 ，学号
            DataTable dt = commonbll.GetListDatatable(list, table, where + order);
            StringBuilder sb = new StringBuilder();


            if (dt.Rows.Count > 0)
            {
                sb.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
                var myidx = -1;
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    if (i < 10)//0,1,2
                    {
                        sb.Append("<tr><td width=\"50\"><span class=\"rank_num\">" + (i + 1) + "</span>  </td>");

                        if (dt.Rows[i]["UserName"] == null || dt.Rows[i]["UserName"].ToString().Length == 0)
                        {
                            sb.Append("<td width=\"100\"><div class=\"td_right\">" + dt.Rows[i]["UserNo"] + "</div></td>");
                        }
                        else
                        {
                            sb.Append("<td width=\"100\"><div class=\"td_right\">" + dt.Rows[i]["UserName"] + "</div></td>");
                        }

                        sb.Append(" <td><div class=\"td_right\">练习时长：" + dt.Rows[i]["ER_Custom3"] + "</div></td>");
                        sb.Append(" <td><div class=\"td_right\">总分：" + dt.Rows[i]["ER_Score"] + "</div>");
                        //最高分得重新计算 每个考试  最高分
                        string sql = @" select sum(ER_Score) from (
 select MAX(ER_Score) as ER_Score from tb_ExaminationResult  where ER_Type=" + Type + " and ER_MId=" + dt.Rows[i]["ER_MId"] + " group by ER_EId) t";
                        var lsMinScore = SqlHelper.ExecuteSclar(sql).ToString();
                        sb.Append("</td><td>历史最高分：" + lsMinScore + "</td></tr>");
                    }

                    //找出我的下标 当前考试在的位置
                    if (dt.Rows[i]["ER_MId"].ToString() == UId)
                    {
                        myidx = i;
                    }

                }
                sb.Append(" </table>");
                sb.Append(" <div class=\"rank_text\">");

                if (myidx == -1)
                {
                    sb.Append("您暂无做题记录，暂无排行！");

                }
                else
                {
                    if (myidx == 0)//第一名
                    {
                        sb.Append("恭喜您！您的<span class=\"text_red\">" + Typetxt + "</span>排名为第<span class=\"text_red\">1</span>名，");
                        sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                        sb.Append("距离上一名还差<span class=\"text_red\">0</span>分！");
                    }
                    else
                    {
                        //上一名得分-当前得分
                        var SYMScore = Convert.ToDecimal(dt.Rows[myidx - 1]["ER_Score"]) - Convert.ToDecimal(dt.Rows[myidx]["ER_Score"]);
                        if (SYMScore != 0)//得分与上一名不相同
                        {
                            sb.Append("您的<span class=\"text_red\">" + Typetxt + "</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
                            sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");

                            sb.Append("距离上一名还差<span class=\"text_red\">" + SYMScore + "</span>分！  继续加油！");

                        }
                        else
                        {
                            //得分相同，校验 时长
                            //如果时长不同
                            if (dt.Rows[myidx - 1]["ER_Custom3"].ToString() != dt.Rows[myidx]["ER_Custom3"].ToString())
                            {
                                DateTime startime = Convert.ToDateTime(dt.Rows[myidx - 1]["ER_Custom3"].ToString());//时间小 y他排在前面呢
                                DateTime endtime = Convert.ToDateTime(dt.Rows[myidx]["ER_Custom3"].ToString());//用时多
                                TimeSpan ts = endtime - startime;
                                sb.Append("您的<span class=\"text_red\">" + Typetxt + "</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
                                sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                                var whenlong = "00:00:00";
                                if (ts.Days > 0)
                                {
                                    var hh = ts.TotalHours + "";
                                    var mm = ts.Minutes + "";
                                    var ss = ts.Seconds + "";
                                    if (ts.TotalHours < 10)
                                    {
                                        hh = "0" + ts.TotalHours;
                                    }
                                    if (ts.Minutes < 10)
                                    {
                                        mm = "0" + ts.Minutes;
                                    }
                                    if (ts.Seconds < 10)
                                    {
                                        ss = "0" + ts.Seconds;
                                    }

                                    whenlong = hh + ":" + mm + ":" + ss;

                                }
                                else
                                {
                                    var hh = ts.Hours + "";
                                    var mm = ts.Minutes + "";
                                    var ss = ts.Seconds + "";
                                    if (ts.Hours < 10)
                                    {
                                        hh = "0" + ts.Hours;
                                    }
                                    if (ts.Minutes < 10)
                                    {
                                        mm = "0" + ts.Minutes;
                                    }
                                    if (ts.Seconds < 10)
                                    {
                                        ss = "0" + ts.Seconds;
                                    }

                                    whenlong = hh + ":" + mm + ":" + ss;
                                }
                                sb.Append("距离上一名还差<span class=\"text_red\">0</span>分,时长差距 " + whenlong + " ！  继续加油！");
                            }
                            else
                            {
                                //得分相同，时长相同的：跳过排名相同的学生，直接与上一名（不同得分或时长）的学生对比。
                                if (myidx > 1)//我至少得在第三名以后才能跳过上一名 myidx - 2
                                {
                                    //先校验得分
                                    //上一名得分-当前得分
                                    var SYMScoreTo = Convert.ToDecimal(dt.Rows[myidx - 2]["ER_Score"]) - Convert.ToDecimal(dt.Rows[myidx]["ER_Score"]);
                                    if (SYMScoreTo != 0)//得分与上一名不相同
                                    {
                                        sb.Append("您的<span class=\"text_red\">" + Typetxt + "</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
                                        sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");

                                        sb.Append("距离上一名还差<span class=\"text_red\">" + SYMScoreTo + "</span>分！  继续加油！");

                                    }
                                    else
                                    {
                                        if (dt.Rows[myidx - 2]["ER_Custom3"].ToString() != dt.Rows[myidx]["ER_Custom3"].ToString())
                                        {
                                            DateTime startime = Convert.ToDateTime(dt.Rows[myidx - 2]["ER_Custom3"].ToString());//时间小 y他排在前面呢
                                            DateTime endtime = Convert.ToDateTime(dt.Rows[myidx]["ER_Custom3"].ToString());//用时多
                                            TimeSpan ts = endtime - startime;
                                            sb.Append("您的<span class=\"text_red\">" + Typetxt + "</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
                                            sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                                            var whenlong = "00:00:00";
                                            if (ts.Days > 0)
                                            {
                                                var hh = ts.TotalHours + "";
                                                var mm = ts.Minutes + "";
                                                var ss = ts.Seconds + "";
                                                if (ts.TotalHours < 10)
                                                {
                                                    hh = "0" + ts.TotalHours;
                                                }
                                                if (ts.Minutes < 10)
                                                {
                                                    mm = "0" + ts.Minutes;
                                                }
                                                if (ts.Seconds < 10)
                                                {
                                                    ss = "0" + ts.Seconds;
                                                }

                                                whenlong = hh + ":" + mm + ":" + ss;

                                            }
                                            else
                                            {
                                                var hh = ts.Hours + "";
                                                var mm = ts.Minutes + "";
                                                var ss = ts.Seconds + "";
                                                if (ts.Hours < 10)
                                                {
                                                    hh = "0" + ts.Hours;
                                                }
                                                if (ts.Minutes < 10)
                                                {
                                                    mm = "0" + ts.Minutes;
                                                }
                                                if (ts.Seconds < 10)
                                                {
                                                    ss = "0" + ts.Seconds;
                                                }

                                                whenlong = hh + ":" + mm + ":" + ss;
                                            }
                                            sb.Append("距离上一名还差<span class=\"text_red\">0</span>分,时长差距 " + whenlong + " ！  继续加油！");
                                        }
                                    }
                                }
                                else
                                {
                                    //否则我就是第二名和以第一名 显示一样的
                                    sb.Append("您的<span class=\"text_red\">" + Typetxt + "</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
                                    sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                                    sb.Append("距离上一名还差<span class=\"text_red\">0</span>分！");

                                }

                            }

                        }
                    }
                }

                sb.Append("</div>");

            }
            else
            {
                //没数据
                sb.Append(" <div class=\"rank_text\">");
                sb.Append("当前竞赛暂无排行！");
                sb.Append("</div>");
            }

            return sb.ToString();

        }



    }
}
