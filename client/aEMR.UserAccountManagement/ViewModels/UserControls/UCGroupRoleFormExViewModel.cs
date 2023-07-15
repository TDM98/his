using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

/*
 * #001 20180921 TNHX: Add method for set AllGroup, allRole, apply BusyIndicator
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUCGroupRoleFormEx)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCGroupRoleFormExViewModel : Conductor<object>, IUCGroupRoleFormEx
        , IHandle<allRoleChangeEvent>
        , IHandle<allGroupChangeEvent>
        , IHandle<GroupChangeCompletedEvent>
        , IHandle<RoleChangeCompletedEvent>
    {
        private long groupID = 0;
        private long roleID = 0;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UCGroupRoleFormExViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            _allGroupRole =new PagedSortableCollectionView<GroupRole>();
            //GetAllRoles();
            //GetAllGroupByGroupID(0);
            allGroupRole.OnRefresh += new EventHandler<RefreshEventArgs>(allGroupRole_OnRefresh);
        }
        protected override void OnActivate()
        {
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //GetAllRoles();
            //GetAllGroupByGroupID(0);
            //base.OnActivate();
            //allGroupRole.OnRefresh += new EventHandler<RefreshEventArgs>(allGroupRole_OnRefresh);
            //==== 20161206 CMN End.
            Globals.EventAggregator.Subscribe(this);
        }

        void allGroupRole_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllGroupRolesGetByID(groupID,roleID,allGroupRole.PageSize,allGroupRole.PageIndex,"",true);
        }

#region properties

        private ObservableCollection<Role> _allRole;
        public ObservableCollection<Role> allRole
        {
            get
            {
                return _allRole;
            }
            set
            {
                if (_allRole == value)
                    return;
                _allRole = value;
                NotifyOfPropertyChange(() => allRole);
            }
        }

        private ObservableCollection<Group> _allGroup;
        public ObservableCollection<Group> allGroup
        {
            get
            {
                return _allGroup;
            }
            set
            {
                if (_allGroup == value)
                    return;
                _allGroup = value;
                NotifyOfPropertyChange(() => allGroup);
            }
        }

        private Group _SelectedGroup;
        public Group SelectedGroup
        {
            get
            {
                return _SelectedGroup;
            }
            set
            {
                if (_SelectedGroup == value)
                    return;
                _SelectedGroup = value;
                NotifyOfPropertyChange(() => SelectedGroup);
                groupID = SelectedGroup.GroupID;
                if (groupID>0)
                {
                    allGroupRole.PageIndex = 0;
                    roleID = 0;
                    GetAllGroupRolesGetByID(groupID, roleID, allGroupRole.PageSize, allGroupRole.PageIndex, "", true);
                    SelectedRole = allRole[0];
                }
            }
        }

        private Role _SelectedRole;
        public Role SelectedRole
        {
            get
            {
                return _SelectedRole;
            }
            set
            {
                if (_SelectedRole == value)
                    return;
                _SelectedRole = value;
                NotifyOfPropertyChange(() => SelectedRole);
                roleID = SelectedRole.RoleID;
                if (roleID>0)
                {
                    allGroupRole.PageIndex = 0;
                    groupID = 0;
                    GetAllGroupRolesGetByID(groupID, roleID, allGroupRole.PageSize, allGroupRole.PageIndex, "", true);
                    SelectedGroup = allGroup[0];
                }
            }
        }

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
#endregion
        public void lnkDeleteClick(object sender, RoutedEvent e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteGroupRoles(SelectedGroupRole.GroupRoleID);
            }
        }

        public void Handle(allRoleChangeEvent obj)
        {
            GetAllRoles();
        }

        public void Handle(allGroupChangeEvent obj)
        {
            GetAllGroupByGroupID(0);
        }

        public void Handle(GroupChangeCompletedEvent obj)
        {
           GetAllGroupByGroupID(0);
        }

        public void Handle(RoleChangeCompletedEvent obj)
        {
            GetAllRoles();
        }

        #region method
        /*▼====: #001*/
        private void GetAllRoles()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRoles(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllRoles(asyncResult);
                                if (results != null)
                                {
                                    allRole = results.ToObservableCollection();
                                    Role usTemp = new Role();
                                    usTemp.RoleID = -1;
                                    usTemp.RoleName = "-----Chọn Một Role-----";
                                    allRole.Insert(0, usTemp);
                                    SelectedRole = usTemp;
                                    NotifyOfPropertyChange(() => allRole);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllGroupByGroupID(long GroupID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllGroupByGroupID(GroupID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllGroupByGroupID(asyncResult);
                                if (results != null)
                                {
                                    allGroup = results.ToObservableCollection();
                                    Group tempGroup = new Group();
                                    tempGroup.GroupID = -1;
                                    tempGroup.GroupName = "-----Chọn Một Group-----";
                                    SelectedGroup = tempGroup;
                                    allGroup.Insert(0, tempGroup);
                                    NotifyOfPropertyChange(() => allGroup);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllGroupRolesGetByID(long GroupID, long RoleID
                                        , int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllGroupRolesGetByID(GroupID, RoleID
                                            , PageSize, PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int total = 0;
                                var results = contract.EndGetAllGroupRolesGetByID(out total, asyncResult);
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
                                    allGroupRole.TotalItemCount = total;
                                    NotifyOfPropertyChange(() => allGroupRole);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void DeleteGroupRoles(long GroupRoleID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteGroupRoles(GroupRoleID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteGroupRoles(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.T0432_G1_Error);
                                    allGroupRole.PageIndex = 0;
                                    GetAllGroupRolesGetByID(groupID, roleID, allGroupRole.PageSize, allGroupRole.PageIndex, "", true);
                                    Globals.EventAggregator.Publish(new DeleteGroupRoleCompletedEvent { });
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        /*▲====: #001*/
        #endregion

    }
}
