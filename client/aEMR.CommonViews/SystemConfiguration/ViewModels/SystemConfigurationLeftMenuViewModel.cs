using System.ComponentModel.Composition;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.SystemConfiguration.ViewModels
{
    [Export(typeof(ISystemConfigurationLeftMenu)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SystemConfigurationLeftMenuViewModel : Conductor<object>, ISystemConfigurationLeftMenu
    {
        [ImportingConstructor]
        public SystemConfigurationLeftMenuViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            Globals.PageName = "";
            Globals.TitleForm = "";
            authorization();
        }
        public void PrinterSettingsCmd()
        {
            var content = Globals.GetViewModel<IPrinterSettings>();
            Globals.GetViewModel<IHome>().ActiveContent = content;
        }
        public void InstallationCmd()
        {
            var content = Globals.GetViewModel<IInstallationManagement>();
            Globals.GetViewModel<IHome>().ActiveContent = content;
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

        private bool _mSystem = true;
        private bool _bPrinterSettings = true;
        private bool _bInstallOutOfBrowser = true;


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

    }
}
