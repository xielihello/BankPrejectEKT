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
   FileName:通用 权限明细推送 试卷
   Copyright（c）2017-金融教育在线技术开发部
   Author:袁学
   Create Date:2017-4-7
   ******************************************************************/
    public class AuthorizationDetailsPushController : BaseController
    {
        //
        // GET: /Admin/AuthorizationDetailsPush/
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        CommonBll commonbll = new CommonBll();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 数据列表
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {
            //赛项和类型
            string wheres = " and UR_CompetitionId='" + Request["CompetitionId"] + "' and UR_CompetitionType=" + Request["CompetitionType"];
            //且院校 当前有这个赛项的权限
            wheres += " and charindex('" + Request["CompetitionType"] + ",',Jurisdiction)>0 ";
            if (Request["SchoolName"] != null && Request["SchoolName"].ToString().Length > 0)
            {
                wheres += " and Name like '%" + Request["SchoolName"] + "%'";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "URId"; //排序必须填写
            m.strFld = "a.*,Name ";
            m.tab = "tb_PaperUserRights a inner join tb_School b on a.UR_SchoolId=b.Id";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));

        }

        /// <summary>
        /// 弹框列表数据读取
        /// </summary>
        /// <returns></returns>
        public string GetList_To()
        {

            //赛项和类型
            string wheres = "";

            //且院校 当前有这个赛项的权限
            wheres += " and charindex('" + Request["CompetitionType"] + ",',Jurisdiction)>0 ";
            //去掉已经在试卷里的了
            wheres += " and Id not in (select UR_SchoolId from tb_PaperUserRights where UR_CompetitionType=" + Request["CompetitionType"] + " and UR_CompetitionId='" + Request["CompetitionId"] + "')";
            if (Request["SchoolNameTo"] != null && Request["SchoolNameTo"].ToString().Length > 0)
            {
                wheres += " and Name like '%" + Request["SchoolNameTo"] + "%'";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "Id"; //排序必须填写
            m.strFld = "* ";
            m.tab = "tb_School";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));

        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <returns></returns>
        public string Del()
        {
            try
            {
                var URId = Request["Id"];
                var resultcount = commonbll.DeleteInfo("tb_PaperUserRights", " and URId=" + URId);
                return resultcount.ToString();
            }
            catch
            {
                return "99";
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        public string Add()
        {
            try
            {
                string CompetitionType = Request["CompetitionType"];
                string CompetitionId = Request["CompetitionId"];
                var Ids = Request["Ids"];

                string table = "tb_PaperUserRights"; //表名
                string list = "UR_CompetitionId, UR_CompetitionType, UR_SchoolId, UR_Operator, UR_AddOperator, UR_AddTime";//列
                string vlaue = "@UR_CompetitionId, @UR_CompetitionType, @UR_SchoolId, @UR_Operator, @UR_AddOperator, @UR_AddTime";

                var listSplit = Ids.Split(',');
                for (var i = 0; i < listSplit.Length; i++)
                {
                    if (listSplit[i].Length > 0)
                    {
                        SqlParameter[] pars = new SqlParameter[] 
                    {
                        new SqlParameter("@UR_CompetitionId",CompetitionId),
                        new SqlParameter("@UR_CompetitionType",CompetitionType),
                        new SqlParameter("@UR_SchoolId",listSplit[i]),
                        new SqlParameter("@UR_Operator",UId),
                        new SqlParameter("@UR_AddOperator",UId),
                        new SqlParameter("@UR_AddTime",DateTime.Now)
                    };
                        commonbll.Add(table, list, vlaue, pars);
                    }

                }
                return "1";
            }
            catch
            {
                return "99";
            }

        }

    }
}
