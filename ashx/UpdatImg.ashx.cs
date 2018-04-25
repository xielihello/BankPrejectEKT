using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using VocationalProject_Bll;
using VocationalProject_DBUtility;

namespace VocationalProject.ashx
{
    /// <summary>
    /// baiduUpload 的摘要说明
    /// </summary>
    public class UpdatImg : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            string name = Request["name"];
            string opr = Request["opr"];

            string result = string.Empty;
            if (opr == "newupload")
            {
                result = NewUploadImg(Request, name);
            }
            context.Response.Write(result);
        }
        private string NewUploadImg(HttpRequest Request, string filename)
        {
            string resulr = Request["resulr"];
            var UserNo = filename;
            var error = new error { code = "1001", message = "Upload Failed" };
            JsonResultWebUpload jresult = new JsonResultWebUpload("2.0") { result = "error" };
            CommonBll commonbll = new CommonBll();
            SqlParameter[] pars = new SqlParameter[] 
                {

                };
            var resultcount = commonbll.UpdateInfo("tb_UserInfo", "UserPic='" + resulr + "'", " and UserNo='" + UserNo + "'", pars);
            jresult.result = "fail";
            jresult.id = UserNo;
            if (resultcount == 1)
            {
                jresult.result = "success";
                jresult.id = "" + UserNo;
            }
            return JsonConvert.SerializeObject(jresult);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class JsonResultWebUpload
    {
        public JsonResultWebUpload()
        {

        }
        public JsonResultWebUpload(string jsonrpc)
        {
            this.jsonrpc = jsonrpc;

        }
        public string jsonrpc { get; set; }

        public string result { get; set; }

        public error error { get; set; }

        public string id { get; set; }

    }

    public class error
    {
        public string code { get; set; }

        public string message { get; set; }
    }
}