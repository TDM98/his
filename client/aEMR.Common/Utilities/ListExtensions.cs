using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace aEMR.Common.Utilities
{
    public static class ListExtensions
    {
        public static void AddToListIfNotExist<TS>(this IList<TS> root, TS item, IEqualityComparer<TS> comparer = null)
        {
            if (!root.Contains(item, comparer))
            {
                root.Add(item);
            }
        }

        public static DataSet ToDataSet<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
               // t.Columns.Add(propInfo.Name, typeof(DBNull));  
              
               t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(
            propInfo.PropertyType) ?? propInfo.PropertyType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();
                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item,null) ?? DBNull.Value;
                }
            }

            return ds;
        }
    }
}
