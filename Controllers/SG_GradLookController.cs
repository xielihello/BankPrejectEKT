using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VocationalProject_Bll;

namespace VocationalProject.Controllers
{
    public class SG_GradLookController : BaseController
    {
        //string UId = "3";//用户ID
        //string UserType = "3";//1管理员2教师3学生4裁判
        //string UserNo = "s00001";//登录账号
        //string UserClassId = "1";//所属班级
        CommonBll commonbll = new CommonBll();
        Manual_ExaminationAnswerBll Eabll = new Manual_ExaminationAnswerBll();
        public ActionResult Index()
        {
            string AchievementId =  Session["ResponseState"].ToString();
            string ExaId =  Session["ExaId"].ToString();
            //string AchievementId = Request["ResponseState"];
            //string ExaId = Request["ExaId"];
            DataTable UserinfoDt = commonbll.GetListDatatable(" u.*,s.Name", "tb_UserInfo u left join tb_School s on u.UserSchoolId=s.Id", " and u.UId=" + UId + "");
            if (UserinfoDt.Rows.Count > 0)
            {
                 //用户头像
                if (UserinfoDt.Rows[0]["UserPic"] == null || UserinfoDt.Rows[0]["UserPic"].ToString() == "")
                {
                    ViewData["Pic"] = "/img/profile_s.jpg";
                }
                else
                {
                    ViewData["Pic"] = UserinfoDt.Rows[0]["UserPic"].ToString();
                }
                ViewData["Name"] = UserinfoDt.Rows[0]["UserName"].ToString();
                ViewData["UserNo"] = UserinfoDt.Rows[0]["UserNo"].ToString();
                ViewData["SchoolName"] = UserinfoDt.Rows[0]["Name"].ToString();
            }
            //获取到做题答案数据
            DataTable AchieIdDt = commonbll.GetListDatatable("*", "tb_Manual_Examination", " and ID=" + ExaId + "");
            if (AchieIdDt.Rows.Count > 0)
            {
                ViewData["Pattern"] = Convert.ToInt32(AchieIdDt.Rows[0]["Pattern"]);
                ViewData["ExaminationName"] = AchieIdDt.Rows[0]["ExaminationName"].ToString();
            }
            DataTable AchievementDt = commonbll.GetListDatatable("*", "tb_Manual_ExaminationAnswer", " and ID=" + AchievementId + "");
            if (AchievementDt.Rows.Count > 0)
            {
                ViewData["TotalScore"] = AchievementDt.Rows[0]["TotalScore"];
                ViewData["RefereePoints"] = AchievementDt.Rows[0]["RefereePoints"];
            }
            return View();
        }
        /// <summary>
        /// 保存裁判的扣分
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            string AnswerId = Request["AnswerId"];
            string UserNo = Request["UserNo"];
            string Pwdid = Request["Pwdid"];
            string Score = Request["Score"];
            DataTable Dt = commonbll.GetListDatatable("*", "tb_UserInfo", " and UserNo='" + UserNo + "' and UserPwd='" + Pwdid + "'");
            string Text = "";
            if (Dt.Rows.Count > 0)
            {
                if (Convert.ToInt32(Dt.Rows[0]["UserType"]) == 4)
                {
                    DataTable AnswerDt = commonbll.GetListDatatable("*", "tb_Manual_ExaminationAnswer", " and ID=" + AnswerId + "");
                    if (AnswerDt.Rows.Count > 0)
                    {
                        decimal TaskScore = Math.Round(Convert.ToDecimal(AnswerDt.Rows[0]["TaskScore"].ToString()), 2);
                        decimal fScore = Math.Round(Convert.ToDecimal(Score), 2);
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
                }
                else
                {
                    Text = "您不是裁判无法进行扣分操作！";
                }
            }
            else
            {
                Text = "用户或者密码不正确！";
            }
            return Text;
        }

        public ActionResult GradLookShow()
        {
            string AchievementId = Request["AnswerId"];
            DataTable AchievementDt = commonbll.GetListDatatable("*", "tb_Manual_ExaminationAnswer", " and ID=" + AchievementId + "");
            if (AchievementDt.Rows.Count > 0)
            {
                ViewData["TaskScore"] = AchievementDt.Rows[0]["TaskScore"];
                ViewData["RefereePoints"] = AchievementDt.Rows[0]["RefereePoints"];
                ViewData["TotalScore"] = AchievementDt.Rows[0]["TotalScore"];

                DataTable UserinfoDt = commonbll.GetListDatatable(" u.*,s.Name", "tb_UserInfo u left join tb_School s on u.UserSchoolId=s.Id", " and u.UId=" + Convert.ToInt32(AchievementDt.Rows[0]["StudenID"] + ""));
                if (UserinfoDt.Rows.Count > 0)
                {

                    //用户头像
                    if (UserinfoDt.Rows[0]["UserPic"] == null || UserinfoDt.Rows[0]["UserPic"].ToString() == "")
                    {
                        ViewData["Pic"] = "/img/profile_s.jpg";
                    }
                    else
                    {
                        ViewData["Pic"] = UserinfoDt.Rows[0]["UserPic"].ToString();
                    }

                    ViewData["Name"] = UserinfoDt.Rows[0]["UserName"].ToString();
                    ViewData["UserNo"] = UserinfoDt.Rows[0]["UserNo"].ToString();
                    ViewData["SchoolName"] = UserinfoDt.Rows[0]["Name"].ToString();
                }
                DataTable AchieIdDt = commonbll.GetListDatatable("*", "tb_Manual_Examination", " and ID=" + Convert.ToInt32(AchievementDt.Rows[0]["ExaminationID"] + ""));
                if (AchieIdDt.Rows.Count > 0)
                {
                    ViewData["ExaminationName"] = AchieIdDt.Rows[0]["ExaminationName"].ToString();
                }
            }
            return View();
        }
    }
}
