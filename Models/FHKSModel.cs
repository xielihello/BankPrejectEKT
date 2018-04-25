using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocationalProject.Models
{
    /// <summary>
    /// 复核
    /// </summary>
    public class FHKSModel
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public string CurrentTime { get; set; }//当前时间
        /// <summary>
        /// 进入时 考试开始时间
        /// </summary>
        public string AddStartDateTime { get; set; }//进入时 考试开始时间
        /// <summary>
        /// +时长 考试结束时间
        /// </summary>
        public string TestStartDateTime { get; set; }//+时长 考试结束时间
        /// <summary>
        /// 有效开始时间
        /// </summary>
        public string E_StartTime { get; set; }//有效开始时间
        /// <summary>
        /// 有效结束时间
        /// </summary>
        public string E_EndTime { get; set; }//有效结束时间
        /// <summary>
        /// 时长
        /// </summary>
        public string E_Whenlong { get; set; }//时长
        /// <summary>
        /// 比赛类型
        /// </summary>
        public string E_Type { get; set; }//比赛类型

        /// <summary>
        /// 试卷分值
        /// </summary>
        public string Score { get; set; }//试卷分值
        /// <summary>
        /// 试卷名称
        /// </summary>
        public string E_Name { get; set; }//试卷名称
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }//用户名
        /// <summary>
        /// 头像
        /// </summary>
        public string UserPic { get; set; }//头像
        /// <summary>
        /// 学校
        /// </summary>
        public string SchoolName { get; set; }//学校

        /********考试分数竞赛资料*********/
        /// <summary>
        /// 是否允许做
        /// </summary>
        public string Isallow { get; set; }//是否允许做
        /// <summary>
        /// 班级名称
        /// </summary>
        public string TeamName { get; set; }//班级名称
        /// <summary>
        /// 登录账号
        /// </summary>
        public string UserNo { get; set; }//登录账号
        /// <summary>
        /// 考试成绩
        /// </summary>
        public string ER_Score { get; set; }//考试成绩
        /// <summary>
        /// 是否加分
        /// </summary>
        public string IsPlus { get; set; }//考试成绩
        /// <summary>
        /// 用户id
        /// </summary>
        public string MId { get; set; }//
    }
}