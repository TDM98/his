using System;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.SystemConfiguration.ViewModels
{
    [Export(typeof(IInstallationManagement)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InstallationManagementViewModel : Conductor<object>, IInstallationManagement,IDisposable
    {
        [ImportingConstructor]
        public InstallationManagementViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            authorization();
            //Application.Current.InstallStateChanged += Current_InstallStateChanged;
        }

        private bool _mInstall = true;

        public bool mInstall
        {
            get
            {
                return _mInstall;
            }
            set
            {
                if (_mInstall == value)
                    return;
                _mInstall = value;
                NotifyOfPropertyChange(() => mInstall);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mInstall = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mSystem_Management
                                               , (int)eSystem_Management.mInstallOutOfBrowser,
                                               (int)oSystem_ManagementEx.mInstall, (int)ePermission.mView);
        }

        //private string _description;
        //public string Description
        //{
        //    //get
        //    //{
        //    //    if (Application.Current.InstallState == InstallState.NotInstalled)
        //    //    {
        //    //        return string.Format("{0}.", eHCMSResources.Z1246_G1_UngDungChuaCaiDat);
        //    //    }
        //    //    if (Application.Current.InstallState == InstallState.Installed)
        //    //    {
        //    //        return string.Format("{0}.", eHCMSResources.Z1245_G1_UngDungDaCaiDat);
        //    //    }
        //    //    return string.Empty;
        //    //}
        //    //set
        //    //{
        //    //    _description = value;
        //    //    NotifyOfPropertyChange(() => Description);
        //    //}
        //}

        private string _installationState;
        public string InstallationState
        {
            get { return _installationState; }
            set
            {
                _installationState = value;
                NotifyOfPropertyChange(() => InstallationState);
            }
        }
       
        private bool _isInstalling;
        public bool IsInstalling
        {
            get
            {
                return _isInstalling;
            }
            set
            {
                _isInstalling = value;
                NotifyOfPropertyChange(() => IsInstalling);

                //NotifyOfPropertyChange(() => CanInstallCmd);
            }
        }

        //public bool CanInstallCmd
        //{
        //    get
        //    {
        //        return !IsInstalling && Application.Current.InstallState != InstallState.Installed
        //               && Application.Current.InstallState != InstallState.Installing;
        //    }
        //}
        public void InstallCmd()
        {
            //if (CanInstallCmd)
            {
                //Application.Current.Install();
            }
        }
        
        void Current_InstallStateChanged(object sender, EventArgs e)
        {
            //switch (Application.Current.InstallState)
            //{
            //    case InstallState.Installing:
            //        InstallationState = eHCMSResources.Z1169_G1_Installing;
            //        break;
            //    case InstallState.InstallFailed:
            //        InstallationState = eHCMSResources.Z1170_G1_InstallingFailed;
            //        IsInstalling = false;
            //        break;
            //    case InstallState.Installed:
            //        InstallationState = eHCMSResources.Z1171_G1_InstallationCompleted;
            //        IsInstalling = false;
            //        break;
            //}
        }

        public void Dispose()
        {
            //Application.Current.InstallStateChanged -= Current_InstallStateChanged;
        }
    }
}
