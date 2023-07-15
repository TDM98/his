using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace aEMR.DataAccessLayer.Providers
{
    public class ReportSqlProvider : DataProviderBase
    {
        static private ReportSqlProvider _instance = null;
        static public ReportSqlProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReportSqlProvider();
                }
                return _instance;
            }
        }
        private void CallReaderIntoSchema(DataTable aTable, IDataParameter[] aParameters, object[] aParameterValues, int? aTimeOut)
        {
            if (aTable == null)
            {
                return;
            }
            if (aParameters != null && aParameters.Length > 0)
            {
                aParameters = aParameters.Where(x => x.Direction == ParameterDirection.Input).ToArray();
            }
            using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["eHCMS.ReportLib.Properties.Settings.eHCMS_DBConnectionString"].ConnectionString))
            {
                aTable.Rows.Clear();
                try
                {
                    cn.Open();
                }
                catch
                {
                    cn.Dispose();
                    return;
                }
                SqlCommand cmd = cn.CreateCommand();
                if (aTimeOut.HasValue && aTimeOut > 0)
                {
                    cmd.CommandTimeout = aTimeOut.Value;
                }
                if (aParameters != null && aParameters.Length > 0)
                {
                    for (int i = 0; i < aParameters.Length; i++)
                    {
                        cmd.Parameters.Add(new SqlParameter(aParameters[i].ParameterName, aParameterValues[i]));
                    }
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = aTable.TableName;
                var reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    var mRow = aTable.NewRow();
                    foreach (DataColumn item in aTable.Columns)
                    {
                        if (reader.HasColumn(item.ColumnName))
                        {
                            mRow[item.ColumnName] = reader[item.ColumnName];
                        }
                    }
                    aTable.Rows.Add(mRow);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cmd.Connection, cmd);
            }
        }
        public void ReaderIntoSchema(DataTable aTable, IDataParameter[] aParameters, object[] aParameterValues)
        {
            CallReaderIntoSchema(aTable, aParameters, aParameterValues, null);
        }
        public void ReaderIntoSchema(DataTable aTable, IDataParameter[] aParameters, object[] aParameterValues, int aTimeOut)
        {
            CallReaderIntoSchema(aTable, aParameters, aParameterValues, aTimeOut);
        }
    }
}