using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;

namespace eHCMS.DAL
{
    public abstract class StaffProvider : DataProviderBase
    {
        static private StaffProvider _instance = null;
        static public StaffProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Common.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Common.Staff.ProviderType);
                    _instance = (StaffProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }
        public StaffProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }
        #region staff member
        public abstract Staff GetStaffByID(Int64 ID);
        public abstract List<Staff> GetAllStaffContain();

        public abstract List<StaffPosition> GetAllStaffPosition();
        #endregion
    }

}
