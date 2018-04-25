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
    /***************************************************************
    FileName:单据题目管理
    Copyright（c）2017-金融教育在线技术开发部
    Author:陈飞
    Create Date:2017-4-5
    ******************************************************************/
    public class BillTopicController : BaseController
    {
        //
        // GET: /Admin/BillTopic/
        CommonBll commBll = new CommonBll();
        Bill_AnswerBll AnswerBll = new Bill_AnswerBll();
        Bill_TopicBll btBll = new Bill_TopicBll();
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判

        /// <summary>
        /// 单据题目管理
        /// </summary>
        /// <returns></returns>
        public ActionResult TopicList()
        {
            DataTable data = commBll.GetListDatatable("distinct Bill_Spare2,BillType", "tb_Bills", "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value='0'>请选择单据类型</option>");
            foreach (DataRow dr in data.Rows)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", dr["BillType"], dr["Bill_Spare2"]);
            }
            ViewBag.BillType = sb.ToString();
            ViewData["UserType"] = UserType;
            return View();
        }

        /// <summary>
        /// 单据题目列表-查询
        /// </summary>
        /// <returns></returns>
        public string GetTopicList()
        {
            string strWhere = string.Empty;
            string BillType = Request["BillType"];
            string BillName = Request["BillName"];
            string TeamId = Request["TeamId"];
            string Creater = Request["Creater"];
            string BillTitle = Request["BillTitle"];
            //单据类型
            if (Convert.ToInt32(BillType) != 0)
            {
                strWhere = " and  b.BillType=" + BillType + "";
            }
            //单据名称
            if (BillName != "0")
            {
                strWhere += "and b.BillName = '" + BillName + "'";
            }
            if (UserType == "1")
            {
                //题目属性
                if (Convert.ToInt32(TeamId) == 0)
                {
                    strWhere += "and a.Kind=1";
                }
                if (Convert.ToInt32(TeamId) == 1 || Convert.ToInt32(TeamId) == 2)
                {
                    strWhere += "and a.Kind=" + TeamId + "";

                }
                if (Convert.ToInt32(TeamId) == 3)
                {
                    strWhere += "and (a.Kind=2 or a.Kind=1)";
                }
                //创建人
                if (Creater != "")
                {
                    strWhere += " and (select UserName from tb_UserInfo where UId=AddOperator) like '%" + Creater + "%'";
                }
            }
            //当前角色是教师
            if (UserType == "2")
            {
                strWhere += "and a.Kind=2 and a.AddOperator=" + UId + "";
            }
            //题目标题
            if (BillTitle != "")
            {
                strWhere += " and a.TopicTitle like '%" + BillTitle + "%'";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.Id desc"; //排序必须填写
            m.strFld = " a.*,b.BillName,b.Bill_Spare2  as BillType1 ,(select UserName from tb_UserInfo where UId=a.AddOperator) as UserName,(select UserNo from tb_UserInfo where UId=a.AddOperator) as UserNo";
            m.tab = "tb_Bill_Topic as a inner join tb_Bills as b  on a.BillType=b.BillType and a.BillFormId=b.BillFormId ";
            m.strWhere = " and a.Bill_Spare1=1" + strWhere;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));
        }

        /// <summary>
        /// 单据名称下拉框
        /// </summary>
        /// <param name="BillType">单据类型</param>
        /// <returns></returns>
        public string BillNameSelect(string BillType)
        {
            DataTable data = commBll.GetListDatatable("ID,BillName", "tb_Bills", " and BillType=" + BillType + "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value='0'>请选择单据名称</option>");
            foreach (DataRow dr in data.Rows)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", dr["BillName"], dr["BillName"]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 新增题目
        /// </summary>
        /// <returns></returns>
        public ActionResult AddTopic()
        {
            string action = Request.Form["action"];
            string strWhere = string.Empty;
            DataTable data = commBll.GetListDatatable("distinct Bill_Spare2,BillType", "tb_Bills", "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value='0'>请选择单据类型</option>");
            foreach (DataRow dr in data.Rows)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", dr["BillType"], dr["Bill_Spare2"]);
            }
            ViewBag.BillType = sb.ToString();
            //保存
            if (action == "Save")
            {
                string Summernote = Request.Form["Summernote"];
                string Title = Request.Form["Title"];
                string TopicId = Request.Form["TopicId"];
                SqlParameter[] para = new SqlParameter[]{
                new SqlParameter("@TaskExplan",Summernote),
                new SqlParameter("@TopicTitle",Title),
                new SqlParameter("@Bill_Spare1",1),
            };
                int Count = commBll.UpdateInfo("tb_Bill_Topic", "TaskExplan=@TaskExplan,TopicTitle=@TopicTitle,Bill_Spare1=@Bill_Spare1", " and ID=" + TopicId + "", para);
                ViewData["Count"] = Count;
            }
            return View();
        }

        /// <summary>
        /// 新增题目中的单据列表以及查询
        /// </summary>
        /// <returns></returns>
        public string AddTopicGetList()
        {
            string strWhere = string.Empty;
            string BillType = Request["BillType"];
            string BillName = Request["BillName"];
            string FormId = Request["FormId"];
            if (BillType != "0")
            {
                strWhere = "and BillType=" + BillType + "";
            }
            if (BillName != "" && BillName != "0")
            {
                strWhere = "and BillName='" + BillName + "'";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = " (case BillFormId when '" + FormId + "' then '1' else '2' end)"; //排序必须填写
            m.strFld = " *,(case BillFormId when '" + FormId + "' then '1' else '2' end) as abc";
            m.tab = "tb_Bills a";
            m.strWhere = strWhere;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt)); ;
        }

        /// <summary> 
        /// 读取表单定位数据
        /// </summary>
        /// <returns></returns>
        public string Bills(string FormId)
        {
            return commBll.GetListSclar("Bill_Spare3", "tb_Bills", " and BillFormId='" + FormId + "'");
        }

        /// <summary>
        ///读取题目答案 
        /// </summary>
        /// <param name="TopicId">题目Id</param>
        /// <param name="FormId">单据表单ID</param>
        /// <returns></returns>
        public string Bill_Data(string TopicId, string FormId)
        {
            DataTable data = commBll.GetListDatatable("*", "[tb_Bill_Answer]", " and Bill_Spare1=" + TopicId + " and  BillFormId='" + FormId + "'");
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// 读取操作数据
        /// </summary>
        /// <returns></returns>
        public string Bill_DataTwo()
        {
            DataTable data = commBll.GetListDatatable("*", "[tb_Operation_Answer]", " and EId=" + Request["ExamId"] + " and  PId='" + Request["PaperId"] + "' and QBId=" + Request["TopicId"] + " and MId=" + Request["UIds"] + " and BillFormId='" + Request["FormId"] + "' and [Type]=4 and State=1");
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// 保存答案以及插入题目数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BillsForm(string FormId, string Title, string Summernote, int Type, string TopicId)
        {
            tb_Bill_Answer ba = new tb_Bill_Answer();
            tb_Bill_Topic bt = new tb_Bill_Topic();
            DataTable data = commBll.GetListDatatable("*", "tb_Bill_Form", " and BillFormId='" + FormId + "'");
            if (TopicId == "" || TopicId == null)
            {
                //插入一行题目数据
                bt.TopicTitle = Title;
                bt.TaskExplan = Summernote;
                bt.BillFormId = FormId;
                bt.BillType = Type;
                bt.Kind = Convert.ToInt32(UserType);
                bt.Operator = UId;
                bt.AddOperator = UId;
                bt.OperatorTime = DateTime.Now;
                bt.Bill_Spare1 = 0;
                bt.Bill_Spare2 = null;
                bt.Bill_Spare3 = null;
                int Count = btBll.AddTopicBll(bt);
                //查询单据题目中某个单据最新的一条数据的ID
                TopicId = btBll.QueryTopicIdBll(Convert.ToInt32(UId), FormId).ToString();
            }
            ViewData["TopicId"] = TopicId;
            ////删除题目以前设置的答案
            int Count1 = commBll.DeleteInfo("tb_Bill_Answer", " and AddOperator=" + UId + " and Bill_Spare1=" + TopicId + "");
            //循环插入答案
            for (int i = 0; i < data.Rows.Count; i++)
            {
                string InputValue = Request["" + data.Rows[i]["EnglishName"] + ""].ToString();
                if (InputValue != null)
                {
                    ba.BillFormId = FormId;
                    ba.EnglishName = data.Rows[i]["EnglishName"].ToString();
                    ba.Answer = InputValue;
                    ba.Kind = Convert.ToInt32(UserType);
                    ba.Operator = UId;
                    ba.AddOperator = UId;
                    ba.OperatorTime = DateTime.Now;
                    ba.Bill_Spare1 = Convert.ToInt32(TopicId); //题目ID
                    ba.Bill_Spare2 = null;
                    ba.Bill_Spare3 = null;
                    Count1 = AnswerBll.AddAnswerBll(ba);
                    ViewData["Count"] = Count1;
                }
            }
            SqlParameter[] para = new SqlParameter[] { };
            commBll.UpdateInfo("tb_Bill_Topic", "BillFormId='" + FormId + "'", "and ID=" + TopicId + "", para);
            return View("Bills" + FormId + "", Count1);
        }

        /// <summary>
        /// 题目查看
        /// </summary>
        /// <returns></returns>
        public ActionResult LookTopic(int ID)
        {

            DataTable data = commBll.GetListDatatable("*", "tb_Bill_Topic", " and ID=" + ID + "");
            if (data.Rows.Count > 0)
            {
                ViewData["TopicTitle"] = data.Rows[0]["TopicTitle"].ToString();
                ViewData["TaskExplan"] = data.Rows[0]["TaskExplan"].ToString();
                ViewData["BillStyle"] = Dictionary.GetDictStyle(data.Rows[0]["BillFormId"].ToString());
            }
            else
            {
                ViewData["TopicTitle"] = "";
                ViewData["TaskExplan"] = "";
            }
            return View();
        }

        /// <summary>
        /// 删除题目
        /// </summary>
        /// <param name="Id">题目ID</param>
        /// <returns></returns>
        public int TopicDele(string TopicId)
        {
            SqlParameter[] para = new SqlParameter[] { };
            int Count = commBll.UpdateInfo("tb_Bill_Topic", "Bill_Spare1='-1'", " and ID in(" + TopicId + ")", para);
            return Count;
        }

        /// <summary>
        /// 判断同一题目是否有设置过答案
        /// </summary>
        /// <param name="TopicId">题目ID</param>
        /// <returns></returns>
        public int Answer_Pd(string TopicId)
        {
            if (TopicId == "" || TopicId == null) { TopicId = "0"; }
            return commBll.GetRecordCount("tb_Bill_Answer", " and Bill_Spare1=" + TopicId + "");
        }

        /// <summary>
        /// 判断是否有相同的题目标题
        /// </summary>
        /// <returns></returns>
        public int Topic_Pd()
        {
            string Title = Request["Title"];
            string Type = Request["Type"];
            string whe = string.Empty;
            if (Type == "1")
            {
                whe = "and ID not in (select ID from tb_Bill_Topic where  TopicTitle='" + Title + "' and  AddOperator=" + UId + " and Bill_Spare1=1)";
            }
            return commBll.GetRecordCount("tb_Bill_Topic", " and TopicTitle='" + Title + "' and  AddOperator=" + UId + " and Bill_Spare1=1" + whe);
        }

        /// <summary>
        /// 编辑页面赋值
        /// </summary>
        /// <param name="ID">题目ID</param>
        /// <param name="FormId">单据表单ID</param>
        /// <returns></returns>
        public ActionResult EditTopic(string ID, string formId)
        {
            StringBuilder sb = new StringBuilder();
            if (ID == null || ID == "")
            {
                ID = Request.Form["TopicId"];
            }
            string strWhere = string.Empty;
            string action = Request.Form["action"];
            ViewData["TopicId"] = ID;
            ViewData["formId"] = formId;
            if (formId != "")
            {
                Session["SeformId"] = formId;
            }
            if (formId == "")
            {
                ViewData["formId"] = Session["SeformId"];
            }
            DataTable data = commBll.GetListDatatable("distinct Bill_Spare2,BillType", "tb_Bills", "");
            sb.Append("<option value='0'>请选择单据类型</option>");
            foreach (DataRow dr in data.Rows)
            {
                sb.AppendFormat("<option value='{0}'>{1}</option>", dr["BillType"], dr["Bill_Spare2"]);
            }
            ViewBag.BillType = sb.ToString();

            if (action == "Save")
            {
                string Summernote = Request.Form["Summernote"];
                string Title = Request.Form["Title"];
                SqlParameter[] para = new SqlParameter[]{
                new SqlParameter("@TaskExplan",Summernote),
                new SqlParameter("@TopicTitle",Title),
                new SqlParameter("@Bill_Spare1",1),
            };
                int Count = commBll.UpdateInfo("tb_Bill_Topic", "TaskExplan=@TaskExplan,TopicTitle=@TopicTitle,Bill_Spare1=@Bill_Spare1", " and ID in(" + ID + ")", para);
                ViewData["Count"] = Count;
            }
            data = commBll.GetListDatatable("*", "tb_Bill_Topic", " and ID=" + ID + "");
            if (data.Rows.Count > 0)
            {
                ViewData["TopicTitle"] = data.Rows[0]["TopicTitle"].ToString();
                ViewData["TaskExplan"] = data.Rows[0]["TaskExplan"].ToString();
            }
            return View();
        }

        /// <summary>
        /// 题目管理编辑后数据保存
        /// </summary>
        /// 
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditSave(string Summernote, string Title, string TopicId, string action)
        {
            DataTable data = new DataTable();

            ViewData["formId"] = Request.Form["FormId"];
            return View("EditTopic", data);
            //return RedirectToAction("EditTopic", "BillTopic", new { ID = TopicId, FormId = FormId });
        }


        #region  单据表单

        /// <summary>
        /// 借款审批单
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010001()
        {
            return View();
        }

        /// <summary>
        /// 银行普通支票
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010002()
        {
            return View();
        }

        /// <summary>
        /// 银行现金支票
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010003()
        {
            return View();
        }

        /// <summary>
        /// 银行转账支票
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010004()
        {
            return View();
        }

        /// <summary>
        /// 银行汇票回联
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010005()
        {
            return View();
        }

        /// <summary>
        /// 银行汇票借方凭证
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010006()
        {
            return View();
        }

        /// <summary>
        /// 银行汇票贷方凭证
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010007()
        {
            return View();
        }

        /// <summary>
        ///出库单—存根
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010008()
        {
            return View();
        }

        /// <summary>
        ///出库单—会计
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010009()
        {
            return View();
        }

        /// <summary>
        ///出库单—提货单位
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010010()
        {
            return View();
        }

        /// <summary>
        ///收料单—存款
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010011()
        {
            return View();
        }

        /// <summary>
        ///收料单—会计附单据
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010012()
        {
            return View();
        }

        /// <summary>
        ///收料单—转仓库
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010013()
        {
            return View();
        }

        /// <summary>
        ///银行本票申请人回联
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010014()
        {
            return View();
        }

        /// <summary>
        ///银行本票申请人借方凭证
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010015()
        {
            return View();
        }

        /// <summary>
        ///银行本票申请人贷方凭证
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010016()
        {
            return View();
        }

        /// <summary>
        ///入库单—仓库存留
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010017()
        {
            return View();
        }

        /// <summary>
        ///入库单—财务记账
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010018()
        {
            return View();
        }

        /// <summary>
        ///入库单—仓库记账
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010019()
        {
            return View();
        }

        /// <summary>
        ///领料单—存根
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010020()
        {
            return View();
        }

        /// <summary>
        ///领料单—会计付款
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010021()
        {
            return View();
        }

        /// <summary>
        ///领料单—记账
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010022()
        {
            return View();
        }

        /// <summary>
        ///报销单
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010023()
        {
            return View();
        }

        /// <summary>
        ///付款单
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010024()
        {
            return View();
        }

        /// <summary>
        ///开票单
        /// </summary>
        /// <returns></returns>
        public ActionResult Bills010025()
        {
            return View();
        }
        #endregion
    }
}
