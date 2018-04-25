using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using VocationalProject_Bll;

namespace VocationalProject.Controllers
{
    public class JudgeController : Controller
    {
        //
        // GET: /Judge/
        CommonBll commonbll = new CommonBll();
        Manual_ExaminationAnswerBll Eabll = new Manual_ExaminationAnswerBll();
        public ActionResult Index()
        {
            ViewData["error"] = "Yes";
            string Student = "StudentId";
            string StudentId = "0";
            try {
                StudentId = Request.Cookies[Student].Value;
            }
            catch 
            {
                StudentId = "0";
                ViewData["error"] = "error";
            }
             
            string Answer = "AnswerId";
            string AnswerId = "0";
            try
            {
                AnswerId = Request.Cookies[Answer].Value;
            }
            catch
            {
                AnswerId = "0";
                ViewData["error"] = "error";
            }
            //
            DataTable UserinfoDt = commonbll.GetListDatatable(" u.*,s.Name", "tb_UserInfo u left join tb_School s on u.UserSchoolId=s.Id", " and u.UId=" + StudentId + "");
            if (UserinfoDt.Rows.Count > 0)
            {
                ViewData["Pic"] = UserinfoDt.Rows[0]["UserPic"].ToString();
                ViewData["Name"] = UserinfoDt.Rows[0]["UserName"].ToString();
                ViewData["UserNo"] = UserinfoDt.Rows[0]["UserNo"].ToString();
                ViewData["SchoolName"] = UserinfoDt.Rows[0]["Name"].ToString();
            }
            DataTable AchievementDt = commonbll.GetListDatatable("*", "tb_Manual_ExaminationAnswer", " and ID=" + AnswerId + "");
            if (AchievementDt.Rows.Count > 0)
            {
                ViewData["RefereePoints"] = Convert.ToInt32(AchievementDt.Rows[0]["RefereePoints"]);
            }
            return View();
        }
        /// <summary>
        /// 提交大裁判加分
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            string Points = Request["Points"];
            string Answer = "AnswerId";
            string AnswerId = Request.Cookies[Answer].Value;
            string Text = "";
            //
            DataTable AnswerDt = commonbll.GetListDatatable("*", "tb_Manual_ExaminationAnswer", " and ID=" + AnswerId + "");
            if (AnswerDt.Rows.Count > 0)
            {
                //float TaskScore = float.Parse(AnswerDt.Rows[0]["TaskScore"].ToString());
                //float fScore = float.Parse(Points);
                //float TolScore = TaskScore - fScore;
                //int UpdateState = Eabll.Update(AnswerId, TaskScore, fScore, TolScore);

                decimal TaskScore = Math.Round(Convert.ToDecimal(AnswerDt.Rows[0]["TaskScore"].ToString()), 2);
                decimal fScore = Math.Round(Convert.ToDecimal(Points), 2);
                decimal TolScore = Math.Round((TaskScore - fScore), 2);
                int UpdateState = Eabll.Update(AnswerId, TaskScore, fScore, TolScore);

                if (UpdateState > 0)
                {
                    SqlParameter[] pars ={
                               
                                };
                    //根据考试id和用户id查询出最新的一条的 考试总表分值
                    var Eid = "0";
                    var Mid = "0";
                    DataTable dt = commonbll.GetListDatatable("StudenID,ExaminationID", "tb_Manual_ExaminationAnswer", " and ID=" + AnswerId);
                    if (dt.Rows.Count > 0)
                    {
                        Eid = dt.Rows[0]["ExaminationID"].ToString();
                        Mid = dt.Rows[0]["StudenID"].ToString();

                        //查询最新的Id
                        var ERId = commonbll.GetListSclar("top 1 ERId", "tb_ExaminationResult", " and [ER_Type]=2  and [ER_State]=0 and ER_EId=" + Eid + " and ER_MId=" + Mid + " order by ERId desc");
                        commonbll.UpdateInfo("tb_ExaminationResult", " ER_Score='" + TolScore + "'", " and ERId=" + ERId, pars);
                    }
                    Text = "扣分成功！";
                }
            }
            return Text;
        }
        public void Out() 
        {
            //返回记住账号密码
            string username = "";
            string IsBer = "";
            string PassPwd = "";
            HttpCookie cookies = Request.Cookies["UserPasswordsModel"];
            if (cookies != null && cookies.HasKeys)
            {
                Encoding utf8 = Encoding.UTF8;
                username = HttpUtility.UrlDecode(cookies["UserName"], utf8);
                IsBer = cookies["IsBer"];
                if (cookies["IsBer"] == "1")
                {
                    PassPwd = cookies["PassWord"];
                }
            }
            ViewData["username"] = username;
            ViewData["IsBer"] = IsBer;
            ViewData["password"] = PassPwd;
        }

    }
}
