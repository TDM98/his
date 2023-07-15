using DataEntities;
using eHCMS.Configurations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace eHCMS.DAL
{
    public abstract class CommonProvider_V2 : DataProviderBase
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
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Common.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Common.Common.ProviderType);
                    _instance = (CommonProvider_V2)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }
        public CommonProvider_V2()
        {
            this.ConnectionString = Globals.Settings.Common.ConnectionString;
        }
        #region LabSoftAPI
        public abstract List<object> LIS_GetCategories(eLabSoftCategory eLabSoftCategory);
        public abstract List<LIS_PCLRequest> LIS_PatientList(string TuNgay, string DenNgay, int TrangThai);
        public abstract List<LIS_PCLRequest> LIS_Order(string SoPhieuChiDinh, int TrangThai);
        public abstract bool LIS_UpdateOrderStatus(string SoPhieuChiDinh, string MaDichVu, int TrangThai, string NgayTiepNhan, out string TrangThaiCapNhat);
        public abstract bool LIS_Result(string SoPhieuChiDinh, string MaDichVu, string KetQua, string CSBT, string DonViTinh, bool BatThuong, int TrangThai, string MaNV_XacNhan, string ThoiGianXacNhan, string MaThietBi);
        public abstract List<LIS_PCLRequest> LIS_GetPCLResult(string SoPhieuChiDinh);
        #endregion
        public abstract long EditRefMedicalServiceGroup(RefMedicalServiceGroups aRefMedicalServiceGroup);
        public abstract List<RefMedicalServiceGroups> GetRefMedicalServiceGroups(string MedicalServiceGroupCode);
        public abstract List<RefMedicalServiceGroupItem> GetRefMedicalServiceGroupItemsByID(long MedicalServiceGroupID);
        public abstract IList<ShortHandDictionary> GetShortHandDictionariesByStaffID(long StaffID);
    }
}