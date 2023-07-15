using System;
using System.Data;
using System.Data.SqlClient;

using System.Data.Common;
using System.Web.Caching;
using System.Web;
using Service.Core.HelperClasses;

/// <summary>
/// Summary description for DataAccess
/// </summary>
/// 

namespace aEMR.DataAccessLayer.Providers
{
    public abstract class DataAccess
    {
        public DataAccess()
        {

        }

        private string m_ConnectionString = "";
        protected string ConnectionString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        private bool m_EnableCaching = true;
        protected bool EnableCaching
        {
            get { return m_EnableCaching; }
            set { m_EnableCaching = value; }
        }

        private int m_CacheDuration = 0;

        protected int CacheDuration
        {
            get { return m_CacheDuration; }
            set { m_CacheDuration = value; }
        }

        protected Cache Cache
        {
            get { return HttpContext.Current.Cache; }
        }

        private string m_EncryptKey = "";
        protected string EncryptKey
        {
            get { return m_EncryptKey; }
        }

        private int m_Timeout = 0;
        protected int TimeOut
        {
            get { return m_Timeout; }
        }

        private bool m_WriteErrorToLog;
        protected bool WriteErrorToLog
        {
            get
            {
                return m_WriteErrorToLog;
            }
            set
            {
                m_WriteErrorToLog = value;
            }
        }

        private string m_errorLogFile;
        public string ErrorLogFile
        {
            get
            {
                return m_errorLogFile;
            }
            set
            {
                m_errorLogFile = value;
            }
        }

        private int _numOfRetryOnDbDeadlock = 3;
        /// <summary>
        /// So lan retry neu gap transaction deadlock.
        /// </summary>
        protected int NumOfRetryOnDbDeadlock
        {
            get
            {
                return _numOfRetryOnDbDeadlock;
            }
            set
            {
                _numOfRetryOnDbDeadlock = value;
            }
        }

        protected object ConvertNullObjectToDBNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }
        protected object ConvertZeroObjectToDBNull(object obj)
        {
            try
            {
                if (Convert.ToDecimal(obj) == 0)
                    return DBNull.Value;
                return obj;
            }
            catch
            {
                return DBNull.Value;
            }
        }
        protected object ConvertNullToZero(object obj)
        {
            try
            {
                if (obj == null)
                    return 0;
                return obj;
            }
            catch
            {
                return 0;
            }
        }
        protected object ConvertNullToFalse(object obj)
        {
            try
            {
                if (obj == null)
                    return false;
                return obj;
            }
            catch
            {
                return false;
            }
        }
        protected Int64 ConvertObjectToInt64(object obj)
        {
            try
            {
                return Convert.ToInt64(obj);
            }
            catch
            {
                return 0;
            }
        }
        protected IDataReader ExecuteReader(DbCommand cmd)
        {
            if (cmd.Connection != null && cmd.Connection is SqlConnection && !(cmd.Connection as SqlConnection).StatisticsEnabled)
            {
                (cmd.Connection as SqlConnection).StatisticsEnabled = true;
            }
            return ExecuteReader(cmd, CommandBehavior.Default);
        }
        protected IDataReader ExecuteReader(DbCommand cmd, CommandBehavior behavior)
        {
            if (cmd.Connection != null && cmd.Connection is SqlConnection && !(cmd.Connection as SqlConnection).StatisticsEnabled)
            {
                (cmd.Connection as SqlConnection).StatisticsEnabled = true;
            }
            return cmd.ExecuteReader(behavior);
        }
        protected object ExecuteScalar(DbCommand cmd)
        {
            if (cmd.Connection != null && cmd.Connection is SqlConnection && !(cmd.Connection as SqlConnection).StatisticsEnabled)
            {
                (cmd.Connection as SqlConnection).StatisticsEnabled = true;
            }
            return cmd.ExecuteScalar();
        }
        protected int ExecuteNonQuery(DbCommand cmd)
        {
            if (cmd.Connection != null && cmd.Connection is SqlConnection && !(cmd.Connection as SqlConnection).StatisticsEnabled)
            {
                (cmd.Connection as SqlConnection).StatisticsEnabled = true;
            }
            return cmd.ExecuteNonQuery();
        }
        protected DataTable ExecuteDataTable(DbCommand cmd)
        {
            if (cmd.Connection != null && cmd.Connection is SqlConnection && !(cmd.Connection as SqlConnection).StatisticsEnabled)
            {
                (cmd.Connection as SqlConnection).StatisticsEnabled = true;
            }
            IDataReader reader = ExecuteReader(cmd, CommandBehavior.CloseConnection);
            DataTable tbl = null;
            if (reader != null)
            {
                tbl = new DataTable();
                tbl.Load(reader);
            }
            return tbl;
        }
        protected DataSet ExecuteDataSet(SqlCommand cmd)
        {
            if (cmd.Connection != null && cmd.Connection is SqlConnection && !(cmd.Connection as SqlConnection).StatisticsEnabled)
            {
                (cmd.Connection as SqlConnection).StatisticsEnabled = true;
            }
            DataSet mDataSet = new DataSet();
            SqlDataAdapter mSqlDataAdapter = new SqlDataAdapter(cmd);
            mSqlDataAdapter.Fill(mDataSet);
            mSqlDataAdapter.Dispose();
            return mDataSet;
        }
        public virtual DbConnection CreateConnection()
        {
            return null;
        }
        /// <summary>
        /// Se thuc hien tiep cau lenh neu co deadlock trong database
        /// </summary>
        /// <param name="retryAction"></param>
        public virtual void RetryIfDeadlock(Action retryAction)
        {
            RetryOnDatabaseDeadlock.RetryUntil(retryAction, NumOfRetryOnDbDeadlock);
        }
        /// <summary>
        /// Se thuc hien tiep cau lenh neu co deadlock trong database
        /// </summary>
        /// <param name="retryAction"></param>
        public virtual T RetryIfDeadlock<T>(Func<T> retryAction)
        {
            return RetryOnDatabaseDeadlock.RetryUntil(retryAction, NumOfRetryOnDbDeadlock);
        }
        protected void CleanUpConnectionAndCommand(SqlConnection TheConnection, SqlCommand TheCommand)
        {
            if (TheCommand != null)
            {
                var mCommandConnection = TheCommand.Connection;
                if (mCommandConnection != null && mCommandConnection.StatisticsEnabled)
                {
                    var ConnectionStats = mCommandConnection.RetrieveStatistics();
                    var ExecutionTime = (long)ConnectionStats["ExecutionTime"];
                    if (ExecutionTime > 0)
                    {
                        string mMethodName = "";
                        var mStackFrame = new System.Diagnostics.StackFrame(1);
                        if (mStackFrame != null)
                        {
                            var mMethod = mStackFrame.GetMethod();
                            if (mMethod != null)
                            {
                                mMethodName = mMethod.Name;
                            }
                        }
                        AxLogging.AxLogger.Instance.SqlLogInfo(string.Format("{0}\t{1}\t{2}", mMethodName, TheCommand.CommandText, ExecutionTime), null);
                    }
                }
                TheCommand.Dispose();
            }
            if (TheConnection != null)
            {
                TheConnection.Dispose();
            }
        }
    }

    public static class DataReaderExtensions
    {
        public static bool HasColumn(this IDataReader dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}