using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace eHCMS.DAL
{
    public static class SqlCommandExtension
    {
        public static void AddParameter(this SqlCommand cmd, string paramName, SqlDbType dbType, int size, ParameterDirection direction)
        {
            SqlParameter curParam = new SqlParameter(paramName, dbType, size);
            curParam.Direction = direction;
            cmd.Parameters.Add(curParam);
        }

        public static void AddParameter(this SqlCommand cmd, string paramName, SqlDbType dbType, int size, object value, ParameterDirection direction)
        {
            SqlParameter curParam = new SqlParameter(paramName, dbType, size);
            curParam.Direction = direction;
            curParam.Value = value;
            cmd.Parameters.Add(curParam);
        }

        public static void AddParameter(this SqlCommand cmd, string paramName, SqlDbType dbType, object value, ParameterDirection direction)
        {
            SqlParameter curParam = new SqlParameter(paramName, dbType);
            curParam.Direction = direction;
            curParam.Value = value;
            cmd.Parameters.Add(curParam);
        }
        public static void AddParameter(this SqlCommand cmd, string paramName, SqlDbType dbType, object value)
        {
            SqlParameter curParam = new SqlParameter(paramName, dbType);
            curParam.Value = value;
            cmd.Parameters.Add(curParam);
        }

        public static void AddParameters(this SqlCommand cmd, string[] paramNames, SqlDbType[] dbTypes, object[] values,ParameterDirection[] directions)
        {
            for(int i=0;i<paramNames.Length;i++)
            {
                cmd.AddParameter(paramNames[i], dbTypes[i], values[i], directions[i]);
            }
        }

        public static void AddParameters(this SqlCommand cmd, string[] paramNames, SqlDbType[] dbTypes, object[] values)
        {
            for (int i = 0; i < paramNames.Length; i++)
            {
                cmd.AddParameter(paramNames[i], dbTypes[i], values[i]);
            }
        }
    }
}
