using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocationalProject_Bll;
using System.Data;
using Newtonsoft.Json;
using VocationalProject_Models;
using System.Data.SqlClient;
using VocationalProject.Areas.Admin.Models;
using VocationalProject_Dal;
using VocationalProject.Controllers;

namespace VocationalProject.Areas.Admin.Controllers
{
    /***************************************************************
   FileName:裁判员管理
   Copyright（c）2017-金融教育在线技术开发部
   Author:袁学
   Create Date:2017-3-30
  ******************************************************************/
    public class RefereeManagementController : BaseController
    {
        //
        // GET: /Admin/RefereeManagement/
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        CommonBll commonbll = new CommonBll();
        UserInfoBll userinfobll = new UserInfoBll();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 裁判列表
        /// </summary>
        /// <returns></returns>
        public string GetList()
        {
            string wheres = " and State=1 and UserType=4";
            if (Request["RefereeName"] != null && Request["RefereeName"].ToString().Length > 0)
            {
                wheres += " and ( UserNo like '%" + Request["RefereeName"] + "%' or UserName like '%" + Request["RefereeName"] + "%') ";
            }
            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = " UId desc"; //排序必须填写
            m.strFld = " * ";
            m.tab = "tb_UserInfo";
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
            var AddName = Request["AddName"];

            //验证裁判名称是否存在
            var checkcount = commonbll.GetRecordCount("tb_UserInfo", " and State=1 and UserType=4 and UserName='" + AddName + "'");
            if (checkcount > 0)
            {
                return "88";
            }

            #region 裁判账号生成规则
            string UserNo = "";
            DataTable dt = commonbll.GetListDatatable("UserNo", "tb_UserInfo", " and UserType=4  order by UserNo desc");
            if (dt.Rows.Count > 0)
            {
                string sp = dt.Rows[0]["UserNo"].ToString();
                if (Convert.ToInt32(sp.Substring(1)) < 9)
                {
                    UserNo = "R0000" + (Convert.ToInt32(sp.Substring(1)) + 1);
                }
                if (Convert.ToInt32(sp.Substring(1)) > 8 && Convert.ToInt32(sp.Substring(1)) < 99)
                {
                    UserNo = "R000" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
                if (Convert.ToInt32(sp.Substring(1)) > 98 && Convert.ToInt32(sp.Substring(1)) < 999)
                {
                    UserNo = "R00" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
                if (Convert.ToInt32(sp.Substring(1)) > 998 && Convert.ToInt32(sp.Substring(1)) < 9999)
                {
                    UserNo = "R0" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }

                if (Convert.ToInt32(sp.Substring(1)) > 9998)
                {
                    UserNo = "R" + (Convert.ToInt32(sp.Substring(1)) + 1);

                }
            }
            else
            {
                UserNo = "R00001";
            }
            #endregion
            tb_UserInfo userinfo = new tb_UserInfo();
            userinfo.UserNo = UserNo;
            userinfo.UserPwd = "dysoft888999";
            userinfo.UserName = AddName;
            userinfo.UserType = 4;
            userinfo.State = 1;
            userinfo.Operator = UId;
            userinfo.AddOperator = UId;
            userinfo.AddTime = DateTime.Now;

            var count = userinfobll.Add(userinfo);
            return count.ToString();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string DelReferee()
        {
            try
            {
                var Ids = Request["Ids"];
                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@Operator",UId),
                    new SqlParameter("@State","0")
                };
                //修改状态 操作人
                commonbll.UpdateInfo("tb_UserInfo", " State=@State,Operator=@Operator", " and UId in(" + Ids + ")", pars);
                return "1";
            }
            catch
            {
                return "99";
            }

        }


        /// <summary>
        /// 批量新增
        /// </summary>
        /// <returns></returns>
        public string BatchAdd()
        {
            var BatchAddNameNum = Request["BatchAddNameNum"];
            var num = 0;
            for (var i = 0; i < int.Parse(BatchAddNameNum); i++)
            {

                #region 裁判账号生成规则
                string UserNo = "";
                DataTable dt = commonbll.GetListDatatable("UserNo", "tb_UserInfo", " and UserType=4  order by UserNo desc");
                if (dt.Rows.Count > 0)
                {
                    string sp = dt.Rows[0]["UserNo"].ToString();
                    if (Convert.ToInt32(sp.Substring(1)) < 9)
                    {
                        UserNo = "R0000" + (Convert.ToInt32(sp.Substring(1)) + 1);
                    }
                    if (Convert.ToInt32(sp.Substring(1)) > 8 && Convert.ToInt32(sp.Substring(1)) < 99)
                    {
                        UserNo = "R000" + (Convert.ToInt32(sp.Substring(1)) + 1);

                    }
                    if (Convert.ToInt32(sp.Substring(1)) > 98 && Convert.ToInt32(sp.Substring(1)) < 999)
                    {
                        UserNo = "R00" + (Convert.ToInt32(sp.Substring(1)) + 1);

                    }
                    if (Convert.ToInt32(sp.Substring(1)) > 998 && Convert.ToInt32(sp.Substring(1)) < 9999)
                    {
                        UserNo = "R0" + (Convert.ToInt32(sp.Substring(1)) + 1);

                    }

                    if (Convert.ToInt32(sp.Substring(1)) > 9998)
                    {
                        UserNo = "R" + (Convert.ToInt32(sp.Substring(1)) + 1);

                    }
                }
                else
                {
                    UserNo = "R00001";
                }
                #endregion
                tb_UserInfo userinfo = new tb_UserInfo();
                userinfo.UserNo = UserNo;
                userinfo.UserPwd = "dysoft888999";
                userinfo.UserType = 4;
                userinfo.State = 1;
                userinfo.Operator = UId;
                userinfo.AddOperator = UId;
                userinfo.AddTime = DateTime.Now;
                var count = userinfobll.Add(userinfo);
                num++;
            }

            return num.ToString();

        }

        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <returns></returns>
        public string GetListById()
        {
            DataTable dt = commonbll.GetListDatatable("*", "tb_UserInfo", " and UId=" + Request["UId"]);

            return JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public string Edit()
        {
            var SUId = Request["UId"];
            var EditName = Request["EditName"];
            var Newpwd = Request["Newpwd"];

            //验证裁判名称是否存在
            var checkcount = commonbll.GetRecordCount("tb_UserInfo", " and State=1 and UserType=4 and UserName='" + EditName + "' and UId!=" + SUId);
            if (checkcount > 0)
            {
                return "88";
            }

            string set = "UserName=@UserName,UserPwd=@UserPwd,Operator=@Operator";
            SqlParameter[] pars = new SqlParameter[] 
            {
              
                new SqlParameter("@UserName",EditName),
                new SqlParameter("@UserPwd",Newpwd),
                new SqlParameter("@Operator",UId),
                new SqlParameter("@UId",SUId)
            };
            var count = commonbll.UpdateInfo("tb_UserInfo", set, " and UId=@UId", pars);
            return count.ToString();
        }
    }
}
