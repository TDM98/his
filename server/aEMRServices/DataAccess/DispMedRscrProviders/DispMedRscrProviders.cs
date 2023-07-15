using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using eHCMS.Configurations;
using System.Xml.Linq;
using eHCMS.Services.Core;
using System.ComponentModel;

namespace eHCMS.DAL
{
    public abstract class DispMedRscrProviders : DataProviderBase
    {
        static private DispMedRscrProviders _instance = null;
        static public DispMedRscrProviders Instance
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
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.DispMedRscr.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type[] types = assem.GetExportedTypes();
                    Type t = assem.GetType(Globals.Settings.DispMedRscr.ProviderType);
                    _instance = (DispMedRscrProviders)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }
        public DispMedRscrProviders()
        {
            this.ConnectionString = Globals.Settings.DispMedRscr.ConnectionString;

        }

        #region 0. Inward disposable resource member

        public abstract List<InwardDMedRscrInvoice> GetAllInwardDMedRscrInvoice(int V_Reason,int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<InwardDMedRscrInvoice> SearchInwardDMedRscrInvoice(int V_Reason, InwardInvoiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDMedRscrInvoice GetInwardDMedRscrInvoiceByID(int V_Reason, long InvDMedRscrID);
        public abstract bool AddInwardDMedRscrInvoice(InwardDMedRscrInvoice InvoiceDrug, out long inwardid);
        public abstract bool UpdateInwardDMedRscrInvoice(InwardDMedRscrInvoice InvoiceDrug);
        public abstract bool DeleteInwardDMedRscrInvoice(long ID);


        #endregion

        #region "1. Resources"
        protected virtual Resource GetResourceFromReader(IDataReader reader)
        {
            Resource p = new Resource();
            //p.RscrID = (long)reader["RscrID"];
            //p.DeprecTypeID = (byte)reader["DeprecTypeID"];
            //p.MedRescrTypeID = (long)reader["MedRescrTypeID"];
            //p.SupplierID =(long)reader["SupplierID"];
            //p.RscrItemCode =Convert.ToString(reader["RscrItemCode"]);
            //p.RscrName_Brand =Convert.ToString(reader["RscrName_Brand"]);
            //p.RscrFunctions = Convert.ToString(reader["RscrFunctions"]);
            //p.RscrTechInfo =Convert.ToString(reader["RscrTechInfo"]);
            //p.RscrDepreciationRate =Convert.ToDouble(reader["RscrDepreciationRate"]);
            //p.RscrPrice =Convert.ToDecimal(reader["RscrPrice"]);
            //p.V_RscrUnit =Convert.ToString(reader["V_RscrUnit"]);
            //p.BeOfHIMedicineList = Convert.ToBoolean(reader["BeOfHIMedicineList"]);
            //p.ResourceType = (byte)reader["ResourceType"];            
            return p;
        }
        protected virtual List<Resource> GetResourceCollectionFromReader(IDataReader reader)
        {
            List<Resource> lst = new List<Resource>();
            while (reader.Read())
            {
                lst.Add(GetResourceFromReader(reader));
            }
            return lst;
        }

        public abstract List<Resource> GetAllResource(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        //public abstract List<Resource> SearchResource(int V_Reason, ResourceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract Resource GetResourceByID(long ResourceID);
        public abstract bool AddResource(Resource objResource, out long ResourceID);
        public abstract bool UpdateResource(Resource objResource);
        public abstract bool DeleteResource(long ID, bool IsDeleted);
        #endregion
    }
}
