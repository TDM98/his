using DataEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace eHCMS.DAL
{
    public class SqlCommonProvider : CommonProvider_V2
    {
        public SqlCommonProvider() : base() {}
        #region LabSoftAPI
        public override List<object> LIS_GetCategories(eLabSoftCategory eLabSoftCategory)
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
                return mCategories;
            }
        }
        public override List<LIS_PCLRequest> LIS_PatientList(string TuNgay, string DenNgay, int TrangThai)
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
                return mCategories;
            }
        }
        public override List<LIS_PCLRequest> LIS_Order(string SoPhieuChiDinh, int TrangThai)
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
                return mCategories;
            }
        }
        public override bool LIS_UpdateOrderStatus(string SoPhieuChiDinh, string MaDichVu, int TrangThai, string NgayTiepNhan, out string TrangThaiCapNhat)
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
                return count > 0;
            }
        }
        public override bool LIS_Result(string SoPhieuChiDinh, string MaDichVu, string KetQua, string CSBT, string DonViTinh, bool BatThuong, int TrangThai, string MaNV_XacNhan, string ThoiGianXacNhan, string MaThietBi)
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
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                return count > 0;
            }
        }
        public override List<LIS_PCLRequest> LIS_GetPCLResult(string SoPhieuChiDinh)
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
                return mCategories;
            }
        }
        #endregion
        public override long EditRefMedicalServiceGroup(RefMedicalServiceGroups aRefMedicalServiceGroup)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spEditRefMedicalServiceGroup", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@EditRefMedicalServiceGroup", SqlDbType.Xml, ConvertNullObjectToDBNull(aRefMedicalServiceGroup.ConvertToXml()));
                cmd.AddParameter("@MedicalServiceGroupID", SqlDbType.BigInt, aRefMedicalServiceGroup.MedicalServiceGroupID, ParameterDirection.Output);
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                if (count > 0 && cmd.Parameters["@MedicalServiceGroupID"].Value != DBNull.Value)
                {
                    return (long)cmd.Parameters["@MedicalServiceGroupID"].Value;
                }
                else return 0;
            }
        }
        public override List<RefMedicalServiceGroups> GetRefMedicalServiceGroups(string MedicalServiceGroupCode)
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
                return mItemCollection;
            }
        }
        public override List<RefMedicalServiceGroupItem> GetRefMedicalServiceGroupItemsByID(long MedicalServiceGroupID)
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
                return mItemCollection;
            }
        }
        public override IList<ShortHandDictionary> GetShortHandDictionariesByStaffID(long StaffID)
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
                return mItemCollection;
            }
        }
    }
}