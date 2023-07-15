using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResourcesHome)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ResourcesHomeViewModel : Conductor<object>, IResourcesHome
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public  ResourcesHomeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //Khi khoi tao module thi load menu ben trai luon.

        }
        protected override void OnActivate()
        {
            base.OnActivate();
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IResourcesLeftMenu>();
            shell.LeftMenu = leftMenu;
            (shell as Conductor<object>).ActivateItem(leftMenu);
            // 20183107 TNHX: Add Top Menu
            var topMenu = Globals.GetViewModel<IResourcesTopMenu>();
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
