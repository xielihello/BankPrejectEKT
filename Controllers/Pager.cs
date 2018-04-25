using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocationalProject.Areas.Admin.Models;
using VocationalProject.Controllers;
using VocationalProject_DBUtility.Error;
using VocationalProject_DBUtility.Sql;

namespace VocationalProject_Dal
{
    public static class Pager
    {

        /// <summary>
        /// 公用分页存储过程
        /// </summary>
        /// <returns></returns>
        public static DataTable GetList(PageModel m, ref int PageCount)
        {
            SqlParameter[] parameters = {
                                            new SqlParameter("@tab", SqlDbType.VarChar, 4000),
                                            new SqlParameter("@strFld", SqlDbType.VarChar, 4000),
                                            new SqlParameter("@strWhere", SqlDbType.VarChar, 4000),
                                            new SqlParameter("@PageIndex ", SqlDbType.Int, 4),
                                            new SqlParameter("@PageSize", SqlDbType.Int, 4),
                                            new SqlParameter("@Sort", SqlDbType.VarChar, 4000),
                                            new SqlParameter("@RecordCount", SqlDbType.Int, 4),      
                                            new SqlParameter("@IsGetCount", SqlDbType.Bit) 
                                        };

            parameters[0].Value = m.tab;
            parameters[1].Value = m.strFld;
            parameters[2].Value = String.IsNullOrEmpty(m.strWhere) ? " 1=1 " : " 1=1 " + m.strWhere;
            parameters[3].Value = m.PageIndex;
            parameters[4].Value = m.PageSize;
            parameters[5].Value = m.Sort; 
            parameters[6].Value = 0;
            parameters[6].Direction = ParameterDirection.Output;
            parameters[7].Value = false;//m.IsGetCount;
            var tb = new DataTable();
            try
            {
                tb = SqlHelper.ExecuteDataTable("proc_Common_PageList ", CommandType.StoredProcedure, parameters);

                PageCount = parameters[6].Value == DBNull.Value ? 0 : Convert.ToInt32(parameters[6].Value);


            }
            catch (Exception e)
            {
                ErrorHandler.WriteError(e);
                throw;
            }
            return tb;//DataTable_List.ConvertTo<TModel>(tb); ;

        }
    }
}
