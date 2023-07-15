using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataEntities;
using System.Data.SqlClient;
using System.Threading;
using DataEntities.CustomDTOs;

/*
 * 20180508 #001 TxD: Commented out all BeginTransaction in this class because it could have caused dead lock in DB (suspection only at this stage)
 * 20180529 #002 TTM: Thêm parameter HIRepResourceCode.
 * 20180601 #003 TTM: Thêm 2 hàm Override cho GetAllResources và GetResourcesForMedicalServices_LoadNotPaging
*/

namespace eHCMS.DAL
{
    public class SqlResourcesManagement : ResourcesManagement
    {

        public SqlResourcesManagement()
            : base()
        {


        }
        #region Common
        public override List<Lookup> GetLookupRsrcUnit()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.RESOURCE_UNIT);
            return objLst;
        }
        public override List<Lookup> GetLookupAllocStatus()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.RESOURCE_ALLOC_STATUS);
            return objLst;
        }
        public override List<Lookup> GetLookupStorageStatus()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.RESOURCE_STORE_REASON);
            return objLst;
        }
        public override List<Lookup> GetLookupResGroupCategory()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.V_ResGroupCategory);
            return objLst;
        }
        public override List<Lookup> GetLookupSupplierType()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.V_SupplierType);
            return objLst;
        }
        //V_AllocStatus
        //public override List<ResourceGroup> GetRefResourceGroup()
        //{
        //    List<ResourceGroup> objLst = null;
        //    //objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.VSIGN_DATA_TYPE);
        //    return objLst;
        //}
        //public override List<ResourceType> GetRefResourceType()
        //{
        //    List<ResourceType> objLst = null;
        //    //objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.VSIGN_DATA_TYPE);
        //    return objLst;
        //}
        #endregion
        public override List<ResourceGroup> GetAllResourceGroup()
        {
            List<ResourceGroup> listRG = new List<ResourceGroup>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@ParDeptID", SqlDbType.Decimal, 2);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ResourceGroup p = new ResourceGroup();
                    p.RscrGroupID = (long)reader["RscrGroupID"];
                    p.GroupName = reader["GroupName"].ToString();
                    p.Description = reader["Description"].ToString();
                    listRG.Add(p);
                }
                reader.Close();
                return listRG;
            }
        }
        public override List<ResourceGroup> GetAllResourceGroupType(long V_ResGroupCategory)
        {
            List<ResourceGroup> listRG = new List<ResourceGroup>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_GetAllType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_ResGroupCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ResGroupCategory));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ResourceGroup p = new ResourceGroup();

                    if (reader.HasColumn("RscrGroupID") && reader["RscrGroupID"] != DBNull.Value)
                    {
                        p.RscrGroupID = (long)reader["RscrGroupID"];
                    }

                    if (reader.HasColumn("GroupName") && reader["GroupName"] != DBNull.Value)
                    {
                        p.GroupName = reader["GroupName"].ToString();
                    }

                    if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                    {
                        p.Description = reader["Description"].ToString();
                    }
                    p.VResGroupCategory = new Lookup();
                    if (reader.HasColumn("V_ResGroupCategory") && reader["V_ResGroupCategory"] != DBNull.Value)
                    {
                        p.V_ResGroupCategory = (long)reader["V_ResGroupCategory"];
                        p.VResGroupCategory.LookupID = (long)reader["V_ResGroupCategory"];
                    }

                    listRG.Add(p);
                }
                reader.Close();
                return listRG;
            }
        }
        public override List<ResourceType> GetAllResourceType()
        {
            List<ResourceType> listRG = new List<ResourceType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceTypes_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@ParDeptID", SqlDbType.Decimal, 2);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ResourceType p = new ResourceType();
                    p.RscrTypeID = (long)reader["RscrTypeID"];
                    p.TypeName = reader["TypeName"].ToString();
                    p.Description = reader["Description"].ToString();
                    listRG.Add(p);
                }
                reader.Close();
            }
            return listRG;
        }
        public override List<ResourceType> GetAllResourceTypeByGID(long RscrGroupID)
        {
            List<ResourceType> listRG = new List<ResourceType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceTypes_GetByGID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ResourceType p = new ResourceType();
                    p.RscrTypeID = (long)reader["RscrTypeID"];
                    p.TypeName = reader["TypeName"].ToString();
                    p.Description = reader["Description"].ToString();
                    listRG.Add(p);
                }
                reader.Close();
            }
            return listRG;
        }
        public override List<Resources> GetAllResource()
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResources_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetResourceCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listReVal;
        }
        //▼=====#003
        public override List<Resources> GetAllResources()
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllResources", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetResourceCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listReVal;
        }

        public override List<Resources> GetResourcesForMedicalServices_LoadNotPaging(long PCLExamTypeID)
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcesForMedicalServices_LoadNotPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourcesForMedicalServicesCollectionFromReader(reader);
                reader.Close();
            }
            return listReVal;
        }

        //▲=====#003
        public override List<Resources> GetAllResource_GetAllPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<Resources> listRs = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResources_GetAllPage", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetResourceCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listRs;
        }
        public override List<Resources> GetAllResourceByChoicePaging(int mChoice
                                               , long RscrID
                                               , string Text
                                                , long V_ResGroupCategory
                                               , int PageSize
                                                , int PageIndex
                                                , string OrderBy
                                                , bool CountTotal
                                                , out int Total)
        {

            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResources_GetByChoicePaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Choice", SqlDbType.TinyInt, ConvertNullObjectToDBNull(mChoice));
                cmd.AddParameter("@RsID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@Text", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Text));

                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));
                cmd.AddParameter("@V_ResGroupCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ResGroupCategory));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }
        public override List<Resources> GetAllResourceByAllFilterPage(long GroupID
                                                , long TypeID
                                                , long SuplierID
                                                , string RsName
                                                , string RsBrand
                                                , int PageSize
                                                , int PageIndex
                                                , string OrderBy
                                                , bool CountTotal
                                                , out int Total)
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResources_GetAllFilterPage", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@GroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupID));
                cmd.AddParameter("@TypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TypeID));
                cmd.AddParameter("@SuplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SuplierID));

                cmd.AddParameter("@RsName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RsName));
                cmd.AddParameter("@RsBrand", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RsBrand));

                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }


        public override List<Resources> GetAllResourceByChoice(int mChoice
                                               , long RscrID
                                               , string Text)
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResources_GetByChoice", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Choice", SqlDbType.TinyInt, ConvertNullObjectToDBNull(mChoice));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@Text", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Text));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceCollectionFromReader(reader);
                reader.Close();
            }
            return listReVal;
        }
        public override List<Supplier> GetAllSupplier()
        {
            List<Supplier> listReVal = new List<Supplier>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSupplier_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;



                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetSupplierCollectionFromReader(reader);
                reader.Close();
            }
            return listReVal;
        }

        public override List<Supplier> GetAllSupplierType(long V_SupplierType)
        {
            List<Supplier> listReVal = new List<Supplier>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSupplier_GetAllType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_SupplierType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_SupplierType));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetSupplierCollectionFromReader(reader);
                reader.Close();
            }
            return listReVal;
        }
        public override bool AddNewResourceGroup(string GroupName, string Description, long V_ResGroupCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GroupName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));
                cmd.AddParameter("@V_ResGroupCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ResGroupCategory));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }
        }
        public override bool UpdateResourceGroup(long RscrGroupID, string GroupName, string Description)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GroupName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }
        }
        public override bool DeleteResourceGroup(long RscrGroupID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }
        }
        public override bool AddNewResourceType(long RscrGroupID, string TypeName, string Description)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceTypes_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cmd.AddParameter("@TypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TypeName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }
        }
        public override bool UpdateResourceType(long RscrTypeID, long RscrGroupID, string TypeName, string Description)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceTypes_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrTypeID));
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cmd.AddParameter("@TypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TypeName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }


        }
        public override bool DeleteResourceType(long RscrTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRe_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrTypeID));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }

        }

        public override bool AddNewResources(int DeprecTypeID
                                                , long RscrGroupID
                                               , long RscrTypeID
                                               , long SupplierID
                                               , string ItemName
                                               , string ItemBrand
                                               , string Functions
                                               , string GeneralTechInfo
                                               , float DepreciationByTimeRate
                                               , float DepreciationByUsageRate
                                               , decimal BuyPrice
                                               , long V_RscrUnit
                                               , bool OnHIApprovedList
                                               , int WarrantyTime
                                               //▼=====#002
                                               , string HIRepResourceCode
                                               //▲=====#002
                                               , bool IsLocatable
                                               , bool IsDeleted
            , long V_ExpenditureSource)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeprecTypeID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(DeprecTypeID));
                cmd.AddParameter("@RscrGroupID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(@RscrGroupID));
                cmd.AddParameter("@RscrTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrTypeID));
                cmd.AddParameter("@ItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ItemName));
                cmd.AddParameter("@ItemBrand", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ItemBrand));
                cmd.AddParameter("@Functions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Functions));
                cmd.AddParameter("@GeneralTechInfo", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GeneralTechInfo));
                cmd.AddParameter("@SupplierID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(SupplierID));

                cmd.AddParameter("@DepreciationByTimeRate", SqlDbType.Float, ConvertNullObjectToDBNull(DepreciationByTimeRate));
                cmd.AddParameter("@DepreciationByUsageRate", SqlDbType.Float, ConvertNullObjectToDBNull(DepreciationByUsageRate));
                cmd.AddParameter("@BuyPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(BuyPrice));
                cmd.AddParameter("@V_RscrUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RscrUnit));
                cmd.AddParameter("@OnHIApprovedList", SqlDbType.Bit, ConvertNullObjectToDBNull(OnHIApprovedList));
                cmd.AddParameter("@WarrantyTime", SqlDbType.SmallInt, ConvertNullObjectToDBNull(WarrantyTime));
                //▼====#002
                cmd.AddParameter("@HIRepResourceCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(HIRepResourceCode));
                //▲====#002
                cmd.AddParameter("@IsLocatable", SqlDbType.Bit, ConvertNullObjectToDBNull(IsLocatable));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDeleted));

                cmd.AddParameter("@V_ExpenditureSource", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ExpenditureSource));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }


        }
        public override bool UpdateResources(long RscrID
                                                , int DeprecTypeID
                                                , long RscrGroupID
                                               , long RscrTypeID
                                               , long SupplierID
                                               , string ItemName
                                               , string ItemBrand
                                               , string Functions
                                               , string GeneralTechInfo
                                               , float DepreciationByTimeRate
                                               , float DepreciationByUsageRate
                                               , decimal BuyPrice
                                               , long V_RscrUnit
                                               , bool OnHIApprovedList
                                               , int WarrantyTime
                                               //▼=====#002
                                               , string HIRepResourceCode
                                               //▲=====#002
                                               , bool IsLocatable
                                               , bool IsDeleted
            , long V_ExpenditureSource)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@DeprecTypeID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(DeprecTypeID));
                cmd.AddParameter("@RscrGroupID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(RscrGroupID));
                cmd.AddParameter("@RscrTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrTypeID));
                cmd.AddParameter("@ItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ItemName));
                cmd.AddParameter("@ItemBrand", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ItemBrand));
                cmd.AddParameter("@Functions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Functions));
                cmd.AddParameter("@GeneralTechInfo", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GeneralTechInfo));
                cmd.AddParameter("@SupplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SupplierID));

                cmd.AddParameter("@DepreciationByTimeRate", SqlDbType.Float, ConvertNullObjectToDBNull(DepreciationByTimeRate));
                cmd.AddParameter("@DepreciationByUsageRate", SqlDbType.Float, ConvertNullObjectToDBNull(DepreciationByUsageRate));
                cmd.AddParameter("@BuyPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(BuyPrice));
                cmd.AddParameter("@V_RscrUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RscrUnit));
                cmd.AddParameter("@OnHIApprovedList", SqlDbType.Bit, ConvertNullObjectToDBNull(OnHIApprovedList));
                cmd.AddParameter("@WarrantyTime", SqlDbType.SmallInt, ConvertNullObjectToDBNull(WarrantyTime));
                //▼====#002
                cmd.AddParameter("@HIRepResourceCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(HIRepResourceCode));
                //▲====#002

                cmd.AddParameter("@IsLocatable", SqlDbType.Bit, ConvertNullObjectToDBNull(IsLocatable));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDeleted));
                cmd.AddParameter("@V_ExpenditureSource", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ExpenditureSource));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }

        }
        public override bool DeleteResources(long RscrID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_Delete", cn);
                bool IsDeleted = true;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@IsDeleted", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsDeleted));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }

        }

        #region Resources Allocations
        public override List<RefDepartment> GetAllRefDepartments()
        {
            List<RefDepartment> listRG = new List<RefDepartment>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRefDepartments_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listRG = GetRefDepartmentCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listRG;
        }
        public override List<RefDepartment> GetAllRefDepartmentsByType(int type)
        {
            List<RefDepartment> listRG = new List<RefDepartment>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_ByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Type", SqlDbType.Int, (type));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listRG = GetRefDepartmentCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listRG;
        }
        public override List<Location> GetAllLocations()
        {
            List<Location> listRG = new List<Location>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLocations_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listRG = GetLocationCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listRG;
        }
        public override List<Location> GetAllLocationsByType(int type)
        {
            List<Location> listRG = new List<Location>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLocations_AllByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Type", SqlDbType.Int, (type));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listRG = GetLocationCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listRG;
        }
        public override List<RoomType> GetAllRoomType()
        {
            List<RoomType> listReVal = new List<RoomType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRoomType_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetRoomTypeCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listReVal;
        }
        public override List<RefStaffCategory> GetRefStaffCategoriesByType(long V_StaffCatType)
        {
            List<RefStaffCategory> listReVal = new List<RefStaffCategory>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefStaffCategories_GetByType", cn);
                cmd.AddParameter("@V_StaffCatType", SqlDbType.BigInt, (V_StaffCatType));
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    RefStaffCategory p = new RefStaffCategory();
                    p.StaffCatgID = (long)reader["StaffCatgID"];
                    p.StaffCatgDescription = reader["StaffCatgDescription"].ToString();
                    listReVal.Add(p);
                }
                reader.Close();
                return listReVal;
            }
        }
        public override List<RefStaffCategory> GetAllRefStaffCategories()
        {
            List<RefStaffCategory> listReVal = new List<RefStaffCategory>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefStaffCategories_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    RefStaffCategory p = new RefStaffCategory();
                    p.StaffCatgID = (long)reader["StaffCatgID"];
                    p.StaffCatgDescription = reader["StaffCatgDescription"].ToString();
                    //20181208 TBL: Lay them V_StaffCatType de biet thuoc loai nhan vien nao de validate
                    if (reader.HasColumn("V_StaffCatType") && reader["V_StaffCatType"] != DBNull.Value)
                    {
                        p.V_StaffCatType = (long)reader["V_StaffCatType"];
                    }
                    listReVal.Add(p);
                }
                reader.Close();
                return listReVal;
            }
        }
        public override List<Staff> GetAllStaff(long StaffCatgID)
        {
            List<Staff> listReVal = new List<Staff>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_GetAllByStaffCatgID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffCatgID", SqlDbType.Int, (StaffCatgID));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    Staff p = new Staff();
                    p.StaffID = (long)reader["StaffID"];
                    p.DeptID = (long)reader["DeptID"];
                    p.StaffCatgID = (long)reader["StaffCatgID"];
                    p.FullName = reader["FullName"].ToString();
                    if (reader.HasColumn("UserAccountsName") && reader["UserAccountsName"]!=DBNull.Value)
                    {
                        p.UserAccountsName = reader["UserAccountsName"].ToString();    
                    }
                    
                    listReVal.Add(p);
                }
                reader.Close();
                return listReVal;
            }
        }

        public override List<Staff> GetAllStaffByStaffCategory(long StaffCategory)
        {
            List<Staff> listReVal = new List<Staff>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_GetAllByStaffCategory", cn);
               // SqlCommand cmd = new SqlCommand("spStaffs_GetAllStaffCatType", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffCategory", SqlDbType.Int, (StaffCategory));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetStaffCollectionFromReader(reader);
                reader.Close();
                return listReVal;
            }
        }
        public override List<DeptLocation> GetDeptLocationByExamType(long examTypeId)
        {
            if (examTypeId <= 0)
            {
                return null;
            }
            var listReVal = new List<DeptLocation>();
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_PclExamType_GetAllDeptLocations", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                cmd.AddParameter("@ExamTypeID", SqlDbType.BigInt, examTypeId);

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetDepartmentCollectionFromReader(reader);
                    reader.Close();

                }
            }
            return listReVal;
        }

        // Txd 16/11/2013 
        //public override List<PCLExamTypeLocation> GetPclExamTypeLocationsByExamTypeList(List<PCLExamType> examTypes)
        //{
        //    if(examTypes == null || examTypes.Count == 0)
        //    {
        //        return null;
        //    }
        //    var listReVal = new List<PCLExamTypeLocation>();
        //    using (var cn = new SqlConnection(this.ConnectionString))
        //    {
        //        var cmd = new SqlCommand("sp_PclExamType_GetAllPCLExamTypeLocations", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cn.Open();
        //        var idList = new IDListOutput<long>() { Ids = examTypes.Select(item => item.PCLExamTypeID).ToList() };
        //        cmd.AddParameter("@ExamTypeIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml(idList)));

        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader != null)
        //        {
        //            listReVal = GetPCLExamTypeLocationCollectionFromReader(reader);
        //            reader.Close();
                    
        //        }
        //    }
        //    return listReVal;
        //}

        public override List<PCLExamTypeLocation> GetPclExamTypeLocationsByExamTypeList(List<PCLExamType> examTypes)
        {
            if (ListAllPCLExamTypeLocations != null)
                return ListAllPCLExamTypeLocations;

            return null;
        }

        public override List<DeptLocation> GetAllDeptLocation()
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetDepartmentCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listReVal;
        }

        public override List<DeptLocation> GetAllDeptLocationByType(int choice)
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetAll_Type", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Choice", SqlDbType.BigInt, (choice));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetDepartmentCollectionFromReader(reader);
                }
                reader.Close();
            }
            return listReVal;
        }
        public override List<ResourceAllocations> GetResourceAllocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourceAllocations> listReVal = new List<ResourceAllocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceAllocations_PagingByDLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceAllocationsFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }
        public override List<ResourceStorages> GetResourceStoragesPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourceStorages> listReVal = new List<ResourceStorages>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceStorages_PagingByDLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceStoragesFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }
        public override List<DeptLocation> GetAllPageDeptLocation(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetAllPage", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetDepartmentCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }
        public override List<DeptLocation> GetAllPageDeptLocationByType(int choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetAllPage_ByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Choice", SqlDbType.Int, (choice));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetDepartmentCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }

        public override List<DeptLocation> GetAllPageDeptLocationGetByChoicePaging(int Choice, long RsID, string Text,
            int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetByChoicePaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.Int, (Choice));
                cmd.AddParameter("@RsID", SqlDbType.Int, (RsID));
                cmd.AddParameter("@Text", SqlDbType.NVarChar, (Text));


                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);



                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetDepartmentCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }
        public override bool AddNewStoragesAllocations(
                                                    long RscrID
                                                    , String GuidID
                                                    , long DeptLocationID
                                                   , long RscrMoveRequestID
                                                   , long StorageStaffID
                                                   , DateTime? StorageDate
                                                    , long V_StorageStatus
                                                    , long V_StorageReason
                                                   , bool HasIdentity
                                                   , string RscrCode
                                                   , string RscrBarcode
                                                   , string SerialNumber
                                                   , int QtyStorage
            //,int QtyInUse 
                                                   , long ResponsibleStaffID
                                                   , bool IsActive
                                                , bool IsDeleted)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceStorages_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(GuidID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@StorageStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StorageStaffID));
                cmd.AddParameter("@StorageDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StorageDate));

                cmd.AddParameter("@V_StorageStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageStatus));
                cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageReason));




                cmd.AddParameter("@HasIdentity", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HasIdentity));
                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));

                cmd.AddParameter("@QtyStorage", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyStorage));
                //cmd.AddParameter("@QtyInUse", SqlDbType.NVarChar, ConvertNullObjectToDBNull(QtyInUse));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDeleted));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }

        }
        public override bool AddNewResourceAllocations(
                                                    long RscrID
                                                    , String GuidID
                                                    , long DeptLocationID
                                                   , long RscrMoveRequestID
                                                   , long AllocStaffID
                                                   , DateTime? AllocDate
                                                   , DateTime? StartUseDate
                                                   , long V_AllocStatus
                                                   , bool HasIdentity
                                                   , string RscrCode
                                                   , string RscrBarcode
                                                   , string SerialNumber
                                                   , int QtyAlloc
                                                   , int QtyInUse
                                                   , long ResponsibleStaffID
                                                   , bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceAllocations_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(GuidID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                cmd.AddParameter("@HasIdentity", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HasIdentity));

                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));

                cmd.AddParameter("@QtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyAlloc));
                cmd.AddParameter("@QtyInUse", SqlDbType.NVarChar, ConvertNullObjectToDBNull(QtyInUse));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.NVarChar, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }

        }

        public override bool UpdateResourceAllocations(long RscrAllocID
                                                    , long RscrID
                                                    , String GuidID
                                                    , long DeptLocationID
                                                   , long RscrMoveRequestID
                                                   , long AllocStaffID
                                                   , DateTime? AllocDate
                                                   , DateTime? StartUseDate
                                                   , long V_AllocStatus
                                                   , bool HasIdentity
                                                   , string RscrCode
                                                   , string RscrBarcode
                                                   , string SerialNumber
                                                   , int QtyAlloc
                                                   , int QtyInUse
                                                   , long ResponsibleStaffID
                                                   , bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceAllocations_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrAllocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrAllocID));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(GuidID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                cmd.AddParameter("@HasIdentity", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HasIdentity));

                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));

                cmd.AddParameter("@QtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyAlloc));
                cmd.AddParameter("@QtyInUse", SqlDbType.NVarChar, ConvertNullObjectToDBNull(QtyInUse));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.NVarChar, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }

        }
        public override bool UpdateResourceStorages(
                                                 long RscrStorageID
                                                 , long RscrID
                                                 , string RscrGUID
                                                 , long DeptLocationID
                                                 , DateTime RecDateCreated
                                                 , long RscrMoveRequestID
                                                 , long StorageStaffID
                                                 , DateTime? StorageDate
                                                 , long V_StorageStatus
                                                 , long V_StorageReason
                                                 , bool HasIdentity
                                                 , string RscrCode
                                                 , String RscrBarcode
                                                 , string SerialNumber
                                                 , int QtyStorage
                                                 , long ResponsibleStaffID
                                                 , bool IsActive
                                                 , bool IsDeleted)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceStorages_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrStorageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrStorageID));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(RscrGUID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@StorageStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StorageStaffID));
                cmd.AddParameter("@StorageDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StorageDate));
                cmd.AddParameter("@V_StorageReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(V_StorageReason));
                cmd.AddParameter("@V_StorageStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageStatus));
                cmd.AddParameter("@HasIdentity", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HasIdentity));

                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));

                cmd.AddParameter("@QtyStorage", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyStorage));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDeleted));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.NVarChar, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }

        }
        public override bool ResourceAllocationsTranferExec(int Choice
                                                        , long RscrAllocID
                                                        , String GuidID
                                                                , long RscrID
                                                               , long DeptLocationID
                                                               , long RscrMoveRequestID
                                                               , long AllocStaffID
                                                               , DateTime? AllocDate
                                                               , DateTime? StartUseDate
                                                               , long V_AllocStatus
                                                               , bool HasIdentity
                                                               , string RscrCode
                                                               , string RscrBarcode
                                                               , string SerialNumber
                                                                , int NewQtyAlloc
                                                                , int NewQtyInUse
                                                               , long ResponsibleStaffID
                                                               , bool IsActive
            //------------
                                                                , long V_StorageReason
                                                                , long RequestStaffID
                                                               , string MoveReason
                                                               , DateTime? EffectiveMoveDate
                                                               , long FromDeptLocID
                                                               , long ToDeptLocID
                                                               , long ApprovedStaffID
                                                               , string Note

                                                    //------------
                                                               , int QtyAllocEx
                                                               , int QtyInUseEx
                                                    )
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceAllocations_TranferExec", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.Int, ConvertNullObjectToDBNull(Choice));
                cmd.AddParameter("@RscrAllocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrAllocID));

                cmd.AddParameter("@GuidID", SqlDbType.VarChar, ConvertNullObjectToDBNull(GuidID));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                cmd.AddParameter("@HasIdentity", SqlDbType.Bit, ConvertNullObjectToDBNull(HasIdentity));
                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));
                cmd.AddParameter("@NewQtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(NewQtyAlloc));
                cmd.AddParameter("@NewQtyInUse", SqlDbType.SmallInt, ConvertNullObjectToDBNull(NewQtyInUse));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));

                cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageReason));



                cmd.AddParameter("@RequestStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RequestStaffID));
                cmd.AddParameter("@MoveReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MoveReason));
                cmd.AddParameter("@EffectiveMoveDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(EffectiveMoveDate));
                cmd.AddParameter("@FromDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FromDeptLocID));
                cmd.AddParameter("@ToDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ToDeptLocID));
                cmd.AddParameter("@ApprovedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ApprovedStaffID));
                cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Note));

                cmd.AddParameter("@QtyAllocEx", SqlDbType.Int, ConvertNullObjectToDBNull(QtyAllocEx));
                cmd.AddParameter("@QtyInUseEx", SqlDbType.Int, ConvertNullObjectToDBNull(QtyInUseEx));


                cn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
        }

        #endregion

        #region New Resource Allocation

        public override bool AddNewResourceProperty(long RscrID
                                                    , string RscrGUID
                                                    , bool HasIdentity
                                                    , string RscrCode
                                                    , string RscrBarcode
                                                    , string SerialNumber
                                                    , int QtyAlloc
                                                    , bool IsActive
                                                    , bool IsDeleted
            //location
                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spResourceProperty_NewInsert", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RsID", SqlDbType.BigInt, ConvertNullObjectToDBNull(0));
                    cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                    cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(RscrGUID));
                    cmd.AddParameter("@HasIdentity", SqlDbType.Bit, ConvertNullObjectToDBNull(HasIdentity));
                    cmd.AddParameter("@RscrCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(RscrCode));
                    cmd.AddParameter("@RscrBarcode", SqlDbType.VarChar, ConvertNullObjectToDBNull(RscrBarcode));
                    cmd.AddParameter("@SerialNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(SerialNumber));
                    cmd.AddParameter("@QtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyAlloc));
                    cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));
                    cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDeleted));

                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                    cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                    cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                    cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                    cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                    cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                    cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageReason));
                    cmd.AddParameter("@ResponsibleStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }
        public override bool MoveResourcePropertyLocation(long RscrPropLocID
                                                    , long RscrPropertyID

                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spResourcePropLocations_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //SqlTransaction transaction;
                    cn.Open();
                    // =====▼ #001                                                        
                    // transaction = cn.BeginTransaction();
                    // =====▲ #001

                    cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrPropertyID));

                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                    cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                    cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                    cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                    cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                    cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                    cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageReason));
                    cmd.AddParameter("@ResponsibleStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                    cmd.AddParameter("@IsActive", SqlDbType.VarChar, ConvertNullObjectToDBNull(true));
                    cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(false));

                    SqlCommand cmd2 = new SqlCommand("spResourcePropLocations_SetActive", cn);

                    cmd2.CommandType = CommandType.StoredProcedure;

                    cmd2.AddParameter("@RscrPropLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrPropLocID));
                    cmd2.AddParameter("@IsActive", SqlDbType.VarChar, ConvertNullObjectToDBNull(false));
                    cmd2.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(false));

                    // =====▼ #001                                                                            
                    // cmd.Transaction = transaction;
                    // cmd2.Transaction = transaction;                                                 
                    // =====▲ #001
                    try
                    {
                        cmd.ExecuteNonQuery();

                        cmd2.ExecuteNonQuery();
                        // =====▼ #001                                                        
                        // transaction.Commit();                        
                        // =====▲ #001

                    }
                    catch
                    {
                        // =====▼ #001                                                        
                        // transaction.Rollback();                        
                        // =====▲ #001
                    }

                }
            }
            catch { return false; }
            return true;
        }
        public override bool BreakResourceProperty(List<ResourcePropLocations> lstResourcePropLocations, long RscrPropertyID, long RscrPropLocID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();
                    // SqlTransaction transaction;
                    cn.Open();

                    // =====▼ #001                                                                            
                    // transaction = cn.BeginTransaction();                                                    
                    // =====▲ #001

                    foreach (ResourcePropLocations rPL in lstResourcePropLocations)
                    {
                        SqlCommand cmd = new SqlCommand("spResourceProperty_NewInsert", cn);

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.AddParameter("@RsID", SqlDbType.BigInt, ConvertNullObjectToDBNull(0));
                        cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VRscrProperty.RscrID));
                        cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(rPL.VRscrProperty.RscrGUID));
                        cmd.AddParameter("@HasIdentity", SqlDbType.Bit, ConvertNullObjectToDBNull(rPL.VRscrProperty.HasIdentity));
                        cmd.AddParameter("@RscrCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(rPL.VRscrProperty.RscrCode));
                        cmd.AddParameter("@RscrBarcode", SqlDbType.VarChar, ConvertNullObjectToDBNull(rPL.VRscrProperty.RscrBarcode));
                        cmd.AddParameter("@SerialNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(rPL.VRscrProperty.SerialNumber));
                        cmd.AddParameter("@QtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(rPL.VRscrProperty.QtyAlloc));
                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(rPL.VRscrProperty.IsActive));
                        cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(rPL.VRscrProperty.IsDelete));

                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VDeptLocation.DeptLocationID));
                        cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VRscrMoveRequest.RscrMoveRequestID));
                        cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VAllocStaff.StaffID));
                        cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(rPL.AllocDate));
                        cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(rPL.StartUseDate));
                        cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VAllocStatus.LookupID));
                        cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VStorageReason.LookupID));
                        cmd.AddParameter("@ResponsibleStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VResponsibleStaff.StaffID));

                        // =====▼ #001                                                        
                        // cmd.Transaction = transaction;                                                    
                        // =====▲ #001

                        lstCmd.Add(cmd);
                    }
                    //status o day

                    SqlCommand cmd1 = new SqlCommand("spResourceProperty_SetActive", cn);
                    SqlCommand cmd2 = new SqlCommand("spResourcePropLocations_SetActive", cn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd1.AddParameter("@RscrPropertyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrPropertyID));
                    cmd1.AddParameter("@IsActive", SqlDbType.VarChar, ConvertNullObjectToDBNull(false));
                    cmd1.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(false));

                    cmd2.AddParameter("@RscrPropLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrPropLocID));
                    cmd2.AddParameter("@IsActive", SqlDbType.VarChar, ConvertNullObjectToDBNull(false));
                    cmd2.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(false));

                    // =====▼ #001                                                        
                    // cmd1.Transaction = transaction;
                    // cmd2.Transaction = transaction;
                    // =====▲ #001

                    try
                    {
                        foreach (SqlCommand sQLCmd in lstCmd)
                        {
                            sQLCmd.ExecuteNonQuery();
                        }
                        cmd1.ExecuteNonQuery();
                        cmd2.ExecuteNonQuery();
                        // =====▼ #001                                                                                
                        // transaction.Commit();                                                      
                        // =====▲ #001
                    }
                    catch
                    {
                        // =====▼ #001                                                                                
                        // transaction.Rollback();                                                  
                        // =====▲ #001
                    }
                }
            }
            catch { return false; }
            return true;
        }


        public override List<ResourcePropLocations> GetResourcePropLocationsPagingByVType(long Choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourcePropLocations> listReVal = new List<ResourcePropLocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcePropLocations_PagingByVType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.BigInt, (Choice));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourcePropLocationsFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }
        public override List<ResourcePropLocations> GetResourcePropLocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourcePropLocations> listReVal = new List<ResourcePropLocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcePropLocations_PagingByDLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourcePropLocationsFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }
        public override List<ResourcePropLocations> GetResourcePropLocationsPagingByFilter(int Choice, string RscrGUID, long RscrPropertyID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourcePropLocations> listReVal = new List<ResourcePropLocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcePropLocations_PagingByFilter", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.Int, (Choice));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, (RscrGUID));
                cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, (RscrPropertyID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourcePropLocationsFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }
        public override List<ResourceMaintenanceLogStatus> GetspResourceMaintenanceLogStatusByID(long RscrMaintLogID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourceMaintenanceLogStatus> listReVal = new List<ResourceMaintenanceLogStatus>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogStatus_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, (RscrMaintLogID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceMaintenanceLogStatusCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listReVal;
        }

        public override long GetResourcePropertyFilterSum(int Choice, string RscrGUID, long RscrID)
        {
            long Sum = 0;
            List<ResourcePropLocations> listReVal = new List<ResourcePropLocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceProperty_FilterSum", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.Int, (Choice));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, (RscrGUID));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, (RscrID));

                cn.Open();
                Object objectSum = cmd.ExecuteScalar();
                if (objectSum != null)
                {
                    Sum = long.Parse(objectSum.ToString());
                }
                else
                    return 0;
            }
            return Sum;
        }

        #endregion




        #region ResourceMaintenance

        public override List<Staff> GetStaffs_All()
        {
            List<Staff> lst = new List<Staff>();

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_Simple_AllCols", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetStaffCollectionFromReader_Simple(reader);
                }
                reader.Close();
            }
            return lst;
        }

        public override List<Supplier> GetSupplierIsMaintenance_All()
        {
            List<Supplier> lst = new List<Supplier>();

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetSupplierIsMaintenance", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetSupplierCollectionFromReader(reader);
                }
                reader.Close();
            }
            return lst;
        }

        public override bool AddNewResourceMaintenanceLog(ResourceMaintenanceLog obj, out long RscrMaintLogID)
        {
            bool results = false;
            RscrMaintLogID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLog_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, obj.RscrPropertyID);
                cmd.AddParameter("@LoggingDate", SqlDbType.DateTime, obj.LoggingDate);
                cmd.AddParameter("@LoggingIssue", SqlDbType.NVarChar, (obj.LoggingIssue == null ? "" : obj.LoggingIssue));
                cmd.AddParameter("@LoggerStaffID", SqlDbType.BigInt, obj.LoggerStaffID);
                cmd.AddParameter("@AssignStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.AssignStaffID));
                cmd.AddParameter("@ExternalFixSupplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.ExternalFixSupplierID));

                cmd.AddParameter("@Comments", SqlDbType.NVarChar, (obj.Comments == null ? "" : obj.Comments));

                cmd.AddParameter("@V_RscrInitialStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.V_RscrInitialStatus));

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                //IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    results = true;
                //    RscrMaintLogID = (long)reader["RscrMaintLogID"];
                //}
                //reader.Close();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    results = true;
            }
            return results;
        }
        public override List<ResourceMaintenanceLog> GetResourceMaintenanceLogSearch_Paging(
            ResourceMaintenanceLogSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            )
        {
            Total = 0;
            List<ResourceMaintenanceLog> lst = new List<ResourceMaintenanceLog>();

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogSearch_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.ToDate));
                cmd.AddParameter("@V_StatusIssueMaintenance", SqlDbType.BigInt, Criteria.V_StatusIssueMaintenance);
                cmd.AddParameter("@LoggingIssue", SqlDbType.NVarChar, Criteria.LoggingIssue);

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetResourceMaintenanceLogCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
            }
            return lst;
        }


        public override bool ResourceMaintenanceLogCanEdit(Int64 RscrMaintLogID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogCanEdit", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                cn.Open();

                DataTable dt = ExecuteDataTable(cmd);

                int iRec = 0;

                if (dt != null && dt.Rows.Count > 0)
                    iRec = Convert.ToInt16(dt.Rows[0][0]);
                if (iRec > 0)
                    return true;
                return false;
            }
        }

        public override ResourceMaintenanceLog GetResourceMaintenanceLogByID(Int64 RscrMaintLogID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetResourceMaintenanceDetail_Full", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                cn.Open();

                ResourceMaintenanceLog obj = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    obj = GetResourceMaintenanceLogFromReader(reader);
                }
                reader.Close();

                return obj;
            }
        }

        public override bool SaveResourceMaintenanceLogOfFix(
            Int64 RscrMaintLogID,
            Nullable<Int64> FixStaffID,
            Nullable<Int64> FixSupplierID,
            DateTime FixDate,
            string FixSolutions,
            string FixComments,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            )
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveResourceMaintenanceLogOfFix", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                cmd.AddParameter("@FixStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FixStaffID));

                cmd.AddParameter("@FixSupplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FixSupplierID));

                cmd.AddParameter("@FixDate", SqlDbType.DateTime, FixDate);

                cmd.AddParameter("@FixSolutions", SqlDbType.NVarChar, FixSolutions);

                cmd.AddParameter("@FixComments", SqlDbType.NVarChar, FixComments);

                cmd.AddParameter("@VerifiedStaffID", SqlDbType.BigInt, VerifiedStaffID);

                cmd.AddParameter("@V_CurrentStatus", SqlDbType.BigInt, V_CurrentStatus);

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                //int iRec = ExecuteNonQuery(cmd);
                //if(iRec > 0)
                //    return true;
                //return false;
                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
            }
            return false;
        }


        public override bool UpdateResourceMaintenanceLog_FinalStatus(
             Int64 RscrMaintLogID,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            )
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateResourceMaintenanceLog_FinalStatus", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                //cmd.AddParameter("@FixStaffID", SqlDbType.BigInt, FixStaffID);                

                cmd.AddParameter("@VerifiedStaffID", SqlDbType.BigInt, VerifiedStaffID);

                cmd.AddParameter("@V_CurrentStatus", SqlDbType.BigInt, V_CurrentStatus);

                cn.Open();

                int iRec = ExecuteNonQuery(cmd);
                if (iRec > 0)
                    return true;
                return false;
            }
        }

        public override bool CheckRscrDiBaoTri(Int64 RscrPropertyID)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckRscrDiBaoTri", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, RscrPropertyID);
                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                //IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    results = true;
                //    RscrMaintLogID = (long)reader["RscrMaintLogID"];
                //}
                //reader.Close();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal == 1)//Duoc phep di bao tri
                    results = true;
            }
            return results;
        }


        public override bool ResourceMaintenanceLog_Delete(Int64 RscrMaintLogID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLog_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);
                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal == 1)//Xóa thành công
                    return true;
                return false;
            }
        }


        public override bool GetCheckForVerified(Int64 RscrMaintLogID)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("GetCheckForVerified", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);
                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                //IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    results = true;
                //    RscrMaintLogID = (long)reader["RscrMaintLogID"];
                //}
                //reader.Close();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal == 1)//Duoc phep di bao tri
                    results = true;
            }
            return results;
        }



        public override bool DeleteAndCreateNewResourceMaintenanceLog(long RscrMaintLogID_Delete, ResourceMaintenanceLog obj)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                //////SqlCommand cmd = new SqlCommand("spResourceMaintenanceLog_Insert", cn);
                //////cmd.CommandType = CommandType.StoredProcedure;

                //////cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID_Delete);

                //////cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, obj.RscrPropertyID);
                //////cmd.AddParameter("@LoggingDate", SqlDbType.DateTime, obj.LoggingDate);
                //////cmd.AddParameter("@LoggingIssue", SqlDbType.NVarChar, (obj.LoggingIssue == null ? "" : obj.LoggingIssue));
                //////cmd.AddParameter("@LoggerStaffID", SqlDbType.BigInt, obj.LoggerStaffID);
                //////cmd.AddParameter("@AssignStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.AssignStaffID));
                //////cmd.AddParameter("@ExternalFixSupplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.ExternalFixSupplierID));

                //////cmd.AddParameter("@Comments", SqlDbType.NVarChar, (obj.Comments == null ? "" : obj.Comments));

                //////cmd.AddParameter("@V_RscrInitialStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.V_RscrInitialStatus));

                //////cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                //////cn.Open();

                ////////IDataReader reader = ExecuteReader(cmd);
                ////////if (reader.Read())
                ////////{
                ////////    results = true;
                ////////    RscrMaintLogID = (long)reader["RscrMaintLogID"];
                ////////}
                ////////reader.Close();

                //////ExecuteNonQuery(cmd);
                //////int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                //////if (ReturnVal > 0)
                //////    results = true;
            }
            return results;
        }












        public override List<ResourceMaintenanceLogStatus> GetResourceMaintenanceLogStatus_Detail_Paging(
            ResourceMaintenanceLogStatusSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetResourceMaintenanceLogStatus_Detail_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, Criteria.RscrMaintLogID);
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.ToDate));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<ResourceMaintenanceLogStatus> lst = null;

                IDataReader reader = ExecuteReader(cmd);

                lst = GetResourceMaintenanceLogStatusCollectionFromReader(reader);
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return lst;
            }
        }


        public override DateTime GetFixDateLast(Int64 RscrMaintLogID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetFixDateLast", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                cn.Open();

                DataTable dt = ExecuteDataTable(cmd);

                if (dt != null && dt.Rows.Count > 0)
                    return Convert.ToDateTime(dt.Rows[0]["FixDate"]);
                return DateTime.Now;
            }
        }


        public override bool AddNewResourceMaintenanceLogStatus(ResourceMaintenanceLogStatus obj)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogStatus_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, obj.RscrMaintLogID);

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);
                cn.Open();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    results = true;
            }
            return results;
        }
        //dinh them
        public override bool AddResourceMaintenanceLogStatus_New(ResourceMaintenanceLogStatus obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogStatus_InsertNew", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, obj.RscrMaintLogID);
                cmd.AddParameter("@StatusChangeDate", SqlDbType.DateTime, obj.StatusChangeDate);
                cmd.AddParameter("@UpdateStatusStaffID", SqlDbType.BigInt, obj.UpdateStatusStaffID);
                cmd.AddParameter("@V_CurrentStatus", SqlDbType.BigInt, obj.V_CurrentStatus);

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;

            }
        }

        public override bool DeleteResourceMaintenanceLogStatus(Int64 RscrMainLogStatusID)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogStatus_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMainLogStatusID", SqlDbType.BigInt, RscrMainLogStatusID);

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);

                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    results = true;
            }
            return results;
        }


        #endregion



    }
}
