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


namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
    FileName:货币知识 考试管理
    Copyright（c）2017-金融教育在线技术开发部
    Author:袁学
    Create Date:2017-4-8
    ******************************************************************/
    public class HB_ExaminationController : BaseController
    {
        //
        // GET: /Admin/HB_Examination/

        //string UId = "2";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        //string UserNo = "admin";//登录账号
        //string TeacherSchoolId = "1";//教师属于的院校

        CommonBll commonbll = new CommonBll();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 视图 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult AddIndex()
        {
            ViewData["UserType"] = UserType;

            string wheres = " and P_State=1";//试卷状态
            if (UserType == "1")//管理员 所有管理员的
            {
                wheres += " and P_Kind=1";
            }
            if (UserType == "2")//教师  如果是教师就只显示自己的试卷和推送过的哦
            {
                wheres += " and (P_AddOperator=" + UId + @" or PId in (select UR_CompetitionId from tb_PaperUserRights 
                where UR_CompetitionType=1 and UR_SchoolId=" + TeacherSchoolId + "))";
            }
            DataTable dt = commonbll.GetListDatatable("*", "tb_HB_Paper", wheres);
            return View(dt);
        }


        /// <summary>
        /// 视图 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult EditIndex()
        {
            ViewData["UserType"] = UserType;

            string wheres = " and P_State=1";//试卷状态
            if (UserType == "1")//管理员 所有管理员的
            {
                wheres += " and P_Kind=1";
            }
            if (UserType == "2")//教师  如果是教师就只显示自己的试卷和推送过的哦
            {
                wheres += " and (P_AddOperator=" + UId + @" or PId in (select UR_CompetitionId from tb_PaperUserRights 
                where UR_CompetitionType=1 and UR_SchoolId=" + TeacherSchoolId + "))";
            }
            DataTable dt = commonbll.GetListDatatable("*", "tb_HB_Paper", wheres);
            return View(dt);
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
                wheres += " and E_Kind=1";
            }
            if (UserType == "2")//教师  只看自己的设置的考试
            {
                wheres += " and E_AddOperator=" + UId;
            }


            if (Request["E_Name"] != null && Request["E_Name"].ToString().Length > 0)//考试名称
            {

                wheres += " and E_Name like '%" + Request["E_Name"].ToString() + "%'";
            }

            if (Request["E_Type"] != null && Request["E_Type"].ToString() != "0")//模式
            {
                wheres += " and E_Type='" + Request["E_Type"].ToString() + "'";
            }


            if (Request["E_IsState"] != null && Request["E_IsState"].ToString() != "0")//激活状态
            {
                wheres += " and E_IsState='" + Request["E_IsState"].ToString() + "'";
            }


            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "E_AddTime desc"; //排序必须填写
            m.strFld = " * ";
            m.tab = "tb_HB_Examination";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));

        }


        /// <summary>
        /// 激活状态修改
        /// </summary>
        /// <returns></returns>
        public string EditIsState()
        {
            try
            {
                var Ids = Request["Ids"];
                var E_IsState = Request["E_IsState"];
                //需要检验
                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@E_IsState",E_IsState)
                   
                };

                //修改状态 操作人
                commonbll.UpdateInfo("tb_HB_Examination", " E_IsState=@E_IsState", " and EId in(" + Ids + ")", pars);
                return "1";
            }
            catch
            {
                return "99";
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string DelExamination()
        {
            try
            {
                var Ids = Request["Ids"];

                commonbll.DeleteInfo("tb_HB_Examination", " and E_IsState=2 and EId in (" + Ids + ")");
                return "1";
            }
            catch
            {
                return "99";
            }
        }


        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <returns></returns>
        public string GetListById()
        {
            DataTable dt = commonbll.GetListDatatable("*,(select P_Name from tb_HB_Paper where Pid=E_PId) as PName,(select count(1) from tb_HB_Paper where Pid=E_PId and P_State=1) as num", "tb_HB_Examination", " and EId=" + Request["EId"]);

            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 获取管理员端所有试卷
        /// </summary>
        /// <returns></returns>
        public string GetListPaper()
        {

            string wheres = " and P_State=1";//试卷状态
            if (UserType == "1")//管理员 所有管理员的
            {
                wheres += " and P_Kind=1";
            }
            if (UserType == "2")//教师  只看自己的设置的考试
            {
                wheres += " and P_AddOperator=" + UId;
            }
            DataTable dt = commonbll.GetListDatatable("*", "tb_HB_Paper", wheres);

            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 获取教师的班级
        /// </summary>
        /// <returns></returns>
        public string GetListTeam()
        {
            string wheres = " and (TeacherId like '" + UId + ",%' or  TeacherId like '%," + UId + ",%' or TeacherId like '%," + UId + "' or TeacherId like '" + UId + "')";
            DataTable dt = commonbll.GetListDatatable("*", "tb_Team", wheres);

            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 考试下的班级名称获取
        /// </summary>
        /// <returns></returns>
        public string GetListTeamByInId()
        {
            string wheres = " and Id in("+Request["Id"]+")";
            DataTable dt = commonbll.GetListDatatable("*", "tb_Team", wheres);
            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public string Add()
        {
            try
            {
                string E_Name = Request["AddP_Name"];//试卷名称
                string E_PId = Request["AddE_PId"];
                string E_Type = Request["AddE_Type"];//模式
                string E_StartTime = Request["AddE_StartTime"];
                string E_EndTime = Request["AddE_EndTime"];
                string E_Whenlong = Request["AddE_Whenlong"];//时长
                string E_TeamId = Request["AddE_TeamId"];//班级
                string E_IsTimeBonus = Request["AddE_IsTimeBonus"];//是否时间加分
                string E_IsState = "2";//未激活
                string E_Kind = UserType;//1系统 2是教师 正好与角色相对应
                string E_Operator = UId;//操作
                string E_AddOperator = UId;//创建人
                DateTime E_AddTime = DateTime.Now;
                string UR_Custom2 = UserNo;//登录账号


                //校验考试名称是否存在
                string wheres = " and E_Name='" + E_Name + "'";//试卷状态

                if (UserType == "1")//管理员 所有管理员的
                {
                    wheres += " and E_Kind=1";
                }
                if (UserType == "2")//教师  只看自己的设置的考试
                {
                    wheres += " and E_AddOperator=" + UId;
                }

                var checkcount = commonbll.GetRecordCount("tb_HB_Examination", wheres);
                if (checkcount > 0)
                {
                    return "88";
                }

                string table = "tb_HB_Examination"; //表名
                string list = "E_Name, E_PId, E_Type, E_StartTime, E_EndTime, E_Whenlong, E_TeamId, E_IsTimeBonus, E_IsState, E_Kind, E_Operator, E_AddOperator, E_AddTime, UR_Custom2";//列
                string vlaue = "@E_Name, @E_PId, @E_Type, @E_StartTime, @E_EndTime, @E_Whenlong, @E_TeamId, @E_IsTimeBonus, @E_IsState, @E_Kind, @E_Operator, @E_AddOperator, @E_AddTime, @UR_Custom2";


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
                        new SqlParameter("@E_IsState",E_IsState),
                        new SqlParameter("@E_Kind",E_Kind),
                        new SqlParameter("@E_Operator",E_Operator),
                        new SqlParameter("@E_AddOperator",E_AddOperator),
              
                        new SqlParameter("@E_AddTime",E_AddTime),
                        new SqlParameter("@UR_Custom2",UR_Custom2)
                    };
                commonbll.Add(table, list, vlaue, pars);

                return "1";
            }
            catch
            {
                return "99";
            }

        }


        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public string Edit() 
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
                string wheres = " and E_Name='" + E_Name + "' and EId!="+EId;//试卷状态

                if (UserType == "1")//管理员 所有管理员的
                {
                    wheres += " and E_Kind=1";
                }
                if (UserType == "2")//教师  只看自己的设置的考试
                {
                    wheres += " and E_AddOperator=" + UId;
                }

                var checkcount = commonbll.GetRecordCount("tb_HB_Examination", wheres);
                if (checkcount > 0)
                {
                    return "88";
                }

                string table = "tb_HB_Examination"; //表名
                string list = @" E_Name=@E_Name, E_PId=@E_PId, E_Type=@E_Type, E_StartTime=@E_StartTime, 
                                      E_EndTime=@E_EndTime, E_Whenlong=@E_Whenlong, E_TeamId=@E_TeamId, 
                                      E_IsTimeBonus=@E_IsTimeBonus, E_Operator=@E_Operator";//列



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
                commonbll.UpdateInfo(table, list, " and EId=@EId", pars);

                return "1";
            }
            catch
            {
                return "99";
            }
        
        }

    }
}
