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
using VocationalProject_Bll;
using VocationalProject_Dal;

namespace VocationalProject.Controllers
{
    public class SG_CompetitionController : BaseController
    {
        /***************************************************************
        FileName:货币知识 学生端竞赛列表
        Copyright（c）2017-金融教育在线技术开发部
        Author:邵世铨
        Create Date:2017-4-8
        ******************************************************************/
        // GET: /HB_Competition/
        //string UId = "3";//用户ID
        //string UserType = "3";//1管理员2教师3学生4裁判
        //string UserNo = "s00001";//登录账号
        //string UserClassId = "1";//所属班级
        CommonBll commonbll = new CommonBll();
        Manual_ExaminationBll ExaBll = new Manual_ExaminationBll();
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
            string wheres = "and a.ExaminationState=1 and a.ID not in (select ER_EId from tb_ExaminationResult where ER_MId=" + UId + " and ER_Type=2)";

            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")//竞赛模式
            {
                wheres += " and Pattern='" + Request["E_Type"].ToString() + "'";
            }

            //竞赛名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {
                wheres += " and ExaminationName like '%" + Request["E_Name"] + "%'";
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.ID desc,EndTime desc"; //排序必须填写
            m.strFld = " a.*,c.Score";
            m.tab = "tb_Manual_Examination a left join tb_Manual_Counting c on a.Taskid=c.ID";
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
                    var ksshishijian = Convert.ToDateTime(dt.Rows[i]["StartTime"].ToString());//有效开始时间
                    var jsshishijian = Convert.ToDateTime(dt.Rows[i]["EndTime"].ToString());//有效结束时间


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
                    sb.Append("<span>" + dt.Rows[i]["ExaminationName"] + "</span>");

                   
                    if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                    }

                    if (dqshijian.AddMinutes(10) >= ksshishijian && dqshijian <= jsshishijian)
                    {
                        sb.Append("<button class=\"btn_go\" onclick=\"Getinto(" + dt.Rows[i]["ID"].ToString() + "," + dt.Rows[i]["Taskid"].ToString() + ")\" type=\"button\">进入</button>");
                    }

                    if (dqshijian.AddMinutes(10) < ksshishijian)//当前时间 小于考试开始时间 考试HIA没有开始 
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">未开始</button>");
                    }

                    sb.Append("</div>");

                    sb.Append(" <div class=\"list_title_smal\">");

                    if (dt.Rows[i]["Pattern"].ToString() == "1")//考试模式
                    {
                        sb.Append("<label>试卷类型：练习模式</label>");
                    }
                    else if (dt.Rows[i]["Pattern"].ToString() == "2")
                    {
                        sb.Append("<label>试卷类型：考试模式</label>");
                    }


                    sb.Append("<label>试卷总分：" + dt.Rows[i]["Score"] + "分</label>");
                    sb.Append("<label>试卷有效时间：" + dt.Rows[i]["StartTime"].ToString() + " —— " + dt.Rows[i]["EndTime"].ToString() + "  </label>");

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
            string wheres = "and a.ExaminationState=1 and a.ID in (select ER_EId from tb_ExaminationResult where ER_MId=" + UId + " and ER_Type=2)";


            if (Request["E_TypeTo"] != null && Request["E_TypeTo"].ToString() != "0")//竞赛模式
            {
                wheres += " and Pattern='" + Request["E_TypeTo"].ToString() + "'";
            }
            //竞赛名称
            if (Request["E_NameTo"] != null && Request["E_NameTo"].ToString().Length > 0)
            {
                wheres += " and ExaminationName like '%" + Request["E_NameTo"] + "%'";
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "Pattern,EndTime desc"; //排序必须填写
            m.strFld = @" a.*,c.Score,(select top 1 ER_Score from tb_ExaminationResult where ER_MId=" + UId + @" and ER_EId=a.ID and ER_Type=2 order by ERId desc) as CZScore,
(select top 1 ER_AddTime from tb_ExaminationResult where ER_MId=" + UId + " and ER_Type=2 and ER_EId=a.ID order by ERId desc) as ER_AddTime";
            m.tab = "tb_Manual_Examination a left join tb_Manual_Counting c on a.Taskid=c.ID";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);

            StringBuilder sb = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    //1.开始时间未到，显示未开始
                    var dqshijian = DateTime.Now;
                    var ksshishijian = Convert.ToDateTime(dt.Rows[i]["StartTime"].ToString());//有效开始时间
                    var jsshishijian = Convert.ToDateTime(dt.Rows[i]["EndTime"].ToString());//有效结束时间
                    sb.Append("<div class=\"item_list_line\">");
                    if (dt.Rows[i]["Pattern"].ToString() == "1")//图像 灰暗 练习模式
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

                    if (dt.Rows[i]["Pattern"].ToString() == "2")//考试模式
                    {
                        sb.Append("<img src=\"../img/student/icoend.jpg\" />");
                    }
                    //sb.Append("<img src=\"../img/student/icoend.jpg\" />");

                    sb.Append("<div class=\"list_title\">");

                    sb.Append("<a href=\"#chart-personPK\" onclick='ViewGrowthTrajectory(" + dt.Rows[i]["ID"] + ")'>" + dt.Rows[i]["ExaminationName"] + "</a>");



                    if (dt.Rows[i]["Pattern"].ToString() == "2")//考试模式 直接已结束
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                    }

                    if (dt.Rows[i]["Pattern"].ToString() == "1")//练习模式
                    {
                        if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                        {
                            sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                        }

                        if (dqshijian >= ksshishijian && dqshijian <= jsshishijian)
                        {
                            sb.Append("<button class=\"btn_ph\" onclick=\"PH(" + dt.Rows[i]["ID"].ToString() + "," + dt.Rows[i]["Taskid"].ToString() + ")\" type=\"button\">排行榜</button>");
                            sb.Append("<button class=\"btn_look\" onclick=\"See(" + dt.Rows[i]["ID"].ToString() + ")\" type=\"button\">查看明细</button>");
                            sb.Append("<button class=\"btn_go2\" onclick=\"Getinto(" + dt.Rows[i]["ID"].ToString() + "," + dt.Rows[i]["Taskid"].ToString() + ")\" type=\"button\">进入</button>");
                        }

                    }
                    sb.Append("</div>");

                    sb.Append(" <div class=\"list_title_smal\">");

                    if (dt.Rows[i]["Pattern"].ToString() == "1")//考试模式
                    {
                        sb.Append("<label>试卷类型：练习模式</label>");
                    }
                    else if (dt.Rows[i]["Pattern"].ToString() == "2")
                    {
                        sb.Append("<label>试卷类型：考试模式</label>");
                    }


                    sb.Append("<label>试卷总分：" + dt.Rows[i]["Score"] + "分</label>");
                    sb.Append("<label>操作得分：" + dt.Rows[i]["CZScore"] + "分</label>");
                    sb.Append("<label>提交时间：" + dt.Rows[i]["ER_AddTime"].ToString() + "</label>");
                    sb.Append("<label>试卷有效时间：" + dt.Rows[i]["StartTime"].ToString() + " —— " + dt.Rows[i]["EndTime"].ToString() + "  </label>");
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
            string EName = commonbll.GetListSclar("ExaminationName", "tb_Manual_Examination", " and ID=" + Eid);

            var json = new object[] {
                        new{
                            scoresstr=scoresstr,
                            EName=EName
                        }
                    };
            return JsonConvert.SerializeObject(json);
        }

        /// <summary>
        /// 手工考试提交
        /// </summary>
        /// <returns></returns>
        public ViewResult SG_CompetitionExamination()
        {
            var Exaid = Request["Exaid"];
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public StringBuilder SG_CompetitionControllerList()
        {
            var Exaid = Request["Id"];
            var PaperId = Request["Pid"];
            StringBuilder sb = new StringBuilder();
            DataTable ExaDt = commonbll.GetListDatatable("*", "tb_Manual_Examination", "and ID=" + Exaid + "");
            var TaskID = 0;
            if (ExaDt.Rows.Count > 0)
            {
                TaskID = Convert.ToInt32(ExaDt.Rows[0]["Taskid"]);
                sb.Append(" <input type=\"hidden\" id=\"ExaminationNameid\" value=" + ExaDt.Rows[0]["ExaminationName"].ToString() + ">");
            }
            DataTable TasDt = commonbll.GetListDatatable("*", "tb_Manual_Counting", "and ID=" + TaskID + "");
            if (TasDt.Rows.Count > 0)
            {
                sb.Append(" <input type=\"hidden\" id=\"Scoreid\" value=" + TasDt.Rows[0]["Score"].ToString() + ">");
            }
            DataTable StudentDt = commonbll.GetListDatatable("u.*,s.Name", "tb_UserInfo u left join tb_School s on u.UserSchoolId=s.Id", " and u.UId=" + UId + "");
            if (StudentDt.Rows.Count > 0)
            {
                sb.Append(" <input type=\"hidden\" id=\"UserNameid\" value=" + StudentDt.Rows[0]["UserName"].ToString() + ">");
                if (StudentDt.Rows[0]["UserPic"] == null || StudentDt.Rows[0]["UserPic"].ToString() == "")
                {
                    sb.Append(" <input type=\"hidden\" id=\"UserPicid\" value=\"/img/profile_s.jpg\">");
                }
                else
                {
                    sb.Append(" <input type=\"hidden\" id=\"UserPicid\" value=" + StudentDt.Rows[0]["UserPic"].ToString() + ">");
                }
                sb.Append(" <input type=\"hidden\" id=\"Schoolid\" value=" + StudentDt.Rows[0]["Name"].ToString() + ">");
            }
            DataTable Dt = commonbll.GetListDatatable("*", "tb_Manual_CountingDetailed", " and TaskID=" + TaskID + "");
            sb.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.Append("<tr><th style=\"width: 33%;\">票面</th><th>张数</th><th>金额</th></tr>");

            if (Dt.Rows.Count > 0)
            {
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    if (Dt.Rows[i]["TaskValue"].ToString() == "壹佰元")
                    {
                        sb.Append(" <tr><td>壹佰元</td><td><input type=\"text\" onkeypress=\"keyPress()\" onblur=\"keyPress2('100idNumber')\" name=\"100NameNumber\" id=\"100idNumber\" value=\"\"/></td>");
                        sb.Append("<td><input type=\"text\" name=\"100NameMoney\" onkeypress=\"keyPress()\" onblur=\"keyPress2('100idMoney')\" id=\"100idMoney\" value=\"\" /></td></tr>");
                    }
                    if (Dt.Rows[i]["TaskValue"].ToString() == "伍拾元")
                    {
                        sb.Append(" <tr><td>伍拾元</td><td><input type=\"text\" onkeypress=\"keyPress()\" onblur=\"keyPress2('50idNumber')\" name=\"50NameNumber\" id=\"50idNumber\" value=\"\"/></td>");
                        sb.Append("<td><input type=\"text\" name=\"50NameMoney\" onkeypress=\"keyPress()\" onblur=\"keyPress2('50idMoney')\" id=\"50idMoney\" value=\"\" /></td></tr>");
                    }
                    if (Dt.Rows[i]["TaskValue"].ToString() == "贰拾元")
                    {
                        sb.Append(" <tr><td>贰拾元</td><td><input type=\"text\" onkeypress=\"keyPress()\" onblur=\"keyPress2('20idNumber')\" name=\"20NameNumber\" id=\"20idNumber\" value=\"\"/></td>");
                        sb.Append("<td><input type=\"text\" name=\"20NameMoney\" onkeypress=\"keyPress()\" onblur=\"keyPress2('20idMoney')\" id=\"20idMoney\" value=\"\" /></td></tr>");
                    }
                    if (Dt.Rows[i]["TaskValue"].ToString() == "拾元")
                    {
                        sb.Append(" <tr><td>拾元</td><td><input type=\"text\" onkeypress=\"keyPress()\" onblur=\"keyPress2('10idNumber')\" name=\"10NameNumber\" id=\"10idNumber\" value=\"\"/></td>");
                        sb.Append("<td><input type=\"text\" name=\"10NameMoney\" onkeypress=\"keyPress()\" onblur=\"keyPress2('10idMoney')\" id=\"10idMoney\" value=\"\" /></td></tr>");
                    }
                    if (Dt.Rows[i]["TaskValue"].ToString() == "伍元")
                    {
                        sb.Append(" <tr><td>伍元</td><td><input type=\"text\" onkeypress=\"keyPress()\" onblur=\"keyPress2('5idNumber')\" name=\"10NameNumber\" name=\"5NameNumber\" id=\"5idNumber\" value=\"\"/></td>");
                        sb.Append("<td><input type=\"text\" name=\"5NameMoney\" onkeypress=\"keyPress()\" onblur=\"keyPress2('5idMoney')\" id=\"5idMoney\" value=\"\" /></td></tr>");
                    }
                    if (Dt.Rows[i]["TaskValue"].ToString() == "贰元")
                    {
                        sb.Append(" <tr><td>贰元</td><td><input type=\"text\"  onkeypress=\"keyPress()\" onblur=\"keyPress2('2idNumber')\" name=\"10NameNumber\" name=\"2NameNumber\" id=\"2idNumber\" value=\"\"/></td>");
                        sb.Append("<td><input type=\"text\" name=\"2NameMoney\" onkeypress=\"keyPress()\" onblur=\"keyPress2('2idMoney')\" id=\"2idMoney\" value=\"\" /></td></tr>");
                    }
                    if (Dt.Rows[i]["TaskValue"].ToString() == "壹元")
                    {
                        sb.Append(" <tr><td>壹元</td><td><input type=\"text\" onkeypress=\"keyPress()\" onblur=\"keyPress2('1idNumber')\" name=\"1NameNumber\" id=\"1idNumber\" value=\"\"/></td>");
                        sb.Append("<td><input type=\"text\" name=\"1NameMoney\" onkeypress=\"keyPress()\" onblur=\"keyPress2('1idMoney')\" id=\"1idMoney\" value=\"\" /></td></tr>");
                    }
                    if (Dt.Rows[i]["TaskValue"].ToString() == "伍角")
                    {
                        sb.Append(" <tr><td>伍角</td><td><input type=\"text\" onkeypress=\"keyPress()\" onblur=\"keyPress2('fiveidNumber')\" name=\"fiveNameNumber\" id=\"fiveidNumber\" value=\"\"/></td>");
                        sb.Append("<td><input type=\"text\" name=\"fiveNameMoney\" onkeypress=\"keyPress()\" onblur=\"keyPress2('fiveidMoney')\" id=\"fiveidMoney\" value=\"\" /></td></tr>");
                    }

                }
            }
            sb.Append("</table>");
            int Second = 0;
            //操作时间
            try
            {
                DateTime Time = Convert.ToDateTime(commonbll.GetListSclar("CD_Time", "tb_CountDown", " and CD_Custom3=2 and CD_EId=" + Exaid + " and CD_PId=" + PaperId + " and CD_MId=" + UId + ""));

                //考试时长
                int ExamLength = Convert.ToInt32(commonbll.GetListSclar("LongTime", "tb_Manual_Examination", " and ID=" + Exaid));
                DateTime BeginTime = Time.AddMinutes(ExamLength);
                DateTime CurrentTime = DateTime.Now;
                if (CurrentTime < BeginTime)
                {
                    DateTime dt1 = CurrentTime;
                    DateTime dt2 = BeginTime;
                    TimeSpan ts = dt2.Subtract(dt1);
                    Second = Convert.ToInt32(ts.TotalSeconds);//秒差
                }
            }
            catch
            {
               
            }
            sb.Append(" <input type=\"hidden\" id=\"Timeid\" value=\"" + Second + "\">");
            return sb;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ExamRemainingTime()
        {
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            int Start = 0;
            //当前时间
            DateTime OperatorTime = DateTime.Now;
            //考试模式
            // int ExamPattern = Convert.ToInt32(commonbll.GetListSclar("Pattern", "tb_Manual_Examination", " and ID=" + ExamId));
            //有效开始时间
            DateTime StartTime = Convert.ToDateTime(commonbll.GetListSclar("StartTime", "tb_Manual_Examination", " and ID=" + ExamId));
            //当前模式是考试模式 不管是考试还是倒计时 都要出倒计时的 擦啊 xx
            //if (ExamPattern == 2 || ExamPattern==1)
            //{
            if (OperatorTime < StartTime)
            {
                Start = 1;
            }
            // }
            return Start.ToString();
        }
        /// <summary>
        /// 考试倒计时页面计算时间
        /// </summary>
        /// <param name="ExamId"></param>
        /// <returns></returns>
        public ActionResult RemainingTime(int ExamId, int PaperId)
        {
            int Second = 0;
            // DateTime StartTime = Convert.ToDateTime(commonbll.GetListSclar("StartTime", "tb_Bill_Exam", " and ID=" + ExamId));
            //当前时间
            DateTime OperatorTime = DateTime.Now;
            //有效开始时间
            DateTime StartTime = Convert.ToDateTime(commonbll.GetListSclar("StartTime", "tb_Manual_Examination", " and ID=" + ExamId));
            if (OperatorTime < StartTime)
            {
                DateTime dt1 = OperatorTime;
                DateTime dt2 = StartTime;
                TimeSpan ts = dt2.Subtract(dt1);
                Second = Convert.ToInt32(ts.TotalSeconds);//秒差
            }
            ViewBag.ExamId = ExamId;
            ViewBag.PaperId = PaperId;
            ViewBag.Second = Second;
            return View();
        }
      
        public string SG_Save()
        {
            //1.0接收参数
            var idNumber100 = Request["idNumber100"];
            var idMoney100 = Request["idMoney100"];
            var idNumber50 = Request["idNumber50"];
            var idMoney50 = Request["idMoney50"];
            var idNumber20 = Request["idNumber20"];
            var idMoney20 = Request["idMoney20"];
            var idNumber10 = Request["idNumber10"];
            var idMoney10 = Request["idMoney10"];
            var idNumber5 = Request["idNumber5"];
            var idMoney5 = Request["idMoney5"];
            var idNumber2 = Request["idNumber2"];
            var idMoney2 = Request["idMoney2"];
            var idNumber1 = Request["idNumber1"];
            var idMoney1 = Request["idMoney1"];
            var fiveidNumber = Request["idNumberfive"];
            var fiveidMoney = Request["idMoneyfive"];
            var ID = Request["Id"].ToString();
            var Time = Request["Time"].ToString();
            string SumTime = Request["SumTime"];//倒计时总时间
            string UsedTime = string.Empty;
            string Backoff = Request["Backoff"];
            #region 袁注释 世铨的
            //if (SumTime != "")
            //{
            //    var send = SumTime.Split(':');
            //    var send1 = Time.Split(':');
            //    //时
            //    if (send[0] != "00")
            //    {
            //        int shijian = Convert.ToInt32(send[0]) - Convert.ToInt32(send1[0]);
            //        if (shijian <= 9)
            //        {
            //            UsedTime = "0" + shijian + ":";
            //        }
            //        if (shijian == 0)
            //        {
            //            UsedTime = "00" + ":";
            //        }
            //        else
            //        {
            //            UsedTime += shijian + ":";
            //        }
            //    }
            //    else
            //    {
            //        UsedTime = "00" + ":";
            //    }
            //    //分
            //    if (send[1] != "00")
            //    {
            //        int shijian = Convert.ToInt32(send[1]) - Convert.ToInt32(send1[1]);
            //        if (shijian <= 9)
            //        {
            //            UsedTime += "0" + shijian + ":";
            //        } if (shijian == 0)
            //        {
            //            UsedTime += "00" + ":";
            //        }
            //        else
            //        {
            //            UsedTime += shijian + ":";
            //        }
            //    }
            //    else
            //    {
            //        UsedTime += "00" + ":";
            //    }
            //    //秒
            //    if (send[2] != "00")
            //    {
            //        int shijian = Convert.ToInt32(send[2]) - Convert.ToInt32(send1[2]);
            //        if (shijian <= 9)
            //        {
            //            UsedTime += "0" + shijian;
            //        }
            //        if (shijian == 0)
            //        {
            //            UsedTime += "00";
            //        }
            //        else
            //        {
            //            UsedTime += shijian;
            //        }
            //    }
            //    else
            //    {
            //        UsedTime += "00";
            //    }
            //}

            #endregion
            //
            //计算时间 做题时长
            DataTable GetDtTaskId = ExaBll.SelectExamination(ID);

            string Pid = GetDtTaskId.Rows[0]["Taskid"].ToString();
            var whenlong = "00:00:00";
            var Towhenlong = 0.00;//存储做题总时间总秒
            DataTable timeDt = commonbll.GetListDatatable("CD_Time", "tb_CountDown", "  and CD_Custom3=2 and CD_EId=" + ID + " and CD_PId='" + Pid + "' and CD_MId=" + UId);
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
            UsedTime = whenlong;

            //1.1查询考试模式
            DataTable Dt = commonbll.GetListDatatable("Pattern,Taskid,Pattern,TimeBonus,LongTime", "tb_Manual_Examination", "and ID=" + ID + "");
            var Taskid = 0;//试卷id
            var Pattern = "";//1练习模式，2考试模式
            var Score = 0;//试卷总分
            decimal Individual = 0;//面值个数
            decimal IndividualScore = 0.00M;//单项分值
            decimal AlreadyScore = 0.00M;
            var TimeBonus = "1";//1加分 2 不加分
            string E_Whenlong = "0";//做题时长
            if (Dt.Rows.Count > 0)
            {
                Pattern = Dt.Rows[0]["Pattern"].ToString();
                Taskid = Convert.ToInt32(Dt.Rows[0]["Taskid"]);
                TimeBonus = Dt.Rows[0]["TimeBonus"].ToString();
                E_Whenlong = Dt.Rows[0]["LongTime"].ToString();
            }
            //1.2查询该考试的总分数
            DataTable TaskDt = commonbll.GetListDatatable("Score", "tb_Manual_Counting", " and ID=" + Taskid + "");
            if (TaskDt.Rows.Count > 0)
            {
                Score = Convert.ToInt32(TaskDt.Rows[0]["Score"]);
            }
            DataTable IndividualNumber = commonbll.GetListDatatable("count(*) as Number", "tb_Manual_CountingDetailed", " and TaskID=" + Taskid + "");
            if (IndividualNumber.Rows.Count > 0)
            {
                Individual = Convert.ToDecimal(IndividualNumber.Rows[0]["Number"]);
            }
            //1.3计算考试每个答案的分数是多少，考试总分/（考试面值数*2）=单项的分数
            if (Individual > 0)
            {
                IndividualScore = Score / (Individual * 2);
            }
            //1.4计算总分数（考试模式：每个项得分的和-裁判扣分，练习模式：每一项目的得分和）
            #region 计算总分数
            DataTable TaskDetailed = commonbll.GetListDatatable("*", "tb_Manual_CountingDetailed", " and TaskID=" + Taskid + "");
            if (TaskDetailed.Rows.Count > 0)
            {
                for (int i = 0; i < TaskDetailed.Rows.Count; i++)
                {
                    try
                    {
                        if (TaskDetailed.Rows[i]["TaskValue"].ToString() == "壹佰元")
                        {
                            if (idNumber100.ToString() != "")
                            {

                                if (Convert.ToInt32(idNumber100) == Convert.ToInt32(TaskDetailed.Rows[i]["Number"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idNumber100 = null;
                            }
                            if (idMoney100.ToString() != "")
                            {

                                if (Convert.ToDecimal(idMoney100) == Convert.ToDecimal(TaskDetailed.Rows[i]["Value"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idMoney100 = null;
                            }
                        }
                        if (TaskDetailed.Rows[i]["TaskValue"].ToString() == "伍拾元")
                        {
                            if (idNumber50.ToString() != "")
                            {

                                if (Convert.ToInt32(idNumber50) == Convert.ToInt32(TaskDetailed.Rows[i]["Number"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idNumber50 = null;
                            }
                            if (idMoney50.ToString() != "")
                            {

                                if (Convert.ToDecimal(idMoney50) == Convert.ToInt32(TaskDetailed.Rows[i]["Value"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idMoney50 = null;
                            }
                        }
                        if (TaskDetailed.Rows[i]["TaskValue"].ToString() == "贰拾元")
                        {
                            if (idNumber20.ToString() != "")
                            {
                                if (Convert.ToInt32(idNumber20) == Convert.ToInt32(TaskDetailed.Rows[i]["Number"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idNumber20 = null;
                            }
                            if (idMoney20.ToString() != "")
                            {
                                if (Convert.ToDecimal(idMoney20) == Convert.ToInt32(TaskDetailed.Rows[i]["Value"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idMoney20 = null;
                            }
                        }
                        if (TaskDetailed.Rows[i]["TaskValue"].ToString() == "拾元")
                        {
                            if (idNumber10.ToString() != "")
                            {
                                if (Convert.ToInt32(idNumber10) == Convert.ToInt32(TaskDetailed.Rows[i]["Number"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idNumber10 = null;
                            }
                            if (idMoney10.ToString() != "")
                            {
                                if (Convert.ToDecimal(idMoney10) == Convert.ToInt32(TaskDetailed.Rows[i]["Value"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idMoney10 = null;
                            }
                        }
                        if (TaskDetailed.Rows[i]["TaskValue"].ToString() == "伍元")
                        {
                            if (idNumber5.ToString() != "")
                            {
                                if (Convert.ToInt32(idNumber5) == Convert.ToInt32(TaskDetailed.Rows[i]["Number"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idNumber5 = null;
                            }
                            if (idMoney5.ToString() != "")
                            {
                                if (Convert.ToDecimal(idMoney5) == Convert.ToInt32(TaskDetailed.Rows[i]["Value"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idMoney5 = null;
                            }
                        }
                        if (TaskDetailed.Rows[i]["TaskValue"].ToString() == "贰元")
                        {
                            if (idNumber2.ToString() != "")
                            {
                                if (Convert.ToInt32(idNumber2) == Convert.ToInt32(TaskDetailed.Rows[i]["Number"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idNumber2 = null;
                            }
                            if (idMoney2.ToString() != "")
                            {
                                if (Convert.ToDecimal(idMoney2) == Convert.ToInt32(TaskDetailed.Rows[i]["Value"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idMoney2 = null;
                            }
                        }
                        if (TaskDetailed.Rows[i]["TaskValue"].ToString() == "壹元")
                        {
                            if (idNumber1.ToString() != "")
                            {
                                if (Convert.ToInt32(idNumber1) == Convert.ToInt32(TaskDetailed.Rows[i]["Number"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idNumber1 = null;
                            }
                            if (idMoney1.ToString() != "")
                            {
                                if (Convert.ToDecimal(idMoney1) == Convert.ToInt32(TaskDetailed.Rows[i]["Value"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                idMoney1 = null;
                            }
                        }
                        if (TaskDetailed.Rows[i]["TaskValue"].ToString() == "伍角")
                        {
                            if (fiveidNumber.ToString() != "")
                            {
                                if (Convert.ToInt32(fiveidNumber) == Convert.ToInt32(TaskDetailed.Rows[i]["Number"]))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                fiveidNumber = null;
                            }
                            if (fiveidMoney.ToString() != "")
                            {
                                // string d = Math.Round(decimal.Parse(fiveidMoney), 2).ToString();
                                if (fiveidMoney == Convert.ToDecimal(TaskDetailed.Rows[i]["Value"]).ToString("0.##"))
                                {
                                    AlreadyScore += IndividualScore;
                                }
                            }
                            else
                            {
                                fiveidMoney = null;
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }

            #endregion
            string ssScore = AlreadyScore.ToString();
            string aAlreadyScore = AlreadyScore.ToString();
            string ResponseState = "";

            ///时间加分 如果试卷总分=做题总分
            //当前考试是否允许时间加分，每提前一秒+0.01分  P_Score
            var longsores = 0.00m;
            //允许时间加分并且全对(考试总分==做题总分)
            if (TimeBonus == "1" && Convert.ToDecimal(Score) == Math.Round( Convert.ToDecimal(AlreadyScore)))
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

            //任务得分
            //总分重新赋值 +时间加分
            ssScore = Math.Round(Convert.ToDecimal(AlreadyScore) + longsores, 2).ToString();
            aAlreadyScore = Math.Round(Convert.ToDecimal(AlreadyScore) + longsores, 2).ToString();


            if (Convert.ToInt32(Dt.Rows[0]["Pattern"]) == 1)//练习模式
            {


                //1.1提交数据到数据库
                int Spare1 = 0;//0表示最新数据，1表示老数据
                //1.2修改老数据状态为1
                ExaBll.UpdateSpare1(UId, ID);
                //1.3插入数据
                ResponseState = ExaBll.AddExamination(UId, ID, "壹佰元", idNumber100, idMoney100, "伍拾元", idNumber50, idMoney50, "贰拾元", idNumber20, idMoney20, "拾元", idNumber10, idMoney10, "伍元", idNumber5, idMoney5, "贰元", idNumber2, idMoney2, "壹元", idNumber1, idMoney1, "伍角", fiveidNumber, fiveidMoney, aAlreadyScore, "0", ssScore, DateTime.Now, Spare1);
                //1.4插入数据到总成绩表
                DataTable Edt = ExaBll.SelectExamination(ID);
                DataTable Udt = ExaBll.SelectUserinfo(UId);

                string Competition = "2";//2表示手工
                string State = "0";
                string tTaskid = Edt.Rows[0]["Taskid"].ToString();
                string UserNo = Udt.Rows[0]["UserNo"].ToString();
                ExaBll.Update1ExaminationTol(UId, Edt.Rows[0]["Taskid"].ToString(), ID);
                ExaBll.AddExaminationTol(ID, tTaskid, UId, Competition, ssScore, State, UserNo, UsedTime);

                //6.
                commonbll.DeleteInfo("tb_CountDown", "  and CD_Custom3=2 and CD_EId='" + ID + "' and CD_PId='" + tTaskid + "' and CD_MId='" + UId + "'");
            }
            else if (Convert.ToInt32(Dt.Rows[0]["Pattern"]) == 2)//考试模式
            {
                //检测是否存在重复提交
                DataTable isDt = commonbll.GetListDatatable("*", "tb_Manual_ExaminationAnswer", " and StudenID=" + UId + " and ExaminationID=" + ID + "");
                if (isDt.Rows.Count > 0)
                {
                  ResponseState = "-1";//数据重复
                }
                else
                {
                    int Spare1 = 0;//0表示最新数据，1表示老数据
                    //1.1提交数据到数据库
                    ResponseState = ExaBll.AddExamination(UId, ID, "壹佰元", idNumber100, idMoney100, "伍拾元", idNumber50, idMoney50, "贰拾元", idNumber20, idMoney20, "拾元", idNumber10, idMoney10, "伍元", idNumber5, idMoney5, "贰元", idNumber2, idMoney2, "壹元", idNumber1, idMoney1, "伍角", fiveidNumber, fiveidMoney, aAlreadyScore, "0", ssScore, DateTime.Now, Spare1);
                    string AnswerId = "AnswerId";
                    HttpCookie cookie = new HttpCookie(AnswerId, ResponseState);
                    //保存cookie到硬盘3天
                    cookie.Expires = DateTime.Now.AddDays(3);
                    //将cookie加到响应的报文中
                    Response.Cookies.Add(cookie);

                    //1.4插入数据到总成绩表
                    DataTable Edt = ExaBll.SelectExamination(ID);
                    DataTable Udt = ExaBll.SelectUserinfo(UId);

                    string Competition = "2";//2表示手工
                    string State = "0";
                    string tTaskid = Edt.Rows[0]["Taskid"].ToString();
                    string UserNo = Udt.Rows[0]["UserNo"].ToString();
                    ExaBll.Update1ExaminationTol(UId, Edt.Rows[0]["Taskid"].ToString(), ID);
                    ExaBll.AddExaminationTol(ID, tTaskid, UId, Competition, ssScore, State, UserNo, UsedTime);
                    commonbll.DeleteInfo("tb_CountDown", "  and CD_Custom3=2 and CD_EId='" + ID + "' and CD_PId='" + tTaskid + "' and CD_MId='" + UId + "'");
                }
            }
            else if (Convert.ToInt32(Dt.Rows[0]["Pattern"]) == 3)//竞赛模式
            {
                DataTable isDt = commonbll.GetListDatatable("*", "tb_Manual_ExaminationAnswer", " and StudenID=" + UId + " and ExaminationID=" + ID + "");
                if (isDt.Rows.Count > 0)
                {
                    ResponseState = "-4";//数据重复
                }
                else
                {
                    int Spare1 = 0;
                    //1.1提交数据到数据库
                    ResponseState = ExaBll.AddExamination(UId, ID, "壹佰元", idNumber100, idMoney100, "伍拾元", idNumber50, idMoney50, "贰拾元", idNumber20, idMoney20, "拾元", idNumber10, idMoney10, "伍元", idNumber5, idMoney5, "贰元", idNumber2, idMoney2, "壹元", idNumber1, idMoney1, "伍角", fiveidNumber, fiveidMoney, aAlreadyScore, "0", ssScore, DateTime.Now, Spare1);
                    string AnswerId = "AnswerId";
                    HttpCookie cookie = new HttpCookie(AnswerId, ResponseState);
                    //保存cookie到硬盘3天
                    cookie.Expires = DateTime.Now.AddDays(3);
                    //讲cookie加到响应的报文中
                    Response.Cookies.Add(cookie);

                    //1.4插入数据到总成绩表
                    DataTable Edt = ExaBll.SelectExamination(ID);
                    DataTable Udt = ExaBll.SelectUserinfo(UId);

                    string Competition = "2";//2表示手工
                    string State = "0";
                    string tTaskid = Edt.Rows[0]["Taskid"].ToString();
                    string UserNo = Udt.Rows[0]["UserNo"].ToString();
                    ExaBll.Update1ExaminationTol(UId, Edt.Rows[0]["Taskid"].ToString(), ID);
                    ExaBll.AddExaminationTol(ID, tTaskid, UId, Competition, ssScore, State, UserNo, UsedTime);
                    commonbll.DeleteInfo("tb_CountDown", "  and CD_Custom3=2 and CD_EId='" + ID + "' and CD_PId='" + tTaskid + "' and CD_MId='" + UId + "'");
                }
            }
            Session["ResponseState"] = ResponseState;
            Session["ExaId"] = ID;
            var text_json = "[{'ResponseState':'" + ResponseState + "','Score':'" + Math.Round(Convert.ToDecimal(AlreadyScore) + longsores, 2) + "','Pattern':'" + Convert.ToInt32(Dt.Rows[0]["Pattern"]) + "','ExaId':'" + ID + "'}]";
            return text_json;
        }

        public ViewResult ShowAchievement()
        {
            return View();
        }

        /// <summary>
        /// 查找做题ID
        /// </summary>
        /// <returns></returns>
        public string See()
        {
            string Eid = Request["Eid"];
            DataTable Dt = ExaBll.See(Eid, UId);
            string AnswerId = "0";
            if (Dt.Rows.Count > 0)
            {
                AnswerId = Dt.Rows[0]["ID"].ToString();
            }
            return AnswerId;
        }
        /// <summary>
        /// 手工点钞 单项赛项排行
        /// </summary>
        /// <returns></returns>
        public string GetPH()
        {

            var Eid = Request["Eid"];
            var Pid = Request["Pid"];

            //1.找出我班级下所有的数据
            var list = @"ERId,ER_EId,ER_PId,ER_MId,
(select ExaminationName from tb_Manual_Examination where ID=a.ER_EId) as E_Name,
UserName,UserNo,StudentNo,ER_Custom3,ER_Score,
(select MAX(ER_Score) from tb_ExaminationResult where ER_Type=a.ER_Type and ER_EId=a.ER_EId  and ER_PId=a.ER_PId  and ER_MId=a.ER_MId) as lsMinScore";
            var table = "tb_ExaminationResult a inner join tb_UserInfo b on a.ER_MId=b.UId";
            var where = " and ER_Type=2 and ER_State=0 and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and UserClassId=" + UserClassId;
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
                    sb.Append("恭喜您！您的<span class=\"text_red\">手工点钞</span>排名为第<span class=\"text_red\">1</span>名，");
                    sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                    sb.Append("距离上一名还差<span class=\"text_red\">0</span>分！");
                }
                else
                {
                    //上一名得分-当前得分
                    var SYMScore = Convert.ToDecimal(dt.Rows[myidx - 1]["ER_Score"]) - Convert.ToDecimal(dt.Rows[myidx]["ER_Score"]);
                    if (SYMScore != 0)//得分与上一名不相同
                    {
                        sb.Append("您的<span class=\"text_red\">手工点钞</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                            sb.Append("您的<span class=\"text_red\">手工点钞</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                    sb.Append("您的<span class=\"text_red\">手工点钞</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                        sb.Append("您的<span class=\"text_red\">手工点钞</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                sb.Append("您的<span class=\"text_red\">手工点钞</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
        /// 查看方法-获取到考试答案与做题答案
        /// </summary>
        /// <returns></returns>
        public StringBuilder SG_AchievementList()
        {
            string AchievementId = Request["AchieID"];
            string Exaid = Request["ExaId"];
            StringBuilder Sb = new StringBuilder();
            //获取到做题答案数据
            DataTable AchieIdDt = commonbll.GetListDatatable("*", "tb_Manual_ExaminationAnswer", " and ID=" + AchievementId + "");

            //获取任务ID
            DataTable ExaDt = commonbll.GetListDatatable("ExaminationName,Taskid", "tb_Manual_Examination", " and ID=" + Exaid + "");
            string TaskID = "";
            if (ExaDt.Rows.Count > 0)
            {
                TaskID = ExaDt.Rows[0]["Taskid"].ToString();
            }
            //获取试卷名称和分值
            DataTable TaskDt = commonbll.GetListDatatable("TaskName,Score", "tb_Manual_Counting", " and ID=" + TaskID + "");

            //获取到学生的名称，同学，学校
            DataTable UserinfoDt = commonbll.GetListDatatable(" u.*,s.Name", "tb_UserInfo u left join tb_School s on u.UserSchoolId=s.Id", " and u.UId=" + UId + "");
            string Strnull = "";
            //获取答案明细
            DataTable AnswerDt = commonbll.GetListDatatable("*", "tb_Manual_CountingDetailed", " and TaskID=" + TaskID + "");
            Sb.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\"> <tr><th rowspan=\"2\">票面</th><th colspan=\"2\">张数</th><th colspan=\"2\">点钞值</th></tr> <tr><th>操作答案</th><th>正确答案</th><th>操作答案</th><th>正确答案</th></tr>");
            if (AnswerDt.Rows.Count > 0)
            {
                for (int i = 0; i < AnswerDt.Rows.Count; i++)
                {
                    if (AnswerDt.Rows[i]["TaskValue"].ToString() == "壹佰元")
                    {
                        Sb.Append("<tr><td>壹佰元</td>");
                        if (AchieIdDt.Rows[0]["HundredNumber"].ToString() != "")
                        {
                            if (Convert.ToInt32(AchieIdDt.Rows[0]["HundredNumber"]) == Convert.ToInt32(AnswerDt.Rows[i]["Number"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["HundredNumber"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["HundredNumber"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Number"] + "</td>");
                        if (AchieIdDt.Rows[0]["HundredMoney"].ToString() != "")
                        {
                            if (Convert.ToDecimal(AchieIdDt.Rows[0]["HundredMoney"]) == Convert.ToDecimal(AnswerDt.Rows[i]["Value"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["HundredMoney"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["HundredMoney"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Value"] + "</td></tr>");

                    }
                    if (AnswerDt.Rows[i]["TaskValue"].ToString() == "伍拾元")
                    {
                        Sb.Append("<tr><td>伍拾元</td>");
                        if (AchieIdDt.Rows[0]["FiftyNumber"].ToString() != "")
                        {
                            if (Convert.ToInt32(AchieIdDt.Rows[0]["FiftyNumber"]) == Convert.ToInt32(AnswerDt.Rows[i]["Number"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["FiftyNumber"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["FiftyNumber"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Number"] + "</td>");
                        if (AchieIdDt.Rows[0]["FiftyMoney"].ToString() != "")
                        {
                            if (Convert.ToDecimal(AchieIdDt.Rows[0]["FiftyMoney"]) == Convert.ToDecimal(AnswerDt.Rows[i]["Value"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["FiftyMoney"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["FiftyMoney"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Value"] + "</td></tr>");

                    }
                    if (AnswerDt.Rows[i]["TaskValue"].ToString() == "贰拾元")
                    {
                        Sb.Append("<tr><td>贰拾元</td>");
                        if (AchieIdDt.Rows[0]["TwentyNumber"].ToString() != "")
                        {
                            if (Convert.ToInt32(AchieIdDt.Rows[0]["TwentyNumber"]) == Convert.ToInt32(AnswerDt.Rows[i]["Number"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["TwentyNumber"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["TwentyNumber"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Number"] + "</td>");
                        if (AchieIdDt.Rows[0]["TwentyMoney"].ToString() != "")
                        {
                            if (Convert.ToDecimal(AchieIdDt.Rows[0]["TwentyMoney"]) == Convert.ToDecimal(AnswerDt.Rows[i]["Value"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["TwentyMoney"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["TwentyMoney"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Value"] + "</td></tr>");

                    }
                    if (AnswerDt.Rows[i]["TaskValue"].ToString() == "拾元")
                    {
                        Sb.Append("<tr><td>拾元</td>");
                        if (AchieIdDt.Rows[0]["TenNumber"].ToString() != "")
                        {
                            if (Convert.ToInt32(AchieIdDt.Rows[0]["TenNumber"]) == Convert.ToInt32(AnswerDt.Rows[i]["Number"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["TenNumber"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["TenNumber"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Number"] + "</td>");
                        if (AchieIdDt.Rows[0]["TenMoney"].ToString() != "")
                        {
                            if (Convert.ToDecimal(AchieIdDt.Rows[0]["TenMoney"]) == Convert.ToDecimal(AnswerDt.Rows[i]["Value"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["TenMoney"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["TenMoney"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Value"] + "</td></tr>");

                    }
                    if (AnswerDt.Rows[i]["TaskValue"].ToString() == "伍元")
                    {
                        Sb.Append("<tr><td>伍元</td>");
                        if (AchieIdDt.Rows[0]["FiveNumber"].ToString() != "")
                        {
                            if (Convert.ToInt32(AchieIdDt.Rows[0]["FiveNumber"]) == Convert.ToInt32(AnswerDt.Rows[i]["Number"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["FiveNumber"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["FiveNumber"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Number"] + "</td>");
                        if (AchieIdDt.Rows[0]["FiveMoney"].ToString() != "")
                        {
                            if (Convert.ToDecimal(AchieIdDt.Rows[0]["FiveMoney"]) == Convert.ToDecimal(AnswerDt.Rows[i]["Value"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["FiveMoney"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["FiveMoney"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Value"] + "</td></tr>");

                    }
                    if (AnswerDt.Rows[i]["TaskValue"].ToString() == "贰元")
                    {
                        Sb.Append("<tr><td>贰元</td>");
                        if (AchieIdDt.Rows[0]["BinaryNumber"].ToString() != "")
                        {
                            if (Convert.ToInt32(AchieIdDt.Rows[0]["BinaryNumber"]) == Convert.ToInt32(AnswerDt.Rows[i]["Number"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["BinaryNumber"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["BinaryNumber"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Number"] + "</td>");
                        if (AchieIdDt.Rows[0]["BinaryMoney"].ToString() != "")
                        {
                            if (Convert.ToDecimal(AchieIdDt.Rows[0]["BinaryMoney"]) == Convert.ToDecimal(AnswerDt.Rows[i]["Value"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["BinaryMoney"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["BinaryMoney"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Value"] + "</td></tr>");

                    }
                    if (AnswerDt.Rows[i]["TaskValue"].ToString() == "壹元")
                    {
                        Sb.Append("<tr><td>壹元</td>");
                        if (AchieIdDt.Rows[0]["OneNumber"].ToString() != "")
                        {
                            if (Convert.ToInt32(AchieIdDt.Rows[0]["OneNumber"]) == Convert.ToInt32(AnswerDt.Rows[i]["Number"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["OneNumber"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["OneNumber"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Number"] + "</td>");
                        if (AchieIdDt.Rows[0]["oneMoney"].ToString() != "")
                        {
                            if (Convert.ToDecimal(AchieIdDt.Rows[0]["oneMoney"]) == Convert.ToDecimal(AnswerDt.Rows[i]["Value"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["oneMoney"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["oneMoney"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Value"] + "</td></tr>");

                    }
                    if (AnswerDt.Rows[i]["TaskValue"].ToString() == "伍角")
                    {
                        Sb.Append("<tr><td>伍角</td>");
                        if (AchieIdDt.Rows[0]["fiftyfenNumber"].ToString() != "")
                        {
                            if (Convert.ToInt32(AchieIdDt.Rows[0]["fiftyfenNumber"]) == Convert.ToInt32(AnswerDt.Rows[i]["Number"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["fiftyfenNumber"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["fiftyfenNumber"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Number"] + "</td>");
                        if (AchieIdDt.Rows[0]["fiftyfenMoney"].ToString() != "")
                        {
                            if (Convert.ToDecimal(AchieIdDt.Rows[0]["fiftyfenMoney"]) == Convert.ToDecimal(AnswerDt.Rows[i]["Value"]))
                            {
                                Sb.Append("<td class=\"text_green\">" + AchieIdDt.Rows[0]["fiftyfenMoney"] + "</td>");
                            }
                            else
                            {
                                Sb.Append("<td class=\"text_red\">" + AchieIdDt.Rows[0]["fiftyfenMoney"] + "</td>");
                            }
                        }
                        else
                        {
                            Sb.Append("<td class=\"text_red\">" + Strnull + "</td>");
                        }
                        Sb.Append("<td class=\"text_green\">" + AnswerDt.Rows[i]["Value"] + "</td></tr>");

                    }


                }
            }
            Sb.Append("</table>");
            //考试名称
            Sb.Append("<input type=\"hidden\" id=\"ExaminationNameid\" value=" + ExaDt.Rows[0]["ExaminationName"].ToString() + ">");
            //考试分数
            Sb.Append("<input type=\"hidden\" id=\"Scoreid\" value=" + TaskDt.Rows[0]["Score"].ToString() + ">");
            //用户名
            Sb.Append("<input type=\"hidden\" id=\"UserNameid\" value=" + UserinfoDt.Rows[0]["UserName"].ToString() + ">");
            //用户头像
            if (UserinfoDt.Rows[0]["UserPic"] == null || UserinfoDt.Rows[0]["UserPic"].ToString() == "")
            {
                Sb.Append(" <input type=\"hidden\" id=\"UserPicid\" value=\"/img/profile_s.jpg\">");
            }
            else
            {
                Sb.Append("<input type=\"hidden\" id=\"UserPicid\" value=" + UserinfoDt.Rows[0]["UserPic"].ToString() + ">");
            }
            //院校名称
            Sb.Append("<input type=\"hidden\" id=\"Schoolid\" value=" + UserinfoDt.Rows[0]["Name"].ToString() + ">");
            //任务得分
            Sb.Append("<input type=\"hidden\" id=\"AchieScoreid\" value=" + AchieIdDt.Rows[0]["TotalScore"].ToString() + ">");
            return Sb;
        }
    }
}
