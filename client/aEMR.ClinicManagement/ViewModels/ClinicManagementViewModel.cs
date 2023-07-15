using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ClinicManagement.ViewModels
{
    //phai su dung nonshared vi loi khi menu nay roi click cau hinh he thong 
    //sau do click lai no thi ko hien thi menu nay
    //[Export(typeof(IClinicManagement)), PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IClinicManagement))]
    public class ClinicManagementViewModel : Conductor<object>, IClinicManagement
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ClinicManagementViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            //Khi khoi tao module thi load menu ben trai luon.
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IClinicLeftMenu>();
            shell.LeftMenu = leftMenu;

            var topMenu = Globals.GetViewModel<IClinicTopMenu>();
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
