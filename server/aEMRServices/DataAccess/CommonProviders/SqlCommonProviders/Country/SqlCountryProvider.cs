using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
namespace eHCMS.DAL
{
    public class SqlCountryProvider : CountryProvider
    {
        public SqlCountryProvider()
            : base()
        {

        }
        public override List<RefCountry> GetAllCountries()
        {
           using (SqlConnection cn = new SqlConnection(this.ConnectionString))
           {
               SqlCommand cmd = new SqlCommand("spGetAllCountries", cn);
               cmd.CommandType = CommandType.StoredProcedure;

               cn.Open();
               List<RefCountry> countries = null;

               IDataReader reader = ExecuteReader(cmd);

               countries = GetCountryCollectionFromReader(reader);
               return countries;
           }
        }
        public override List<CitiesProvince> GetAllProvinces()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllProvinces", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<CitiesProvince> provinces = null;

                IDataReader reader = ExecuteReader(cmd);

                provinces = GetCityProvinceCollectionFromReader(reader);
                return provinces;
            }
        }

        public override List<Currency> GetAllCurrency(bool? IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCurrency_Load", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));
                cn.Open();
                List<Currency> provinces = null;
                IDataReader reader = ExecuteReader(cmd);
                provinces =GetCurrencyCollectionFromReader(reader);
                return provinces;
            }
        }
        public override List<Hospital> Hospital_ByCityProvinceID(long? CityProvinceID)
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
                return provinces;
            }
        }

        public override List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Get()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefGenDrugBHYT_Category", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefGenDrugBHYT_Category> provinces = null;
                IDataReader reader = ExecuteReader(cmd);
                provinces = GetRefGenDrugBHYT_CategoryCollectionFromReader(reader);
                return provinces;
            }
        }
        
        public override List<RefGenericDrugCategory_1> RefGenericDrugCategory_1_Get(long V_MedProductType)
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
                return provinces;
            }
        }

        public override List<RefGenericDrugCategory_2> RefGenericDrugCategory_2_Get(long V_MedProductType)
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
                return provinces;
            }
        }

        public override List<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLoadRefPharmacyDrugCategory", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefPharmacyDrugCategory> ListRefPharmacyDrugCategory = null;
                IDataReader reader = ExecuteReader(cmd);
                ListRefPharmacyDrugCategory = GetRefPharmacyDrugCategoryCollectionFromReader(reader);
                return ListRefPharmacyDrugCategory;
            }
        }
        //▼===== 25072018 TTM
        public override List<RefPharmacyDrugCategory> LoadRefPharmacyDrugCategory_New()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLoadRefPharmacyDrugCategory", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefPharmacyDrugCategory> ListRefPharmacyDrugCategory = null;
                IDataReader reader = ExecuteReader(cmd);
                ListRefPharmacyDrugCategory = GetRefPharmacyDrugCategoryCollectionFromReader(reader);
                return ListRefPharmacyDrugCategory;
            }
        }
        //▼===== 25072018 TTM

    }
}
