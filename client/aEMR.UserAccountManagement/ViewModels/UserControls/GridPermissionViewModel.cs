using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IGridPermission)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GridPermissionViewModel : Conductor<object>, IGridPermission,IHandle<SelectRoleChangeEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GridPermissionViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _allPermission =new PagedSortableCollectionView<Permission>();
            _allPermission.OnRefresh += new EventHandler<RefreshEventArgs>(_allPermission_OnRefresh);
        }

        void _allPermission_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllPermissions_GetByID(RoleID, 0, allPermission.PageSize, allPermission.PageIndex, "", true);
        }
        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
             Globals.EventAggregator.Subscribe(this);
             //==== 20161206 CMN Begin: Disable method called in onloaded
             //_allPermission = new PagedSortableCollectionView<Permission>();
             //_allPermission.OnRefresh += new EventHandler<RefreshEventArgs>(_allPermission_OnRefresh);
            //==== 20161206 CMN End.
        }
        #region property

        private PagedSortableCollectionView<Permission> _allPermission;
        public PagedSortableCollectionView<Permission> allPermission
        {
            get
            {
                return _allPermission;
            }
            set
            {
                if (_allPermission == value)
                    return;
                _allPermission = value;
                NotifyOfPropertyChange(() => allPermission);
            }
        }

        private Permission _SelectedPermission;
        public Permission SelectedPermission
        {
            get
            {
                return _SelectedPermission;
            }
            set
            {
                if (_SelectedPermission == value)
                    return;
                _SelectedPermission = value;
                NotifyOfPropertyChange(() => SelectedPermission);
            }
        }

        public long RoleID = 0;
        #endregion
        public void DoubleClick(object sender)
        {
            //Globals.EventAggregator.Publish(new SelectRoleChangeEvent { SelectedRole = SelectedGroupRole.Role });
        }
        public void Handle(SelectRoleChangeEvent obj)
        {
            if (obj != null)
            {
                RoleID = ((Role)obj.SelectedRole).RoleID;
                allPermission.PageIndex = 0;
                GetAllPermissions_GetByID(RoleID, 0, allPermission.PageSize, allPermission.PageIndex, "", true);
            }
        }
        #region method
        private void GetAllPermissions_GetByID(long RoleID, long OperationID, int PageSize, int PageIndex, string OrderBy,
                             bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllPermissions_GetByID(RoleID, OperationID, PageSize, PageIndex, OrderBy,
                             CountTotal, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     int Total = 0;
                                     var results = contract.EndGetAllPermissions_GetByID(out Total, asyncResult);
                                     if (results != null)
                                     {
                                         if (allPermission == null)
                                         {
                                             allPermission = new PagedSortableCollectionView<Permission>();
                                         }
                                         else
                                         {
                                             allPermission.Clear();
                                         }
                                         foreach (var p in results)
                                         {
                                             allPermission.Add(p);
                                         }
                                         allPermission.TotalItemCount = Total;
                                         NotifyOfPropertyChange(() => allPermission);
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                     Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                 }
                                 finally
                                 {
                                     //Globals.IsBusy = false;
                                     IsLoading = false;
                                 }

                             }), null);

                }

            });

            t.Start();
        }
        #endregion
    }
}
