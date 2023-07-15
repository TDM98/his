using DataEntities;
using eHCMS.Configurations;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using eHCMS.DAL;
using System.Data;
/*
 * 20210118 #001 TNHX: Thêm service cho PAC
 * 20210217 #002 TNHX: Thêm mã SID - SampleCode cho LIS
 */
namespace aEMR.DataAccessLayer.Providers
{
    public class CommonProvider_V2 : DataProviderBase
    {
        static private CommonProvider_V2 _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public CommonProvider_V2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommonProvider_V2();
                }
                return _instance;
            }
        }
        public CommonProvider_V2()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }

        public List<object> LIS_GetCategories(eLabSoftCategory eLabSoftCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_LIS_List_API", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@para", SqlDbType.Int, (int)eLabSoftCategory);
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<object> mCategories = GetLabSoftCategoryCollectionFromReader(mReader, eLabSoftCategory);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mCategories;
            }
        }
        public List<LIS_PCLRequest> LIS_PatientList(string TuNgay, string DenNgay, int TrangThai)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_LIS_PatientList_API", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@tungay", SqlDbType.DateTime, ConvertNullObjectToDBNull(TuNgay));
                cmd.AddParameter("@denngay", SqlDbType.DateTime, ConvertNullObjectToDBNull(DenNgay));
                cmd.AddParameter("@trangthai", SqlDbType.Int, ConvertNullObjectToDBNull(TrangThai));
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<LIS_PCLRequest> mCategories = GetLIS_PCLRequestCollectionFromReader(mReader);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mCategories;
            }
        }
        public List<LIS_PCLRequest> LIS_Order(string SoPhieuChiDinh, int TrangThai)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_LIS_Order_API", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SoPhieuChiDinh", SqlDbType.VarChar, ConvertNullObjectToDBNull(SoPhieuChiDinh));
                cmd.AddParameter("@Para", SqlDbType.Bit, ConvertNullObjectToDBNull(TrangThai));
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<LIS_PCLRequest> mCategories = GetLIS_PCLRequestCollectionFromReader(mReader);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mCategories;
            }
        }
        public bool LIS_UpdateOrderStatus(string SoPhieuChiDinh, string MaDichVu, int TrangThai, string NgayTiepNhan, out string TrangThaiCapNhat)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_LIS_UpdateOrderStatus_API", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@sophieuyeucau", SqlDbType.VarChar, ConvertNullObjectToDBNull(SoPhieuChiDinh));
                cmd.AddParameter("@Madichvu", SqlDbType.VarChar, ConvertNullObjectToDBNull(MaDichVu));
                cmd.AddParameter("@trangthai", SqlDbType.Int, ConvertNullObjectToDBNull(TrangThai));
                cmd.AddParameter("@ngaycapphat", SqlDbType.DateTime, ConvertNullObjectToDBNull(NgayTiepNhan));
                cmd.AddParameter("@trangthaicapnhat", SqlDbType.VarChar, 3, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                if (cmd.Parameters["@trangthaicapnhat"].Value != DBNull.Value)
                {
                    TrangThaiCapNhat = cmd.Parameters["@trangthaicapnhat"].Value.ToString();
                }
                else TrangThaiCapNhat = null;

                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }

        //▼====: #002
        public bool LIS_Result(string SoPhieuChiDinh, string MaDichVu, string KetQua, string CSBT, string DonViTinh
            , bool BatThuong, int TrangThai, string MaNV_XacNhan, string ThoiGianXacNhan, string MaThietBi
            , string SampleCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_LIS_Result_API", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SoPhieuChiDinh", SqlDbType.VarChar, ConvertNullObjectToDBNull(SoPhieuChiDinh));
                cmd.AddParameter("@MaDichVu", SqlDbType.VarChar, ConvertNullObjectToDBNull(MaDichVu));
                cmd.AddParameter("@KetQua", SqlDbType.NVarChar, ConvertNullObjectToDBNull(KetQua));
                cmd.AddParameter("@ChiSoBinhThuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(CSBT));
                cmd.AddParameter("@DonViTinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(DonViTinh));
                cmd.AddParameter("@BatThuong", SqlDbType.Bit, ConvertNullObjectToDBNull(BatThuong));
                cmd.AddParameter("@TrangThai", SqlDbType.TinyInt, ConvertNullObjectToDBNull(TrangThai));
                cmd.AddParameter("@NguoiXacNhan", SqlDbType.VarChar, ConvertNullObjectToDBNull(MaNV_XacNhan));
                cmd.AddParameter("@TGXacNhan", SqlDbType.DateTime, ConvertNullObjectToDBNull(ThoiGianXacNhan));
                cmd.AddParameter("@HIRepResourceCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(MaThietBi));
                cmd.AddParameter("@SampleCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(SampleCode));
                cn.Open();
                int count = cmd.ExecuteNonQuery();

                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }
        //▲====: #002

        public List<LIS_PCLRequest> LIS_GetPCLResult(string SoPhieuChiDinh)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_LIS_GetResult_API", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SoPhieuChiDinh", SqlDbType.VarChar, ConvertNullObjectToDBNull(SoPhieuChiDinh));
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<LIS_PCLRequest> mCategories = GetLIS_PCLRequestCollectionFromReader(mReader);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mCategories;
            }
        }
        
        public long EditRefMedicalServiceGroup(RefMedicalServiceGroups aRefMedicalServiceGroup)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spEditRefMedicalServiceGroup", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@EditRefMedicalServiceGroup", SqlDbType.Xml, ConvertNullObjectToDBNull(aRefMedicalServiceGroup.ConvertToXml()));
                cmd.AddParameter("@MedicalServiceGroupID", SqlDbType.BigInt, aRefMedicalServiceGroup.MedicalServiceGroupID, ParameterDirection.Output);
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                long nMedicalServiceGroupID = 0;
                if (count > 0 && cmd.Parameters["@MedicalServiceGroupID"].Value != DBNull.Value)
                {
                    nMedicalServiceGroupID = (long)cmd.Parameters["@MedicalServiceGroupID"].Value;
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return nMedicalServiceGroupID;
            }
        }
        public List<RefMedicalServiceGroups> GetRefMedicalServiceGroups(string MedicalServiceGroupCode)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRefMedicalServiceGroups", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@MedicalServiceGroupCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MedicalServiceGroupCode));
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<RefMedicalServiceGroups> mItemCollection = GetRefMedicalServiceGroupCollectionFromReader(mReader);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mItemCollection;
            }
        }
        public List<RefMedicalServiceGroupItem> GetRefMedicalServiceGroupItemsByID(long MedicalServiceGroupID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRefMedicalServiceGroupItemsByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@MedicalServiceGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedicalServiceGroupID));
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<RefMedicalServiceGroupItem> mItemCollection = GetRefMedicalServiceGroupItemCollectionFromReader(mReader);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mItemCollection;
            }
        }
        public IList<ShortHandDictionary> GetShortHandDictionariesByStaffID(long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetShortHandDictionaries", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<ShortHandDictionary> mItemCollection = GetShortHandDictionariesFromReader(mReader);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mItemCollection;
            }
        }
        public bool UpdateOrderNumberRegistration(long PtRegDetailID, long PCLReqItemID, long PrescriptID, long OrderNumber, long RoomId)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateOrderNumberRegistration", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                cmd.AddParameter("@PCLReqItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLReqItemID));
                cmd.AddParameter("@PrescriptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PrescriptID));
                cmd.AddParameter("@OrderNumber", SqlDbType.BigInt, ConvertNullObjectToDBNull(OrderNumber));
                cmd.AddParameter("@RoomID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RoomId));

                cn.Open();
                int count = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return count>0;
            }
        }
        public bool UpdateIsBlockBill(bool IsBlock, long PtRegistrationID, long DeptID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateIsBlockBill", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@IsBlock", SqlDbType.Bit, ConvertNullObjectToDBNull(IsBlock));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                cn.Open();
                int count = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }
        //▼====: #001
        #region PAC
        public List<Staff> PAC_GetListDoctors()
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_PAC_GetListDoctors", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<Staff> staffs = null;

                IDataReader reader = ExecuteReader(cmd);

                staffs = GetStaffFullNameCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return staffs;
            }
        }
        #endregion
        //▲====: #001
        public IList<DiseaseProgression> GetAllDiseaseProgression(bool UseInConfig)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDiseaseProgression", cn);
                cmd.AddParameter("@UseInConfig", SqlDbType.Bit, ConvertNullObjectToDBNull(UseInConfig));
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<DiseaseProgression> mItemCollection = GetDiseaseProgressionCollectionFromReader(mReader);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mItemCollection;
            }
        }
        public IList<DiseaseProgressionDetails> GetAllDiseaseProgressionDetails()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllDiseaseProgressionDetails", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                IDataReader mReader = ExecuteReader(cmd);
                List<DiseaseProgressionDetails> mItemCollection = GetDiseaseProgressionDetailsCollectionFromReader(mReader);
                mReader.Close();
                mReader.Dispose();

                CleanUpConnectionAndCommand(cn, cmd);
                return mItemCollection;
            }
        }
    }
}