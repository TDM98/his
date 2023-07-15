using System.Collections.Generic;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using eHCMS.Configurations;
using eHCMS.DAL;

namespace aEMR.DataAccessLayer.Providers
{
    public class DispMedRscrProviders : DataProviderBase
    {
        static private DispMedRscrProviders _instance = null;
        static public DispMedRscrProviders Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DispMedRscrProviders();
                }
                return _instance;
            }
        }
        public DispMedRscrProviders()
        {
            this.ConnectionString = Globals.Settings.DispMedRscr.ConnectionString;
        }
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

  

        public  List<InwardDMedRscrInvoice> GetAllInwardDMedRscrInvoice(int V_Reason, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInwardDMedRscrInvoice_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramReason = new SqlParameter("@V_Reason", SqlDbType.Int);
                paramReason.Value = V_Reason;

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.Int);
                paramPageSize.Value = pageSize;
                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.Int);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramOrderBy = new SqlParameter("@OrderBy", SqlDbType.NVarChar);
                paramOrderBy.Value = "";

                SqlParameter paramCountTotal = new SqlParameter("@CountTotal", SqlDbType.Bit);
                paramCountTotal.Value = bCountTotal;
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramReason);
                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramOrderBy);
                cmd.Parameters.Add(paramCountTotal);
                cmd.Parameters.Add(paramTotal);

                cn.Open();
                List<InwardDMedRscrInvoice> lst = null;

                IDataReader reader = ExecuteReader(cmd);

                lst = GetInwardDMedRscrInvoiceCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public  List<InwardDMedRscrInvoice> SearchInwardDMedRscrInvoice(int V_Reason, InwardInvoiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInwardDMedRscrInvoices_Search", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramInwardID = new SqlParameter("@InwardID", SqlDbType.VarChar, 128);
                paramInwardID.Value = ConvertNullObjectToDBNull(criteria.InwardID);
                SqlParameter paramInvoiveNumber = new SqlParameter("@InvoiceNumber", SqlDbType.VarChar, 128);
                paramInvoiveNumber.Value = ConvertNullObjectToDBNull(criteria.InvoiceNumber);
                SqlParameter paramDateInvoice = new SqlParameter("@fromdate", SqlDbType.DateTime);
                paramDateInvoice.Value = ConvertNullObjectToDBNull(criteria.FromDate);
                SqlParameter paramSupplierID = new SqlParameter("@SupplierID", SqlDbType.BigInt);
                paramSupplierID.Value = ConvertNullObjectToDBNull(criteria.SupplierID);
                SqlParameter paramDateInput = new SqlParameter("@todate", SqlDbType.DateTime);
                paramDateInput.Value = ConvertNullObjectToDBNull(criteria.ToDate);

                SqlParameter paramReason = new SqlParameter("@V_Reason", SqlDbType.Int);
                paramReason.Value = V_Reason;

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.Int);
                paramPageSize.Value = pageSize;
                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.Int);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramOrderBy = new SqlParameter("@OrderBy", SqlDbType.NVarChar);
                paramOrderBy.Value = ConvertNullObjectToDBNull(criteria.OrderBy);

                SqlParameter paramCountTotal = new SqlParameter("@CountTotal", SqlDbType.Bit);
                paramCountTotal.Value = bCountTotal;
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramInwardID);
                cmd.Parameters.Add(paramInvoiveNumber);
                cmd.Parameters.Add(paramDateInvoice);
                cmd.Parameters.Add(paramSupplierID);
                cmd.Parameters.Add(paramDateInput);

                cmd.Parameters.Add(paramReason);

                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramOrderBy);
                cmd.Parameters.Add(paramCountTotal);
                cmd.Parameters.Add(paramTotal);

                cn.Open();
                List<InwardDMedRscrInvoice> lst = null;

                IDataReader reader = ExecuteReader(cmd);

                lst = GetInwardDMedRscrInvoiceCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public  InwardDMedRscrInvoice GetInwardDMedRscrInvoiceByID(int V_Reason, long InvDMedRscrID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInwardDMedRscrInvoices_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@InvDMedRscrID", SqlDbType.BigInt, InvDMedRscrID);
                cmd.AddParameter("@V_Reason", SqlDbType.Int, V_Reason);
                cn.Open();
                InwardDMedRscrInvoice invoicedrug = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    invoicedrug = GetInwardDMedRscrInvoiceFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return invoicedrug;
            }
        }

        public  bool AddInwardDMedRscrInvoice(InwardDMedRscrInvoice InvoiceDrug, out long inwardid)
        {
            bool results = false;
            inwardid = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInwardDMedRscrInvoices_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SupplierID", SqlDbType.BigInt, InvoiceDrug.SelectedSupplier.SupplierID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, InvoiceDrug.SelectedStaff.StaffID);
                cmd.AddParameter("@InvDMedRscrNumber", SqlDbType.VarChar, InvoiceDrug.InvDMedRscrNumber);
                cmd.AddParameter("@DateInvDMedRscrNumber", SqlDbType.DateTime, InvoiceDrug.DateInvDMedRscrNumber);
                cmd.AddParameter("@V_Reason", SqlDbType.BigInt, InvoiceDrug.V_Reason);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    results = true;
                    inwardid = (long)reader["InvDMedRscrID"];
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public  bool UpdateInwardDMedRscrInvoice(InwardDMedRscrInvoice InvoiceDrug)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInwardDMedRscrInvoices_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@InvDMedRscrID", SqlDbType.BigInt, InvoiceDrug.InvDMedRscrID);
                cmd.AddParameter("@SupplierID", SqlDbType.BigInt, InvoiceDrug.SelectedSupplier.SupplierID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, InvoiceDrug.SelectedStaff.StaffID);
                cmd.AddParameter("@InvDMedRscrNumber", SqlDbType.VarChar, InvoiceDrug.InvDMedRscrNumber);
                cmd.AddParameter("@DateInvDMedRscrNumber", SqlDbType.DateTime, InvoiceDrug.DateInvDMedRscrNumber);
                cmd.AddParameter("@V_Reason", SqlDbType.BigInt, InvoiceDrug.V_Reason);
                cn.Open();
                int count = 0;
                count = cmd.ExecuteNonQuery();
                if (count != 0)
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public  bool DeleteInwardDMedRscrInvoice(long ID)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInwardDMedRscrInvoices_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@InvDMedRscrID", SqlDbType.BigInt, ID);
                cn.Open();
                int count = 0;
                count = cmd.ExecuteNonQuery();
                if (count != 0)
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        #region "Resource"
        public  List<Resource> GetAllResource(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_AllPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramPageSize = new SqlParameter("@PageSize", SqlDbType.Int);
                paramPageSize.Value = pageSize;

                SqlParameter paramPageIndex = new SqlParameter("@PageIndex", SqlDbType.Int);
                paramPageIndex.Value = pageIndex;

                SqlParameter paramOrderBy = new SqlParameter("@OrderBy", SqlDbType.NVarChar);
                paramOrderBy.Value = "";

                SqlParameter paramCountTotal = new SqlParameter("@CountTotal", SqlDbType.Bit);
                paramCountTotal.Value = bCountTotal;

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramPageSize);
                cmd.Parameters.Add(paramPageIndex);
                cmd.Parameters.Add(paramOrderBy);
                cmd.Parameters.Add(paramCountTotal);
                cmd.Parameters.Add(paramTotal);

                cn.Open();
                List<Resource> lst = null;

                IDataReader reader = ExecuteReader(cmd);

                lst = GetResourceCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && paramTotal.Value != null)
                {
                    totalCount = (int)paramTotal.Value;
                }
                else
                {
                    totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public  Resource GetResourceByID(long ResourceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetResourceByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ResourceID);
                cn.Open();
                Resource objResource = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    objResource = GetResourceFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objResource;
            }
        }

        public  bool AddResource(Resource objResource, out long ResourceID)
        {
            bool results = false;
            ResourceID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                //SqlCommand cmd = new SqlCommand("spResource_Insert", cn);
                //cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@DeprecTypeID", SqlDbType.TinyInt, objResource.DeprecTypeID);
                //cmd.AddParameter("@MedRescrTypeID", SqlDbType.BigInt, objResource.MedRescrTypeID);
                //cmd.AddParameter("@SupplierID", SqlDbType.BigInt, objResource.SupplierID);
                //cmd.AddParameter("@RscrItemCode", SqlDbType.VarChar, objResource.RscrItemCode);
                //cmd.AddParameter("@RscrName_Brand", SqlDbType.NVarChar, objResource.RscrName_Brand);
                //cmd.AddParameter("@RscrFunctions", SqlDbType.NVarChar, objResource.RscrFunctions);
                //cmd.AddParameter("@RscrTechInfo", SqlDbType.NVarChar, objResource.RscrTechInfo);
                //cmd.AddParameter("@RscrDepreciationRate", SqlDbType.Float, objResource.RscrDepreciationRate);
                //cmd.AddParameter("@RscrPrice", SqlDbType.Money, objResource.RscrPrice);
                //cmd.AddParameter("@V_RscrUnit", SqlDbType.NVarChar, objResource.V_RscrUnit);
                //cmd.AddParameter("@BeOfHIMedicineList", SqlDbType.Bit, objResource.BeOfHIMedicineList);
                //cmd.AddParameter("@ResourceType", SqlDbType.TinyInt, objResource.ResourceType);

                //cn.Open();
                //IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    results = true;
                //    ResourceID = (long)reader["ResourceID"];
                //}
                //reader.Close();
            }
            return results;
        }

        public  bool UpdateResource(Resource objResource)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@RscrID", SqlDbType.BigInt, objResource.RscrID);
                //cmd.AddParameter("@DeprecTypeID", SqlDbType.TinyInt, objResource.DeprecTypeID);
                //cmd.AddParameter("@MedRescrTypeID", SqlDbType.BigInt, objResource.MedRescrTypeID);
                //cmd.AddParameter("@SupplierID", SqlDbType.BigInt, objResource.SupplierID);
                //cmd.AddParameter("@RscrItemCode", SqlDbType.VarChar, objResource.RscrItemCode);
                //cmd.AddParameter("@RscrName_Brand", SqlDbType.NVarChar, objResource.RscrName_Brand);
                //cmd.AddParameter("@RscrFunctions", SqlDbType.NVarChar, objResource.RscrFunctions);
                //cmd.AddParameter("@RscrTechInfo", SqlDbType.NVarChar, objResource.RscrTechInfo);
                //cmd.AddParameter("@RscrDepreciationRate", SqlDbType.Float, objResource.RscrDepreciationRate);
                //cmd.AddParameter("@RscrPrice", SqlDbType.Money, objResource.RscrPrice);
                //cmd.AddParameter("@V_RscrUnit", SqlDbType.NVarChar, objResource.V_RscrUnit);
                //cmd.AddParameter("@BeOfHIMedicineList", SqlDbType.Bit, objResource.BeOfHIMedicineList);
                //cmd.AddParameter("@ResourceType", SqlDbType.TinyInt, objResource.ResourceType);

                cn.Open();
                int count = 0;
                count = cmd.ExecuteNonQuery();
                if (count != 0)
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public  bool DeleteResource(long ID, bool IsDeleted)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, IsDeleted);
                cn.Open();
                int count = 0;
                count = cmd.ExecuteNonQuery();
                if (count != 0)
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }
        #endregion
    }
}
