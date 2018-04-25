using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VocationalProject.App_Start;
using VocationalProject.Areas.Admin.Models;
using VocationalProject.Models;
using VocationalProject_Bll;
using VocationalProject_Dal;
using VocationalProject_DBUtility.Sql;

namespace VocationalProject.Controllers
{
    /***************************************************************
    FileName:复核报表 学生端竞赛列表
    Copyright（c）2017-金融教育在线技术开发部
    Author:唐
    Create Date:2017-4-14
    ******************************************************************/
    public class FH_ManagementController : BaseController
    {
        //
        // GET: /FH_Management/

        CommonBll commonbll = new CommonBll();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 未完成竞赛列表ExaminationType
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList()
        {
            string wheres = " and Isactivation=1 and (Spare2 like '%," + UserClassId + ",%' or Spare2='') and Id not in (select ER_EId from tb_ExaminationResult where ER_Type=3 and ER_MId=" + UId + ")";

            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")//竞赛模式
            {
                wheres += " and ExaminationType='" + Request["E_Type"].ToString() + "'";
            }

            //竞赛名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {
                wheres += " and ExaminationName like '%" + Request["E_Name"] + "%'";
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "Id desc,ExaminationEndTime desc"; //排序必须填写
            m.strFld = "  a.*,(select SUM(t.TestsFraction) from tb_FH_RelationTopic t where t.TestPaperID=a.TestPaperID) as Score";
            m.tab = "tb_FH_Examination a";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);

            StringBuilder sb = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    //根据时间判断
                    //1.开始时间未到，显示未开始
                    var dqshijian = DateTime.Now;
                    var ksshishijian = Convert.ToDateTime(dt.Rows[i]["ExaminationStartTime"].ToString());//有效开始时间
                    var jsshishijian = Convert.ToDateTime(dt.Rows[i]["ExaminationEndTime"].ToString());//有效结束时间

                    sb.Append("<div class=\"item_list_line\">");

                    if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                    {
                        sb.Append("<img src=\"../img/student/icoend.jpg\" />");
                    }
                    else
                    {
                        sb.Append("<img src=\"../img/student/iconew.jpg\" />");
                    }
                    sb.Append("<div class=\"list_title\">");

                 
                    sb.Append("<span>" + dt.Rows[i]["ExaminationName"] + "</span>");

                    if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                    }

                    if (dqshijian.AddMinutes(10) >= ksshishijian && dqshijian <= jsshishijian)
                    {
                        sb.Append("<button class=\"btn_go\"  onclick=\"Getinto(" + dt.Rows[i]["Id"] + "," + dt.Rows[i]["TestPaperID"] + ")\" type=\"button\">进入</button>");
                    }

                    if (dqshijian.AddMinutes(10) < ksshishijian)//当前时间 小于考试开始时间 考试HIA没有开始 
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">未开始</button>");
                    }

                    sb.Append("</div>");

                    sb.Append(" <div class=\"list_title_smal\">");

                    if (dt.Rows[i]["ExaminationType"].ToString() == "2")//考试模式
                    {
                        sb.Append("<label>试卷类型：考试模式</label>");
                    }
                    else
                    {
                        sb.Append("<label>试卷类型：练习模式</label>");
                    }


                    sb.Append("<label>试卷总分：" + dt.Rows[i]["Score"] + "分</label>");
                    sb.Append("<label>试卷有效时间：" + dt.Rows[i]["ExaminationStartTime"].ToString() + " —— " + dt.Rows[i]["ExaminationEndTime"].ToString() + "  </label>");

                    sb.Append("</div>");
                    sb.Append("</div>");
                }
            }

            return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, sb.ToString());
        }
        /// <summary>
        /// 已完成竞赛列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetListTo()
        {
            string wheres = " and Isactivation=1  and (Spare2 like '%," + UserClassId + ",%' or Spare2='')  and Id  in (select ER_EId from tb_ExaminationResult where ER_Type=3 and ER_MId=" + UId + ")";

            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")//竞赛模式
            {
                wheres += " and ExaminationType='" + Request["E_Type"].ToString() + "'";
            }

            //竞赛名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {
                wheres += " and ExaminationName like '%" + Request["E_Name"] + "%'";
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
           
            m.Sort = "ExaminationType,ExaminationEndTime desc";
            m.strFld = @" a.*,(select SUM(t.TestsFraction) from tb_FH_RelationTopic t where t.TestPaperID=a.TestPaperID) as Score,
(select top 1 ER_Score from tb_ExaminationResult where ER_MId=" + UId + @" and ER_EId=a.Id and ER_Type=3 and ER_State=0 order by ERId desc) as CZScore,
 (select top 1 ER_AddTime from tb_ExaminationResult where ER_MId=" + UId + @" and ER_EId=a.Id and ER_Type=3 and ER_State=0  order by ERId desc) as ER_AddTime";
            m.tab = "tb_FH_Examination a";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);

            StringBuilder sb = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    //根据时间判断
                    //1.开始时间未到，显示未开始
                    var dqshijian = DateTime.Now;
                    var ksshishijian = Convert.ToDateTime(dt.Rows[i]["ExaminationStartTime"].ToString());//有效开始时间
                    var jsshishijian = Convert.ToDateTime(dt.Rows[i]["ExaminationEndTime"].ToString());//有效结束时间

                    sb.Append("<div class=\"item_list_line\">");
                    string img = "";
                    if (dt.Rows[i]["ExaminationType"].ToString() == "2")//考试模式 直接已结束
                    {
                        img = "<img src=\"../img/student/icoend.jpg\" />";
                    }
                    else
                    {
                        img = "<img src=\"../img/student/iconew.jpg\" />"; 
                    }
                    if (dt.Rows[i]["ExaminationType"].ToString() == "1")//练习模式
                    {
                        if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                        {
                            img = "<img src=\"../img/student/icoend.jpg\" />";
                        }
                        else
                        {
                            img = "<img src=\"../img/student/iconew.jpg\" />"; 
                        }
                    }
                    sb.Append(img);

                    sb.Append("<div class=\"list_title\">");

                    sb.Append("<a href=\"#chart-personPK\" onclick='ViewGrowthTrajectory(" + dt.Rows[i]["Id"] + ")' >" + dt.Rows[i]["ExaminationName"] + "</a>");

                    if (dt.Rows[i]["ExaminationType"].ToString() == "2")//考试模式 直接已结束
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                    }
                    if (dt.Rows[i]["ExaminationType"].ToString() == "1")//练习模式
                    {
                        if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                        {
                            sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                        }

                        if (dqshijian >= ksshishijian && dqshijian <= jsshishijian)
                        {
                            sb.Append("<button class=\"btn_ph\" onclick='PH(" + dt.Rows[i]["Id"] + "," + dt.Rows[i]["TestPaperId"] + ")' type=\"button\">排行榜</button>");
                            sb.Append("<button class=\"btn_look\" onclick='Viewdetails(" + dt.Rows[i]["Id"] + "," + dt.Rows[i]["TestPaperId"] + ")' type=\"button\">查看明细</button>");
                            sb.Append("<button class=\"btn_go2\" onclick=\"Getinto(" + dt.Rows[i]["Id"] + "," + dt.Rows[i]["TestPaperId"] + ")\" type=\"button\">进入</button>");
                        }

                    }
                    sb.Append("</div>");
                    sb.Append(" <div class=\"list_title_smal\">");
                    if (dt.Rows[i]["ExaminationType"].ToString() == "2")//考试模式
                    {
                        sb.Append("<label>试卷类型：考试模式</label>");
                    }
                    else
                    {
                        sb.Append("<label>试卷类型：练习模式</label>");
                    }
                    sb.Append("<label>试卷总分：" + dt.Rows[i]["Score"] + "分</label>");
                    sb.Append("<label>操作得分：" + dt.Rows[i]["CZScore"] + "分</label>");
                    sb.Append("<label>提交时间：" + dt.Rows[i]["ER_AddTime"].ToString() + "</label>");
                    sb.Append("<label>试卷有效时间：" + ksshishijian + " —— " + jsshishijian + "  </label>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                }
            }

            return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, sb.ToString());
        }


        /// <summary>
        /// 计算前 N个日 考试做题情况
        /// </summary>
        /// <returns></returns>
        public string GeViewGrowthTrajectory()
        {
            string Eid = Request["Eid"];
            string datet = Request["datet"];
            var datetlist = datet.Split(',');
            string scoresstr = "";//拼接成绩字符串
            if (datetlist.Length > 0)
            {
                for (int i = 0; i < datetlist.Length; i++)
                {
                    if (datetlist[i].Length > 0)
                    {
                        //查询学生当天做题情况 次数的平均分
                        DataTable dt = commonbll.GetListDatatable("SUM(ER_Score) as ER_Score,count(ER_Score) as num", "tb_ExaminationResult", " and ER_Type=3 and ER_EId=" + Eid + " and ER_MId=" + UId + " and datediff(day,ER_AddTime,'" + datetlist[i] + "')=0");
                        if (dt.Rows.Count > 0)
                        {
                            int count = Convert.ToInt32(dt.Rows[0]["num"]);//当天做题次数
                            if (count > 0)
                            {
                                double score = Convert.ToDouble(dt.Rows[0]["ER_Score"]);//当天做题总分数

                                //计算平均分
                                scoresstr += (score / count) + ",";
                            }
                            else
                            {
                                //不存在做题记录
                                scoresstr += "0,";
                            }

                        }
                        else
                        {
                            //不存在做题记录
                            scoresstr += "0,";
                        }
                    }

                }
            }
            //
            //select E_Name from tb_HB_Examination where EId=ER_EId
            string EName = commonbll.GetListSclar("ExaminationName", "tb_FH_Examination", " and Id=" + Eid);

            var json = new object[] {
                        new{
                            scoresstr=scoresstr,
                            EName=EName
                        }
                    };
            return JsonConvert.SerializeObject(json);
        }


        /// <summary>
        /// 单据录入 单项赛项排行
        /// </summary>
        /// <returns></returns>
        public string GetPH()
        {

            var Eid = Request["Eid"];
            var Pid = Request["Pid"];

            //1.找出我班级下所有的数据
            var list = @"ERId,ER_EId,ER_PId,ER_MId,
(select  top 1  ExaminationName from tb_FH_Examination where TestPaperId=a.ER_EId) as E_Name,
UserName,UserNo,StudentNo,ER_Custom3,ER_Score,
(select MAX(ER_Score) from tb_ExaminationResult where ER_Type=a.ER_Type and ER_EId=a.ER_EId  and ER_PId=a.ER_PId  and ER_MId=a.ER_MId) as lsMinScore";
            var table = "tb_ExaminationResult a inner join tb_UserInfo b on a.ER_MId=b.UId";
            var where = " and ER_Type=3 and ER_State=0 and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and UserClassId=" + UserClassId;
            var order = " order by ER_Score desc,ER_Custom3 asc,StudentNo desc";
            //排序 分值 ， 用时 ，学号
            DataTable dt = commonbll.GetListDatatable(list, table, where + order);
            StringBuilder sb = new StringBuilder();


            if (dt.Rows.Count > 0)
            {
                sb.Append(" <div class=\"tc_rank_title\">" + dt.Rows[0]["E_Name"] + "</div>");

                sb.Append("<div class=\"rank_content\" style=\"min-height: 300px;\">");
                sb.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
                var myidx = 0;
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    if (i < 3)//0,1,2
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
                        sb.Append(" <td><div class=\"td_right\">得分：" + dt.Rows[i]["ER_Score"] + "</div>");
                        sb.Append("</td><td>历史最高分：" + dt.Rows[i]["lsMinScore"] + "</td></tr>");
                    }

                    //找出我的下标 当前考试在的位置
                    if (dt.Rows[i]["ER_MId"].ToString() == UId)
                    {
                        myidx = i;
                    }

                }
                sb.Append(" </table>");
                sb.Append(" <div class=\"rank_text\">");


                if (myidx == 0)//第一名
                {
                    sb.Append("恭喜您！您的<span class=\"text_red\">复核报表</span>排名为第<span class=\"text_red\">1</span>名，");
                    sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                    sb.Append("距离上一名还差<span class=\"text_red\">0</span>分！");
                }
                else
                {
                    //上一名得分-当前得分
                    var SYMScore = Convert.ToDecimal(dt.Rows[myidx - 1]["ER_Score"]) - Convert.ToDecimal(dt.Rows[myidx]["ER_Score"]);
                    if (SYMScore != 0)//得分与上一名不相同
                    {
                        sb.Append("您的<span class=\"text_red\">复核报表</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                            sb.Append("您的<span class=\"text_red\">复核报表</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                    sb.Append("您的<span class=\"text_red\">复核报表</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                        sb.Append("您的<span class=\"text_red\">复核报表</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                sb.Append("您的<span class=\"text_red\">复核报表</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
                                sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                                sb.Append("距离上一名还差<span class=\"text_red\">0</span>分！");

                            }

                        }

                    }

                }

                sb.Append("</div>");

                sb.Append("</div>");

            }




            return sb.ToString();
        }






        /// <summary>
        /// 倒计时 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult TimeIndex()
        {
            HBKaoShiModel km = new HBKaoShiModel();
            var Eid = Request["Eid"];
            var Pid = Request["Pid"];
            if (Eid != null && Pid != null && Eid.Length > 0 && Pid.Length > 0)
            {
                DataTable dt = commonbll.GetListDatatable("*", "tb_FH_Examination", " and Id=" + Eid);
                if (dt.Rows.Count > 0)
                {
                    km.AddStartDateTime = Convert.ToDateTime(dt.Rows[0]["ExaminationStartTime"]).ToString("yyyy-MM-dd HH:mm:01");
                }
                km.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            }
            return View(km);
        }



        /// <summary>
        /// 做题页
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamPreview()
        {

            var Eid = Request["Eid"];
            var Pid = Request["Pid"];
            var m = new FHKSModel();
            if (!String.IsNullOrEmpty(Eid) && !String.IsNullOrEmpty(Pid))
            {
                string list = @"e.Id,e.IsPlus,e.TestPaperId,e.ExaminationName,e.ExaminationStartTime,e.ExaminationEndTime,e.ExaminationLength,e.ExaminationType,
(select SUM(t.TestsFraction) from tb_FH_RelationTopic t where t.TestPaperID=e.TestPaperId) as Score,
(select Name from tb_School a inner join tb_Team b on b.[SchoolId]=a.Id  where b.Id=" + UserClassId + ") as SchoolName";//列
                DataTable tb = commonbll.GetListDatatable(list, "tb_FH_Examination e", " and e.Id=" + Eid);
                if (tb != null && tb.Rows.Count > 0)
                {
                    m.E_Name = tb.Rows[0]["ExaminationName"].ToString();//竞赛名称 IsPlus
                    m.Score = tb.Rows[0]["Score"].ToString();//分值
                    m.UserName = Base_UserInfo.UserName;//姓名
                    m.UserPic = Base_UserInfo.UserPic;//头像
                    m.SchoolName = tb.Rows[0]["SchoolName"].ToString(); ;//学校
                    m.IsPlus = tb.Rows[0]["IsPlus"].ToString();
                    m.E_StartTime = Convert.ToDateTime(tb.Rows[0]["ExaminationStartTime"]).ToString("yyyy-MM-dd HH:mm:01");
                    m.E_EndTime = Convert.ToDateTime(tb.Rows[0]["ExaminationEndTime"]).ToString("yyyy-MM-dd HH:mm:01");
                    m.E_Whenlong = tb.Rows[0]["ExaminationLength"].ToString();//竞赛时长
                    m.E_Type = tb.Rows[0]["ExaminationType"].ToString();//竞赛类型
                    //考试模式下是否已经做过了 直接返回到
                    if (m.E_Type == "1")//考试模式
                    {
                        var resultcount = commonbll.GetRecordCount("tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=3 and ER_State=0");
                        if (resultcount > 0)
                        {
                            m.Isallow = "1";
                        }
                        else
                        {
                            m.Isallow = "0";
                        }

                    }
                    DateTime E_StartTime = Convert.ToDateTime(m.E_StartTime);//有效开始时间

                    if (E_StartTime > DateTime.Now)//有效开始时间大于 说明是倒计时
                    {
                        TimeSpan ts = E_StartTime - DateTime.Now;
                        int fz = ts.Minutes + 1;//分差
                        int mz = ts.Seconds;//秒差
                        DateTime d = DateTime.Now.AddMinutes(fz).AddSeconds(mz);//在当前时间上加上这个秒钟

                        //当前进入时间 算开始 开始时间
                        m.AddStartDateTime = d.ToString("yyyy-MM-dd HH:mm:01");
                        //考试结束时间
                        m.TestStartDateTime = d.AddMinutes(int.Parse(m.E_Whenlong)).ToString("yyyy-MM-dd HH:mm:01");

                    }
                    else
                    {
                        //计算 第一次进入剩余倒计时 2017-04-12 15:05:00
                        //根据这个倒计时和当前时间比较得到处 还相差几分钟，得出差值

                        DataTable timeDt = commonbll.GetListDatatable("CD_Time", "tb_CountDown", "  and CD_Custom3=3 and CD_EId=" + Eid + " and CD_PId=" + Pid + " and CD_MId=" + UId);
                        var shoucitime = "2017-04-01";
                        if (timeDt.Rows.Count > 0)
                        {
                            shoucitime = timeDt.Rows[0]["CD_Time"].ToString();
                        }

                        DateTime shouciDtime = Convert.ToDateTime(shoucitime);//记载时间表

                        //不存在时差 计算
                        //那么开始时间就是
                        m.AddStartDateTime = shouciDtime.ToString("yyyy-MM-dd HH:mm:ss");
                        //时间记载表上加时长
                        m.TestStartDateTime = shouciDtime.AddMinutes(int.Parse(m.E_Whenlong)).ToString("yyyy-MM-dd HH:mm:ss");


                    }

                    m.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间

                    var data = commonbll.GetListDatatable("*,(select top 1 Title from tb_FH_Topic where Id=TopicId) as Title,(select COUNT(1) from tb_ExaminationDetails where ED_MId="+UId+" and ED_Type=3 and ED_State=0 and ED_QBId=TopicId and ED_PId=TestPaperID) as siz", "tb_FH_RelationTopic", " and TestPaperID=" + Pid);
                    ViewData["RelationTopicTable"] = data;
                }

            }
            return View(m);
        }


        /// <summary>
        /// 获取已完成题目的。
        /// </summary>
        /// <returns></returns>
        public string GetModelById()
        {

            var Eid = Request["Eid"];//考试ID
            var Pid = Request["Pid"];//试卷ID
            var QBId = Request["QBId"];//试题ID
            var tb = commonbll.GetListDatatable("*", "tb_Operation_Answer", " and Type=3 and State=0 and EId=" + Eid + " and PId=" + Pid + " and QBId=" + QBId + " and MId=" + UId);
            

            return JsonConvert.SerializeObject(tb);
        }
        /// <summary>
        /// 明细获取已完成题目的。
        /// </summary>
        /// <returns></returns>
        public string GetModelsById()
        {

            var Eid = Request["Eid"];//考试ID
            var Pid = Request["Pid"];//试卷ID
            var QBId = Request["QBId"];//试题ID
            var MId = Request["MId"];//试题ID
            var tb = commonbll.GetListDatatable("*", "tb_Operation_Answer", " and Type=3 and Spare3='0' and State=1 and MId=" + MId + "  and EId=" + Eid + " and PId=" + Pid + " and QBId=" + QBId);


            return JsonConvert.SerializeObject(tb);
        }
        public string GetModelTestsId()
        {
            var Id = Request["Id"];//考试ID
            var tb = commonbll.GetListDatatable("*", "tb_FH_Tests", " and TopicId=" + Id);
            return JsonConvert.SerializeObject(tb);
        }

        public int AddExamination(string entityList, int Eid, int Pid, int QBId, string dctijiao)
        {
            //
            var TestsList = new List<FH_TestsModel>();
            try
            {
                TestsList = JsonHelper.JSONToObject<List<FH_TestsModel>>(HttpUtility.UrlDecode(entityList));
            }
            catch
            {
                return -1;
            }
            if (dctijiao == "2")
            {
                var checkcount = commonbll.GetRecordCount("tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=3 and ER_State=0");
                if (checkcount > 0)
                {
                    return 88;
                }
            }
            //题目
            var Tests = commonbll.GetListDatatable("*", "tb_FH_Tests", "and Line not in(15,29,30,41,46,47,52,53) and (FinalBalance is not null or BeginningBalance is not null) and TopicId=" + QBId);
            if (TestsList != null)
            {
                if (Tests != null && Tests.Rows.Count > 0)
                {
                    int Correct = 0;  //正确数量
                    int whole = 0;    //全部数量
                    StringBuilder str = new StringBuilder();
                    str.Append(" delete from tb_Operation_Answer where EId=" + Eid + " and PId=" + Pid + " and QBId=" + QBId + " and MId=" + UId + " and Type=3 and State=0 ");

                    string insert = "insert into tb_Operation_Answer(EId,PId,QBId,MId,Type,OperationAnswer,OkNo,RightAnswer,OperationTime,State,Spare1,Spare2,Spare3) ";
                    foreach (DataRow dic in Tests.Rows)
                    {
                        decimal FinalBalance = !string.IsNullOrEmpty(dic["FinalBalance"].ToString()) ? Convert.ToDecimal(dic["FinalBalance"].ToString()) : 0;
                        decimal BeginningBalance = !string.IsNullOrEmpty(dic["BeginningBalance"].ToString()) ? Convert.ToDecimal(dic["BeginningBalance"].ToString()) : 0; 
                        var Line = Convert.ToInt32(dic["Line"]);

                        foreach (var item in TestsList)
                        {
                            if (Line == item.Line)
                            {

                                decimal Beginning = decimal.Round(Convert.ToDecimal(item.Beginning), 2);
                                if (item.Beginning == null)
                                {
                                    Beginning = - 1;
                                }
                                decimal Final = decimal.Round(Convert.ToDecimal(item.Final), 2);
                                if (item.Final == null)
                                {
                                    Final = -1;
                                }
                                if (!string.IsNullOrEmpty(dic["BeginningBalance"].ToString()))  //必须出过题。
                                {
                                    str.Append(insert);
                                    whole++;
                                    string OkNo = "错误";
                                    if (Beginning == BeginningBalance)//期初 0
                                    {
                                        Correct++;
                                        OkNo = "正确";
                                    }
                                    str.Append("values(" + Eid + "," + Pid + "," + QBId + "," + UId + ",3,'" + (Beginning > 0 ? Beginning.ToString("0.00") : "") + "','" + OkNo + "','" + BeginningBalance + "','" + DateTime.Now + "',0," + Line + ",'0','0')");
                                    item.Isinsert1 = 1;
                                }
                                if (!string.IsNullOrEmpty(dic["FinalBalance"].ToString()))//必须出过题。 
                                {

                                    str.Append(insert);
                                    whole++;
                                    string OkNo = "错误";
                                    if (Final == FinalBalance)//期末 1
                                    {
                                        Correct++;
                                        OkNo = "正确";
                                    }
                                    str.Append("values(" + Eid + "," + Pid + "," + QBId + "," + UId + ",3,'" + (Final > 0 ? Final.ToString("0.00") : "") + "','" + OkNo + "','" + FinalBalance + "','" + DateTime.Now + "',0," + Line + ",'1','0')");
                                    item.Isinsert1 = 2;
                                }
                            }
                        }

                    }
                    foreach (var item in TestsList)
                    {
                        if (item.Line != 15 && item.Line != 29 && item.Line != 30 && item.Line != 41 && item.Line != 46 && item.Line != 47 && item.Line != 52 && item.Line != 53)
                        {
                            if (item.Beginning != null && item.Isinsert1 == null)
                            {
                                str.Append(insert);
                                str.Append("values(" + Eid + "," + Pid + "," + QBId + "," + UId + ",3,'" + Convert.ToDecimal(item.Beginning).ToString("0.00") + "','','','" + DateTime.Now + "',0," + item.Line + ",'0','1')");

                            }
                            if (item.Final != null && item.Isinsert2 == null)
                            {
                                str.Append(insert);
                                str.Append("values(" + Eid + "," + Pid + "," + QBId + "," + UId + ",3,'" + Convert.ToDecimal(item.Final).ToString("0.00") + "','','','" + DateTime.Now + "',0," + item.Line + ",'1','1')");

                            }
                        }
                    }
                    decimal Goal = 0;//分数
                    decimal Score = 0;//总分数
                    string isOkNo = "正确";
                    string TestsFraction = commonbll.GetListSclar("TestsFraction", "tb_FH_RelationTopic", " and TopicId=" + QBId + " and TestPaperID=" + Pid);
                    if (!string.IsNullOrEmpty(TestsFraction))
                    {
                        decimal Fraction = Convert.ToDecimal(TestsFraction);

                        Goal = Math.Round((Fraction / whole) * Correct,2);//Math.Round(Fraction, 2)
                    }
                    if (Correct != whole)
                    {
                        isOkNo = "错误";
                    }
                    //var Details = commonbll.GetListDatatable("*", "tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId=" + Pid + " and ED_QBId=" + QBId + " and ED_MId=" + UId + " and ED_Type=3 and ED_State=0");
                    str.Append("delete from tb_ExaminationDetails where ED_EId=" + Eid + " and ED_PId=" + Pid + " and ED_QBId=" + QBId + " and ED_MId=" + UId + " and ED_Type=3 and ED_State=0 ");
                    str.Append("insert into tb_ExaminationDetails(ED_EId,ED_PId,ED_QBId,ED_MId,ED_Type,ED_OkNo,ED_Goal,ED_State,ED_Operator,ED_AddTime,ED_Custom2) ");
                    str.Append("values (" + Eid + "," + Pid + "," + QBId + "," + UId + ",3,'" + isOkNo + "'," + Goal + ",0,'" + UId + "','" + DateTime.Now + "','" + Base_UserInfo.UserNo + "') ");
                    SqlHelper.ExecuteNonQuery(str.ToString());
 
                    //str = new StringBuilder();
                    //var Result = commonbll.GetListDatatable("*", "tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId=" + Pid + "  and ER_MId=" + UId + " and ER_Type=3 and ER_State=0");
                    //if (Result != null && Result.Rows.Count > 0)
                    //{
                    //    string socres = "(select SUM(ED_Goal) from tb_ExaminationDetails where ED_EId=" + Eid + " and ED_PId=" + Pid + " and ED_MId=" + UId + " and ED_Type=3 and ED_State=0) ";
                    //    str.Append("update tb_ExaminationResult set ER_Score=" + socres + " where ");
                    //    str.Append(" ERId=" + Result.Rows[0]["ERId"]);

                    //}
                    //else
                    //{
                    //    Score = Goal;
                    //    str.Append("insert into tb_ExaminationResult(ER_EId,ER_PId,ER_MId,ER_Type,ER_Score,ER_State,ER_Operator,ER_AddTime) ");
                    //    str.Append("values (" + Eid + "," + Pid + "," + UId + ",3," + Score + ",0,'" + UId + "','" + DateTime.Now + "') ");

                    //}

                    //SqlHelper.ExecuteNonQuery(str.ToString());
                    return 1;

                }
            }

            return 0;
        }






        /// <summary>
        /// 总提交按钮
        /// </summary>
        /// <returns></returns>
        public string AddExaminationResult()
        {

            string Pid = Request["Pid"];
            string Eid = Request["Eid"];

            string dctijiao = Request["dctijiao"];//竞赛模式 1 考试模式
            string E_IsTimeBonus = Request["E_IsTimeBonus"];//是否时间加分
            string P_Score = Request["P_Score"];//考试总分
            string E_Whenlong = Request["E_Whenlong"];//做题时长

            Session["Pid_M"] = Pid;
            Session["Eid_M"] = Eid;

            //校验是否提交过 仅校验考试模式下 不允许多次提交
            if (dctijiao == "2")
            {
                var checkcount = commonbll.GetRecordCount("tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=3 and ER_State=0");
                if (checkcount > 0)
                {
                    return "88";
                }
            }

            var resulutcount = commonbll.GetRecordCount("tb_CountDown", " and CD_EId=" + Eid + " and CD_PId=" + Pid + " and CD_MId=" + UId + "  and CD_Custom3=3");
            if (resulutcount == 0)
            {
                //如果在提交后在重复刷新 
                return "1";
            }

            try
            {



                //走总提交模式
                //1.先删除明细表 ED_State 状态=1 的明细
                //2.把所有为0的明细状态 修改为1
                //3.把所有明细分值累加 到总分表

                //4.把之前总分提交过的数据状态修改1
                //5.最新的总分数据 状态为0
                //6.删除时间记载表

                //1.
                commonbll.DeleteInfo("tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + "  and ED_Type=3 and ED_State=1  ");
                commonbll.DeleteInfo("tb_Operation_Answer", " and EId=" + Eid + " and PId='" + Pid + "' and MId=" + UId + "  and Type=3 and State=1  ");
                //2.  //3.
                var SumSocres = commonbll.GetListSclar("SUM(ED_Goal)", "tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + "  and ED_Type=3 and ED_State=0  ");

                
                SqlParameter[] parstResult = new SqlParameter[] { new SqlParameter("@ER_State", "1") };
                commonbll.UpdateInfo("tb_ExaminationResult", "ER_State=@ER_State", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + "  and ER_Type=3 and ER_State=0", parstResult);

                SqlParameter[] Answer = new SqlParameter[] { new SqlParameter("@State", "1") };
                commonbll.UpdateInfo("tb_Operation_Answer", "State=@State", " and EId=" + Eid + " and PId='" + Pid + "' and MId=" + UId + "  and Type=3 and State=0", Answer);
              
              

                //计算时间 做题时长
                var whenlong = "00:00:00";
                var Towhenlong = 0.00;//存储做题总时间总秒
                DataTable timeDt = commonbll.GetListDatatable("CD_Time", "tb_CountDown", "  and CD_Custom3=3 and CD_EId=" + Eid + " and CD_PId=" + Pid + " and CD_MId=" + UId);
                DateTime shoucitime = DateTime.Now;
                if (timeDt.Rows.Count > 0)
                {
                    shoucitime = Convert.ToDateTime(timeDt.Rows[0]["CD_Time"].ToString());
                }
                TimeSpan ts = DateTime.Now - shoucitime;
                Towhenlong = ts.TotalSeconds;
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
                    if (ts.TotalHours > 23)
                    {
                        hh = "23";
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

                //时间加分 如果试卷总分=做题总分
                //当前考试是否允许时间加分，每提前一秒+0.01分  P_Score
                var longsores = 0.00m;
                //允许时间加分并且全对(考试总分==做题总分)
                if (E_IsTimeBonus == "1" && Convert.ToDecimal(P_Score) == Convert.ToDecimal(SumSocres))
                {
                    DateTime statetime = DateTime.Now.AddSeconds(Towhenlong);//开始（结束）均已当前时间为准 总共做了这么久

                    DateTime endtime = DateTime.Now.AddMinutes(int.Parse(E_Whenlong));//当前时间 + 时长 为结束时间

                    //结束时间-开始时间 计算出本次还剩余多长时间
                    TimeSpan tsto = endtime - statetime;
                    if (tsto.TotalSeconds > 0)
                    {
                        longsores = Convert.ToDecimal(tsto.TotalSeconds) * 0.01m;//多少秒就多少分
                    }
                   

                }
                //4.
                    
                var str = new StringBuilder();
                var Result = commonbll.GetListDatatable("*", "tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId=" + Pid + "  and ER_MId=" + UId + " and ER_Type=3 and ER_State=0");

                //string socres = "(select ISNULL(SUM(ED_Goal),0)+" + Math.Round(longsores,2) + " from tb_ExaminationDetails where ED_EId=" + Eid + " and ED_PId=" + Pid + " and ED_MId=" + UId + " and ED_Type=3 and ED_State=0) ";

                var ED_Goal = commonbll.GetListDatatable(" ISNULL(SUM(ED_Goal),0) as TotalGoal ", "tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId=" + Pid + " and ED_MId=" + UId + " and ED_Type=3 and ED_State=0 ");
                decimal Goal = 0;
                if (ED_Goal != null && ED_Goal.Rows.Count > 0)
                {
                    Goal = Convert.ToDecimal(ED_Goal.Rows[0]["TotalGoal"].ToString());
                }

                if (Result != null && Result.Rows.Count > 0)
                {
                    str.Append("update tb_ExaminationResult set ER_Score=" +  Math.Round((Goal+longsores),2) + ",ER_Custom3='" + whenlong + "' where ");
                    str.Append(" ERId=" + Result.Rows[0]["ERId"]);

                }
                else
                {
                    str.Append("insert into tb_ExaminationResult(ER_EId,ER_PId,ER_MId,ER_Type,ER_Score,ER_State,ER_Operator,ER_AddTime,ER_Custom2,ER_Custom3) ");
                    str.Append("values (" + Eid + "," + Pid + "," + UId + ",3," + Math.Round((Goal + longsores), 2) + ",0,'" + UId + "','" + DateTime.Now + "','" + UserNo + "','" + whenlong + "') ");

                }

                SqlHelper.ExecuteNonQuery(str.ToString());

                SqlParameter[] parsto = new SqlParameter[] { new SqlParameter("@ED_State", "1") };
                commonbll.UpdateInfo("tb_ExaminationDetails", "ED_State=@ED_State", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + "  and ED_Type=3 and ED_State=0", parsto);

                commonbll.DeleteInfo("tb_CountDown", "  and CD_Custom3=3 and CD_EId=" + Eid + " and CD_PId=" + Pid + " and CD_MId=" + UId + "");
                return "1";
            }
            catch
            {
                return "99";
            }


        }






        /// <summary>
        /// 本次做题总成绩分数
        /// </summary>
        /// <returns></returns>
        public string GetExaminationResultBys()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            DataTable dt = commonbll.GetListDatatable("ER_Score", @"tb_ExaminationResult", "and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=3 and ER_State=0 ");
            return JsonConvert.SerializeObject(dt);
        }
        //退出清掉做题成绩
        public string DelExamResult()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            //清掉明细 货币知识 状态为0 最新 做题过程中的
            commonbll.DeleteInfo("tb_CountDown", "  and CD_Custom3=3  and CD_EId=" + Eid + " and CD_PId=" + Pid + " and CD_MId=" + UId + "");
            commonbll.DeleteInfo("tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + "  and ED_Type=3 and ED_State=0  ");
            //commonbll.DeleteInfo("tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + "  and ER_Type=3 and ER_State=0  ");
            commonbll.DeleteInfo("tb_Operation_Answer", " and EId=" + Eid + " and PId='" + Pid + "' and MId=" + UId + "  and Type=3 and State=0  ");
            return "1";
        }

        /// <summary>
        /// 考试明细 Type==1? 后台进入前台查看明细：学生自己看明细
        /// </summary>
        /// <returns></returns>
        public ActionResult Preview()
        {
            var Eid = Request["Eid"];
            var Type = Request["Type"];
            var UIds = Request["UIds"];
            if (string.IsNullOrEmpty(Type) || Type == "0")
            { 
                UIds = UId;
            }
            var Pid = Request["Pid"];
            var m = new FHKSModel();
            m.MId = UIds;
            if (!String.IsNullOrEmpty(Eid) && !String.IsNullOrEmpty(Pid))
            {
                string list = @"e.Id,e.TestPaperId,e.ExaminationName,e.ExaminationStartTime,e.ExaminationEndTime,e.ExaminationLength,e.ExaminationType,
(select SUM(t.TestsFraction) from tb_FH_RelationTopic t where t.TestPaperID=e.TestPaperId) as Score, 
(select Name from tb_School a inner join tb_Team b on b.[SchoolId]=a.Id  where b.Id=" + UserClassId + ") as SchoolName";//列
                DataTable tb = commonbll.GetListDatatable(list, "tb_FH_Examination e", " and e.Id=" + Eid);


                if (tb != null && tb.Rows.Count > 0)
                {
                    m.E_Name = tb.Rows[0]["ExaminationName"].ToString();//竞赛名称
                    m.Score = tb.Rows[0]["Score"].ToString();//分值
                    if (string.IsNullOrEmpty(Type) || Type == "0")
                    {
                        m.UserName = Base_UserInfo.UserName;//姓名
                        m.UserPic = Base_UserInfo.UserPic;//头像
                        m.SchoolName = tb.Rows[0]["SchoolName"].ToString(); ;//学校
                    }
                    else
                    {
                        DataTable Utb = commonbll.GetListDatatable(" u.UserName,u.UserPic,(select Name from tb_School where Id=u.UserSchoolId) as SchoolName", "tb_UserInfo u", " and u.UId=" + UIds);
                        if (Utb != null && Utb.Rows.Count > 0)
                        {
                            m.UserName = Utb.Rows[0]["UserName"].ToString();
                            m.UserPic = Utb.Rows[0]["UserPic"].ToString();
                            m.SchoolName = Utb.Rows[0]["SchoolName"].ToString();
                        }
                    }
                    m.E_StartTime = Convert.ToDateTime(tb.Rows[0]["ExaminationStartTime"]).ToString("yyyy-MM-dd HH:mm:01");
                    m.E_EndTime = Convert.ToDateTime(tb.Rows[0]["ExaminationEndTime"]).ToString("yyyy-MM-dd HH:mm:01");
                    m.E_Whenlong = tb.Rows[0]["ExaminationLength"].ToString();//竞赛时长
                    m.E_Type = tb.Rows[0]["ExaminationType"].ToString();//竞赛类型
                    var ER_Score = commonbll.GetListSclar(" top 1  ISNULL(ER_Score,0)", "tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UIds + " and ER_Type=3 and ER_State=0 ");

                    ViewData["ER_Score"] = ER_Score;


                    var data = commonbll.GetListDatatable("*,(select top 1 Title from tb_FH_Topic where Id=TopicId) as Title,(select COUNT(1) from tb_ExaminationDetails where ED_Type=3 and ED_MId=" + UIds + " and ED_QBId=TopicId and ED_PId=TestPaperID and ED_State=1) as siz,(select COUNT(1) from tb_Operation_Answer where EId=" + Eid + @" and MId=" + UIds + " and PId=TestPaperID and QBId =TopicId and [Type]=3 and   [State]=1) as atotal,(select COUNT(1) from tb_Operation_Answer where EId=" + Eid + @" and MId=" + UIds + " and PId=TestPaperID and QBId =TopicId and [Type]=3 and  OkNo='错误' and [State]=1) as iscw", "tb_FH_RelationTopic", " and TestPaperID=" + Pid);
                    ViewData["RelationTopicTable"] = data;



                }

            }
            return View(m);
        }

    }
}
