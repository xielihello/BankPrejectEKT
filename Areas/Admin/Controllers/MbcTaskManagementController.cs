using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VocationalProject.Areas.Admin.Models;
using VocationalProject.Controllers;
using VocationalProject_Bll;
using VocationalProject_Dal;

namespace VocationalProject.Areas.Admin.Controllers
{
    public class MbcTaskManagementController : BaseController
    {
        /***************************************************************
         FileName:手工点钞-任务管理
         Copyright（c）2017-金融教育在线技术开发部
         Author:邵世铨
         Create Date:2017-3-31
        ******************************************************************/
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        Manual_CountingBll MCbll = new Manual_CountingBll();
        CommonBll commBll = new CommonBll();
        int id = 0;

        public ActionResult MbcTaskManagement()
        {
            ViewData["State"] = "NoAddandEdit";
            ViewData["id"] = 0;
            ViewData["UserType"] = UserType;
            return View();
        }

        /// <summary>
        /// 试卷列表
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {

            string wheres = "";//试卷状态

            //角色区分  //暂时不考虑独立打包客户，需要web加个配置 读取 控制 后期加
            if (UserType == "2")//教师  如果是教师就只显示自己的试卷和推送过的哦
            {
                wheres += " and (M.AddOperator=" + UId + @" or ID in (select UR_CompetitionId from tb_PaperUserRights 
                where UR_CompetitionType=2 and UR_SchoolId=" + TeacherSchoolId + "))";
            }


            if (Request["P_Name"] != null && Request["P_Name"].ToString().Length > 0)//试卷名称
            {

                wheres += " and TaskName like '%" + Request["P_Name"].ToString() + "%'";
            }

            if (Request["P_Kind"] != null && Request["P_Kind"].ToString() != "0")//试卷属性
            {
                if (Request["P_Kind"].ToString() == "1")
                {
                    wheres += " and Kind='" + Request["P_Kind"].ToString() + "'";
                }
                if (Request["P_Kind"].ToString() == "2")
                {
                    wheres += " and Kind='" + Request["P_Kind"].ToString() + "'";
                }
                if (Request["P_Kind"].ToString() == "3")
                {

                }

            }
            if (Request["P_Kind"] == null || Request["P_Kind"].ToString() == "0")//试卷属性
            {
                if (UserType == "1")
                {
                    wheres += " and Kind=1";
                }
                else if (UserType == "2")
                {
                    wheres += " and Kind=2";
                }
            }
            if (Request["P_Custom2"] != null && Request["P_Custom2"].ToString().Length > 0)//试卷来源
            {
                wheres += " and (UserName like '%" + Request["P_Custom2"].ToString() + "%' or UserNo like '%" + Request["P_Custom2"].ToString() + "%')";


            }

            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "M.AddTime desc"; //排序必须填写
            m.strFld = " M.*,U.UserNo,U.UserName,UserType ";
            m.tab = "tb_Manual_Counting M left join tb_UserInfo U on U.UId=M.AddOperator ";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));

        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public string Add()
        {
            string TaskName = Request.Form["TaskName"];
            string TaskScore = Request.Form["TaskScore"];
            //百元面额
            string MoneyName100 = Request.Form["MoneyName100"];
            string MoneyNumber100 = Request.Form["MoneyNumber100"];
            string MoneyTol100 = Request.Form["MoneyTol100"];
            //五十面额
            string MoneyName50 = Request.Form["MoneyName50"];
            string MoneyNumber50 = Request.Form["MoneyNumber50"];
            string MoneyTol50 = Request.Form["MoneyTol50"];
            //二十面额
            string MoneyName20 = Request.Form["MoneyName20"];
            string MoneyNumber20 = Request.Form["MoneyNumber20"];
            string MoneyTol20 = Request.Form["MoneyTol20"];
            //十元面额
            string MoneyName10 = Request.Form["MoneyName10"];
            string MoneyNumbe10 = Request.Form["MoneyNumber10"];
            string MoneyTol10 = Request.Form["MoneyTol10"];
            //五元面额
            string MoneyName5 = Request.Form["MoneyName5"];
            string MoneyNumbe5 = Request.Form["MoneyNumber5"];
            string MoneyTol5 = Request.Form["MoneyTol5"];
            //二元面额
            string MoneyName2 = Request.Form["MoneyName2"];
            string MoneyNumbe2 = Request.Form["MoneyNumber2"];
            string MoneyTol2 = Request.Form["MoneyTol2"];
            //一元面额
            string MoneyName11 = Request.Form["MoneyName1"];
            string MoneyNumbe11 = Request.Form["MoneyNumber1"];
            string MoneyTol11 = Request.Form["MoneyTol1"];
            //五角面额
            string MoneyName_5 = Request.Form["MoneyName15"];
            string MoneyNumbe_5 = Request.Form["MoneyNumber15"];
            string MoneyTol1_5 = Request.Form["MoneyTol15"];

            id = MCbll.AddTask(UId, TaskName, TaskScore, UserType, TeacherSchoolId);

            if (id == -1)
            {
                //任务名称已经存在
                ViewData["id"] = id.ToString();
                return "-1";
            }
            else
            {
                //任务存在时允许添加明细
                if (MoneyName100 != null)
                {
                    MCbll.AddTaskDetailed(UId, id, MoneyName100, MoneyNumber100, MoneyTol100);
                }
                if (MoneyName50 != null)
                {
                    MCbll.AddTaskDetailed(UId, id, MoneyName50, MoneyNumber50, MoneyTol50);
                }
                if (MoneyName20 != null)
                {
                    MCbll.AddTaskDetailed(UId, id, MoneyName20, MoneyNumber20, MoneyTol20);
                }
                if (MoneyName10 != null)
                {
                    MCbll.AddTaskDetailed(UId, id, MoneyName10, MoneyNumbe10, MoneyTol10);
                }
                if (MoneyName5 != null)
                {
                    MCbll.AddTaskDetailed(UId, id, MoneyName5, MoneyNumbe5, MoneyTol5);
                }
                if (MoneyName2 != null)
                {
                    MCbll.AddTaskDetailed(UId, id, MoneyName2, MoneyNumbe2, MoneyTol2);
                }
                if (MoneyName11 != null)
                {
                    MCbll.AddTaskDetailed(UId, id, MoneyName11, MoneyNumbe11, MoneyTol11);
                }
                if (MoneyName_5 != null)
                {
                    MCbll.AddTaskDetailed(UId, id, MoneyName_5, MoneyNumbe_5, MoneyTol1_5);
                }

            }
            DataTable Data = commBll.GetListDatatable(" M.*,U.UserNo", "tb_Manual_Counting M left join tb_UserInfo U on U.UId=M.AddOperator", "");
            if (Data.Rows.Count > 0)
            {
                if (Convert.ToInt32(Data.Rows[0]["Kind"]) == 1)
                {
                    ViewData["Kind"] = "系统任务";
                }
                else if (Convert.ToInt32(Data.Rows[0]["Kind"]) == 2)
                {
                    ViewData["Kind"] = "教师任务";
                }
            }
            return "1";
        }

        /// <summary>
        /// 查看任务详情
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns></returns>
        public ActionResult ShowSelect(int id)
        {
            DataTable Dt = commBll.GetListDatatable("*", "tb_Manual_Counting", " and id=" + id + "");
            if (Dt.Rows.Count > 0)
            {
                ViewData["TaskName"] = Dt.Rows[0]["TaskName"].ToString();
                ViewData["Score"] = Dt.Rows[0]["Score"].ToString();
            }

            DataTable data = commBll.GetListDatatable("*", "tb_Manual_CountingDetailed", " and TaskID=" + id + "");
            return View(data);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns></returns>
        public StringBuilder Edit(int id)
        {
            StringBuilder Sb = new StringBuilder();
            DataTable Dt = commBll.GetListDatatable("*", "tb_Manual_Counting", " and id=" + id + "");
            string TaskName = "";
            string Score = "";
            string Taskid = "";
            if (Dt.Rows.Count > 0)
            {
                Taskid = Dt.Rows[0]["ID"].ToString();
                TaskName = Dt.Rows[0]["TaskName"].ToString();
                Score = Dt.Rows[0]["Score"].ToString();
                Sb.Append("<tr style=\"display:none\"><TD style=\"display:none\"><input Name=\"TankName\" id=\"TankName\" hidden=\"hidden\" value=" + TaskName + " /><input Name=\"TankScore\" id=\"TankScore\" hidden=\"hidden\" value=" + Score + " /><input Name=\"EditTankId\" id=\"TankId\" hidden=\"hidden\" value=" + Taskid + " /></td></tr>");
            }

            DataTable data = commBll.GetListDatatable("*", "tb_Manual_CountingDetailed", " and TaskID=" + id + "");
            if (data.Rows.Count > 0)
            {

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    string Money = data.Rows[i]["TaskValue"].ToString();
                    string Name = "";
                    string Name2 = "";
                    string Name3 = "";
                    string MoneyValues = "";
                    if (Money == "壹佰元")
                    {
                        Name = "MoneyName100";
                        Name2 = "MoneyNumber100";
                        Name3 = "MoneyTol100";
                        MoneyValues = "1";
                        Sb.Append("<tr id=100>");
                        Sb.Append("<td><input id=\"subcheck\" type=\"checkbox\" value=" + MoneyValues + " class=\"i-checks\" name=\"authority\">");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name + " value=" + Money + " readonly=\"readonly\" /></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name2 + " value=" + data.Rows[i]["Number"].ToString() + " readonly=\"readonly\"/></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name3 + " value=" + data.Rows[i]["Value"].ToString() + " readonly=\"readonly\"/></td></tr>");
                    }
                    if (Money == "伍拾元")
                    {
                        Name = "MoneyName50";
                        Name2 = "MoneyNumber50";
                        Name3 = "MoneyTol50";
                        MoneyValues = "1";
                        Sb.Append("<tr id=50>");
                        Sb.Append("<td><input id=\"subcheck\" type=\"checkbox\" value=" + MoneyValues + " class=\"i-checks\" name=\"authority\">");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name + " value=" + Money + " readonly=\"readonly\" /></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name2 + " value=" + data.Rows[i]["Number"].ToString() + " readonly=\"readonly\"/></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name3 + " value=" + data.Rows[i]["Value"].ToString() + " readonly=\"readonly\"/></td></tr>");
                    }
                    if (Money == "贰拾元")
                    {
                        Name = "MoneyName20";
                        Name2 = "MoneyNumber20";
                        Name3 = "MoneyTol20";
                        MoneyValues = "1";
                        Sb.Append("<tr id=20>");
                        Sb.Append("<td><input id=\"subcheck\" type=\"checkbox\" value=" + MoneyValues + " class=\"i-checks\" name=\"authority\">");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name + " value=" + Money + " readonly=\"readonly\" /></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name2 + " value=" + data.Rows[i]["Number"].ToString() + " readonly=\"readonly\"/></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name3 + " value=" + data.Rows[i]["Value"].ToString() + " readonly=\"readonly\"/></td></tr>");
                    }
                    if (Money == "拾元")
                    {
                        Name = "MoneyName10";
                        Name2 = "MoneyNumber10";
                        Name3 = "MoneyTol10";
                        MoneyValues = "1";
                        Sb.Append("<tr id=10>");
                        Sb.Append("<td><input id=\"subcheck\" type=\"checkbox\" value=" + MoneyValues + " class=\"i-checks\" name=\"authority\">");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name + " value=" + Money + " readonly=\"readonly\" /></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name2 + " value=" + data.Rows[i]["Number"].ToString() + " readonly=\"readonly\"/></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name3 + " value=" + data.Rows[i]["Value"].ToString() + " readonly=\"readonly\"/></td></tr>");
                    }
                    if (Money == "伍元")
                    {
                        Name = "MoneyName5";
                        Name2 = "MoneyNumber5";
                        Name3 = "MoneyTol5";
                        MoneyValues = "1";
                        Sb.Append("<tr id=5>");
                        Sb.Append("<td><input id=\"subcheck\" type=\"checkbox\" value=" + MoneyValues + " class=\"i-checks\" name=\"authority\">");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name + " value=" + Money + " readonly=\"readonly\" /></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name2 + " value=" + data.Rows[i]["Number"].ToString() + " readonly=\"readonly\"/></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name3 + " value=" + data.Rows[i]["Value"].ToString() + " readonly=\"readonly\"/></td></tr>");
                    }
                    if (Money == "贰元")
                    {
                        Name = "MoneyName2";
                        Name2 = "MoneyNumber2";
                        Name3 = "MoneyTol2";
                        MoneyValues = "1";
                        Sb.Append("<tr id=2>");
                        Sb.Append("<td><input id=\"subcheck\" type=\"checkbox\" value=" + MoneyValues + " class=\"i-checks\" name=\"authority\">");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name + " value=" + Money + " readonly=\"readonly\" /></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name2 + " value=" + data.Rows[i]["Number"].ToString() + " readonly=\"readonly\"/></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name3 + " value=" + data.Rows[i]["Value"].ToString() + " readonly=\"readonly\"/></td></tr>");
                    }
                    if (Money == "壹元")
                    {
                        Name = "MoneyName1";
                        Name2 = "MoneyNumber1";
                        Name3 = "MoneyTol1";
                        MoneyValues = "1";
                        Sb.Append("<tr id=11>");
                        Sb.Append("<td><input id=\"subcheck\" type=\"checkbox\" value=" + MoneyValues + " class=\"i-checks\" name=\"authority\">");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name + " value=" + Money + " readonly=\"readonly\" /></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name2 + " value=" + data.Rows[i]["Number"].ToString() + " readonly=\"readonly\"/></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name3 + " value=" + data.Rows[i]["Value"].ToString() + " readonly=\"readonly\"/></td></tr>");
                    }
                    if (Money == "伍角")
                    {
                        Name = "MoneyName0.5";
                        Name2 = "MoneyNumber0.5";
                        Name3 = "MoneyTol0.5";
                        MoneyValues = "1";
                        Sb.Append("<tr id=1>");
                        Sb.Append("<td><input id=\"subcheck\" type=\"checkbox\" value=" + MoneyValues + " class=\"i-checks\" name=\"authority\">");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name + " value=" + Money + " readonly=\"readonly\" /></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name2 + " value=" + data.Rows[i]["Number"].ToString() + " readonly=\"readonly\"/></td>");
                        Sb.Append("<td><input type=\"text\" style=\"border:none\" name=" + Name3 + " value=" + data.Rows[i]["Value"].ToString() + " readonly=\"readonly\"/></td></tr>");
                    }
                }
            }
            return Sb;
        }

        public string SaveEdit()
        {
            string TaskName = Request.Form["EditTaskName"];
            string TaskScore = Request.Form["EditTaskScore"];
            string TaskId = Request.Form["EditTankId"];
            //百元面额
            string MoneyName100 = Request.Form["MoneyName100"];
            string MoneyNumber100 = Request.Form["MoneyNumber100"];
            string MoneyTol100 = Request.Form["MoneyTol100"];
            //五十面额
            string MoneyName50 = Request.Form["MoneyName50"];
            string MoneyNumber50 = Request.Form["MoneyNumber50"];
            string MoneyTol50 = Request.Form["MoneyTol50"];
            //二十面额
            string MoneyName20 = Request.Form["MoneyName20"];
            string MoneyNumber20 = Request.Form["MoneyNumber20"];
            string MoneyTol20 = Request.Form["MoneyTol20"];
            //十元面额
            string MoneyName10 = Request.Form["MoneyName10"];
            string MoneyNumbe10 = Request.Form["MoneyNumber10"];
            string MoneyTol10 = Request.Form["MoneyTol10"];
            //五元面额
            string MoneyName5 = Request.Form["MoneyName5"];
            string MoneyNumbe5 = Request.Form["MoneyNumber5"];
            string MoneyTol5 = Request.Form["MoneyTol5"];
            //二元面额
            string MoneyName2 = Request.Form["MoneyName2"];
            string MoneyNumbe2 = Request.Form["MoneyNumber2"];
            string MoneyTol2 = Request.Form["MoneyTol2"];
            //一元面额
            string MoneyName11 = Request.Form["MoneyName1"];
            string MoneyNumbe11 = Request.Form["MoneyNumber1"];
            string MoneyTol11 = Request.Form["MoneyTol1"];
            //五角面额
            string MoneyName_5 = Request.Form["MoneyName15"];
            string MoneyNumbe_5 = Request.Form["MoneyNumber15"];
            string MoneyTol1_5 = Request.Form["MoneyTol15"];

            int State = MCbll.EditTask(UId, TaskId, TaskName, TaskScore);
            ViewData["EditState"] = State;
            //删除原有任务名称
            MCbll.DeleteTask(TaskId);
            //新增任务明细
            //任务存在时允许添加明细

            if (MoneyName100 != null)
            {
                MCbll.AddTaskDetailed(UId, Convert.ToInt32(TaskId), MoneyName100, MoneyNumber100, MoneyTol100);
            }
            if (MoneyName50 != null)
            {
                MCbll.AddTaskDetailed(UId, Convert.ToInt32(TaskId), MoneyName50, MoneyNumber50, MoneyTol50);
            }
            if (MoneyName20 != null)
            {
                MCbll.AddTaskDetailed(UId, Convert.ToInt32(TaskId), MoneyName20, MoneyNumber20, MoneyTol20);
            }
            if (MoneyName10 != null)
            {
                MCbll.AddTaskDetailed(UId, Convert.ToInt32(TaskId), MoneyName10, MoneyNumbe10, MoneyTol10);
            }
            if (MoneyName5 != null)
            {
                MCbll.AddTaskDetailed(UId, Convert.ToInt32(TaskId), MoneyName5, MoneyNumbe5, MoneyTol5);
            }
            if (MoneyName2 != null)
            {
                MCbll.AddTaskDetailed(UId, Convert.ToInt32(TaskId), MoneyName2, MoneyNumbe2, MoneyTol2);
            }
            if (MoneyName11 != null)
            {
                MCbll.AddTaskDetailed(UId, Convert.ToInt32(TaskId), MoneyName11, MoneyNumbe11, MoneyTol11);
            }
            if (MoneyName_5 != null)
            {
                MCbll.AddTaskDetailed(UId, Convert.ToInt32(TaskId), MoneyName_5, MoneyNumbe_5, MoneyTol1_5);
            }
            DataTable Data = commBll.GetListDatatable(" M.*,U.UserNo", "tb_Manual_Counting M left join tb_UserInfo U on U.UId=M.AddOperator", "");
            if (Data.Rows.Count > 0)
            {
                if (Convert.ToInt32(Data.Rows[0]["Kind"]) == 1)
                {
                    ViewData["Kind"] = "系统任务";
                }
                else if (Convert.ToInt32(Data.Rows[0]["Kind"]) == 2)
                {
                    ViewData["Kind"] = "教师任务";
                }
            }
            return "1";
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id">任务ID</param>
        /// <returns></returns>
        public int Dele(string Id)
        {
            commBll.DeleteInfo("tb_Manual_CountingDetailed", " and TaskID in(" + Id + ")");
            return commBll.DeleteInfo("tb_Manual_Counting", " and Id in(" + Id + ")");
        }

        public ActionResult Select(string TeacherName, string TeamId, string TeacherSource)
        {
            DataTable data = commBll.GetListDatatable("m.*,u.UserNo", "tb_Manual_Counting m left join tb_UserInfo u on u.UId=m.AddOperator", " and m.TaskName like '%" + TeacherName + "%'and m.Kind=" + TeamId + " and u.UserNo='" + TeacherSource + "'");
            return View("MbcTaskManagement", data);
        }
    }
}
