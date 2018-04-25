
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Configuration;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Reflection;
using VocationalProject.App_Start;
using VocationalProject.Areas.Admin.Models; 
using VocationalProject_Bll; 
using VocationalProject_Models;   

namespace VocationalProject.Controllers
{
    [SystemLoginVerification]
    public class BaseController : Controller
    {
        CommonBll commonbll = new CommonBll();
        #region 分页
        /// <summary>
        /// 分页  
        /// </summary>
        /// <typeparam name="TModel">类型</typeparam>
        /// <param name="total">总共条数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="model">IEnumerable<TModel></param>
        /// <returns></returns>
        protected JsonResult JsonResultPagedList(int total, int pageIndex, int PageSize, string Html)
        {
            PageListModel m = new PageListModel();
            m.Total = total;
            m.PageIndex = pageIndex;
            m.PageTotal = total % PageSize == 0 ? (total / PageSize) : (total / PageSize) + 1;
            m.PageSize = PageSize;
            m.TableHTML = Html;
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            json.Data = m;
            return json;
        }/// <summary>
        /// 分页  
        /// </summary>
        /// <typeparam name="TModel">类型</typeparam>
        /// <param name="total">总共条数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="model">IEnumerable<TModel></param>
        /// <returns></returns>
        protected JsonResult JsonResultPagedList(int total, int pageIndex, int PageSize, DataTable tb)
        {
            PageListModel m = new PageListModel();
            m.Total = total;
            m.PageIndex = pageIndex;
            m.PageTotal = total % PageSize == 0 ? (total / PageSize) : (total / PageSize) + 1;
            m.PageSize = PageSize;
            m.Tb = tb;
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            json.Data = m;
            return json;
        }
        /// 分页  
        /// </summary>
        /// <typeparam name="TModel">类型</typeparam>
        /// <param name="total">总共条数</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="model">IEnumerable<TModel></param>
        /// <returns></returns>
        protected PageListModel JsonResultPagedLists(int total, int pageIndex, int PageSize, DataTable tb)
        {
            PageListModel m = new PageListModel();
            m.Total = total;
            m.PageIndex = pageIndex;
            m.PageTotal = total % PageSize == 0 ? (total / PageSize) : (total / PageSize) + 1;
            m.PageSize = PageSize;
            m.Tb = tb;

            return m;
        }
        /// <summary>
        /// 返回
        /// </summary>
        /// <typeparam name="TModel"></typeparam> 
        /// <returns></returns>
        protected JsonResult PhotosJsonResult<TModel>(IEnumerable<TModel> model)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            json.Data = model;
            return json;
        }

        protected DataTable PrepareActionResultPageSize(int totalRecode, int pageIndex, int PageSize, DataTable data)
        {
            ViewBag.Total = totalRecode;
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageTotal = totalRecode % PageSize == 0 ? (totalRecode / PageSize) : (totalRecode / PageSize) + 1;
            return data;
        }


        protected JsonResult PrepareJsonResultPageSize<TModel>(int totalRecode, int pageIndex, int PageSize, IEnumerable<TModel> model, object groups = null)
        {
            var data = new
            {
                Total = totalRecode,
                PageIndex = pageIndex,
                PageTotal = totalRecode % PageSize == 0 ? (totalRecode / PageSize) : (totalRecode / PageSize) + 1,
                Data = model,
                Groups = groups,
            };

            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            json.Data = data;

            return json;

            //return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 字典

        #endregion

        #region 防sql注入

        public bool ProcessSqlStr(string inputString)
        {
            string SqlStr = @"and|or|exec|execute|insert|select|delete|update|alter|create|drop|count|\*|chr|char|asc|mid|substring|master|truncate|declare|xp_cmdshell|restore|backup|net +user|net +localgroup +administrators";
            try
            {
                if ((inputString != null) && (inputString != String.Empty))
                {
                    string str_Regex = @"\b(" + SqlStr + @")\b";

                    Regex Regex = new Regex(str_Regex, RegexOptions.IgnoreCase);
                    //string s = Regex.Match(inputString).Value; 
                    if (true == Regex.IsMatch(inputString))
                        return false;

                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 是否相同的记录
        public bool GetCount(string tab, string where)
        {
            int Result = commonbll.GetRecordCount(tab, where);
            if (Result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #endregion

        #region 防止重复提交 及 导出excel
        /// <summary>
        /// 防止重复提交:false 重复提交
        /// </summary>
        /// <returns></returns>
        public bool IsTimeStamp()
        {
            DateTime timeStamp;

            if (Session["TimeStamp"] != null)
            {
                DateTime.TryParse(Session["TimeStamp"].ToString(), out timeStamp);
                TimeSpan ts = DateTime.Now - timeStamp;

                if (ts.TotalMilliseconds < 500)
                {
                    return false;
                }
            }
            Session["TimeStamp"] = DateTime.Now;

            return true;
        }

        [HttpPost]
        public ActionResult ExportExcelHelp(FormCollection form, string FileName)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<html xmlns:x=\"urn:schemas-microsoft-com:office:excel\">");
            sb.Append(" <head>");
            sb.Append(" <!--[if gte mso 9]><xml>");
            sb.Append("<x:ExcelWorkbook>");
            sb.Append("<x:ExcelWorksheets>");
            sb.Append("<x:ExcelWorksheet>");
            sb.Append("<x:Name></x:Name>");
            sb.Append("<x:WorksheetOptions>");
            sb.Append("<x:Print>");
            sb.Append("<x:ValidPrinterInfo />");
            sb.Append(" </x:Print>");
            sb.Append("</x:WorksheetOptions>");
            sb.Append("</x:ExcelWorksheet>");
            sb.Append("</x:ExcelWorksheets>");
            sb.Append("</x:ExcelWorkbook>");
            sb.Append("</xml>");
            sb.Append("<![endif]-->");
            sb.Append(" </head>");
            sb.Append("<body>");

            string htmlt = "</body></html>";
            string strHtml = sb + form["hHtml"] + htmlt;

            strHtml = HttpUtility.HtmlDecode(strHtml);//Html解码 

            byte[] b = System.Text.Encoding.UTF8.GetBytes(strHtml);//字串转byte阵列 

            return File(b, "application/vnd.ms-excel", FileName + "[" + DateTime.Now.ToString("yyyy/MM/dd") + "].xls");//输出档案给Client端
        }
        #endregion


        #region 公用方法
        //        /// <summary>
        //        /// 根据用户查询
        //        /// </summary>
        //        /// <param name="userId"></param>
        //        /// <param name="UserType"></param>
        //        /// <returns></returns>
        //        public List<TerminalModel> GetTerminalIdByUser(int userId, int Type)
        //        {
        //            StringBuilder sql = new StringBuilder() { };
        //            if (!string.IsNullOrEmpty(userId.ToString()))
        //            {
        //                switch (Type)
        //                {
        //                    case 0://商户用户
        //                        sql.Append(string.Format(@" SELECT D.* from tb_departmentusers A 
        //                                                    INNER JOIN tb_departmentproject B ON A.DepartmentId = B.DepartmentId and  A.UsersId = {0}
        //                                                    INNER JOIN tb_parkproject C ON  B.ProjectId = C.Id
        //                                                    INNER JOIN tb_merchantterminal D ON D.ProjectCode = C.`Code`
        //                                                    ", userId));

        //                        break;
        //                    case 1://部门用户
        //                        sql.Append(string.Format(@" select C.* from tb_usersproject A 
        //                                                INNER JOIN tb_parkproject B ON A.ProjectId = B.Id AND A.UsersId = {0}
        //                                                INNER JOIN tb_merchantterminal C on C.ProjectCode = B.`Code` and C.IsDelete = 0
        //                                                ", userId));
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }

        //            DataTable dt = Shove.Database.MySQL.Select(sql.ToString());
        //            var list = new List<TerminalModel>();
        //            if (dt != null && dt.Rows.Count > 0)
        //            {

        //                foreach (DataRow item in dt.Rows)
        //                {
        //                    TerminalModel m = new TerminalModel();

        //                    m.Id = Int32.Parse(item["Id"].ToString());
        //                    m.TerminalId = item["TerminalId"].ToString();
        //                    m.MerchantCode = item["MerchantCode"].ToString();
        //                    m.Position = item["Position"].ToString();
        //                    list.Add(m);
        //                }
        //            }

        //            return list;
        //        }


        #endregion


        ///// <summary>
        ///// 移除接口缓存
        ///// </summary>
        ///// <param name="MerchantCode">商户编号</param>
        ///// <param name="Type">清除类型：1、产品 2、商户信息</param>
        //public string RemoClearCache(int Type, string MerchantCode)
        //{
        //    try
        //    {
        //        PayWebReference.MobileTicket ws = new PayWebReference.MobileTicket();
        //        var urlzu = ConfigurationManager.AppSettings["PayWebRefeUrl"].ToString().Split(',');// "PayWebRefeUrl";
        //        if (urlzu.Length > 0)
        //        {
        //            for (int i = 0; i < urlzu.Length; i++)
        //            {
        //                if (!string.IsNullOrEmpty(urlzu[i].ToString()))
        //                {
        //                    ws.Url = urlzu[i].ToString();
        //                    ws.ClearCache(Type, MerchantCode);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return "接口连接出错！";
        //    }
        //    return "OK";
        //    //http://localhost:23158/MobileTicket.asmx

        //}


        ///// <summary>
        ///// 退款接口
        ///// </summary>
        //public string RefundConfigurationStr
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["RefundPayWebRefeUrl"].ToString();//RefundPayWebRefeUrl
        //    }
        //}






        #region session实体 


        //public UserModel Base_LoginUser
        //{
        //    get
        //    {
        //        return this.Session["LoginUser"] as UserModel;
        //    }
        //}  

        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType
        {
            get
            {
                return this.Session["UserType"].ToString();
            }
        }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UId
        {
            get
            {
                var m = this.Session["UserInfo"] as tb_UserInfo;
                return m.UId.ToString() ;
            }
        }
        /// <summary>
        /// 用户UserNo
        /// </summary>
        public string UserNo
        {
            get
            {
                var m = this.Session["UserInfo"] as tb_UserInfo;
                return m.UserNo.ToString();
            }
        }
        /// <summary>
        /// 用户TeacherSchoolId
        /// </summary>
        public string TeacherSchoolId
        {
            get
            {
                var m = this.Session["UserInfo"] as tb_UserInfo;
                return m.UserSchoolId.ToString();
            }
        }
        /// <summary>
        /// 用户UserClassId
        /// </summary>
        public string UserClassId
        {
            get
            {
                var m = this.Session["UserInfo"] as tb_UserInfo;
                return m.UserClassId.ToString();
            }
        }
        /// <summary>
        /// 用户实体
        /// </summary>
        public tb_UserInfo Base_UserInfo
        {
            get
            {
                return this.Session["UserInfo"] as tb_UserInfo;
            }
        }  

        #endregion



        #region  MD5





        #endregion




        #region 其他
        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <param name="isTop">是否改变为返回该类、枚举类型的头 Description 属性，而不是当前的属性或枚举变量值的 Description 属性</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string GetDescription(object obj)
        {
            bool isTop = false;
            if (obj == null)
            {
                return string.Empty;
            }
            try
            {
                Type _enumType = obj.GetType();
                DescriptionAttribute dna = null;
                if (isTop)
                {
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(_enumType, typeof(DescriptionAttribute));
                }
                else
                {
                    FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, obj));
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(
                       fi, typeof(DescriptionAttribute));
                }
                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                    return dna.Description;
            }
            catch
            {
            }
            return obj.ToString();
        }

        ///// <summary>
        ///// 短信模版内容
        ///// </summary>
        ///// <param name="TContent">模板内容</param>
        ///// <param name="Content">短信参数值</param>
        ///// <returns>完整短信内容</returns>
        //public static string ShowShortContent(string TContent, string Content)
        //{
        //    string Str = "";

        //    //解析json
        //    SendSMS sms = Common.JsonDeserialize<SendSMS>(Content);
        //    PropertyInfo[] properties = sms.GetType().GetProperties();
        //    //替换字段  得到字段的数量 逐个替换
        //    foreach (PropertyInfo p in properties)
        //    {
        //        // Console.WriteLine("Name:{0} ", p.Name, p.GetValue(sms, null));
        //        string name = p.Name;
        //        string value = "";
        //        try
        //        {
        //            value = p.GetValue(sms, null).ToString();
        //        }
        //        catch { }
        //        TContent = TContent.Replace("${" + name.ToLower() + "}", value);
        //    }
        //    Str = TContent;
        //    return Str;
        //}

        ///// <summary>
        ///// 订单短信发送情况
        ///// </summary>
        ///// <param name="OrderNO">订单号</param>
        ///// <returns>0 没有失败信息， 1 有失败信息</returns>
        //public static int SmsState(string OrderNO)
        //{
        //    int num = 0;
        //    try
        //    {
        //        DataTable tb = Shove._Web.Cache.GetCacheAsDataTable("ASDFGHJKLDQW");

        //        string select = "select * from tb_shortmessage where OrderNO='" + OrderNO + "' ";
        //        if (tb == null || !string.IsNullOrEmpty(select))
        //        {
        //            tb = MySQL.Select(select.ToString());

        //            Shove._Web.Cache.SetCache("ASDFGHJKLDQW", tb);
        //        }
        //        foreach (DataRow item in tb.Rows)
        //        {
        //            num = 0;
        //            ShortmessageModel m = new ShortmessageModel();
        //            m.Id = Int32.Parse(item["Id"].ToString());
        //            if (Int32.Parse(item["State"].ToString()) == 3)
        //            {
        //                num++;
        //            }
        //            if (Int32.Parse(item["State"].ToString()) == 2)
        //            {
        //                num = -1;
        //                break;
        //            }
        //        }
        //    }
        //    catch { }
        //    return num;
        //}

        ///// <summary>
        ///// 得到发送短信条数
        ///// </summary>
        ///// <param name="count">短信字符数量</param>
        ///// <param name="index">每条短信字符数量</param>
        ///// <returns></returns>
        //public static int Numberlength(int count, int index)
        //{
        //    int scount = 0;
        //    int num1 = count % index;

        //    if (num1 > 0)
        //    {
        //        int num2 = (count - num1) / index;
        //        scount = num2 + 1;
        //    }
        //    else
        //    {
        //        scount = (count - num1) / index;
        //    }
        //    return scount;
        //}
        ///// <summary>
        ///// 修改短信状态
        ///// </summary>
        ///// <param name="OrderNo">订单号</param>
        ///// <returns></returns>
        //public static bool UpdateshortState(string OrderNo)
        //{
        //    bool flag = false;
        //    string str = "update tb_shortmessage set State=0 where  OrderNo='" + OrderNo + "' and State=3;";
        //    var tb = Shove.Database.MySQL.ExecuteNonQuery(str);
        //    if (tb >= 0)
        //    {
        //        flag = true;
        //    }
        //    return flag;
        //}
        #endregion
    }
}

