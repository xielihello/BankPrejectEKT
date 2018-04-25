using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using VocationalProject.Controllers;
using VocationalProject_Bll;
using VocationalProject_DBUtility;

namespace VocationalProject.App_Start
{

    public class UploaderCalculators
    {

        //存入任务的队列
        private static Queue<string> _urlQueue;
        private static Queue<string> _typeQueue;
        private static Queue<string> _KcidQueue;
        private static Queue<string> _UserIdQueue;
        private static Queue<string> _imgUrlQueue;
        private static Queue<string> _imgQueue;
        private static ManualResetEvent _hasNew;
        private static bool isFirst = true;

        static UploaderCalculators()
        {
            _urlQueue = new Queue<string>();
            _typeQueue = new Queue<string>();
            _KcidQueue = new Queue<string>();
            _UserIdQueue = new Queue<string>();
            _imgUrlQueue = new Queue<string>();
            _imgQueue = new Queue<string>();
            _hasNew = new ManualResetEvent(false);

            //这里创建一个线程，使其执行Process方法。
            Thread thread = new Thread(Process);

            //设置当前线程为后台线程。
            thread.IsBackground = true;

            //开启线程
            thread.Start();
        }

        private static void Process()
        {
            while (true)
            {
                //等待接受信号，阻塞线程
                _hasNew.WaitOne();

                //接收信号，重置 信号器，信号关闭。
                _hasNew.Reset();

                threadStart();
            }
        }

        private static void threadStart()
        {
            CommonBll comm = new CommonBll();
            BaseController Base = new BaseController();
            while (true)
            {
                if (_urlQueue.Count > 0 && _typeQueue.Count > 0 && _KcidQueue.Count > 0 && _UserIdQueue.Count > 0)
                {
                    try
                    {
                        //文件路径
                        string Url = _urlQueue.Count > 0 ? _urlQueue.Dequeue() : "";//从队列的开始出返回一个对象;
                        //文件后缀名
                        string fileType = _typeQueue.Count > 0 ? _typeQueue.Dequeue() : "";//从队列的开始出返回一个对象;
                        //课程Id
                        string KcId = _KcidQueue.Count > 0 ? _KcidQueue.Dequeue() : "";//从队列的开始出返回一个对象;
                        //操作人Id
                        string UserId = _UserIdQueue.Count > 0 ? _UserIdQueue.Dequeue() : "";//从队列的开始出返回一个对象;
                        //转换图片后的存入路径
                        string imgUrl = _imgUrlQueue.Count > 0 ? _imgUrlQueue.Dequeue() : "";//从队列的开始出返回一个对象;
                        //
                        string img = _imgQueue.Count > 0 ? _imgQueue.Dequeue() : "";//从队列的开始出返回一个对象;
                        PictureConversion pc = new PictureConversion();

                        string imgName = string.Empty;
                        if (fileType == "docx" || fileType == "doc")
                        {
                            imgName = pc.ConvertToImage_Words(Url, imgUrl, 0, 0, null, 145, img);
                        }
                        if (fileType == "pptx" || fileType == "xlsx" || fileType == "xls" || fileType == "excel" || fileType == "excelx" || fileType == "ppt")
                        {
                            imgName = pc.ConvertToImage_PPT(Url, imgUrl, 0, 0, 145, fileType, img);
                        }
                        if (fileType == "pdf")
                        {
                            imgName = pc.ConvertToImage_PDF(Url, imgUrl, 0, 0, 145, img);
                        }
                        if (imgName != null && imgName != "")
                        {
                            comm.DeleteInfo("ykt_PptTransferMap", " and CourseId=" + KcId);
                            string listtwo = @"[CourseId],[UserId],[PictureUrl],[Spare1],[Spare2],[Spare3]";
                            string vlauetwo = "@CourseId,@UserId,@PictureUrl,@Spare1,@Spare2,@Spare3";
                            SqlParameter[] parstwo = new SqlParameter[] 
                                                        {
                                                            new SqlParameter("@CourseId",KcId),
                                                            new SqlParameter("@UserId",UserId),
                                                            new SqlParameter("@PictureUrl",imgName),
                                                            new SqlParameter("@Spare1",null),
                                                            new SqlParameter("@Spare2",null),
                                                            new SqlParameter("@Spare3",null)
                                                        };
                            Convert.ToInt32(comm.AddIdentity("ykt_PptTransferMap", listtwo, vlauetwo, parstwo));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                    }
                }
            }
        }


        public static void Add(string Url, string Type, string KcId, string UserId, string imgUrl, string img)
        {
            _urlQueue.Enqueue(Url);
            _typeQueue.Enqueue(Type);
            _KcidQueue.Enqueue(KcId);
            _UserIdQueue.Enqueue(UserId);
            _imgUrlQueue.Enqueue(imgUrl);
            _imgQueue.Enqueue(img);
            if (isFirst)
            {
                isFirst = false;
                _hasNew.Set();
            }
        }


        
    }

}