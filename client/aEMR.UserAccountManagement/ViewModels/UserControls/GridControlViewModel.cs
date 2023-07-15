using System;
using System.ComponentModel.Composition;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IGridControl)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GridControlViewModel : Conductor<object>, IGridControl
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GridControlViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            var GridGroupVM = Globals.GetViewModel<IGridGroup>();
            GridGroupView = GridGroupVM;
            this.ActivateItem(GridGroupVM);

            var GridUserGroupVM = Globals.GetViewModel<IGridUserGroup>();
            GridUserGroupView = GridUserGroupVM;
            this.ActivateItem(GridUserGroupVM);

            var GridGroupRoleVM = Globals.GetViewModel<IGridGroupRole>();
            GridGroupRoleView = GridGroupRoleVM;
            this.ActivateItem(GridGroupRoleVM);

            var GridPermissionVM = Globals.GetViewModel<IGridPermission>();
            GridPermissionView = GridPermissionVM;
            this.ActivateItem(GridPermissionVM);
             
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
             //==== 20161206 CMN Begin: Disable method called in onloaded
             //var GridGroupVM = Globals.GetViewModel<IGridGroup>();
             //GridGroupView = GridGroupVM;
             //this.ActivateItem(GridGroupVM);

             //var GridUserGroupVM = Globals.GetViewModel<IGridUserGroup>();
             //GridUserGroupView = GridUserGroupVM;
             //this.ActivateItem(GridUserGroupVM);

             //var GridGroupRoleVM = Globals.GetViewModel<IGridGroupRole>();
             //GridGroupRoleView = GridGroupRoleVM;
             //this.ActivateItem(GridGroupRoleVM);

             //var GridPermissionVM = Globals.GetViewModel<IGridPermission>();
             //GridPermissionView = GridPermissionVM;
             //this.ActivateItem(GridPermissionVM);
            //==== 20161206 CMN End.
        }

        public object GridGroupView{ get; set; }
        public object GridUserGroupView{ get; set; }
        public object GridGroupRoleView{ get; set; }
        public object GridPermissionView { get; set; }
        
    }
}
