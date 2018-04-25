using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocationalProject_Bll;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using VocationalProject_Models;
using System.IO;
using VocationalProject_DBUtility;
using System.Data.SqlClient;
using VocationalProject.Areas.Admin.Models;
using VocationalProject_Dal;
using VocationalProject.Controllers;
using VocationalProject.Models;
using System.Web.SessionState;
using System.Collections;
using VocationalProject_DBUtility.Sql;

namespace VocationalProject.Controllers
{
    /***************************************************************
   FileName:货币知识 学生端 考试做题
   Copyright（c）2017-金融教育在线技术开发部
   Author:袁学
   Create Date:2017-4-12
   ******************************************************************/
    public class HB_kaoshiController : BaseController, IRequiresSessionState
    {
        //
        // GET: /HB_kaoshi/
        //string UId = "3";//用户ID
        //string UserType = "3";//1管理员2教师3学生4裁判
        //string UserNo = "s00001";//登录账号
        //string UserClassId = "1";//所属班级
        CommonBll commonbll = new CommonBll();

        public ActionResult Index()
        {
            var Eid = Request["Eid"];
            var Pid = Request["Pid"];
            HBKaoShiModel km = new HBKaoShiModel();
            if (Eid != null && Pid != null && Eid.Length > 0 && Pid.Length > 0)
            {
                DataTable dt = commonbll.GetListDatatable("*", "tb_HB_Examination", " and Eid=" + Eid);

                string list = @"E_Name,
(select SUM(EP_Score) from tb_HB_ExaminationPapers where EP_PId=E_PId) as Score,
(select UserName from tb_UserInfo where UId=" + UId + @") as UserName,
(select UserPic from tb_UserInfo where UId=" + UId + @") as UserPic,
(select Name from tb_School a inner join tb_Team b on b.[SchoolId]=a.Id  where b.Id=" + UserClassId + ") as SchoolName";//列
                DataTable Mdt = commonbll.GetListDatatable(list, "tb_HB_Examination", " and Eid=" + Request["Eid"]);
                if (Mdt.Rows.Count > 0)
                {
                    km.E_Name = Mdt.Rows[0]["E_Name"].ToString();//竞赛名称
                    km.Score = Mdt.Rows[0]["Score"].ToString();//分值
                    km.UserName = Mdt.Rows[0]["UserName"].ToString();//姓名
                    if (Mdt.Rows[0]["UserPic"] == null || Mdt.Rows[0]["UserPic"].ToString() == "")
                    {
                        km.UserPic = "/img/profile_s.jpg";//头像
                    }
                    else
                    {
                        km.UserPic = Mdt.Rows[0]["UserPic"].ToString();//头像
                    }

                    km.SchoolName = Mdt.Rows[0]["SchoolName"].ToString();//学校
                }

                if (dt.Rows.Count > 0)
                {
                    //比赛有效开始时间和结束时间
                    km.E_StartTime = Convert.ToDateTime(dt.Rows[0]["E_StartTime"]).ToString("yyyy-MM-dd HH:mm:01");
                    km.E_EndTime = Convert.ToDateTime(dt.Rows[0]["E_EndTime"]).ToString("yyyy-MM-dd HH:mm:01");
                    km.E_Whenlong = dt.Rows[0]["E_Whenlong"].ToString();//竞赛时长
                    km.E_Type = dt.Rows[0]["E_Type"].ToString();//竞赛类型
                    km.E_IsTimeBonus = dt.Rows[0]["E_IsTimeBonus"].ToString();//时间加分

                    //考试模式下是否已经做过了 直接返回到
                    if (dt.Rows[0]["E_Type"].ToString() == "1")//考试模式
                    {
                        var resultcount = commonbll.GetRecordCount("tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=1 and ER_State=0");
                        if (resultcount > 0)
                        {
                            km.Isallow = "1";
                        }
                        else
                        {
                            km.Isallow = "0";
                        }

                    }

                    DateTime E_StartTime = Convert.ToDateTime(km.E_StartTime);//有效开始时间
                    if (E_StartTime > DateTime.Now)//有效开始时间大于 说明是倒计时
                    {
                        TimeSpan ts = E_StartTime - DateTime.Now;
                        int fz = ts.Minutes;//分差
                        int mz = ts.Seconds;//秒差
                        DateTime d = DateTime.Now.AddMinutes(fz).AddSeconds(mz);//在当前时间上加上这个秒钟

                        //当前进入时间 算开始 开始时间
                        km.AddStartDateTime = d.ToString("yyyy-MM-dd HH:mm:01");
                        //考试结束时间
                        km.TestStartDateTime = d.AddMinutes(int.Parse(dt.Rows[0]["E_Whenlong"].ToString())).ToString("yyyy-MM-dd HH:mm:01");

                    }
                    else
                    {
                        //计算 第一次进入剩余倒计时 2017-04-12 15:05:00
                        //根据这个倒计时和当前时间比较得到处 还相差几分钟，得出差值

                        DataTable timeDt = commonbll.GetListDatatable("CD_Time", "tb_CountDown", " and CD_Custom3=1   and CD_EId=" + Eid + " and CD_PId='" + Pid + "' and CD_MId=" + UId);
                        var shoucitime = "2017-04-01";
                        if (timeDt.Rows.Count > 0)
                        {
                            shoucitime = timeDt.Rows[0]["CD_Time"].ToString();
                        }

                        DateTime shouciDtime = Convert.ToDateTime(shoucitime);//记载时间表

                        //不存在时差 计算
                        //那么开始时间就是
                        km.AddStartDateTime = shouciDtime.ToString("yyyy-MM-dd HH:mm:ss");
                        //时间记载表上加时长
                        km.TestStartDateTime = shouciDtime.AddMinutes(int.Parse(dt.Rows[0]["E_Whenlong"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");


                    }

                    km.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
                }
            }

            return View(km);
        }

        /// <summary>
        /// 查看考试分数 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult GetScoresIndex()
        {
            var Eid = Request["Eid"];//考试id
            var Pid = Request["Pid"];//试卷id
            var Type = Request["Type"];//竞赛类型 
            if (Session["Pid_M"] != null)
            {
                Pid = Session["Pid_M"].ToString();
            }
            if (Session["Eid_M"] != null)
            {
                Eid = Session["Eid_M"].ToString();
            }
            HBKaoShiModel km = new HBKaoShiModel();

            DataTable dt = commonbll.GetListDatatable("TeamName,a.*", "tb_UserInfo a inner join tb_Team b on a.UserClassId=b.Id", " and UId=" + UId + " and UserType=3");
            if (dt.Rows.Count > 0)
            {
                km.TeamName = dt.Rows[0]["TeamName"].ToString();//代表队
                km.UserName = dt.Rows[0]["UserName"].ToString();//姓名

                if (dt.Rows[0]["UserPic"] == null || dt.Rows[0]["UserPic"].ToString() == "")
                {
                    km.UserPic = "/img/profile_s.jpg";//头像
                }
                else
                {
                    km.UserPic = dt.Rows[0]["UserPic"].ToString();//头像
                }

                km.UserNo = dt.Rows[0]["UserNo"].ToString();//登录账号

                DataTable Mdt = commonbll.GetListDatatable("ER_Score", @"tb_ExaminationResult", "and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=" + Type + " and ER_State=0 ");
                if (Mdt.Rows.Count > 0)
                {
                    km.ER_Score = Mdt.Rows[0]["ER_Score"].ToString();//总成绩
                }

                //货币竞赛名称
                if (Type == "1")
                {

                    DataTable Edt = commonbll.GetListDatatable("E_Name", "tb_HB_Examination", " and Eid=" + Eid);
                    if (Edt.Rows.Count > 0)
                    {
                        km.E_Name = Edt.Rows[0]["E_Name"].ToString();
                    }
                }
                if (Type == "3")//复核
                {
                    DataTable Edt = commonbll.GetListDatatable("ExaminationName", "tb_FH_Examination", " and Id=" + Eid);
                    if (Edt.Rows.Count > 0)
                    {
                        km.E_Name = Edt.Rows[0]["ExaminationName"].ToString();
                    }

                }
                if (Type == "4")//单据
                {
                    DataTable Edt = commonbll.GetListDatatable("ExamName", "tb_Bill_Exam", " and ID=" + Eid);
                    if (Edt.Rows.Count > 0)
                    {
                        km.E_Name = Edt.Rows[0]["ExamName"].ToString();
                    }

                }

            }
            return View(km);
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
                DataTable dt = commonbll.GetListDatatable("*", "tb_HB_Examination", " and Eid=" + Eid);
                if (dt.Rows.Count > 0)
                {
                    km.AddStartDateTime = Convert.ToDateTime(dt.Rows[0]["E_StartTime"]).ToString("yyyy-MM-dd HH:mm:01");
                }
                km.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            }
            return View(km);
        }

        /// <summary>
        /// 修改当前剩余表时间
        /// </summary>
        /// <returns></returns>
        public string UpDateTime()
        {
            var Eid = Request["Eid"];
            var Pid = Request["Pid"];
            var Type = Request["Type"];
            var resulutcount = commonbll.GetRecordCount("tb_CountDown", "  and CD_EId=" + Eid + " and CD_PId='" + Pid + "' and CD_MId=" + UId + " and CD_Custom1=1 and CD_Custom3=" + Type);
            if (resulutcount == 0)
            {

                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@CD_Time",DateTime.Now),
                    new SqlParameter("@CD_EId",Eid),
                    new SqlParameter("@CD_PId",Pid),
                    new SqlParameter("@CD_MId",UId),
                    new SqlParameter("@CD_Custom1","1"),
                    new SqlParameter("@CD_Custom3",Type)
                };
                commonbll.UpdateInfo("tb_CountDown", " CD_Time=@CD_Time,CD_Custom1=@CD_Custom1", " and CD_EId=@CD_EId and CD_PId=@CD_PId and CD_MId=@CD_MId and CD_Custom3=@CD_Custom3", pars);
                return "1";

            }
            return "1";
        }

        /// <summary>
        /// 获取题号数据
        /// </summary>
        /// <returns></returns>
        public string KaoShiByPId_ZQList()
        {
            string QB_Type = Request["QB_Type"];//试题类型
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            string isto = Request["isto"];//第几次读取
            //首先判断这个试卷 是否为打乱顺序的
            var P_IsOrder = commonbll.GetListSclar("P_IsOrder", "tb_HB_Paper", " and PId='" + Pid + "'");
            if (isto != null && isto == "1")
            {
                if (P_IsOrder == "0") //不打乱顺序
                {
                    DataTable dt = commonbll.GetListDatatable("QuestionBId,QB_Type,(select count(1) from tb_HB_ExaminationSB where SBEId=" + Eid + " and SBPId=" + Pid + " and SBQId=q.QuestionBId and SBMId=" + UId + ") as EP_Custom3",
                 @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on e.EP_QBId=q.QuestionBId", " and EP_PId='" + Pid + "' and QB_Type=" + QB_Type + "  order by QB_Type");
                    return JsonConvert.SerializeObject(dt);
                }
                else
                {
                    //随机打乱顺序
                    DataTable dt = commonbll.GetListDatatable("QuestionBId,QB_Type,(select count(1) from tb_HB_ExaminationSB where SBEId=" + Eid + " and SBPId=" + Pid + " and SBQId=q.QuestionBId and SBMId=" + UId + ") as EP_Custom3",
                 @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on e.EP_QBId=q.QuestionBId", " and EP_PId='" + Pid + "' and QB_Type=" + QB_Type + "  order by QB_Type");
                    if (dt.Rows.Count > 0)
                    {
                        string[] arr = new string[dt.Rows.Count];
                        if (Session["yxlxxx" + Eid + "_" + QB_Type] == null)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                arr[i] += dt.Rows[i]["QuestionBId"].ToString();
                            }

                            Hashtable hashtable = new Hashtable();
                            Hashtable hashtable1 = new Hashtable();
                            Random rm = new Random();

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                var num = rm.Next(0, dt.Rows.Count);
                                var num1 = rm.Next(0, dt.Rows.Count);
                                if (!hashtable.ContainsValue(num) && num != 0 && !hashtable1.ContainsValue(num1) && num1 != 0)
                                {
                                    hashtable.Add(num, num);
                                    hashtable1.Add(num1, num1);
                                    var temp = "";
                                    temp = arr[num];
                                    arr[num] = arr[num1];
                                    arr[num1] = temp;
                                }
                            }

                            DataTable dtM = new DataTable("mydatatable");
                            dtM.Columns.Add("QuestionBId", typeof(System.String));//
                            dtM.Columns.Add("QB_Type", typeof(System.String));
                            dtM.Columns.Add("EP_Custom3", typeof(System.String));
                            DataRow row = dtM.NewRow();

                            for (int i = 0; i < arr.Length; i++)
                            {
                                if (arr[i] != "" && arr[i].Length > 0)
                                {
                                    row = dtM.NewRow();
                                    row["QuestionBId"] = arr[i];
                                    row["QB_Type"] = QB_Type;
                                    //可根据题目ID查询和试卷id再查询 是否标记了

                                    var EP_Custom3 = commonbll.GetRecordCount("tb_HB_ExaminationSB", " and SBEId=" + Eid + " and SBPId=" + Pid + " and SBQId=" + arr[i] + " and SBMId=" + UId);
                                    row["EP_Custom3"] = EP_Custom3;
                                    dtM.Rows.Add(row);
                                }
                            }

                            Session["yxlxxx" + Eid + "_" + QB_Type] = dtM;
                            return JsonConvert.SerializeObject(dtM);
                        }
                        else
                        {
                            //sesstion 存在直接取sesstion
                            DataTable dtM = new DataTable();
                            if (Session["yxlxxx" + Eid + "_" + QB_Type] != null)
                            {
                                //dtM = Session["yxlxxx" + Eid + "_" + QB_Type] as DataTable;
                                dtM = Session["yxlxxx" + Eid + "_" + QB_Type] as DataTable;
                                for (int i = 0; i < dtM.Rows.Count; i++)
                                {

                                    //可根据题目ID查询和试卷id再查询 是否标记了
                                    var EP_Custom3 = commonbll.GetRecordCount("tb_HB_ExaminationSB", " and SBEId=" + Eid + " and SBPId=" + Pid + " and SBQId=" + dtM.Rows[i]["QuestionBId"] + " and SBMId=" + UId);

                                    dtM.Rows[i]["EP_Custom3"] = EP_Custom3;

                                }
                            }

                            return JsonConvert.SerializeObject(dtM);

                        }

                    }

                }

            }
            else
            {

                DataTable dt = commonbll.GetListDatatable("QuestionBId,QB_Type,(select count(1) from tb_HB_ExaminationSB where SBEId=" + Eid + " and SBPId=" + Pid + " and SBQId=q.QuestionBId and SBMId=" + UId + ") as EP_Custom3",
             @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on e.EP_QBId=q.QuestionBId", " and EP_PId='" + Pid + "' and QB_Type=" + QB_Type + "  order by QB_Type");
                return JsonConvert.SerializeObject(dt);
            }
            return JsonConvert.SerializeObject(new DataTable());
        }


        /// <summary>
        /// 获取单题信息
        /// </summary>
        /// <returns></returns>
        public string GetQuestionBankByIdAndPid()
        {
            string QuestionBId = Request["QuestionBId"];
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            DataTable dt = commonbll.GetListDatatable("q.*,EP_Score,(select count(1) from tb_HB_ExaminationSB where SBEId=" + Eid + " and SBPId=" + Pid + " and SBQId=" + QuestionBId + " and SBMId=" + UId + ") as EP_Custom3", @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on
 e.EP_QBId=q.QuestionBId", " and QuestionBId=" + QuestionBId + " and EP_PId='" + Pid + "'");
            return JsonConvert.SerializeObject(dt);
        }

        public string setbj()
        {
            string QuestionBId = Request["QuestionBId"];
            string Pid = Request["Pid"];
            string num = Request["num"];
            string Eid = Request["Eid"];
            //其他的 都是删除
            SqlHelper.ExecuteNonQuery("delete from tb_HB_ExaminationSB where SBEId=" + Eid + " and SBPId=" + Pid + " and SBQId=" + QuestionBId + " and SBMId=" + UId);
            if (num == "1")
            {//标记就是新增
                SqlHelper.ExecuteNonQuery("insert into tb_HB_ExaminationSB(SBEId, SBPId, SBQId, SBMId, SBAddTime) values('" + Eid + "','" + Pid + "','" + QuestionBId + "','" + UId + "','" + DateTime.Now + "') ");
            }
            // SqlHelper.ExecuteNonQuery("update tb_HB_ExaminationPapers set EP_Custom3=" + num + " where EP_PId=" + Pid + " and EP_QBId=" + QuestionBId);

            return "1";
        }

        /// <summary>
        /// 获取当前题型 题量 总分值
        /// </summary>
        /// <returns></returns>
        public string GetTypeNum()
        {
            string Pid = Request["Pid"];
            DataTable dt = commonbll.GetListDatatable(" QB_Type,count(QuestionBId) as Tnum,SUM(EP_Score) as Tscoers", @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on
 e.EP_QBId=q.QuestionBId", " and EP_PId='" + Pid + "' group by QB_Type");
            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 获取做过的题
        /// </summary>
        /// <returns></returns>
        public string GetGotiColor()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            string QB_Type = Request["QB_Type"];
            DataTable dt = commonbll.GetListDatatable("ED_QBId", @"tb_ExaminationDetails e inner join tb_HB_QuestionBank q on
 e.ED_QBId=q.QuestionBId", " and QB_Type=" + QB_Type + " and  ED_EId='" + Eid + "' and  ED_PId='" + Pid + "' and ED_MId='" + UId + "' and ED_Type=1 and ED_State=0 and len(ED_Content)>0 and ED_Content!='' ");
            return JsonConvert.SerializeObject(dt);
        }


        /// <summary>
        /// 单题做过的信息
        /// </summary>
        /// <returns></returns>
        public string GetExaminationDetailsbys()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            string QBId = Request["QBId"];
            DataTable dt = commonbll.GetListDatatable("QB_Type,ED_Content", @"tb_ExaminationDetails e inner join tb_HB_QuestionBank q on
 e.ED_QBId=q.QuestionBId", " and ED_EId='" + Eid + "' and  ED_PId='" + Pid + "' and ED_MId='" + UId + "' and ED_QBId=" + QBId + " and ED_Type=1 and ED_State=0");
            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 做题 单题保存
        /// </summary>
        /// <returns></returns>
        public string AddExaminationDetails()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];

            string dctijiao = Request["dctijiao"];//竞赛模式 1 考试模式

            //校验是否提交过 仅校验考试模式下 不允许多次提交
            if (dctijiao == "1")
            {
                var checkcount = commonbll.GetRecordCount("tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=1 and ER_State=0");
                if (checkcount > 0)
                {
                    return "88";
                }
            }
            //
            string QBId = Request["QBId"];
            string ED_OkNo = Request["ED_OkNo"];
            string ED_Content = Request["ED_Content"];
            string ED_Goal = Request["ED_Goal"];
            //单题保存 新增
            //判断考试中的试卷 是否已经提交 存在 则修改
            var checkcount2 = commonbll.GetRecordCount("tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + " and ED_QBId='" + QBId + "' and ED_Type=1 and ED_State=0 ");
            if (checkcount2 > 0)
            {
                //修改
                string sql = @"update tb_ExaminationDetails set ED_Content='" + ED_Content + "',ED_OkNo='" + ED_OkNo + "',ED_Goal='"
                    + ED_Goal + "' where ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + " and ED_QBId='" + QBId + "' and ED_Type=1 and ED_State=0";
                return SqlHelper.ExecuteNonQuery(sql).ToString();
            }
            else
            {
                //新增
                string sql = @"insert into  tb_ExaminationDetails(ED_EId, ED_PId, ED_QBId, ED_MId,ED_Type, ED_Content, 
ED_OkNo, ED_Goal, ED_State,ED_Operator, ED_AddTime, ED_Custom2) values(@ED_EId, @ED_PId, @ED_QBId, @ED_MId,@ED_Type, @ED_Content, 
@ED_OkNo, @ED_Goal, @ED_State,@ED_Operator, @ED_AddTime, @ED_Custom2)";
                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@ED_EId",Eid),
                    new SqlParameter("@ED_PId",Pid),
                    new SqlParameter("@ED_QBId",QBId),
                    new SqlParameter("@ED_MId",UId),

                     new SqlParameter("@ED_Type","1"),
                     new SqlParameter("@ED_Content",ED_Content),
                     new SqlParameter("@ED_OkNo",ED_OkNo),
                     new SqlParameter("@ED_Goal",ED_Goal),

                     new SqlParameter("@ED_State","0"),
                     new SqlParameter("@ED_Operator",UId),
                     new SqlParameter("@ED_AddTime",DateTime.Now),
                     new SqlParameter("@ED_Custom2",UserNo)

                };
                return SqlHelper.ExecuteNonQuery(sql, CommandType.Text, pars).ToString();
            }

        }

        public int CheckPapers() 
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];

            string dctijiao = Request["dctijiao"];//竞赛模式 1 考试模式
            string E_IsTimeBonus = Request["E_IsTimeBonus"];//是否时间加分
            string P_Score = Request["P_Score"];//考试总分
            string E_Whenlong = Request["E_Whenlong"];//做题时长

            Session["Pid_M"] = Pid;
            Session["Eid_M"] = Eid;
            int allsqldtcount = commonbll.GetRecordCount(@"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on
 e.EP_QBId=q.QuestionBId", " and EP_PId='" + Pid + "' ");
            int allsqlcount = commonbll.GetRecordCount(@"tb_ExaminationDetails", " and ED_PId='" + Pid + "'and ED_MId=" + UId + " and ED_EId=" + Eid + " and ED_State=0");
            if (allsqldtcount > allsqlcount)
            {
                return 1;
            }
            else 
            {
                return 0;
            }
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

            //清除标记
            SqlHelper.ExecuteNonQuery("delete from tb_HB_ExaminationSB where SBEId=" + Eid + " and SBPId=" + Pid + "  and SBMId=" + UId);

            //校验是否提交过 仅校验考试模式下 不允许多次提交
            if (dctijiao == "1")
            {
                var checkcount = commonbll.GetRecordCount("tb_ExaminationResult", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=1 and ER_State=0");
                if (checkcount > 0)
                {
                    return "88";
                }
            }
           

            var resulutcount = commonbll.GetRecordCount("tb_CountDown", " and CD_EId=" + Eid + " and CD_PId='" + Pid + "' and CD_MId=" + UId + " and CD_Custom1=1 and CD_Custom3=1");
            if (resulutcount == 0)
            {
                //如果在提交后在重复刷新 
                return "1";
            }

            try
            {


                //查询出套试卷所有的试题Id
                DataTable allsqldt = commonbll.GetListDatatable("QuestionBId", @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on
 e.EP_QBId=q.QuestionBId", " and EP_PId='" + Pid + "' order by QB_Type");

                if (allsqldt.Rows.Count > 0)
                {
                    string txtsql = "";
                    for (var i = 0; i < allsqldt.Rows.Count; i++)
                    {
                        //查询是否做过
                        var resultcount = commonbll.GetRecordCount("tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + " and ED_QBId='" + allsqldt.Rows[i]["QuestionBId"] + "' and ED_Type=1 and ED_State=0 ");
                        if (resultcount == 0)
                        {
                            //没做过新增明细记录
                            txtsql += @"insert into  tb_ExaminationDetails(ED_EId, ED_PId, ED_QBId, ED_MId,ED_Type, ED_Content, 
ED_OkNo, ED_Goal, ED_State,ED_Operator, ED_AddTime, ED_Custom2)
values('" + Eid + "', '" + Pid + "','" + allsqldt.Rows[i]["QuestionBId"] + "','" + UId + "','1', '','错误','0', '0','" + UId + "','" + DateTime.Now + "','" + UserNo + "'); ";

                        }

                    }
                    //批量新增
                    if (txtsql.Length > 0)
                    {
                        SqlHelper.ExecuteNonQuery(txtsql);
                    }
                }

                //走总提交模式
                //1.先删除明细表 ED_State 状态=1 的明细
                //2.把所有为0的明细状态 修改为1
                //3.把所有明细分值累加 到总分表

                //4.把之前总分提交过的数据状态修改1
                //5.最新的总分数据 状态为0
                //6.删除时间记载表

                //1.
                commonbll.DeleteInfo("tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + "  and ED_Type=1 and ED_State=1  ");
                //2.
                SqlParameter[] parsto = new SqlParameter[] { new SqlParameter("@ED_State", "1") };
                commonbll.UpdateInfo("tb_ExaminationDetails", "ED_State=@ED_State", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + "  and ED_Type=1 and ED_State=0", parsto);
                //3.
                var SumSocres = commonbll.GetListSclar("SUM(ED_Goal)", "tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + "  and ED_Type=1 and ED_State=1  ");
                //4.
                SqlParameter[] parsex = new SqlParameter[] { new SqlParameter("@ER_State", "1") };
                commonbll.UpdateInfo("tb_ExaminationResult", "ER_State=@ER_State", " and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=1 and ER_State=0 ", parsex);

                //计算时间 做题时长
                var whenlong = "00:00:00";
                var Towhenlong = 0.00;//存储做题总时间总秒
                DataTable timeDt = commonbll.GetListDatatable("CD_Time", "tb_CountDown", "  and CD_Custom3=1 and CD_EId=" + Eid + " and CD_PId='" + Pid + "' and CD_MId=" + UId);
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
                //5.总分表新增
                string sqlreuslt = @"insert into tb_ExaminationResult(ER_EId, ER_PId, ER_MId, ER_Type, ER_Score, ER_State, ER_Operator, ER_AddTime, ER_Custom2,ER_Custom3) 
                values(@ER_EId, @ER_PId, @ER_MId, @ER_Type, @ER_Score, @ER_State, @ER_Operator, @ER_AddTime, @ER_Custom2,@ER_Custom3)";
                SqlParameter[] parsreult = new SqlParameter[] 
                {
                    new SqlParameter("@ER_EId",Eid),
                    new SqlParameter("@ER_PId",Pid),
                    new SqlParameter("@ER_MId",UId),
                    new SqlParameter("@ER_Type","1"),
                    
                    new SqlParameter("@ER_Score", Math.Round(Convert.ToDecimal(SumSocres)+longsores,2)),
                    new SqlParameter("@ER_State","0"),
                    new SqlParameter("@ER_Operator",UId),
                    new SqlParameter("@ER_AddTime",DateTime.Now),
                    new SqlParameter("@ER_Custom2",UserNo),
                    new SqlParameter("@ER_Custom3",whenlong)
                };
                SqlHelper.ExecuteNonQuery(sqlreuslt, CommandType.Text, parsreult);
                //6.
                commonbll.DeleteInfo("tb_CountDown", " and CD_Custom3=1 and CD_EId='" + Eid + "' and CD_PId='" + Pid + "' and CD_MId='" + UId + "'");
                return "1";
            }
            catch
            {
                return "99";
            }


        }


        //退出清掉做题成绩
        public string DelExamResult()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            //清掉明细 货币知识 状态为0 最新 做题过程中的
            commonbll.DeleteInfo("tb_ExaminationDetails", " and ED_EId=" + Eid + " and ED_PId='" + Pid + "' and ED_MId=" + UId + "  and ED_Type=1 and ED_State=0  ");
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
            DataTable dt = commonbll.GetListDatatable("ER_Score", @"tb_ExaminationResult", "and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + UId + " and ER_Type=1 and ER_State=0 ");
            return JsonConvert.SerializeObject(dt);
        }
    }
}
