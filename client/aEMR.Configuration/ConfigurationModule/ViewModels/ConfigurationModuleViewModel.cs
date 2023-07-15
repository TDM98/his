using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Configuration.ConfigurationModule.ViewModels
{
    //phai su dung nonshared vi loi khi menu nay roi click cau hinh he thong 
    //sau do click lai no thi ko hien thi menu nay
    //[Export(typeof(IConfigurationModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IConfigurationModule)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ConfigurationModuleViewModel : Conductor<object>, IConfigurationModule
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ConfigurationModuleViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void OnActivate()
        {
            MainContent = null;
            
            base.OnActivate();

            //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IConfigurationLeftMenu>();
            shell.LeftMenu = leftMenu;

            var topMenu = Globals.GetViewModel<IConfigurationTopMenu>();
            shell.TopMenuItems = topMenu;
            (shell as Conductor<object>).ActivateItem(topMenu);

            shell.OutstandingTaskContent = null;
            shell.IsExpandOST = false;
        }


        private object _mainContent;

        public object MainContent
        {
            get { return _mainContent; }
            set
            {
                _mainContent = value;
                NotifyOfPropertyChange(() => MainContent);
            }
        }
    }
}
