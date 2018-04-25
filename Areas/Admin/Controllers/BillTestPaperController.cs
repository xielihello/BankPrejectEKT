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
using VocationalProject.Controllers;
using VocationalProject_Bll;
using VocationalProject_Dal;
using VocationalProject_DBUtility.Common;
using VocationalProject_Models;

namespace VocationalProject.Areas.Admin.Controllers
{
    public class BillTestPaperController : BaseController
    {
        //
        // GET: /Admin/BillTestPaper/
        CommonBll commBll = new CommonBll();
        Bill_TestPaperBll perBll = new Bill_TestPaperBll();
        Bill_TestPaperEvenBll evenBll = new Bill_TestPaperEvenBll();
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        //string TeacherSchoolId = "1";//教师属于的院校

        /// <summary>
        /// 试卷管理
        /// </summary>
        public ActionResult TestPaperList()
        {
            ViewData["UserType"] = UserType;
            ViewData["UserId"] = UId;
            return View();
        }

        /// <summary>
        /// 试卷列表以及查询
        /// </summary>
        /// <returns></returns>
        public string GetTestPaperList()
        {
            DataTable data = new DataTable();
            string strWhere = " and a.Bill_Spare1=1 ";

            string TestAttribute = Request.Form["TestAttribute"];
            string PaperSource = Request.Form["PaperSource"];
            string PaperName = Request.Form["PaperName"];
            if (UserType == "2")
            {
                strWhere += "and ((a.Operator=" + UId + "or a.Id in(select UR_CompetitionId from tb_PaperUserRights where UR_CompetitionType=4 and UR_SchoolId=" + TeacherSchoolId + ")) or a.AddOperator=" + UId + ")";
            }
            if (TestAttribute == "3")
            {
                strWhere += " and (a.Kind=1 or a.Kind=2)";
            }
            if (TestAttribute == "0" || TestAttribute == "1")
            {
                strWhere += " and a.Kind=1";
            }
            if (TestAttribute == "2")
            {
                strWhere += " and a.Kind=" + TestAttribute + "";
            }
            //strWhere += " and a.Kind=" + TestAttribute + "";
            if (PaperSource != "")
            {
                strWhere += " and a.AddOperator in(select UId from tb_UserInfo where UserName like '" + PaperSource + "')";
            }
            if (PaperName != "")
            {
                strWhere += " and a.PaperName like '%" + PaperName.Trim() + "%'";
            }
            string table = "tb_Bill_TestPaper as a inner join tb_UserInfo as c on a.AddOperator=c.UId";
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.Id desc"; //排序必须填写
            m.strFld = "a.*,(select sum(Score) from tb_Bill_TestPaperEven where PaperID=a.ID and  Bill_Spare1=1) as Score,c.UserType,c.UserNo,c.UserName";
            m.tab = table;
            m.strWhere = strWhere;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));
        }

        /// <summary>
        /// 试卷-手工组卷
        /// </summary>
        /// <returns></returns>
        public ActionResult ManualTestPaper()
        {
            string actin = Request.Form["action"];
            //保存
            if (actin == "Save")
            {
                SqlParameter[] para = new SqlParameter[] { };
                string PaperId = Request["PaperId"];
                int Count = commBll.UpdateInfo("tb_Bill_TestPaperEven", "Bill_Spare1=1", " and PaperID=" + PaperId + "", para);
                Count += commBll.UpdateInfo("tb_Bill_TestPaper", "Bill_Spare1=1", " and ID=" + PaperId + "", para);
                ViewData["Count"] = Count;
            }
            DataTable data = commBll.GetListDatatable("distinct Bill_Spare2,BillType", "tb_Bills", "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value='0'>请选择单据类型</option>");
            foreach (DataRow dr in data.Rows)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", dr["BillType"], dr["Bill_Spare2"]);
            }
            ViewBag.BillType = sb.ToString();
            return View();
        }

        /// <summary>
        /// 试卷-手工组卷列表以及查询
        /// </summary>
        /// <returns></returns>
        public string GetManualTestPaper()
        {
            string strWhere = string.Empty;
            string BillType = Request.Form["BillType"];
            if (BillType != "0")
            {
                strWhere = "and a.BillType=" + BillType + "";
            }
            string BillName = Request.Form["BillName"];
            if (BillName != "0")
            {
                strWhere += " and b.BillName='" + BillName + "'";
            }
            string CoilState = Request.Form["CoilState"];
            if (CoilState != "0")
            {
                if (CoilState == "1")
                {
                    strWhere += "and (select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID and PaperID='" + Request["PaperId"] + "')>0";
                }
                else
                {
                    strWhere += "and (select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID and PaperID='" + Request["PaperId"] + "')<=0";
                }
            }
            string UsingState = Request.Form["UsingState"];
            if (UsingState != "0")
            {
                if (UsingState == "1")
                {
                    strWhere += "and (select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID )>0";
                }
                else
                {
                    strWhere += "and (select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID )<=0";
                }
            }
            string Title = Request.Form["Title"];
            if (Title != "")
            {
                strWhere += "and a.TopicTitle like '%" + Title + "%'";
            }
            if (UserType == "2")
            {
                strWhere += "and a.AddOperator=" + UId + "";
            }
            string line = @"a.*,b.Bill_Spare2 as Bill_Spare,b.BillName,(select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID and Bill_Spare1=1 ) as UsingState,
(select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID and PaperID='" + Request.Form["PaperID"] + "' ) as CoilState,(select Score from tb_Bill_TestPaperEven where TopicID=a.ID and PaperID='" + Request.Form["PaperID"] + "' ) as Score";
            string table = "tb_Bill_Topic as a left join tb_Bills as b on a.BillFormId=b.BillFormId and a.BillType=b.BillType";
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.Id desc"; //排序必须填写
            m.strFld = line;
            m.tab = table;
            m.strWhere = "and a.Bill_Spare1=1 and a.Kind=" + UserType + "" + strWhere;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));
        }

        /// <summary>
        /// 手工组卷-加入试卷
        /// </summary>
        /// <param name="TopicId">题目ID(多个)</param>
        /// <param name="PaperName">试卷名称</param>
        /// <param name="Sequence">是否打乱顺序</param>
        /// <param name="Score">每到题目的分数（多个）</param>
        /// <returns></returns>
        public string AddTestPaper(string TopicId, string PaperName, string Sequence, string Score)
        {
            string PaperId = Request["PaperId"];
            if (PaperId == "")
            {
                int Count = commBll.GetRecordCount("tb_Bill_TestPaper", "");
                tb_Bill_TestPaper paper = new tb_Bill_TestPaper();
                paper.PaperNumber = Count;
                paper.PaperName = PaperName.ToString();
                paper.Sequence = Convert.ToInt32(Sequence);
                paper.SumExtract = null;
                paper.ExtractUnused = null;
                paper.Kind = Convert.ToInt32(UserType);
                paper.Operator = UId;
                paper.AddOperator = UId;
                paper.OperatorTime = DateTime.Now;
                paper.Bill_Spare1 = 0;
                paper.Bill_Spare2 = null;
                paper.Bill_Spare3 = null;
                PaperId = perBll.AddTestPaperBll(paper);
            }
            // string PaperId = commBll.GetListSclar(" top 1 ID", "tb_Bill_TestPaper", " and AddOperator=" + UId + " order by OperatorTime desc");
            for (int i = 0; i < TopicId.ToString().Split(',').Length - 1; i++)
            {
                tb_Bill_TestPaperEven en = new tb_Bill_TestPaperEven();
                en.PaperID = Convert.ToInt32(PaperId);
                en.TopicID = Convert.ToInt32(TopicId.ToString().Split(',')[i]);
                en.Score = Convert.ToInt32(Score.ToString().Split(',')[i]);
                en.Operator = UId;
                en.AddOperator = UId;
                en.OperatorTime = DateTime.Now;
                en.Bill_Spare1 = 0;
                en.Bill_Spare2 = null;
                en.Bill_Spare3 = null;
                evenBll.AddTestPaperBll(en);
            }
            return JsonConvert.SerializeObject(PaperId);
        }

        /// <summary>
        /// 手工组卷-移除试卷
        /// </summary>
        /// <param name="TopicId">题目ID(多个)</param>
        /// <param name="PaperId">试卷ID</param>
        /// <returns></returns>
        public int DelTestPaper(string TopicId, string PaperId)
        {
            SqlParameter[] para = new SqlParameter[] { };
            int Count = 0;
            for (int i = 0; i < TopicId.Split(',').Length - 1; i++)
            {
                Count += commBll.UpdateInfo("tb_Bill_TestPaperEven", "Bill_Spare1=0", " and PaperID=" + PaperId + " and TopicID=" + TopicId.Split(',')[i] + "", para);
            }
            return Count;
        }

        /// <summary>
        /// 删除试卷
        /// </summary>
        /// <param name="PaperId">试卷ID(多个)</param>
        /// <returns></returns>
        public int DeleTestPaper()
        {
            string s = Request["TopicId"].ToString();
            SqlParameter[] para = new SqlParameter[] { };
            int Count = commBll.UpdateInfo("tb_Bill_TestPaperEven", "Bill_Spare1=-1", " and PaperID in(" + Request["TopicId"] + ")", para);
            Count += commBll.UpdateInfo("tb_Bill_TestPaper", "Bill_Spare1=-1", " and ID in(" + Request["TopicId"] + ")", para);
            return Count;
        }

        /// <summary>
        ///  试卷-智能组卷
        /// </summary>
        /// <returns></returns>
        public ActionResult CapacityTestPaper()
        {
            if (UserType == "2")
            {
                ViewData["Sum_Number"] = commBll.GetRecordCount("tb_Bill_Topic", " and Kind=" + UserType + " and Bill_Spare1=1 and AddOperator=" + UId + "");
                //select TOP (100) PERCENT --order by OperatorTime desc
                ViewData["Unused"] = commBll.GetRecordCount("tb_Bill_Topic", " and Kind=" + UserType + " and  ID not in(select TopicID from tb_Bill_TestPaperEven  ) and AddOperator=" + UId + "");
            }
            else
            {
                ViewData["Sum_Number"] = commBll.GetRecordCount("tb_Bill_Topic", " and Kind=" + UserType + " and Bill_Spare1=1");
                //select TOP (100) PERCENT --order by OperatorTime desc
                ViewData["Unused"] = commBll.GetRecordCount("tb_Bill_Topic", " and Kind=" + UserType + " and  ID not in(select TopicID from tb_Bill_TestPaperEven  )");
            }

            return View();
        }

        /// <summary>
        /// 智能组卷-保存
        /// </summary>
        /// <returns></returns>
        public string SaveTestPaper()
        {
            DataTable data = new DataTable();
            string strWhere = string.Empty;
            //试卷名称
            string PaperName = Request.Form["PaperName"];
            //共随机抽取题目数量
            string SumExtract = Request.Form["SumExtract"];
            //未被使用题目数量
            string ExtractUnused = Request.Form["ExtractUnused"] == "" ? "0" : Request.Form["ExtractUnused"];
            //每到题目分数
            string Score = Request.Form["Score"];
            //是否打乱同一题型下的试题顺序
            string Sequence = Request.Form["Sequence"];
            int Count = commBll.GetRecordCount("tb_Bill_TestPaper", "");
            tb_Bill_TestPaper paper = new tb_Bill_TestPaper();
            paper.PaperNumber = Count;
            paper.PaperName = PaperName.ToString();
            paper.Sequence = Convert.ToInt32(Sequence);
            paper.SumExtract = Convert.ToInt32(SumExtract);
            paper.ExtractUnused = Convert.ToInt32(ExtractUnused);
            paper.Kind = Convert.ToInt32(UserType);
            paper.Operator = UId;
            paper.AddOperator = UId;
            paper.OperatorTime = DateTime.Now;
            paper.Bill_Spare1 = 1;
            paper.Bill_Spare2 = null;
            paper.Bill_Spare3 = null;
            int RowCount = 0;
            string PaperId = perBll.AddTestPaperBll(paper);
            //string PaperId = commBll.GetListSclar(" top 1 ID", "tb_Bill_TestPaper", " and AddOperator=" + UId + " order by OperatorTime desc");
            //判断不为空时需要抽取没有被使用的题目
            if (ExtractUnused != "0")
            {

                //查询出没有使用过的题目
                strWhere = " and Kind=" + UserType + " and Bill_Spare1=1 and ID not in(select Top " + ExtractUnused + " TopicID from tb_Bill_TestPaperEven where Bill_Spare1=1 order by OperatorTime desc)";
                data = commBll.GetListDatatable("TOP " + ExtractUnused + " *", "tb_Bill_Topic", "" + strWhere + "ORDER BY NEWID()");
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    tb_Bill_TestPaperEven en = new tb_Bill_TestPaperEven();
                    en.PaperID = Convert.ToInt32(PaperId);
                    en.TopicID = Convert.ToInt32(data.Rows[i]["ID"]);
                    en.Score = Convert.ToInt32(Score);
                    en.Operator = UId;
                    en.AddOperator = UId;
                    en.OperatorTime = DateTime.Now;
                    en.Bill_Spare1 = 1;
                    en.Bill_Spare2 = null;
                    en.Bill_Spare3 = null;
                    RowCount += evenBll.AddTestPaperBll(en);
                }
                //查询出有使用过的题目
                strWhere = " and Kind=" + UserType + " and Bill_Spare1=1 and ID in(select Top " + (Convert.ToInt32(SumExtract) - Convert.ToInt32(ExtractUnused)) + " TopicID from tb_Bill_TestPaperEven where Bill_Spare1=1 order by OperatorTime desc)";
                data = commBll.GetListDatatable("TOP " + (Convert.ToInt32(SumExtract) - Convert.ToInt32(ExtractUnused)) + " *", "tb_Bill_Topic", "" + strWhere + "ORDER BY NEWID()");
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    tb_Bill_TestPaperEven en = new tb_Bill_TestPaperEven();
                    en.PaperID = Convert.ToInt32(PaperId);
                    en.TopicID = Convert.ToInt32(data.Rows[i]["ID"]);
                    en.Score = Convert.ToInt32(Score);
                    en.Operator = UId;
                    en.AddOperator = UId;
                    en.OperatorTime = DateTime.Now;
                    en.Bill_Spare1 = 1;
                    en.Bill_Spare2 = null;
                    en.Bill_Spare3 = null;
                    RowCount += evenBll.AddTestPaperBll(en);
                }
            }
            else
            {
                //查询出有使用过的题目
                //strWhere = " and Kind=" + UserType + " and Bill_Spare1=1 and ID in(select Top " + SumExtract + " TopicID from tb_Bill_TestPaperEven where Bill_Spare1=1 order by OperatorTime desc)";
                data = commBll.GetListDatatable("TOP " + SumExtract + " *", "tb_Bill_Topic", "" + strWhere + "ORDER BY NEWID()");
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    tb_Bill_TestPaperEven en = new tb_Bill_TestPaperEven();
                    en.PaperID = Convert.ToInt32(PaperId);
                    en.TopicID = Convert.ToInt32(data.Rows[i]["ID"]);
                    en.Score = Convert.ToInt32(Score);
                    en.Operator = UId;
                    en.AddOperator = UId;
                    en.OperatorTime = DateTime.Now;
                    en.Bill_Spare1 = 1;
                    en.Bill_Spare2 = null;
                    en.Bill_Spare3 = null;
                    RowCount += evenBll.AddTestPaperBll(en);
                }
            }
            return JsonConvert.SerializeObject(RowCount);
        }

        /// <summary>
        /// 编辑试卷
        /// </summary>
        /// <returns></returns>
        public ActionResult EditTestPaper()
        {
            int ID = Convert.ToInt32(Request["ID"]);
            ViewData["PaperId"] = ID;
            string action = Request["action"];
            //保存
            if (action == "Save")
            {
                string PaperName = Request.Form["PaperName"];
                string Sequence = Request.Form["rad"];
                SqlParameter[] para1 = new SqlParameter[]{
                new SqlParameter("@PaperName",PaperName),
                new SqlParameter("@Sequence",Sequence)};
                commBll.UpdateInfo("tb_Bill_TestPaper", "PaperName=@PaperName,Sequence=@Sequence", " and ID=" + ID + "", para1);

                SqlParameter[] para = new SqlParameter[] { };
                int Count = commBll.UpdateInfo("tb_Bill_TestPaperEven", "Bill_Spare1=1", " and PaperID=" + ID + "", para);
                Count += commBll.UpdateInfo("tb_Bill_TestPaper", "Bill_Spare1=1", " and ID=" + ID + "", para);
                ViewData["Count"] = Count;

                //移除试卷
                string GLId = Request["GLId"];
                if (GLId != "")
                {
                    for (int i = 0; i < GLId.Split(',').Length - 1; i++)
                    {
                        Count += commBll.UpdateInfo("tb_Bill_TestPaperEven", "Bill_Spare1=0,Score=NULL", " and PaperID=" + Request["PaperId"] + " and TopicID=" + GLId.Split(',')[i] + "", para);
                    }
                }
            }
            DataTable data = commBll.GetListDatatable("distinct Bill_Spare2,BillType", "tb_Bills", "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value='0'>请选择单据类型</option>");
            foreach (DataRow dr in data.Rows)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", dr["BillType"], dr["Bill_Spare2"]);
            }
            ViewBag.BillType = sb.ToString();
            data = commBll.GetListDatatable("*,(select sum(Score) from tb_Bill_TestPaperEven where PaperID=a.ID) as Score,(select Count(*) from tb_Bill_TestPaperEven where PaperID=a.ID) as Number", "tb_Bill_TestPaper as a", " and ID=" + ID + "");
            if (data.Rows.Count > 0)
            {
                ViewData["PaperName"] = data.Rows[0]["PaperName"].ToString();
                ViewData["Sequence"] = data.Rows[0]["Sequence"].ToString();
                ViewData["Score"] = data.Rows[0]["Score"].ToString();
                ViewData["Number"] = data.Rows[0]["Number"].ToString();
            }
            return View();
        }

        /// <summary>
        /// 编辑试卷列表以及查询
        /// </summary>
        /// <returns></returns>
        public string GetEditTestPaperlist()
        {
            string Id = Request["Id"];
            string strWhere = string.Empty;
            string BillType = Request.Form["BillType"];
            if (BillType != "0")
            {
                strWhere = "and a.BillType=" + BillType + "";
            }
            string BillName = Request.Form["BillName"];
            if (BillName != "0")
            {
                strWhere += " and b.BillName='" + BillName + "'";
            }
            string CoilState = Request.Form["CoilState"];
            if (CoilState != "0")
            {
                if (CoilState == "1")
                {
                    strWhere += "and (select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID and PaperID='" + Id + "')>0";
                }
                else
                {
                    strWhere += "and (select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID and PaperID='" + Id + "')<=0";
                }
            }
            string UsingState = Request.Form["UsingState"];
            if (UsingState != "0")
            {
                if (UsingState == "1")
                {
                    strWhere += "and (select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID )>0";
                }
                else
                {
                    strWhere += "and (select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID )<=0";
                }
            }
            string Title = Request.Form["Title"];
            if (Title != "")
            {
                strWhere += "and a.TopicTitle like '%" + Title + "%'";
            }
            string line = "a.*,e.Score,b.Bill_Spare2 as Bill_Spare,b.BillName,(select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID and Bill_Spare1=1) as UsingState,(select COUNT(1) from tb_Bill_TestPaperEven where TopicID=a.ID and PaperID='" + Id + "' and Bill_Spare1=1) as CoilState";
            string table = "tb_Bill_Topic as a left join tb_Bills as b on a.BillFormId=b.BillFormId and a.BillType=b.BillType left join tb_Bill_TestPaperEven as e  on a.ID=e.TopicID and e.PaperID=" + Id + "";
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.Id desc"; //排序必须填写
            m.strFld = line;
            m.tab = table;
            m.strWhere = "and a.Bill_Spare1=1 and a.Kind=" + UserType + "" + strWhere;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));
        }

        /// <summary>
        /// 编辑试卷-加入试卷
        /// </summary>
        /// <returns></returns>
        public int AddTestPaper_Edit()
        {
            int RowCount = 0;
            string TopicId = Request.Form["TopicId"];
            string PaperId = Request.Form["PaperId"];
            string PaperName = Request.Form["PaperName"];
            string Sequence = Request.Form["Sequence"];
            string Score = Request.Form["Score"];
            SqlParameter[] para = new SqlParameter[]{
                new SqlParameter("@PaperName",PaperName),
                new SqlParameter("@Sequence",Sequence)
            };
            commBll.UpdateInfo("tb_Bill_TestPaper", "PaperName=@PaperName,Sequence=@Sequence", " and ID=" + PaperId + "", para);
            for (int i = 0; i < TopicId.ToString().Split(',').Length - 1; i++)
            {
                int Count = commBll.GetRecordCount("tb_Bill_TestPaperEven", " and PaperID=" + PaperId + " and TopicID=" + TopicId.ToString().Split(',')[i] + "");
                if (Count > 0)
                {
                    commBll.DeleteInfo("tb_Bill_TestPaperEven", " and PaperID=" + PaperId + " and TopicID=" + TopicId.ToString().Split(',')[i] + "");
                }
                tb_Bill_TestPaperEven en = new tb_Bill_TestPaperEven();
                en.PaperID = Convert.ToInt32(PaperId);
                en.TopicID = Convert.ToInt32(TopicId.ToString().Split(',')[i]);
                en.Score = Convert.ToInt32(Score.ToString().Split(',')[i]);
                en.Operator = UId;
                en.AddOperator = UId;
                en.OperatorTime = DateTime.Now;
                en.Bill_Spare1 = 0;
                en.Bill_Spare2 = null;
                en.Bill_Spare3 = null;
                RowCount += evenBll.AddTestPaperBll(en);
            }
            return RowCount;
        }

        /// <summary>
        /// 试卷预览
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult LookTestPaper(int Id)
        {
            string Where = string.Empty;
            string EvenId = Request.Form["EvenId"];

            StringBuilder sb = new StringBuilder();
            ViewBag.PaperName = commBll.GetListSclar("PaperName", "tb_Bill_TestPaper", " and ID=" + Id);
            DataTable data = commBll.GetListDatatable("a.*,b.TopicTitle,b.TaskExplan,b.BillFormId,b.TopicTitle,(select sum(Score) from tb_Bill_TestPaperEven where PaperID=a.PaperID) as Score1", "tb_Bill_TestPaperEven as a inner join tb_Bill_Topic as b on a.TopicID=b.ID ", " and a.PaperID=" + Id + " and a.Bill_Spare1=1");
            ViewData["RelationTopicTable"] = data;
            ViewBag.Score = data.Rows[0]["Score1"].ToString();
            return View();
        }

        /// <summary>
        /// 读取题目信息
        /// </summary>
        /// <returns></returns>
        public string GetTaskPlan()
        {
            string EvenId = Request["EvenId"];
            string TaskPlan = commBll.GetListSclar("b.TaskExplan", "tb_Bill_TestPaperEven as a inner join tb_Bill_Topic as b on a.TopicID=b.ID", "and a.ID=" + EvenId);
            return JsonConvert.SerializeObject(TaskPlan);
        }

        /// <summary>
        /// 判断是否有相同的试卷名称
        /// </summary>
        /// <returns></returns>
        public int Paper_Pd()
        {
            string PaperName = Request["PaperName"];
            string Type = Request["Type"];
            string whe = string.Empty;
            if (Type == "1")
            {
                whe = "and ID not in (select ID from tb_Bill_TestPaper where  PaperName='" + PaperName + "' and  AddOperator=" + UId + " and Bill_Spare1=1)";
            }
            return commBll.GetRecordCount("tb_Bill_TestPaper", " and PaperName='" + PaperName + "' and  AddOperator=" + UId + " and Bill_Spare1=1" + whe);
        }

        /// <summary>
        /// 单据试卷导出脚本
        /// </summary>
        /// <returns></returns>
        public string BillExport()
        {
            string TopicId = Request["TopicId"];
            StringBuilder sb = new StringBuilder();
            //单据试卷关联表
            DataTable datagGL = commBll.GetListDatatable("*", "tb_Bill_TestPaperEven", " and PaperID in(" + TopicId + ") ");
            //单据试卷表
            DataTable dataPa = commBll.GetListDatatable("*", "tb_Bill_TestPaper", " and ID in(" + TopicId + ") ");
            //单据题目表
            DataTable dataTp = commBll.GetListDatatable("*", "tb_Bill_Topic", " and ID in(select TopicID from tb_Bill_TestPaperEven where PaperID in(" + TopicId + ")) ");
            //单据题目答案表
            string whr = " and BillFormId in (select BillFormId from tb_Bill_Topic where ID in(select TopicID from tb_Bill_TestPaperEven where PaperID in(" + TopicId + @")))
			and Bill_Spare1 in (select ID from tb_Bill_Topic where ID in(select TopicID from tb_Bill_TestPaperEven where PaperID in(" + TopicId + ")))";
            DataTable dataAn = commBll.GetListDatatable("*", "tb_Bill_Answer", whr);
            sb.AppendLine("set xact_abort on");
            sb.AppendLine("begin tran");
            sb.AppendLine("declare @TestPaperName nvarchar(max)='单据试卷'     --试卷名称");
            sb.AppendLine("declare @UserNo nvarchar(max)='T00001'     --所属登录编号");

            sb.AppendLine("declare @UserId int  -- 所属id ");//试卷id 
            sb.AppendLine("declare @UserType int  -- 所属类型 ");
            sb.AppendLine("declare @PaperId int  -- 试卷ID ");
            sb.AppendLine("declare @TopicId int  -- 题目ID ");
            sb.AppendLine("select top 1 @UserId=UId,@UserType=UserType from tb_UserInfo where UserNo=@UserNo and State<>0 ;");
            sb.AppendLine("if(@UserId is not null) ");
            sb.AppendLine("begin");
            //单据试卷表
            for (int j = 0; j < dataPa.Rows.Count; j++)
            {
                string ExtractUnused = dataPa.Rows[j]["ExtractUnused"].ToString();
                if (ExtractUnused == "")
                {
                    ExtractUnused = "NULL";
                }
                string SumExtract = dataPa.Rows[j]["SumExtract"].ToString();
                if (SumExtract == "")
                {
                    SumExtract = "NULL";
                }
                sb.AppendLine(" INSERT INTO tb_Bill_TestPaper   VALUES(" + dataPa.Rows[j]["PaperNumber"] + ",@TestPaperName," + dataPa.Rows[j]["Sequence"] + "," + SumExtract + "," + ExtractUnused + "," + dataPa.Rows[j]["Kind"] + ",@UserId,@UserId,GETDATE(),1,NULL,NULL) select @PaperId=SCOPE_IDENTITY();");
            }
            //单据题目表
            for (int q = 0; q < dataTp.Rows.Count; q++)
            {
                sb.AppendLine(" INSERT INTO tb_Bill_Topic   VALUES('" + dataTp.Rows[q]["TopicTitle"] + "','" + dataTp.Rows[q]["TaskExplan"].ToString().Replace("'", "\"") + "','" + dataTp.Rows[q]["BillFormId"] + "'," + dataTp.Rows[q]["BillType"] + ",@UserType,@UserId,@UserId,GETDATE(),1,NULL,NULL) select @TopicId=SCOPE_IDENTITY();");
                //单据试卷关联表
                sb.AppendLine(" INSERT INTO tb_Bill_TestPaperEven   VALUES(@PaperId,@TopicId," + datagGL.Rows[q]["Score"] + ",@UserId,@UserId,GETDATE(),1,NULL,NULL)");
                //单据题目答案表
                for (int a = 0; a < dataAn.Rows.Count; a++)
                {
                    if (dataTp.Rows[q]["BillFormId"].ToString() == dataAn.Rows[a]["BillFormId"].ToString())
                    {
                        sb.AppendLine(" INSERT INTO tb_Bill_Answer   VALUES('" + dataAn.Rows[a]["BillFormId"] + "','" + dataAn.Rows[a]["EnglishName"] + "','" +
                            dataAn.Rows[a]["Answer"] + "',@UserType,@UserId,@UserId,GETDATE(),@TopicId,NULL,NULL)");
                    }
                }
            }


            sb.AppendLine("end");
            sb.AppendLine("else");
            sb.AppendLine("begin");
            sb.AppendLine(" select '您输入的登录账号不存在用户！' ");
            sb.AppendLine("end");
            sb.AppendLine("commit tran");
            string fullPath = Server.MapPath("/Export");
            System.IO.File.WriteAllText(fullPath + "/单据试卷脚本.sql", sb.ToString(), Encoding.Default);
            string url = "/Export/单据试卷脚本.sql";
            return JsonConvert.SerializeObject(url);
        }
    }
}
