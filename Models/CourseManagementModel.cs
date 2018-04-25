using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocationalProject.Models
{
    public class CourseManagementModel
    {
        /// <summary>
        /// 课程Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }


        /// <summary>
        /// 上传人
        /// </summary>
        public int? UploadName { get; set; }


        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime? UploadTime { get; set; }

        /// <summary>
        /// 课程存放路径
        /// </summary>
        public string CourseUrl { get; set; }

        /// <summary>
        /// 封面图片存放路径
        /// </summary>
        public string CourseCoverUrl { get; set; }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public int? Jurisdiction { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int? FileSize { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int? IsDelete { get; set; }

    }
}