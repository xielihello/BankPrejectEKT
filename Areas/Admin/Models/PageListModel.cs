using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace VocationalProject.Areas.Admin.Models
{
    public class PageListModel
    {
        /// <summary>
        /// 总共条数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 每页记录数 
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页数 
        /// </summary>
        public int PageIndex { get; set; } 
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageTotal { get; set; }
        /// <summary>
        /// 拼接html
        /// </summary>
        public string TableHTML { get; set; }
        /// <summary>
        /// DataTable
        /// </summary>
        public DataTable Tb { get; set; }
       

    }
    public class PageModel
    { 
        /// <summary>
        /// 每页容纳的记录数 
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码 
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string tab { get; set; }
        /// <summary>
        /// 字段字符串
        /// </summary>
        public string strFld { get; set; }
        /// <summary>
        /// where条件 
        /// </summary>
        public string strWhere { get; set; } 
        /// <summary>
        /// 排序字段及规则,不用加order by
        /// </summary>
        public string Sort { get; set; } 
        /// <summary>
        /// 是否得到记录总数，1为得到记录总数，0为不得到记录总数，返回记录集(是否分页)暂不处理
        /// </summary>
        public bool IsGetCount { get; set; }

    }
}