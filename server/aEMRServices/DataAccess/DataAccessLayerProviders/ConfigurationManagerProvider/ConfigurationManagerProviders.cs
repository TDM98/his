/*
 * 20170512 #001 CMN:   Added method to get pcl room by catid
 * 20190725 #002 TTM:   BM 0011965: Bổ sung V_PCLMainCategory để lưu cho chính xác vì đã bổ sung thêm 1 số loại mới.
 * 20191003 #003 TTM:   BM 0017393: Bổ sung thông tin chuyên khoa cho dịch vụ
 * 20220620 #004 DatTB: Thêm filter danh mục xét nghiệm
 * 20230327 #005 BLQ: Thêm check pp sinh và check có thiết bị
 * 20230509 #006 DatTB: 
 * + IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
 * + IssueID: 3255 | Thêm trường ghi chú cho danh mục phòng
 * 20230518 #007 DatTB: Thêm service cho mẫu bệnh phẩm
 * 20230519 #008 DatTB: Thêm cột tên tiếng anh các danh mục xét nghiệm, khoa phòng
 * 20230531 #009 QTD:   Thêm quản lý danh mục
 * 20230601 #010 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
 * 20230622 #011 DatTB:
 * + Thêm filter theo mã ICD đã lưu của nhóm bệnh
 * + Thêm function chỉnh sửa ICD của nhóm bệnh
 * + Thêm function xuất excel ICD của nhóm bệnh
 * + Thay đổi điều kiện gàng buộc ICD
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using eHCMS.Configurations;
using System.Collections.ObjectModel;
using eHCMS.DAL;
using AxLogging;
using System.Xml.Linq;
using DataEntities.MedicalInstruction;
using System.IO;

namespace aEMR.DataAccessLayer.Providers
{
    public class ConfigurationManagerProviders : DataProviderBase
    {
        static private ConfigurationManagerProviders _instance = null;
        static public ConfigurationManagerProviders Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigurationManagerProviders();
                }
                return _instance;
            }
        }
        public ConfigurationManagerProviders()
        {
            this.ConnectionString = Globals.Settings.ConfigurationManager.ConnectionString;
        }

        #region RefDepartments member

        protected virtual DeptLocation GetDepartmentObjFromReader(IDataReader reader)
        {
            DeptLocation p = new DeptLocation();
            try
            {
                if (reader.HasColumn("LID") && reader["LID"] != DBNull.Value)
                {
                    p.LID = (long)reader["LID"];
                }

                if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
                {
                    p.DeptID = (long)reader["DeptID"];
                }

                if (reader.HasColumn("DeptLocationID") && reader["DeptLocationID"] != DBNull.Value)
                {
                    p.DeptLocationID = (long)reader["DeptLocationID"];
                }


                p.RefDepartment = new RefDepartment();
                try
                {
                    if (reader.HasColumn("DeptName") && reader["DeptName"] != DBNull.Value)
                    {
                        p.RefDepartment.DeptName = reader["DeptName"].ToString();
                    }

                    if (reader.HasColumn("DeptDescription") && reader["DeptDescription"] != DBNull.Value)
                    {
                        p.RefDepartment.DeptDescription = reader["DeptDescription"].ToString();
                    }

                    if (reader.HasColumn("V_DeptType") && reader["V_DeptType"] != DBNull.Value)
                    {
                        p.RefDepartment.V_DeptType = (long)reader["V_DeptType"];
                    }

                }
                catch { }

                p.Location = new Location();
                try
                {
                    p.Location.LocationName = reader["LocationName"].ToString();
                    p.Location.LocationDescription = reader["LocationDescription"].ToString();
                    p.Location.RoomType = new RoomType();
                    p.Location.RoomType.RmTypeName = reader["RmTypeName"].ToString();
                    p.Location.RoomType.RmTypeDescription = reader["RmTypeDescription"].ToString();
                }
                catch { }
            }
            catch
            { return null; }
            return p;
        }
        protected virtual List<DeptLocation> GetDepartmentCollectionFromReader(IDataReader reader)
        {
            List<DeptLocation> lst = new List<DeptLocation>();
            while (reader.Read())
            {
                lst.Add(GetDepartmentObjFromReader(reader));
            }
            return lst;
        }



        protected override MedServiceItemPrice GetMedServiceItemPriceFromReader(IDataReader reader)
        {
            MedServiceItemPrice p = base.GetMedServiceItemPriceFromReader(reader);

            //20191204 TBL: Comment chỗ này ra vì khi tạo bảng giá hoặc xem lại bảng giá dịch vụ thì được gọi nhiều lần mà không biết để làm gì
            //Xét CanEdit, CanDelete cho Items này
            //bool CanEdit = false;
            //bool CanDelete = false;
            //string PriceType = "";

            //Int64 MedServItemPriceID = 0;
            //Int64.TryParse(reader["MedServItemPriceID"].ToString(), out MedServItemPriceID);
            //if (MedServItemPriceID > 0)
            //{
            //    MedServiceItemPrice_CheckCanEditCanDelete(p.MedServItemPriceID, out CanEdit, out CanDelete, out PriceType);
            //}
            //p.CanEdit = CanEdit;
            //p.CanDelete = CanDelete;
            //p.PriceType = PriceType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }




        protected override PCLExamTypePrice GetPCLExamTypePriceFromReader(IDataReader reader)
        {
            PCLExamTypePrice p = base.GetPCLExamTypePriceFromReader(reader);
            ////Xét CanEdit, CanDelete cho Items này
            //bool CanEdit = false;
            //bool CanDelete = false;
            //string PriceType = "";

            //Int64 PCLExamTypePriceID = 0;
            //Int64.TryParse(reader["PCLExamTypePriceID"].ToString(), out PCLExamTypePriceID);
            ////if (PCLExamTypePriceID > 0)
            ////{
            ////    PCLExamTypePrices_CheckCanEditCanDelete(Convert.ToInt64(reader["PCLExamTypePriceID"]), out CanEdit, out CanDelete, out PriceType);
            ////}
            //p.CanEdit = CanEdit;
            //p.CanDelete = CanDelete;
            //p.PriceType = PriceType;
            ////Xét CanEdit, CanDelete cho Items này
            return p;
        }



        protected override MedServiceItemPriceList GetMedServiceItemPriceListFromReader(IDataReader reader)
        {
            MedServiceItemPriceList p = base.GetMedServiceItemPriceListFromReader(reader);

            //Xét CanEdit, CanDelete cho Items này
            bool CanEdit = false;
            bool CanDelete = false;
            string PriceListType = "";


            Int64 MedServiceItemPriceListID = 0;
            Int64.TryParse(reader["MedServiceItemPriceListID"].ToString(), out MedServiceItemPriceListID);
            if (MedServiceItemPriceListID > 0)
            {
                MedServiceItemPriceList_CheckCanEditCanDelete(p.MedServiceItemPriceListID, out CanEdit, out CanDelete, out PriceListType);
            }
            p.CanEdit = CanEdit;
            p.CanDelete = CanDelete;
            p.PriceListType = PriceListType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }



        protected override PCLExamTypePriceList GetPCLExamTypePriceListFromReader(IDataReader reader)
        {
            PCLExamTypePriceList p = base.GetPCLExamTypePriceListFromReader(reader);

            //Xét CanEdit, CanDelete cho Items này
            //bool CanEdit = false;
            //bool CanDelete = false;
            //string PriceListType = "";


            Int64 PCLExamTypePriceListID = 0;
            Int64.TryParse(reader["PCLExamTypePriceListID"].ToString(), out PCLExamTypePriceListID);
            //if (PCLExamTypePriceListID > 0)
            //{
            //    PCLExamTypePriceList_CheckCanEditCanDelete(p.PCLExamTypePriceListID, out CanEdit, out CanDelete, out PriceListType);
            //}
            //p.CanEdit = CanEdit;
            //p.CanDelete = CanDelete;
            //p.PriceListType = PriceListType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }



        public RefDepartments GetRefDepartmentsByID(long DeptID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objRefDepartments;
            }
        }

        public List<RefDepartments> GetRefDepartments_AllParent()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        public List<RefDepartments> GetRefDepartments_All()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        //List<RefDepartments> by collect V_DeptID
        public List<RefDepartments> RefDepartments_ByInStrV_DeptType(string strV_DeptType, string V_DeptTypeOperation = null)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //List<RefDepartments> by collect V_DeptID


        public List<RefDepartments> RefDepartments_ByParDeptID(Int64 ParDeptID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }


        public bool DeleteRefDepartmentsByID(long DeptID)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }

        public void UpdateRefDepartments(RefDepartments obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRefDepartments_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, obj.DeptID);
                cmd.AddParameter("@ParDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.ParDeptID));
                cmd.AddParameter("@DeptName", SqlDbType.NVarChar, obj.DeptName);
                //▼==== #008
                cmd.AddParameter("@DeptNameEng", SqlDbType.NVarChar, obj.DeptNameEng);
                //▲==== #008
                cmd.AddParameter("@V_DeptTypeOperation", SqlDbType.BigInt, obj.V_DeptTypeOperation);
                cmd.AddParameter("@DeptDescription", SqlDbType.NVarChar, obj.DeptDescription);

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                {
                    Result = cmd.Parameters["@Result"].Value.ToString();
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public bool AddNewRefDepartments(RefDepartments obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ParDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.ParDeptID));
                cmd.AddParameter("@DeptName", SqlDbType.NVarChar, obj.DeptName);
                //▼==== #008
                cmd.AddParameter("@DeptNameEng", SqlDbType.NVarChar, obj.DeptNameEng);
                //▲==== #008
                cmd.AddParameter("@V_DeptTypeOperation", SqlDbType.BigInt, obj.V_DeptTypeOperation);
                cmd.AddParameter("@DeptDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(obj.DeptDescription));

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);

                CleanUpConnectionAndCommand(cn, cmd);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }

        public List<RefDepartments> SearchRefDepartments(RefDepartmentsSearchCriteria criteria)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        public List<DeptLocation> GetAllDeptLocationByDeptIDFunction(long DeptID, long RoomFunction)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        //---Dinh viet them ham nay---
        public List<DeptLocation> GetDeptLocationFunc(long V_DeptType, long V_RoomFunction)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }

        public List<DeptLocation> GetDeptLocationFuncExt(long V_DeptType, long V_RoomFunction, long V_DeptTypeOperation)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }

        public List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }

        public List<DeptLocation> GetAllDeptLocLaboratory()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }



        //<GetRefDepartment in Table DeptMedServiceItems
        public List<RefDepartments> GetRefDepartmentTree_InTable_DeptMedServiceItems()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //<GetRefDepartment in Table DeptMedServiceItems

        #endregion

        #region RefMedicalServiceItem

        public List<RefMedicalServiceItem> RefMedicalServiceItemsByMedicalServiceTypeID(Int64 MedicalServiceTypeID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }


        //
        public List<RefMedicalServiceItem> RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(RefMedicalServiceItemsSearchCriteria Criteria,
                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        //


        public List<RefMedicalServiceItem> RefMedicalServiceItems_ByMedicalServiceTypeID_Paging(RefMedicalServiceItemsSearchCriteria Criteria,
                                int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<RefMedicalServiceItem> RefMedicalServiceItems_IsPCLByMedServiceID_Paging(RefMedicalServiceItemsSearchCriteria Criteria,
                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public void RefMedicalServiceItems_IsPCL_Save(RefMedicalServiceItem Obj, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void RefMedicalServiceItems_MarkDeleted(Int64 MedServiceID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void RefMedicalServiceItems_EditInfo(RefMedicalServiceItem Obj, out string Result)
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
                {
                    Result = cmd.Parameters["@Result"].Value.ToString();
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void RefMedicalServiceItems_NotPCL_Add(DeptMedServiceItems Obj, out string Result)
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
                {
                    Result = cmd.Parameters["@Result"].Value.ToString();
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public bool RefMedicalServiceItems_NotPCL_Add(RefMedicalServiceItem Obj, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_NotPCL_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.MedicalServiceTypeID));
                    cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceCode));
                    cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName));
                    cmd.AddParameter("@V_RefMedServiceItemsUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_RefMedServiceItemsUnit));
                    cmd.AddParameter("@HITTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.HITTypeID));
                    cmd.AddParameter("@ExpiryDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.ExpiryDate));
                    cmd.AddParameter("@V_NewPriceType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_NewPriceType));
                    cmd.AddParameter("@IsPackageService", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsPackageService));
                    cmd.AddParameter("@HICode5084", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode5084 != null ? Obj.HICode5084.Trim() : null));
                    cmd.AddParameter("@MedServiceName_Ax", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName_Ax));
                    cmd.AddParameter("HICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode));
                    cmd.AddParameter("@V_Surgery_Tips_Item", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Surgery_Tips_Item));
                    cmd.AddParameter("@V_Surgery_Tips_Type", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Surgery_Tips_Type));
                    cmd.AddParameter("@HIPayRatio", SqlDbType.Decimal, ConvertNullObjectToDBNull(Obj.HIPayRatio));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));

                    /*▼====: #002*/
                    cmd.AddParameter("@HIApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.HIApproved));
                    /*▲====: #002*/

                    //▼===== #003
                    cmd.AddParameter("@V_SpecialistType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_SpecialistType));
                    //▲===== #003
                    cmd.AddParameter("@V_AppointmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_AppointmentType));
                    cmd.AddParameter("@IsMedicalExamination", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsMedicalExamination));
                    cmd.AddParameter("@GenderType", SqlDbType.Char, ConvertNullObjectToDBNull(Obj.GenderType));
                    cmd.AddParameter("@V_DVKTPhanTuyen", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_DVKTPhanTuyen));
                    cmd.AddParameter("@NgoaiDinhSuat", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.NgoaiDinhSuat));
                    cmd.AddParameter("@UseAnalgesic", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.UseAnalgesic));
                    cmd.AddParameter("@InCategoryCOVID", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.InCategoryCOVID));
                    //▼===== #005
                    cmd.AddParameter("@IsBirthMethod", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsBirthMethod));
                    cmd.AddParameter("@IsHaveEquip", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsHaveEquip));
                    //▲===== #005
                    cn.Open();

                    ExecuteNonQuery(cmd);

                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(ex.Message);
                }

            }
        }

        public bool RefMedicalServiceItems_NotPCL_Update(RefMedicalServiceItem Obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("spRefMedicalServiceItems_NotPCL_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

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
                    cmd.AddParameter("@V_NewPriceType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_NewPriceType));
                    cmd.AddParameter("@IsPackageService", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsPackageService));
                    cmd.AddParameter("@HICode5084", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode5084 != null ? Obj.HICode5084.Trim() : null));
                    cmd.AddParameter("@MedServiceName_Ax", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.MedServiceName_Ax));
                    cmd.AddParameter("@HICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HICode));
                    cmd.AddParameter("@UpdatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.UpdatedStaffID));
                    cmd.AddParameter("@UpdatedTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.UpdatedTime));
                    cmd.AddParameter("@V_Surgery_Tips_Item", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Surgery_Tips_Item));
                    cmd.AddParameter("@V_Surgery_Tips_Type", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_Surgery_Tips_Type));
                    cmd.AddParameter("@HIPayRatio", SqlDbType.Decimal, ConvertNullObjectToDBNull(Obj.HIPayRatio));

                    //▼====: #002
                    cmd.AddParameter("@HIApproved", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.HIApproved));
                    //▲====: #002                  

                    //▼===== #003
                    cmd.AddParameter("@V_SpecialistType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_SpecialistType));
                    //▲===== #003
                    cmd.AddParameter("@IsMedicalExamination", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsMedicalExamination));
                    cmd.AddParameter("@GenderType", SqlDbType.Char, ConvertNullObjectToDBNull(Obj.GenderType));
                    cmd.AddParameter("@V_DVKTPhanTuyen", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_DVKTPhanTuyen));
                    cmd.AddParameter("@NgoaiDinhSuat", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.NgoaiDinhSuat));
                    cmd.AddParameter("@UseAnalgesic", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.UseAnalgesic));
                    cmd.AddParameter("@InCategoryCOVID", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.InCategoryCOVID));
                    //▼===== #005
                    cmd.AddParameter("@IsBirthMethod", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsBirthMethod));
                    cmd.AddParameter("@IsHaveEquip", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsHaveEquip));
                    //▲===== #005
                    cn.Open();

                    ExecuteNonQuery(cmd);

                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(ex.Message);
                }

            }
        }


        public List<RefMedicalServiceItem> RefMedicalServiceItems_In_DeptLocMedServices(RefMedicalServiceItemsSearchCriteria SearchCriteria)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<RefMedicalServiceItem> GetMedServiceItems_Paging(RefMedicalServiceItemsSearchCriteria Criteria,
                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        #endregion

        #region "PCLExamGroup"
        public List<PCLExamGroup> GetPCLExamGroup_ByMedServiceID_NoPaging(Int64 MedServiceID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        public List<RefDepartments> PCLExamGroup_GetListDeptID()
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        #endregion

        #region "PCLExamTypes"


        public List<PCLExamType> PCLExamTypes_NotYetPCLLabResultSectionID_Paging(PCLExamTypeSearchCriteria Criteria,
                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public List<PCLExamType> GetPCLExamTypes_Paging(PCLExamTypeSearchCriteria Criteria,
                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public List<PCLExamType> PCLExamTypesAndPriceIsActive_Paging(PCLExamTypeSearchCriteria SearchCriteria,
                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public void PCLExamTypes_Save_NotIsLab(PCLExamType Obj, bool IsInsert, long StaffID, out string Result, out long PCLExamTypeID_New)
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
                    cmd.AddParameter("@IsUsed", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsUsed));
                    cmd.AddParameter("@IsInsert", SqlDbType.Bit, ConvertNullObjectToDBNull(IsInsert));
                    cmd.AddParameter("@IsRegimenChecking", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsRegimenChecking));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));

                    cmd.AddParameter("@ModalityCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.ModalityCode));

                    cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);
                    cmd.AddParameter("@PCLExamTypeID_New", SqlDbType.BigInt, sizeof(Int64), ParameterDirection.Output);
                    //▼===== #002
                    cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_PCLMainCategory));
                    //CMN: Lưu thêm cờ xác định là dịch vụ cận lâm sàng bị giới hạn số chỉ tiêu thực hiện dành cho filter trên chức năng hẹn CLS sổ
                    cmd.AddParameter("@IsCasePermitted", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsCasePermitted));
                    //▲===== #002
                    cn.Open();

                    ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@Result"].Value != null)
                        Result = cmd.Parameters["@Result"].Value.ToString();
                    // Hpt 28/11/2015: Sau khi lưu xuống, Stored trả về ID của dòng mới để thực hiện cập nhật trực tiếp MAPPCLExamTypeDeptLoc mà không cần restart Service. (CHỈ ÁP DỤNG CẬP NHẬT, LƯU MỚI TỪ TỪ TÍNH)
                    // Nếu không cập nhật danh sách MAPPCLExamTypeDeptLoc sau khi cập nhật dịch vụ, khi lưu bill chứa dịch vụ mới cấu hình lại mà chưa restart Service sẽ không lấy ra được danh sách phòng thực hiện dịch vụ --> Error
                    if (IsUpdate && cmd.Parameters["@PCLExamTypeID_New"].Value != null && (Convert.ToInt64(cmd.Parameters["@PCLExamTypeID_New"].Value) > 0))
                    {
                        Obj.ObjDeptLocationList = DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID].ObjDeptLocationList;
                        PCLExamTypeID_New = Convert.ToInt64(cmd.Parameters["@PCLExamTypeID_New"].Value);
                        Obj.PCLExamTypeID = PCLExamTypeID_New;
                        DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID] = Obj;
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }


        public void PCLExamTypes_Save_IsLab_Old(PCLExamType Obj,
            // ExamType la ExamTest
            bool TestItemIsExamType,
            string PCLExamTestItemUnitForPCLExamType,
            string PCLExamTestItemRefScaleForPCLExamType,
            // ExamType la ExamTest

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
                {
                    Result = cmd.Parameters["@Result"].Value.ToString();
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void PCLExamTypes_Save_IsLab(PCLExamType Obj, bool IsInsert,
              // ExamType la ExamTest
              bool TestItemIsExamType,
              string PCLExamTestItemUnitForPCLExamType,
              string PCLExamTestItemRefScaleForPCLExamType,
              // ExamType la ExamTest
              long StaffID,
              IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
              IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
              IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete, out string Result, out long PCLExamTypeID_New)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    Result = "";
                    PCLExamTypeID_New = 0;
                    bool IsUpdate = (Obj.PCLExamTypeID > 0);
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
                    cmd.AddParameter("@IsUsed", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsUsed));
                    cmd.AddParameter("@IsInsert", SqlDbType.Bit, ConvertNullObjectToDBNull(IsInsert));
                    cmd.AddParameter("@IsRegimenChecking", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsRegimenChecking));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cn.Open();

                    ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@Result"].Value != null)
                    {
                        Result = cmd.Parameters["@Result"].Value.ToString();
                    }
                    if (IsUpdate && cmd.Parameters["@PCLExamTypeID_New"].Value != null && (Convert.ToInt64(cmd.Parameters["@PCLExamTypeID_New"].Value) > 0))
                    {

                        Obj.ObjDeptLocationList = DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID].ObjDeptLocationList;
                        Obj.PCLExamTypeID = Convert.ToInt64(cmd.Parameters["@PCLExamTypeID_New"].Value);
                        DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID] = Obj;
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }

        public void PCLExamTypes_MarkDelete(Int64 PCLExamTypeID, out string Result)
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
                    {
                        Result = cmd.Parameters["@Result"].Value.ToString();
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public List<PCLExamType> PCLExamTypes_List_Paging(PCLExamTypeSearchCriteria Criteria,
                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public List<PCLExamType> PCLExamTypesByDeptLocationID_LAB(long DeptLocationID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<PCLExamType> PCLExamTypesByDeptLocationID_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID, long DeptLocationID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public void PCLExamTypes_Save_General(PCLExamType Obj, bool IsInsert,
             // ExamType la ExamTest
             bool TestItemIsExamType,
             string PCLExamTestItemUnitForPCLExamType,
             string PCLExamTestItemRefScaleForPCLExamType,
             // ExamType la ExamTest
             long StaffID,
             IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
             IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
             IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete, out string Result, out long PCLExamTypeID_New)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    Result = "";
                    PCLExamTypeID_New = 0;
                    bool IsUpdate = (Obj.PCLExamTypeID > 0);
                    SqlCommand cmd = new SqlCommand("spPCLExamTypes_Save_General", cn);
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
                    cmd.AddParameter("@IsUsed", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsUsed));
                    cmd.AddParameter("@IsInsert", SqlDbType.Bit, ConvertNullObjectToDBNull(IsInsert));
                    cmd.AddParameter("@IsRegimenChecking", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsRegimenChecking));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_PCLMainCategory));
                    cmd.AddParameter("@ModalityCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.ModalityCode));
                    cmd.AddParameter("@IsCasePermitted", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsCasePermitted));
                    cmd.AddParameter("@V_ReportForm", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_ReportForm));
                    cmd.AddParameter("@InCategoryCOVID", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.InCategoryCOVID));
                    cmd.AddParameter("@PCLExamTypeTemplateResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeTemplateResult));
                    cmd.AddParameter("@IsAllowEditAfterDischarge", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsAllowEditAfterDischarge));
                    cmd.AddParameter("@DateAllowEditAfterDischarge", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.DateAllowEditAfterDischarge));
                    //▼==== #008
                    cmd.AddParameter("@PCLExamTypeNameEng", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLExamTypeNameEng));
                    //▲==== #008
                    cn.Open();

                    ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@Result"].Value != null)
                    {
                        Result = cmd.Parameters["@Result"].Value.ToString();
                    }
                    if (IsUpdate && cmd.Parameters["@PCLExamTypeID_New"].Value != null && (Convert.ToInt64(cmd.Parameters["@PCLExamTypeID_New"].Value) > 0))
                    {

                        Obj.ObjDeptLocationList = DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID].ObjDeptLocationList;
                        Obj.PCLExamTypeID = Convert.ToInt64(cmd.Parameters["@PCLExamTypeID_New"].Value);
                        DataProviderBase.MAPPCLExamTypeDeptLoc[Obj.PCLExamTypeID] = Obj;
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region PCLExamTypeCombo

        public List<PCLExamTypeCombo> PCLExamTypeCombo_Search(GeneralSearchCriteria SearchCriteria)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<PCLExamTypeComboItem> PCLExamTypeComboItems_ByComboID(long ComboID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<PCLExamTypeComboItem> PCLExamTypeComboItems_All(long? PCLExamTypePriceListID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeComboItems_All", cn);
                cmd.AddParameter("@PCLExamTypePriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypePriceListID));
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<PCLExamTypeComboItem> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTypeComboItemColectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public bool PCLExamTypeCombo_Save(PCLExamTypeCombo item, List<PCLExamTypeComboItem> ComboXML_Insert, List<PCLExamTypeComboItem> ComboXML_Update, List<PCLExamTypeComboItem> ComboXML_Delete, out long ID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return ID > 0;
            }
        }

        public bool PCLExamTypeCombo_Delete(long ID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeCombo_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeComboID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ID));
                cn.Open();
                bool bRes = cmd.ExecuteNonQuery() > 0;

                CleanUpConnectionAndCommand(cn, cmd);
                return bRes;
            }
        }
        #endregion

        #region PCLExamTypeMedServiceDefItems

        public List<PCLExamType> PCLExamTypeMedServiceDefItems_ByMedServiceID(PCLExamTypeSearchCriteria SearchCriteria, Int64 MedServiceID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //Danh sach da chon co trong DB


        //Save danh sách tập hợp
        public bool PCLExamTypeMedServiceDefItems_XMLInsert(Int64 MedServiceID, IEnumerable<PCLExamType> ObjPCLExamTypeList)
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

                CleanUpConnectionAndCommand(cn, cmd1);
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
        public List<MedServiceItemPrice> GetDeptMedServiceItems_Paging(DeptMedServiceItemsSearchCriteria Criteria,
                                int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<DeptMedServiceItems> GetDeptMedServiceItems_DeptIDPaging(DeptMedServiceItemsSearchCriteria Criteria,
                                    int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<MedServiceItemPrice> GetMedServiceItemPrice_Paging(MedServiceItemPriceSearchCriteria Criteria,
                                int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        public bool DeptMedServiceItems_TrueDelete(Int64 DeptMedServItemID, Int64 MedServItemPriceID, Int64 MedServiceID)
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

                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }



        public bool DeptMedServiceItems_InsertXML(IList<DeptMedServiceItems> lstDeptMedServiceItems)
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

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool DeptMedServiceItems_DeleteXML(IList<DeptMedServiceItems> lstDeptMedServiceItems)
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

                    CleanUpConnectionAndCommand(cn, cmd);
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

        #endregion

        #region "RefMedicalServiceTypes"
        public List<RefMedicalServiceType> GroupRefMedicalServiceTypes_ByMedicalServiceTypeID(Int64 DeptID, string MedServiceName)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<RefMedicalServiceType> GetAllMedicalServiceTypes()
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<RefMedicalServiceType> RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<RefMedicalServiceType> GetAllMedicalServiceTypes_SubtractPCL()
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<RefMedicalServiceType> RefMedicalServiceTypes_Paging(RefMedicalServiceTypeSearchCriteria Criteria,
                                    int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public void RefMedicalServiceTypes_CheckBeforeInsertUpdate(RefMedicalServiceType Obj, out string Result)
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
                {
                    Result = cmd.Parameters["@Result"].Value.ToString();
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        public void RefMedicalServiceTypes_AddEdit(RefMedicalServiceType Obj, out string Result)
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
                {
                    Result = cmd.Parameters["@Result"].Value.ToString();
                }

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public bool RefMedicalServiceTypes_MarkDelete(Int64 MedicalServiceTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefMedicalServiceTypes_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedicalServiceTypeID));

                cn.Open();

                bool bRes = (ExecuteNonQuery(cmd) > 0);

                CleanUpConnectionAndCommand(cn, cmd);
                if (bRes)
                    return true;
                return false;
            }
        }

        public IList<RefMedicalServiceType> RefMedicalServiceTypes_ByV_RefMedicalServiceTypes(Int64 V_RefMedicalServiceTypes)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }


        #endregion

        #region "MedServiceItemPrice"
        public List<MedServiceItemPrice> MedServiceItemPriceByDeptMedServItemID_Paging(MedServiceItemPriceSearchCriteria Criteria,
                                int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void MedServiceItemPrice_CheckCanEditCanDelete(Int64 MedServItemPriceID, out bool CanEdit, out bool CanDelete, out string PriceType)
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

                CleanUpConnectionAndCommand(cn, cmd);

            }
        }


        public void MedServiceItemPrice_Save(MedServiceItemPrice Obj, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void MedServiceItemPrice_MarkDelete(Int64 MedServItemPriceID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        //Đọc by ID
        public MedServiceItemPrice MedServiceItemPrice_ByMedServItemPriceID(Int64 MedServItemPriceID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return ObjMedServiceItemPrice;
            }
        }
        //Đọc by ID


        #endregion

        #region PCLItem


        public bool PCLItems_XMLInsert(Int64 PCLFormID, IEnumerable<PCLExamType> ObjPCLExamTypeList)
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

                CleanUpConnectionAndCommand(cn, cmd1);
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


        public List<PCLExamType> PCLItems_GetPCLExamTypeIDByPCLSectionID(string PCLExamTypeName, Int64 PCLSectionID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<PCLExamType> PCLExamType_WithDeptLocIDs_GetAll()
        {
            var listPCLExamTypes = BuildPCLExamTypeDeptLocMapBase(false);
            if (listPCLExamTypes != null)
                return (List<PCLExamType>)listPCLExamTypes;
            return null;
        }

        public List<PCLExamType> PCLItems_ByPCLFormID(PCLExamTypeSearchCriteria SearchCriteria, Int64 PCLFormID, long? PCLExamTypePriceListID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLItems_ByPCLFormID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeName));
                cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLFormID));
                cmd.AddParameter("@PCLExamTypePriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypePriceListID));
                cn.Open();
                List<PCLExamType> objList = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }


        public List<PCLExamType> PCLItems_SearchAutoComplete(PCLExamTypeSearchCriteria SearchCriteria,
                                int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public List<PCLExamType> GetPCLExamType_byComboID(long PCLExamTypeComboID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<PCLExamType> GetPCLExamType_byHosID(long HosID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        #endregion

        #region PCLForms
        //
        public List<PCLForm> PCLForms_GetList_Paging(PCLFormsSearchCriteria SearchCriteria,
                                    int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void PCLForms_Save(PCLForm Obj, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void PCLForms_MarkDelete(Int64 PCLFormID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        //

        #endregion

        #region PCLSections

        public List<PCLSection> PCLSections_All()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        //▼==== #004
        public List<PCLExamType> PCLExamTypes_All()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;


                cn.Open();
                List<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeCollectionFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //▲==== #004

        public List<PCLSection> PCLSections_GetList_Paging(PCLSectionsSearchCriteria SearchCriteria,
                                int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void PCLSections_Save(PCLSection Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spPCLSections_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLSectionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLSectionID));
                cmd.AddParameter("@PCLSectionName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLSectionName));
                //▼==== #008
                cmd.AddParameter("@PCLSectionNameEng", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.PCLSectionNameEng));
                //▲==== #008
                //cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DeptID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void PCLSections_MarkDelete(Int64 PCLSectionID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        #endregion

        #region Locations
        public void Locations_InsertUpdate(Location Obj, bool SaveToDB, out string Result)
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
                //▼==== #006
                cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Notes));
                //▲==== #006
                cmd.AddParameter("@V_SpecialistClinicType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_SpecialistClinicType == null ? 0 : Obj.V_SpecialistClinicType.LookupID));
                cmd.AddParameter("@SaveToDB", SqlDbType.Bit, SaveToDB);

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        //Save list 
        public bool Locations_XMLInsert(Location objCollect)
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

                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }

        }
        //Save list 

        //list paging
        public List<Location> Locations_ByRmTypeID_Paging(LocationSearchCriteria Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        //list paging


        public void Locations_MarkDeleted(Int64 LID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        #endregion

        #region RoomType
        public List<RoomType> RoomType_GetAll()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<RoomType> RoomType_GetList_Paging(RoomTypeSearchCriteria Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void RoomType_Save(RoomType Obj, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void RoomType_MarkDelete(Int64 RmTypeID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        #endregion

        #region ICD

        public List<ICD> ICD_GetAll()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spICD_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@HITTypeID", SqlDbType.Int, HITTypeID);

                cn.Open();
                List<ICD> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetICDCollectionFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        public List<DiseaseChapters> DiseaseChapters_GetAll()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiseaseChapters_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@HITTypeID", SqlDbType.Int, HITTypeID);

                cn.Open();
                List<DiseaseChapters> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetDiseaseChapterCollectionFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        public List<DiseaseChapters> Chapter_Paging(string SearchChapter,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spChapter_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseChapterNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchChapter));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<DiseaseChapters> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetDiseaseChapterCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<Diseases> Diseases_Paging(int DiseaseChapterID, string SearchDisease,
                                      int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiseases_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseChapterID", SqlDbType.Int, ConvertNullObjectToDBNull(DiseaseChapterID));
                cmd.AddParameter("@DiseaseNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchDisease));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<Diseases> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetDiseasesCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<ICD> ICD_ByIDCode_Paging(ICDSearchCriteria Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spICD_ByIDCode_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ICD10Code", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.ICD10Code));
                cmd.AddParameter("@DiseaseNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.DiseaseNameVN));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Criteria.IsActive));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<ICD> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetICDCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<ICD> ICD_ByDiseaseID_Paging(long Disease_ID,
                                       int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spICD_ByDiseaseID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Disease_ID));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<ICD> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetICDCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<Diseases> Diseases_ByChapterID(int DiseaseChapterID)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiseases_ByChapterID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseChapterID", SqlDbType.Int, ConvertNullObjectToDBNull(DiseaseChapterID));

                cn.Open();
                List<Diseases> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetDiseasesCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void Chapter_Save(DiseaseChapters Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spChapter_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseChapterID", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.DiseaseChapterID));
                cmd.AddParameter("@DiseaseChapterNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DiseaseChapterNameVN));
                cmd.AddParameter("@ICDXCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.ICDXCode));

                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ModifiedLog));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public void Diseases_Save(Diseases Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spDiseases_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DiseaseID));
                cmd.AddParameter("@DiseaseNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DiseaseNameVN));
                cmd.AddParameter("@ICDXCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.ICDXCode));
                cmd.AddParameter("@DiseaseChapterID", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.DiseaseChapterID));
                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ModifiedLog));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public void ICD_Save(ICD Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spICD_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.IDCode));
                cmd.AddParameter("@ICD10Code", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ICD10Code));
                cmd.AddParameter("@DiseaseNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DiseaseNameVN));
                cmd.AddParameter("@DiseaseDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DiseaseDescription));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));
                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ModifiedLog));
                cmd.AddParameter("@Gender", SqlDbType.Char, ConvertNullObjectToDBNull(Obj.Gender.ID));
                cmd.AddParameter("@AgeFrom", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.AgeFrom));
                cmd.AddParameter("@AgeTo", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.AgeTo));
                cmd.AddParameter("@NotBeMain", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.NotBeMain));
                cmd.AddParameter("@IsNewInYear", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsNewInYear));
                cmd.AddParameter("@IsLongTermIllness", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsLongTermIllness));
                cmd.AddParameter("@IsICD10CodeYHCT", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsICD10CodeYHCT));
                cmd.AddParameter("@ICD10CodeFromYHCT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ICD10CodeFromYHCT));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void ICD_MarkDelete(Int64 IDCode, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spICD_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public bool ICD_XMLInsert(Int64 Disease_ID, IEnumerable<ICD> objCollect)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spICD_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@DiseaseID", SqlDbType.BigInt, Disease_ID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertListICDToXml(Disease_ID, objCollect));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);

                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }
        public string ConvertListICDToXml(Int64 Disease_ID, IEnumerable<ICD> objCollect)
        {
            if (objCollect != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (ICD item in objCollect)
                {
                    sb.Append("<ICD>");
                    sb.AppendFormat("<DiseaseID>{0}</DiseaseID>", Disease_ID);
                    sb.AppendFormat("<IDCode>{0}</IDCode>", item.IDCode);
                    sb.AppendFormat("<ICD10Code>{0}</ICD10Code>", item.ICD10Code);
                    sb.Append("</ICD>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        public string ConvertListOutpatientTreatmentTypeICD10LinkToXml(IEnumerable<OutpatientTreatmentTypeICD10Link> objCollect)
        {
            if (objCollect != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (OutpatientTreatmentTypeICD10Link item in objCollect)
                {
                    sb.Append("<OutpatientTreatmentTypeICD10Link>");
                    sb.AppendFormat("<OutpatientTreatmentTypeID>{0}</OutpatientTreatmentTypeID>", item.OutpatientTreatmentTypeID);
                    sb.AppendFormat("<IDCode>{0}</IDCode>", item.IDCode);
                    sb.AppendFormat("<ICD10>{0}</ICD10>", item.ICD10);
                    sb.Append("</OutpatientTreatmentTypeICD10Link>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        public List<ICD> SearchICD_Paging(long DiseaseChapterID, long Disease_ID, string ICD10Code,
                                      int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSearchICD_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseChapterID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiseaseChapterID));
                cmd.AddParameter("@DiseaseID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Disease_ID));
                cmd.AddParameter("@ICD10Code", SqlDbType.VarChar, ConvertNullObjectToDBNull(ICD10Code));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<ICD> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetICDCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        #endregion

        #region InsuranceBenefitCategorie

        public List<InsuranceBenefitCategories_Data> InsuranceBenefitPaging(string HIPCode,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsuranceBenefitPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@HIPCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HIPCode));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<InsuranceBenefitCategories_Data> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetInsuranceBenefitCategories_DataCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void InsuranceBenefitCategories_Save(InsuranceBenefitCategories_Data Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spInsuranceBenefitCategories_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@IBeID", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.IBeID));
                cmd.AddParameter("@HIPCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.HIPCode));
                cmd.AddParameter("@BenefitCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.BenefitCode));
                cmd.AddParameter("@RebatePercentage", SqlDbType.Float, ConvertNullObjectToDBNull(Obj.RebatePercentage));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));
                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ModifiedLog));
                cmd.AddParameter("@HIPName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HIPName));
                cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Notes));
                cmd.AddParameter("@HIPGroup", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.HIPGroup));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        //public void ICD_MarkDelete(Int64 IDCode, out string Result)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        Result = "";

        //        SqlCommand cmd = new SqlCommand("spICD_MarkDelete", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));

        //        cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

        //        cn.Open();

        //        ExecuteNonQuery(cmd);

        //        if (cmd.Parameters["@Result"].Value != null)
        //            Result = cmd.Parameters["@Result"].Value.ToString();

        //        CleanUpConnectionAndCommand(cn, cmd);
        //    }
        //}

        #endregion

        #region Hospital


        public List<Hospital> HospitalPaging(HospitalSearchCriteria Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spHospitalPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.CityProvinceID));
                cmd.AddParameter("@HosName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.HosName));
                cmd.AddParameter("@HosAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.HosAddress));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<Hospital> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetHospitalCollectionFromReader_New(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        //public void ICD_Save(ICD Obj, out string Result)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        Result = "";

        //        SqlCommand cmd = new SqlCommand("spICD_Save", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.IDCode));
        //        cmd.AddParameter("@ICD10Code", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ICD10Code));
        //        cmd.AddParameter("@DiseaseNameVN", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DiseaseNameVN));
        //        cmd.AddParameter("@DiseaseDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DiseaseDescription));
        //        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));

        //        cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

        //        cn.Open();

        //        ExecuteNonQuery(cmd);

        //        if (cmd.Parameters["@Result"].Value != null)
        //            Result = cmd.Parameters["@Result"].Value.ToString();

        //        CleanUpConnectionAndCommand(cn, cmd);
        //    }
        //}

        //public void ICD_MarkDelete(Int64 IDCode, out string Result)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        Result = "";

        //        SqlCommand cmd = new SqlCommand("spICD_MarkDelete", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));

        //        cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

        //        cn.Open();

        //        ExecuteNonQuery(cmd);

        //        if (cmd.Parameters["@Result"].Value != null)
        //            Result = cmd.Parameters["@Result"].Value.ToString();

        //        CleanUpConnectionAndCommand(cn, cmd);
        //    }
        //}

        #endregion

        #region CitiesProvince

        public List<CitiesProvince> CitiesProvince_Paging(string SearchCitiesProvinces,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCitiesProvincePaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.CityProvinceID));
                cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCitiesProvinces));
                //cmd.AddParameter("@HosAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.HosAddress));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<CitiesProvince> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetCityProvinceCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<SuburbNames> SuburbNames_Paging(long CityProvinceID, string SearchSuburbNames,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSuburbNamesPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CityProvinceID));
                cmd.AddParameter("@SuburbName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchSuburbNames));
                //cmd.AddParameter("@HosAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.HosAddress));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<SuburbNames> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetSuburbNamesCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<WardNames> WardNames_Paging(long CityProvinceID, long SuburbNameID, string SearchWardNames,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spWardNamesPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CityProvinceID));
                cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SuburbNameID));
                cmd.AddParameter("@WardName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchWardNames));
                //cmd.AddParameter("@HosAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.HosAddress));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<WardNames> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetWardNamesCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void CitiesProvinces_Save(CitiesProvince Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spCitiesProvince_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CityProvinceID));
                cmd.AddParameter("@CityProvinceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.CityProvinceName));
                cmd.AddParameter("@CityProviceCode", SqlDbType.Char, ConvertNullObjectToDBNull(Obj.CityProviceCode));
                cmd.AddParameter("@CityProviceHICode", SqlDbType.Char, ConvertNullObjectToDBNull(Obj.CityProviceHICode));
                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ModifiedLog));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public void SuburbNames_Save(SuburbNames Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spSuburbNames_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.SuburbNameID));
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CityProvinceID));
                cmd.AddParameter("@SuburbCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.SuburbCode));
                cmd.AddParameter("@SuburbName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.SuburbName));
                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ModifiedLog));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public void WardNames_Save(WardNames Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spWardNames_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@WardNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.WardNameID));
                cmd.AddParameter("@SuburbNameID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.SuburbNameID));
                cmd.AddParameter("@WardCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.WardCode));
                cmd.AddParameter("@WardName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.WardName));
                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ModifiedLog));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        //public void ICD_MarkDelete(Int64 IDCode, out string Result)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        Result = "";

        //        SqlCommand cmd = new SqlCommand("spICD_MarkDelete", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));

        //        cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

        //        cn.Open();

        //        ExecuteNonQuery(cmd);

        //        if (cmd.Parameters["@Result"].Value != null)
        //            Result = cmd.Parameters["@Result"].Value.ToString();

        //        CleanUpConnectionAndCommand(cn, cmd);
        //    }
        //}

        #endregion

        #region Job
        public List<Lookup> Job_Paging(string Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spJob_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@JobName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria));


                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<Lookup> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetLookupCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void Job_Save(Lookup Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spJob_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LookupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.LookupID));
                cmd.AddParameter("@ObjectValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ObjectValue));
                cmd.AddParameter("@Code", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.Code));
                cmd.AddParameter("@DateModified", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateModified));
                cmd.AddParameter("@ModifiedLog", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ModifiedLog));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        //public void ICD_MarkDelete(Int64 IDCode, out string Result)
        //{
        //    using (SqlConnection cn = new SqlConnection(this.ConnectionString))
        //    {
        //        Result = "";

        //        SqlCommand cmd = new SqlCommand("spICD_MarkDelete", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.AddParameter("@IDCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(IDCode));

        //        cmd.AddParameter("@Result", SqlDbType.NVarChar, 200, ParameterDirection.Output);

        //        cn.Open();

        //        ExecuteNonQuery(cmd);

        //        if (cmd.Parameters["@Result"].Value != null)
        //            Result = cmd.Parameters["@Result"].Value.ToString();

        //        CleanUpConnectionAndCommand(cn, cmd);
        //    }
        //}

        #endregion

        #region AdmissionCriteria
        public List<AdmissionCriteria> AdmissionCriteria_Paging(string Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAdmissionCriteria_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriteriaName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria));


                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<AdmissionCriteria> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetAdmissionCriteriaCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void AdmissionCriteria_Save(AdmissionCriteria Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spAdmissionCriteria_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriteriaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.AdmissionCriteriaID));
                cmd.AddParameter("@AdmissionCriteriaCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.AdmissionCriteriaCode));
                cmd.AddParameter("@V_AdmissionCriteriaType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_AdmissionCriteriaType));
                cmd.AddParameter("@AdmissionCriteriaName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.AdmissionCriteriaName));
                cmd.AddParameter("@AdmissionCriteriaName_Ax", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.AdmissionCriteriaName_Ax));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CreatedStaffID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);


                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public List<AdmissionCriteria> GetListAdmissionCriteria()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetListAdmissionCriteria", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<AdmissionCriteria> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetAdmissionCriteriaCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        #endregion

        #region AdmissionCriterion
        public List<SymptomCategory> SymptomCategory_Paging(string Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSymptomCategory_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CriteriaName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria));


                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<SymptomCategory> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetSymptomCategoryCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void SymptomCategory_Save(SymptomCategory Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spSymptomCategory_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SymptomCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.SymptomCategoryID));
                cmd.AddParameter("@V_SymptomType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_SymptomType));
                cmd.AddParameter("@SymptomCategoryName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.SymptomCategoryName));
                cmd.AddParameter("@IsDelete", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsDelete));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CreatedStaffID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);


                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public List<SymptomCategory> GetAllSymptom()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllSymptom", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<SymptomCategory> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetSymptomCategoryCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<AdmissionCriterion> AdmissionCriterion_Paging(string Criteria,
                                    int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAdmissionCriterion_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CriteriaName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria));


                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<AdmissionCriterion> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetAdmissionCriterionCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void AdmissionCriterion_Save(AdmissionCriterion Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spAdmissionCriterion_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriterionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.AdmissionCriterionID));
                cmd.AddParameter("@AdmissionCriterionName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.AdmissionCriterionName));
                cmd.AddParameter("@IsDelete", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsDelete));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CreatedStaffID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);


                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public List<AdmissionCriterionAttachICD> GetICDListByAdmissionCriterionID(long AdmissionCriterionID)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetICDListByAdmissionCriterionID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriterionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AdmissionCriterionID));

                cn.Open();
                List<AdmissionCriterionAttachICD> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetAdmissionCriterionAttachICDCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public bool InsertAdmissionCriterionAttachICD(List<AdmissionCriterionAttachICD> listAdmissionCriterionAttachICD, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsertAdmissionCriterionAttachICD", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@XMLDetail", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertAdmissionCriterionAttachICDToXml(listAdmissionCriterionAttachICD).ToString()));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }

        public bool DeleteAdmissionCriterionAttachICD(long ACAI_ID, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteAdmissionCriterionAttachICD", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ACAI_ID", SqlDbType.Int, ACAI_ID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public List<AdmissionCriterionAttachICD> GetAllAdmissionCriterionAttachICD()
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllAdmissionCriterionAttachICD", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<AdmissionCriterionAttachICD> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetAdmissionCriterionAttachICDCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<AdmissionCriterionAttachSymptom> GetSymptomListByAdmissionCriterionID(long AdmissionCriterionID)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetSymptomListByAdmissionCriterionID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriterionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AdmissionCriterionID));

                cn.Open();
                List<AdmissionCriterionAttachSymptom> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetAdmissionCriterionAttachSymptomCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public bool InsertAdmissionCriterionAttachSymptom(List<AdmissionCriterionAttachSymptom> listAdmissionCriterionAttachSymptom, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsertAdmissionCriterionAttachSymptom", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@XMLDetail", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertAdmissionCriterionAttachSymptomToXml(listAdmissionCriterionAttachSymptom).ToString()));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }

        public bool DeleteAdmissionCriterionAttachSymptom(long ACAS_ID, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteAdmissionCriterionAttachSymptom", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ACAS_ID", SqlDbType.Int, ACAS_ID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public List<GroupPCLs> GroupPCLs_Paging(string Criteria,
                                  int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGroupPCLs_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CriteriaName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria));


                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<GroupPCLs> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetGroupPCLsCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void GroupPCLs_Save(GroupPCLs Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spGroupPCLs_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupPCLID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.GroupPCLID));
                cmd.AddParameter("@GroupPCLName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.GroupPCLName));
                cmd.AddParameter("@IsDelete", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsDelete));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CreatedStaffID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);


                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public List<GroupPCLs> GetAllGroupPCLs()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllGroupPCLs", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<GroupPCLs> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetGroupPCLsCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<AdmissionCriterionAttachGroupPCL> GetGroupPCLListByAdmissionCriterionID(long AdmissionCriterionID)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetGroupPCLListByAdmissionCriterionID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriterionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AdmissionCriterionID));

                cn.Open();
                List<AdmissionCriterionAttachGroupPCL> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetAdmissionCriterionAttachGroupPCLCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public bool InsertAdmissionCriterionAttachGroupPCL(List<AdmissionCriterionAttachGroupPCL> listAdmissionCriterionAttachGroupPCL, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsertAdmissionCriterionAttachGroupPCL", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@XMLDetail", SqlDbType.Xml, ConvertNullObjectToDBNull(ConvertAdmissionCriterionAttachGroupPCLToXml(listAdmissionCriterionAttachGroupPCL).ToString()));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }

        public bool DeleteAdmissionCriterionAttachGroupPCL(long ACAG_ID, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteAdmissionCriterionAttachGroupPCL", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ACAG_ID", SqlDbType.Int, ACAG_ID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public IList<PCLExamType> PCLExamType_ByGroupPCLID(long GroupPCLID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamType_ByGroupPCLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTypeName));
                cmd.AddParameter("@GroupPCLID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupPCLID));

                cn.Open();
                IList<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        public bool PCLExamTypeGroupPCL_XMLInsert(long GroupPCLID, IEnumerable<PCLExamType> ObjList, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spPCLExamTypeGroupPCL_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@GroupPCLID", SqlDbType.BigInt, GroupPCLID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertObjPCLExamTypeGroupPCL_ListToXml(GroupPCLID, ObjList));
                cmd1.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);

                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }
       
        public string ConvertObjPCLExamTypeGroupPCL_ListToXml(long GroupPCLID, IEnumerable<PCLExamType> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PCLExamType items in ObjList)
                {
                    sb.Append("<GroupPCLs_Detail>");
                    sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", items.PCLExamTypeID);
                    sb.AppendFormat("<GroupPCLID>{0}</GroupPCLID>", GroupPCLID);
                    sb.Append("</GroupPCLs_Detail>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        public IList<GroupPCLs> GroupPCL_PCLExamType_ByAdmissionCriterionID(long AdmissionCriterionID, long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGroupPCL_PCLExamType_ByAdmissionCriterionID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTypeName));
                cmd.AddParameter("@AdmissionCriterionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AdmissionCriterionID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cn.Open();
                IList<GroupPCLs> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetGroupPCLs_DetailCollectionFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        public AdmissionCriterionDetail GetAdmissionCriterionDetailByPtRegistrationID(long PtRegistrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAdmissionCriterionDetailByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));

                cn.Open();
                AdmissionCriterionDetail obj = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    obj = GeAdmissionCriterionDetailFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }
        public bool SaveAdmissionCriterionDetail(AdmissionCriterionDetail CurrentAdmissionCriterionDetail)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveAdmissionCriterionDetail", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriterionDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentAdmissionCriterionDetail.AdmissionCriterionDetailID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentAdmissionCriterionDetail.PtRegistrationID));
                cmd.AddParameter("@SymptomList", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CurrentAdmissionCriterionDetail.SymptomList));
                cmd.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentAdmissionCriterionDetail.CreatedStaffID));
                cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(CurrentAdmissionCriterionDetail.CreatedDate));
                cmd.AddParameter("@LastUpdateStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CurrentAdmissionCriterionDetail.LastUpdateStaffID));
                cmd.AddParameter("@LastUpdateDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(CurrentAdmissionCriterionDetail.LastUpdateDate));

                cn.Open();
                int result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }
        public bool SaveAdmissionCriterionDetail_PCLResult(AdmissionCriterionDetail_PCLResult Obj)
        {
            var strFolderPath = "";
            try
            {
                strFolderPath = Path.GetFullPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Globals.AxServerSettings.Hospitals.PCLStorePool, Globals.AxServerSettings.Pcls.PCLImageResultFolder, DateTime.Now.ToString("yyMMdd")));
                if (!System.IO.Directory.Exists(strFolderPath))
                {
                    System.IO.Directory.CreateDirectory(strFolderPath);
                }
                string strFileName = Obj.ImageResultUrl;
                string strFolderPathAndFileName = Path.Combine(strFolderPath, strFileName);
                if (!File.Exists(strFolderPathAndFileName) && Obj.File != null)
                {
                    using (FileStream fs = new FileStream(strFolderPathAndFileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(Obj.File, 0, Obj.File.Length);
                        fs.Close();
                    }
                    //item.PCLResultFileName = strFileName;
                    //item.PCLResultLocation = strFolderPath;
                }
            }
            catch (Exception ex)
            {
                //List<string> ListFilePathForDeletion = new List<string>();
                //foreach (var item in ResultFile)
                //{
                //    ListFilePathForDeletion.Add(Path.Combine(item.PCLResultLocation, item.PCLResultFileName));
                //}
                //DeleteStoredImageFile(ListFilePathForDeletion);
                throw (ex);
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveAdmissionCriterionDetail_PCLResult", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriterionDetail_PCLResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.AdmissionCriterionDetail_PCLResultID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PtRegistrationID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.PCLExamTypeID));
                cmd.AddParameter("@V_ResultType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_ResultType));
                cmd.AddParameter("@ImageResultUrl", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ImageResultUrl));
                cmd.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CreatedStaffID));
                cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.CreatedDate));
                cmd.AddParameter("@LastUpdateStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.LastUpdateStaffID));
                cmd.AddParameter("@LastUpdateDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.LastUpdateDate));

                cn.Open();
                int result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }
        #endregion

        #region BedCategory
        public List<BedCategory> BedCategory_Paging(BedCategorySearchCriteria Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedCategory_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_BedType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.V_BedType));
                cmd.AddParameter("@HosBedCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.HosBedCode));
                cmd.AddParameter("@HosBedName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.HosBedName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@IsBookedBed", SqlDbType.Bit, Criteria.IsBookBed);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<BedCategory> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetBedCategoryCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<BedCategory> BedCategory_ByDeptLocID_Paging(BedCategorySearchCriteria Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedCategory_ByDeptLocID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.DeptLocID));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<BedCategory> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetBedCategoryCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public bool BedCategory_InsertXML(IList<BedCategory> lstBedCategory)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedCategory_InsertXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, BedCategory_ConvertListToXml(lstBedCategory));
                    cn.Open();
                    cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool BedCategory_DeleteXML(IList<BedCategory> lstBedCategory)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedCategory_DeleteXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, BedCategory_ConvertListToXml(lstBedCategory));
                    cn.Open();
                    cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool CheckValidBedCategory(long BedCategoryID)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckValidBedCategory", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@BedCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedCategoryID));

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);

                CleanUpConnectionAndCommand(cn, cmd);
                if (ReturnVal > 0)//Tồn Tại rồi có rồi
                    return true;
                return false;
            }

        }
        public void BedCategory_Save(BedCategory Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spBedCategory_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@BedCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.BedCategoryID));
                cmd.AddParameter("@HIBedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.HIBedCode));
                cmd.AddParameter("@HIBedName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HIBedName));
                cmd.AddParameter("@HosBedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.HosBedCode));
                cmd.AddParameter("@HosBedName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.HosBedName));
                cmd.AddParameter("@V_BedType", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.V_BedType));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        private string BedCategory_ConvertListToXml(IList<BedCategory> allUserGroup)
        {
            if (allUserGroup != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (BedCategory item in allUserGroup)
                {
                    sb.Append("<BedCategory>");
                    sb.AppendFormat("<BedCategoryID>{0}</BedCategoryID>", item.BedCategoryID);
                    sb.AppendFormat("<DeptLocID>{0}</DeptLocID>", item.DeptLocID);
                    sb.AppendFormat("<HosBedCode>{0}</HosBedCode>", item.HosBedCode);
                    sb.Append("</BedCategory>");
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

        #region DeptLocation
        public List<Location> DeptLocation_ByDeptID(Int64 DeptID, Int64 RmTypeID, string LocationName)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public List<RoomType> DeptLocation_GetRoomTypeByDeptID(Int64 DeptID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public bool DeptLocation_CheckLIDExists(Int64 DeptID, Int64 LID)
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

                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)//Tồn Tại rồi có rồi
                    return true;
                return false;
            }
        }

        public bool DeptLocation_XMLInsert(Int64 DeptID, IEnumerable<Location> objCollect)
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

                CleanUpConnectionAndCommand(cn, cmd1);
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


        public void DeptLocation_MarkDeleted(Int64 DeptLocationID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        public List<DeptLocation> DeptLocation_ByMedicalServiceTypeIDDeptID(Int64 MedicalServiceTypeID, Int64 DeptID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }


        public List<DeptLocation> GetAllLocationsByDeptID(long? DeptID, long? V_RoomFunction = null)
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

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }


        public List<DeptLocation> ListDeptLocation_ByPCLExamTypeID(long PCLExamTypeID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }


        #endregion

        #region DeptLocMedServices

        //Group loại dv từ ds dv not paging
        public List<RefMedicalServiceType> DeptMedServiceItems_GroupMedSerTypeID_AllForChoose(Int64 DeptID, Int64 MedicalServiceTypeID, string MedServiceName)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //Cho chọn

        //đã lưu ở DB        
        public List<RefMedicalServiceItem> DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(Int64 DeptID, Int64 DeptLocationID, Int64 MedicalServiceTypeID, string MedServiceName)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //Group loại dv từ ds dv not paging
        public List<RefMedicalServiceType> DeptLocMedServices_GroupMedSerTypeID_HasChoose(
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //đã lưu ở DB

        //Save list 
        public bool DeptLocMedServices_XMLInsert(Int64 DeptLocationID, IEnumerable<RefMedicalServiceItem> objCollect)
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

                CleanUpConnectionAndCommand(cn, cmd1);
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
        public List<PCLExamTypePrice> PCLExamTypePrices_ByPCLExamTypeID_Paging(PCLExamTypePriceSearchCriteria Criteria, int PageIndex,
                                    int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void PCLExamTypePrices_CheckCanEditCanDelete(Int64 PCLExamTypePriceID, out bool CanEdit, out bool CanDelete, out string PriceType)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void PCLExamTypePrices_Save(PCLExamTypePrice Obj, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void PCLExamTypePrices_MarkDelete(Int64 PCLExamTypePriceID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public PCLExamTypePrice PCLExamTypePrices_ByPCLExamTypePriceID(Int64 PCLExamTypePriceID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return ObjPCLExamTypePrice;
            }
        }

        #endregion

        #region PCLGroups
        public IList<PCLGroup> PCLGroups_GetAll(long? V_PCLCategory)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }


        public List<PCLGroup> PCLGroups_GetList_Paging(PCLGroupsSearchCriteria SearchCriteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void PCLGroups_Save(PCLGroup Obj, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void PCLGroups_MarkDelete(Int64 PCLGroupID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        #endregion

        #region MedServiceItemPriceList
        //Save
        public void MedServiceItemPriceList_AddNew(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjMedServiceItemPrice, out string Result_PriceList)
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

                CleanUpConnectionAndCommand(cn, cmd1);
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
        public List<MedServiceItemPriceList> MedServiceItemPriceList_GetList_Paging(MedServiceItemPriceListSearchCriteria SearchCriteria,
                                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public void MedServiceItemPriceList_CheckCanEditCanDelete(Int64 MedServiceItemPriceListID, out bool CanEdit, out bool CanDelete, out string PriceListType)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        //MarkDelete
        public void MedServiceItemPriceList_MarkDelete(Int64 MedServiceItemPriceListID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        //MedServiceItemPriceList_Detail
        public List<MedServiceItemPrice> MedServiceItemPriceList_Detail(DeptMedServiceItemsSearchCriteria Criteria,
                                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        //Update
        public void MedServiceItemPriceList_Update(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjCollection, IEnumerable<MedServiceItemPrice> ObjCollection_Insert, IEnumerable<MedServiceItemPrice> ObjCollection_Update, out string Result_PriceList)
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

                CleanUpConnectionAndCommand(cn, cmd1);

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
        public void MedServiceItemPriceList_CheckCanAddNew(Int64 DeptID, Int64 MedicalServiceTypeID, out bool CanAddNew)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        #endregion

        //
        #region PCLExamTypePriceList
        //Check CanAddNew
        public void PCLExamTypePriceList_CheckCanAddNew(out bool CanAddNew)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        //Save
        public void PCLExamTypePriceList_AddNew(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjPCLExamType, out string Result_PriceList)
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

                CleanUpConnectionAndCommand(cn, cmd1);
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

        public List<PCLExamTypePriceList> PCLExamTypePriceList_GetList_Paging(PCLExamTypePriceListSearchCriteria SearchCriteria,
                                            int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public void PCLExamTypePriceList_CheckCanEditCanDelete(Int64 PCLExamTypePriceListID, out bool CanEdit, out bool CanDelete, out string PriceListType)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        //MarkDelete
        public void PCLExamTypePriceList_Delete(
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        //PCLExamTypePriceList Detail
        public List<PCLExamType> PCLExamTypePriceList_Detail(long PCLExamTypePriceListID,
                                    int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        //Update
        public void PCLExamTypePriceList_Update(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjCollection_Update, out string Result_PriceList)
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

                CleanUpConnectionAndCommand(cn, cmd1);
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
        public List<PCLExamTypeTestItems> PCLExamTestItems_ByPCLExamTypeID(Int64 PCLExamTypeID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        #endregion

        #region PCLResultParamImplementations
        /*==== #001 ====*/
        //public  List<PCLResultParamImplementations> PCLResultParamImplementations_GetAll()
        public List<PCLResultParamImplementations> PCLResultParamImplementations_GetAll(long? PCLExamTypeSubCategoryID = null)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        /*==== #001 ====*/
        #endregion

        #region PCLExamTypeSubCategory
        public List<PCLExamTypeSubCategory> PCLExamTypeSubCategory_ByV_PCLMainCategory(Int64 V_PCLMainCategory)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        #endregion

        #region PCLExamTypeLocations
        public IList<PCLExamType> PCLExamTypeLocations_ByDeptLocationID(string PCLExamTypeName, Int64 DeptLocationID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }

        public bool PCLExamTypeLocations_XMLInsert(Int64 DeptLocationID, IEnumerable<PCLExamType> ObjList)
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

                CleanUpConnectionAndCommand(cn, cmd1);
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

        public void PCLExamTypeLocations_MarkDeleted(Int64 PCLExamTypeID, Int64 DeptLocationID, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }


        #endregion

        #region HITransactionType
        public IList<HITransactionType> HITransactionType_GetListNoParentID()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        //▼===== 25072018 TTM
        public IList<HITransactionType> HITransactionType_GetListNoParentID_New()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        //▲===== 25072018 TTM
        #endregion

        #region "RefMedicalServiceGroups"
        public IList<RefMedicalServiceGroups> RefMedicalServiceGroups_GetAll()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        #endregion


        #region PCLExamTypeExamTestPrint

        public IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrint_GetList_Paging(PCLExamTypeExamTestPrintSearchCriteria SearchCriteria,
                                                int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrintIndex_GetAll()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void PCLExamTypeExamTestPrint_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public void PCLExamTypeExamTestPrintIndex_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result)
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

                CleanUpConnectionAndCommand(cn, cmd);
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
        public new Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc()
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

        public List<MedServiceItemPrice> GetBedAllocationAll_ByDeptID(DeptMedServiceItemsSearchCriteria Criteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetBedAllocationAll_ByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedicalServiceTypeID));
                cmd.AddParameter("@MedServiceItemPriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.MedServiceItemPriceListID));
                cmd.AddParameter("@MedServiceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceCode));
                cmd.AddParameter("@MedServiceName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.MedServiceName));

                cn.Open();
                List<MedServiceItemPrice> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetMedServiceItemPriceCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<PCLExamTestItems> PCLExamTestItem_ByPCLExamTypeID(long PCLExamTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTestItem_ByPCLExamTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));

                cn.Open();
                List<PCLExamTestItems> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPCLExamTestItemsColectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public bool SavePCLExamTestItem(List<PCLExamTestItems> listDetail)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                XDocument xmlDetail = listDetail == null ? null : GenerateListToXML_PCLExamTestItemDetail(listDetail);
                XDocument xmlIndex = listDetail == null ? null : GenerateListToXML_PCLExamTestItemIndex(listDetail);
                SqlCommand cmd = new SqlCommand("spSavePCLExamTestItem", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@XMLDetail", SqlDbType.Xml, ConvertNullObjectToDBNull(xmlDetail.ToString()));
                cmd.AddParameter("@XMLIndex", SqlDbType.Xml, ConvertNullObjectToDBNull(xmlIndex.ToString()));

                cn.Open();
                int result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }
        protected virtual XDocument GenerateListToXML_PCLExamTestItemDetail(IList<PCLExamTestItems> listDetail)
        {
            var xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                       new XElement("Details",
                       from detail in listDetail
                       select new XElement("Detail",
                        new XElement("PCLExamTestItemID", detail.PCLExamTestItemID),
                        new XElement("IsBold", detail.IsBold),
                        new XElement("IsNoNeedResult", detail.IsNoNeedResult),
                        new XElement("IsTechnique", detail.IsTechnique),
                        new XElement("IsForMen", detail.IsForMen == null ? null : detail.IsForMen)
                        )));
            return xmlDocument;
        }
        protected virtual XDocument GenerateListToXML_PCLExamTestItemIndex(IList<PCLExamTestItems> listDetail)
        {
            var xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                       new XElement("Details",
                       from detail in listDetail
                       select new XElement("Detail",
                        new XElement("PCLExamTypeTestItemID", detail.PCLExamTypeTestItemID),
                        new XElement("PrintIdx", detail.PrintIdx),
                         new XElement("ColumnValue", detail.ColumnValue == null ? null : detail.ColumnValue)
                        )));
            return xmlDocument;
        }

        #endregion
        #region Exemptions


        //Save Exemptions
        public bool Exemptions_InsertUpdate(PromoDiscountProgram objCollect, out string Result, out long NewID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";
                NewID = 0;
                SqlCommand cmd1 = new SqlCommand("spExemptions_InsertUpdate", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                //cmd1.AddParameter("@DataXML", SqlDbType.Xml, objCollect.ConvertObjLocation_ListToXml(objCollect.ObjLocation_List));
                cmd1.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(objCollect.PromoDiscProgID));
                cmd1.AddParameter("@PromoDiscName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(objCollect.PromoDiscName));
                cmd1.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(objCollect.StaffID));
                cmd1.AddParameter("@ValidFromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(objCollect.ValidFromDate));
                cmd1.AddParameter("@ValidToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(objCollect.ValidToDate));
                cmd1.AddParameter("@DiscountPercent", SqlDbType.Decimal, ConvertNullObjectToDBNull(objCollect.DiscountPercent));
                cmd1.AddParameter("@IsOnPriceDiscount", SqlDbType.Bit, ConvertNullObjectToDBNull(objCollect.IsOnPriceDiscount));
                cmd1.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(objCollect.V_RegistrationType));
                cmd1.AddParameter("@V_DiscountTypeCount", SqlDbType.BigInt, ConvertNullObjectToDBNull(objCollect.V_DiscountTypeCount.LookupID));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);
                cmd1.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);
                cmd1.AddParameter("@NewID", SqlDbType.BigInt, 100, ParameterDirection.Output);
                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (cmd1.Parameters["@Result"].Value != null)
                    Result = cmd1.Parameters["@Result"].Value.ToString();
                if (cmd1.Parameters["@NewID"].Value != null)
                    NewID = Convert.ToInt64(cmd1.Parameters["@NewID"].Value);
                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }
        //Save Exemptions
        public bool ExemptionsMedServiceItems_InsertXML(IList<PromoDiscountItems> lstMedServiceItems)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spExemptionsMedServiceItems_InsertXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, ExemptionsMedServiceItems_ConvertListToXml(lstMedServiceItems));
                    cn.Open();
                    cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool ExemptionsMedServiceItems_DeleteXML(IList<PromoDiscountItems> lstMedServiceItems)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spExemptionsMedServiceItems_DeleteXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, ExemptionsMedServiceItems_ConvertListToXml(lstMedServiceItems));
                    cn.Open();
                    cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        private string ExemptionsMedServiceItems_ConvertListToXml(IList<PromoDiscountItems> promoDiscountItems)
        {
            if (promoDiscountItems != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PromoDiscountItems item in promoDiscountItems)
                {
                    sb.Append("<PromoDiscountItems>");
                    sb.AppendFormat("<PromoDiscItemID>{0}</PromoDiscItemID>", item.PromoDiscItemID);
                    sb.AppendFormat("<PromoDiscProgID>{0}</PromoDiscProgID>", item.PromoDiscProgID);
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", item.MedServiceID);
                    sb.Append("</PromoDiscountItems>");
                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        public bool PCLExamTypeExemptions_XMLInsert(Int64 PromoDiscProgID, IEnumerable<PCLExamType> ObjList)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spPCLExamTypeExemptions_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, PromoDiscProgID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertObjPCLExamTypeExemptions_ListToXml(PromoDiscProgID, ObjList));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);

                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }
        public string ConvertObjPCLExamTypeExemptions_ListToXml(Int64 PromoDiscProgID, IEnumerable<PCLExamType> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PCLExamType items in ObjList)
                {
                    sb.Append("<PromoDiscountItems>");
                    sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", items.PCLExamTypeID);
                    sb.AppendFormat("<PromoDiscProgID>{0}</PromoDiscProgID>", PromoDiscProgID);
                    sb.Append("</PromoDiscountItems>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        //list paging
        public List<PromoDiscountProgram> Exemptions_Paging(string Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spExemptions_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@RmTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.RmTypeID));
                cmd.AddParameter("@ExemptionsName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PromoDiscountProgram> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPromoDiscountProgramCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<PromoDiscountItems> GetExemptionsMedServiceItems_Paging(ExemptionsMedServiceItemsSearchCriteria Criteria,
                                  int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetExemptionsMedServiceItems_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.PromoDiscProgID));
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
                List<PromoDiscountItems> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    //lst = GetMedServiceItemPriceCollectionFromReader(reader); 
                    lst = GetPromoDiscountItemsCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public IList<PCLExamType> PCLExamTypeExemptions(string PCLExamTypeName, Int64 PromoDiscProgID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeExemptions", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTypeName));
                cmd.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PromoDiscProgID));

                cn.Open();
                IList<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //list paging


        public void Exemptions_MarkDeleted(Int64 PromoDiscProgID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spExemptions_MarkDeleted", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PromoDiscProgID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public List<PromoDiscountProgram> GetAllExemptions()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllExemptions", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<PromoDiscountProgram> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPromoDiscountProgramCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                foreach (var item in lst)
                {
                    item.PromoDiscountItems = GetPromoDiscountItems_ByID(item.PromoDiscProgID);
                }
                return lst;
            }
        }
        public List<PromoDiscountItems> GetPromoDiscountItems_ByID(long PromoDiscProgID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPromoDiscountItems_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PromoDiscProgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PromoDiscProgID));
                cn.Open();
                List<PromoDiscountItems> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPromoDiscountItemsCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        #endregion

        public List<DiseaseProgression> DiseaseProgression_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiseaseProgression_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DiseaseProgressionName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<DiseaseProgression> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    //lst = GetMedServiceItemPriceCollectionFromReader(reader); 
                    lst = GetDiseaseProgressionCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void DiseaseProgression_Save(DiseaseProgression Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spDiseaseProgression_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseProgressionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DiseaseProgressionID));
                cmd.AddParameter("@DiseaseProgressionName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DiseaseProgressionName));
                cmd.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CreatedStaffID));
                cmd.AddParameter("@UseForWebsite", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.UseForWebsite));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public void DiseaseProgression_MarkDelete(long DiseaseProgressionID, long StaffID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";
                SqlCommand cmd = new SqlCommand("spDiseaseProgression_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DiseaseProgressionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiseaseProgressionID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);
                cn.Open();
                ExecuteNonQuery(cmd);
                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public List<DiseaseProgressionDetails> DiseaseProgressionDetails_Paging(long DiseaseProgressionID, string SearchDiseaseProgressionDetails, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDiseaseProgressionDetails_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseProgressionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiseaseProgressionID));
                cmd.AddParameter("@SearchDiseaseProgressionDetails", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchDiseaseProgressionDetails));
                //cmd.AddParameter("@HosAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria.HosAddress));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<DiseaseProgressionDetails> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetDiseaseProgressionDetailsCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public void DiseaseProgressionDetails_Save(DiseaseProgressionDetails Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spDiseaseProgressionDetails_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DiseaseProgressionDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DiseaseProgressionDetailID));
                cmd.AddParameter("@DiseaseProgressionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.DiseaseProgressionID));
                cmd.AddParameter("@DiseaseProgressionDetailName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.DiseaseProgressionDetailName));
                cmd.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.CreatedStaffID));
               
              
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public void DiseaseProgressionDetail_MarkDelete(long DiseaseProgressionDetailID, long StaffID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";
                SqlCommand cmd = new SqlCommand("spDiseaseProgressionDetail_MarkDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DiseaseProgressionDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DiseaseProgressionDetailID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);
                cn.Open();
                ExecuteNonQuery(cmd);
                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        #region Gói DVKT PackageTechnicalService
        public bool PackageTechnicalService_InsertUpdate(PackageTechnicalService objCollect, long LoggedStaffID, out string Result, out long NewID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                Result = "";
                NewID = 0;
                SqlCommand cmd1 = new SqlCommand("spPackageTechnicalService_InsertUpdate", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@PackageTechnicalServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(objCollect.PackageTechnicalServiceID));
                cmd1.AddParameter("@Title", SqlDbType.NVarChar, ConvertNullObjectToDBNull(objCollect.Title));
                cmd1.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(objCollect.CreatedStaffID));
                cmd1.AddParameter("@LoggedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(LoggedStaffID));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);
                cmd1.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);
                cmd1.AddParameter("@NewID", SqlDbType.BigInt, 100, ParameterDirection.Output);
                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);
                if (cmd1.Parameters["@Result"].Value != null)
                    Result = cmd1.Parameters["@Result"].Value.ToString();
                if (cmd1.Parameters["@NewID"].Value != null)
                    NewID = Convert.ToInt64(cmd1.Parameters["@NewID"].Value);
                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }

        public bool PackageTechnicalServiceMedServiceItems_InsertXML(IList<PackageTechnicalServiceDetail> lstMedServiceItems)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPackageTechnicalServiceMedServiceItems_InsertXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, PackageTechnicalServiceMedServiceItems_ConvertListToXml(lstMedServiceItems));
                    cn.Open();
                    cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public bool PackageTechnicalServiceMedServiceItems_DeleteXML(IList<PackageTechnicalServiceDetail> lstMedServiceItems)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPackageTechnicalServiceMedServiceItems_DeleteXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, PackageTechnicalServiceMedServiceItems_ConvertListToXml(lstMedServiceItems));
                    cn.Open();
                    cmd.ExecuteNonQuery();

                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        private string PackageTechnicalServiceMedServiceItems_ConvertListToXml(IList<PackageTechnicalServiceDetail> promoDiscountItems)
        {
            if (promoDiscountItems != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PackageTechnicalServiceDetail item in promoDiscountItems)
                {
                    sb.Append("<PackageTechnicalServiceDetail>");
                    sb.AppendFormat("<PackageTechnicalServiceDetailID>{0}</PackageTechnicalServiceDetailID>", item.PackageTechnicalServiceDetailID);
                    sb.AppendFormat("<PackageTechnicalServiceID>{0}</PackageTechnicalServiceID>", item.PackageTechnicalServiceID);
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", item.MedServiceID);
                    sb.AppendFormat("<Qty>{0}</Qty>", item.Qty);
                    sb.Append("</PackageTechnicalServiceDetail>");
                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public bool PCLExamTypePackageTechnicalService_XMLInsert(long PackageTechnicalServiceID, IEnumerable<PCLExamType> ObjList)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spPCLExamTypePackageTechnicalService_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@PackageTechnicalServiceID", SqlDbType.BigInt, PackageTechnicalServiceID);
                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertObjPCLExamTypePackageTechnicalService_ListToXml(PackageTechnicalServiceID, ObjList));
                cmd1.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd1);
                int ReturnVal = (cmd1.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd1.Parameters["@ReturnVal"].Value) : 0);

                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }

        public string ConvertObjPCLExamTypePackageTechnicalService_ListToXml(long PackageTechnicalServiceID, IEnumerable<PCLExamType> ObjList)
        {
            if (ObjList != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PCLExamType items in ObjList)
                {
                    sb.Append("<PackageTechnicalServiceDetail>");
                    sb.AppendFormat("<PCLExamTypeID>{0}</PCLExamTypeID>", items.PCLExamTypeID);
                    sb.AppendFormat("<PackageTechnicalServiceID>{0}</PackageTechnicalServiceID>", PackageTechnicalServiceID);
                    sb.Append("</PackageTechnicalServiceDetail>");
                }
                sb.Append("</DS>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public List<PackageTechnicalService> PackageTechnicalService_Paging(string Criteria,
                                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPackageTechnicalService_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Title", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Criteria));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PackageTechnicalService> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPackageTechnicalServiceCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public List<PackageTechnicalServiceDetail> GetPackageTechnicalServiceMedServiceItems_Paging(ExemptionsMedServiceItemsSearchCriteria Criteria,
                                  int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPackageTechnicalServiceMedServiceItems_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PackageTechnicalServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Criteria.PromoDiscProgID));
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
                List<PackageTechnicalServiceDetail> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPackageTechnicalServiceDetailCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public IList<PCLExamType> PCLExamTypePackageTechnicalService(string PCLExamTypeName, long PackageTechnicalServiceID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypePackageTechnicalService", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PCLExamTypeName));
                cmd.AddParameter("@PackageTechnicalServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PackageTechnicalServiceID));

                cn.Open();
                IList<PCLExamType> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPCLExamTypeColectionFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;
            }
        }
        //list paging


        public void PackageTechnicalService_MarkDeleted(long PackageTechnicalServiceID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spExemptions_MarkDeleted", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PackageTechnicalServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PackageTechnicalServiceID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public List<PackageTechnicalService> GetAllPackageTechnicalServices()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllPackageTechnicalServices", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<PackageTechnicalService> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPackageTechnicalServiceCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                foreach (var item in lst)
                {
                    item.PackageTechnicalServiceDetailList = GetPackageTechnicalServiceDetail_ByID(item.PackageTechnicalServiceID);
                }
                return lst;
            }
        }
        public List<PackageTechnicalServiceDetail> GetPackageTechnicalServiceDetail_ByID(long PackageTechnicalServiceID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPromoDiscountItems_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PackageTechnicalServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PackageTechnicalServiceID));
                cn.Open();
                List<PackageTechnicalServiceDetail> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPackageTechnicalServiceDetailCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<PackageTechnicalService> PackageTechnicalService_Search(GeneralSearchCriteria SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPackageTechnicalService_Search", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ComboName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FindName));
                cn.Open();
                List<PackageTechnicalService> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPackageTechnicalServiceCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<PackageTechnicalServiceDetail> PackageTechnicalServiceDetail_ByID(long PackageTechnicalServiceID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPackageTechnicalServiceDetail_ByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PackageTechnicalServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PackageTechnicalServiceID));
                cn.Open();
                List<PackageTechnicalServiceDetail> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPackageTechnicalServiceDetailCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<PackageTechnicalServiceDetail> PackageTechnicalServiceDetail_All(long? PCLExamTypePriceListID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPackageTechnicalServiceDetail_All", cn);
                cmd.AddParameter("@PCLExamTypePriceListID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypePriceListID));
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<PackageTechnicalServiceDetail> lst = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPackageTechnicalServiceDetailCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        #endregion

        #region Từ điển viết tắt
        public List<ShortHandDictionary> ShortHandDictionary_Paging(string SearchValue,long StaffID,
                                      int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spShortHandDictionary_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SearchValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchValue));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<ShortHandDictionary> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetShortHandDictionariesFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void ShortHandDictionary_Save(ShortHandDictionary Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spShortHandDictionary_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ShortHandDictionaryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ShortHandDictionaryID));
                cmd.AddParameter("@ShortHandDictionaryKey", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ShortHandDictionaryKey));
                cmd.AddParameter("@ShortHandDictionaryValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ShortHandDictionaryValue));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.StaffID));

                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        #endregion

        #region Cấu hình mã nhóm bệnh
        public List<OutpatientTreatmentType> GetOutpatientTreatmentType_Paging(string SearchCode, string SearchName, bool IsDelete, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetOutpatientTreatmentType_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SearchCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCode));
                cmd.AddParameter("@SearchName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchName));
                cmd.AddParameter("@IsDelete", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDelete));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<OutpatientTreatmentType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetOutpatientTreatmentTypeCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void OutPatientTreatmentType_Save(OutpatientTreatmentType Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spOutPatientTreatmentType_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@OutpatientTreatmentTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.OutpatientTreatmentTypeID));
                cmd.AddParameter("@OutpatientTreatmentCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.OutpatientTreatmentCode));
                cmd.AddParameter("@OutpatientTreatmentName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.OutpatientTreatmentName));
                cmd.AddParameter("@MaxNumOfDayMedicalRecord", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.MaxNumOfDayMedicalRecord));
                cmd.AddParameter("@MinNumOfDayMedicalRecord", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.MinNumOfDayMedicalRecord));
                cmd.AddParameter("@MaxNumOfDayTreatment", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.MaxNumOfDayTreatment));
                cmd.AddParameter("@MinNumOfDayTreatment", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.MinNumOfDayTreatment));
                cmd.AddParameter("@MaxNumOfDayMedicine", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.MaxNumOfDayMedicine));
                cmd.AddParameter("@MinNumOfDayMedicine", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.MinNumOfDayMedicine));
                cmd.AddParameter("@IsChronic", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.IsChronic));
                cmd.AddParameter("@IsDeleted", SqlDbType.Int, ConvertNullObjectToDBNull(Obj.IsDeleted));
                cmd.AddParameter("@Log", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Log));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public List<OutpatientTreatmentType> OutpatientTreatmentType_GetAll()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spOutpatientTreatmentType_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

              
                cn.Open();
                List<OutpatientTreatmentType> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetOutpatientTreatmentTypeCollectionFromReader(reader);
                }
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        //▼==== #011
        public List<OutpatientTreatmentTypeICD10Link> OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(long OutpatientTreatmentTypeID, string ICD10, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@OutpatientTreatmentTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutpatientTreatmentTypeID));
                cmd.AddParameter("@ICD10", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ICD10));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<OutpatientTreatmentTypeICD10Link> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetOutpatientTreatmentTypeICD10LinkCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<OutpatientTreatmentTypeICD10Link> OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID(long OutpatientTreatmentTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@OutpatientTreatmentTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OutpatientTreatmentTypeID));

                cn.Open();
                List<OutpatientTreatmentTypeICD10Link> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetOutpatientTreatmentTypeICD10LinkCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public bool OutpatientTreatmentTypeICD10Link_Edit(OutpatientTreatmentTypeICD10Link Obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spOutpatientTreatmentTypeICD10Link_Edit", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@OutpatientTreatmentTypeICD10LinkID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.OutpatientTreatmentTypeICD10LinkID));
                cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.Note));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActive));

                cn.Open();

                var ReturnVal = ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }
        //▲==== #011

        public bool OutpatientTreatmentTypeICD10Link_XMLInsert(ObservableCollection<OutpatientTreatmentTypeICD10Link> objCollect)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("spOutpatientTreatmentTypeICD10Link_XMLInsert", cn);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.AddParameter("@DataXML", SqlDbType.Xml, ConvertListOutpatientTreatmentTypeICD10LinkToXml(objCollect));

                cn.Open();

                var ReturnVal = ExecuteNonQuery(cmd1);

                CleanUpConnectionAndCommand(cn, cmd1);
                if (ReturnVal > 0)
                    return true;
                return false;
            }
        }
        #endregion
        #region Cấu hình RefApplicationConfig
        public List<RefApplicationConfig> RefApplicationConfig_Paging(string SearchRefApplicationConfig, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefApplicationConfig_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SearchRefApplicationConfig", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchRefApplicationConfig));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<RefApplicationConfig> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetRefApplicationConfigCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        public void RefApplicationConfig_Save(RefApplicationConfig Obj, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spRefApplicationConfig_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ConfigItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ConfigItemID));
                cmd.AddParameter("@ConfigItemValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ConfigItemValue));
                cmd.AddParameter("@ConfigItemNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ConfigItemNotes));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();

                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        #endregion
        
        //▼==== #007
        public List<Specimen> GetAllSpecimen()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllSpecimen", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<Specimen> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetSpecimenCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        //▲==== #007
        
        //▼==== #010
        public List<List<string>> ExportExcelConfigurationManager(ConfigurationReportParams Params)
        {
            try
            {
                DataSet dsExportToExcellAll = new DataSet();
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = null;
                    switch (Params.ConfigurationName)
                    {
                        case ConfigurationName.DiseaseChapters:
                            cmd = null;
                            cmd = new SqlCommand("spDiseasesReferences_Export", cn);
                            cmd.AddParameter("@DiseaseChapterID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.DiseaseChapterID));
                            cmd.AddParameter("@DiseaseID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.DiseaseID));
                            break;
                        case ConfigurationName.RefMedicalServiceTypes:
                            cmd = null;
                            cmd = new SqlCommand("spRefMedicalServiceTypes_ExportAll", cn);
                            break;
                        case ConfigurationName.Locations:
                            cmd = null;
                            cmd = new SqlCommand("spLocations_ExportAll", cn);
                            break;
                        case ConfigurationName.PCLExamTypes:
                            cmd = null;
                            cmd = new SqlCommand("spPCLExamTypes_Export", cn);
                            cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.V_PCLMainCategory));
                            cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.PCLExamTypeSubCategoryID));
                            break;
                        case ConfigurationName.DiseaseProgression:
                            cmd = null;
                            cmd = new SqlCommand("spDiseaseProgressionDetails_ExportAll", cn);
                            break;
                        case ConfigurationName.AdmissionCriterionAttachICD:
                            cmd = null;
                            cmd = new SqlCommand("spAdmissionCriterion_ExportICD", cn);
                            cmd.AddParameter("@AdmissionCriterionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.AdmissionCriterionID));
                            break;
                        case ConfigurationName.AdmissionCriterionAttachGroupPCL:
                            cmd = null;
                            cmd = new SqlCommand("spAdmissionCriterion_ExportGroupPCL", cn);
                            cmd.AddParameter("@AdmissionCriterionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.AdmissionCriterionID));
                            break;
                        case ConfigurationName.AdmissionCriterionAttachSymptom:
                            cmd = null;
                            cmd = new SqlCommand("spAdmissionCriterion_ExportSymptom", cn);
                            cmd.AddParameter("@AdmissionCriterionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.AdmissionCriterionID));
                            break;
                        case ConfigurationName.RefMedicalServiceItems:
                            cmd = null;
                            cmd = new SqlCommand("spRefMedicalServiceItems_Export", cn);
                            cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.MedicalServiceTypeID));
                            break;
                        //▼==== #011
                        case ConfigurationName.OutpatientTreatmentTypeICD10Link:
                            cmd = null;
                            cmd = new SqlCommand("spOutpatientTreatmentTypeICD10Link_Export", cn);
                            cmd.AddParameter("@OutpatientTreatmentTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Params.OutpatientTreatmentTypeID));
                            break;
                        //▲==== #011
                        default:
                            break;
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▲==== #010

        public void Lookup_Save(Lookup Obj, long StaffID, out string Result)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                Result = "";

                SqlCommand cmd = new SqlCommand("spLookup_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LookupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.LookupID));
                cmd.AddParameter("@ObjectValue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ObjectValue));
                cmd.AddParameter("@Code130", SqlDbType.VarChar, ConvertNullObjectToDBNull(Obj.Code130));
                cmd.AddParameter("@ObjectTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Obj.ObjectTypeID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@DateActive", SqlDbType.DateTime, ConvertNullObjectToDBNull(Obj.DateActive));
                cmd.AddParameter("@IsActiveLookup", SqlDbType.Bit, ConvertNullObjectToDBNull(Obj.IsActiveLookup));
                cmd.AddParameter("@ObjectNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Obj.ObjectNotes));
                cmd.AddParameter("@Result", SqlDbType.NVarChar, 100, ParameterDirection.Output);

                cn.Open();
                ExecuteNonQuery(cmd);

                if (cmd.Parameters["@Result"].Value != null)
                    Result = cmd.Parameters["@Result"].Value.ToString();

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
    }
}


