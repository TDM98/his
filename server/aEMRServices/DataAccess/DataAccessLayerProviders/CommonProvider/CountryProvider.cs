
using System.Collections.Generic;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using eHCMS.Configurations;
using eHCMS.DAL;

namespace aEMR.DataAccessLayer.Providers
{
    public class CountryProvider : DataProviderBase
    {
        static private CountryProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public CountryProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CountryProvider();
                }
                return _instance;
            }
        }

        public CountryProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }

        public  List<RefCountry> GetAllCountries()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllCountries", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<RefCountry> countries = null;

                IDataReader reader = ExecuteReader(cmd);

                countries = GetCountryCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return countries;
            }
        }
        public  List<CitiesProvince> GetAllProvinces()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllProvinces", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<CitiesProvince> provinces = null;

                IDataReader reader = ExecuteReader(cmd);

                provinces = GetCityProvinceCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return provinces;
            }
        }

        public  List<Currency> GetAllCurrency(bool? IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCurrency_Load", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));
                cn.Open();
                List<Currency> provinces = null;
                IDataReader reader = ExecuteReader(cmd);
                provinces = GetCurrencyCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return provinces;
            }
        }
        public  List<Hospital> Hospital_ByCityProvinceID(long? CityProvinceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHospital_ByCityProvinceID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CityProvinceID));
                cn.Open();
                List<Hospital> provinces = null;
                IDataReader reader = ExecuteReader(cmd);
                provinces = GetHospitalCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return provinces;
            }
        }

        public  List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Get()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefGenDrugBHYT_Category", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefGenDrugBHYT_Category> provinces = null;
                IDataReader reader = ExecuteReader(cmd);
                provinces = GetRefGenDrugBHYT_CategoryCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return provinces;
            }
        }

        public  List<RefGenericDrugCategory_1> RefGenericDrugCategory_1_Get(long V_MedProductType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefGenericDrugCategory_1", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_MedProductType));
                cn.Open();
                List<RefGenericDrugCategory_1> provinces = null;
                IDataReader reader = ExecuteReader(cmd);
                provinces = GetRefGenericDrugCategory_1CollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return provinces;
            }
        }

        public  List<RefGenericDrugCategory_2> RefGenericDrugCategory_2_Get(long V_MedProductType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefGenericDrugCategory_2", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_MedProductType));
                cn.Open();
                List<RefGenericDrugCategory_2> provinces = null;
                IDataReader reader = ExecuteReader(cmd);
                provinces = GetRefGenericDrugCategory_2CollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return provinces;
            }
        }

        public  List<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLoadRefPharmacyDrugCategory", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefPharmacyDrugCategory> ListRefPharmacyDrugCategory = null;
                IDataReader reader = ExecuteReader(cmd);
                ListRefPharmacyDrugCategory = GetRefPharmacyDrugCategoryCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return ListRefPharmacyDrugCategory;
            }
        }
        //▼===== 25072018 TTM
        public  List<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory_New()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLoadRefPharmacyDrugCategory", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefPharmacyDrugCategory> ListRefPharmacyDrugCategory = null;
                IDataReader reader = ExecuteReader(cmd);
                ListRefPharmacyDrugCategory = GetRefPharmacyDrugCategoryCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return ListRefPharmacyDrugCategory;
            }
        }
        //▼===== 25072018 TTM

        public List<RefNationality> GetAllNationalities()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllNationalities", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<RefNationality> countries = null;

                IDataReader reader = ExecuteReader(cmd);

                countries = GetNationalityCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return countries;
            }
        }
    }
}
