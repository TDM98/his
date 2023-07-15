using eHCMSLanguage;
using System.Windows.Controls;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUserAccountTopMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserAccountTopMenuViewModel : Conductor<object>, IUserAccountTopMenu
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UserAccountTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
        }

        public void authorization()
        {
            //2022-05-16 Tạm thời kiểm tra quyền danh sách user trước khi check Admin
            bUserAccountStaffCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mUserAccount
                        , (int)eUserAccount.mPtUserList);
            if (!Globals.isAccountCheck)
            {
                return;
            }

            bUserAccountStaffInfoCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mUserAccount
                        , (int)eUserAccount.mPtListConfig);
            bUserAccountListCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mUserAccount
                        , (int)eUserAccount.mPtListConfig);
            bUserConfigCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mUserAccount
                        , (int)eUserAccount.mPtUserConfig) || !Globals.isAccountCheck;
            bLoginHistoryCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mUserAccount
                        , (int)eUserAccount.mPtHistoryLogin);
            mPtUserInfo = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mUserAccount
                        , (int)eUserAccount.mPtUserInfo);
            bStaffDeptResponCmd = false;
            bDoctorAuthoCmd = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mUserAccount
                        , (int)eUserAccount.mDoctorAutho);
        }

        #region bool properties
        private bool _bUserAccountListCmd = true;
        private bool _bUserConfigCmd = true;
        private bool _bLoginHistoryCmd = true;
        private bool _bUserAccountStaffCmd = true;
        private bool _bUserAccountStaffInfoCmd = true;
        private bool _mPtUserInfo = true;
        private bool _bStaffDeptResponCmd = true;

        public bool bUserAccountListCmd
        {
            get
            {
                return _bUserAccountListCmd;
            }
            set
            {
                if (_bUserAccountListCmd == value)
                    return;
                _bUserAccountListCmd = value;
            }
        }

        public bool bUserConfigCmd
        {
            get
            {
                return _bUserConfigCmd;
            }
            set
            {
                if (_bUserConfigCmd == value)
                    return;
                _bUserConfigCmd = value;
            }
        }

        public bool bLoginHistoryCmd
        {
            get
            {
                return _bLoginHistoryCmd;
            }
            set
            {
                if (_bLoginHistoryCmd == value)
                    return;
                _bLoginHistoryCmd = value;
            }
        }

        public bool bUserAccountStaffCmd
        {
            get
            {
                return _bUserAccountStaffCmd;
            }
            set
            {
                if (_bUserAccountStaffCmd == value)
                    return;
                _bUserAccountStaffCmd = value;
            }
        }

        public bool bUserAccountStaffInfoCmd
        {
            get
            {
                return _bUserAccountStaffInfoCmd;
            }
            set
            {
                if (_bUserAccountStaffInfoCmd == value)
                    return;
                _bUserAccountStaffInfoCmd = value;
            }
        }

        public bool mPtUserInfo
        {
            get
            {
                return _mPtUserInfo;
            }
            set
            {
                if (_mPtUserInfo == value)
                    return;
                _mPtUserInfo = value;
            }
        }

        public bool bStaffDeptResponCmd
        {
            get
            {
                return _bStaffDeptResponCmd;
            }
            set
            {
                if (_bStaffDeptResponCmd == value)
                    return;
                _bStaffDeptResponCmd = value;
            }
        }
        #endregion

        private void UserAccountStaffInfoCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IUserAccountManagementHome>();
            var GridControlVm = Globals.GetViewModel<IUserAccountInfo>();
            regModule.mainContent = GridControlVm;
            this.ActivateItem(GridControlVm);
        }

        public void UserAccountStaffInfoCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T0768_G1_TaiKhoanNguoiDung;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                UserAccountStaffInfoCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        UserAccountStaffInfoCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void UserAccountListCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;

            var regModule = Globals.GetViewModel<IUserAccountManagementHome>();
            var GridControlVm = Globals.GetViewModel<IGridControl>();
            regModule.mainContent = GridControlVm;
            this.ActivateItem(GridControlVm);
        }

        public void UserAccountListCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0460_G1_QuanLyDSPhanQuyen;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                UserAccountListCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        UserAccountListCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void UserConfigCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IUserAccountManagementHome>();
            var regVm = Globals.GetViewModel<IModulesTab>();
            regModule.mainContent = regVm;
            this.ActivateItem(regVm);
        }

        public void UserConfigCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.K1709_G1_CHinhPhQuyen;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                UserConfigCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        UserConfigCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void LoginHistoryCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IUserAccountManagementHome>();
            var regVm = Globals.GetViewModel<ILoginHistory>();
            regModule.mainContent = regVm;
            this.ActivateItem(regVm);
        }

        public void LoginHistoryCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.T2589_G1_LSuDNhap;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                LoginHistoryCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        LoginHistoryCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        private void UserAccountStaffCmd_In(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IUserAccountManagementHome>();
            var regVm = Globals.GetViewModel<IUserTab>();
            regModule.mainContent = regVm;
            this.ActivateItem(regVm);
        }

        public void UserAccountStaffCmd(object source)
        {
            Globals.TitleForm = eHCMSResources.Q0461_G1_QuanLyDSUser;
            if (string.IsNullOrEmpty(Globals.PageName))
            {
                UserAccountStaffCmd_In(source);
            }
            else if (Globals.PageName != Globals.TitleForm)
            {
                Coroutine.BeginExecute(GlobalsNAV.DoMessageBox(), null, (o, e) =>
                {
                    if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                    {
                        UserAccountStaffCmd_In(source);
                        GlobalsNAV.msgb = null;
                    }
                });
            }
        }

        public void StaffDeptResponCmd(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IUserAccountManagementHome>();
            var regVm = Globals.GetViewModel<IStaffDeptResponsibility>();
            regModule.mainContent = regVm;
            this.ActivateItem(regVm);
        }

        public void DoctorAuthoCmd(object source)
        {
            Globals.PageName = Globals.TitleForm;
            var regModule = Globals.GetViewModel<IUserAccountManagementHome>();
            var regVm = Globals.GetViewModel<IDoctorAutho>();
            regModule.mainContent = regVm;
            this.ActivateItem(regVm);
        }

        public void ManagementUserOfficialCmd(object source)
        {
            Globals.PageName = "Quản lý User official";
            var regModule = Globals.GetViewModel<IUserAccountManagementHome>();
            var regVm = Globals.GetViewModel<IManagementUserOfficial>();
            regModule.mainContent = regVm;
            ActivateItem(regVm);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            Globals.EventAggregator.Subscribe(this);
        }

        private bool _bDoctorAuthoCmd = true;
        public bool bDoctorAuthoCmd
        {
            get
            {
                return _bDoctorAuthoCmd;
            }
            set
            {
                if (_bDoctorAuthoCmd == value)
                    return;
                _bDoctorAuthoCmd = value;
            }
        }
    }
}
