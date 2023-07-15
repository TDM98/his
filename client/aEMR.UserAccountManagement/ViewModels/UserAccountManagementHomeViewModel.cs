using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUserAccountManagementHome))]
    public class UserAccountManagementHomeViewModel : Conductor<object>, IUserAccountManagementHome
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UserAccountManagementHomeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IUserAccountLeftMenu>();
            shell.LeftMenu = leftMenu;
            (shell as Conductor<object>).ActivateItem(leftMenu);

            // 20180731 TNHX: Add Top Menu
            var topMenu = Globals.GetViewModel<IUserAccountTopMenu>();
            shell.TopMenuItems = topMenu;
            (shell as Conductor<object>).ActivateItem(topMenu);
        }
        private object _mainContent;
        public object mainContent
        {
            get
            {
                return _mainContent;
            }
            set
            {
                if (_mainContent == value)
                    return;
                _mainContent = value;
                NotifyOfPropertyChange(() => mainContent);
            }
        }
    }
}
