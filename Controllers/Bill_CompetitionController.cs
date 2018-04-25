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
using VocationalProject_DBUtility.Common;
using VocationalProject_Models;

namespace VocationalProject.Controllers
{
    /***************************************************************
   FileName:单据录入 学生端 考试做题和竞赛列表
   Copyright（c）2017-金融教育在线技术开发部
   Author:陈飞
   Create Date:2017-4-14
   ******************************************************************/
    public class Bill_CompetitionController : BaseController
    {

        CommonBll commonbll = new CommonBll();
        ExaminationDetailsBll examBll = new ExaminationDetailsBll();
        Operation_AnswerBll oaBll = new Operation_AnswerBll();
        ExaminationResultBll erBll = new ExaminationResultBll();
        //
        // GET: /Bill_Competition/
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult GrowthProcess()
        {
            return View();
        }

        /// <summary>
        /// 未完成竞赛列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetList()
        {
            string wheres = " and a.Bill_Spare1=1 and a.ID not in (select ER_EId from tb_ExaminationResult where ER_MId=" + UId + " and ER_Type=4) and  (GradeID like '%," + UserClassId + ",%' or GradeID='' or GradeID like'%undefined%' )";
            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")//竞赛模式
            {
                wheres += " and ExamPattern='" + Request["E_Type"].ToString() + "'";
            }
            //竞赛名称
            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)
            {
                wheres += " and ExamName like '%" + Request["E_Name"] + "%'";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.id desc,endTime desc"; //排序必须填写
            m.strFld = " a.*,(select SUM(Score) from tb_Bill_TestPaperEven where PaperID=a.PaperID) as Score";
            m.tab = "tb_Bill_Exam a";
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
                    var jsshishijian = Convert.ToDateTime(dt.Rows[i]["endTime"].ToString());//有效结束时间
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
                    sb.Append("<span>" + dt.Rows[i]["ExamName"] + "</span>");

                    if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                    }
                    if (dqshijian.AddMinutes(10) >= ksshishijian && dqshijian <= jsshishijian)
                    {
                        sb.Append("<button class=\"btn_go\"  onclick=\"Getinto(" + dt.Rows[i]["ID"] + "," + dt.Rows[i]["PaperID"] + ")\" type=\"button\">进入</button>");
                    }
                    if (dqshijian.AddMinutes(10) < ksshishijian)//当前时间 小于考试开始时间 考试HIA没有开始 
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">未开始</button>");
                    }
                    sb.Append("</div>");
                    sb.Append(" <div class=\"list_title_smal\">");
                    if (dt.Rows[i]["ExamPattern"].ToString() == "2")//考试模式
                    {
                        sb.Append("<label>试卷类型：考试模式</label>");
                    }
                    else
                    {
                        sb.Append("<label>试卷类型：练习模式</label>");
                    }
                    sb.Append("<label>试卷总分：" + dt.Rows[i]["Score"] + "分</label>");
                    sb.Append("<label>试卷有效时间：" + dt.Rows[i]["StartTime"].ToString() + " —— " + dt.Rows[i]["endTime"].ToString() + "  </label>");
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
            string wheres = " and a.Bill_Spare1=1 and (GradeID like '%," + UserClassId + ",%' or GradeID='' or GradeID like'%undefined%' ) and Id  in (select ER_EId from tb_ExaminationResult where ER_MId=" + UId + " and ER_Type=4)";
            if (Request["E_TypeTo"] != null && Request["E_TypeTo"].ToString() != "0")//竞赛模式
            {
                wheres += " and ExamPattern='" + Request["E_TypeTo"].ToString() + "'";
            }
            //竞赛名称
            if (Request["E_NameTo"] != null && Request["E_NameTo"].ToString().Length > 0)
            {
                wheres += " and ExamName like '%" + Request["E_NameTo"] + "%'";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "[ExamPattern],[endTime] desc"; //排序必须填写
            m.strFld = @" a.*,(select SUM(Score) from tb_Bill_TestPaperEven where PaperID=a.PaperID) as Score,
 (select top 1 ER_Score from tb_ExaminationResult where ER_MId=" + UId + @" and ER_EId=a.ID and ER_Type=4 and ER_State=0 order by ERId desc) as CZScore,
 (select top 1 ER_AddTime from tb_ExaminationResult where ER_MId=" + UId + @" and ER_EId=a.ID and ER_Type=4 and ER_State=0 order by ERId desc) as ER_AddTime";
            m.tab = "tb_Bill_Exam a";
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
                    var jsshishijian = Convert.ToDateTime(dt.Rows[i]["endTime"].ToString());//有效结束时间

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
                    sb.Append("<a href=\"#chart-personPK\" onclick='ViewGrowthTrajectory(" + dt.Rows[i]["ID"] + ")'>" + dt.Rows[i]["ExamName"] + "</a>");

                    if (dt.Rows[i]["ExamPattern"].ToString() == "2")//考试模式 直接已结束
                    {
                        sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                    }
                    if (dt.Rows[i]["ExamPattern"].ToString() == "1")//练习模式
                    {
                        if (dqshijian > jsshishijian)//当前时间大于结束时间了都  显示结束
                        {
                            sb.Append("<button class=\"btn_nogo\" type=\"button\">已结束</button>");
                        }

                        if (dqshijian >= ksshishijian && dqshijian <= jsshishijian)
                        {
                            sb.Append("<button class=\"btn_ph\" onclick='PH(" + dt.Rows[i]["ID"] + "," + dt.Rows[i]["PaperID"] + ")' type=\"button\">排行榜</button>");
                            sb.Append("<button class=\"btn_look\" onclick='Viewdetails(" + dt.Rows[i]["ID"] + "," + dt.Rows[i]["PaperID"] + ")' type=\"button\">查看明细</button>");
                            sb.Append("<button class=\"btn_go2\" onclick=\"GetintoTwo(" + dt.Rows[i]["ID"] + "," + dt.Rows[i]["PaperID"] + ")\" type=\"button\">进入</button>");
                        }

                    }
                    sb.Append("</div>");
                    sb.Append(" <div class=\"list_title_smal\">");
                    if (dt.Rows[i]["ExamPattern"].ToString() == "2")//考试模式
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
                    sb.Append("<label>试卷有效时间：" + dt.Rows[i]["StartTime"].ToString() + " —— " + dt.Rows[i]["endTime"].ToString() + "  </label>");
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
                        DataTable dt = commonbll.GetListDatatable("SUM(ER_Score) as ER_Score,count(ER_Score) as num", "tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_MId=" + UId + " and ER_Type=4 and datediff(day,ER_AddTime,'" + datetlist[i] + "')=0");
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
            string EName = commonbll.GetListSclar("ExamName", "tb_Bill_Exam", " and Id=" + Eid);

            var json = new object[] {
                        new{
                            scoresstr=scoresstr,
                            EName=EName
                        }
                    };
            return JsonConvert.SerializeObject(json);
        }

        /// <summary>
        /// 做题
        /// </summary>
        /// <returns></returns>
        public ActionResult Examination(int ExamId, int PaperId)
        {
            string line = @"a.*,(select SUM(Score) from tb_Bill_TestPaperEven where PaperID=a.PaperID) as Score,
  (select b.Name from tb_UserInfo a inner join tb_School  b on a.UserSchoolId=b.Id where a.UId=" + UId + ") as SchoolName";
            DataTable data = commonbll.GetListDatatable(line, "tb_Bill_Exam  as a", " and a.id=" + ExamId);
            if (data.Rows.Count > 0)
            {
                ViewBag.UserName = commonbll.GetListSclar("UserName", "tb_UserInfo", " and UId=" + UId);
                ViewBag.UserPic = commonbll.GetListSclar("UserPic", "tb_UserInfo", " and UId=" + UId);
                ViewBag.SchoolName = data.Rows[0]["SchoolName"].ToString();
                ViewBag.ExamName = data.Rows[0]["ExamName"].ToString();
                ViewBag.Score = data.Rows[0]["Score"].ToString();
                ViewBag.TopicCount = commonbll.GetRecordCount("tb_Bill_TestPaperEven", " and PaperID=" + PaperId);
            }
            return View();
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
            // int ExamPattern = Convert.ToInt32(commonbll.GetListSclar("ExamPattern", "tb_Bill_Exam", " and ID=" + ExamId));
            //有效开始时间
            DateTime StartTime = Convert.ToDateTime(commonbll.GetListSclar("StartTime", "tb_Bill_Exam", " and ID=" + ExamId));
            //当前模式是考试模式
            //if (ExamPattern == 2)
            //{
            if (OperatorTime < StartTime)
            {
                Start = 1;
            }
            // }
            return JsonConvert.SerializeObject(Start);
        }

        /// <summary>
        /// 查询有多少个题目数量
        /// </summary>
        /// <returns></returns>
        public string DataBind()
        {
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            StringBuilder sb = new StringBuilder();
            string whe = string.Empty;
            //是否打乱顺序
            string Sequence = commonbll.GetListSclar("Sequence", "tb_Bill_TestPaper", " and ID=" + PaperId);
            if (Sequence == "1")
            {
                whe = " order by newid()";
            }
            DataTable data = commonbll.GetListDatatable("*", "tb_Bill_TestPaperEven as a inner join  tb_Bill_Topic as b  on a.TopicID=b.ID", " and PaperID=" + PaperId + whe);
            //循环试卷中的所有题目
            for (int i = 0; i < data.Rows.Count; i++)
            {
                ////判断session里面的值是否等于题目ID
                //if (Session["Topic_" + ExamId + ""] != null)
                //{
                //    if (Session["Topic_" + ExamId + ""].ToString() == data.Rows[i]["TopicID"].ToString())
                //    {
                //        //当前回答
                //        sb.Append("<li class=\"num_sty\" data-erial='" + (i + 1) + "' data-Type='0' TopicId='" + data.Rows[i]["TopicID"] + "' FormId='" + data.Rows[i]["BillFormId"] + "'  onclick='Topic(" + (i + 1) + ")'><span class=\"num_bj color_now\">" + (i + 1) + "</span></li>");
                //        continue;
                //    }
                //}
                ////查询成绩明细表
                int RowCount = commonbll.GetRecordCount("tb_ExaminationDetails", " and ED_MId=" + UId + " and ED_EId=" + ExamId + " and ED_PId=" + PaperId + " and ED_QBId=" + data.Rows[i]["TopicID"] + " and ED_State=0 and ED_Type=4");
                //判断查询后的总数大于0表示题目已经做过进入了成绩明细表中
                if (RowCount > 0)
                {
                    //已回答
                    sb.Append("  <li class=\"num_sty\" data-erial='" + (i + 1) + "' data-Type='1' TopicId='" + data.Rows[i]["TopicID"] + "' FormId='" + data.Rows[i]["BillFormId"] + "'   onclick='Topic(" + (i + 1) + ")'><span class=\"num_bj color_sure\">" + (i + 1) + "</span></li>");
                }
                else
                {
                    //未回答
                    sb.Append("<li class=\"num_sty\" data-erial='" + (i + 1) + "' data-Type='0'  TopicId='" + data.Rows[i]["TopicID"] + "'  FormId='" + data.Rows[i]["BillFormId"] + "' onclick='Topic(" + (i + 1) + ")'><span class=\"num_bj color_no\">" + (i + 1) + "</span></li>");
                }
            }
            int Second = 0;

            //操作时间
            DateTime Time = Convert.ToDateTime(commonbll.GetListSclar("CD_Time", "tb_CountDown", " and CD_Custom3=4 and CD_EId=" + ExamId + " and CD_PId=" + PaperId + " and CD_MId=" + UId + ""));
            //考试时长
            int ExamLength = Convert.ToInt32(commonbll.GetListSclar("TimeLong", "tb_Bill_Exam", " and ID=" + ExamId));
            DateTime BeginTime = Time.AddMinutes(ExamLength);
            DateTime CurrentTime = DateTime.Now;
            if (CurrentTime < BeginTime)
            {
                DateTime dt1 = CurrentTime;
                DateTime dt2 = BeginTime;
                TimeSpan ts = dt2.Subtract(dt1);
                Second = Convert.ToInt32(ts.TotalSeconds);//秒差
            }
            //转换成json
            var json = new object[] {
                       new{
                            str=sb.ToString(),
                            Second=Second,
                        }
            };
            return JsonConvert.SerializeObject(json);
        }

        /// <summary>
        /// 单个题目的显示
        /// </summary>
        /// <returns></returns>
        public string SingleTopic()
        {
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            string TopicId = Request["TopicId"];
            var data = new DataTable();
            data = commonbll.GetListDatatable(" a.*,b.BillFormId,b.TaskExplan,b.TopicTitle ", "tb_Bill_TestPaperEven as a inner join  tb_Bill_Topic as b  on a.TopicID=b.ID", " and PaperID=" + PaperId + " and TopicID= " + TopicId);
            string BillStyle = Dictionary.GetDictStyle(data.Rows[0]["BillFormId"].ToString());
            Session["Topic_" + Request["ExamId"] + ""] = TopicId;
            //转换成json
            var json = new object[] {
                       new{
                            BillForm=data.Rows[0]["BillFormId"].ToString(),
                            TaskExplan=data.Rows[0]["TaskExplan"].ToString(),
                            TopicId=data.Rows[0]["TopicID"].ToString(),
                            BillStyle=BillStyle,
                            SingleScore=data.Rows[0]["Score"].ToString(),
                            TopicTitle=data.Rows[0]["TopicTitle"].ToString(),
                           // PageTotal=PageCount,
                           // PageIndex=m.PageIndex
                        }
            };
            return JsonConvert.SerializeObject(json);
        }

        /// <summary>
        /// 单个题目的保存计算单个得分
        /// </summary>tb_Bill_Exam
        /// <returns></returns>
        public string SingleTopicSave()
        {
            int rows = 0;
            string FormId = Request["FormId"];
            DataTable tbExam = commonbll.GetListDatatable("ExamPattern", "tb_Bill_Exam", " and ID=" + Request["ExamId"]);
            if (tbExam != null && tbExam.Rows.Count > 0)
            {
                if (tbExam.Rows[0]["ExamPattern"].ToString() == "2")
                {
                    var checkcount = commonbll.GetRecordCount("tb_ExaminationResult", " and ER_EId=" + Request["ExamId"] + " and ER_PId='" + Request["PaperId"] + "' and ER_MId=" + UId + " and ER_Type=4 and ER_State=0");
                    if (checkcount > 0)
                    {
                        return "88";
                    }
                }
            }


            if (FormId != "")
            {
                Session["Pid_M"] = Request["PaperId"];
                Session["Eid_M"] = Request["ExamId"];
                DataTable data = commonbll.GetListDatatable("*", "tb_Bill_Form", " and BillFormId='" + FormId + "' ");
                string Score = commonbll.GetListSclar("Score", "tb_Bill_TestPaperEven", " and PaperID=" + Request["PaperId"] + " and TopicID=" + Request["TopicId"] + "");
                int RowCount = commonbll.GetRecordCount("tb_Bill_Answer", " and BillFormId='" + FormId + "' and Bill_Spare1=" + Request["TopicId"] + " and Answer is not null and Answer<>''");
                decimal Fraction = 0;
                if (RowCount > 0)
                {
                    Fraction = Convert.ToDecimal(Score) / RowCount;
                }
                decimal SingleScore = 0;
                int RowC = commonbll.GetRecordCount("tb_Operation_Answer", " and EId=" + Convert.ToInt32(Request["ExamId"]) + " and QBId=" + Convert.ToInt32(Request["TopicId"]) + " and PId=" + Convert.ToInt32(Request["PaperId"]) + " and MId=" + UId + " and [Type]=4 and [State]=0");
                if (RowC > 0)
                {
                    commonbll.DeleteInfo("tb_Operation_Answer", " and EId=" + Convert.ToInt32(Request["ExamId"]) + " and QBId=" + Convert.ToInt32(Request["TopicId"]) + "  and PId=" + Convert.ToInt32(Request["PaperId"]) + " and MId=" + UId + " and [Type]=4 and [State]=0");
                }
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    string OkNo = string.Empty;
                    string SingleValue = Request[data.Rows[i]["EnglishName"].ToString()];
                    string Answer = commonbll.GetListSclarNull("Answer", "tb_Bill_Answer", " and BillFormId='" + FormId + "' and Bill_Spare1=" + Request["TopicId"] + " and EnglishName='" + data.Rows[i]["EnglishName"] + "'");
                    //if (Answer != "" && Answer != null)
                    //{
                    if (SingleValue != null && SingleValue != "")
                    {
                        if (SingleValue.ToString().Trim() == Answer.Trim())
                        {
                            SingleScore += Fraction;
                            OkNo = "正确";
                        }
                        else
                        {
                            OkNo = "错误";
                        }
                    }
                    else
                    {
                        if (SingleValue != null)
                        {
                            if (SingleValue.ToString().Trim() != Answer.Trim())
                            {
                                OkNo = "错误";
                            }
                        }
                    }
                    //else
                    //{
                    //    if (Answer == "" || Answer == null)
                    //    {
                    //        SingleScore += Fraction;
                    //    }
                    //}
                    tb_Operation_Answer oa = new tb_Operation_Answer();
                    oa.EId = Convert.ToInt32(Request["ExamId"]);
                    oa.PId = Convert.ToInt32(Request["PaperId"]);
                    oa.QBId = Convert.ToInt32(Request["TopicId"]);
                    oa.MId = Convert.ToInt32(UId);
                    oa.Type = 4;
                    oa.OperationAnswer = SingleValue;
                    oa.OkNo = OkNo;
                    oa.EnglishName = data.Rows[i]["EnglishName"].ToString();
                    oa.BillFormId = FormId;
                    oa.RightAnswer = Answer;
                    oa.OperationTime = DateTime.Now;
                    oa.State = 0;
                    oa.Spare1 = null;
                    oa.Spare2 = null;
                    //if (Answer == "" || Answer == null || OkNo == "错误")
                    //{
                    //    oa.Spare3 = null;
                    //}
                    //else
                    //{
                    oa.Spare3 = Math.Round(Fraction, 2).ToString();
                    //}
                    oaBll.AddOperation_AnswerBll(oa);
                    //}
                }
                //查询成绩明细表
                int Details = commonbll.GetRecordCount("tb_ExaminationDetails", " and ED_EId=" + Request["ExamId"] + " and ED_PId=" + Request["PaperId"] + " and ED_QBId=" + Request["TopicId"] + " and ED_MId=" + UId + " and ED_Type=4 and ED_State=0");
                if (Details <= 0)
                {
                    tb_ExaminationDetails tb = new tb_ExaminationDetails();
                    tb.ED_EId = Convert.ToInt32(Request["ExamId"]);
                    tb.ED_PId = Convert.ToInt32(Request["PaperId"]);
                    tb.ED_QBId = Convert.ToInt32(Request["TopicId"]);
                    tb.ED_MId = Convert.ToInt32(UId);
                    tb.ED_Type = 4;
                    tb.ED_Content = null;
                    tb.ED_OkNo = null;
                    tb.ED_Goal = SingleScore;
                    tb.ED_State = 0;
                    tb.ED_Operator = UId;
                    tb.ED_AddTime = DateTime.Now;
                    tb.ED_Custom1 = null;
                    tb.ED_Custom2 = null;
                    tb.ED_Custom3 = null;
                    rows = examBll.AddExaminationDetailsBll(tb);
                }
                else
                {
                    string wheres = " and ED_EId=" + Request["ExamId"] + " and ED_PId=" + Request["PaperId"] + " and ED_QBId=" + Request["TopicId"] + " and ED_MId=" + UId + " and ED_Type=4 and ED_State=0";
                    SqlParameter[] para = new SqlParameter[]{
                    new SqlParameter("@ED_Goal",SingleScore)
                };
                    rows = commonbll.UpdateInfo("tb_ExaminationDetails", "ED_Goal=@ED_Goal", wheres, para);
                }
            }
            return JsonConvert.SerializeObject(rows);

        }

        /// <summary> 
        /// 读取表单定位数据
        /// </summary>
        /// <returns></returns>
        public string Bills(string FormId)
        {
            return commonbll.GetListSclar("Bill_Spare3", "tb_Bills", " and BillFormId='" + FormId + "'");
        }

        /// <summary>
        ///读取题目操作数据 
        /// </summary>
        /// <returns></returns>
        public string Bill_Data()
        {
            DataTable data = commonbll.GetListDatatable("*", "[tb_Operation_Answer]", " and EId=" + Request["ExamId"] + " and  PId='" + Request["PaperId"] + "' and QBId=" + Request["TopicId"] + " and MId=" + UId + " and BillFormId='" + Request["FormId"] + "' and [Type]=4 and State=0");
            return JsonConvert.SerializeObject(data);
        }
        public string Bill_DataTwo()
        {
            DataTable data = commonbll.GetListDatatable("*", "[tb_Operation_Answer]", " and EId=" + Request["ExamId"] + " and  PId='" + Request["PaperId"] + "' and QBId=" + Request["TopicId"] + " and MId=" + UId + " and BillFormId='" + Request["FormId"] + "' and [Type]=4 and State=1");
            return JsonConvert.SerializeObject(data);
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
            DateTime StartTime = Convert.ToDateTime(commonbll.GetListSclar("StartTime", "tb_Bill_Exam", " and ID=" + ExamId));
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

        /// <summary>
        /// 单个题目选中
        /// </summary>
        /// <returns></returns>
        public string Topic_Data()
        {
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            string TopicId = Request["TopicId"];
            StringBuilder sb = new StringBuilder();
            int PageCount = 0;//总数
            PageModel m = new PageModel();
            DataTable data = commonbll.GetListDatatable("*", "tb_Bill_TestPaperEven as a inner join  tb_Bill_Topic as b  on a.TopicID=b.ID", " and PaperID=" + PaperId);
            //循环试卷中的所有题目
            for (int i = 0; i < data.Rows.Count; i++)
            {
                //判断session里面的值是否等于题目ID
                if (TopicId == data.Rows[i]["TopicID"].ToString())
                {
                    //当前回答
                    sb.Append("<li class=\"num_sty\" onclick='Topic(" + data.Rows[i]["TopicID"] + ")'><span class=\"num_bj color_now\">" + (i + 1) + "</span></li>");
                    continue;
                }
                //查询成绩明细表
                int RowCount = commonbll.GetRecordCount("tb_ExaminationDetails", " and ED_EId=" + ExamId + " and ED_PId=" + PaperId + " and ED_QBId=" + data.Rows[i]["TopicID"] + " and ED_State=0 and ED_Type=4");
                //判断查询后的总数大于0表示题目已经做过进入了成绩明细表中
                if (RowCount > 0)
                {
                    //已回答
                    sb.Append("  <li class=\"num_sty\" onclick='Topic(" + data.Rows[i]["TopicID"] + ")'><span class=\"num_bj color_sure\">" + (i + 1) + "</span></li>");
                }
                else
                {
                    //未回答
                    sb.Append("<li class=\"num_sty\" onclick='Topic(" + data.Rows[i]["TopicID"] + ")'><span class=\"num_bj color_no\">" + (i + 1) + "</span></li>");
                }
            }
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 1;
            m.Sort = "a.Id"; //排序必须填写
            m.strFld = " a.*,b.BillFormId,b.TaskExplan,b.TopicTitle ";
            m.tab = "tb_Bill_TestPaperEven as a inner join  tb_Bill_Topic as b  on a.TopicID=b.ID";
            m.strWhere = " and PaperID=" + PaperId + " and TopicID=" + TopicId;
            data = Pager.GetList(m, ref PageCount);
            var list = JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, data);
            string BillStyle = Dictionary.GetDictStyle(data.Rows[0]["BillFormId"].ToString());
            Session["Topic_" + Request["ExamId"] + ""] = data.Rows[0]["TopicID"].ToString();
            Session["page" + Request["ExamId"] + ""] = m.PageIndex;
            //转换成json
            var json = new object[] {
                       new{
                            StrSb=sb.ToString(),
                            BillForm=data.Rows[0]["BillFormId"].ToString(),
                            TaskExplan=data.Rows[0]["TaskExplan"].ToString(),
                            TopicId=data.Rows[0]["TopicID"].ToString(),
                            BillStyle=BillStyle,
                            SingleScore=data.Rows[0]["Score"].ToString(),
                            TopicTitle=data.Rows[0]["TopicTitle"].ToString(),
                            PageTotal=PageCount,
                            PageIndex=m.PageIndex
                        }
            };
            return JsonConvert.SerializeObject(json);
        }

        /// <summary>
        /// 总提交
        /// </summary>
        /// <returns></returns>
        public int Jump()
        {
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            string TopicId = Request["TopicId"];
            string Time = Request["Time"]; //倒计时剩余时间
            string SumTime = Request["SumTime"];//倒计时总时间
            string UsedTime = "00:00:00";

            DataTable tbExam = commonbll.GetListDatatable("ExamPattern", "tb_Bill_Exam", " and ID=" + ExamId);
            if (tbExam != null && tbExam.Rows.Count > 0)
            {
                if (tbExam.Rows[0]["ExamPattern"].ToString() == "2")
                {
                    var checkcount = commonbll.GetRecordCount("tb_ExaminationResult", " and ER_EId=" + ExamId + " and ER_PId='" + PaperId + "' and ER_MId=" + UId + " and ER_Type=4 and ER_State=0");
                    if (checkcount > 0)
                    {
                        return 88;
                    }
                }
            }
            Session["Pid_M"] = PaperId;
            Session["Eid_M"] = ExamId;

            //计算考试用时
            string Pid = PaperId;
            var whenlong = "00:00:00";
            var Towhenlong = 0.00;//存储做题总时间总秒
            DataTable timeDt = commonbll.GetListDatatable("CD_Time", "tb_CountDown", "  and CD_Custom3=4 and CD_EId=" + ExamId + " and CD_PId='" + Pid + "' and CD_MId=" + UId);
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
            SqlParameter[] para = new SqlParameter[] { };
            //把以前的总成绩记录更新成旧数据
            commonbll.UpdateInfo("tb_ExaminationResult", "ER_State=1", " and ER_EId=" + ExamId + " and ER_PId=" + PaperId + " and ER_MId=" + UId + " and ER_Type=4 and ER_State=0", para);
            decimal SingleScore = 0;
            //根据成绩明细累加考试中试卷的所有单个题目成绩
            DataTable data = commonbll.GetListDatatable("*", "tb_ExaminationDetails", " and ED_EId=" + ExamId + " and ED_PId=" + PaperId + " and ED_MId=" + UId + " and ED_Type=4 and ED_State=0");
            for (int i = 0; i < data.Rows.Count; i++)
            {
                SingleScore += Convert.ToDecimal(data.Rows[i]["ED_Goal"]);
            }
            //总分表插入新的总分数据
            tb_ExaminationResult er = new tb_ExaminationResult();
            er.ER_EId = Convert.ToInt32(Request["ExamId"]);
            er.ER_PId = Convert.ToInt32(Request["PaperId"]);
            er.ER_MId = Convert.ToInt32(UId);
            er.ER_Type = 4;
            er.ER_Score = Math.Round(SingleScore, 2);
            er.ER_State = 0;
            er.ER_Operator = UId;
            er.ER_AddTime = DateTime.Now;
            er.ER_Custom1 = null;
            er.ER_Custom2 = UserNo;
            er.ER_Custom3 = UsedTime;
            erBll.AddExaminationResultBll(er);
            ////删除旧的操作数据记录 and EId=" + ExamId + " and PId=" + PaperId + "
            commonbll.DeleteInfo("tb_Operation_Answer", "  and MId=" + UId + " and [Type]=4 and [State]=1");
            //更新操作数据记录
            commonbll.UpdateInfo("tb_Operation_Answer", "[State]=1", " and MId=" + UId + " and [Type]=4 and [State]=0", para);
            ////删除旧的成绩明细记录
            commonbll.DeleteInfo("tb_ExaminationDetails", " and ED_EId=" + ExamId + " and ED_PId=" + PaperId + "  and ED_MId=" + UId + " and ED_Type=4 and ED_State=1");
            //更新成绩明细记录
            commonbll.UpdateInfo("tb_ExaminationDetails", "ED_State=1", " and ED_EId=" + ExamId + " and ED_PId=" + PaperId + " and ED_MId=" + UId + " and ED_Type=4 and ED_State=0", para);
            ////删除旧的总成绩记录
            //commonbll.DeleteInfo("tb_ExaminationResult", " and ER_EId=" + ExamId + " and ER_PId=" + PaperId + " and ER_MId=" + UId + " and ER_Type=4 and ER_State=1");
            string TimePlus = commonbll.GetListSclar("TimePlus", "tb_Bill_Exam", " and ID=" + ExamId + "");
            //试卷总分
            decimal ExaminationScore = Convert.ToDecimal(commonbll.GetListSclar("sum(Score) as ExaminationScore", "tb_Bill_TestPaperEven", " and PaperID=" + PaperId + ""));
            //计算出的加分数
            decimal Score = TimeBonus.TimePlus(ExamId, PaperId, UId, Time, TimePlus, SingleScore, ExaminationScore);
            erBll.UpdaExaminationResultBll(Score, Convert.ToInt32(ExamId), Convert.ToInt32(PaperId), Convert.ToInt32(UId));
            return Convert.ToInt32(commonbll.GetListSclar("ExamPattern", "tb_Bill_Exam", " and ID=" + ExamId));
        }

        /// <summary>
        /// 成绩明细
        /// </summary>
        /// <returns></returns>
        public ActionResult ScoreDetails(int ExamId, int PaperId, int UIds)
        {
            if (UIds == 0)
            {
                UIds = Convert.ToInt32(UId);
            }
            ViewBag.UIds = UIds;
            string line = @"a.*,(select SUM(Score) from tb_Bill_TestPaperEven where PaperID=a.PaperID) as Score";
            DataTable data = commonbll.GetListDatatable(line, "tb_Bill_Exam  as a", " and a.id=" + ExamId);
            if (data.Rows.Count > 0)
            {
                ViewBag.UserName = commonbll.GetListSclar("UserName", "tb_UserInfo", " and UId=" + UIds);
                ViewBag.UserPic = commonbll.GetListSclar("UserPic", "tb_UserInfo", " and UId=" + UIds);
                ViewBag.SchoolName = commonbll.GetListSclar("b.Name", "tb_UserInfo a inner join tb_School  b on a.UserSchoolId=b.Id", " and a.UId=" + UIds);
                ViewBag.ExamName = data.Rows[0]["ExamName"].ToString();
                ViewBag.Score = data.Rows[0]["Score"].ToString();
                ViewBag.TopicCount = commonbll.GetRecordCount("tb_Bill_TestPaperEven", " and PaperID=" + PaperId);
            }
            //ViewBag.ExamScore = commonbll.GetListSclar("SUM(ED_Goal) as Score", "tb_ExaminationDetails", " and ED_EId=" + ExamId + " and ED_PId=" + PaperId + " and ED_MId=" + UIds + " and ED_Type=4 and ED_State=1");
            ViewBag.ExamScore = commonbll.GetListSclar("ER_Score", "tb_ExaminationResult", " and ER_EId=" + ExamId + " and ER_PId=" + PaperId + " and ER_MId=" + UIds + " and ER_Type=4 and ER_State=0");
            StringBuilder sb = new StringBuilder();
            data = commonbll.GetListDatatable("*", "tb_Bill_TestPaperEven as a inner join  tb_Bill_Topic as b  on a.TopicID=b.ID", " and PaperID=" + PaperId + "");
            //data = commonbll.GetListDatatable("a.*,b.BillFormId ", "tb_ExaminationDetails as a inner join tb_Bill_Topic as b on a.ED_QBId=b.ID", " and ED_EId=" + ExamId + " and ED_PId=" + PaperId + " and ED_MId=" + UId + " and ED_State=1 order by a.EDId desc");
            for (int i = 0; i < data.Rows.Count; i++)
            {
                int Rows = commonbll.GetRecordCount("tb_Operation_Answer", " and Mid=" + UIds + " and EId=" + ExamId + " and PId=" + PaperId + " and QBId=" + data.Rows[i]["TopicId"] + " and [Type]=4 and  OkNo='错误' and [State]=1  and RightAnswer<>'' ");
                int RowCount = commonbll.GetRecordCount("tb_Operation_Answer", "and Mid=" + UIds + "  and EId=" + ExamId + " and PId=" + PaperId + " and QBId=" + data.Rows[i]["TopicId"] + " and [Type]=4  and [State]=1");
                if (RowCount == 0)
                {
                    sb.Append("<li class=\"num_sty\" TopicId='" + data.Rows[i]["TopicId"] + "' FormId='" + data.Rows[i]["BillFormId"] + "' ExamId='" + ExamId + "' PaperId='" + PaperId + "' UId='" + UIds + "' onclick='Topic(" + (i + 1) + ")'><span class=\"num_bj color_no\">" + (i + 1) + "</span></li>");
                }
                if (Rows > 0)
                {
                    sb.Append("<li class=\"num_sty\" TopicId='" + data.Rows[i]["TopicId"] + "' FormId='" + data.Rows[i]["BillFormId"] + "'  ExamId='" + ExamId + "' PaperId='" + PaperId + "' UId='" + UIds + "' onclick='Topic(" + (i + 1) + ")'><span class=\"num_bj color_now \">" + (i + 1) + "</span></li>");
                }
                if (Rows <= 0 && RowCount != 0)
                {
                    sb.Append("<li class=\"num_sty\" TopicId='" + data.Rows[i]["TopicId"] + "' FormId='" + data.Rows[i]["BillFormId"] + "'  ExamId='" + ExamId + "' PaperId='" + PaperId + "' UId='" + UIds + "' onclick='Topic(" + (i + 1) + ")'><span class=\"num_bj color_look_sure\">" + (i + 1) + "</span></li>");
                }
            }
            ViewBag.ExamSb = sb.ToString();
            ViewBag.ExamId = ExamId;
            ViewBag.PaperId = PaperId;
            return View();
        }

        /// <summary>
        /// 成绩明细
        /// </summary>
        /// <returns></returns>
        public string ScoreDetailsTwo()
        {
            string FormId = Request["FormId"];
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            string TopicId = Request["TopicId"];
            string BillStyle = Dictionary.GetDictStyle(FormId);
            DataTable data = commonbll.GetListDatatable("a.*,b.BillFormId,b.TaskExplan,b.TopicTitle", "tb_Bill_TestPaperEven as a inner join  tb_Bill_Topic as b  on a.TopicID=b.ID", " and PaperID=" + PaperId + " and TopicID=" + TopicId);
            //转换成json
            var json = new object[] {
                       new{
                            BillStyle=BillStyle,
                            TaskExplan=data.Rows[0]["TaskExplan"].ToString(),
                            TopicTitle= data.Rows[0]["TopicTitle"].ToString(),
                            SingleScore=data.Rows[0]["Score"].ToString(),
                        }
            };
            return JsonConvert.SerializeObject(json);
        }

        /// <summary>
        /// 成绩明细列表
        /// </summary>
        /// <returns></returns>
        public string OperationDetails()
        {
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            string TopicId = Request["TopicId"];
            string Uids = Request["Uids"];
            StringBuilder sb = new StringBuilder();
            if (Uids == "" || Uids == null || Uids == "0")
            {
                Uids = UId;
            }
            DataTable data = commonbll.GetListDatatable("a.*,b.ChineseName", "tb_Operation_Answer as a  inner join tb_Bill_Form as b on a.BillFormId=b.BillFormId and a.EnglishName=b.EnglishName", " and a.EId=" + ExamId + " and a.PId=" + PaperId + " and a.QBId=" + TopicId + " and a.MId=" + Uids + " and a.[Type]=4 and a.[State]=1and RightAnswer <>'' and RightAnswer is not NULL");
            sb.Append("<tr><th>序号</th><th>考点</th><th>输入答案</th><th>正确答案</th>	<th>状态</tr>");
            for (int i = 0; i < data.Rows.Count; i++)
            {
                sb.Append("	<tr><td>" + (i + 1) + "</td><td>" + data.Rows[i]["ChineseName"] + "</td>");
                if (data.Rows[i]["OkNo"].ToString() == "正确")
                {
                    sb.Append(" <td style='color:#05d1c5'>" + data.Rows[i]["OperationAnswer"] + "</td>");
                }
                else
                {
                    sb.Append(" <td style='color:red'>" + data.Rows[i]["OperationAnswer"] + "</td>");
                }
                sb.Append("<td style='color:#05d1c5'>" + data.Rows[i]["RightAnswer"] + "</td>");
                if (data.Rows[i]["OkNo"].ToString() == "正确")
                {
                    sb.Append("<td style='color:#05d1c5'>" + data.Rows[i]["OkNo"] + "</td>");
                }
                else
                {
                    sb.Append("<td style='color:red'>" + data.Rows[i]["OkNo"] + "</td>");
                }
                sb.Append("</tr>");
                //if (data.Rows[i]["OkNo"].ToString() == "正确")
                //{
                //    sb.Append("<td>" + data.Rows[i]["Spare3"] + "</td></tr>");
                //}
                //else
                //{
                //    sb.Append("<td>0</td>");
                //}

            }
            return JsonConvert.SerializeObject(sb.ToString());
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
(select E_Name from tb_HB_Examination where EId=a.ER_EId) as E_Name,
UserName,UserNo,StudentNo,ER_Custom3,ER_Score,
(select MAX(ER_Score) from tb_ExaminationResult where ER_Type=a.ER_Type and ER_EId=a.ER_EId  and ER_PId=a.ER_PId  and ER_MId=a.ER_MId) as lsMinScore";
            var table = "tb_ExaminationResult a inner join tb_UserInfo b on a.ER_MId=b.UId";
            var where = " and ER_Type=4 and ER_State=0 and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and UserClassId=" + UserClassId;
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
                    sb.Append("恭喜您！您的<span class=\"text_red\">单据录入</span>排名为第<span class=\"text_red\">1</span>名，");
                    sb.Append("任务得分<span class=\"text_red\">" + dt.Rows[myidx]["ER_Score"] + "</span>分，");
                    sb.Append("距离上一名还差<span class=\"text_red\">0</span>分！");
                }
                else
                {
                    //上一名得分-当前得分
                    var SYMScore = Convert.ToDecimal(dt.Rows[myidx - 1]["ER_Score"]) - Convert.ToDecimal(dt.Rows[myidx]["ER_Score"]);
                    if (SYMScore != 0)//得分与上一名不相同
                    {
                        sb.Append("您的<span class=\"text_red\">单据录入</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                            sb.Append("您的<span class=\"text_red\">单据录入</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                    sb.Append("您的<span class=\"text_red\">单据录入</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                        sb.Append("您的<span class=\"text_red\">单据录入</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
                                sb.Append("您的<span class=\"text_red\">单据录入</span>排名为第<span class=\"text_red\">" + (myidx + 1) + "</span>名，");
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
        /// 考试做题页面的退出，清空成绩明细和操作数据
        /// </summary>
        /// <returns></returns>
        public string SignOut()
        {
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            int Rows = commonbll.DeleteInfo("tb_Operation_Answer", " and EId=" + ExamId + " and PId=" + PaperId + " and MId=" + UId + " and [Type]=4 and [State]=0");
            Rows += commonbll.DeleteInfo("tb_ExaminationDetails", " and ED_EId=" + ExamId + " and ED_PId=" + PaperId + " and ED_MId=" + UId + " and ED_Type=4 and ED_State=0");
            return JsonConvert.SerializeObject(Rows);
        }

        /// <summary>
        /// 考试倒计时结束后更新进入做题的时间
        /// </summary>
        /// <returns></returns>
        public string UpdaTime()
        {
            string ExamId = Request["ExamId"];
            string PaperId = Request["PaperId"];
            SqlParameter[] para = new SqlParameter[] { };
            commonbll.UpdateInfo("tb_CountDown", "CD_Time=getdate()", " and CD_Custom3=4 and CD_EId=" + ExamId + " and CD_PId=" + PaperId + " and CD_MId=" + UId + "", para);
            return "1";
        }

        /// <summary>
        /// 本次做题总成绩分数
        /// </summary>
        /// <returns></returns>
        public string GetExaminationResultBys()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            DataTable dt = commonbll.GetListDatatable("ER_Score", @"tb_ExaminationResult", "and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=4 and ER_State=0 ");
            return JsonConvert.SerializeObject(dt);
        }
    }
}
