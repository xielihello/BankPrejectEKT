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
  FileName:货币知识 学生端 考试明细查看
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-4-12
  ******************************************************************/
    public class HB_ExamPreviewController : BaseController
    {
        //
        // GET: /ExamPreview/

        //string UId = "3";//用户ID

        //string UserType = "3";//1管理员2教师3学生4裁判
        //string UserNo = "s00001";//登录账号
        //string UserClassId = "1";//所属班级

        CommonBll commonbll = new CommonBll();
        ///HB_ExamPreview?Eid=1&Pid=10000&Type=2&Mid=3
        ///HB_ExamPreview?Eid=1&Pid=10000&Type=1
        public ActionResult Index()
        {
            //

            var Type = Request["Type"];//学生自己查看  //2.是管理端教师端查
            string SUId = "0";
            if (Type == "1")
            {
                SUId = UId;

            }
            if (Type == "2")
            {//教师端别的
                SUId = Request["MId"];
            }

            var Eid = Request["Eid"];
            var Pid = Request["Pid"];
            HBKaoShiModel km = new HBKaoShiModel();

            if (Eid != null && Pid != null && Eid.Length > 0 && Pid.Length > 0)
            {
                string list = @"E_Name,
(select SUM(EP_Score) from tb_HB_ExaminationPapers where EP_PId=E_PId) as Score,
(select UserName from tb_UserInfo where UId=" + SUId + @") as UserName,
(select UserPic from tb_UserInfo where UId=" + SUId + @") as UserPic,
(select Name from tb_School a inner join tb_Team b on b.[SchoolId]=a.Id  where b.Id in(select UserClassId from tb_UserInfo where UId=" + SUId + ")) as SchoolName";//列
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

                DataTable dt = commonbll.GetListDatatable("ER_Score", @"tb_ExaminationResult", "and ER_EId=" + Eid + " and ER_PId='" + Pid + "' and ER_MId=" + SUId + " and ER_Type=1 and ER_State=0 ");
                if (dt.Rows.Count > 0)
                {
                    km.ER_Score = dt.Rows[0]["ER_Score"].ToString();//总成绩
                }
            }

            return View(km);
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
        /// 成绩统计
        /// </summary>
        /// <returns></returns>
        public string GetResult()
        {
            string Pid = Request["Pid"];
            string Type = Request["Type"];
            string Mid = Request["Mid"];
            string Eid = Request["Eid"];
            string SUId = "0";
            if (Type == "1")
            {
                SUId = UId;

            }
            if (Type == "2")
            {//教师端别的
                SUId = Mid;
            }

            string list = @"QB_Type,SUM(EP_Score) as Tscoers,count(QuestionBId) as Tnum,( select SUM(ED_Goal)  from tb_ExaminationDetails a inner join tb_HB_QuestionBank b on a.ED_QBId=b.QuestionBId
  where ED_PId='" + Pid + "'  and QB_Type=q.QB_Type and ED_State=1 and ED_MId=" + SUId + " and ED_EId="+Eid+") as Goal";
            DataTable dt = commonbll.GetListDatatable(list, @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on
 e.EP_QBId=q.QuestionBId", " and EP_PId='" + Pid + "' group by QB_Type");
            return JsonConvert.SerializeObject(dt);
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
                    DataTable dt = commonbll.GetListDatatable("QuestionBId,QB_Type",
                 @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on e.EP_QBId=q.QuestionBId", " and EP_PId='" + Pid + "' and QB_Type=" + QB_Type + "  order by QB_Type");
                    return JsonConvert.SerializeObject(dt);
                }
                else
                {
                    //随机打乱顺序
                    DataTable dt = commonbll.GetListDatatable("QuestionBId,QB_Type",
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
                            DataRow row = dtM.NewRow();

                            for (int i = 0; i < arr.Length; i++)
                            {
                                if (arr[i] != "" && arr[i].Length > 0)
                                {
                                    row = dtM.NewRow();
                                    row["QuestionBId"] = arr[i];
                                    row["QB_Type"] = QB_Type;
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
                                dtM = Session["yxlxxx" + Eid + "_" + QB_Type] as DataTable;
                            }
                            return JsonConvert.SerializeObject(dtM);

                        }

                    }

                }

            }
            else
            {

                DataTable dt = commonbll.GetListDatatable("QuestionBId,QB_Type",
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
            DataTable dt = commonbll.GetListDatatable("q.*,EP_Score", @"tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on
 e.EP_QBId=q.QuestionBId", " and QuestionBId=" + QuestionBId + " and EP_PId='" + Pid + "'");
            return JsonConvert.SerializeObject(dt);
        }


        /// <summary>
        /// 单题做题情况
        /// </summary>
        /// <returns></returns>
        public string GetExaminationDetailsbysTo()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            string QBId = Request["QBId"];
            string Type = Request["Type"];
            string Mid = Request["Mid"];

            string SUId = "0";
            if (Type == "1")
            {
                SUId = UId;

            }
            if (Type == "2")
            {//教师端别的
                SUId = Mid;
            }


            DataTable dt = commonbll.GetListDatatable("QB_Type,ED_Content,ED_OkNo,QB_Answer,QB_Keyword", @"tb_ExaminationDetails e inner join tb_HB_QuestionBank q on
 e.ED_QBId=q.QuestionBId", " and ED_EId='" + Eid + "' and  ED_PId='" + Pid + "' and ED_MId='" + SUId + "' and ED_QBId=" + QBId + " and ED_Type=1 and ED_State=1");
            if (dt.Rows.Count == 0)
            {
                dt = commonbll.GetListDatatable("QB_Type,'' as ED_Content,'' as ED_OkNo,QB_Answer,QB_Keyword", @"tb_HB_QuestionBank", " and QuestionBId=" + QBId);
            }
            return JsonConvert.SerializeObject(dt);

        }


        /// <summary>
        /// 单题做题情况
        /// </summary>
        /// <returns></returns>
        public string GetExaminationDetailsbys()
        {
            string Pid = Request["Pid"];
            string Eid = Request["Eid"];
            string QBId = Request["QBId"];
            string Type = Request["Type"];
            string Mid = Request["Mid"];
            string QB_Type = Request["QB_Type"];

            string SUId = "0";
            if (Type == "1")
            {
                SUId = UId;

            }
            if (Type == "2")
            {//教师端别的
                SUId = Mid;
            }

            DataTable dt = commonbll.GetListDatatable("QB_Type,ED_Content,ED_OkNo,QB_Answer,QB_Keyword,QuestionBId", @"tb_ExaminationDetails e inner join tb_HB_QuestionBank q on
 e.ED_QBId=q.QuestionBId", " and ED_EId='" + Eid + "' and  ED_PId='" + Pid + "' and ED_MId='" + SUId + "' and QB_Type=" + QB_Type + " and ED_Type=1 and ED_State=1");

            return JsonConvert.SerializeObject(dt);

        }

    }
}
