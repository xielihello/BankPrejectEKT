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
    public class FH_ExaminationController : BaseController
    {
        /***************************************************************
        FileName:班级管理
        Copyright（c）2017-金融教育在线技术开发部
        Author:邵世铨
        Create Date:2017-4-11
       ******************************************************************/
        CommonBll commBll = new CommonBll();
        //string UId = "5";//用户ID
        //string UserType = "2";//1管理员2教师3学生4裁判
        FH_ExaminationBll FHexaBll = new FH_ExaminationBll();
        public ActionResult FHExamination()
        {
            //    ViewData["UserType"] = UserType;
            //     DataTable Data =new DataTable();
            //    if(Convert.ToInt32(UserType)==1)
            //    {
            //        Data = commBll.GetListDatatable("*", "tb_FH_Examination order by id desc", "");
            //    }
            //    else if (Convert.ToInt32(UserType) == 2)
            //    {
            //        Data = commBll.GetListDatatable("*", "tb_FH_Examination", "and Kind=2 order by id desc");
            //    }

            //    return View(Data);
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
                wheres += " and ExaminationType='" + Request["E_Type"].ToString() + "'";
            }


            if (Request["E_IsState"] != null && Request["E_IsState"].ToString() != "0")//激活状态
            {
                wheres += " and Isactivation='" + Request["E_IsState"].ToString() + "'";
            }


            PageModel m = new PageModel();
            m.PageIndex = !string.IsNullOrEmpty(Request["page"]) ? int.Parse(Request["page"]) : 1;
            m.PageSize = !string.IsNullOrEmpty(Request["PageSize"]) ? int.Parse(Request["PageSize"]) : 10;
            m.Sort = "AddTime desc"; //排序必须填写
            m.strFld = " * ";
            m.tab = "tb_FH_Examination";
            m.strWhere = wheres;
            int PageCount = 0;//总数
            var dt = Pager.GetList(m, ref PageCount);
            return JsonConvert.SerializeObject(JsonResultPagedLists(PageCount, m.PageIndex, m.PageSize, dt));

        }
        /// <summary>
        /// 搜索
        /// </summary>
        /// <returns></returns>
        public ActionResult Select(string EamxName, string PatternName, string StateName)
        {
            ViewData["UserType"] = UserType;
            DataTable data = commBll.GetListDatatable("*", "tb_FH_Examination", " and ExaminationName like '%" + EamxName + "%' or ExaminationType=" + PatternName + " or Isactivation=" + StateName + "");
            return View("FHExamination", data);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id">任务id</param>
        /// <returns></returns>
        public int Delete(string Id)
        {

            try
            {
                //SqlParameter[] pars = new SqlParameter[] 
                //{
                //    new SqlParameter("@Operator",UId),
                //    new SqlParameter("@IsDelete","0")
                //};
                ////修改状态 操作人
                //commBll.UpdateInfo("tb_FH_Examination", " IsDelete=@IsDelete,Operator=@Operator", " and Id in(" + Id + ") and Isactivation=2", pars);
                commBll.DeleteInfo("tb_FH_Examination", " and Isactivation=2 and Id in (" + Id + ")");
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="Id">考试ID</param>
        /// <returns></returns>
        public int Activation(string Id)
        {
            return FHexaBll.Activation(Id);
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="id">任务</param>
        /// <returns></returns>
        public int Close(string id)
        {
            return FHexaBll.Close(id);
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
        public StringBuilder SelectTask()
        {
           // DataTable data = commBll.GetListDatatable("*", "tb_FH_TestPaper", " and IsDelete=1");
            DataTable data = new DataTable();
            string wheres = " and IsDelete=1";
            if (UserType == "1")//管理员 所有管理员的
            {
                wheres += " and Kind=1";
                data = commBll.GetListDatatable("*", "tb_FH_TestPaper", wheres);
            }
            if (UserType == "2")//教师只看自己的设置的考试或者是权限分配的
            {
                wheres += " and (AddOperator=" + UId + " or Id in(select P.UR_CompetitionId from tb_UserInfo U left join  tb_PaperUserRights P on P.UR_SchoolId=U.UserSchoolId where P.UR_CompetitionType=3 and U.UId=" + UId + "))";
                data = commBll.GetListDatatable("*", "tb_FH_TestPaper", wheres);
            }

            StringBuilder SB = new StringBuilder();
            if (data.Rows.Count > 0)
            {
                SB.Append(" <option value=\"0\">请选择试卷</option>");
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    SB.Append("<option value=" + data.Rows[i]["Id"] + ">" + data.Rows[i]["TestPaperName"] + "</option>");
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
            //    SB.Append(" <option value=\"0\">请选择班级</option>");
                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    SB.Append("<option value=" + Dt.Rows[i]["ID"] + ">" + Dt.Rows[i]["TeamName"] + "</option>");
                }
            }
            return SB;
        }

        /// <summary>
        /// 新增保存
        /// </summary>
        /// <returns></returns>
        public int AddSave()
        {
            try
            {
                string E_Name = Request["EamxName"];//试卷名称
                string E_PId = Request["Taskid"];
                string E_Type = Request["Pattern"];//模式
                string E_StartTime = Request["StartTime"];
                string E_EndTime = Request["EndTime"];
                string E_Whenlong = Request["minId"];//时长
                string E_TeamId = Request["Classid"];//班级
                string E_IsTimeBonus = Request["Plus"];//是否时间加分
                string E_IsState = "2";//未激活
                string E_Kind = UserType;//1系统 2是教师 正好与角色相对应
                string E_Operator = UId;//操作
                string E_AddOperator = UId;//创建人
                DateTime E_AddTime = DateTime.Now;
                //string UR_Custom2 = UserNo;//登录账号


                //校验考试名称是否存在
                string wheres = " and ExaminationName='" + E_Name + "'";//试卷状态

                if (UserType == "1")//管理员 所有管理员的
                {
                    wheres += " and Kind=1";
                }
                if (UserType == "2")//教师  只看自己的设置的考试
                {
                    wheres += " and AddOperator=" + UId;
                }

                var checkcount = commBll.GetRecordCount("tb_FH_Examination", wheres);
                if (checkcount > 0)
                {
                    return 88;
                }

                string table = "tb_FH_Examination"; //表名
                string list = "ExaminationName, TestPaperId, ExaminationType, ExaminationStartTime, ExaminationEndTime, ExaminationLength, Spare2, IsPlus, Isactivation, Kind, Operator, AddOperator, AddTime";//列
                string vlaue = "@E_Name, @E_PId, @E_Type, @E_StartTime, @E_EndTime, @E_Whenlong, @E_TeamId, @E_IsTimeBonus, @E_IsState, @E_Kind, @E_Operator, @E_AddOperator, @E_AddTime";
                if (E_TeamId == ",undefined,")
                {
                    E_TeamId = "";
                }

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
                    };
                commBll.Add(table, list, vlaue, pars);

                return 1;
            }
            catch
            {
                return 99;
            }
        }
        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <returns></returns>
        public string GetListById()
        {
            var iid = Request["EId"];
            DataTable dt = commBll.GetListDatatable("*", "tb_FH_Examination", "and Id=" + iid + "");
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
        /// 编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            ViewBag.UserType = UserType;
            string wheres = " and IsDelete=1";//试卷状态
            DataTable dt = new DataTable();
            if (UserType == "1")//管理员 所有管理员的
            {
                wheres += " and Kind=1";
                dt = commBll.GetListDatatable("*", "tb_FH_TestPaper", wheres);
            }
            if (UserType == "2")//教师只看自己的设置的考试或者是权限分配的
            {
                wheres += " and (AddOperator=" + UId + " or Id in(select P.UR_CompetitionId from tb_UserInfo U left join  tb_PaperUserRights P on P.UR_SchoolId=U.UserSchoolId where P.UR_CompetitionType=3 and U.UId=" + UId + "))";
                dt = commBll.GetListDatatable("*", "tb_FH_TestPaper", wheres);
            }
            return View(dt);
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
                if (E_TeamId == ",undefined,")
                {
                    E_TeamId = "";
                }

                //校验考试名称是否存在
                string wheres = " and ExaminationName='" + E_Name + "' and ID!=" + EId;//试卷状态

                if (UserType == "1")//管理员 所有管理员的
                {
                    wheres += " and Kind=1";
                }
                if (UserType == "2")//教师  只看自己的设置的考试
                {
                    wheres += " and AddOperator=" + UId;
                }

                var checkcount = commBll.GetRecordCount("tb_FH_Examination", wheres);
                if (checkcount > 0)
                {
                    return 88;
                }

                string table = "tb_FH_Examination"; //表名
                string list = @" ExaminationName=@E_Name, TestPaperId=@E_PId, ExaminationType=@E_Type, ExaminationStartTime=@E_StartTime, 
                                      ExaminationEndTime=@E_EndTime, ExaminationLength=@E_Whenlong, Spare2=@E_TeamId, 
                                      IsPlus=@E_IsTimeBonus, Operator=@E_Operator";//列



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
                commBll.UpdateInfo(table, list, " and Id=@EId", pars);

                return 1;
            }
            catch
            {
                return 99;
            }

        }
    }
}
