using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace BFNApi.Util
{
    public class Helper
    {
        private object[,] sp_parameters;
        public string CRMconn = ConfigurationManager.AppSettings["CRMconn"].ToString();
        public static string RootPath = ConfigurationManager.AppSettings["RootPath"].ToString();
        public string ProjectCode = ConfigurationManager.AppSettings["ProjectCode"].ToString();
        private unilab.crm.business.interfaces.IDataManagementService dataManagementService;
        private static Random random = new Random();
        #region "   Sql    "

        public Helper()
        {
            dataManagementService = unilab.crm.factory.BusinessDelegateFactory.GetInstance().GetDataManagementManager();
        }

        public DataTable GetTokenByProjectCode(string ProjectCode)
        {
            sp_parameters = new object[,] { { "@ProjectCode", ProjectCode } };
            return ExecuteReader(CRMconn, "usp_GetTokenByProjectCode", sp_parameters);
        }

        public DataTable ExecuteReader(string connString, string storedproc, object[,] sp_parameters)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedproc;
                cmd.CommandTimeout = 1200;
                if (sp_parameters.GetLength(0) != 0)
                {
                    for (int i = 0; i < sp_parameters.GetLength(0); i++)
                        cmd.Parameters.AddWithValue(sp_parameters[i, 0].ToString(), sp_parameters[i, 1]);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }

            return dt;
        }

        #endregion

        #region "   LOG "

        public void Log(string Message, string SubFolderFilename)
        {
            string FileName = RootPath + @"\" + SubFolderFilename + "_" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
            using (StreamWriter streamWriter = File.AppendText(FileName))
            {
                WriteLog(Message, streamWriter);
            }
        }

        public void WriteLog(string Message, TextWriter w)
        {
            w.WriteLine(Message);
        }

        #endregion
    }
}