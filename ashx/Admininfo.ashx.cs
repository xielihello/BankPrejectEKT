using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using VocationalProject_Bll;
using VocationalProject_Models;
using System.Web.SessionState;
using System.IO;
using System.Drawing;

namespace VocationalProject.ashx
{
    /***************************************************************
    FileName:管理员信息修改
    Copyright（c）2017-金融教育在线技术开发部
    Author:柯思金
    Create Date:2017-4-12
   ******************************************************************/
    public class Admininfo : IHttpHandler, IRequiresSessionState
    {
        tb_UserInfo userinfo = new tb_UserInfo();
        CommonBll commonbll = new CommonBll();
        public void ProcessRequest(HttpContext context)
        {
           
            var UserNo = context.Request["UID"];
            string action = context.Request["action"];
            string Uname = context.Request["name"];
            string Pwd = context.Request["Upwd"];
            string img = context.Request["img"];
            string diyImg = context.Request["diyImg"];
            string sex = context.Request["sex"];
            string phone = context.Request["phone"];
            string email = context.Request["email"];
                        string File_Send = "";
            try
            {
                switch (action)
                {
                    #region 管理员信息
                    //信息初始化
                    case "loadinfo":
                        //类型：全部订单、已支付、未支付           
                        DataTable dt = commonbll.GetListDatatable("UserNo,UserPwd,UserName,UserPic", "tb_UserInfo", " and UserNo='" + UserNo + "'");
                        JsonConvert.SerializeObject(dt);
                        context.Response.Write(dt);
                        break;
                    //教师信息修改
                    case "updateinfo1": 
                        if (diyImg == "1")
                        {
                            File_Send = context.Request["ImgUrl"];   
                            //HttpPostedFile hpFile = context.Request.Files[0];
                            //string filePath = "/img";
                            ////获得上传上来的文件名称  
                            //string fileName = System.IO.Path.GetFileName(hpFile.FileName);
                            //File_Send = "/img/" + fileName;
                            //filePath = context.Server.MapPath(filePath + "/" + fileName);
                            ////将上传来的 文件数据 保存在 对应的 物理路径上  
                            //hpFile.SaveAs(filePath);
                            userinfo.UserName = Uname;
                            userinfo.UserPic = File_Send;
                            userinfo.UserSex = sex;
                            userinfo.UserPhone = phone;
                            userinfo.UserEmail = email;
                            userinfo.UserNo = UserNo;
                            SqlParameter[] pars = new SqlParameter[]{
                        new SqlParameter("@UserName",userinfo.UserName),
                        new SqlParameter("@UserPic",userinfo.UserPic)};
                            commonbll.UpdateInfo("tb_UserInfo", "UserName='" + userinfo.UserName + "'" + ",UserPic='" + userinfo.UserPic + "'" + ",UserSex='" + userinfo.UserSex + "'" + ",UserPhone='" + userinfo.UserPhone + "'" + ",UserEmail='" + userinfo.UserEmail + "'", " and UserNo='" + UserNo + "'", pars);
                            context.Response.Write(File_Send);
                           
                        }
                        else 
                        {
                            userinfo.UserName = Uname;
                           // userinfo.UserPic = File_Send;
                            userinfo.UserSex = sex;
                            userinfo.UserPhone = phone;
                            userinfo.UserEmail = email;
                            userinfo.UserNo = UserNo;
                            SqlParameter[] pars = new SqlParameter[]{
                        new SqlParameter("@UserName",userinfo.UserName)
                        };
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserName='" + userinfo.UserName + "'" + ",UserSex='" + userinfo.UserSex + "'" + ",UserPhone='" + userinfo.UserPhone + "'" + ",UserEmail='" + userinfo.UserEmail + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        // return count;
                        break;
                    //修改信息
                    case "updateinfo":
                        if (diyImg == "1")
                        {
                            File_Send = context.Request["ImgUrl"];   
                            //HttpPostedFile hpFile = context.Request.Files[0];
                            //string filePath = "/img";
                            ////获得上传上来的文件名称  
                            //string fileName = System.IO.Path.GetFileName(hpFile.FileName);
                            //Base64StringToImage(hpFile.FileName);
                            //File_Send = "/img/" + fileName;
                            //filePath = context.Server.MapPath(filePath + "/" + fileName);
                            //将上传来的 文件数据 保存在 对应的 物理路径上  
                            //hpFile.SaveAs(filePath);
                            userinfo.UserName = Uname;
                            userinfo.UserPic = File_Send;
                            userinfo.UserNo = UserNo;
                            SqlParameter[] pars = new SqlParameter[]{
                        new SqlParameter("@UserName",userinfo.UserName),
                        new SqlParameter("@UserPic",userinfo.UserPic)};
                            commonbll.UpdateInfo("tb_UserInfo", "UserName='" + userinfo.UserName + "'" + ",UserPic='" + userinfo.UserPic + "'", " and UserNo='" + UserNo + "'", pars);
                            context.Response.Write(File_Send);
                        }
                        else 
                        {
                            userinfo.UserName = Uname;
                          //  userinfo.UserPic = File_Send;
                            userinfo.UserNo = UserNo;
                            SqlParameter[] pars = new SqlParameter[]{
                        new SqlParameter("@UserName",userinfo.UserName)
                        };
                            
                            context.Response.Write(commonbll.UpdateInfo("tb_UserInfo", "UserName='" + userinfo.UserName + "'", " and UserNo='" + UserNo + "'", pars));
                        }
                        // return count;
                        break;
                    //修改密码
                    case "UpdateP":
                        userinfo.UserPwd = Pwd;
                        userinfo.UserNo = UserNo;
                        SqlParameter[] pars1 = new SqlParameter[]{
                        new SqlParameter("@UserPwd",userinfo.UserPwd)};
                        //  new SqlParameter("@UserPic",userinfo.UserPic)};
                        commonbll.UpdateInfo("tb_UserInfo", "UserPwd='" + userinfo.UserPwd + "'", " and UserNo='" + UserNo + "'", pars1);
                        context.Response.Write(Pwd);
                        break;
                    #endregion


                }

            }
            catch
            {
                return ;
            }
        }
        
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        //base64编码的文本 转为    图片
        private void Base64StringToImage(string txtFileName)
        {
            try
            {
                FileStream ifs = new FileStream(txtFileName, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(ifs);

                String inputStr = sr.ReadToEnd();
                byte[] arr = Convert.FromBase64String(inputStr);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);

                //bmp.Save(txtFileName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmp.Save(txtFileName + ".bmp", ImageFormat.Bmp);
                //bmp.Save(txtFileName + ".gif", ImageFormat.Gif);
                //bmp.Save(txtFileName + ".png", ImageFormat.Png);
                ms.Close();
                sr.Close();
                ifs.Close();
               // this.pictureBox2.Image = bmp;
                if (File.Exists(txtFileName))
                {
                    File.Delete(txtFileName);
                }
                //MessageBox.Show("转换成功！");
            }
            catch (Exception ex)
            {
               // MessageBox.Show("Base64StringToImage 转换失败\nException：" + ex.Message);
            }
        }
    }
}