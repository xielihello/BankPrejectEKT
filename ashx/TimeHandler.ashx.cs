using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocationalProject.ashx
{
    /// <summary>
    /// TimeHandler 的摘要说明
    /// </summary>
    public class TimeHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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