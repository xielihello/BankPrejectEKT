using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using VocationalProject_Bll;
using System.Data;
using Newtonsoft.Json;
using VocationalProject_Models;
using System.Data.SqlClient;
using VocationalProject_DBUtility.Sql;

namespace VocationalProject.ashx
{
    /***************************************************************
 FileName:导入excel
 Copyright（c）2017-金融教育在线技术开发部
 Author:袁学
 Create Date:2017-3-31
******************************************************************/
    public class hbExport : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            string action = context.Request["action"];
            var m = context.Session["UserInfo"] as tb_UserInfo;
            string UId = "1";//用户ID
            string UserType = "1";//1管理员2教师3学生4裁判
            string UserNo = "admin";//登录账号
            string TeacherSchoolId = "1";//教师属于的院校

            if (m != null)
            {
                UId = m.UId.ToString();//用户ID
                UserType = m.UserType.ToString();//1管理员2教师3学生4裁判
                UserNo = m.UserNo.ToString();//登录账号
                TeacherSchoolId = m.UserSchoolId.ToString();//教师属于的院校

                CommonBll commonbll = new CommonBll();

                #region 货币知识题库试题上传
                if (action == "HBZS_QuestionBankImport")
                {
                    HttpPostedFile hpFile = context.Request.Files[0];
                    //获得浏览器端 传过来 第一个文件选择框的数据  
                    string filePath = "/Export";
                    //获得上传上来的文件名称  
                    string fileName = System.IO.Path.GetFileName(hpFile.FileName);

                    string fileNameNew = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + "" + DateTime.Now.Second + "" + DateTime.Now.Millisecond + "货币知识试题" + fileName.Substring(fileName.LastIndexOf('.'));
                    //获得 要保存的物理路径  

                    filePath = context.Server.MapPath(filePath + "/" + fileNameNew);
                    //将上传来的 文件数据 保存在 对应的 物理路径上  
                    hpFile.SaveAs(filePath);

                    //然后
                    Aspose.Cells.Workbook wk = new Aspose.Cells.Workbook();
                    string str = HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"];
                    wk.Open(str + "Export/" + fileNameNew);

                    string QB_CourseId = "1";
                    string QB_Type = string.Empty;//题型
                    string QB_Description = string.Empty;//描述
                    string QB_A = string.Empty;//选项A-E
                    string QB_B = string.Empty;
                    string QB_C = string.Empty;
                    string QB_D = string.Empty;
                    string QB_E = string.Empty;

                    string QB_Answer = string.Empty;//标准答案
                    string QB_Keyword = string.Empty;//关键字
                    string QB_State = "1";//状态

                    string QB_Kind = UserType;//1系统 2是教师 正好与角色相对应

                    string QB_Operator = UId;//操作
                    string QB_AddOperator = UId;//创建人
                    DateTime QB_AddTime = DateTime.Now;
                    string QB_Custom2 = UserNo;//登录账号

                    string table = "tb_HB_QuestionBank"; //表名
                    string list = "QB_CourseId, QB_Type, QB_Description, QB_A, QB_B, QB_C, QB_D, QB_E, QB_Answer, QB_Keyword, QB_State, QB_Kind, QB_Operator, QB_AddOperator, QB_AddTime, QB_Custom2";//列
                    string vlaue = "@QB_CourseId, @QB_Type, @QB_Description, @QB_A, @QB_B, @QB_C, @QB_D, @QB_E, @QB_Answer, @QB_Keyword, @QB_State, @QB_Kind, @QB_Operator, @QB_AddOperator, @QB_AddTime, @QB_Custom2";


                    int success = 0;//成功新增个数
                    int repeat = 0;//重复


                    try
                    {

                        for (int i = 1; i < wk.Worksheets[0].Cells.Rows.Count; i++)
                        {
                            QB_Type = string.Empty;
                            QB_Description = string.Empty;
                            QB_A = string.Empty;
                            QB_B = string.Empty;
                            QB_C = string.Empty;
                            QB_D = string.Empty;
                            QB_E = string.Empty;
                            QB_Answer = string.Empty;
                            QB_Keyword = string.Empty;
                            // 判断结束
                            if (wk.Worksheets[0].Cells[i, 0].Value != null)
                            {

                                if (wk.Worksheets[0].Cells[i, 0].Value != null)
                                {
                                    QB_Type = wk.Worksheets[0].Cells[i, 0].Value.ToString().Trim();
                                }
                                if (wk.Worksheets[0].Cells[i, 1].Value != null)
                                {
                                    QB_Description = wk.Worksheets[0].Cells[i, 1].Value.ToString().Trim();
                                    QB_Description = QB_Description.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                                    QB_Description = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(QB_Description, "");
                                    QB_Description = QB_Description.Replace("?", "");

                                }
                                if (wk.Worksheets[0].Cells[i, 2].Value != null)
                                {
                                    QB_A = wk.Worksheets[0].Cells[i, 2].Value.ToString().Trim();
                                }
                                if (wk.Worksheets[0].Cells[i, 3].Value != null)
                                {
                                    QB_B = wk.Worksheets[0].Cells[i, 3].Value.ToString().Trim();
                                }
                                if (wk.Worksheets[0].Cells[i, 4].Value != null)
                                {
                                    QB_C = wk.Worksheets[0].Cells[i, 4].Value.ToString().Trim();
                                }
                                if (wk.Worksheets[0].Cells[i, 5].Value != null)
                                {
                                    QB_D = wk.Worksheets[0].Cells[i, 5].Value.ToString().Trim();
                                }
                                if (wk.Worksheets[0].Cells[i, 6].Value != null)
                                {
                                    QB_E = wk.Worksheets[0].Cells[i, 6].Value.ToString().Trim();
                                }
                                if (wk.Worksheets[0].Cells[i, 7].Value != null)
                                {
                                    //传说的答案
                                    //判断多选题 用','分开或者没分开 2多选
                                    if (QB_Type == "2")
                                    {
                                        string STstr = wk.Worksheets[0].Cells[i, 7].Value.ToString().Trim();
                                        string SPSTstr = "";

                                        STstr = STstr.Replace('，', ',');
                                        if (STstr.IndexOf(',') == -1)//不存在
                                        {
                                            for (var x = 0; x < STstr.Length; x++)
                                            {
                                                SPSTstr += STstr[x] + ",";
                                            }
                                            QB_Answer = SPSTstr.Substring(0, SPSTstr.Length - 1);

                                            //
                                        }
                                        else
                                        {
                                            QB_Answer = STstr;
                                        }

                                        var nx = "A,B,C,D,E,";

                                        var mc = "";
                                        var n1 = QB_Answer.Split(',');
                                        var n2 = nx.Split(',');
                                        for (var h = 0; h < n2.Length; h++)
                                        {

                                            for (var k = 0; k < n1.Length; k++)
                                            {
                                                if (n1[k].Length != 0 && n2[h].Length != 0)
                                                {
                                                    if (n1[k] == n2[h])
                                                    {
                                                        mc += n2[h] + ",";
                                                    }
                                                }

                                            }

                                        }
                                        if (mc.Length > 0)
                                        {
                                            QB_Answer = mc.Substring(0, mc.Length - 1);
                                        }
                                    }
                                    else
                                    {
                                        QB_Answer = wk.Worksheets[0].Cells[i, 7].Value.ToString().Trim();
                                    }

                                }
                                if (wk.Worksheets[0].Cells[i, 8].Value != null)
                                {
                                    QB_Keyword = wk.Worksheets[0].Cells[i, 8].Value.ToString().Trim();
                                }


                                //验证是否 同一试题下重复
                                //教师和管理员 校验范围不一样
                                string checkewheres = "";
                                if (UserType == "1")//管理员
                                {
                                    //匹配所有系统的题重复
                                    checkewheres += "  and QB_Kind=1";

                                }
                                if (UserType == "2")//教师
                                {
                                    //匹配自己的
                                    checkewheres += "  and QB_AddOperator=" + QB_AddOperator;
                                }
                                //同一题型下 题目描述和选项均相同
                                checkewheres += " and QB_State=1 and QB_Type=" + QB_Type + " and QB_Description='" + QB_Description + "'";
                                checkewheres += " and QB_A='" + QB_A + "' and QB_B='" + QB_B + "' and QB_C='" + QB_C + "' and QB_D='" + QB_D + "' and QB_E='" + QB_E + "'";

                                //重复校验
                                int checekcount = commonbll.GetRecordCount("tb_HB_QuestionBank", checkewheres);
                                if (checekcount > 0)//存在重复
                                {
                                    repeat++;
                                }
                                else
                                {

                                    SqlParameter[] pars = new SqlParameter[] 
                                {
                                    new SqlParameter("@QB_CourseId",QB_CourseId),
                                    new SqlParameter("@QB_Type",QB_Type),
                                    new SqlParameter("@QB_Description",QB_Description),
                                    new SqlParameter("@QB_A",QB_A),
                                    new SqlParameter("@QB_B",QB_B),
                                    new SqlParameter("@QB_C",QB_C),
                                    new SqlParameter("@QB_D",QB_D),
                                    new SqlParameter("@QB_E",QB_E),
                                    new SqlParameter("@QB_Answer",QB_Answer),
                                    new SqlParameter("@QB_Keyword",QB_Keyword),
                                    new SqlParameter("@QB_State",QB_State),
                                    new SqlParameter("@QB_Kind",QB_Kind),
                                    new SqlParameter("@QB_Operator",QB_Operator),
                                    new SqlParameter("@QB_AddOperator",QB_AddOperator),
                                    new SqlParameter("@QB_AddTime",QB_AddTime),
                                    new SqlParameter("@QB_Custom2",QB_Custom2)


                                };
                                    var resultcount = commonbll.Add(table, list, vlaue, pars);
                                    if (resultcount == 1)
                                    {
                                        success++;//成功
                                    }

                                }
                            }

                        }
                        //循环完事
                        var json = new object[] {
                        new{
                            repeat=repeat,//
                            success=success,
                            error=""//异常
                        }
                    };
                        context.Response.Write(JsonConvert.SerializeObject(json));
                        return;

                    }
                    catch (Exception e)
                    {
                        //循环完事
                        var json = new object[] {
                        new{
                            repeat=repeat,
                            success=success,
                            error=e.Message
                        }
                    };
                        context.Response.Write(JsonConvert.SerializeObject(json));
                        return;
                    }
                }
                #endregion

                #region 货币知识试卷导入
                if (action == "HBZS_PaperImport")
                {
                    HttpPostedFile hpFile = context.Request.Files[0];
                    //获得浏览器端 传过来 第一个文件选择框的数据  
                    string filePath = "/Export";
                    //获得上传上来的文件名称  
                    string fileName = System.IO.Path.GetFileName(hpFile.FileName);

                    string fileNameNew = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + "" + DateTime.Now.Hour + "" + DateTime.Now.Minute + "" + DateTime.Now.Second + "" + DateTime.Now.Millisecond + "货币知识试题" + fileName.Substring(fileName.LastIndexOf('.'));
                    //获得 要保存的物理路径  

                    filePath = context.Server.MapPath(filePath + "/" + fileNameNew);
                    //将上传来的 文件数据 保存在 对应的 物理路径上  
                    hpFile.SaveAs(filePath);

                    //然后
                    Aspose.Cells.Workbook wk = new Aspose.Cells.Workbook();
                    string str = HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"];
                    wk.Open(str + "Export/" + fileNameNew);

                    string Scores = "0";
                    string QB_CourseId = "1";
                    string QB_Type = string.Empty;//题型
                    string QB_Description = string.Empty;//描述
                    string QB_A = string.Empty;//选项A-E
                    string QB_B = string.Empty;
                    string QB_C = string.Empty;
                    string QB_D = string.Empty;
                    string QB_E = string.Empty;

                    string QB_Answer = string.Empty;//标准答案
                    string QB_Keyword = string.Empty;//关键字
                    string QB_State = "1";//状态

                    string QB_Kind = UserType;//1系统 2是教师 正好与角色相对应

                    string QB_Operator = UId;//操作
                    string QB_AddOperator = UId;//创建人
                    DateTime QB_AddTime = DateTime.Now;
                    string QB_Custom2 = UserNo;//登录账号

                    string table = "tb_HB_QuestionBank"; //表名
                    string list = "QB_CourseId, QB_Type, QB_Description, QB_A, QB_B, QB_C, QB_D, QB_E, QB_Answer, QB_Keyword, QB_State, QB_Kind, QB_Operator, QB_AddOperator, QB_AddTime, QB_Custom2";//列
                    string vlaue = "@QB_CourseId, @QB_Type, @QB_Description, @QB_A, @QB_B, @QB_C, @QB_D, @QB_E, @QB_Answer, @QB_Keyword, @QB_State, @QB_Kind, @QB_Operator, @QB_AddOperator, @QB_AddTime, @QB_Custom2";

                    string sqlstr = "";
                    int success = 0;//同步更新题库个数

                    //首先新增试卷
                    if (wk.Worksheets[0].Cells.Rows.Count > 0)//存在两行记录
                    {
                        //先建试卷后 新增试卷试题表数据 返回 试卷Id
                        string table1 = "tb_HB_Paper"; //表名
                        string list1 = "P_Name, P_IsOrder, P_State, P_Kind, P_Operator, P_AddOperator, P_AddTime,P_Custom2";//列
                        string vlaue1 = "@P_Name, @P_IsOrder, @P_State, @P_Kind, @P_Operator, @P_AddOperator, @P_AddTime,@P_Custom2";

                        string AddP_Name = wk.Worksheets[0].Cells[1, 0].Value.ToString().Trim();//获取试卷名称
                        string IsOrder = "0";
                        string P_Kind = UserType;//1系统 2是教师 正好与角色相对应

                        string P_Operator = UId;//操作
                        string P_AddOperator = UId;//创建人
                        DateTime P_AddTime = DateTime.Now;
                        string P_Custom2 = UserNo;//登录账号


                        string wheres = " and P_State=1 and P_Name='" + AddP_Name + "'";//试卷状态

                        //角色区分  //暂时不考虑独立打包客户，需要web加个配置 读取 控制 后期加
                        if (UserType == "2")//教师  如果是教师就只显示自己的试卷和推送过的哦
                        {
                            wheres += " and (P_AddOperator=" + UId + " or PId in (select UR_CompetitionId from tb_PaperUserRights where UR_CompetitionType=1 and UR_SchoolId=" + TeacherSchoolId + "))";
                        }
                        //验证试卷名称
                        var checkcount = commonbll.GetRecordCount("tb_HB_Paper", wheres);
                        if (checkcount > 0)
                        {
                            var json = new object[] {
                        new{
                            error="试卷名称已经存在,请修改试卷名称"
                        }
                        };
                            context.Response.Write(JsonConvert.SerializeObject(json));
                            return;
                        }

                        //////////继续走/////////////////////////////////////
                        SqlParameter[] pars1 = new SqlParameter[] 
                        {
                            new SqlParameter("@P_Name",AddP_Name),
                            new SqlParameter("@P_IsOrder",IsOrder),
                            new SqlParameter("@P_State","0"),
                            new SqlParameter("@P_Kind",P_Kind),
                            new SqlParameter("@P_Operator",P_Operator),

                            new SqlParameter("@P_AddOperator",P_AddOperator),
                            new SqlParameter("@P_AddTime",P_AddTime),
                            new SqlParameter("@P_Custom2",P_Custom2)
                        };
                        var resultcount = commonbll.Add(table1, list1, vlaue1, pars1);
                        if (resultcount == 1)
                        {
                            try
                            {
                                //获取最新 新增的
                                string NPid = commonbll.GetListSclar("max(PId) as PId", "tb_HB_Paper", " and P_State=0 and P_Kind='" + P_Kind + "' and P_AddOperator='" + P_AddOperator + "'");

                                //
                                for (int i = 1; i < wk.Worksheets[0].Cells.Rows.Count; i++)
                                {
                                    Scores = "0";
                                    QB_Type = string.Empty;
                                    QB_Description = string.Empty;
                                    QB_A = string.Empty;
                                    QB_B = string.Empty;
                                    QB_C = string.Empty;
                                    QB_D = string.Empty;
                                    QB_E = string.Empty;
                                    QB_Answer = string.Empty;
                                    QB_Keyword = string.Empty;

                                    // 判断结束
                                    if (wk.Worksheets[0].Cells[i, 1].Value != null && wk.Worksheets[0].Cells[i, 2].Value != null)
                                    {

                                        if (wk.Worksheets[0].Cells[i, 1].Value != null)//分值
                                        {
                                            Scores = wk.Worksheets[0].Cells[i, 1].Value.ToString().Trim();
                                        }
                                        if (wk.Worksheets[0].Cells[i, 2].Value != null)//题型
                                        {
                                            QB_Type = wk.Worksheets[0].Cells[i, 2].Value.ToString().Trim();
                                        }
                                        if (wk.Worksheets[0].Cells[i, 3].Value != null)//描述
                                        {
                                            QB_Description = wk.Worksheets[0].Cells[i, 3].Value.ToString().Trim();
                                            QB_Description = QB_Description.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                                            QB_Description = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(QB_Description, "");
                                            QB_Description = QB_Description.Replace("?", "");

                                        }
                                        if (wk.Worksheets[0].Cells[i, 4].Value != null)
                                        {
                                            QB_A = wk.Worksheets[0].Cells[i, 4].Value.ToString().Trim();
                                        }
                                        if (wk.Worksheets[0].Cells[i, 5].Value != null)
                                        {
                                            QB_B = wk.Worksheets[0].Cells[i, 5].Value.ToString().Trim();
                                        }
                                        if (wk.Worksheets[0].Cells[i, 6].Value != null)
                                        {
                                            QB_C = wk.Worksheets[0].Cells[i, 6].Value.ToString().Trim();
                                        }
                                        if (wk.Worksheets[0].Cells[i, 7].Value != null)
                                        {
                                            QB_D = wk.Worksheets[0].Cells[i, 7].Value.ToString().Trim();
                                        }
                                        if (wk.Worksheets[0].Cells[i, 8].Value != null)
                                        {
                                            QB_E = wk.Worksheets[0].Cells[i, 8].Value.ToString().Trim();
                                        }
                                        if (wk.Worksheets[0].Cells[i, 9].Value != null)
                                        {
                                            //传说的答案
                                            //判断多选题 用','分开或者没分开 2多选
                                            if (QB_Type == "2")
                                            {
                                                string STstr = wk.Worksheets[0].Cells[i, 9].Value.ToString().Trim();
                                                string SPSTstr = "";

                                                STstr = STstr.Replace('，', ',');
                                                if (STstr.IndexOf(',') == -1)//不存在
                                                {
                                                    for (var x = 0; x < STstr.Length; x++)
                                                    {
                                                        SPSTstr += STstr[x] + ",";
                                                    }
                                                    QB_Answer = SPSTstr.Substring(0, SPSTstr.Length - 1);

                                                    //
                                                }
                                                else
                                                {
                                                    QB_Answer = STstr;
                                                }

                                                var nx = "A,B,C,D,E,";

                                                var mc = "";
                                                var n1 = QB_Answer.Split(',');
                                                var n2 = nx.Split(',');
                                                for (var h = 0; h < n2.Length; h++)
                                                {

                                                    for (var k = 0; k < n1.Length; k++)
                                                    {
                                                        if (n1[k].Length != 0 && n2[h].Length != 0)
                                                        {
                                                            if (n1[k] == n2[h])
                                                            {
                                                                mc += n2[h] + ",";
                                                            }
                                                        }

                                                    }

                                                }
                                                if (mc.Length > 0)
                                                {
                                                    QB_Answer = mc.Substring(0, mc.Length - 1);
                                                }
                                            }
                                            else
                                            {
                                                QB_Answer = wk.Worksheets[0].Cells[i, 9].Value.ToString().Trim();
                                            }

                                        }
                                        if (wk.Worksheets[0].Cells[i, 10].Value != null)
                                        {
                                            QB_Keyword = wk.Worksheets[0].Cells[i, 10].Value.ToString().Trim();
                                        }


                                        //验证是否 同一试题下重复
                                        //教师和管理员 校验范围不一样
                                        string checkewheres = "";
                                        if (UserType == "1")//管理员
                                        {
                                            //匹配所有系统的题重复
                                            checkewheres += "  and QB_Kind=1";

                                        }
                                        if (UserType == "2")//教师
                                        {
                                            //匹配自己的
                                            checkewheres += "  and QB_AddOperator=" + QB_AddOperator;
                                        }
                                        //同一题型下 题目描述和选项均相同
                                        checkewheres += " and QB_State=1 and QB_Type=" + QB_Type + " and QB_Description='" + QB_Description + "'";
                                        checkewheres += " and QB_A='" + QB_A + "' and QB_B='" + QB_B + "' and QB_C='" + QB_C + "' and QB_D='" + QB_D + "' and QB_E='" + QB_E + "'";

                                        //重复校验
                                        int checekcount = commonbll.GetRecordCount("tb_HB_QuestionBank", checkewheres);
                                        if (checekcount > 0)//存在重复
                                        {

                                        }
                                        else
                                        {

                                            SqlParameter[] pars = new SqlParameter[] 
                                        {
                                            new SqlParameter("@QB_CourseId",QB_CourseId),
                                            new SqlParameter("@QB_Type",QB_Type),
                                            new SqlParameter("@QB_Description",QB_Description),
                                            new SqlParameter("@QB_A",QB_A),
                                            new SqlParameter("@QB_B",QB_B),
                                            new SqlParameter("@QB_C",QB_C),
                                            new SqlParameter("@QB_D",QB_D),
                                            new SqlParameter("@QB_E",QB_E),
                                            new SqlParameter("@QB_Answer",QB_Answer),
                                            new SqlParameter("@QB_Keyword",QB_Keyword),
                                            new SqlParameter("@QB_State",QB_State),
                                            new SqlParameter("@QB_Kind",QB_Kind),
                                            new SqlParameter("@QB_Operator",QB_Operator),
                                            new SqlParameter("@QB_AddOperator",QB_AddOperator),
                                            new SqlParameter("@QB_AddTime",QB_AddTime),
                                            new SqlParameter("@QB_Custom2",QB_Custom2)
                                        };
                                            resultcount = commonbll.Add(table, list, vlaue, pars);
                                            if (resultcount == 1)
                                            {
                                                success++;//成功
                                            }

                                        }

                                        //获取试题Id 插入试卷试题表
                                        string NQuestionBId = commonbll.GetListSclar("QuestionBId", "tb_HB_QuestionBank", checkewheres);

                                        //验证该题是否已经加入过了
                                        var checkcountTo = commonbll.GetRecordCount("tb_HB_ExaminationPapers", " and EP_PId='" + NPid + "' and EP_QBId='" + NQuestionBId + "'");
                                        if (checkcountTo == 0)
                                        {
                                            sqlstr += @"insert into tb_HB_ExaminationPapers values('" + NPid + "','" + NQuestionBId + "','" + Scores + "','" + UId + "','" + UId + "','" + DateTime.Now + "',null,'" + UserNo + "',null);";
                                        }

                                    }

                                }
                                //插入
                                SqlHelper.ExecuteNonQuery(sqlstr);
                                //修改试卷状态 1
                                SqlHelper.ExecuteNonQuery("update tb_HB_Paper set P_State=1 where PId='" + NPid + "'");

                                //循环完事
                                var json = new object[] {
                                new{
                                    success=success,
                                    error=""//异常
                                }
                            };
                                context.Response.Write(JsonConvert.SerializeObject(json));
                                return;


                            }
                            catch (Exception e)
                            {
                                //循环完事
                                var json = new object[] {
                                new{
                                    success=success,
                                    error=e.Message
                                }
                            };
                                context.Response.Write(JsonConvert.SerializeObject(json));
                                return;
                            }
                        }


                    }

                }
                #endregion
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}