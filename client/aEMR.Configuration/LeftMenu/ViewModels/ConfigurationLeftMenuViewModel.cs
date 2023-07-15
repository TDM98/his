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

namespace aEMR.Configuration.LeftMenu.ViewModels
{
    [Export(typeof(IConfigurationLeftMenu))]
    public class ConfigurationLeftMenuViewModel : Conductor<object>, IConfigurationLeftMenu
    {
        [ImportingConstructor]
        public ConfigurationLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.PageName = "";
            Globals.TitleForm = "";
        }

        public void authorization()
        {
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

        #endregion
        private void Locations_Mgnt_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeMedServiceDefItems>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLItems_Mgnt(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLItems>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        private void RefMedicalServiceTypes_Mgnt_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLGroups>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        private void MedServiceItemPriceList_Mgnt_In(object source)
        {
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IPCLExamTypeServiceTarget>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
        }

        public void PCLAgency_Mgnt(object source)
        {
            Globals.TitleForm = eHCMSResources.K1697_G1_CHinhCLSNgVien;
            SetHyperlinkSelectedStyle(source as Button);
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
            SetHyperlinkSelectedStyle(source as Button);
            Globals.PageName = Globals.TitleForm;
            var ConfigurationModule = Globals.GetViewModel<IConfigurationModule>();
            var VM = Globals.GetViewModel<IRefDepartmentReqCashAdv>();

            ConfigurationModule.MainContent = VM;
            (ConfigurationModule as Conductor<object>).ActivateItem(VM);
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
        private void SetHyperlinkSelectedStyle(Button lnk)
        {
            if (_currentView != null)
            {
                _currentView.ResetMenuColor();
            }
            if (lnk != null)
            {
                lnk.Style = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.Selected"];
            }
        }
        #endregion
    }
}
