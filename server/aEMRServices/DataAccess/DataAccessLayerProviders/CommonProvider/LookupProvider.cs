using System;
using System.Collections.Generic;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using eHCMS.Configurations;
using eHCMS.DAL;

namespace aEMR.DataAccessLayer.Providers
{
    public class LookupProvider : DataProviderBase
    {
        static private LookupProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public LookupProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LookupProvider();
                }
                return _instance;
            }
        }

        public LookupProvider()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }

        public  List<Lookup> GetAllLookupsForTransferForm(LookupValues lookupType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllLookupForTransferForm", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<Lookup> lookups = null;
                IDataReader reader = ExecuteReader(cmd);

                lookups = GetLookupCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lookups;
            }
        }

        public  List<Lookup> GetAllLookupsByType(LookupValues lookupType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllLookupByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ObjectTypeID", SqlDbType.BigInt, (long)lookupType);
                cn.Open();
                List<Lookup> lookups = null;
                IDataReader reader = ExecuteReader(cmd);

                lookups = GetLookupCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return lookups;
            }
        }
        public  List<RefDepartment> GetAllDepartments(bool bIncludeDeleted = false)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDepartments", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@IncludeDeletedDept", SqlDbType.Bit, (bIncludeDeleted ? 1 : 0));
                cn.Open();
                List<RefDepartment> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRefDeptsCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public  List<DeptTransferDocReq> GetAllDocTypeRequire()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDocTypeRequire", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<DeptTransferDocReq> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetDocTypeRequiresCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public  List<RefDepartment> GetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDepartmentsByV_DeptTypeOperation", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_DeptTypeOperation", SqlDbType.BigInt, V_DeptTypeOperation);
                cn.Open();
                List<RefDepartment> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRefDeptsCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }


        public  List<RefDepartment> GetDepartments(long locationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDepartments", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ParDeptID", SqlDbType.Decimal, 2);
                cmd.AddParameter("@LocationID", SqlDbType.Decimal, locationID);
                cn.Open();
                List<RefDepartment> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRefDeptsCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public  List<PatientPaymentAccount> GetAllPatientPaymentAccounts()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPaymentAccounts_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<PatientPaymentAccount> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientPaymentAccountCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public  List<ResourceGroup> GetAllResourceGroup()
        {
            List<ResourceGroup> listRG = null;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllResourceGroup", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@ParDeptID", SqlDbType.Decimal, 2);
                cn.Open();
                ResourceGroup p = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    p.RscrGroupID = (long)reader["RscrGroupID"];
                    p.GroupName = reader["GroupName"].ToString();
                    p.Description = reader["Description"].ToString();
                    listRG.Add(p);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        public  List<ResourceType> GetAllResourceType()
        {
            List<ResourceType> listRG = new List<ResourceType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllResourceType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                ResourceType p = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    p.RscrTypeID = (long)reader["RscrTypeID"];
                    p.TypeName = reader["TypeName"].ToString();
                    p.Description = reader["Description"].ToString();
                    listRG.Add(p);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRG;
        }
        public  IList<DrugDeptProductGroupReportType> GetDrugDeptProductGroupReportTypes()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDrugDeptProductGroupReportTypes", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<DrugDeptProductGroupReportType> mDrugDeptMATReportTypeCollection = GetDrugDeptMATReportTypeCollectionFromReader(mReader);

                mReader.Close();                
                CleanUpConnectionAndCommand(cn, cmd);
                return mDrugDeptMATReportTypeCollection;
            }
        }

        public List<Lookup> GetAllLookupsByType_V2(LookupValues lookupType, string SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllLookupForMgnt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ObjectTypeID", SqlDbType.BigInt, (long)lookupType);
                cmd.AddParameter("@SearchCriteria", SqlDbType.NVarChar, SearchCriteria);
                cn.Open();
                List<Lookup> lookups = null;
                IDataReader reader = ExecuteReader(cmd);

                lookups = GetLookupCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return lookups;
            }
        }
    }
}
