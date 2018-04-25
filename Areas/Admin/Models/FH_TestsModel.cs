using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VocationalProject.Areas.Admin.Models
{
    public class FH_TestsModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 题目名称
        /// </summary>
        public string Assets { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// 类型0 资产、1 负资产
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Isinsert1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? Isinsert2 { get; set; }
        /// <summary>
        /// 期末
        /// </summary>
        public decimal? Final { get; set; }
        /// <summary>
        /// 期初
        /// </summary>
        public decimal? Beginning { get; set; } 
    }
    public class FH_TestsModels
    {
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Score { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? type { get; set; }
    }
}