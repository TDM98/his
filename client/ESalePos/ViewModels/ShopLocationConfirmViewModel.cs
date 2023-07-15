
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;
using EPos.BusinessLayers;
using EPos.DataTransferObjects;

using aEMR.Infrastructure;
using aEMR.Infrastructure.Extensions;
using aEMR.Infrastructure.Utils;
using aEMR.Infrastructure.ViewUtils;
using EPos.ServiceContracts;
using EPos.ViewContracts;
using EPos.ViewContracts.SettingViewContracts;
using ESalePos.Views;
using Castle.Facilities.WcfIntegration;
using EPos.Common;
using System.Linq;
using aEMR.ViewContracts;

namespace ESalePos.ViewModels
{
    [Export(typeof(aEMR.ViewContracts.IShopLocationConfirmViewModel))]
    public class ShopLocationConfirmViewModel : CommonView<StaffDto>, aEMR.ViewContracts.IShopLocationConfirmViewModel
    {
        private readonly INavigationService _navigationService;
        
        [ImportingConstructor]
        public ShopLocationConfirmViewModel(IWindsorContainer container, INavigationService navigationService) : base(container)
        {
            _navigationService = navigationService;
        }

 
        private string _LoggedInStaffName;
        public string LoggedInStaffName
        {
            get
            {
                return _LoggedInStaffName;
            }
            set
            {
                _LoggedInStaffName = value;
                NotifyOfPropertyChange(() => LoggedInStaffName);
            }
        }

        private string _LoggedInSiteName;
        public string LoggedInSiteName
        {
            get
            {
                return _LoggedInSiteName;
            }
            set
            {
                _LoggedInSiteName = value;
                NotifyOfPropertyChange(() => LoggedInSiteName);
            }
        }

     
        public override void Initial()
        {
            LoggedInStaffName = Globals.LoggedUser.Person._FullName;
            LoggedInSiteName = Globals.SiteLogged.Sitename;
        }


 
        public void LoginCmd()
        {
            _navigationService.NavigationTo<aEMR.ViewContracts.IHomeViewModel>();
        }



        public void AppExitCmd()
        {
            Application.Current.MainWindow.Close();
        }

    }
}




