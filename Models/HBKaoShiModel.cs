using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocationalProject.Models
{
    /// <summary>
    /// 货币知识 考试做题 参数
    /// </summary>
    public class HBKaoShiModel
    {
        public string CurrentTime { get; set; }//当前时间
        public string AddStartDateTime { get; set; }//进入时 考试开始时间
        public string TestStartDateTime { get; set; }//+时长 考试结束时间
        public string E_StartTime { get; set; }//有效开始时间
        public string E_EndTime { get; set; }//有效结束时间
        public string E_Whenlong { get; set; }//时长
        public string E_Type { get; set; }//比赛类型
        public string E_IsTimeBonus { get; set; }//是否允许时间加分

        public string Score { get; set; }//试卷分值
        public string E_Name { get; set; }//试卷名称
        public string UserName { get; set; }//用户名
        public string UserPic { get; set; }//头像
        public string SchoolName { get; set; }//学校

        /********考试分数竞赛资料*********/
        public string Isallow { get; set; }//是否允许做
        public string TeamName { get; set; }//班级名称
        public string UserNo { get; set; }//登录账号
        public string ER_Score { get; set; }//考试成绩

    }
}