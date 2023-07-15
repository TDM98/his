using eHCMSLanguage;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
/*
 * 20221224 #001 BaoLQ: Thêm menu cấu hình  RefApplicationConfigs
 * 20230531 #002 QTD:   Thêm menu quản lý các danh mục bệnh viện
 */
namespace aEMR.Configuration.ConfigurationModule.ViewModels
{
    [Export(typeof(IConfigurationTopMenu))]
    public class ConfigurationTopMenuViewModel : Conductor<object>, IConfigurationTopMenu
    {
        [ImportingConstructor]
        public ConfigurationTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.PageName = "";
            Globals.TitleForm = "";
        }

        public void authorization()
        {
            bRefApplicationConfigMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mAdmin
                                                      , (int)eAdmin.mQuanLyCauHinh);
            if (!Globals.isAccountCheck)
            {
                return;
            }

            bRefDepartments_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mKhoa_VanPhong_Kho);

            bRefMedicalServiceItems_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa);


            bRefMedicalServiceTypes_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucLoaiDichVu);

            bLocations_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucPhong);

            bRoomType_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucLoaiPhong);

            bRefMedicalServiceItems_IsPCL_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mQLGoiDichVuCLSCuaKhoa);

            bDeptLocMedServices_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mPhanBoTatCaDichVu_PhongCuaKhoa);

            bPCLForms_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucPCLForm);

            bPCLSections_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucPCLSessions);

            bPCLExamType_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucPCLExamTypes);

            bPCLGroups_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucPCLGroup);

            bBedAlloc_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mQuanLyGiuongBenh);


            //bPCLExamTypeMedServiceDefItems_Mgnt = Globals.listRefModule[(int)eModules.mConfiguration_Management]
            //    .lstFunction[(int)eConfiguration_Management.mPtDashboardCommonRecs].mFunction != null;
            bPCLItems_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mCauHinhPCLExamTypes_Sessions);

            bMedServiceItemPriceList_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mBangGiaDichVu);

            bPCLExamTypePriceList_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mBangGiaPCLExamType);
            //2021-02-08 BAOLQ Tạo phân quyền bổ sung các menu chưa có phân quyền
           
            bICD_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucICD);
            bQLBH_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucQLBH);
            bBenhVien_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucBenhVien);
            bTinhThanh_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucTinh);
            bNgheNghiep_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDanhMucNgheNghiep);
            bEncrypt_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mEncrypt);
            bPhanBoXNVaoPhong_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mPhanBoXNVaoPhong);
            bThuTuHienThiKQXN_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mThuTuHienThiKQXN);
            bMauLoiDan_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mMauLoiDan);
            bCauHinhCLSSo_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mCauHinhCLSSo);
            bCauHinhCLSNgoaiVien_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mCauHinhCLSNgoaiVien);
            bCauHinhInXN_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mCauHinhInXN);
            bAdmissionCriteria_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mAdmissionCriteria);

            //2022-05-25 QTD: Bổ sung quyền
            bPCLExamTypeCombo_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mPCLExamTypeCombo_Mgnt);
            bDiseaseProgression_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mDiseaseProgression_Mgnt);
            bConsultingDiagnosysEditAuth = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mConsultingDiagnosysEditAuth);
            bSymptomCategory_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mSymptomCategory_Mgnt);
            bBedCategory_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mBedCategory_Mgnt);
            bPCLExamTypeExamTestPrint_NewMgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mPCLExamTypeExamTestPrint_NewMgnt);
            bRefDepartmentReqCashAdv_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mRefDepartmentReqCashAdv_Mgnt);
            bExemptions_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mExemptions_Mgnt);
            bOutPatientTreatmentType = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                      , (int)eConfiguration_Management.mOutPatientTreatmentType_Mgnt);

            //▼====: #002
            bLookupList_Mgnt = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                         , (int)eConfiguration_Management.mLookupList_Mgnt);
            //▲====: #002

            bDM_BYT_Mgnt = bICD_Mgnt || bQLBH_Mgnt || bBenhVien_Mgnt|| bNgheNghiep_Mgnt || bLookupList_Mgnt;
            bDM_BV_Mgnt = bTinhThanh_Mgnt || bRefMedicalServiceTypes_Mgnt || bLocations_Mgnt || bRoomType_Mgnt || bPCLExamType_Mgnt || bPCLExamType_Mgnt || bPCLSections_Mgnt 
                || bPCLExamTypeCombo_Mgnt || bAdmissionCriteria_Mgnt || bDiseaseProgression_Mgnt || bConsultingDiagnosysEditAuth || bSymptomCategory_Mgnt || bOutPatientTreatmentType;
            bDanhMuc_Mgnt = bDM_BYT_Mgnt || bDM_BV_Mgnt;

            bConfig = bRefDepartments_Mgnt || bRefMedicalServiceItems_Mgnt || bDeptLocMedServices_Mgnt || bPCLForms_Mgnt || bRefMedicalServiceItems_IsPCL_Mgnt
                || bPhanBoXNVaoPhong_Mgnt || bCauHinhInXN_Mgnt || bThuTuHienThiKQXN_Mgnt || bBedAlloc_Mgnt || bEncrypt_Mgnt || bMauLoiDan_Mgnt || bCauHinhCLSSo_Mgnt
                //▼====: #001
                || bCauHinhCLSNgoaiVien_Mgnt || bBedCategory_Mgnt || bPCLExamTypeExamTestPrint_NewMgnt || bRefApplicationConfigMgnt;
                //▲====: #001
            bPriceList = bMedServiceItemPriceList_Mgnt || bPCLExamTypePriceList_Mgnt || bRefDepartmentReqCashAdv_Mgnt || bExemptions_Mgnt;
        }

        #region bool checking
        private bool _bRefDepartments_Mgnt = true;
        private bool _bRoomType_Mgnt = true;
        private bool _bRefMedicalServiceItems_Mgnt = true;
        private bool _bRefMedicalServiceItems_IsPCL_Mgnt = true;
        private bool _bPCLExamType_Mgnt = true;
        private bool _bPCLExamTypeMedServiceDefItems_Mgnt = true;
        private bool _bPCLItems_Mgnt = true;
        private bool _bRefMedicalServiceTypes_Mgnt = true;
        private bool _bLocations_Mgnt = true;

        private bool _bDeptLocMedServices_Mgnt = true;
        private bool _bBedAlloc_Mgnt = true;
        private bool _bPCLForms_Mgnt = true;
        private bool _bPCLSections_Mgnt = true;
        private bool _bPCLGroups_Mgnt = true;
        private bool _bMedServiceItemPriceList_Mgnt = true;
        private bool _bPCLExamTypePriceList_Mgnt = true;
        private bool _bICD_Mgnt = true;
        private bool _bQLBH_Mgnt = true;
        private bool _bBenhVien_Mgnt = true;
        private bool _bTinhThanh_Mgnt = true;
        private bool _bNgheNghiep_Mgnt = true;
        private bool _bEncrypt_Mgnt = true;
        private bool _bPhanBoXNVaoPhong_Mgnt = true;
        private bool _bThuTuHienThiKQXN_Mgnt = true;
        private bool _bMauLoiDan_Mgnt = true;
        private bool _bCauHinhCLSSo_Mgnt = true;
        private bool _bCauHinhCLSNgoaiVien_Mgnt = true;
        private bool _bCauHinhInXN_Mgnt = true;
        private bool _bDanhMuc_Mgnt = true;
        private bool _bDM_BYT_Mgnt = true;
        private bool _bDM_BV_Mgnt = true;
        private bool _bAdmissionCriteria_Mgnt = true;
        public bool bRefDepartments_Mgnt
        {
            get
            {
                return _bRefDepartments_Mgnt;
            }
            set
            {
                if (_bRefDepartments_Mgnt == value)
                    return;
                _bRefDepartments_Mgnt = value;
            }
        }
        public bool bRoomType_Mgnt
        {
            get
            {
                return _bRoomType_Mgnt;
            }
            set
            {
                if (_bRoomType_Mgnt == value)
                    return;
                _bRoomType_Mgnt = value;
            }
        }
        public bool bRefMedicalServiceItems_Mgnt
        {
            get
            {
                return _bRefMedicalServiceItems_Mgnt;
            }
            set
            {
                if (_bRefMedicalServiceItems_Mgnt == value)
                    return;
                _bRefMedicalServiceItems_Mgnt = value;
            }
        }
        public bool bRefMedicalServiceItems_IsPCL_Mgnt
        {
            get
            {
                return _bRefMedicalServiceItems_IsPCL_Mgnt;
            }
            set
            {
                if (_bRefMedicalServiceItems_IsPCL_Mgnt == value)
                    return;
                _bRefMedicalServiceItems_IsPCL_Mgnt = value;
            }
        }
        public bool bPCLExamType_Mgnt
        {
            get
            {
                return _bPCLExamType_Mgnt;
            }
            set
            {
                if (_bPCLExamType_Mgnt == value)
                    return;
                _bPCLExamType_Mgnt = value;
            }
        }
        public bool bPCLExamTypeMedServiceDefItems_Mgnt
        {
            get
            {
                return _bPCLExamTypeMedServiceDefItems_Mgnt;
            }
            set
            {
                if (_bPCLExamTypeMedServiceDefItems_Mgnt == value)
                    return;
                _bPCLExamTypeMedServiceDefItems_Mgnt = value;
            }
        }
        public bool bPCLItems_Mgnt
        {
            get
            {
                return _bPCLItems_Mgnt;
            }
            set
            {
                if (_bPCLItems_Mgnt == value)
                    return;
                _bPCLItems_Mgnt = value;
            }
        }

        public bool bRefMedicalServiceTypes_Mgnt
        {
            get
            {
                return _bRefMedicalServiceTypes_Mgnt;
            }
            set
            {
                if (_bRefMedicalServiceTypes_Mgnt == value)
                    return;
                _bRefMedicalServiceTypes_Mgnt = value;
            }
        }
        public bool bLocations_Mgnt
        {
            get
            {
                return _bLocations_Mgnt;
            }
            set
            {
                if (_bLocations_Mgnt == value)
                    return;
                _bLocations_Mgnt = value;
            }
        }
        public bool bDeptLocMedServices_Mgnt
        {
            get
            {
                return _bDeptLocMedServices_Mgnt;
            }
            set
            {
                if (_bDeptLocMedServices_Mgnt == value)
                    return;
                _bDeptLocMedServices_Mgnt = value;
            }
        }
        public bool bBedAlloc_Mgnt
        {
            get
            {
                return _bBedAlloc_Mgnt;
            }
            set
            {
                if (_bBedAlloc_Mgnt == value)
                    return;
                _bBedAlloc_Mgnt = value;
            }
        }
        public bool bPCLForms_Mgnt
        {
            get
            {
                return _bPCLForms_Mgnt;
            }
            set
            {
                if (_bPCLForms_Mgnt == value)
                    return;
                _bPCLForms_Mgnt = value;
            }
        }
        public bool bPCLSections_Mgnt
        {
            get
            {
                return _bPCLSections_Mgnt;
            }
            set
            {
                if (_bPCLSections_Mgnt == value)
                    return;
                _bPCLSections_Mgnt = value;
            }
        }

        public bool bPCLGroups_Mgnt
        {
            get
            {
                return _bPCLGroups_Mgnt;
            }
            set
            {
                if (_bPCLGroups_Mgnt == value)
                    return;
                _bPCLGroups_Mgnt = value;
            }
        }
        public bool bMedServiceItemPriceList_Mgnt
        {
            get
            {
                return _bMedServiceItemPriceList_Mgnt;
            }
            set
            {
                if (_bMedServiceItemPriceList_Mgnt == value)
                    return;
                _bMedServiceItemPriceList_Mgnt = value;
            }
        }
        public bool bPCLExamTypePriceList_Mgnt
        {
            get
            {
                return _bPCLExamTypePriceList_Mgnt;
            }
            set
            {
                if (_bPCLExamTypePriceList_Mgnt == value)
                    return;
                _bPCLExamTypePriceList_Mgnt = value;
            }
        }
        public bool bICD_Mgnt
        {
            get
            {
                return _bICD_Mgnt;
            }
            set
            {
                if (_bICD_Mgnt == value)
                    return;
                _bICD_Mgnt = value;
            }
        }
        public bool bQLBH_Mgnt
        {
            get
            {
                return _bQLBH_Mgnt;
            }
            set
            {
                if (_bQLBH_Mgnt == value)
                    return;
                _bQLBH_Mgnt = value;
            }
        }
        public bool bBenhVien_Mgnt
        {
            get
            {
                return _bBenhVien_Mgnt;
            }
            set
            {
                if (_bBenhVien_Mgnt == value)
                    return;
                _bBenhVien_Mgnt = value;
            }
        }
        public bool bTinhThanh_Mgnt
        {
            get
            {
                return _bTinhThanh_Mgnt;
            }
            set
            {
                if (_bTinhThanh_Mgnt == value)
                    return;
                _bTinhThanh_Mgnt = value;
            }
        }
        public bool bNgheNghiep_Mgnt
        {
            get
            {
                return _bNgheNghiep_Mgnt;
            }
            set
            {
                if (_bNgheNghiep_Mgnt == value)
                    return;
                _bNgheNghiep_Mgnt = value;
            }
        }
        public bool bEncrypt_Mgnt
        {
            get
            {
                return _bEncrypt_Mgnt;
            }
            set
            {
                if (_bEncrypt_Mgnt == value)
                    return;
                _bEncrypt_Mgnt = value;
            }
        }
        public bool bPhanBoXNVaoPhong_Mgnt
        {
            get
            {
                return _bPhanBoXNVaoPhong_Mgnt;
            }
            set
            {
                if (_bPhanBoXNVaoPhong_Mgnt == value)
                    return;
                _bPhanBoXNVaoPhong_Mgnt = value;
            }
        }
        public bool bThuTuHienThiKQXN_Mgnt
        {
            get
            {
                return _bThuTuHienThiKQXN_Mgnt;
            }
            set
            {
                if (_bThuTuHienThiKQXN_Mgnt == value)
                    return;
                _bThuTuHienThiKQXN_Mgnt = value;
            }
        }
        public bool bMauLoiDan_Mgnt
        {
            get
            {
                return _bMauLoiDan_Mgnt;
            }
            set
            {
                if (_bMauLoiDan_Mgnt == value)
                    return;
                _bMauLoiDan_Mgnt = value;
            }
        }
        public bool bCauHinhCLSSo_Mgnt
        {
            get
            {
                return _bCauHinhCLSSo_Mgnt;
            }
            set
            {
                if (_bCauHinhCLSSo_Mgnt == value)
                    return;
                _bCauHinhCLSSo_Mgnt = value;
            }
        }
        public bool bCauHinhCLSNgoaiVien_Mgnt
        {
            get
            {
                return _bCauHinhCLSNgoaiVien_Mgnt;
            }
            set
            {
                if (_bCauHinhCLSNgoaiVien_Mgnt == value)
                    return;
                _bCauHinhCLSNgoaiVien_Mgnt = value;
            }
        }
        public bool bCauHinhInXN_Mgnt
        {
            get
            {
                return _bCauHinhInXN_Mgnt;
            }
            set
            {
                if (_bCauHinhInXN_Mgnt == value)
                    return;
                _bCauHinhInXN_Mgnt = value;
            }
        }
        public bool bDanhMuc_Mgnt
        {
            get
            {
                return _bDanhMuc_Mgnt;
            }
            set
            {
                if (_bDanhMuc_Mgnt == value)
                    return;
                _bDanhMuc_Mgnt = value;
            }
        }
        public bool bDM_BYT_Mgnt
        {
            get
            {
                return _bDM_BYT_Mgnt;
            }
            set
            {
                if (_bDM_BYT_Mgnt == value)
                    return;
                _bDM_BYT_Mgnt = value;
            }
        }
        public bool bDM_BV_Mgnt
        {
            get
            {
                return _bDM_BV_Mgnt;
            }
            set
            {
                if (_bDM_BV_Mgnt == value)
                    return;
                _bDM_BV_Mgnt = value;
            }
        }
        public bool bAdmissionCriteria_Mgnt
        {
            get
            {
                return _bAdmissionCriteria_Mgnt;
            }
            set
            {
                if (_bAdmissionCriteria_Mgnt == value)
                    return;
                _bAdmissionCriteria_Mgnt = value;
            }
        }
        #endregion
        private void Locations_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<ILocations_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void Locations_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K2904_G1_DMucPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Locations_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Locations_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefDepartments_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IRefDepartments>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void RefDepartments_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1798_G1_QLyKhoaPhgKho;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefDepartments_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefDepartments_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RoomType_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IRoomType>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void RoomType_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K2899_G1_DMucLoaiPg.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RoomType_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RoomType_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void RefMedicalServiceItems_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            //var VM = Globals.GetViewModel<IRefMedicalServiceItems>();
            var VM = Globals.GetViewModel<IDeptRefMedicalServiceItemsContent>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void RefMedicalServiceItems_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1799_G1_QLyDVKhoa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefMedicalServiceItems_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefMedicalServiceItems_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void RefMedicalServiceItems_IsPCL_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IRefMedicalServiceItems_IsPCL>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void RefMedicalServiceItems_IsPCL_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0465_G1_QuanLyDVCLS.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefMedicalServiceItems_IsPCL_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefMedicalServiceItems_IsPCL_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLExamType_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypes>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamType_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1800_G1_QLyPCLExamtype;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLExamType_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamType_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLExamTypeCombo_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeCombo>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTypeCombo_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1801_G1_QLyBoXN;
            PCLExamTypeCombo_Mgnt_In(source);
        }

        public void PCLExamTypeMedServiceDefItems_Mgnt(object source)
        {
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeMedServiceDefItems>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLItems_Mgnt(object source)
        {
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLItems>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        private void RefMedicalServiceTypes_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IRefMedicalServiceTypes>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void RefMedicalServiceTypes_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K2897_G1_DMucLoaiDV.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefMedicalServiceTypes_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefMedicalServiceTypes_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void DeptLocMedServices_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IDeptLocMedServices>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void DeptLocMedServices_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.P0337_G1_PhanBoDVVaoKP;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DeptLocMedServices_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DeptLocMedServices_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void BedAlloc_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var PtBedAllocationsVM = Globals.GetViewModel<IPtBedAllocations>();

            ConfigurationModule.MainContent = PtBedAllocationsVM;
            (ConfigurationModule as Conductor<object>).ActivateItem(PtBedAllocationsVM);
        }
        public void BedAlloc_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0467_G1_QuanLyGiuongBenh;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BedAlloc_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BedAlloc_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLForms_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLForms>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLForms_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1802_G1_QLyForms;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLForms_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLForms_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLSections_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLSections>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLSections_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K2903_G1_DMucPCLsections;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLSections_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLSections_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void PCLGroups_Mgnt(object source)
        {
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLGroups>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        private void MedServiceItemPriceList_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IMedServiceItemPriceList>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void MedServiceItemPriceList_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K1028_G1_BGiaDV;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                MedServiceItemPriceList_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        MedServiceItemPriceList_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLExamTypePriceList_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypePriceList>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTypePriceList_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K1032_G1_BGiaPCLExamtype;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLExamTypePriceList_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTypePriceList_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Encrypt_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IEncrypt>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void Encrypt_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.T0045_G1_EncryptConfig;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Encrypt_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Encrypt_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NoteTemplate_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IfrmPrescriptionNoteTempType>();
            VM.isPopup = false;
            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void NoteTemplate_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.T3708_G1_MauLoiDan;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NoteTemplate_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NoteTemplate_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLExamTypeLocationsCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeLocations>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTypeLocationsCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1803_G1_PBoPCLExamtypeVaoPhgCuaKhoa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLExamTypeLocationsCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTypeLocationsCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLExamTypeExamTestPrintMgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeExamTestPrint>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTypeExamTestPrintMgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1804_G1_CauHinhInPCLExamtypeTest;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLExamTypeExamTestPrintMgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTypeExamTestPrintMgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void PCLExamTypeExamTestPrintIndexMgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeExamTestPrintIndex>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTypeExamTestPrintIndexMgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z1805_G1_CauHinhThuTuHienThi;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLExamTypeExamTestPrintIndexMgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTypeExamTestPrintIndexMgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }


        private void PatientApptLocTargetsClick_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPatientApptLocTargets>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PatientApptLocTargetsClick(object source)
        {
            Globals.TitleForm = eHCMSResources.K1700_G1_CHinhHenChoPgCa;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PatientApptLocTargetsClick_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PatientApptLocTargetsClick_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void PCLExamTypeServiceTarget_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K1698_G1_CHinhCLSSo;
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeServiceTarget>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLAgency_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K1697_G1_CHinhCLSNgVien;

            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IHospitalAgencyContent>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        //RefDepartmentReqCashAdv_Mgnt

        public void RefDepartmentReqCashAdv_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.T1143_G1_GiaTUTungKhoa;


            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IRefDepartmentReqCashAdv>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        private void ICD_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IICD_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void ICD_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3101_G1_DM_ICD.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                ICD_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        ICD_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void QLBH_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IInsuranceBenefitCategories_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void QLBH_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3102_G1_DM_QLBH.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                QLBH_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        QLBH_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BenhVien_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IHospital_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void BenhVien_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3103_G1_DM_BenhVien.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BenhVien_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BenhVien_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void TinhThanh_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<ICitiesProvinces_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void TinhThanh_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3104_G1_DM_TinhThanh.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                TinhThanh_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        TinhThanh_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void NgheNghiep_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IJob_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void NgheNghiep_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3105_G1_DM_NgheNghiep.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                NgheNghiep_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        NgheNghiep_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void BedCategory_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IBedCategory_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void BedCategory_Mgnt(object source)
        {
            Globals.TitleForm = "Quản lý giường bệnh".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                BedCategory_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        BedCategory_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void AdmissionCriteria_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IAdmissionCriteria_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void AdmissionCriteria_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3119_G1_TieuChiVaoVien.ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                AdmissionCriteria_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        AdmissionCriteria_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void PCLExamTypeExamTestPrint_NewMgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeExamTestPrint_New>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLExamTypeExamTestPrint_NewMgnt(object source)
        {
            Globals.TitleForm = "Cấu hình in CLS";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                PCLExamTypeExamTestPrint_NewMgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        PCLExamTypeExamTestPrint_NewMgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▼====: #001
        private void RefApplicationConfigMgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IRefApplicationConfig_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void RefApplicationConfigMgnt(object source)
        {
            Globals.TitleForm = "Quản lý cấu hình";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                RefApplicationConfigMgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        RefApplicationConfigMgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #001

        private void Exemptions_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IExemptions_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void Exemptions_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.Z3230_G1_DMienGiam;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Exemptions_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Exemptions_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void DiseaseProgression_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IDiseaseProgression_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void DiseaseProgression_Mgnt(object source)
        {
            Globals.TitleForm = "Danh mục diễn tiến bệnh";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                DiseaseProgression_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        DiseaseProgression_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void Package_DVKT_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPackageTechnicalServices_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void Package_DVKT(object source)
        {
            Globals.TitleForm = "Gói DVKT";
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                Package_DVKT_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        Package_DVKT_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void SymptomCategory_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IAdmissionCriterion_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void SymptomCategory_Mgnt(object source)
        {
            Globals.TitleForm = "Quản lý tiêu chuẩn nhập viện".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                SymptomCategory_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        SymptomCategory_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        private void OutPatientTreatmentType_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IOutPatientTreatmentTypeListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void OutPatientTreatmentType_Mgnt(object source)
        {
            Globals.TitleForm = "Danh mục nhóm bệnh".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                OutPatientTreatmentType_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        OutPatientTreatmentType_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        #region menu
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            Globals.EventAggregator.Subscribe(this);
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
        }

        ILeftMenuView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as ILeftMenuView;
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
        }

        #endregion

        // 2022-05-25 QTD: Bổ sung phân quyền
        private bool _bPCLExamTypeCombo_Mgnt = true;
        private bool _bDiseaseProgression_Mgnt = true;
        private bool _bConsultingDiagnosysEditAuth = true;
        private bool _bSymptomCategory_Mgnt = true;
        private bool _bBedCategory_Mgnt = true;
        private bool _bPCLExamTypeExamTestPrint_NewMgnt = true;
        private bool _bRefDepartmentReqCashAdv_Mgnt = true;
        private bool _bExemptions_Mgnt = true;
        private bool _bConfig = true;
        private bool _bPriceList = true;
        public bool bPCLExamTypeCombo_Mgnt
        {
            get
            {
                return _bPCLExamTypeCombo_Mgnt;
            }
            set
            {
                if (_bPCLExamTypeCombo_Mgnt == value)
                    return;
                _bPCLExamTypeCombo_Mgnt = value;
            }
        }
        public bool bDiseaseProgression_Mgnt
        {
            get
            {
                return _bDiseaseProgression_Mgnt;
            }
            set
            {
                if (_bDiseaseProgression_Mgnt == value)
                    return;
                _bDiseaseProgression_Mgnt = value;
            }
        }
        public bool bConsultingDiagnosysEditAuth
        {
            get
            {
                return _bConsultingDiagnosysEditAuth;
            }
            set
            {
                if (_bConsultingDiagnosysEditAuth == value)
                    return;
                _bConsultingDiagnosysEditAuth = value;
            }
        }
        public bool bSymptomCategory_Mgnt
        {
            get
            {
                return _bSymptomCategory_Mgnt;
            }
            set
            {
                if (_bSymptomCategory_Mgnt == value)
                    return;
                _bSymptomCategory_Mgnt = value;
            }
        }
        public bool bBedCategory_Mgnt
        {
            get
            {
                return _bBedCategory_Mgnt;
            }
            set
            {
                if (_bBedCategory_Mgnt == value)
                    return;
                _bBedCategory_Mgnt = value;
            }
        }
        public bool bPCLExamTypeExamTestPrint_NewMgnt
        {
            get
            {
                return _bPCLExamTypeExamTestPrint_NewMgnt;
            }
            set
            {
                if (_bPCLExamTypeExamTestPrint_NewMgnt == value)
                    return;
                _bPCLExamTypeExamTestPrint_NewMgnt = value;
            }
        }
        public bool bRefDepartmentReqCashAdv_Mgnt
        {
            get
            {
                return _bRefDepartmentReqCashAdv_Mgnt;
            }
            set
            {
                if (_bRefDepartmentReqCashAdv_Mgnt == value)
                    return;
                _bRefDepartmentReqCashAdv_Mgnt = value;
            }
        }
        public bool bExemptions_Mgnt
        {
            get
            {
                return _bExemptions_Mgnt;
            }
            set
            {
                if (_bExemptions_Mgnt == value)
                    return;
                _bExemptions_Mgnt = value;
            }
        }
        public bool bConfig
        {
            get
            {
                return _bConfig;
            }
            set
            {
                if (_bConfig == value)
                    return;
                _bConfig = value;
            }
        }
        public bool bPriceList
        {
            get
            {
                return _bPriceList;
            }
            set
            {
                if (_bPriceList == value)
                    return;
                _bPriceList = value;
            }
        }

        private bool _bOutPatientTreatmentType = true;
        public bool bOutPatientTreatmentType
        {
            get
            {
                return _bOutPatientTreatmentType;
            }
            set
            {
                if (_bOutPatientTreatmentType == value)
                    return;
                _bOutPatientTreatmentType = value;
            }
        }
        //▼====: #001
        private bool _bRefApplicationConfigMgnt = false;
        public bool bRefApplicationConfigMgnt
        {
            get
            {
                return _bRefApplicationConfigMgnt;
            }
            set
            {
                if (_bRefApplicationConfigMgnt == value)
                    return;
                _bRefApplicationConfigMgnt = value;
            }
        }
        //▲====: #001

        //▼====: #002
        private bool _bLookupList_Mgnt = true;
        public bool bLookupList_Mgnt
        {
            get
            {
                return _bLookupList_Mgnt;
            }
            set
            {
                if (_bLookupList_Mgnt == value)
                    return;
                _bLookupList_Mgnt = value;
            }
        }

        private void LookupList_Mgnt_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<ILookup_ListFind>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);

        }

        public void LookupList_Mgnt(object source)
        {
            Globals.TitleForm = "Quản lý danh mục chung".ToUpper();
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                LookupList_Mgnt_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        LookupList_Mgnt_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }
        //▲====: #002
    }
}
