using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace VocationalProject.ashx
{
    /***************************************************************
  FileName:导出 传值路径
  Copyright（c）2017-金融教育在线技术开发部
  Author:袁学
  Create Date:2017-3-30
 ******************************************************************/
    public class download : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string downUrl = context.Request.QueryString["downurl"];//路径
                context.Response.Buffer = true;
                context.Response.Clear();
                context.Response.ContentType = "application/octet-stream";
                string downFile = ""; //记录下载文件的名称 

                var strtt = downUrl;
                downFile = strtt.Substring(strtt.LastIndexOf('/') + 1);
                //downFile = downFile.Substring(0, downFile.Length - 1);
                string EncodeFileName = HttpUtility.UrlEncode(downFile, System.Text.Encoding.UTF8);//防止中文出现乱码
                context.Response.AddHeader("Content-Disposition", "attachment;filename=" + EncodeFileName + ";");
                string name = context.Server.MapPath(downUrl);
                byte[] bytes = System.IO.File.ReadAllBytes(name);
                context.Response.BinaryWrite(bytes);//返回文件数据给客户端下载

            }
            catch
            {

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