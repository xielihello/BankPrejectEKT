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

namespace VocationalProject.Areas.Admin.Controllers
{
    public class ExaminationManagementController : BaseController
    {
        /***************************************************************
        FileName:考试管理
        Copyright（c）2017-金融教育在线技术开发部
        Author:邵世铨
        Create Date:2017-3-29
        ******************************************************************/
        //string UId = "5";//用户ID
        //string UserType = "2";//1管理员2教师3学生4裁判
        Manual_ExaminationBll Mebll = new Manual_ExaminationBll();
        CommonBll commBll = new CommonBll();
        string id = "";

        public ActionResult Examination()
        {
            //ViewData["UserType"] = UserType;
            //DataTable data = new DataTable();
            //if (Convert.ToInt32(UserType) == 1)//管理员
            //{
            //    data = commBll.GetListDatatable("*", "tb_Manual_Examination", "");
            //}
            //else if (Convert.ToInt32(UserType) == 2)//教师
            //{
            //    data = commBll.GetListDatatable("*", "tb_Manual_Examination", "and Kind=2");
            //}
          
            //return View(data);

            return View();
        }

        /// <summary>
        /// 试卷列表
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {
            string wheres = " ";//试卷状态

            if (UserType == "1")//管理员 所有管理员的
            {
                wheres += " and Kind=1";
            }
            if (UserType == "2")//教师  只看自己的设置的考试
            {
                wheres += " and AddOperator=" + UId;
            }


            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)//考试名称
            {

                wheres += " and ExaminationName like '%" + Request["E_Name"].ToString() + "%'";
            }

            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")//模式
            {
                wheres += " and Pattern='" + Request["E_Type"].ToString() + "'";
            }


            if (Request["E_IsState"] != null && Request["E_IsState"].ToString() != "0")//激活状态
            {
                wheres += " and ExaminationState='" + Request["E_IsState"].ToString() + "'";
            }


            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "AddTime desc"; //排序必须填写
            m.strFld = " * ";
            m.tab = "tb_Manual_Examination";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));

        }
        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <returns></returns>
        public string GetListById()
        {
            var iid = Request["EId"];
            DataTable dt = commBll.GetListDatatable("*", "tb_Manual_Examination", "and ID=" + iid + "");
            return JsonConvert.SerializeObject(dt);
        }
 
        /// <summary>
        /// 考试下的班级名称获取
        /// </summary>
        /// <returns></returns>
        public string GetListTeamByInId()
        {
            string wheres = " and Id in(" + Request["id"] + ")";
            DataTable dt = commBll.GetListDatatable("*", "tb_Team", wheres);

            return JsonConvert.SerializeObject(dt);
        }
        /// <summary>
        /// 查找
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Select(string EamxName, string PatternName, string StateName)
        {
            DataTable data = commBll.GetListDatatable("*", "tb_Manual_Examination", " and ExaminationName like '%" + EamxName + "%' or Pattern=" + PatternName + " or ExaminationState=" + StateName + "");
            return View("Examination", data);
        }
        /// <summary>
        /// 新增视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            ViewData["UserType"] = UserType;
            return View();
        }
        /// <summary>
        /// 查找任务
        /// </summary>
        /// <returns></returns>
        public StringBuilder SelectTask()
        {
            DataTable data = new DataTable();
            string wheres = "";
            if (UserType == "1")//管理员 所有管理员的
            {
                wheres += " and Kind=1";
                data = commBll.GetListDatatable("*", "tb_Manual_Counting", wheres);
            }
            if (UserType == "2")//教师  只看自己的设置的考试
            {
                wheres += " and (AddOperator=" + UId + " or Id in(select P.UR_CompetitionId from tb_UserInfo U left join  tb_PaperUserRights P on P.UR_SchoolId=U.UserSchoolId where P.UR_CompetitionType=2 and U.UId=" + UId + "))";
                data = commBll.GetListDatatable("*", "tb_Manual_Counting", wheres);
            }
           

            StringBuilder SB = new StringBuilder();
            if (data.Rows.Count > 0)
            {
                SB.Append(" <option value=\"0\">请选择试卷</option>");
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    SB.Append("<option value=" + data.Rows[i]["ID"] + ">" + data.Rows[i]["TaskName"] + "</option>");
                }
            }

            return SB;
        }
        /// <summary>
        /// 获取教师的班级
        /// </summary>
        /// <returns></returns>
        public StringBuilder GetListTeam()
        {
            string wheres = " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            DataTable Dt = commBll.GetListDatatable("*", "tb_Team", wheres);
            StringBuilder SB = new StringBuilder();
            if (Dt.Rows.Count > 0)
            {
                //SB.Append(" <option value=\"0\">请选择班级</option>");
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    SB.Append("<option value=" + Dt.Rows[i]["ID"] + ">" + Dt.Rows[i]["TeamName"] + "</option>");
                }
            }
            return SB;
        }
        /// <summary>
        /// 获取教师的班级
        /// </summary>
        /// <returns></returns>
        public string EditGetListTeam()
        {
            string wheres = " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            DataTable dt = commBll.GetListDatatable("*", "tb_Team", wheres);

            return JsonConvert.SerializeObject(dt);
        }
        /// <summary>
        /// 新增保存
        /// </summary>
        /// <returns></returns>
        public int AddSave()
        {
            try
            {
                string Taskid = Request["Taskid"];
                string EamxName = Request["EamxName"];
                string StartTime = Request["StartTime"];
                string EndTime = Request["EndTime"];
                string Pattern = Request["Pattern"];
                string min = Request["minId"];
                string Plus = Request["Plus"];
                string Classid = Request["Classid"];

                return Mebll.AddSave(Taskid, EamxName, StartTime, EndTime, Pattern, min, Plus, UserType, UId, Classid);
            }
            catch
            {
                return 99;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id">任务id</param>
        /// <returns></returns>
        public int Delete(string Id)
        {
            return commBll.DeleteInfo("tb_Manual_Examination", " and Id in(" + Id + ") and ExaminationState=2");
        }
        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="Id">考试ID</param>
        /// <returns></returns>
        public int Activation(string Id)
        {
            return Mebll.Activation(Id);
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="id">任务</param>
        /// <returns></returns>
        public int Close(string id)
        {
            return Mebll.Close(id);
        }
        /// <summary>
        /// 编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            ViewBag.UserType = UserType;
            //string wheres = " and P_State=1";//试卷状态
            string wheres = "";
            DataTable dt = new DataTable();
            if (UserType == "1")//管理员 所有管理员的
            {
                wheres += " and Kind=1";
                dt = commBll.GetListDatatable("*", "tb_Manual_Counting", wheres);
            }
            if (UserType == "2")//教师  只看自己的设置的考试
            {
                wheres += " and (AddOperator=" + UId + " or Id in(select P.UR_CompetitionId from tb_UserInfo U left join  tb_PaperUserRights P on P.UR_SchoolId=U.UserSchoolId where P.UR_CompetitionType=2 and U.UId=" + UId + "))";
                dt = commBll.GetListDatatable("*", "tb_Manual_Counting", wheres);
            }
           
            return View(dt);
        }
        /// <summary>
        /// 获取修改的数据
        /// </summary>
        /// <returns></returns>
        public void GetEdit()
        {
            DataTable Dt = commBll.GetListDatatable("*", "tb_Manual_Examination", "");
        }
        /// <summary>
        /// 修改提交功能
        /// </summary>
        public int EditSave() 
        {
            try
            {
                string E_Name = Request["EditP_Name"];//试卷名称
                string E_PId = Request["EditE_PId"];
                string E_Type = Request["EditE_Type"];//模式
                string E_StartTime = Request["EditE_StartTime"];
                string E_EndTime = Request["EditE_EndTime"];
                string E_Whenlong = Request["EditE_Whenlong"];//时长
                string E_TeamId = Request["EditTeamId"];//班级
                string E_IsTimeBonus = Request["EditE_IsTimeBonus"];//是否时间加分
                string EId = Request["EId"];
                string E_Operator = UId;


                //校验考试名称是否存在
                string wheres = " and ExaminationName='" + E_Name + "' and ID!=" + EId;//试卷状态

                if (UserType == "1")//管理员 所有管理员的
                {
                    wheres += " and Taskid=1";
                }
                if (UserType == "2")//教师  只看自己的设置的考试
                {
                    wheres += " and AddOperator=" + UId;
                }

                var checkcount = commBll.GetRecordCount("tb_Manual_Examination", wheres);
                if (checkcount > 0)
                {
                    return 88;
                }

                string table = "tb_Manual_Examination"; //表名
                string list = @" ExaminationName=@E_Name, Taskid=@E_PId, Pattern=@E_Type, StartTime=@E_StartTime, 
                                      EndTime=@E_EndTime, LongTime=@E_Whenlong, Class=@E_TeamId, 
                                      TimeBonus=@E_IsTimeBonus, Operator=@E_Operator";//列



                SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@E_Name",E_Name),
                        new SqlParameter("@E_PId",E_PId),
                        new SqlParameter("@E_Type",E_Type),
                        new SqlParameter("@E_StartTime",E_StartTime),
                        new SqlParameter("@E_EndTime",E_EndTime),
                        new SqlParameter("@E_Whenlong",E_Whenlong),
                        new SqlParameter("@E_TeamId",E_TeamId),
                        new SqlParameter("@E_IsTimeBonus",E_IsTimeBonus),
                        new SqlParameter("@E_Operator",E_Operator),
                        new SqlParameter("@EId",EId)
                    };
                commBll.UpdateInfo(table, list, " and ID=@EId", pars);

                return 1;
            }
            catch
            {
                return 99;
            }

        }

    }
}
