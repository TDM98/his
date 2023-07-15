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
    public abstract class LookupProvider : DataProviderBase
    {
        static private LookupProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public LookupProvider Instance
        {
            get
            {
                lock (typeof(LookupProvider))
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
                        Type t = assem.GetType(Globals.Settings.Common.Lookup.ProviderType);
                        _instance = (LookupProvider)Activator.CreateInstance(t);
                        //_instance = (PatientProvider)Activator.CreateInstance(Type.GetType(Globals.Settings.Patients.ProviderType));
                    } 
                }
                return _instance;
            }
        }

        public LookupProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }
        public abstract List<Lookup> GetAllLookupsByType(LookupValues lookupType);

        public abstract List<Lookup> GetAllLookupsForTransferForm(LookupValues lookupType);

        public abstract List<DeptTransferDocReq> GetAllDocTypeRequire();

        public abstract List<RefDepartment> GetAllDepartments(bool bIncludeDeleted);

        public abstract List<RefDepartment> GetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation);

        public abstract List<RefDepartment> GetDepartments(long locationID);

        public abstract List<PatientPaymentAccount> GetAllPatientPaymentAccounts();

        //Dinh them phan resoueces
        public abstract List<ResourceGroup> GetAllResourceGroup();
        public abstract List<ResourceType> GetAllResourceType();
        public abstract IList<DrugDeptProductGroupReportType> GetDrugDeptProductGroupReportTypes();
    }
}
