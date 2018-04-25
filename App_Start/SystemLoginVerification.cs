using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using VocationalProject_Bll;
using VocationalProject_Models;

namespace VocationalProject.App_Start
{
    public class SystemLoginVerification : ActionFilterAttribute
    {

        CommonBll commonbll = new CommonBll();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var fcinfo = new filterContextInfo(filterContext);
            string areaName = filterContext.RequestContext.RouteData.DataTokens["area"] == null ?
                             "" : filterContext.RequestContext.RouteData.DataTokens["area"].ToString();

            if (HttpContext.Current.Session["UserInfo"] == null || HttpContext.Current.Session["UserType"] == null)
            {  
                    if (isUserAgent(filterContext))
                    {

                        filterContext.Result = new RedirectResult("/login");
                        return;
                    }

            }
            var UserType = HttpContext.Current.Session["UserType"].ToString();
            if (areaName.ToLower() == "admin")
            {
                if (UserType == "3" || UserType == "4")
                {
                    if (UserType == "3")
                    {
                        var m = HttpContext.Current.Session["UserInfo"] as tb_UserInfo;
                        var tbSchool = commonbll.GetListDatatable("*", "tb_School", " and Id=" + m.UserSchoolId);
                        if (isUserAgent(filterContext))
                        {
                            string url = "/HB_Competition";
                            if (tbSchool != null && tbSchool.Rows.Count > 0)
                            {
                              string Jurisdiction = tbSchool.Rows[0]["Jurisdiction"].ToString();
                              if (Jurisdiction.Contains("1"))
                              {
                                  url = "/HB_Competition";
                              } 
                              else if (Jurisdiction.Contains("2"))
                              {
                                  url = "/SG_Competition";
                              }
                              else if (Jurisdiction.Contains("3"))
                              {
                                  url = "/FH_Management";
                              }
                              else
                              {
                                  url = "/Bill_Competition/GrowthProcess";
                              }
                            }
                            filterContext.Result = new RedirectResult(url);
                            return;
                        }
                    }
                    else
                    {  
                        if (isUserAgent(filterContext))
                        {
                            filterContext.Result = new RedirectResult("/HB_Competition");
                            return;
                        }
                    }
                }
                return;
            }
            //if (areaName.ToLower() != "admin")
            //{
            //    if (UserType == "1" || UserType == "2")
            //    {
            //        if (isUserAgent(filterContext))
            //        {
            //            if (UserType == "1")
            //            {
            //                filterContext.Result = new RedirectResult("/Admin/CollegeManagement/College");
            //                return;
            //            }
            //            else
            //            {
            //                filterContext.Result = new RedirectResult("/Admin/T_StudentManage");
            //                return;

            //            }
            //        }
            //    }
            //    return;

            //}


            //else if (HttpContext.Current.Session["H_LoginUser"] == null && (areaName.ToLower() == "admin" || areaName.ToLower() == "Admin"))
            //{
            //    //if ()
            //    //{ 
            //    filterContext.Result = new RedirectResult("/admin/login");

            //    return;
            //    // }
            //}

            //if (areaName.ToLower() != "admin" && areaName.ToLower() != "Admin")
            //{
            //    if (Convert.ToString(HttpContext.Current.Session["CIsAdmin"]) == "1")//判断是否超级管理员
            //    {
            //        //
            //        filterContext.Result = new RedirectResult("/Admin/Home/");
            //        return;
            //    }
            //} 
            //验证是否是WAP手机端的
            //if (!isUserAgent(filterContext))
            //{
            //    filterContext.RequestContext.RouteData.DataTokens["area"] = "wap";
            //}

            //base.OnActionExecuting(filterContext);
        }
        /// <summary>
        /// 验证是否是WAP手机端的
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool isUserAgent(ActionExecutingContext filterContext)
        {
            string u = filterContext.HttpContext.Request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!(b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
            {
                return true;//电脑
            }
            return false;
        }
    }

     public class filterContextInfo
    {
        public filterContextInfo(ActionExecutingContext filterContext)
        {
            #region 获取链接中的字符
            // 获取域名
            domainName = filterContext.HttpContext.Request.Url.Authority;

            //获取模块名称
            //  module = filterContext.HttpContext.Request.Url.Segments[1].Replace('/', ' ').Trim();

            //获取 controllerName 名称
            controllerName = filterContext.RouteData.Values["controller"].ToString();

            //获取ACTION 名称
            actionName = filterContext.RouteData.Values["action"].ToString();

            #endregion
        }
        /// <summary>
        /// 获取域名
        /// </summary>
        public string domainName { get; set; }
        /// <summary>
        /// 获取模块名称
        /// </summary>
        public string module { get; set; }
        /// <summary>
        /// 获取 controllerName 名称
        /// </summary>
        public string controllerName { get; set; }
        /// <summary>
        /// 获取ACTION 名称
        /// </summary>
        public string actionName { get; set; }

    }
 
}