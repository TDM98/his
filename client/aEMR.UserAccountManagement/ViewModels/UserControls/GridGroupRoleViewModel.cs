using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
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
    [Export(typeof(IGridGroupRole)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GridGroupRoleViewModel : Conductor<object>, IGridGroupRole, IHandle<SelectGroupChangeEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GridGroupRoleViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _allGroupRole = new PagedSortableCollectionView<GroupRole>();
            _allGroupRole.PageIndex = 0;
            _allGroupRole.OnRefresh += new EventHandler<RefreshEventArgs>(_allGroupRole_OnRefresh);
            
        }

        void _allGroupRole_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllGroupRolesGetByID(groupID, 0, allGroupRole.PageSize, allGroupRole.PageIndex, "", true);
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
             Globals.EventAggregator.Subscribe(this);
             //==== 20161206 CMN Begin: Disable method called in onloaded
             //_allGroupRole = new PagedSortableCollectionView<GroupRole>();
             //_allGroupRole.OnRefresh += new EventHandler<RefreshEventArgs>(_allGroupRole_OnRefresh);
            //==== 20161206 CMN End.
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

        #region property

        private PagedSortableCollectionView<GroupRole> _allGroupRole;
        public PagedSortableCollectionView<GroupRole> allGroupRole
        {
            get
            {
                return _allGroupRole;
            }
            set
            {
                if (_allGroupRole == value)
                    return;
                _allGroupRole = value;
                NotifyOfPropertyChange(() => allGroupRole);
            }
        }

        private GroupRole _SelectedGroupRole;
        public GroupRole SelectedGroupRole
        {
            get
            {
                return _SelectedGroupRole;
            }
            set
            {
                if (_SelectedGroupRole == value)
                    return;
                _SelectedGroupRole = value;
                NotifyOfPropertyChange(() => SelectedGroupRole);
            }
        }

        public long groupID = 0;
        #endregion
        public void DoubleClick(object sender)
        {
            Globals.EventAggregator.Publish(new SelectRoleChangeEvent { SelectedRole = SelectedGroupRole.Role });
        }
        public void Handle(SelectGroupChangeEvent obj)
        {
            if(obj!=null)
            {
                groupID=((Group)obj.SelectedGroup).GroupID;
                allGroupRole.PageIndex = 0;
                GetAllGroupRolesGetByID(groupID, 0, allGroupRole.PageSize, allGroupRole.PageIndex, "", true);
            }
        }
        #region method
        private void GetAllGroupRolesGetByID(long GroupID, long RoleID, int PageSize, int PageIndex, string OrderBy,
                             bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllGroupRolesGetByID(GroupID, RoleID, PageSize, PageIndex, OrderBy,
                             CountTotal, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     int Total = 0;
                                     var results = contract.EndGetAllGroupRolesGetByID(out Total, asyncResult);
                                     if (results != null)
                                     {
                                         if (allGroupRole == null)
                                         {
                                             allGroupRole = new PagedSortableCollectionView<GroupRole>();
                                         }
                                         else
                                         {
                                             allGroupRole.Clear();
                                         }
                                         foreach (var p in results)
                                         {
                                             allGroupRole.Add(p);
                                         }
                                         allGroupRole.TotalItemCount = Total;
                                         NotifyOfPropertyChange(() => allGroupRole);
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
