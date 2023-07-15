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
    public abstract class CountryProvider : DataProviderBase
    {
        static private CountryProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public CountryProvider Instance
        {
            get
            {
                lock (typeof(CountryProvider))
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
                        Type t = assem.GetType(Globals.Settings.Common.Country.ProviderType);
                        _instance = (CountryProvider)Activator.CreateInstance(t);
                    } 
                }
                return _instance;
            }
        }

        public CountryProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }
        public abstract List<RefCountry> GetAllCountries();

        public abstract List<CitiesProvince> GetAllProvinces();

        public abstract List<Currency> GetAllCurrency(bool? IsActive);
        public abstract List<Hospital> Hospital_ByCityProvinceID(long? CityProvinceID);

        public abstract List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Get();
        public abstract List<RefGenericDrugCategory_1> RefGenericDrugCategory_1_Get(long V_MedProductType);
        public abstract List<RefGenericDrugCategory_2> RefGenericDrugCategory_2_Get(long V_MedProductType);
        public abstract List<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory();

        //▼===== 25072018 TTM
        public abstract List<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory_New();
        //▼===== 25072018 TTM
    }
}
