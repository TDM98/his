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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;

/*
 * #001 20180921 TNHX: Add method for set AllGroup, allRole, apply BusyIndicator
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUCGroupRoleForm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCGroupRoleFormViewModel : Conductor<object>, IUCGroupRoleForm
        , IHandle<allRoleChangeEvent>
        , IHandle<allGroupChangeEvent>
        , IHandle<GroupChangeCompletedEvent>
        , IHandle<RoleChangeCompletedEvent>
        , IHandle<DeleteGroupRoleCompletedEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UCGroupRoleFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            /*▼====: #001*/
            //GetAllRoles();
            //GetAllGroupByGroupID(0);
            //GetAllGroupRolesGetByID(0, 0, 500, 0, "", true);
            /*▲====: #001*/
            _allGroupRole = new ObservableCollection<GroupRole>();
            _allOldGroupRole=new ObservableCollection<GroupRole>();
            _SelectedRole=new Role();
            _SelectedGroup=new Group();
        }

        protected override void OnActivate()
        {
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //GetAllRoles();
            //GetAllGroupByGroupID(0);
            //GetAllGroupRolesGetByID(0, 0, 500, 0, "", true);
            //==== 20161206 CMN End.
            base.OnActivate();    
            Globals.EventAggregator.Subscribe(this);
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
                NotifyOfPropertyChange(() => CanbutAdd);
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
                NotifyOfPropertyChange(()=>CanbutAdd);
            }
        }

        private ObservableCollection<GroupRole> _allGroupRole;
        public ObservableCollection<GroupRole> allGroupRole
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

        private ObservableCollection<GroupRole> _allOldGroupRole;
        public ObservableCollection<GroupRole> allOldGroupRole
        {
            get
            {
                return _allOldGroupRole;
            }
            set
            {
                if (_allOldGroupRole == value)
                    return;
                _allOldGroupRole = value;
                NotifyOfPropertyChange(() => allOldGroupRole);
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

        public bool CheckExist()
        {
            if (allOldGroupRole!=null)
            {
                foreach (var UG in allOldGroupRole)
                {
                    if (UG.RoleID == SelectedRole.RoleID
                        && UG.GroupID == SelectedGroup.GroupID)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1759_G1_ThemMoiVaiTro, "");
                        return false;
                    }
                }    
            }
            
            return true;
        }

        public bool CheckExist(ObservableCollection<GroupRole> lstGR,GroupRole GR)
        {
            if (lstGR != null)
            {
                foreach (var UG in lstGR)
                {
                    if (UG.Role.RoleID == GR.Role.RoleID
                        && UG.Group.GroupID == GR.Group.GroupID)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1759_G1_ThemMoiVaiTro, "");
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanbutAdd
        {
            get { return SelectedGroup.GroupID > 0 && SelectedRole.RoleID > 0 ? true : false; }
        }

        public void butAdd()
        {
            if (CheckExist())
            {
                GroupRole UG = new GroupRole();
                UG.Group = new Group();
                UG.Role = new Role();
                UG.Group = SelectedGroup;
                UG.Role = SelectedRole;
                if (CheckExist(allGroupRole, UG))
                {
                    UG.GroupID = UG.Group.GroupID;
                    UG.RoleID = UG.Role.RoleID;
                    allGroupRole.Add(UG);    
                }
            }
        }

        public void butSave()
        {
            if (allGroupRole.Count > 0)
            {
                //for (int i = 0; i < allGroupRole.Count; i++)
                //{
                //    AddNewGroupRoles(allGroupRole[i].Group.GroupID, allGroupRole[i].Role.RoleID);
                //}
                AddNewGroupRolesXML(allGroupRole);
            }
        }

        public void lnkDeleteClick(object sender, RoutedEvent e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                allGroupRole.Remove(SelectedGroupRole);
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
        public void Handle(DeleteGroupRoleCompletedEvent obj)
        {
            GetAllGroupRolesGetByID(0, 0, 500, 0, "", true);
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

        private void AddNewGroupRoles(int GroupID, int RoleID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewGroupRoles(GroupID, RoleID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewGroupRoles(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.T0432_G1_Error);
                                    allGroupRole.Clear();
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

        private void AddNewGroupRolesXML(ObservableCollection<GroupRole> lstGroupRole)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewGroupRolesXML(lstGroupRole, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewGroupRolesXML(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, eHCMSResources.T0432_G1_Error);
                                    foreach (var GroupRole in lstGroupRole)
                                    {
                                        allOldGroupRole.Add(GroupRole);
                                    }

                                    allGroupRole.Clear();
                                    GetAllGroupRolesGetByID(0, 0, 500, 0, "", true);
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
                                        var results = contract.EndGetAllGroupRolesGetByID(out int total, asyncResult);
                                        if (results != null)
                                        {
                                            allOldGroupRole = results.ToObservableCollection();
                                            NotifyOfPropertyChange(() => allOldGroupRole);
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

