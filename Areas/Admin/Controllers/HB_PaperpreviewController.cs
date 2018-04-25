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
   FileName:货币知识 试卷管理 预览微调
   Copyright（c）2017-金融教育在线技术开发部
   Author:袁学
   Create Date:2017-4-7
   ******************************************************************/
    public class HB_PaperpreviewController : BaseController
    {
        //
        // GET: /Admin/HB_Paperpreview/
        //string UId = "1";//用户ID
        //string UserType = "1";//1管理员2教师3学生4裁判
        //string UserNo = "admin";//登录账号
        //string TeacherSchoolId = "1";//教师属于的院校
        CommonBll commonbll = new CommonBll();
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 视图 加点题
        /// </summary>
        /// <returns></returns>
        public ActionResult AddTIndex()
        {
            var P_Name = commonbll.GetListSclar("P_Name", "tb_HB_Paper", " and PId="+Request["Pid"]);
            ViewData["PName"] = P_Name;
            return View();
        }

        /// <summary>
        /// 获取试卷信息
        /// </summary>
        /// <returns></returns>
        public string GetPreview()
        {
            DataTable dt = commonbll.GetListDatatable("q.*,EP_Score,P_Name",
                      @"tb_HB_QuestionBank q
                        inner join tb_HB_ExaminationPapers e on e.EP_QBId=q.QuestionBId
                        inner join tb_HB_Paper p on p.PId=e.EP_PId", " and PId=" + Request["PId"] + " order by QB_Type");

            return JsonConvert.SerializeObject(dt);

        }

        /// <summary>
        /// 单个设置分值
        /// </summary>
        /// <returns></returns>
        public string IndividualSetScore()
        {
            try
            {
                var EP_Score = Request["EP_Score"];
                var EP_QBId = Request["EP_QBId"];
                var EP_PId = Request["EP_PId"];
                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@EP_Score",EP_Score),
                    new SqlParameter("@EP_QBId",EP_QBId),
                    new SqlParameter("@EP_PId",EP_PId),
                    new SqlParameter("@EP_Operator",UId)
                };
                commonbll.UpdateInfo("tb_HB_ExaminationPapers", " EP_Score=@EP_Score,EP_Operator=@EP_Operator", " and EP_QBId=@EP_QBId and EP_PId=@EP_PId", pars);

                return "1";
            }
            catch
            {
                return "99";

            }
        }

        /// <summary>
        /// 批量设置分值
        /// </summary>
        /// <returns></returns>
        public string SetBatchScores()
        {
            try
            {

                var EP_Score = Request["EP_Score"];
                var Type = Request["Type"];
                var EP_PId = Request["EP_PId"];
                SqlParameter[] pars = new SqlParameter[] 
                {
                    new SqlParameter("@EP_Score",EP_Score),
                    new SqlParameter("@QB_Type",Type),
                    new SqlParameter("@EP_PId",EP_PId),
                    new SqlParameter("@EP_Operator",UId)
                };
                commonbll.UpdateInfo("tb_HB_ExaminationPapers", " EP_Score=@EP_Score,EP_Operator=@EP_Operator",
                    " and EPId in(select EPId from tb_HB_ExaminationPapers e inner join tb_HB_QuestionBank q on e.EP_QBId=q.QuestionBId where EP_PId=@EP_PId and QB_Type=@QB_Type)", pars);

                return "1";
            }
            catch
            {
                return "99";

            }
        }

        /// <summary>
        /// 移除试题
        /// </summary>
        /// <returns></returns>
        public string delExaminationPapers()
        {
            try
            {
                string EP_QBId = Request["EP_QBId"];
                string EP_PId = Request["EP_PId"];
                //然后 数据存入试卷试题关联表
                commonbll.DeleteInfo("tb_HB_ExaminationPapers", " and EP_PId='" + EP_PId + "' and EP_QBId='" + EP_QBId + "'");
                return "1";
            }
            catch
            {
                return "99";
            }
        }

    }
}
