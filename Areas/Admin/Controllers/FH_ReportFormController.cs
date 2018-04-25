using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VocationalProject.App_Start;
using VocationalProject.Areas.Admin.Models;
using VocationalProject.Controllers;
using VocationalProject_Bll;
using VocationalProject_Dal;
using VocationalProject_DBUtility.Sql;
using VocationalProject_Models;

namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
      FileName:复核报表
      Copyright（c）2017-金融教育在线技术开发部
      Author:唐
      Create Date:
     ******************************************************************/
    public class FH_ReportFormController : BaseController
    {
        // GET: /Admin/HB_QuestionBank/
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        //string UserNo = "admin";//登录账号
        CommonBll commonbll = new CommonBll();
        FH_TestPaperBll perBll = new FH_TestPaperBll();
        FH_RelationTopicBll evenBll = new FH_RelationTopicBll();

        #region  题目管理 2017-4-1
        /// <summary>
        /// 题目列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Topic()
        {
            ViewData["UserType"] = UserType;
            ViewData["UserId"] = UId;
            return View();
        }


        /// <summary>
        /// 题目列表
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {
            string wheres = " and a.IsDelete=1 ";

            //角色区分  //暂时不考虑独立打包客户，需要web加个配置 读取 控制 后期加
            
            if (!string.IsNullOrEmpty(Request["Title"]))//题目标题
            {
                wheres += " and a.Title like'%" + Request["Title"].ToString() + "%'";
            }
            if (UserType == "2")//教师  如果是教师就只显示自己的题目
            {
                wheres += " and a.Kind=2 and a.AddOperator=" + UId + @" ";//"or Id in (select UR_CompetitionId from tb_PaperUserRights where UR_CompetitionType=3 and UR_SchoolId=" + TeacherSchoolId + "))";
            }
            else
            {
                if (!string.IsNullOrEmpty(Request["Kind"]) && Request["Kind"].ToString() != "0")//题目属性  1系统 2教师
                {
                    if (Request["Kind"].ToString() != "3")
                    {
                        wheres += " and a.Kind=" + Request["Kind"].ToString() + "";
                    }
                    else 
                    { 
                    
                    }
                }
                else
                {
                    wheres += " and a.Kind=" + 1 + "";
                }
            }
            if (!string.IsNullOrEmpty(Request["AddOperator"]))//题目来源
            {
                wheres += " and b.UserName like '%" + Request["AddOperator"].ToString() + "%'";
            } 
            //UserType 把用角色拼接
            // DataTable dt = commonbll.GetListDatatable("*", "tb_HB_QuestionBank", wheres);

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.Id desc"; //排序必须填写
            m.strFld = " a.*,b.UserNo,b.UserName ";
            m.tab = "tb_FH_Topic  a left join tb_UserInfo b on(a.AddOperator=b.UId) ";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);

            var list = JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt);
            //JsonResult json = new JsonResult();
            //json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //json.Data = list;
            //return json;
           // return JsonConvert.SerializeObject(list); ;
            //return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, dt);
            return JsonConvert.SerializeObject(list);
        }
        /// <summary>
        /// 删除题目
        /// </summary>
        /// <returns></returns>
        public string DelStudent()
        {
            try
            {
                var Ids = Request["Ids"];
                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@Operator",UId),
                    new SqlParameter("@IsDelete","0")
                };
                //修改状态 操作人
                commonbll.UpdateInfo("tb_FH_Topic", " IsDelete=@IsDelete,Operator=@Operator", " and Id in(" + Ids + ")", pars);

                return "1";
            }
            catch
            {
                return "99";
            }

        }

        /// <summary>
        /// 题目编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult EditTopic(int? Id, string Title = "")
        {
            ViewData["Id"] = Id;
            ViewData["Titles"] = Title;
            ViewData["Type"] = 0;
            ViewData["IsType"] = 0;
            return View();
        }
        /// <summary>
        /// 查看
        /// </summary>
        /// <returns></returns>
        public ActionResult EditsTopic(int? Id, int Type = 0, string Title = "")
        {
            ViewData["Id"] = Id;
            ViewData["Titles"] = Title;
            ViewData["Type"] = 1;
            ViewData["IsType"] = Type;
            return View();
        }

        public string GetModelById(int Id)
        { 
            var tb = commonbll.GetListDatatable("*", "tb_FH_Tests", " and TopicId=" + Id);
            return JsonConvert.SerializeObject(tb);
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public int Edit(string entityList, string Title,int? Id)
        { 
            int count = 0;
            tb_FH_Topic Topic = new tb_FH_Topic();
            var TestsList = new List<FH_TestsModel>(); 
            try
            { 
                TestsList = JsonHelper.JSONToObject<List<FH_TestsModel>>(HttpUtility.UrlDecode(entityList));
            }
            catch
            {
                return -1;
            }
            Topic.Title = Title;
            //Topic.TaskExplan = ""; //说明
            Topic.Operator = UId;
            

            string table = "tb_FH_Topic"; //表名

            string Countstr = UserType == "2" ? " and AddOperator='" + UId + "'" : "";
            if (Id == null)//新增
            {
                Topic.AddOperator = UId;
                Topic.AddTime = DateTime.Now;
                Topic.Kind = UserType == "1" ? 1 : 2;//归属 1系统 2教师
                Topic.PushType = 0;    //推送类型 0否，1是。是否系统推送（推送不能修改）
                if (!GetCount("tb_FH_Topic", " and IsDelete=1 and Title='" + Title + "' and Kind=" + (UserType == "1" ? 1 : 2) + Countstr))
                {
                    string list = "Title, PushType, Kind, Operator, AddOperator, AddTime";//列
                    string vlaue = "@Title, @PushType, @Kind, @Operator, @AddOperator, @AddTime";
                    SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@Title",Topic.Title),
                        new SqlParameter("@PushType",Topic.PushType), 
                        new SqlParameter("@Kind",Topic.Kind),
                        new SqlParameter("@Operator",Topic.Operator),
                        new SqlParameter("@AddOperator",Topic.AddOperator),
                        new SqlParameter("@AddTime",Topic.AddTime) 

                     };
                    var r = commonbll.AddIdentity(table, list, vlaue, pars);
                    if (Convert.ToInt32(r) > 0)
                    {
                        AddTestslist(TestsList, Convert.ToInt32(r));
                    }
                    else
                    {
                        return -3;
                    }
                }
                else
                {
                    return -2; //存在重复
                }
            }
            else//修改
            {
                if (!GetCount("tb_FH_Topic", "  and IsDelete=1  and Id<>" + Id + " and Title='" + Title + "' and Kind=" + (UserType == "1" ? 1 : 2) + Countstr))
                {
                    string set = @"Title=@Title,Operator=@Operator, UpdateTime=@UpdateTime";
                    string strWhere = " and Id=@Id";
                    SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@Title",Topic.Title),
                        new SqlParameter("@Operator",Topic.Operator), 
                        new SqlParameter("@UpdateTime",DateTime.Now),
                        new SqlParameter("@Id",Id)

                     };
                    var r = commonbll.UpdateInfo(table, set, strWhere, pars);
                    if (r >= 0)
                    {
                        AddTestslist(TestsList, (int)Id);
                    }
                    else
                    {
                        return -3;
                    }
                }
                else
                {
                    return -2; //存在重复
                }
            }
             
            return count;
        }
        public int AddTestslist(List<FH_TestsModel> TestsList, int Id)
        {
            if (TestsList.Count > 0)
            {
                string sql = " delete from tb_FH_Tests where TopicId=" + Id + " ;";
                foreach (var item in TestsList)
                {
                    string fie = "";
                    string val = "";
                    if (item.Final != null)
                    {
                        fie += ",FinalBalance";
                        val += "," + item.Final;
                    }
                    if (item.Beginning != null)
                    {
                        fie += ",BeginningBalance";
                        val += "," + item.Beginning;
                    }

                    sql += " insert into tb_FH_Tests(TopicId,Assets,Line" + fie + ",Rights)";
                    sql += " values(" + Id + ",'" + item.Assets + "'," + item.Line + "" + val + "," + item.Type + ");";
                }
                SqlHelper.ExecuteNonQuery(sql.ToString());
            }
            return 0;
        }



        #endregion

        #region  试卷管理 2017-4-8
        public ActionResult TestPaper()
        {
            ViewData["UserType"] = UserType;
            ViewData["UserId"] = UId;
            return View();
        } 
        /// <summary>
        /// 试卷列表
        /// </summary>
        /// <returns></returns>
        public string GetTestPaperList()
        {
            string wheres = " and a.IsDelete=1 ";

            //角色区分  //暂时不考虑独立打包客户，需要web加个配置 读取 控制 后期加
           
            if (!string.IsNullOrEmpty(Request["TestPaperName"]))//题目标题
            {
                wheres += " and a.TestPaperName like'%" + Request["TestPaperName"].ToString() + "%'";
            }
            if (UserType == "2")//教师  如果是教师就只显示自己的题目
            {
                wheres += "  and (a.AddOperator=" + UId + @" or Id in (select UR_CompetitionId from tb_PaperUserRights where UR_CompetitionType=3 and UR_SchoolId=" + TeacherSchoolId + "))";
            }
            else
            {
                if (!string.IsNullOrEmpty(Request["Kind"]) && Request["Kind"].ToString() != "0")//题目属性  1系统 2教师
                {
                    if (Request["Kind"].ToString() != "3")
                    {
                        wheres += " and a.Kind=" + Request["Kind"].ToString() + "";
                    }
                    
                }
                else
                {
                    wheres += " and a.Kind=" + 1 + "";
                }
            }
            if (!string.IsNullOrEmpty(Request["AddOperator"]))//题目来源
            {
                wheres += " and b.UserName like '%" + Request["AddOperator"].ToString() + "%'";
            }
            //UserType 把用角色拼接
            // DataTable dt = commonbll.GetListDatatable("*", "tb_HB_QuestionBank", wheres);

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.Id desc"; //排序必须填写
            m.strFld = " a.*,b.UserNo,b.UserName,(select sum(c.TestsFraction) from tb_FH_RelationTopic c where c.TestPaperID=a.Id ) as Total ";
            m.tab = "tb_FH_TestPaper  a left join tb_UserInfo b on(a.AddOperator=b.UId)";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);

            var list = JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt);
            //JsonResult json = new JsonResult();
            //json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //json.Data = list;
            //return json;
            // return JsonConvert.SerializeObject(list); ;
            //return JsonResultPagedList(PageCount, m.PageIndex, m.PageSize, dt);
            return JsonConvert.SerializeObject(list);
        }


        /// <summary>
        /// 试卷手工组卷、智能组卷、修改
        /// </summary>
        /// <param name="Id">试卷Id</param>
        /// <param name="Type">类型 1=手工、2=智能</param>
        /// <returns></returns>
        public ActionResult EditTestPaper(int? Id,int Type=1)
        {
            ViewData["UserType"] = UserType; //角色类型
            ViewData["Type"] = Type;  //试卷类型
            ViewData["Id"] = Id;     //试卷id

            ViewData["Sum_Number"] = ""; //题目总和
            ViewData["Unused"] = ""; //题目总和（未使用）

            ViewData["Table"] = null; 
            string whe = "";
            if (UserType == "2")
            {
                whe = " and AddOperator=" + UId + "";
            }
            if (Type == 2)
            {
                ViewData["Sum_Number"] = commonbll.GetRecordCount("tb_FH_Topic", whe + " and Kind=" + UserType + "   and IsDelete=1");
                ViewData["Unused"] = commonbll.GetRecordCount("tb_FH_Topic", whe + " and Kind=" + UserType + " and IsDelete=1 and  Id not in(select TopicID from tb_FH_RelationTopic where TestPaperID in(select Id from tb_FH_TestPaper where IsDelete=1 and Kind=" + UserType + whe + " ))");

            }
            else
            {
                string wher = "";
                if (Id != null)
                {
                    wher = " and t.Id=" + Id;

                    string fm = @",(select ISNULL(SUM(TestsFraction),0) from tb_FH_RelationTopic  where TestPaperID=t.Id) as Fraction,
                                (select ISNULL(count(1),0) from tb_FH_RelationTopic where TestPaperID=t.Id ) as count";
                    ViewData["Table"] = commonbll.GetListDatatable("t.*" + fm, "tb_FH_TestPaper t", wher);
                     

                }

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
            string ExtractUnused = Request.Form["ExtractUnused"];
            //每到题目分数
            string Score = Request.Form["Score"];
            //是否打乱同一题型下的试题顺序
            string Sequence = Request.Form["Sequence"];
            int Count = commonbll.GetRecordCount("tb_Bill_TestPaper", "");
            tb_FH_TestPaper paper = new tb_FH_TestPaper();
            paper.TestPaperN0 = Count.ToString();
            paper.TestPaperName = PaperName.ToString();
            paper.IsType = Convert.ToInt32(Sequence);
            paper.RandomNumber = Convert.ToInt32(SumExtract);
            paper.NotUsedNumber = Convert.ToInt32(ExtractUnused);
            paper.Kind = Convert.ToInt32(UserType);
            paper.Operator = UId;
            paper.AddOperator = UId;
            paper.AddTime = DateTime.Now;
            paper.IsDelete = 1;
            var RowCount = 0;
            string Countstr = UserType == "2" ? " and AddOperator='" + UId + "'" : "";
            if (GetCount("tb_FH_TestPaper", " and IsDelete=1 and TestPaperName='" + PaperName + "' and Kind=" + (UserType == "1" ? 1 : 2) + Countstr))
            {
                return JsonConvert.SerializeObject(-2);
            } 
            //string PaperId = commonbll.GetListSclar(" top 1 ID", "tb_FH_TestPaper", " and AddOperator=" + UId + " order by OperatorTime desc");
            //判断不为空时需要抽取没有被使用的题目
            string publiwhere = "";
            if (UserType == "2")//教师  如果是教师就只显示自己的题目
            {
                publiwhere = "  and AddOperator=" + UId + @" ";//"or Id in (select UR_CompetitionId from tb_PaperUserRights where UR_CompetitionType=3 and UR_SchoolId=" + TeacherSchoolId + "))";
            }
            if (!string.IsNullOrEmpty(ExtractUnused) && ExtractUnused != "0")
            {
                var data1 = new DataTable();
                var data2 = new DataTable();
               
                //查询出有使用过的题目
                strWhere = " and Kind=" + UserType + " and IsDelete=1 and  Id in(select TopicID from tb_FH_RelationTopic where TestPaperID in(select Id from tb_FH_TestPaper where IsDelete=1 and Kind=" + UserType + publiwhere + "))";
                data1 = commonbll.GetListDatatable("TOP " + (Convert.ToInt32(SumExtract) - Convert.ToInt32(ExtractUnused)) + " *", "tb_FH_Topic", "" + strWhere + publiwhere + "ORDER BY NEWID()");
               

                //查询出没有使用过的题目
                strWhere = " and Kind=" + UserType + " and IsDelete=1 and  Id not in(select TopicID from tb_FH_RelationTopic where TestPaperID in(select Id from tb_FH_TestPaper where IsDelete=1 and Kind=" + UserType + publiwhere + "))";


                data2 = commonbll.GetListDatatable("TOP " + ExtractUnused + " *", "tb_FH_Topic", "" + strWhere + publiwhere + "ORDER BY NEWID()");

                int datacout = (data1 != null ? data1.Rows.Count : 0) + (data2 != null ? data2.Rows.Count : 0);
                if (datacout != Convert.ToInt32(SumExtract))
                {
                    return JsonConvert.SerializeObject(-1);
                } 
                var PaperId = perBll.AddTestPaperBll(paper);

                for (int i = 0; i < data1.Rows.Count; i++)
                {
                    tb_FH_RelationTopic en = new tb_FH_RelationTopic();
                    en.TestPaperID = Convert.ToInt32(PaperId);
                    en.TopicId = Convert.ToInt32(data1.Rows[i]["ID"]);
                    en.TestsFraction = Convert.ToDecimal(Score);
                    var r = evenBll.AddTestPaperBll(en);
                    if (Convert.ToInt32(r) <= 0)
                    {
                        RowCount = 1;
                    }
                }
                for (int i = 0; i < data2.Rows.Count; i++)
                {
                    tb_FH_RelationTopic en = new tb_FH_RelationTopic();
                    en.TestPaperID = Convert.ToInt32(PaperId);
                    en.TopicId = Convert.ToInt32(data2.Rows[i]["ID"]);
                    en.TestsFraction = Convert.ToDecimal(Score);
                    var r = evenBll.AddTestPaperBll(en);
                    if (Convert.ToInt32(r) <= 0)
                    {
                        RowCount = 1;
                    }
                }
            }
            else
            {
                //查询出有使用过的题目
                strWhere = " and Kind=" + UserType + " and IsDelete=1 ";
                data = commonbll.GetListDatatable("TOP " + SumExtract + " *", "tb_FH_Topic", "" + strWhere + publiwhere + "ORDER BY NEWID()");

                int datacout = (data != null ? data.Rows.Count : 0); 
                if (data.Rows.Count != Convert.ToInt32(SumExtract))
                {
                    return JsonConvert.SerializeObject(-1);
                } 
                var PaperId = perBll.AddTestPaperBll(paper);
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    tb_FH_RelationTopic en = new tb_FH_RelationTopic();
                    en.TestPaperID = Convert.ToInt32(PaperId);
                    en.TopicId = Convert.ToInt32(data.Rows[i]["ID"]);
                    en.TestsFraction = Convert.ToDecimal(Score);
                    var r = evenBll.AddTestPaperBll(en);
                    if (Convert.ToInt32(r) <= 0)
                    {
                        RowCount = 1;
                    }
                }
            }
            return JsonConvert.SerializeObject(RowCount);
        }

        /// <summary>
        /// 导出脚本
        /// </summary>
        /// <returns></returns>
        public string ExportJob(string TestPaperId)
        {
            var TestPaper = SqlHelper.ExecuteDataTable(string.Format("select * from tb_FH_TestPaper where Id in({0})", TestPaperId));//试卷表
            StringBuilder str = new StringBuilder();
            if (TestPaper != null)
            {
                str.AppendLine("set xact_abort on");
                str.AppendLine("begin tran");
                str.AppendLine("declare @TestPaperName nvarchar(max)='试卷1'     --试卷名称");//试卷id
                str.AppendLine("declare @UserNo nvarchar(max)='T00001'     --所属登录编号");//试卷id

                str.AppendLine("declare @UserId int  -- 所属id ");//试卷id
                str.AppendLine("declare @UserType int  -- 所属类型 ");//试卷id
                str.AppendLine("declare @TestPaperId int -- 试卷id ");//试卷id
                str.AppendLine("declare @TopicId int  -- 试卷题目主表id ");//试卷题目主表id

                str.AppendLine("select top 1 @UserId=UId,@UserType=UserType from tb_UserInfo where UserNo=@UserNo and State<>0 ;");
                str.AppendLine("if(@UserId is not null) ");
                str.AppendLine("begin");
                foreach (DataRow Testitem in TestPaper.Rows)
                {
                    str.AppendLine(" insert into tb_FH_TestPaper(TestPaperN0,TestPaperName,IsType,kind,Operator,AddOperator,IsDelete)");
                    str.AppendLine(" values('" + Testitem["TestPaperN0"] + "',@TestPaperName," + Testitem["IsType"] + ",@UserType,@UserId,@UserId," + Testitem["IsDelete"] + ")");
                    str.AppendLine(";select @TestPaperId=SCOPE_IDENTITY();");

                    var RelationTopic = SqlHelper.ExecuteDataTable(string.Format("select t.*,r.TestsFraction from tb_FH_Topic t left join tb_FH_RelationTopic r on(t.Id=r.TopicId) where  t.IsDelete=1 and r.TestPaperId={0}", Testitem["Id"]));//题目和试卷关联表

                    foreach (DataRow Topicitem in RelationTopic.Rows)
                    {
                        str.AppendLine(" insert into tb_FH_Topic(Title,PushType,Kind,Operator,AddOperator,IsDelete)");
                        str.AppendLine(" values('" + Topicitem["Title"] + "'," + Topicitem["PushType"] + ",@UserType,@UserId,@UserId," + Topicitem["IsDelete"] + ")");
                        str.AppendLine(";select @TopicId=SCOPE_IDENTITY();");

                        str.AppendLine(" insert into tb_FH_RelationTopic(TopicId,TestPaperID,TestsFraction) ");
                        str.AppendLine(" values (@TopicId,@TestPaperId," + Topicitem["TestsFraction"] + "); ");


                        var Tests = SqlHelper.ExecuteDataTable(string.Format("select * from tb_FH_Tests where TopicId={0}", Topicitem["Id"]));//题目详细列表
                        foreach (DataRow item in Tests.Rows)
                        {
                            var FinalBalance = "";
                            var FinalBalances = "";

                            var BeginningBalance = "";
                            var BeginningBalances = "";
                            if (item["FinalBalance"] != null && !string.IsNullOrEmpty(item["FinalBalance"].ToString()))
                            {
                                FinalBalance = ",FinalBalance";
                                FinalBalances = "," + item["FinalBalance"] + "";
                            }
                            if (item["BeginningBalance"] != null && !string.IsNullOrEmpty(item["BeginningBalance"].ToString()))
                            {
                                BeginningBalance = ",BeginningBalance";
                                BeginningBalances = "," + item["BeginningBalance"] + "";
                            }
                            str.AppendLine(" insert into tb_FH_Tests(TopicId,Assets,Line" + FinalBalance + BeginningBalance + ",Rights) ");
                            str.AppendLine(" values (@TopicId,'" + item["Assets"] + "'," + item["Line"] + FinalBalances + BeginningBalances + "," + item["Rights"] + "); ");

                        }

                    }
                }

                str.AppendLine("end");
                str.AppendLine("else");
                str.AppendLine("begin");
                str.AppendLine(" select '您输入的登录账号不存在用户！' ");
                str.AppendLine("end");

                str.AppendLine("commit tran");
            }



            //str.AppendLine("set xact_abort on");
            //str.AppendLine("begin tran");
            //str.AppendLine("declare @planId int,@taskId int,@ContestId int,@BankSiteId int");
            //str.AppendLine("select top 1 @ContestId= Id from dal_Contest where Id=" + ContestId + " order by Id");
            //str.AppendLine("if(@ContestId is null)");
            //str.AppendLine("begin");
            //str.AppendLine("set @ContestId=1");
            //str.AppendLine("end");
            ////str.AppendLine(string.Format("select @ContestId={0}", ContestId));
            //str.AppendLine(string.Format("insert into dbo.dal_ComplexPlan(Plan_Name,Contest_Id,Status,Remarks)values('{0}',{1},1,null);select @planId=SCOPE_IDENTITY()", NewComplexPlanName, "@ContestId"));
            ////DataTable dtTask = SqlHelper.ExecuteDataTable(string.Format("select * from dal_ComplexTask where ComplexPlan_Id={0}", ComplexPlanId));

            ////foreach (DataRow row in dtTask.Rows)
            ////{
            ////    string Remarks = row["Remarks"].ToString().Replace("'", "''");
            ////    string Important_Info = row["Important_Info"].ToString().Replace("'", "''");
            ////    str.AppendLine(string.Format("insert into dbo.dal_ComplexTask(ComplexPlan_Id,Contest_Id,Task_Name,Remarks,Important_Info,Idx,IfUsed)values({0},{1},'{2}','{3}','{4}',{5},{6});select @taskId=SCOPE_IDENTITY()", "@planId", "@ContestId", row["Task_Name"], Remarks, Important_Info, row["Idx"], Convert.IsDBNull(row["IfUsed"]) ? "null" : row["IfUsed"]));
            ////    DataTable dtItems = SqlHelper.ExecuteDataTable(string.Format("select * from dal_ComplexTask_Items where Task_Id={0}", row["Id"]));
            ////    foreach (DataRow item in dtItems.Rows)
            ////    {
            ////        str.AppendLine(string.Format("insert into dbo.dal_ComplexTask_Items(Task_Id,TM_Parent_No,TM_No,Remark_Id,Remark_Name,Key_Ids,Key_Names,Score,Is_Connect_Up,Connect_UP_Id,NewId,Idx)values({0},'{1}','{2}',{3},'{4}','{5}','{6}',{7},'{8}',{9},{10},{11})", " @taskId", item["TM_Parent_No"], item["TM_No"], Convert.IsDBNull(item["Remark_Id"]) ? "null" : item["Remark_Id"], item["Remark_Name"], item["Key_Ids"], item["Key_Names"], item["Score"], item["Is_Connect_Up"], Convert.IsDBNull(item["Connect_UP_Id"]) ? "null" : item["Connect_UP_Id"], Convert.IsDBNull(item["NewId"]) ? "null" : item["NewId"], Convert.IsDBNull(item["Idx"]) ? "null" : item["Idx"]));
            ////    }
            ////}
            //str.AppendLine("select top 1 @BankSiteId=Id from dbo.bi_BankSite order by Id desc");
            //str.AppendLine("if(@BankSiteId is  null)");
            //str.AppendLine("begin");
            //str.AppendLine("set @BankSiteId=1");
            //str.AppendLine("end");
            //str.AppendLine("insert into dbo.dal_ComplexTimer (ContestId,BankSiteId,ComplexPlanId,BeginTime,ExamLength,ExamStatus,AddTime,IfTimeScore,RepeatNum)values(@ContestId,@BankSiteId,@planId,GETDATE(),1000060,1,GETDATE(),0,9999)");

            string fullPath = Server.MapPath("/Export");
            System.IO.File.WriteAllText(fullPath + "/复核试卷脚本.sql", str.ToString(), Encoding.Default);
            string url = "/Export/复核试卷脚本.sql";
            return JsonConvert.SerializeObject(url);

        }

        /// <summary>
        /// 试卷预览
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult PreviewTestPaper(int? Id, string Title)
        {
            //string TestsFraction = commonbll.GetListSclar(" sum(TestsFraction) ", "tb_FH_RelationTopic", " and TestPaperID=" + Id);
            ViewData["Id"] = Id;
            ViewData["TitleName"] = Title;
            //ViewData["TestsFraction"] = TestsFraction;
            var data = commonbll.GetListDatatable("*,(select top 1 Title from tb_FH_Topic where Id=TopicId) as Title", "tb_FH_RelationTopic", " and TestPaperID=" + Id);
            ViewData["RelationTopicTable"] = data;
            return View();
        } 
        /// <summary>
        /// 编辑查询
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string GetTestPaperByListId()
        {
            string wheres = " and a.IsDelete=1 and a.Kind= " + UserType;
            var Id = Request["Id"].ToString();
            //角色区分  //暂时不考虑独立打包客户，需要web加个配置 读取 控制 后期加
            if (UserType == "2")//教师  如果是教师就只显示自己的题目
            {
                wheres += " and (a.AddOperator=" + UId + @" or Id in (select UR_CompetitionId from tb_PaperUserRights where UR_CompetitionType=3 and UR_SchoolId=" + TeacherSchoolId + "))";
            }
            if (!string.IsNullOrEmpty(Request["Title"]))//题目标题
            {
                wheres += " and a.Title like'%" + Request["Title"].ToString() + "%'";
            }
            if (string.IsNullOrEmpty(Id))
            {
                Id = "-1";
            }
            if (!string.IsNullOrEmpty(Request["State"]) && Request["State"].ToString() != "0")//是否加入当前试卷 1 是、 0 否
            {
                if (Request["State"].ToString() == "1")
                {
                    wheres += " and (select  top 1  c1.TestPaperID from tb_FH_RelationTopic c1 where c1.TopicId=a.Id and c1.TestPaperID=" + Id + ") is not null ";
                }
                else
                {
                    wheres += " and (select  top 1  c1.TestPaperID from tb_FH_RelationTopic c1 where c1.TopicId=a.Id and c1.TestPaperID=" + Id + ") is null ";
                }
            }
            if (!string.IsNullOrEmpty(Request["PaperIsUse"]) && Request["PaperIsUse"].ToString() != "0")//是否被使用 1 是、 0 否
            {
                if (Request["PaperIsUse"].ToString() == "1")
                {
                    wheres += " and (select  top 1  c1.TestPaperID from tb_FH_RelationTopic c1 where c1.TopicId=a.Id) is not null ";
                }
                else
                {
                    wheres += " and (select  top 1  c1.TestPaperID from tb_FH_RelationTopic c1 where c1.TopicId=a.Id) is null ";
                }
            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "a.Id desc"; //排序必须填写
            m.strFld = " a.*,";
            m.strFld += " (select top 1 c1.TestsFraction from tb_FH_RelationTopic c1  left join tb_FH_TestPaper f on (c1.TestPaperID=f.Id) where c1.TopicId=a.Id and f.IsDelete=1 and c1.TestPaperID=" + Id + ") as TestsFraction, ";
            m.strFld += " (select COUNT(1) from tb_FH_RelationTopic c1  left join tb_FH_TestPaper f on (c1.TestPaperID=f.Id) where c1.TopicId=a.Id and f.IsDelete=1 and c1.TestPaperID=" + Id + ") as State, ";
            m.strFld += "(select  COUNT(1) from tb_FH_RelationTopic c1   left join tb_FH_TestPaper f on (c1.TestPaperID=f.Id) where c1.TopicId=a.Id and f.IsDelete=1 )as PaperIsUse ";
            m.tab = "tb_FH_Topic  a ";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);

            var list = JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt); 
            return JsonConvert.SerializeObject(list);
        }

        public int AjaxAddTestPager(string entityList, string TestName, int IsOrder, int? Id)
        {
            var TestsList = new List<FH_TestsModels>();
            try
            {
                if (!string.IsNullOrEmpty(entityList))
                {
                    TestsList = JsonHelper.JSONToObject<List<FH_TestsModels>>(HttpUtility.UrlDecode(entityList));
                }
            }
            catch
            {
                return -1;
            }
            string table = "tb_FH_TestPaper";
            string Countstr = UserType == "2" ? " and AddOperator='" + UId + "'" : "";
            if (Id == null)//添加
            {
                if (!GetCount("tb_FH_TestPaper", " and IsDelete=1 and TestPaperName='" + TestName + "' and Kind=" + (UserType == "1" ? 1 : 2) + Countstr))
                {
                    string list = "TestPaperN0, TestPaperName, TestPaperType, IsType, PushType, Kind, Operator, AddOperator, AddTime,IsDelete";//列
                    string vlaue = "@TestPaperN0, @TestPaperName, @TestPaperType, @IsType, @PushType, @Kind, @Operator, @AddOperator, @AddTime, @IsDelete";
                    SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@TestPaperN0", DateTime.Now.ToString("yyyyMMddHHmmss")),
                        new SqlParameter("@TestPaperName",TestName),
                        new SqlParameter("@TestPaperType", 0),
                        new SqlParameter("@IsType", IsOrder),
                        new SqlParameter("@PushType", 0),
                        new SqlParameter("@Kind",UserType), 
                        new SqlParameter("@Operator",UId), 
                        new SqlParameter("@AddOperator",UId),
                        new SqlParameter("@AddTime",DateTime.Now), 
                        new SqlParameter("@IsDelete",1) 

                     };
                    var r = commonbll.AddIdentity(table, list, vlaue, pars);
                    if (Convert.ToInt32(r) > 0)
                    {
                        SqlTestPaper(TestsList, Convert.ToInt32(r));
                    }
                    else
                    {
                        return -3;
                    }
                }
                else
                {
                    return -2; //存在重复
                }

            }
            else
            {
                if (!GetCount("tb_FH_TestPaper", "  and IsDelete=1  and Id<>" + Id + " and TestPaperName='" + TestName + "' and Kind=" + (UserType == "1" ? 1 : 2) + Countstr))
                {
                    string set = @"TestPaperName=@TestPaperName,Operator=@Operator, UpdateTime=@UpdateTime";
                    string strWhere = " and Id=@Id";
                    SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@TestPaperName",TestName),
                        new SqlParameter("@Operator",UId), 
                        new SqlParameter("@UpdateTime",DateTime.Now),
                        new SqlParameter("@Id",Id)

                     };
                    var r = commonbll.UpdateInfo(table, set, strWhere, pars);
                    if (r != 99)
                    {
                        SqlTestPaper(TestsList, (int)Id);
                    }
                    else
                    {
                        return -3;
                    }
                }
                else
                {
                    return -2; //存在重复
                }
            }
            return 1;
        }
        /// <summary>
        /// 建立关系sql
        /// </summary>
        /// <param name="list"></param>
        /// <param name="Id"></param>
        public void SqlTestPaper(List<FH_TestsModels> list,int Id)
        {
            if (list != null && list.Count > 0)
            {
                var sql = new StringBuilder();
                foreach (var item in list)
                {
                    if (item.type == 0)//添加
                    {
                        sql.Append(" delete from tb_FH_RelationTopic where TopicId=" + item.Id + " and TestPaperID=" + Id + "; "); 
                        sql.Append(" insert into tb_FH_RelationTopic(TopicId,TestPaperID,TestsFraction) values ");
                        sql.Append(" (" + item.Id + "," + Id + "," + item.Score + ");");
                    }
                    else
                    {
                        sql.Append(" delete from tb_FH_RelationTopic where TopicId=" + item.Id + " and TestPaperID=" + Id + "; "); 
                    }
                }
                SqlHelper.ExecuteNonQuery(sql.ToString());
            }
        }
        /// <summary>
        /// 编辑保存、加入试卷、移除试卷
        /// </summary>
        /// <param name="entityList">集合</param>
        /// <param name="TestName">名称</param>
        /// <param name="IsOrder">是否打乱顺序</param>
        /// <param name="type">1 保存、2 移除</param>
        /// <param name="State">A 缓存、B 保存</param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int UpdateTestPager(string entityList, string TestName, int IsOrder, int type, string State, int? Id)
        {
            int count = -3;
            var TestsList = new List<FH_TestsModels>();
            try
            {
                if (!string.IsNullOrEmpty(entityList))
                {
                    TestsList = JsonHelper.JSONToObject<List<FH_TestsModels>>(HttpUtility.UrlDecode(entityList));
                }
            }
            catch
            {
                return -1;
            }
            if (type == 1)//1 保存、2 移除
            {
                string table = "tb_FH_TestPaper"; //表名
                tb_FH_TestPaper Topic = new tb_FH_TestPaper();
                Topic.TestPaperName = TestName;
                Topic.Operator = UId;
                Topic.IsType = IsOrder;
                if (State != "A")
                {
                    var r = update_TestPaper(Topic, table, (int)Id, 1);
                    if (r >= 0)
                    {
                        return Convert.ToInt32(Id);
                    }
                    else
                    {
                        return -3;//保存失败
                    }
                }



                string Countstr = UserType == "2" ? " and AddOperator='" + UId + "'" : "";
                if (Id == null)//新增
                {
                    Topic.IsDelete = State == "A" ? 0 : 1;
                    Topic.TestPaperN0 = DateTime.Now.ToString("yyyyMMddHHmmss");
                    Topic.AddOperator = UId;
                    Topic.TestPaperType = 0;
                    Topic.AddTime = DateTime.Now;
                    Topic.Kind = UserType == "1" ? 1 : 2;//归属 1系统 2教师
                    Topic.PushType = 0;    //推送类型 0否，1是。是否系统推送（推送不能修改）
                    if (!GetCount("tb_FH_TestPaper", " and IsDelete=1 and TestPaperName='" + TestName + "' and Kind=" + (UserType == "1" ? 1 : 2) + Countstr))
                    {
                        string list = "TestPaperN0, TestPaperName, TestPaperType, IsType, PushType, Kind, Operator, AddOperator, AddTime,IsDelete";//列
                        string vlaue = "@TestPaperN0, @TestPaperName, @TestPaperType, @IsType, @PushType, @Kind, @Operator, @AddOperator, @AddTime, @IsDelete";
                        SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@TestPaperN0",Topic.TestPaperN0),
                        new SqlParameter("@TestPaperName",Topic.TestPaperName),
                        new SqlParameter("@TestPaperType",Topic.TestPaperType),
                        new SqlParameter("@IsType",Topic.IsType),
                        new SqlParameter("@PushType",Topic.PushType),
                        new SqlParameter("@Kind",Topic.Kind),  
                        new SqlParameter("@Operator",Topic.Operator),
                        new SqlParameter("@AddOperator",Topic.AddOperator),
                        new SqlParameter("@AddTime",Topic.AddTime), 
                        new SqlParameter("@IsDelete",Topic.IsDelete) 

                     };
                        var r = commonbll.AddIdentity(table, list, vlaue, pars);
                        count = Convert.ToInt32(r);
                        if (Convert.ToInt32(r) > 0)
                        {
                            AddRelationTopiclist(TestsList, Convert.ToInt32(r), 0);
                        }
                        else
                        {
                            return -3;
                        }
                    }
                    else
                    {
                        return -2; //存在重复
                    }
                }
                else//修改
                {
                    if (!GetCount("tb_FH_TestPaper", "  and IsDelete=1  and Id<>" + Id + " and TestPaperName='" + TestName + "' and Kind=" + (UserType == "1" ? 1 : 2) + Countstr))
                    {
                        //    string set = @"TestPaperName=@TestPaperName,Operator=@Operator, UpdateTime=@UpdateTime, IsType=@IsType";
                        //    string strWhere = " and Id=@Id";
                        //    SqlParameter[] pars = new SqlParameter[] 
                        //{
                        //    new SqlParameter("@TestPaperName",Topic.TestPaperName),
                        //    new SqlParameter("@Operator",Topic.Operator), 
                        //    new SqlParameter("@UpdateTime",DateTime.Now),
                        //    new SqlParameter("@IsType",Topic.IsType),
                        //    new SqlParameter("@Id",Id)

                        // };

                        //    var r = commonbll.UpdateInfo(table, set, strWhere, pars);
                        var r = update_TestPaper(Topic, table, (int)Id);
                        count = (int)Id;
                        if (r >= 0)
                        {
                            AddRelationTopiclist(TestsList, (int)Id, 0);
                        }
                        else
                        {
                            return -3;
                        }
                    }
                    else
                    {
                        return -2; //存在重复
                    }
                }
            }
            else
            {
                AddRelationTopiclist(TestsList, (int)Id, 1);
                count = (int)Id;
            }

            return count;
        }

        public int update_TestPaper(tb_FH_TestPaper Topic, string table, int Id, int type = 0)
        {
            string set = @"TestPaperName=@TestPaperName,Operator=@Operator, UpdateTime=@UpdateTime, IsType=@IsType";
            if (type != 0)
            {
                set = @"TestPaperName=@TestPaperName,Operator=@Operator, UpdateTime=@UpdateTime,IsType=@IsType,IsDelete=@IsDelete";
            }
            string strWhere = " and Id=@Id";
            if (type != 0)
            {
                SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@TestPaperName",Topic.TestPaperName),
                        new SqlParameter("@Operator",Topic.Operator), 
                        new SqlParameter("@UpdateTime",DateTime.Now),
                        new SqlParameter("@IsType",Topic.IsType),
                        new SqlParameter("@IsDelete",1),
                        new SqlParameter("@Id",Id)

                     };
                var r = commonbll.UpdateInfo(table, set, strWhere, pars);
                return r;
            }
            else
            {

                SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@TestPaperName",Topic.TestPaperName),
                        new SqlParameter("@Operator",Topic.Operator), 
                        new SqlParameter("@UpdateTime",DateTime.Now),
                        new SqlParameter("@IsType",Topic.IsType),
                        new SqlParameter("@Id",Id)

                     };
                var r = commonbll.UpdateInfo(table, set, strWhere, pars);
                return r;
            }
        }

        /// <summary>
        /// 加入、移除  试题
        /// </summary>
        /// <param name="TestsList"></param>
        /// <param name="Id"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        public int AddRelationTopiclist(List<FH_TestsModels> TestsList, int Id,int T)
        {

            string sql = "";
            if (T == 0)//加入
            {
                if (TestsList.Count > 0)
                { 
                    foreach (var item in TestsList)
                    {
                        var wher = " and TopicId=" + item.Id + " and TestPaperID=" + Id;
                        var tb = commonbll.GetListDatatable("*", "tb_FH_RelationTopic", wher);
                        if (tb != null && tb.Rows.Count>0)
                        {
                            sql += " update tb_FH_RelationTopic set TestsFraction=" + item.Score + " where Id=" + tb.Rows[0]["Id"] + ";";
                        }
                        else
                        {
                            sql += " insert into tb_FH_RelationTopic(TopicId,TestPaperID,TestsFraction)";
                            sql += " values(" + item.Id + "," + Id + "," + item.Score + ");";
                        }
                    }
                }
            }
            else//移除
            {
                foreach (var item in TestsList)
                {
                    sql += " delete from tb_FH_RelationTopic  where TopicId=" + item.Id + " and TestPaperID=" + Id + " ;";
                }
            }
            SqlHelper.ExecuteNonQuery(sql.ToString());
            return 0;
        }
        /// <summary>
        /// 删除试卷
        /// </summary>
        /// <returns></returns>
        public string DelTestPaper()
        {
            try
            {
                var Ids = Request["Ids"];
                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@Operator",UId),
                    new SqlParameter("@IsDelete","0")
                };
                //修改状态 操作人
                commonbll.UpdateInfo("tb_FH_TestPaper", " IsDelete=@IsDelete,Operator=@Operator", " and Id in(" + Ids + ")", pars);

                return "1";
            }
            catch
            {
                return "99";
            }

        }

        #endregion 

         
        /// <summary>
        /// 退出后台
        /// </summary>
        public void PostUserExit()
        {
            Session.Abandon();
            Session.Clear();
            Response.Redirect("/login");
        }
    }
}
