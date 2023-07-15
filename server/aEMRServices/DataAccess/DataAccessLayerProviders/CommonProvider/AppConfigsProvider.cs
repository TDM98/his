using System;
using System.Collections.Generic;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using eHCMS.Configurations;

using eHCMS.DAL;

/*
 * 20181113 #001 TTM: BM 00005228: Thêm hàm lấy danh sách phường xã.
 * 20230316 #002 QTD: Dữ liệu 130
 */
namespace aEMR.DataAccessLayer.Providers
{
    public class AppConfigsProvider : DataProviderBase
    {
        static public List<string> ConfigItemvalue= new List<string>();
        static private AppConfigsProvider _instance = null;

        static public AppConfigsProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppConfigsProvider();
                }
                return _instance;
            }
        }

        public AppConfigsProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }


        public  bool UpdateRefApplicationConfigs(string ConfigItemKey, string ConfigItemValue, string ConfigItemNotes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefApplicationConfigs_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ConfigItemKey", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ConfigItemKey));
                cmd.AddParameter("@ConfigItemValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ConfigItemValue));
                cmd.AddParameter("@ConfigItemNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ConfigItemNotes));

                cn.Open();
                var res = cmd.ExecuteNonQuery();

                CleanUpConnectionAndCommand(cn, cmd);

                if (res > 0)
                    return true;
                else
                    return false;
            }
        }

        public  void PriceList_Reset()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPriceList_Reset", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                var res = cmd.ExecuteNonQuery();
            }
        }

        public  List<SuburbNames> GetAllSuburbNames()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSuburbNames_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                var lsVal = new List<SuburbNames>();
                IDataReader reader = ExecuteReader(cmd);

                // TxD 21/07/2016 : Get rid of the buggy outer while loop that caused the very first suburb (happened to be Ba Dinh - Ha Noi) NOT TO BE ADDED into list
                //while (reader.Read())
                //{
                lsVal = GetSuburbNamesCollectionFromReader(reader);
                //}

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return lsVal;
            }
        }
        //▼====== #001
        public  List<WardNames> GetAllWardNames()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spWardNames_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                var lsVal = new List<WardNames>();
                IDataReader reader = ExecuteReader(cmd);
                lsVal = GetWardNamesCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lsVal;
            }
        }
        //▲====== #001
        //▼====== #002
        public List<RefJob> GetAllJobs()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllJobs", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                var lsVal = new List<RefJob>();
                IDataReader reader = ExecuteReader(cmd);
                lsVal = GetJobCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lsVal;
            }
        }
        //▲====== #002
        //export excel all
        #region Export excel from database

        public  List<List<string>> ExportToExcellAll_ListRefGenericDrug(DrugSearchCriteria criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRefGenericDrugDetails_ExportExcel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@isinsurance", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsInsurance));
                cmd.AddParameter("@isconsult", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsConsult));
                cmd.AddParameter("@IsActive", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsActive));
                cmd.AddParameter("@DrugClassID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FaID));
                cmd.AddParameter("@IsShow", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsShow));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }

                CleanUpConnectionAndCommand(cn, cmd);

                return returnAllExcelData;
            }
        }

        //▼===== 25072018 TTM
        public  List<List<string>> ExportToExcellAll_ListRefGenericDrug_New(DrugSearchCriteria criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRefGenericDrugDetails_ExportExcel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@isinsurance", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsInsurance));
                cmd.AddParameter("@isconsult", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsConsult));
                cmd.AddParameter("@IsActive", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsActive));
                cmd.AddParameter("@DrugClassID", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.FaID));
                cmd.AddParameter("@IsShow", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsShow));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }
        //▲===== 25072018 TTM

        public  List<List<string>> ExportToExcellAll_ListRefGenMedProductDetail(RefGenMedProductDetailsSearchCriteria criteria)
        {
            DataSet dsExportToExcellAll = new DataSet();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = null;
                cmd = new SqlCommand("spRefGenMedProductDetails_ExportExcel", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@isinsurance", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsInsurance));
                cmd.AddParameter("@isconsult", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsConsult));
                cmd.AddParameter("@IsActive", SqlDbType.TinyInt, ConvertNullObjectToDBNull(criteria.IsActive));
                cmd.AddParameter("@V_MedProductType", SqlDbType.BigInt, ConvertNullObjectToDBNull(criteria.V_MedProductType));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dsExportToExcellAll);
                List<List<string>> returnAllExcelData = new List<List<string>>();

                //Add the below 4 lines to add the column names to show on the Excel file
                List<string> colname = new List<string>();
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                {
                    colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                }

                returnAllExcelData.Add(colname);
                for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                {
                    List<string> rowData = new List<string>();
                    for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                    {
                        rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                    }
                    returnAllExcelData.Add(rowData);
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return returnAllExcelData;
            }
        }

    }

    #endregion
}
