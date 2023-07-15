using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Web;
using AxLogging;

/*
 * 20170512 #001 CMN: Added method to get pcl room by catid
 * 20180508 #002 TBLD: Added parameter HIApproved
 * 20180602 #003 TTM: Thêm Parameter XMLResources
*/

namespace eHCMS.DAL
{
    public class SqlConfigurationManagerProviders : ConfigurationManagerProviders
    {
        public SqlConfigurationManagerProviders()
            : base()
        {
        }

        #region RefDepartments member

        public override RefDepartments GetRefDepartmentsByID(long DeptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_ByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramDeptID = new SqlParameter("@DeptID", SqlDbType.BigInt);
                paramDeptID.Value = DeptID;

                cmd.Parameters.Add(paramDeptID);

                cn.Open();

                RefDepartments objRefDepartments = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    objRefDepartments = GetRefDepartmentsFromReader(reader);
                }

                reader.Close();
                return objRefDepartments;
            }
        }

        public override List<RefDepartments> GetRefDepartments_AllParent()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_AllParent", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefDepartments> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRefDepartmentsCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        public override List<RefDepartments> GetRefDepartments_All()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRefDepartments_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefDepartments> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRefDepartmentsCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        //List<RefDepartments> by collect V_DeptID
        public override List<RefDepartments> RefDepartments_ByInStrV_DeptType(string strV_DeptType, string V_DeptTypeOperation = null)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_ByInStrV_DeptType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@strV_DeptType", SqlDbType.NVarChar, strV_DeptType);
                cmd.AddParameter("@V_DeptTypeOperation", SqlDbType.NVarChar, V_DeptTypeOperation);

                cn.Open();
                List<RefDepartments> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRefDepartmentsCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        //List<RefDepartments> by collect V_DeptID


        public override List<RefDepartments> RefDepartments_ByParDeptID(Int64 ParDeptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_ByParDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ParDeptID", SqlDbType.NVarChar, ParDeptID);

                cn.Open();
                List<RefDepartments> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRefDepartmentsCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }


        public override bool DeleteRefDepartmentsByID(long DeptID)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, true);

                cn.Open();
                int count = 0;
                count = cmd.ExecuteNonQuery();
                if (count != 0)
                {
                    results = true;
                }
                cmd.Dispose();
            }
            return results;
        }

        public override void UpdateRefDepartments(RefDepartments obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRefDepartments_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, obj.DeptID);
                cmd.AddParameter("@ParDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.ParDeptID));
                cmd.AddParameter("@DeptName", SqlDbType.NVarChar, obj.DeptName);
                cmd.AddParameter("@V_DeptTypeOperation", SqlDbType.BigInt, obj.V_DeptTypeOperation);
                cmd.AddParameter("@DeptDescription", SqlDbType.NVarChar, obj.DeptDescription);

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override bool AddNewRefDepartments(RefDepartments obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ParDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.ParDeptID));
                cmd.AddParameter("@DeptName", SqlDbType.NVarChar, obj.DeptName);
                cmd.AddParameter("@V_DeptTypeOperation", SqlDbType.BigInt, obj.V_DeptTypeOperation);
                cmd.AddParameter("@DeptDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(obj.DeptDescription));

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }

        public override List<RefDepartments> SearchRefDepartments(RefDepartmentsSearchCriteria criteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_SearchTree", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptName", SqlDbType.NVarChar, criteria.DeptName);
                cn.Open();
                List<RefDepartments> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRefDepartmentsCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        public override List<DeptLocation> GetAllDeptLocationByDeptIDFunction(long DeptID, long RoomFunction)
        {
            List<DeptLocation> listRG = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetByDeptIDFunction", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cmd.AddParameter("@V_RoomFunction", SqlDbType.BigInt, RoomFunction);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetDepartmentCollectionFromReader(reader);

                reader.Close();
                return listRG;
            }
        }
        //---Dinh viet them ham nay---
        public override List<DeptLocation> GetDeptLocationFunc(long V_DeptType, long V_RoomFunction)
        {
            List<DeptLocation> listRG = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocationGetFunc", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_DeptType", SqlDbType.BigInt, V_DeptType);
                cmd.AddParameter("@V_RoomFunction", SqlDbType.BigInt, V_RoomFunction);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetDepartmentCollectionFromReader(reader);

                reader.Close();
                return listRG;
            }
        }

        public override List<DeptLocation> GetDeptLocationFuncExt(long V_DeptType, long V_RoomFunction, long V_DeptTypeOperation)
        {
            List<DeptLocation> listRG = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocationGetFuncExt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_DeptType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_DeptType));
                cmd.AddParameter("@V_RoomFunction", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RoomFunction));
                cmd.AddParameter("@V_DeptTypeOperation", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_DeptTypeOperation));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetDepartmentCollectionFromReader(reader);

                reader.Close();
                return listRG;
            }
        }

        public override List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID)
        {
            List<DeptLocation> listRG = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetDepartmentCollectionFromReader(reader);

                reader.Close();
                return listRG;
            }
        }

        public override List<DeptLocation> GetAllDeptLocLaboratory()
        {
            List<DeptLocation> listRG = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDeptLocLaboratory", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetDepartmentCollectionFromReader(reader);

                reader.Close();
                return listRG;
            }
        }



        //<GetRefDepartment in Table DeptMedServiceItems
        public override List<RefDepartments> GetRefDepartmentTree_InTable_DeptMedServiceItems()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRefDepartmentTree_InTable_DeptMedServiceItems", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<RefDepartments> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRefDepartmentsCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        //<GetRefDepartment in Table DeptMedServiceItems

        #endregion

        #region RefMedicalServiceItem

        public override List<RefMedicalServiceItem> RefMedicalServiceItemsByMedicalServiceTypeID(Int64 MedicalServiceTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItemsByMedicalServiceTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedicalServiceTypeID));

                cn.Open();
                List<RefMedicalServiceItem> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceItemCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }


        //
        public override List<RefMedicalServiceItem> RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(
     RefMedicalServiceItemsSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefMedicalServiceItem> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedicalServiceItemCollectionFromReader(reader);
                }
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
        //


        public override List<RefMedicalServiceItem> RefMedicalServiceItems_ByMedicalServiceTypeID_Paging(
        RefMedicalServiceItemsSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_ByMedicalServiceTypeID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefMedicalServiceItem> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedicalServiceItemCollectionFromReader(reader);
                }
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

        public override List<RefMedicalServiceItem> RefMedicalServiceItems_IsPCLByMedServiceID_Paging(
        RefMedicalServiceItemsSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_IsPCLByMedServiceID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefMedicalServiceItem> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedicalServiceItemCollectionFromReader(reader);
                }
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


        public override void RefMedicalServiceItems_IsPCL_Save(
        RefMedicalServiceItem Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_IsPCL_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedServiceID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName));
                cmd.AddParameter("@V_RefMedServiceItemsUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RefMedServiceItemsUnit));
                cmd.AddParameter("@ExpiryDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ExpiryDate));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void RefMedicalServiceItems_MarkDeleted(
         Int64 MedServiceID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_MarkDeleted", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void RefMedicalServiceItems_EditInfo(
   RefMedicalServiceItem Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_EditInfo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DeptID));
                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedServiceID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName));
                cmd.AddParameter("@V_RefMedServiceItemsUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RefMedServiceItemsUnit));
                cmd.AddParameter("@HITTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HITTypeID));

                cmd.AddParameter("@V_AppointmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_AppointmentType));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void RefMedicalServiceItems_NotPCL_Add(DeptMedServiceItems Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_NotPCL_Add", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedServiceID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ObjRefMedicalServiceItem.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ObjRefMedicalServiceItem.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ObjRefMedicalServiceItem.MedServiceName));
                cmd.AddParameter("@V_RefMedServiceItemsUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RefMedServiceItemsUnit));
                cmd.AddParameter("@HITTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ObjRefMedicalServiceItem.HITTypeID));
                cmd.AddParameter("@ExpiryDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ObjRefMedicalServiceItem.ExpiryDate));

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DeptID));

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.StaffID));
                cmd.AddParameter("@ApprovedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.ApprovedStaffID));
                cmd.AddParameter("@VATRate", SqlDbType.Float, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.VATRate));
                cmd.AddParameter("@NormalPrice", SqlDbType.Money, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.NormalPrice));
                cmd.AddParameter("@PriceForHIPatient", SqlDbType.Money, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.PriceForHIPatient));
                cmd.AddParameter("@PriceDifference", SqlDbType.Money, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.PriceDifference));
                cmd.AddParameter("@HIAllowedPrice", SqlDbType.Money, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.HIAllowedPrice));
                cmd.AddParameter("@EffectiveDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.EffectiveDate));
                cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ObjMedServiceItemPrice.Note));

                cmd.AddParameter("@V_AppointmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ObjApptService.V_AppointmentType));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override bool RefMedicalServiceItems_NotPCL_Add(RefMedicalServiceItem Obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_NotPCL_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DeptID));
                    cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeID));
                    cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceCode));
                    cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName));
                    cmd.AddParameter("@V_RefMedServiceItemsUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RefMedServiceItemsUnit));
                    cmd.AddParameter("@HITTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HITTypeID));

                    cmd.AddParameter("@ExpiryDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ExpiryDate));

                    //Hpt 12/11/2015: Thêm loại giá dịch vụ
                    cmd.AddParameter("@V_NewPriceType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_NewPriceType));
                    cmd.AddParameter("@IsPackageService", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsPackageService));

                    //cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);
                    //Hpt 24/08/2016: Thêm mã 5084
                    cmd.AddParameter("@HICode5084", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode5084 != null ? Obj.HICode5084.Trim() : null));
                    cmd.AddParameter("@MedServiceName_Ax", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName_Ax));
                    // tai 07/08/2017 thêm mã HICode
                    cmd.AddParameter("HICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode));
                    /*TMA*/
                    cmd.AddParameter("@V_Surgery_Tips_Item", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Surgery_Tips_Item));
                    cmd.AddParameter("@V_Surgery_Tips_Type", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Surgery_Tips_Type));
                    /*TMA*/
                    /*▼====: #002*/
                    cmd.AddParameter("@HIApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.HIApproved));
                    /*▲====: #002*/
                    cn.Open();

                    ExecuteNonQuery(cmd);
                    return true;
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(ex.Message);
                }

            }
        }

        public override bool RefMedicalServiceItems_NotPCL_Update(RefMedicalServiceItem Obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    
                    SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_NotPCL_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    //cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DeptID));
                    cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedServiceID));
                    cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeID));
                    cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceCode));
                    cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName));
                    cmd.AddParameter("@V_RefMedServiceItemsUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RefMedServiceItemsUnit));
                    cmd.AddParameter("@HITTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HITTypeID));
                    cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));
                    cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsDeleted));
                    cmd.AddParameter("@V_AppointmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_AppointmentType));

                    cmd.AddParameter("@ExpiryDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ExpiryDate));

                    //Hpt 12/11/2015: Thêm loại giá dịch vụ
                    cmd.AddParameter("@V_NewPriceType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_NewPriceType));
                    cmd.AddParameter("@IsPackageService", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsPackageService));
                    cmd.AddParameter("@HICode5084", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode5084 != null ? Obj.HICode5084.Trim() : null));
                    cmd.AddParameter("@MedServiceName_Ax", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName_Ax));
                    //tai 07/08/2017: thêm mã bảo hiểm
                    cmd.AddParameter("@HICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode));
                    cmd.AddParameter("@UpdatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.UpdatedStaffID));
                    cmd.AddParameter("@UpdatedTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.UpdatedTime));
                    /*TMA*/
                    cmd.AddParameter("@V_Surgery_Tips_Item", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Surgery_Tips_Item));
                    cmd.AddParameter("@V_Surgery_Tips_Type", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Surgery_Tips_Type));
                    /*TMA*/
                    /*▼====: #002*/
                    cmd.AddParameter("@HIApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.HIApproved));
                    /*▲====: #002*/
                    cn.Open();

                    ExecuteNonQuery(cmd);
                    return true;
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(ex.Message);
                }

            }
        }


        public override List<RefMedicalServiceItem> RefMedicalServiceItems_In_DeptLocMedServices(
        RefMedicalServiceItemsSearchCriteria SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_In_DeptLocMedServices", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.DeptLocationID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.MedServiceName));


                cn.Open();
                List<RefMedicalServiceItem> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceItemCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<RefMedicalServiceItem> GetMedServiceItems_Paging(
           RefMedicalServiceItemsSearchCriteria Criteria,
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
                SqlCommand cmd = new SqlCommand("spGetRefMedServiceItems_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefMedicalServiceItem> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceItemCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return objList;
            }
        }
        #endregion

        #region "PCLExamGroup"
        public override List<PCLExamGroup> GetPCLExamGroup_ByMedServiceID_NoPaging(Int64 MedServiceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPCLExamGroup_ByMedServiceID_NoPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, MedServiceID);

                cn.Open();
                List<PCLExamGroup> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetExamGroupCollectionsFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        public override List<RefDepartments> PCLExamGroup_GetListDeptID()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamGroup_GetListDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<RefDepartments> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRefDepartmentsCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        #endregion

        #region "PCLExamTypes"


        public override List<PCLExamType> PCLExamTypes_NotYetPCLLabResultSectionID_Paging
           (
           PCLExamTypeSearchCriteria Criteria,
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
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_NotYetPCLLabResultSectionID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.PCLExamTypeName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLExamType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeColectionFromReader(reader);
                }
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


        public override List<PCLExamType> GetPCLExamTypes_Paging
           (
           PCLExamTypeSearchCriteria Criteria,
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
                SqlCommand cmd = new SqlCommand("spGetPCLExamTypes_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.PCLExamTypeName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLExamType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeColectionFromReader(reader);
                }
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

        //public override List<RefMedicalServiceItem> PCLExamTypes_GetListMedServiceID()
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLExamTypes_GetListMedServiceID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cn.Open();
        //        List<RefMedicalServiceItem> objList = null;

        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader != null)
        //        {
        //            objList = GetMedicalServiceItemCollectionFromReader(reader);
        //        }
        //        reader.Close();
        //        return objList;
        //    }
        //}



        public override List<PCLExamType> PCLExamTypesAndPriceIsActive_Paging(
          PCLExamTypeSearchCriteria SearchCriteria,

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
                SqlCommand cmd = new SqlCommand("spPCLExamTypesAndPriceIsActive_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLExamType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeColectionFromReader(reader);
                }
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


        public override void PCLExamTypes_Save_NotIsLab(
       PCLExamType Obj, out string Result, out long PCLExamTypeID_New)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    Result = "";
                    PCLExamTypeID_New = 0;
                    bool IsUpdate = (Obj.PCLExamTypeID > 0);

                    SqlCommand cmd = new SqlCommand("spPCLExamTypes_Save_NotIsLab", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeID));
                    cmd.AddParameter("@PCLSectionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLSectionID));
                    cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLResultParamImpID));
                    cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeSubCategoryID));
                    cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeName));
                    cmd.AddParameter("@PCLExamTypeDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeDescription));
                    cmd.AddParameter("@PCLExamTypeCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeCode));
                    cmd.AddParameter("@V_PCLExamTypeUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_PCLExamTypeUnit));
                    cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsExternalExam));
                    cmd.AddParameter("@HosIDofExternalExam", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HosIDofExternalExam));
                    cmd.AddParameter("@HIApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.HIApproved));
                    cmd.AddParameter("@HITTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HITTypeID));
                    cmd.AddParameter("@HIIssueCode1", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HIIssueCode1));
                    cmd.AddParameter("@HIIssueCode2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HIIssueCode2));
                    cmd.AddParameter("@HICode5084", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode5084 != null ? Obj.HICode5084.Trim() : null));
                    cmd.AddParameter("@HICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode));
                    cmd.AddParameter("@PCLExamTypeName_Ax", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeName_Ax));
                    //▼=====#003
                    cmd.AddParameter("@Resource", SqlDbType.Xml, ConvertNullObjectToDBNull(Obj.ConvertDetailsListToXmlForResources()));
                    //▲=====#003
                    // Hpt 25/11/2015: Lưu loại giá dịch vụ
                    cmd.AddParameter("@V_NewPriceType", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.V_NewPriceType));

                    cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);
                    cmd.AddParameter("@PCLExamTypeID_New", SqlDbType.BigInt, sizeof(Int64), ParameterDirection.Output);
                    cn.Open();

                    ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@Result"].Value != null)
                        Result = cmd.Parameters["@Result"].Value.ToString();
                    // Hpt 28/11/2015: Sau khi lưu xuống, Stored trả về ID của dòng mới để thực hiện cập nhật trực tiếp MAPPCLExamTypeDeptLoc mà không cần restart Service. (CHỈ ÁP DỤNG CẬP NHẬT, LƯU MỚI TỪ TỪ TÍNH)
                    // Nếu không cập nhật danh sách MAPPCLExamTypeDeptLoc sau khi cập nhật dịch vụ, khi lưu bill chứa dịch vụ mới cấu hình lại mà chưa restart Service sẽ không lấy ra được danh sách phòng thực hiện dịch vụ --> Error
                    if (IsUpdate && cmd.Parameters["@PCLExamTypeID_New"].Value != null)
                    {
                        Obj.ObjDeptLocationList = DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID].ObjDeptLocationList;
                        PCLExamTypeID_New = Convert.ToInt64(cmd.Parameters["@PCLExamTypeID_New"].Value);
                        Obj.PCLExamTypeID = PCLExamTypeID_New;
                        DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID] = Obj;
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }


        public void PCLExamTypes_Save_IsLab_Old(
       PCLExamType Obj,

            /*ExamType la ExamTest*/
            bool TestItemIsExamType,
            string PCLExamTestItemUnitForPCLExamType,
            string PCLExamTestItemRefScaleForPCLExamType,
            /*ExamType la ExamTest*/

            IEnumerable<PCLExamTestItems> DataPCLExamTestItems_Insert,
            IEnumerable<PCLExamTestItems> DataPCLExamTestItems_Update,
            IEnumerable<PCLExamTestItems> DataPCLExamTestItems_Delete,
            out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypes_Save_IsLab", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeID));
                cmd.AddParameter("@PCLSectionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLSectionID));
                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLResultParamImpID));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeName.Trim()));
                cmd.AddParameter("@PCLExamTypeDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeDescription));
                cmd.AddParameter("@PCLExamTypeCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeCode.Trim()));
                cmd.AddParameter("@V_PCLExamTypeUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_PCLExamTypeUnit));
                cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsExternalExam));
                cmd.AddParameter("@HosIDofExternalExam", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HosIDofExternalExam));
                cmd.AddParameter("@HIApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.HIApproved));
                cmd.AddParameter("@HITTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HITTypeID));
                cmd.AddParameter("@HIIssueCode1", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HIIssueCode1));
                cmd.AddParameter("@HIIssueCode2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HIIssueCode2));
                cmd.AddParameter("@V_NewPriceType", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.V_NewPriceType));

                /*ExamType la ExamTest*/
                cmd.AddParameter("@TestItemIsExamType", SqlDbType.Bit, ConvertNullObjectToDBNull(TestItemIsExamType));
                cmd.AddParameter("@PCLExamTestItemUnitForPCLExamType", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTestItemUnitForPCLExamType));
                cmd.AddParameter("@PCLExamTestItemRefScaleForPCLExamType", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTestItemRefScaleForPCLExamType));
                /*ExamType la ExamTest*/

                cmd.AddParameter("@DataPCLExamTestItems_Insert", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertListPCLExamTestItemsToXml(DataPCLExamTestItems_Insert)));
                cmd.AddParameter("@DataPCLExamTestItems_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertListPCLExamTestItemsToXml(DataPCLExamTestItems_Update)));
                cmd.AddParameter("@DataPCLExamTestItems_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertListPCLExamTestItemsToXml(DataPCLExamTestItems_Delete)));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void PCLExamTypes_Save_IsLab(
     PCLExamType Obj,

          /*ExamType la ExamTest*/
          bool TestItemIsExamType,
          string PCLExamTestItemUnitForPCLExamType,
          string PCLExamTestItemRefScaleForPCLExamType,
            /*ExamType la ExamTest*/

          IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
          IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
          IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete,
          out string Result, out long PCLExamTypeID_New)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    ;
                    Result = "";
                    PCLExamTypeID_New = 0;
                    bool IsUpdate = (Obj.PCLExamTypeID != null && Obj.PCLExamTypeID > 0);
                    SqlCommand cmd = new SqlCommand("spPCLExamTypes_Save_IsLab", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeID));
                    cmd.AddParameter("@PCLSectionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLSectionID));
                    cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLResultParamImpID));
                    cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeSubCategoryID));
                    cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeName.Trim()));
                    cmd.AddParameter("@PCLExamTypeDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeDescription.Trim()));
                    cmd.AddParameter("@PCLExamTypeCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeCode.Trim()));
                    cmd.AddParameter("@V_PCLExamTypeUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_PCLExamTypeUnit));
                    cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsExternalExam));
                    cmd.AddParameter("@HosIDofExternalExam", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HosIDofExternalExam));
                    cmd.AddParameter("@HIApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.HIApproved));
                    cmd.AddParameter("@HITTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HITTypeID));
                    cmd.AddParameter("@HIIssueCode1", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HIIssueCode1));
                    cmd.AddParameter("@HIIssueCode2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HIIssueCode2));
                    cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLFormID));
                    cmd.AddParameter("@HICode5084", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode5084 != null ? Obj.HICode5084.Trim() : null));
                    // Hpt 25/11/2015: Lưu loại giá
                    cmd.AddParameter("@V_NewPriceType", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.V_NewPriceType));

                    /*ExamType la ExamTest*/
                    cmd.AddParameter("@TestItemIsExamType", SqlDbType.Bit, ConvertNullObjectToDBNull(TestItemIsExamType));
                    cmd.AddParameter("@PCLExamTestItemUnitForPCLExamType", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTestItemUnitForPCLExamType));
                    cmd.AddParameter("@PCLExamTestItemRefScaleForPCLExamType", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTestItemRefScaleForPCLExamType));
                    /*ExamType la ExamTest*/
                    //▼=====#003
                    cmd.AddParameter("@Resource", SqlDbType.Xml, ConvertNullObjectToDBNull(Obj.ConvertDetailsListToXmlForResources()));
                    //▲=====#003
                    cmd.AddParameter("@DataPCLExamTestItems_Insert", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertListPCLExamTypeTestItemsToXml(DataPCLExamTestItems_Insert)));
                    cmd.AddParameter("@DataPCLExamTestItems_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertListPCLExamTypeTestItemsToXml(DataPCLExamTestItems_Update)));
                    cmd.AddParameter("@DataPCLExamTestItems_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertListPCLExamTypeTestItemsToXml(DataPCLExamTestItems_Delete)));

                    cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);
                    cmd.AddParameter("@PCLExamTypeID_New", SqlDbType.BigInt, 10000, ParameterDirection.Output);
                    cmd.AddParameter("@HICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode));
                    cmd.AddParameter("@PCLExamTypeName_Ax", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeName_Ax));
                    cn.Open();

                    ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@Result"].Value != null)
                        Result = cmd.Parameters["@Result"].Value.ToString();
                    if (IsUpdate && cmd.Parameters["@PCLExamTypeID_New"].Value != null)
                    {
                        Obj.ObjDeptLocationList = DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID].ObjDeptLocationList;
                        Obj.PCLExamTypeID = Convert.ToInt64(cmd.Parameters["@PCLExamTypeID_New"].Value);
                        DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID] = Obj;
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }

        public override void PCLExamTypes_MarkDelete(Int64 PCLExamTypeID, out string Result)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    Result = "";

                    SqlCommand cmd = new SqlCommand("spPCLExamTypes_MarkDelete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));

                    cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                    cn.Open();

                    ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@Result"].Value != null)
                        Result = cmd.Parameters["@Result"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public override List<PCLExamType> PCLExamTypes_List_Paging(
        PCLExamTypeSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_List_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@IsNotInPCLItems", SqlDbType.Bit, ConvertNullObjectToDBNull(Criteria.IsNotInPCLItems));
                cmd.AddParameter("@IsNotInPCLExamTypeLocations", SqlDbType.Bit, ConvertNullObjectToDBNull(Criteria.IsNotInPCLExamTypeLocations));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.PCLExamTypeName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLExamType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeColectionFromReader(reader);
                }
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


        public override List<PCLExamType> PCLExamTypesByDeptLocationID_LAB(long DeptLocationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypesByDeptLocationID_LAB", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cn.Open();
                List<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<PCLExamType> PCLExamTypesByDeptLocationID_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID, long DeptLocationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypesByDeptLocationID_NotLAB", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeSubCategoryID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cn.Open();
                List<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }


        #endregion

        #region PCLExamTypeCombo

        public override List<PCLExamTypeCombo> PCLExamTypeCombo_Search(GeneralSearchCriteria SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeCombo_Search", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ComboName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FindName));
                cn.Open();
                List<PCLExamTypeCombo> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeComboColectionFromReader(reader);
                }
                reader.Close();
                return lst;
            }
        }

        public override List<PCLExamTypeComboItem> PCLExamTypeComboItems_ByComboID(long ComboID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeComboItems_ByComboID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeComboID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ComboID));
                cn.Open();
                List<PCLExamTypeComboItem> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeComboItemColectionFromReader(reader);
                }
                reader.Close();
                return lst;
            }
        }

        public override List<PCLExamTypeComboItem> PCLExamTypeComboItems_All()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeComboItems_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<PCLExamTypeComboItem> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeComboItemColectionFromReader(reader);
                }
                reader.Close();
                return lst;
            }
        }

        public override bool PCLExamTypeCombo_Save(PCLExamTypeCombo item, List<PCLExamTypeComboItem> ComboXML_Insert, List<PCLExamTypeComboItem> ComboXML_Update, List<PCLExamTypeComboItem> ComboXML_Delete, out long ID)
        {
            ID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeCombo_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeComboID", SqlDbType.BigInt, ConvertNullObjectToDBNull(item.PCLExamTypeComboID));
                cmd.AddParameter("@CreatorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(item.CreatorStaffID));
                cmd.AddParameter("@ComboName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(item.ComboName));
                cmd.AddParameter("@ComboDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(item.ComboDescription));


                cmd.AddParameter("@ComboXML_Insert", SqlDbType.Xml, ConvertNullObjectToDBNull(GetPCLExamTypeComboItemToXML(ComboXML_Insert).ToString()));
                cmd.AddParameter("@ComboXML_Update", SqlDbType.Xml, ConvertNullObjectToDBNull(GetPCLExamTypeComboItemToXML(ComboXML_Update).ToString()));
                cmd.AddParameter("@ComboXML_Delete", SqlDbType.Xml, ConvertNullObjectToDBNull(GetPCLExamTypeComboItemToXML(ComboXML_Delete).ToString()));
                cmd.AddParameter("@ID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                cmd.ExecuteNonQuery();
                if (cmd.Parameters["@ID"].Value != DBNull.Value)
                {
                    ID = (long)cmd.Parameters["@ID"].Value;
                }
                return ID > 0;
            }
        }

        public override bool PCLExamTypeCombo_Delete(long ID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeCombo_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeComboID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ID));
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region PCLExamTypeMedServiceDefItems

        public override List<PCLExamType> PCLExamTypeMedServiceDefItems_ByMedServiceID(PCLExamTypeSearchCriteria SearchCriteria, Int64 MedServiceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeMedServiceDefItems_ByMedServiceID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));

                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeName));
                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, MedServiceID);/*Gói CLSàng nào*/

                cn.Open();

                List<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        //Danh sach da chon co trong DB


        //Save danh sách tập hợp
        public override bool PCLExamTypeMedServiceDefItems_XMLInsert(Int64 MedServiceID, IEnumerable<PCLExamType> ObjPCLExamTypeList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spPCLExamTypeMedServiceDefItems_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.AddParameter("@MedServiceID", SqlDbType.BigInt, MedServiceID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertListPCLExamTypeMedServiceDefItemsToXml(MedServiceID, ObjPCLExamTypeList));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
                return false;
            }

        }
        public string ConvertListPCLExamTypeMedServiceDefItemsToXml(Int64 MedServiceID, IEnumerable<PCLExamType> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PCLExamType details in items)
                {
                    sb.Append("<PCLExamTypeMedServiceDefItems>");
                    sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", details.PCLExamTypeID);
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", MedServiceID);
                    sb.AppendFormat("<IsActive>{0}</IsActive>", 1);
                    sb.Append("</PCLExamTypeMedServiceDefItems>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public string ConvertListPCLExamTestItemsToXml(IEnumerable<PCLExamTestItems> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                if (ObjList.Count() > 0)
                {
                    sb.Append("<PCLExamTestItems>");
                    foreach (PCLExamTestItems item in ObjList)
                    {
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<PCLExamTestItemID>{0}</PCLExamTestItemID>", item.PCLExamTestItemID);
                        sb.AppendFormat("<PCLExamTestItemName>{0}</PCLExamTestItemName>", ConvertNullObjectToDBNull(ConvertHTMLCharacters(item.PCLExamTestItemName.Trim())));
                        sb.AppendFormat("<PCLExamTestItemDescription>{0}</PCLExamTestItemDescription>", ConvertNullObjectToDBNull(ConvertHTMLCharacters(item.PCLExamTestItemDescription.Trim())));
                        sb.AppendFormat("<PCLExamTestItemCode>{0}</PCLExamTestItemCode>", ConvertNullObjectToDBNull(ConvertHTMLCharacters(item.PCLExamTestItemCode.Trim())));
                        sb.AppendFormat("<PCLExamTestItemUnit>{0}</PCLExamTestItemUnit>", ConvertNullObjectToDBNull(ConvertHTMLCharacters(item.PCLExamTestItemUnit.Trim())));
                        sb.AppendFormat("<PCLExamTestItemRefScale>{0}</PCLExamTestItemRefScale>", ConvertNullObjectToDBNull(ConvertHTMLCharacters(item.PCLExamTestItemRefScale.Trim())));
                        sb.Append("</RecInfo>");
                    }
                    sb.Append("</PCLExamTestItems>");
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public string ConvertListPCLExamTypeTestItemsToXml(IEnumerable<PCLExamTypeTestItems> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                if (ObjList.Count() > 0)
                {
                    sb.Append("<PCLExamTestItems>");
                    foreach (PCLExamTypeTestItems item in ObjList)
                    {
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<PCLExamTestItemID>{0}</PCLExamTestItemID>", item.PCLExamTestItemID);
                        sb.AppendFormat("<PCLExamTypeTestItemID>{0}</PCLExamTypeTestItemID>", item.PCLExamTypeTestItemID);
                        sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", item.PCLExamTypeID);
                        sb.Append("</RecInfo>");
                    }
                    sb.Append("</PCLExamTestItems>");
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private String ConvertHTMLCharacters(string Str)
        {
            return System.Security.SecurityElement.Escape(Str);
        }

        //Save danh sách tập hợp


        #endregion

        #region "DeptMedServiceItems"
        public override List<MedServiceItemPrice> GetDeptMedServiceItems_Paging(
            DeptMedServiceItemsSearchCriteria Criteria,
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
                SqlCommand cmd = new SqlCommand("spGetDeptMedServiceItems_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedServiceItemPriceListID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<MedServiceItemPrice> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedServiceItemPriceCollectionFromReader(reader);
                    //lst = GetDeptMedServiceItemsCollectionFromReader(reader); 
                }
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

        public override List<DeptMedServiceItems> GetDeptMedServiceItems_DeptIDPaging(
            DeptMedServiceItemsSearchCriteria Criteria,
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
                SqlCommand cmd = new SqlCommand("spGetDeptMedServiceItems_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedServiceItemPriceListID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<DeptMedServiceItems> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    //lst = GetMedServiceItemPriceCollectionFromReader(reader); 
                    lst = GetDeptMedServiceItemsCollectionFromReader(reader);
                }
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

        public override List<MedServiceItemPrice> GetMedServiceItemPrice_Paging(
           MedServiceItemPriceSearchCriteria Criteria,
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
                SqlCommand cmd = new SqlCommand("spGetRefMedServiceItems_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedServiceItemPriceListID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<MedServiceItemPrice> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedServiceItemPriceCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                return objList;
            }
        }
        public override bool DeptMedServiceItems_TrueDelete(
            Int64 DeptMedServItemID,
            Int64 MedServItemPriceID,
            Int64 MedServiceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spDeptMedServiceItems_TrueDelete", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@DeptMedServItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptMedServItemID));
                cmd1.AddParameter("@MedServItemPriceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServItemPriceID));
                cmd1.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));

                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }



        public override bool DeptMedServiceItems_InsertXML(IList<DeptMedServiceItems> lstDeptMedServiceItems)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeptMedServiceItems_InsertXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, DeptMedServiceItems_ConvertListToXml(lstDeptMedServiceItems));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }
        public override bool DeptMedServiceItems_DeleteXML(IList<DeptMedServiceItems> lstDeptMedServiceItems)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeptMedServiceItems_DeleteXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, DeptMedServiceItems_ConvertListToXml(lstDeptMedServiceItems));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }
        private string DeptMedServiceItems_ConvertListToXml(IList<DeptMedServiceItems> allUserGroup)
        {
            if (allUserGroup != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (DeptMedServiceItems item in allUserGroup)
                {
                    sb.Append("<DeptMedServiceItems>");
                    sb.AppendFormat("<DeptMedServItemID>{0}</DeptMedServItemID>", item.DeptMedServItemID);
                    sb.AppendFormat("<DeptID>{0}</DeptID>", item.DeptID);
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", item.MedServiceID);
                    sb.Append("</DeptMedServiceItems>");
                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        //public override List<RefDepartments> DeptMedServiceItems_GetDeptHasMedserviceIsPCL()
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spDeptMedServiceItems_GetDeptHasMedserviceIsPCL", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cn.Open();
        //        List<RefDepartments> objList = null;

        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader != null)
        //        {
        //            objList = GetRefDepartmentsCollectionFromReader(reader);
        //        }
        //        reader.Close();
        //        return objList;
        //    }
        //}


        #endregion

        #region "RefMedicalServiceTypes"
        public override List<RefMedicalServiceType> GroupRefMedicalServiceTypes_ByMedicalServiceTypeID(Int64 DeptID, string MedServiceName)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGroupRefMedicalServiceTypes_ByMedicalServiceTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, MedServiceName);

                cn.Open();
                List<RefMedicalServiceType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<RefMedicalServiceType> GetAllMedicalServiceTypes()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllMedicalServiceTypes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<RefMedicalServiceType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<RefMedicalServiceType> RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cmd.AddParameter("@V", SqlDbType.Int, V);/*0: all, 1: not PCL(Subtract PCL)*/

                cn.Open();
                List<RefMedicalServiceType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<RefMedicalServiceType> GetAllMedicalServiceTypes_SubtractPCL()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllMedicalServiceTypes_SubtractPCL", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                //cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, MedServiceName);

                cn.Open();
                List<RefMedicalServiceType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<RefMedicalServiceType> RefMedicalServiceTypes_Paging(
           RefMedicalServiceTypeSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceTypes_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceGroupID));
                cmd.AddParameter("@V_RefMedicalServiceTypes", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.V_RefMedicalServiceTypes));
                cmd.AddParameter("@MedicalServiceTypeCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeCode));
                cmd.AddParameter("@MedicalServiceTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefMedicalServiceType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetRefMedicalServiceTypeCollectionFromReader(reader);
                }
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


        public override void RefMedicalServiceTypes_CheckBeforeInsertUpdate(
        RefMedicalServiceType Obj,
            out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRefMedicalServiceTypes_CheckBeforeInsertUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeID));
                cmd.AddParameter("@MedicalServiceTypeCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeCode));
                cmd.AddParameter("@MedicalServiceTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeName));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }


        public override void RefMedicalServiceTypes_AddEdit(RefMedicalServiceType Obj, out string Result)
        {
            Result = "";

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceTypes_AddEdit", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeID));
                cmd.AddParameter("@MedicalServiceGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedicalServiceGroupID));
                cmd.AddParameter("@MedicalServiceTypeCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeCode));
                cmd.AddParameter("@MedicalServiceTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeName));
                cmd.AddParameter("@MedicalServiceTypeDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeDescription));
                cmd.AddParameter("@V_RefMedicalServiceInOutOthers", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RefMedicalServiceInOutOthers));
                cmd.AddParameter("@V_RefMedicalServiceTypes", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RefMedicalServiceTypes));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override bool RefMedicalServiceTypes_MarkDelete(Int64 MedicalServiceTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceTypes_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedicalServiceTypeID));

                cn.Open();

                if (ExecuteNonQuery(cmd) > 0)
                    return true;
                return false;
            }
        }

        public override IList<RefMedicalServiceType> RefMedicalServiceTypes_ByV_RefMedicalServiceTypes(Int64 V_RefMedicalServiceTypes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceTypes_ByV_RefMedicalServiceTypes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);

                cmd.AddParameter("@V_RefMedicalServiceTypes", SqlDbType.BigInt, V_RefMedicalServiceTypes);

                cn.Open();
                List<RefMedicalServiceType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }


        #endregion

        #region "MedServiceItemPrice"
        public override List<MedServiceItemPrice> MedServiceItemPriceByDeptMedServItemID_Paging(
            MedServiceItemPriceSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spMedServiceItemPriceByDeptMedServItemID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptMedServItemID", SqlDbType.BigInt, Criteria.DeptMedServItemID);
                cmd.AddParameter("@V_TypePrice", SqlDbType.BigInt, Criteria.V_TypePrice);

                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.ToDate));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<MedServiceItemPrice> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedServiceItemPriceCollectionFromReader(reader);
                }
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


        public override void MedServiceItemPrice_CheckCanEditCanDelete(
            Int64 MedServItemPriceID,
            out bool CanEdit,
            out bool CanDelete,
            out string PriceType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                CanEdit = false;
                CanDelete = false;
                PriceType = "";

                SqlCommand cmd = new SqlCommand("spMedServiceItemPrice_CheckCanEditCanDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServItemPriceID", SqlDbType.BigInt, MedServItemPriceID);
                cmd.AddParameter("@CanEdit", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@CanDelete", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PriceType", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@CanEdit"].Value != null)
                    CanEdit = Convert.ToBoolean(cmd.Parameters["@CanEdit"].Value);
                if (cmd.Parameters["@CanDelete"].Value != null)
                    CanDelete = Convert.ToBoolean(cmd.Parameters["@CanDelete"].Value);
                if (cmd.Parameters["@PriceType"].Value != null)
                    PriceType = cmd.Parameters["@PriceType"].Value.ToString();

            }
        }


        public override void MedServiceItemPrice_Save(MedServiceItemPrice Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spMedServiceItemPrice_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServItemPriceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedServItemPriceID));
                cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedServiceID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));
                cmd.AddParameter("@ApprovedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ApprovedStaffID));
                cmd.AddParameter("@VATRate", SqlDbType.Float, ConvertNullObjectToDBNull(Obj.VATRate));
                cmd.AddParameter("@NormalPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(Obj.NormalPrice));
                cmd.AddParameter("@PriceForHIPatient", SqlDbType.Decimal, ConvertNullObjectToDBNull(Obj.PriceForHIPatient));
                cmd.AddParameter("@PriceDifference", SqlDbType.Decimal, ConvertNullObjectToDBNull(Obj.PriceDifference));
                cmd.AddParameter("@HIAllowedPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(Obj.HIAllowedPrice));
                cmd.AddParameter("@EffectiveDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.EffectiveDate));
                cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Note));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void MedServiceItemPrice_MarkDelete(Int64 MedServItemPriceID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spMedServiceItemPrice_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServItemPriceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServItemPriceID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        //Đọc by ID
        public override MedServiceItemPrice MedServiceItemPrice_ByMedServItemPriceID(Int64 MedServItemPriceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedServiceItemPrice_ByMedServItemPriceID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramDeptID = new SqlParameter("@MedServItemPriceID", SqlDbType.BigInt);
                paramDeptID.Value = MedServItemPriceID;

                cmd.Parameters.Add(paramDeptID);

                cn.Open();

                MedServiceItemPrice ObjMedServiceItemPrice = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    ObjMedServiceItemPrice = GetMedServiceItemPriceFromReader(reader);
                }

                reader.Close();
                return ObjMedServiceItemPrice;
            }
        }
        //Đọc by ID


        #endregion

        #region PCLItem
        //public override void PCLItemsInsertUpdate(PCLItem Obj, bool SaveToDB, out string Result)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        Result = "";

        //        SqlCommand cmd = new SqlCommand("spPCLItemsInsertUpdate", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@PCLItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLItemID));
        //        cmd.AddParameter("@PCLSectionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLSectionID));
        //        cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeID));
        //        cmd.AddParameter("@Idx", SqlDbType.TinyInt, ConvertNullObjectToDBNull(Obj.Idx));
        //        cmd.AddParameter("@SaveToDB", SqlDbType.Bit, SaveToDB);                

        //        cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

        //        cn.Open();

        //        ExecuteNonQuery(cmd);

        //        if (cmd.Parameters["@Result"].Value != null)
        //            Result = cmd.Parameters["@Result"].Value.ToString();
        //    }
        //}

        public override bool PCLItems_XMLInsert(Int64 PCLFormID, IEnumerable<PCLExamType> ObjPCLExamTypeList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spPCLItems_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.AddParameter("@PCLFormID", SqlDbType.BigInt, PCLFormID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertListPCLItemsToXml(PCLFormID, ObjPCLExamTypeList));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
                return false;
            }

        }


        public string ConvertListPCLItemsToXml(Int64 PCLFormID, IEnumerable<PCLExamType> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PCLExamType details in items)
                {
                    sb.Append("<PCLItems>");
                    sb.AppendFormat("<PCLFormID>{0}</PCLFormID>", PCLFormID);
                    sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", details.PCLExamTypeID);
                    sb.AppendFormat("<Idx>{0}</Idx>", 0);
                    sb.AppendFormat("<Qty>{0}</Qty>", 0);
                    sb.Append("</PCLItems>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }


        //public override List<PCLItem> PCLItems_GetPCLExamTypeIDByMedServiceID(Int64 MedServiceID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLItems_GetPCLExamTypeIDByMedServiceID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, MedServiceID);                

        //        cn.Open();
        //        List<PCLItem> objList = null;

        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader != null)
        //        {
        //            objList = GetPCLItemCollectionFromReader(reader);
        //        }
        //        reader.Close();
        //        return objList;
        //    }
        //}

        public override List<PCLExamType> PCLItems_GetPCLExamTypeIDByPCLSectionID(string PCLExamTypeName, Int64 PCLSectionID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLItems_GetPCLExamTypeIDByPCLSectionID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTypeName));
                cmd.AddParameter("@PCLSectionID", SqlDbType.BigInt, PCLSectionID);

                cn.Open();

                List<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<PCLExamType> PCLExamType_WithDeptLocIDs_GetAll()
        {
            var listPCLExamTypes = BuildPCLExamTypeDeptLocMapBase(false);
            if (listPCLExamTypes != null)
                return (List<PCLExamType>)listPCLExamTypes;
            return null;
        }

        public override List<PCLExamType> PCLItems_ByPCLFormID(PCLExamTypeSearchCriteria SearchCriteria, Int64 PCLFormID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLItems_ByPCLFormID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));

                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeName));
                cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLFormID));

                cn.Open();

                List<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }


        public override List<PCLExamType> PCLItems_SearchAutoComplete(
           PCLExamTypeSearchCriteria SearchCriteria,

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
                SqlCommand cmd = new SqlCommand("spPCLItems_SearchAutoComplete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeName));
                cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLFormID));
                cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(SearchCriteria.IsExternalExam));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLExamType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeColectionFromReader(reader);
                }
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


        public override List<PCLExamType> GetPCLExamType_byComboID(long PCLExamTypeComboID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamType_byComboID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeComboID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeComboID));
                cn.Open();
                List<PCLExamType> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                lst = GetPCLExamTypeColectionFromReader(reader);
                reader.Close();

                return lst;
            }
        }

        public override List<PCLExamType> GetPCLExamType_byHosID(long HosID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamType_byHosID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@HosID", SqlDbType.BigInt, ConvertNullObjectToDBNull(HosID));
                cn.Open();
                List<PCLExamType> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                lst = GetPCLExamTypeColectionFromReader(reader);
                reader.Close();

                return lst;
            }
        }
        #endregion

        #region PCLForms
        //
        public override List<PCLForm> PCLForms_GetList_Paging(
 PCLFormsSearchCriteria SearchCriteria,

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
                SqlCommand cmd = new SqlCommand("spPCLForms_GetList_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLFormName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLFormName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLForm> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLFormsColectionsFromReader(reader);
                }
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

        public override void PCLForms_Save(PCLForm Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLForms_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLFormID));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_PCLMainCategory));
                cmd.AddParameter("@PCLFormName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLFormName));
                cmd.AddParameter("@ApplicatorDay", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ApplicatorDay));
                cmd.AddParameter("@ExpiredDay", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ExpiredDay));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void PCLForms_MarkDelete(Int64 PCLFormID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLForms_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLFormID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }
        //

        #endregion

        #region PCLSections

        public override List<PCLSection> PCLSections_All()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLSections_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;


                cn.Open();
                List<PCLSection> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLSectionCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        //public override List<PCLSection> PCLSectionsByPCLFormID(Int64 PCLFormID)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spPCLSectionsByPCLFormID", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, PCLFormID);

        //        cn.Open();
        //        List<PCLSection> objList = null;

        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader != null)
        //        {
        //            objList = GetPCLSectionCollectionFromReader(reader);
        //        }
        //        reader.Close();
        //        return objList;
        //    }
        //}


        public override List<PCLSection> PCLSections_GetList_Paging(
 PCLSectionsSearchCriteria SearchCriteria,

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
                SqlCommand cmd = new SqlCommand("spPCLSections_GetList_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLSectionName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLSectionName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLSection> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLSectionCollectionFromReader(reader);
                }
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

        public override void PCLSections_Save(PCLSection Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLSections_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLSectionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLSectionID));
                cmd.AddParameter("@PCLSectionName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLSectionName));
                //cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DeptID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void PCLSections_MarkDelete(Int64 PCLSectionID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLSections_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLSectionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLSectionID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }


        #endregion

        #region Locations
        public override void Locations_InsertUpdate(
        Location Obj, bool SaveToDB, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spLocations_InsertUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.LID));
                cmd.AddParameter("@V_LocationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_LocationType));
                cmd.AddParameter("@RmTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.RmTypeID));
                cmd.AddParameter("@LocationName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.LocationName));
                cmd.AddParameter("@LocationDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.LocationDescription));
                cmd.AddParameter("@SaveToDB", SqlDbType.Bit, SaveToDB);

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        //Save list 
        public override bool Locations_XMLInsert(Location objCollect)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spLocations_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@DataXML", SqlDbType.Xml, objCollect.ConvertObjLocation_ListToXml(objCollect.ObjLocation_List));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
                return false;
            }

        }
        //Save list 

        //list paging
        public override List<Location> Locations_ByRmTypeID_Paging(
        LocationSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spLocations_ByRmTypeID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RmTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.RmTypeID));
                cmd.AddParameter("@LocationName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.LocationName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<Location> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetLocationCollectionFromReader(reader);
                }
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
        //list paging


        public override void Locations_MarkDeleted(
           Int64 LID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spLocations_MarkDeleted", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LID", SqlDbType.BigInt, ConvertNullObjectToDBNull(LID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }


        #endregion

        #region RoomType
        public override List<RoomType> RoomType_GetAll()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRoomType_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@HITTypeID", SqlDbType.Int, HITTypeID);

                cn.Open();
                List<RoomType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRoomTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<RoomType> RoomType_GetList_Paging(
   RoomTypeSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spRoomType_GetList_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_RoomFunction", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.V_RoomFunction));
                cmd.AddParameter("@RmTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.RmTypeName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RoomType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetRoomTypeCollectionFromReader(reader);
                }
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

        public override void RoomType_Save(
        RoomType Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRoomType_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RmTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.RmTypeID));
                cmd.AddParameter("@RmTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.RmTypeName));
                cmd.AddParameter("@RmTypeDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.RmTypeDescription));
                cmd.AddParameter("@V_RoomFunction", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RoomFunction));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void RoomType_MarkDelete(
         Int64 RmTypeID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRoomType_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RmTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RmTypeID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        #endregion

        #region DeptLocation
        public override List<Location> DeptLocation_ByDeptID(Int64 DeptID, Int64 RmTypeID, string LocationName)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_ByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cmd.AddParameter("@RmTypeID", SqlDbType.BigInt, RmTypeID);
                cmd.AddParameter("@LocationName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(LocationName));

                cn.Open();
                List<Location> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetLocationCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override List<RoomType> DeptLocation_GetRoomTypeByDeptID(Int64 DeptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetRoomTypeByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);

                cn.Open();
                List<RoomType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetRoomTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override bool DeptLocation_CheckLIDExists(Int64 DeptID, Int64 LID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spDeptLocation_CheckLIDExists", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cmd1.AddParameter("@LID", SqlDbType.BigInt, LID);

                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)//Tồn Tại rồi có rồi
                    return true;
                return false;
            }
        }

        public override bool DeptLocation_XMLInsert(Int64 DeptID, IEnumerable<Location> objCollect)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spDeptLocation_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertObjDeptLocation_ListToXml(DeptID, objCollect));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }
        public string ConvertObjDeptLocation_ListToXml(Int64 DeptID, IEnumerable<Location> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (Location items in ObjList)
                {
                    sb.Append("<DeptLocation>");
                    sb.AppendFormat("<LID>{0}</LID>", items.LID);
                    sb.AppendFormat("<DeptID>{0}</DeptID>", DeptID);
                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", 0);
                    sb.Append("</DeptLocation>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }


        public override void DeptLocation_MarkDeleted(Int64 DeptLocationID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spDeptLocation_MarkDeleted", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }


        public override List<DeptLocation> DeptLocation_ByMedicalServiceTypeIDDeptID(Int64 MedicalServiceTypeID, Int64 DeptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_ByMedicalServiceTypeIDDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, MedicalServiceTypeID);
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);


                cn.Open();
                List<DeptLocation> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetDeptLocationCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }


        public override List<DeptLocation> GetAllLocationsByDeptID(long? DeptID, long? V_RoomFunction = null)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllLocationsByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cmd.AddParameter("@V_RoomFunction", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RoomFunction));

                cn.Open();
                List<DeptLocation> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetDeptLocationCollectionFromReader(reader);
                }
                return objList;
            }
        }


        public override List<DeptLocation> ListDeptLocation_ByPCLExamTypeID(long PCLExamTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spListDeptLocation_ByPCLExamTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);

                cn.Open();
                List<DeptLocation> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetDeptLocationCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }


        #endregion

        #region DeptLocMedServices
        //Cho chọn
        //ds not paging
        //public override List<RefMedicalServiceItem> DeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose(
        //     Int64 DeptID,               
        //     Int64 MedicalServiceTypeID,  
        //     string MedServiceName)        
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        SqlCommand cmd = new SqlCommand("spDeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@DeptID", SqlDbType.BigInt,ConvertNullObjectToDBNull(DeptID));                                
        //        cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt,ConvertNullObjectToDBNull(MedicalServiceTypeID));                
        //        cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar,ConvertNullObjectToDBNull(MedServiceName));                


        //        cn.Open();
        //        List<RefMedicalServiceItem> objList = null;

        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader != null)
        //        {
        //            objList = GetMedicalServiceItemCollectionFromReader(reader);
        //        }
        //        reader.Close();
        //        return objList;
        //    }
        //}
        //ds not paging

        //Group loại dv từ ds dv not paging
        public override List<RefMedicalServiceType> DeptMedServiceItems_GroupMedSerTypeID_AllForChoose(
            Int64 DeptID,
            Int64 MedicalServiceTypeID,
            string MedServiceName)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptMedServiceItems_GroupMedSerTypeID_AllForChoose", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MedServiceName));


                cn.Open();
                List<RefMedicalServiceType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        //Cho chọn

        //đã lưu ở DB        
        public override List<RefMedicalServiceItem> DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(
            Int64 DeptID,
            Int64 DeptLocationID,
            Int64 MedicalServiceTypeID,
            string MedServiceName)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MedServiceName));


                cn.Open();
                List<RefMedicalServiceItem> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceItemCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        //Group loại dv từ ds dv not paging
        public override List<RefMedicalServiceType> DeptLocMedServices_GroupMedSerTypeID_HasChoose(
            Int64 DeptID,
            Int64 DeptLocationID,
            Int64 MedicalServiceTypeID,
            string MedServiceName)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocMedServices_GroupMedSerTypeID_HasChoose", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MedServiceName));


                cn.Open();
                List<RefMedicalServiceType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetMedicalServiceTypeCollectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }
        //đã lưu ở DB

        //Save list 
        public override bool DeptLocMedServices_XMLInsert(Int64 DeptLocationID, IEnumerable<RefMedicalServiceItem> objCollect)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spDeptLocMedServices_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@DeptLocationID", SqlDbType.BigInt, DeptLocationID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertListRefMedicalServiceItemToXml(DeptLocationID, objCollect));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
                return false;
            }

        }
        public string ConvertListRefMedicalServiceItemToXml(Int64 DeptLocationID, IEnumerable<RefMedicalServiceItem> objCollect)
        {
            if (objCollect != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (RefMedicalServiceItem item in objCollect)
                {
                    sb.Append("<DeptLocMedServices>");
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", item.MedServiceID);
                    sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", DeptLocationID);
                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", 0);
                    sb.Append("</DeptLocMedServices>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        //Save list 

        #endregion

        #region PCLExamTypePrices
        public override List<PCLExamTypePrice> PCLExamTypePrices_ByPCLExamTypeID_Paging(
        PCLExamTypePriceSearchCriteria Criteria,

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
                SqlCommand cmd = new SqlCommand("spPCLExamTypePrices_ByPCLExamTypeID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.PCLExamTypeID));
                cmd.AddParameter("@PriceType", SqlDbType.Int, ConvertNullObjectToDBNull(Criteria.PriceType));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.ToDate));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLExamTypePrice> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypePriceColectionsFromReader(reader);
                }
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

        public override void PCLExamTypePrices_CheckCanEditCanDelete(
        Int64 PCLExamTypePriceID,
        out bool CanEdit,
        out bool CanDelete,
        out string PriceType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                CanEdit = false;
                CanDelete = false;
                PriceType = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypePrices_CheckCanEditCanDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypePriceID", SqlDbType.BigInt, PCLExamTypePriceID);
                cmd.AddParameter("@CanEdit", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@CanDelete", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PriceType", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@CanEdit"].Value != null)
                    CanEdit = Convert.ToBoolean(cmd.Parameters["@CanEdit"].Value);
                if (cmd.Parameters["@CanDelete"].Value != null)
                    CanDelete = Convert.ToBoolean(cmd.Parameters["@CanDelete"].Value);
                if (cmd.Parameters["@PriceType"].Value != null)
                    PriceType = cmd.Parameters["@PriceType"].Value.ToString();
            }
        }

        public override void PCLExamTypePrices_Save(PCLExamTypePrice Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypePrices_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypePriceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypePriceID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));
                cmd.AddParameter("@ApprovedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ApprovedStaffID));
                cmd.AddParameter("@NormalPrice", SqlDbType.Money, ConvertNullObjectToDBNull(Obj.NormalPrice));
                cmd.AddParameter("@PriceForHIPatient", SqlDbType.Money, ConvertNullObjectToDBNull(Obj.PriceForHIPatient));
                cmd.AddParameter("@HIAllowedPrice", SqlDbType.Money, ConvertNullObjectToDBNull(Obj.HIAllowedPrice));
                cmd.AddParameter("@EffectiveDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.EffectiveDate));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void PCLExamTypePrices_MarkDelete(Int64 PCLExamTypePriceID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypePrices_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypePriceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypePriceID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override PCLExamTypePrice PCLExamTypePrices_ByPCLExamTypePriceID(Int64 PCLExamTypePriceID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypePrices_ByPCLExamTypePriceID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramDeptID = new SqlParameter("@PCLExamTypePriceID", SqlDbType.BigInt);
                paramDeptID.Value = PCLExamTypePriceID;

                cmd.Parameters.Add(paramDeptID);

                cn.Open();

                PCLExamTypePrice ObjPCLExamTypePrice = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    ObjPCLExamTypePrice = GetPCLExamTypePriceFromReader(reader);
                }

                reader.Close();
                return ObjPCLExamTypePrice;
            }
        }

        #endregion

        #region PCLGroups
        public override IList<PCLGroup> PCLGroups_GetAll(long? V_PCLCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLGroups_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLCategory));
                cn.Open();
                List<PCLGroup> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLGroupColectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }


        public override List<PCLGroup> PCLGroups_GetList_Paging(
PCLGroupsSearchCriteria SearchCriteria,

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
                SqlCommand cmd = new SqlCommand("spPCLGroups_GetList_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLCategory));
                cmd.AddParameter("@PCLGroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLGroupName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLGroup> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLGroupColectionFromReader(reader);
                }
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

        public override void PCLGroups_Save(PCLGroup Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLGroups_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLGroupID));
                cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_PCLCategory));
                cmd.AddParameter("@PCLGroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLGroupName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Description));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void PCLGroups_MarkDelete(Int64 PCLGroupID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLGroups_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLGroupID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        #endregion

        #region MedServiceItemPriceList
        //Save
        public override void MedServiceItemPriceList_AddNew(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjMedServiceItemPrice, out string Result_PriceList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result_PriceList = "";

                SqlCommand cmd1 = new SqlCommand("spMedServiceItemPriceList_AddNew", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, Obj.MedicalServiceTypeID);
                cmd1.AddParameter("@PriceListTitle", SqlDbType.NVarChar, Obj.PriceListTitle);
                cmd1.AddParameter("@StaffID_PriceList", SqlDbType.BigInt, Obj.StaffID);
                cmd1.AddParameter("@ApprovedStaffID_PriceList", SqlDbType.BigInt, Obj.ApprovedStaffID);
                cmd1.AddParameter("@EffectiveDate_PriceList", SqlDbType.DateTime, Obj.EffectiveDate);

                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertListMedServiceItemPriceToXml(ObjMedServiceItemPrice));

                cmd1.AddParameter("@Result_PriceList", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd1);

                if (cmd1.Parameters["@Result_PriceList"].Value != null)
                    Result_PriceList = cmd1.Parameters["@Result_PriceList"].Value.ToString();

            }

        }
        public string ConvertListMedServiceItemPriceToXml(IEnumerable<MedServiceItemPrice> ObjCollect)
        {
            if (ObjCollect != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (MedServiceItemPrice item in ObjCollect)
                {
                    sb.Append("<MedServiceItemPrice>");
                    sb.AppendFormat("<MedServItemPriceID>{0}</MedServItemPriceID>", item.MedServItemPriceID);
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", item.MedServiceID);
                    sb.AppendFormat("<VATRate>{0}</VATRate>", item.VATRate);
                    sb.AppendFormat("<NormalPrice>{0}</NormalPrice>", item.NormalPrice);
                    sb.AppendFormat("<PriceForHIPatient>{0}</PriceForHIPatient>", item.PriceForHIPatient);
                    sb.AppendFormat("<PriceDifference>{0}</PriceDifference>", item.PriceDifference);
                    sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", item.HIAllowedPrice);
                    sb.AppendFormat("<Note>{0}</Note>", item.Note);
                    sb.Append("</MedServiceItemPrice>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        //Save

        //Paging
        public override List<MedServiceItemPriceList> MedServiceItemPriceList_GetList_Paging(MedServiceItemPriceListSearchCriteria SearchCriteria,
                                                                                                int PageIndex,
                                                                                                int PageSize,
                                                                                                string OrderBy,
                                                                                                bool CountTotal,
                                                                                                out int Total )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedServiceItemPriceList_GetList_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.MedicalServiceTypeID));
                cmd.AddParameter("@PriceListTitle", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PriceListTitle));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.Year));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<MedServiceItemPriceList> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedServiceItemPriceListColectionsFromReader(reader);
                }
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


        public override void MedServiceItemPriceList_CheckCanEditCanDelete(
            Int64 MedServiceItemPriceListID,
            out bool CanEdit,
            out bool CanDelete,
            out string PriceListType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                CanEdit = false;
                CanDelete = false;
                PriceListType = "";

                SqlCommand cmd = new SqlCommand("spMedServiceItemPriceList_CheckCanEditCanDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, MedServiceItemPriceListID);
                cmd.AddParameter("@CanEdit", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@CanDelete", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PriceListType", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@CanEdit"].Value != null)
                    CanEdit = Convert.ToBoolean(cmd.Parameters["@CanEdit"].Value);
                if (cmd.Parameters["@CanDelete"].Value != null)
                    CanDelete = Convert.ToBoolean(cmd.Parameters["@CanDelete"].Value);
                if (cmd.Parameters["@PriceListType"].Value != null)
                    PriceListType = cmd.Parameters["@PriceListType"].Value.ToString();

            }
        }

        //MarkDelete
        public override void MedServiceItemPriceList_MarkDelete(
        Int64 MedServiceItemPriceListID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spMedServiceItemPriceList_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceItemPriceListID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }


        //MedServiceItemPriceList_Detail
        public override List<MedServiceItemPrice> MedServiceItemPriceList_Detail(
         DeptMedServiceItemsSearchCriteria Criteria,
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
                SqlCommand cmd = new SqlCommand("spMedServiceItemPriceList_Detail", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedServiceItemPriceListID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<MedServiceItemPrice> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedServiceItemPriceCollectionFromReader(reader);
                }
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

        //Update
        public override void MedServiceItemPriceList_Update
            (MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjCollection, IEnumerable<MedServiceItemPrice> ObjCollection_Insert, IEnumerable<MedServiceItemPrice> ObjCollection_Update, out string Result_PriceList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result_PriceList = "";

                SqlCommand cmd1 = new SqlCommand("spMedServiceItemPriceList_Update", cn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, Obj.MedServiceItemPriceListID);
                cmd1.AddParameter("@DeptID", SqlDbType.BigInt, Obj.DeptID);
                cmd1.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, Obj.MedicalServiceTypeID);
                cmd1.AddParameter("@PriceListTitle", SqlDbType.NVarChar, Obj.PriceListTitle);
                cmd1.AddParameter("@StaffID_PriceList", SqlDbType.BigInt, Obj.StaffID);
                cmd1.AddParameter("@ApprovedStaffID_PriceList", SqlDbType.BigInt, Obj.ApprovedStaffID);
                cmd1.AddParameter("@EffectiveDate_PriceList", SqlDbType.DateTime, Obj.EffectiveDate);

                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertListMedServiceItemPriceToXml_Update(ObjCollection));
                cmd1.AddParameter("@DataXML_Insert", SqlDbType.Xml, ConvertListMedServiceItemPriceToXml_Update(ObjCollection_Insert));
                cmd1.AddParameter("@DataXML_Update", SqlDbType.Xml, ConvertListMedServiceItemPriceToXml_Update(ObjCollection_Update));

                cmd1.AddParameter("@Result_PriceList", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd1);

                if (cmd1.Parameters["@Result_PriceList"].Value != null)
                    Result_PriceList = cmd1.Parameters["@Result_PriceList"].Value.ToString();

            }

        }
        public string ConvertListMedServiceItemPriceToXml_Update(IEnumerable<MedServiceItemPrice> ObjCollect)
        {
            if (ObjCollect != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (MedServiceItemPrice item in ObjCollect)
                {
                    sb.Append("<MedServiceItemPrice>");
                    sb.AppendFormat("<MedServItemPriceID>{0}</MedServItemPriceID>", item.MedServItemPriceID);
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", item.MedServiceID);
                    sb.AppendFormat("<MedServiceItemPriceListID>{0}</MedServiceItemPriceListID>", item.MedServiceItemPriceListID);
                    sb.AppendFormat("<VATRate>{0}</VATRate>", item.VATRate);
                    sb.AppendFormat("<NormalPrice>{0}</NormalPrice>", item.NormalPrice);
                    sb.AppendFormat("<PriceForHIPatient>{0}</PriceForHIPatient>", item.PriceForHIPatient);
                    sb.AppendFormat("<PriceDifference>{0}</PriceDifference>", item.PriceDifference);
                    sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", item.HIAllowedPrice);
                    sb.AppendFormat("<Note>{0}</Note>", item.Note);
                    sb.Append("</MedServiceItemPrice>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }


        //Check CanAddNew
        public override void MedServiceItemPriceList_CheckCanAddNew(Int64 DeptID, Int64 MedicalServiceTypeID, out bool CanAddNew)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                CanAddNew = false;

                SqlCommand cmd = new SqlCommand("spMedServiceItemPriceList_CheckCanAddNew", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, MedicalServiceTypeID);
                cmd.AddParameter("@CanAddNew", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@CanAddNew"].Value != null)
                    CanAddNew = Convert.ToBoolean(cmd.Parameters["@CanAddNew"].Value);
            }
        }


        #endregion

        //
        #region PCLExamTypePriceList
        //Check CanAddNew
        public override void PCLExamTypePriceList_CheckCanAddNew(out bool CanAddNew)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                CanAddNew = false;

                SqlCommand cmd = new SqlCommand("spPCLExamTypePriceList_CheckCanAddNew", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CanAddNew", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@CanAddNew"].Value != null)
                    CanAddNew = Convert.ToBoolean(cmd.Parameters["@CanAddNew"].Value);
            }
        }

        //Save
        public override void PCLExamTypePriceList_AddNew(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjPCLExamType, out string Result_PriceList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result_PriceList = "";

                SqlCommand cmd1 = new SqlCommand("spPCLExamTypePriceList_AddNew", cn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.AddParameter("@PCLExamTypePriceListID", SqlDbType.BigInt, Obj.PCLExamTypePriceListID);
                cmd1.AddParameter("@PriceListTitle", SqlDbType.NVarChar, Obj.PriceListTitle);
                cmd1.AddParameter("@StaffID_PriceList", SqlDbType.BigInt, Obj.StaffID);
                cmd1.AddParameter("@ApprovedStaffID_PriceList", SqlDbType.BigInt, Obj.ApprovedStaffID);
                cmd1.AddParameter("@EffectiveDate_PriceList", SqlDbType.DateTime, Obj.EffectiveDate);

                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertListPCLExamTypePriceToXml(ObjPCLExamType));

                cmd1.AddParameter("@Result_PriceList", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd1);

                if (cmd1.Parameters["@Result_PriceList"].Value != null)
                    Result_PriceList = cmd1.Parameters["@Result_PriceList"].Value.ToString();

            }

        }

        public string ConvertListPCLExamTypePriceToXml(IEnumerable<PCLExamType> ObjCollect)
        {
            if (ObjCollect != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PCLExamType item in ObjCollect)
                {
                    sb.Append("<PCLExamTypePrices>");
                    sb.AppendFormat("<PCLExamTypePriceID>{0}</PCLExamTypePriceID>", item.ObjPCLExamTypePrice.PCLExamTypePriceID);
                    sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", item.ObjPCLExamTypePrice.PCLExamTypeID);
                    sb.AppendFormat("<NormalPrice>{0}</NormalPrice>", item.ObjPCLExamTypePrice.NormalPrice);
                    sb.AppendFormat("<PriceForHIPatient>{0}</PriceForHIPatient>", item.ObjPCLExamTypePrice.PriceForHIPatient);
                    sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", item.ObjPCLExamTypePrice.HIAllowedPrice);
                    sb.Append("</PCLExamTypePrices>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        //Save

        //PriceList Paging

        public override List<PCLExamTypePriceList> PCLExamTypePriceList_GetList_Paging(
        PCLExamTypePriceListSearchCriteria SearchCriteria,

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
                SqlCommand cmd = new SqlCommand("spPCLExamTypePriceList_GetList_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PriceListTitle", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PriceListTitle));
                cmd.AddParameter("@Month", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.Month));
                cmd.AddParameter("@Year", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.Year));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLExamTypePriceList> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypePriceListColectionsFromReader(reader);
                }
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


        public override void PCLExamTypePriceList_CheckCanEditCanDelete(
            Int64 PCLExamTypePriceListID,
            out bool CanEdit,
            out bool CanDelete,
            out string PriceListType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                CanEdit = false;
                CanDelete = false;
                PriceListType = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypePriceList_CheckCanEditCanDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypePriceListID", SqlDbType.BigInt, PCLExamTypePriceListID);
                cmd.AddParameter("@CanEdit", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@CanDelete", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PriceListType", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@CanEdit"].Value != null)
                    CanEdit = Convert.ToBoolean(cmd.Parameters["@CanEdit"].Value);
                if (cmd.Parameters["@CanDelete"].Value != null)
                    CanDelete = Convert.ToBoolean(cmd.Parameters["@CanDelete"].Value);
                if (cmd.Parameters["@PriceListType"].Value != null)
                    PriceListType = cmd.Parameters["@PriceListType"].Value.ToString();

            }
        }

        //MarkDelete
        public override void PCLExamTypePriceList_Delete(
        Int64 PCLExamTypePriceListID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypePriceList_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypePriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypePriceListID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }


        //PCLExamTypePriceList Detail
        public override List<PCLExamType> PCLExamTypePriceList_Detail(
         long PCLExamTypePriceListID,

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
                SqlCommand cmd = new SqlCommand("spPCLExamTypePriceList_Detail", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //////cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.V_PCLCategory));
                //cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.PCLGroupID));            
                //cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.PCLExamTypeName));
                cmd.AddParameter("@PCLExamTypePriceListID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTypePriceListID));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLExamType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeColectionFromReader(reader);
                }
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

        //Update
        public override void PCLExamTypePriceList_Update(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjCollection_Update, out string Result_PriceList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result_PriceList = "";

                SqlCommand cmd1 = new SqlCommand("spPCLExamTypePriceList_Update", cn);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.AddParameter("@PCLExamTypePriceListID", SqlDbType.BigInt, Obj.PCLExamTypePriceListID);
                cmd1.AddParameter("@PriceListTitle", SqlDbType.NVarChar, Obj.PriceListTitle);
                cmd1.AddParameter("@StaffID_PriceList", SqlDbType.BigInt, Obj.StaffID);
                cmd1.AddParameter("@ApprovedStaffID_PriceList", SqlDbType.BigInt, Obj.ApprovedStaffID);
                cmd1.AddParameter("@EffectiveDate_PriceList", SqlDbType.DateTime, Obj.EffectiveDate);

                cmd1.AddParameter("@DataXML_Update", SqlDbType.Xml, ConvertPCLExamTypePriceListToXml_Update(ObjCollection_Update));

                cmd1.AddParameter("@Result_PriceList", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd1);

                if (cmd1.Parameters["@Result_PriceList"].Value != null)
                    Result_PriceList = cmd1.Parameters["@Result_PriceList"].Value.ToString();

            }

        }
        public string ConvertPCLExamTypePriceListToXml_Update(IEnumerable<PCLExamType> ObjCollection_Update)
        {
            if (ObjCollection_Update != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PCLExamType item in ObjCollection_Update)
                {
                    sb.Append("<PCLExamTypePrices>");
                    sb.AppendFormat("<PCLExamTypePriceID>{0}</PCLExamTypePriceID>", item.ObjPCLExamTypePrice.PCLExamTypePriceID);
                    sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", item.ObjPCLExamTypePrice.PCLExamTypeID);
                    sb.AppendFormat("<NormalPrice>{0}</NormalPrice>", item.ObjPCLExamTypePrice.NormalPrice);
                    sb.AppendFormat("<PriceForHIPatient>{0}</PriceForHIPatient>", item.ObjPCLExamTypePrice.PriceForHIPatient);
                    sb.AppendFormat("<HIAllowedPrice>{0}</HIAllowedPrice>", item.ObjPCLExamTypePrice.HIAllowedPrice);
                    sb.Append("</PCLExamTypePrices>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region "PCLExamTestItems"
        public override List<PCLExamTypeTestItems> PCLExamTestItems_ByPCLExamTypeID(Int64 PCLExamTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTestItems_ByPCLExamTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cn.Open();
                List<PCLExamTypeTestItems> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTypeTestItemsColectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        #endregion

        #region PCLResultParamImplementations
        /*==== #001 ====*/
        //public override List<PCLResultParamImplementations> PCLResultParamImplementations_GetAll()
        public override List<PCLResultParamImplementations> PCLResultParamImplementations_GetAll(long? PCLExamTypeSubCategoryID = null)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResultParamImplementations_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                /*==== #001 ====*/
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, PCLExamTypeSubCategoryID);
                /*==== #001 ====*/
                cn.Open();
                List<PCLResultParamImplementations> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLResultParamImplementationsColectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        /*==== #001 ====*/
        #endregion

        #region PCLExamTypeSubCategory
        public override List<PCLExamTypeSubCategory> PCLExamTypeSubCategory_ByV_PCLMainCategory(Int64 V_PCLMainCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeSubCategory_ByV_PCLMainCategory", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));

                cn.Open();
                List<PCLExamTypeSubCategory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTypeSubCategoryColectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        #endregion

        #region PCLExamTypeLocations
        public override IList<PCLExamType> PCLExamTypeLocations_ByDeptLocationID(string PCLExamTypeName, Int64 DeptLocationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeLocations_ByDeptLocationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTypeName));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cn.Open();
                IList<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }
                reader.Close();
                return objList;
            }
        }

        public override bool PCLExamTypeLocations_XMLInsert(Int64 DeptLocationID, IEnumerable<PCLExamType> ObjList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spPCLExamTypeLocations_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@DeptLocationID", SqlDbType.BigInt, DeptLocationID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertObjPCLExamTypeLocations_ListToXml(DeptLocationID, ObjList));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }

        public string ConvertObjPCLExamTypeLocations_ListToXml(Int64 DeptLocationID, IEnumerable<PCLExamType> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PCLExamTypeLocations>");
                foreach (PCLExamType items in ObjList)
                {
                    sb.Append("<RowInfo>");
                    sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", items.PCLExamTypeID);
                    sb.AppendFormat("<DeptID>{0}</DeptID>", DeptLocationID);
                    sb.Append("</RowInfo>");
                }
                sb.Append("</PCLExamTypeLocations>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public override void PCLExamTypeLocations_MarkDeleted(Int64 PCLExamTypeID, Int64 DeptLocationID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypeLocations_MarkDeleted", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }


        #endregion

        #region HITransactionType
        public override IList<HITransactionType> HITransactionType_GetListNoParentID()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHITransactionType_GetListNoParentID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLCategory));

                cn.Open();
                List<HITransactionType> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetHITransactionTypeColectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }

        //▼===== 25072018 TTM
        public override IList<HITransactionType> HITransactionType_GetListNoParentID_New()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHITransactionType_GetListNoParentID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLCategory));

                cn.Open();
                List<HITransactionType> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetHITransactionTypeColectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        //▲===== 25072018 TTM
        #endregion

        #region "RefMedicalServiceGroups"
        public override IList<RefMedicalServiceGroups> RefMedicalServiceGroups_GetAll()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceGroups_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLCategory));

                cn.Open();
                List<RefMedicalServiceGroups> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefMedicalServiceGroupsColectionFromReader(reader);
                reader.Close();
                return objLst;
            }
        }
        #endregion


        #region PCLExamTypeExamTestPrint

        public override IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrint_GetList_Paging(
            PCLExamTypeExamTestPrintSearchCriteria SearchCriteria,

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
                SqlCommand cmd = new SqlCommand("spPCLExamTypeExamTestPrint_GetList_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Code", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.Code));
                cmd.AddParameter("@Name", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.Name));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                IList<PCLExamTypeExamTestPrint> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeExamTestPrintColectionFromReader(reader);
                }
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


        public override IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrintIndex_GetAll()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeExamTestPrintIndex_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IList<PCLExamTypeExamTestPrint> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeExamTestPrintColectionFromReader(reader);
                }
                reader.Close();
                return lst;
            }
        }

        public override void PCLExamTypeExamTestPrint_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypeExamTestPrint_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DataXML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertPCLExamTypeExamTestPrintListToXml(ObjList)));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }

        public override void PCLExamTypeExamTestPrintIndex_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLExamTypeExamTestPrintIndex_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DataXML", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertPCLExamTypeExamTestPrintListToXml(ObjList)));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
            }
        }
        public string ConvertPCLExamTypeExamTestPrintListToXml(IEnumerable<PCLExamTypeExamTestPrint> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PCLExamTypeExamTestPrint details in items)
                {
                    sb.Append("<RecInfo>");
                    sb.AppendFormat("<PCLExamTypeTestItemID>{0}</PCLExamTypeTestItemID>", details.PCLExamTypeTestItemID);
                    sb.AppendFormat("<ID>{0}</ID>", details.ID);
                    sb.AppendFormat("<Code>{0}</Code>", details.Code);
                    sb.AppendFormat("<Name>{0}</Name>", details.Name);
                    sb.AppendFormat("<IsBold>{0}</IsBold>", details.IsBold);
                    sb.AppendFormat("<Indent>{0}</Indent>", details.Indent);
                    sb.AppendFormat("<PrintIdx>{0}</PrintIdx>", details.PrintIndex);
                    sb.AppendFormat("<IsDisplay>{0}</IsDisplay>", details.IsDisplay);
                    sb.AppendFormat("<IsNoNeedResult>{0}</IsNoNeedResult>", details.IsNoNeedResult);
                    sb.AppendFormat("<IsPCLExamType>{0}</IsPCLExamType>", details.IsPCLExamType);
                    sb.AppendFormat("<IsPCLExamTest>{0}</IsPCLExamTest>", details.IsPCLExamTest);
                    sb.Append("</RecInfo>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region MAPPCLExamTypeDeptLoc
        public override Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc()
        {
            var result = new Dictionary<long, PCLExamType>();

            DataTable dt = PCLExamTypes_AllActive();
            foreach (DataRow row in dt.Rows)
            {
                PCLExamType p = new PCLExamType();
                p.PCLExamTypeID = Convert.ToInt64(row["PCLExamTypeID"]);
                p.ObjDeptLocationList = new ObservableCollection<DeptLocation>();
                var lst = ResourcesManagement.Instance.GetDeptLocationByExamType(p.PCLExamTypeID);
                p.ObjDeptLocationList = new ObservableCollection<DeptLocation>(lst);
                result.Add(p.PCLExamTypeID, p);
            }
            return result;
        }

        private DataTable PCLExamTypes_AllActive()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_AllActive", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                return ExecuteDataTable(cmd);
            }
        }

        #endregion



    }

}