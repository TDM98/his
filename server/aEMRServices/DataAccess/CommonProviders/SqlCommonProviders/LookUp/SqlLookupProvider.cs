using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
namespace eHCMS.DAL
{
    public class SqlLookupProvider : LookupProvider
    {
        public SqlLookupProvider()
            : base()
        {

        }

        public override List<Lookup> GetAllLookupsForTransferForm(LookupValues lookupType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllLookupForTransferForm", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<Lookup> lookups = null;
                IDataReader reader = ExecuteReader(cmd);

                lookups = GetLookupCollectionFromReader(reader);
                return lookups;
            }
        }

        public override List<Lookup> GetAllLookupsByType(LookupValues lookupType)
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
               return lookups;
           }
        }
        public override List<RefDepartment> GetAllDepartments(bool bIncludeDeleted = false)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDepartments", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@IncludeDeletedDept", SqlDbType.Bit, (bIncludeDeleted ? 1 : 0));
                cn.Open();
                List<RefDepartment> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRefDeptsCollectionFromReader(reader) ;
                return retVal;
            } 
        }

        public override List<DeptTransferDocReq> GetAllDocTypeRequire()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDocTypeRequire", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<DeptTransferDocReq> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetDocTypeRequiresCollectionFromReader(reader);
                return retVal;
            }
        }

        public override List<RefDepartment> GetAllDepartmentsByV_DeptTypeOperation(long V_DeptTypeOperation)
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
                return retVal;
            }
        }

        protected override RefMedicalServiceItem GetMedicalServiceItemFromReader(IDataReader reader)
        {
            RefMedicalServiceItem p = new RefMedicalServiceItem();
            p.MedServiceID = (long)reader["MedServiceID"];
            p.DeptID = reader["DeptID"] as long?;
            p.MedicalServiceTypeID = reader["MedicalServiceTypeID"] as long?;

            p.PartnerShipID = reader["PartnerShipID"] as long?;

            p.MedServiceCode = reader["MedServiceCode"] as string;
            p.MedServiceName = reader["MedServiceName"] as string;
            p.V_RefMedServiceItemsUnit =(long)reader["V_RefMedServiceItemsUnit"];

            p.ObjV_RefMedServiceItemsUnit = new Lookup();

            try
            {
                p.ObjV_RefMedServiceItemsUnit.LookupID = (long)reader["V_RefMedServiceItemsUnit"];
                if(reader.HasColumn("V_RefMedServiceItemsUnitName"))
                {
                    p.ObjV_RefMedServiceItemsUnit.ObjectValue = reader["V_RefMedServiceItemsUnitName"] == null ? "" : reader["V_RefMedServiceItemsUnitName"].ToString().Trim();
                }
            }
            catch
            {
                
            }

            /*TMA*/
            p.V_Surgery_Tips_Type = (long)reader["V_Surgery_Tips_Type"];

            p.ObjV_Surgery_Tips_Type = new Lookup();

            try
            {
                p.ObjV_Surgery_Tips_Type.LookupID = (long)reader["V_Surgery_Tips_Type"];
                if (reader.HasColumn("V_Surgery_Tips_Type"))
                {
                    p.ObjV_Surgery_Tips_Type.ObjectValue = reader["V_Surgery_Tips_Type"] == null ? "" : reader["V_Surgery_Tips_Type"].ToString().Trim();
                }
            }
            catch
            {

            }
            p.V_Surgery_Tips_Item = (long)reader["V_Surgery_Tips_Item"];
            try
            {
                p.ObjV_Surgery_Tips_Item.LookupID = (long)reader["V_Surgery_Tips_Item"];
                if (reader.HasColumn("V_Surgery_Tips_Item"))
                {
                    p.ObjV_Surgery_Tips_Item.ObjectValue = reader["V_Surgery_Tips_Item"] == null ? "" : reader["V_Surgery_Tips_Item"].ToString().Trim();
                }
            }
            catch
            {

            }
            /*TMA*/

            p.VATRate = reader["VATRate"] as double?;
            p.NormalPrice = (decimal)reader["NormalPrice"];
            p.PriceDifference = (decimal)reader["PriceDifference"];
            p.ChildrenUnderSixPrice = (decimal)reader["ChildrenUnderSixPrice"];
            p.HIAllowedPrice = reader["HIAllowedPrice"] as decimal?;

            p.ExpiryDate = (DateTime)reader["EffectiveDate"];
            p.IsExpiredDate = reader["IsExpiredDate"] as bool?;
            p.ByRequest = reader["ByRequest"] as bool?;
            p.ServiceMainTime = reader["ServiceMainTime"] as byte?;
            return p;
        }
      
        public override List<RefDepartment> GetDepartments(long locationID)
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
                return retVal;
            }
        }

        public override List<PatientPaymentAccount> GetAllPatientPaymentAccounts()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPaymentAccounts_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<PatientPaymentAccount> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPatientPaymentAccountCollectionFromReader(reader);
                return retVal;
            }
        }

        public override List<ResourceGroup> GetAllResourceGroup()
        {
            List<ResourceGroup> listRG=null;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllResourceGroup", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@ParDeptID", SqlDbType.Decimal, 2);
                cn.Open();
                ResourceGroup p= null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    p.RscrGroupID=(long)reader["RscrGroupID"];
                    p.GroupName=reader["GroupName"].ToString();
                    p.Description=reader["Description"].ToString();
                    listRG.Add(p);
                }                
                return listRG;
            }
        }
        public override List<ResourceType> GetAllResourceType()
        {
            List<ResourceType> listRG=new List<ResourceType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllResourceType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                ResourceType p= null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    p.RscrTypeID=(long)reader["RscrTypeID"];
                    p.TypeName=reader["TypeName"].ToString();
                    p.Description=reader["Description"].ToString();
                    listRG.Add(p);
                }                
                
            }
            return listRG;
        }
        public override IList<DrugDeptProductGroupReportType> GetDrugDeptProductGroupReportTypes()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDrugDeptProductGroupReportTypes", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<DrugDeptProductGroupReportType> mDrugDeptMATReportTypeCollection = GetDrugDeptMATReportTypeCollectionFromReader(mReader);
                mReader.Close();
                return mDrugDeptMATReportTypeCollection;
            }
        }
    }
}
