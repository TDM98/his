using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;

namespace aEMR.Pharmacy.ViewModels
{
     [Export(typeof(IPharmacyModule)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PharmacyModuleViewModel : Conductor<object>, IPharmacyModule
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacyModuleViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //this.Activated += new System.EventHandler<ActivationEventArgs>(PharmacyModuleViewModel_Activated);
        }
         protected override void OnActivate()
         {
             base.OnActivate();
             MainContent = null;

             //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IPharmacyLeftMenu>();
            var topMenu = Globals.GetViewModel<IPharmacyTopMenu>();
            shell.LeftMenu = leftMenu;
            shell.TopMenuItems = topMenu;
            ActivateItem(leftMenu);
            ActivateItem(topMenu);
        }

         void PharmacyModuleViewModel_Activated(object sender, ActivationEventArgs e)
         {
             //Khi khoi tao module thi load menu ben trai luon.
            var shell = Globals.GetViewModel<IHome>();
            var leftMenu = Globals.GetViewModel<IPharmacyLeftMenu>();
            var topMenu = Globals.GetViewModel<IPharmacyTopMenu>();
            shell.LeftMenu = leftMenu;
            shell.TopMenuItems = topMenu;
            (shell as Conductor<object>).ActivateItem(leftMenu);
            (shell as Conductor<object>).ActivateItem(topMenu);
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
