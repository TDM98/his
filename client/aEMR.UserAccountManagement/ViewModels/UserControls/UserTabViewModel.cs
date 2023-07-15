using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUserTab)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserTabViewModel : Conductor<object>, IUserTab
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UserTabViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            var UserAccountVM = Globals.GetViewModel<IUserAccount>();
            UserAccountForm = UserAccountVM;
            this.ActivateItem(UserAccountVM);

            var StaffGridVM = Globals.GetViewModel<IStaffGrid>();
            StaffForm = StaffGridVM;
            this.ActivateItem(StaffGridVM);
        }

        protected override void  OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //var UserAccountVM = Globals.GetViewModel<IUserAccount>();
            //UserAccountForm = UserAccountVM;
            //this.ActivateItem(UserAccountVM);


            //var StaffGridVM = Globals.GetViewModel<IStaffGrid>();
            //StaffForm = StaffGridVM;
            //this.ActivateItem(StaffGridVM);
            //==== 20161206 CMN End.
        }
        public object UserAccountForm { get; set; }
        public object StaffForm { get; set; }
        
    }
}
