using System.ComponentModel.Composition;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.CommonViews.SystemConfiguration.ViewModels
{
    [Export(typeof(ISystemConfigurationTopMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SystemConfigurationTopMenuViewModel : Conductor<object>, ISystemConfigurationTopMenu
    {
        #region Events
        [ImportingConstructor]
        public SystemConfigurationTopMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
        }
        public void PrinterSettingsCmd()
        {
            var mChildContent = Globals.GetViewModel<IPrinterSettings>();
            Globals.GetViewModel<IHome>().ActiveContent = mChildContent;
        }
        public void InstallationCmd()
        {
            var mChildContent = Globals.GetViewModel<IInstallationManagement>();
            Globals.GetViewModel<IHome>().ActiveContent = mChildContent;
        }
        public void SystemConfigCmd()
        {
            var mChildContent = Globals.GetViewModel<IGeneralSystemConfig>();
            Globals.GetViewModel<IHome>().ActiveContent = mChildContent;
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            else
            {

                bPrinterSettings = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mSystem_Management,
                                  (int)eSystem_Management.mPrinterSettings);
                bInstallOutOfBrowser = Globals.CheckFunction(Globals.listRefModule, (int)eModules.mSystem_Management,
                                  (int)eSystem_Management.mInstallOutOfBrowser);
                mSystem = bPrinterSettings || bInstallOutOfBrowser;
            }
        }
        private void RefApplicationConfigMgnt_In(object source)
        {
            var VM = Globals.GetViewModel<IRefApplicationConfig_ListFind>();
            Globals.GetViewModel<IHome>().ActiveContent = VM;
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
        #endregion
        #region Properties
        private bool _mSystem = true;
        private bool _bPrinterSettings = true;
        private bool _bInstallOutOfBrowser = true;
        private bool _IsUserAdmin = !Globals.isAccountCheck;
        public bool mSystem
        {
            get
            {
                return _mSystem;
            }
            set
            {
                if (_mSystem == value)
                    return;
                _mSystem = value;
                NotifyOfPropertyChange(() => mSystem);
            }
        }
        public bool bPrinterSettings
        {
            get
            {
                return _bPrinterSettings;
            }
            set
            {
                if (_bPrinterSettings == value)
                    return;
                _bPrinterSettings = value;
                NotifyOfPropertyChange(() => bPrinterSettings);
            }
        }
        public bool bInstallOutOfBrowser
        {
            get
            {
                return _bInstallOutOfBrowser;
            }
            set
            {
                if (_bInstallOutOfBrowser == value)
                    return;
                _bInstallOutOfBrowser = value;
                NotifyOfPropertyChange(() => bInstallOutOfBrowser);
            }
        }
        public bool IsUserAdmin
        {
            get
            {
                return _IsUserAdmin;
            }
            set
            {
                if (_IsUserAdmin == value)
                {
                    return;
                }
                _IsUserAdmin = value;
                NotifyOfPropertyChange(() => IsUserAdmin);
            }
        }
        #endregion
    }
}