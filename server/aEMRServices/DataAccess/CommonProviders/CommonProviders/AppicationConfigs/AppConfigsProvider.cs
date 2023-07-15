using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
/*
 * 20181113 #001 TTM: BM 00005228: Thêm hàm lấy danh sách phường xã.
 */
namespace eHCMS.DAL
{
    public abstract class AppConfigsProvider : DataProviderBase
    {
        static public List<string> ConfigItemvalue= new List<string>();
        static private AppConfigsProvider _instance = null;

        static public AppConfigsProvider Instance
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
                    Type t = assem.GetType(Globals.Settings.Common.AppicationConfig.ProviderType);
                    _instance = (AppConfigsProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public AppConfigsProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }

        //public abstract List<string> GetAllConfigItemValues();
        //public abstract string GetConfigItemsValueBySerialNumber(int sNumber);

        public abstract bool UpdateRefApplicationConfigs(string ConfigItemKey, string ConfigItemValue, string ConfigItemNotes);
        public abstract void PriceList_Reset();
        public abstract List<SuburbNames> GetAllSuburbNames();
        //▼====== #001
        public abstract List<WardNames> GetAllWardNames();
        //▲====== #001
        //export excel all
        #region Export excel from database
        public abstract List<List<string>> ExportToExcellAll_ListRefGenericDrug(DrugSearchCriteria criteria);

        //▼===== 25072018 TTM
        public abstract List<List<string>> ExportToExcellAll_ListRefGenericDrug_New(DrugSearchCriteria criteria);
        //▲===== 25072018 TTM

        public abstract List<List<string>> ExportToExcellAll_ListRefGenMedProductDetail(RefGenMedProductDetailsSearchCriteria criteria);
        #endregion
    }
}
