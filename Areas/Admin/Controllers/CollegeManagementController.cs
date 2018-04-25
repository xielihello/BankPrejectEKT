using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocationalProject.Areas.Admin.Models;
using VocationalProject.Controllers;
using VocationalProject_Bll;
using VocationalProject_Dal;
using VocationalProject_Models;

namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
    FileName:院校管理
    Copyright（c）2017-金融教育在线技术开发部
    Author:陈飞
    Create Date:2017-3-29
    ******************************************************************/
    public class CollegeManagementController : BaseController
    {
        //
        // GET: /Admin/CollegeManagement/
        SchoolBll schoolBll = new SchoolBll();
        CommonBll commBll = new CommonBll();
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        public ActionResult College()
        {

            return View();
        }

        /// <summary>
        /// 院校列表以及查询
        /// </summary>
        /// <returns></returns>
        public string CollegeGetList()
        {
            string strWhere = string.Empty;
            string Name = Request["Name"];
            if (Name != "")
            {
                strWhere = "and  Name like '%" + Name + "%'";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 5;
            m.Sort = "a.Id desc"; //排序必须填写
            m.strFld = " *";
            m.tab = "tb_School a";
            m.strWhere = strWhere;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));
        }

        /// <summary>
        /// 院校用户-新增
        /// </summary>
        /// <param name="CollegeName">院校名称</param>
        /// <param name="Authority">用户权限设置（如：1,2,3,4）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(string CollegeName, string Authority)
        {

            tb_School scholl = new tb_School();
            scholl.Name = CollegeName;
            scholl.Jurisdiction = "1,";
            scholl.Operator = UId;
            scholl.AddOperator = UId;
            scholl.AddTime = DateTime.Now;
            scholl.Custom1 = null;
            scholl.Custom2 = null;
            scholl.Custom3 = null;
            //新增一条院校数据
            schoolBll.AddSchoolBll(scholl);
            return RedirectToAction("College", "CollegeManagement");


        }

        /// <summary>
        /// 验证院校名称是否重复
        /// </summary>
        /// <param name="CollegeName">院校名称</param>
        /// <returns></returns>
        public int CollegeCheck(string CollegeName)
        {
            return commBll.GetRecordCount("tb_School", " and Name='" + CollegeName + "'");
        }
        /// <summary>
        ///  院校用户-删除
        /// </summary>
        /// <param name="Id">多个或单个ID</param>
        /// <returns></returns>
        public string CollegeDele(string Id)
        {
            int Count = commBll.GetRecordCount("tb_Team", " and SchoolId in(" + Id + ") ");
            Count += commBll.GetRecordCount("tb_UserInfo", " and UserSchoolId in(" + Id + ") and State=1");
            if (Count > 0)
            {
                return "-1";
            }
            else
            {
                return commBll.DeleteInfo("tb_School", " and Id in(" + Id + ")").ToString();
            }
        }

        /// <summary>
        /// 院校用户-编辑读取数据
        /// </summary>
        /// <param name="Id">院校用户表ID</param>
        /// <returns></returns>
        public string CollegeEdit(string Id)
        {
            DataTable data = commBll.GetListDatatable("Id,Name,Jurisdiction", "tb_School", " and Operator=" + UId + " and Id=" + Id + "");
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        ///  院校用户-编辑保存
        /// </summary>
        /// <param name="Id">院校用户表ID</param>
        /// <param name="CollegeName">院校名称</param>
        /// <param name="Jurisdiction">用户权限设置</param>
        /// <returns></returns>
        public string CollegeEditSave(int Id, string CollegeName, string Jurisdiction)
        {
            int Count = commBll.GetRecordCount("tb_School", "and Id not in(" + Id + ") and Name = '" + CollegeName + "'");
            if (Count > 0)
            {
                return "-1";
            }
            else
            {
                tb_School scholl = new tb_School();
                scholl.Id = Id;
                scholl.Name = CollegeName;
                scholl.Jurisdiction = Jurisdiction;
                //编辑一条院校数据
                return schoolBll.EditSchoolBll(scholl).ToString();
            }
        }
    }
}
