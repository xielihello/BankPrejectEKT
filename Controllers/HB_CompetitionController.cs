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
    FileName:货币知识 学生端竞赛列表
    Copyright（c）2017-金融教育在线技术开发部
    Author:袁学
    Create Date:2017-4-8
    ******************************************************************/
    public class HB_CompetitionController : BaseController
    {
        //
        // GET: /HB_Competition/
        //string UId = "3";//用户ID
        //string UserType = "3";//1管理员2教师3学生4裁判
        //string UserNo = "s00001";//登录账号
        //string UserClassId = "1";//所属班级
        CommonBll commonbll = new CommonBll();

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 未完成竞赛列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList()
        {
            string wheres = " and (E_TeamId like '%," + UserClassId + ",%' or E_TeamId='') and E_IsState=1 and EId not in (select ER_EId from tb_ExaminationResult where ER_MId=" + UId + " and  ER_Type=1)";

            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")//竞赛模式
            {
                wheres += " and E_Type='" + Request["E_Type"].ToString() + "'";
            }

            //竞赛名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {
                wheres += " and E_Name like '%" + Request["E_Name"] + "%'";
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "EId desc,E_EndTime desc"; //排序必须填写
            m.strFld = " a.*,(select SUM(EP_Score) from tb_HB_ExaminationPapers where EP_PId=E_PId) as Score";
            m.tab = "tb_HB_Examination a";
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
                    var ksshishijian = Convert.ToDateTime(dt.Rows[i]["E_StartTime"].ToString());//有效开始时间
                    var jsshishijian = Convert.ToDateTime(dt.Rows[i]["E_EndTime"].ToString());//有效结束时间


                    sb.Append("<div class=\"item_list_line\">");
                    if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束 图像 灰暗
                    {
                        sb.Append("<img src=\"../img/student/icoend.jpg\" />");
                    }
                    else
                    {
                        sb.Append("<img src=\"../img/student/iconew.jpg\" />");
                    }

                    sb.Append("<div class=\"list_title\">");

                    sb.Append("<span>" + dt.Rows[i]["E_Name"] + "</span>");
                    
                    if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                    }

                    if (dqshijian.AddMinutes(10) >= ksshishijian && dqshijian <= jsshishijian)
                    {
                        sb.Append("<button class=\"btn_go\"  onclick=\"Getinto(" + dt.Rows[i]["EId"] + "," + dt.Rows[i]["E_PId"] + ")\" type=\"button\">进入</button>");
                    }

                    if (dqshijian.AddMinutes(10) < ksshishijian)//当前时间 小于考试开始时间 考试HIA没有开始 
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">未开始</button>");
                    }

                    sb.Append("</div>");

                    sb.Append(" <div class=\"list_title_smal\">");

                    if (dt.Rows[i]["E_Type"].ToString() == "1")//考试模式
                    {
                        sb.Append("<label>试卷类型：考试模式</label>");
                    }
                    else
                    {
                        sb.Append("<label>试卷类型：练习模式</label>");
                    }


                    sb.Append("<label>试卷总分：" + dt.Rows[i]["Score"] + "分</label>");
                    sb.Append("<label>试卷有效时间：" + dt.Rows[i]["E_StartTime"].ToString() + " —— " + dt.Rows[i]["E_EndTime"].ToString() + "  </label>");

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
            string wheres = " and (E_TeamId like '%," + UserClassId + ",%' or E_TeamId='') and E_IsState=1 and EId  in (select ER_EId from tb_ExaminationResult where ER_MId=" + UId + "  and  ER_Type=1 )";

            if (Request["E_TypeTo"] != null && Request["E_TypeTo"].ToString() != "0")//竞赛模式
            {
                wheres += " and E_Type='" + Request["E_TypeTo"].ToString() + "'";
            }

            //竞赛名称
            if (Request["E_NameTo"] != null && Request["E_NameTo"].ToString().Length > 0)
            {
                wheres += " and E_Name like '%" + Request["E_NameTo"] + "%'";
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "[E_Type] desc,[E_EndTime] desc"; //排序必须填写
            m.strFld = @" a.*,(select SUM(EP_Score) from tb_HB_ExaminationPapers where EP_PId=E_PId) as Score,(select top 1 ER_Score from tb_ExaminationResult where ER_MId=" + UId + @" and  ER_Type=1 and ER_EId=EId order by ERId desc) as CZScore,
(select top 1 ER_AddTime from tb_ExaminationResult where ER_MId=" + UId + " and  ER_Type=1 and ER_EId=EId order by ERId desc) as ER_AddTime";
            m.tab = "tb_HB_Examination a";
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
                    var ksshishijian = Convert.ToDateTime(dt.Rows[i]["E_StartTime"].ToString());//有效开始时间
                    var jsshishijian = Convert.ToDateTime(dt.Rows[i]["E_EndTime"].ToString());//有效结束时间

                    sb.Append("<div class=\"item_list_line\">");

                    if (dt.Rows[i]["E_Type"].ToString() == "1")//图像 灰暗
                    {
                        sb.Append("<img src=\"../img/student/icoend.jpg\" />");
                    }

                    if (dt.Rows[i]["E_Type"].ToString() == "2")//练习模式
                    {
                        if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束 图像 灰暗
                        {
                            sb.Append("<img src=\"../img/student/icoend.jpg\" />");
                        }
                        else
                        {
                            sb.Append("<img src=\"../img/student/iconew.jpg\" />");
                        }
                    }




                    sb.Append("<div class=\"list_title\">");

                    sb.Append("<a href=\"#chart-personPK\" onclick='ViewGrowthTrajectory(" + dt.Rows[i]["EId"] + ")'>" + dt.Rows[i]["E_Name"] + "</a>");


                    if (dt.Rows[i]["E_Type"].ToString() == "1")//考试模式 直接已结束
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                    }

                    if (dt.Rows[i]["E_Type"].ToString() == "2")//练习模式
                    {
                        if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                        {
                            sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                        }

                        if (dqshijian >= ksshishijian && dqshijian <= jsshishijian)
                        {
                            sb.Append("<button class=\"btn_ph\" onclick=\"PH(" + dt.Rows[i]["EId"] + "," + dt.Rows[i]["E_PId"] + ")\" type=\"button\">排行榜</button>");
                            sb.Append("<button class=\"btn_look\" onclick=\"Viewdetails(" + dt.Rows[i]["EId"] + "," + dt.Rows[i]["E_PId"] + ")\" type=\"button\">查看明细</button>");
                            sb.Append("<button class=\"btn_go2\" onclick=\"Getinto(" + dt.Rows[i]["EId"] + "," + dt.Rows[i]["E_PId"] + ")\" type=\"button\">进入</button>");
                        }

                    }
                    sb.Append("</div>");

                    sb.Append(" <div class=\"list_title_smal\">");

                    if (dt.Rows[i]["E_Type"].ToString() == "1")//考试模式
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
                    sb.Append("<label>试卷有效时间：" + dt.Rows[i]["E_StartTime"].ToString() + " —— " + dt.Rows[i]["E_EndTime"].ToString() + "  </label>");

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
                        DataTable dt = commonbll.GetListDatatable("SUM(ER_Score) as ER_Score,count(ER_Score) as num", "tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_MId=" + UId + " and datediff(day,ER_AddTime,'" + datetlist[i] + "')=0");
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
            string EName = commonbll.GetListSclar("E_Name", "tb_HB_Examination", " and EId=" + Eid);

            var json = new object[] {
                        new{
                            scoresstr=scoresstr,
                            EName=EName
                        }
                    };
            return JsonConvert.SerializeObject(json);
        }


        /// <summary>
        /// 列表进入做题页 逻辑处理
        /// </summary>
        /// <returns></returns>
        public string CheckGointo()
        {

            try
            {
                var Eid = Request["Eid"];
                var Pid = Request["Pid"];
                var Type = Request["Type"];
                var resultcount = commonbll.GetRecordCount("tb_CountDown", " and CD_Custom3="+Type+" and CD_EId='" + Eid + "' and CD_PId='" + Pid + "' and CD_MId='" + UId + "'");
                if (resultcount == 0)
                {
                    SqlHelper.ExecuteNonQuery("insert into tb_CountDown(CD_EId, CD_PId, CD_MId, CD_Time,CD_Custom3) values('" + Eid + "','" + Pid + "','" + UId + "','" + DateTime.Now + "',"+Type+")");

                }

                return "1";
            }
            catch
            {
                return "99";
            }
        }


       
        /// <summary>
        /// 货币知识 单项赛项排行
        /// </summary>
        /// <returns></returns>
        public string GetPH()
        {

            var Eid = Request["Eid"];
            var Pid = Request["Pid"];

            //1.找出我班级下所有的数据
            var list = @"ERId,ER_EId,ER_PId,ER_MId,
(select E_Name from tb_HB_Examination where EId=a.ER_EId) as E_Name,
UserName,UserNo,StudentNo,ER_Custom3,ER_Score,
(select MAX(ER_Score) from tb_ExaminationResult where ER_Type=a.ER_Type and ER_EId=a.ER_EId  and ER_PId=a.ER_PId  and ER_MId=a.ER_MId) as lsMinScore";
            var table = "tb_ExaminationResult a inner join tb_UserInfo b on a.ER_MId=b.UId";
            var where = " and ER_Type=1 and ER_State=0 and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and UserClassId=" + UserClassId;
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
                    sb.Append("恭喜您！您的<span class=\"text_red\">理论知识</span>排名为第<span class=\"text_red\">1</span>名，");
                    sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                    sb.Append("距离上一名还差<span class=\"text_red\">0</span>分！");
                }
                else
                {
                    //上一名得分-当前得分
                    var SYMScore = Convert.ToDecimal(dt.Rows[myidx - 1]["ER_Score"]) - Convert.ToDecimal(dt.Rows[myidx]["ER_Score"]);
                    if (SYMScore != 0)//得分与上一名不相同
                    {
                        sb.Append("您的<span class=\"text_red\">理论知识</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                            sb.Append("您的<span class=\"text_red\">理论知识</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                    sb.Append("您的<span class=\"text_red\">理论知识</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                        sb.Append("您的<span class=\"text_red\">理论知识</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                sb.Append("您的<span class=\"text_red\">理论知识</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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

    }
}
