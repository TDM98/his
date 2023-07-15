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
 * #001 20180921 TNHX: Apply BusyIndicator, add allModulesTree for Parent's set then it set for child, refactor code, Remove unuses handle
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IPermissonForm)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PermissonFormViewModel : Conductor<object>, IPermissonForm
        , IHandle<OperationChangeEvent>
        , IHandle<allRoleChangeEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PermissonFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            var UCModulesTreeVM = Globals.GetViewModel<IUCModulesTreePer>();
            UCModulesTreeView = UCModulesTreeVM;
            this.ActivateItem(UCModulesTreeVM);
            _allRole=new ObservableCollection<Role>();
            
            GetAllRoles();
            allPermission = new PagedSortableCollectionView<Permission>();
            allPermission.PageIndex = 0;
            SelectedRole = new Role();
            SelectedRole.RoleID = 0;
            //Globals.EventAggregator.Subscribe(this);
        }
        
        void allPermission_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllPermissions_GetByID(SelectedRole.RoleID, 0, allPermission.PageSize, allPermission.PageIndex, "", true);
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            allPermission.OnRefresh += allPermission_OnRefresh;
            GetAllPermissions_GetByID(0, 0, allPermission.PageSize, allPermission.PageIndex, "", true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //_allPermission = new PagedSortableCollectionView<Permission>();
            //_curPermission = new Permission();
            //var UCModulesTreeVM = Globals.GetViewModel<IUCModulesTreePer>();
            //UCModulesTreeView = UCModulesTreeVM;
            //this.ActivateItem(UCModulesTreeVM);
            //GetAllRoles();
            //==== 20161206 CMN End.
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public object UCModulesTreeView { get; set; }

        #region property

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
                NotifyOfPropertyChange(()=>allRole);
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
                NotifyOfPropertyChange(() => CanbutSave);
                if (SelectedRole != null)
                {
                    allPermission.PageIndex = 0;
                    GetAllPermissions_GetByID(SelectedRole.RoleID, 0, allPermission.PageSize, allPermission.PageIndex, "", true);
                    if (curPermission == null)
                    {
                        curPermission = new Permission(); 
                    }
                    curPermission.Role = SelectedRole;
                    NotifyOfPropertyChange(() => curPermission);
                }
                
            }
        }

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
                NotifyOfPropertyChange(()=>allPermission);
            }
        }

        private Operation _SelectedOperation;
        public Operation SelectedOperation
        {
            get
            {
                return _SelectedOperation;
            }
            set
            {
                if (_SelectedOperation == value)
                    return;
                _SelectedOperation = value;
                NotifyOfPropertyChange(() => SelectedOperation);
                NotifyOfPropertyChange(() => CanbutSave);
             
                //if (SelectedOperation!=null)
                //{
                //    if (curPermission == null)
                //    {
                //        curPermission = new Permission();
                //    }
                //    curPermission.Operation = SelectedOperation;
                //}
                
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

        private Permission _curPermission;
        public Permission curPermission
        {
            get
            {
                return _curPermission;
            }
            set
            {
                if (_curPermission == value)
                    return;
                _curPermission = value;
                NotifyOfPropertyChange(() => curPermission);
            }
        }

        public bool CanbutSave
        {
            get
            {
                return SelectedOperation != null && SelectedRole!=null ? true : false;
            } 
        }

        #endregion
        public long checkExist(Operation curOperation)
        {
            foreach (var Permission in allPermission)
            {
                if (Permission.OperationID == curOperation.OperationID)//update
                {
                    return Permission.PermissionItemID;
                }
            }
            return 0;
        }
        
        public void butSave()
        {
            if (curPermission != null
                && SelectedOperation.OperationID > 0
                )
            {
                long permisionID = checkExist(SelectedOperation);
                if (permisionID>0)
                {
                    UpdatePermissions(permisionID
                         
                        , curPermission.pFullControl
                        , curPermission.pView
                        , curPermission.pAdd
                        , curPermission.pUpdate
                        , curPermission.pDelete
                        , curPermission.pReport
                        , curPermission.pPrint
                        );
                }
                else
                {
                    AddNewPermissions(
                         SelectedRole.RoleID
                         , SelectedOperation.OperationID
                        , curPermission.pFullControl
                        , curPermission.pView
                        , curPermission.pAdd
                        , curPermission.pUpdate
                        , curPermission.pDelete
                        , curPermission.pReport
                        , curPermission.pPrint
                        );
                }
                

            }    
        }
        
        public void Handle(OperationChangeEvent obj)
        {
            if (obj != null) 
            {
                SelectedOperation = (Operation)obj.curOperation;
                //NotifyOfPropertyChange(() => SelectedOperation);
            }
        }
        public void Handle(allRoleChangeEvent obj)
        {
            GetAllRoles();
        }
        
        public void lnkDeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeletePermissions(SelectedPermission.PermissionItemID);
            }
        }

        /*▼====: #001*/
        private ObservableCollection<ModulesTree> _allModulesTree;
        public ObservableCollection<ModulesTree> allModulesTree
        {
            get { return _allModulesTree; }
            set
            {
                if (_allModulesTree == value)
                    return;
                _allModulesTree = value;
                ((IUCModulesTreePer)UCModulesTreeView).allModulesTree = _allModulesTree;
                NotifyOfPropertyChange(() => allModulesTree);
            }
        }

        #region method
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

        private void GetAllPermissions_GetByID(long RoleID, long OperationID
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
                        contract.BeginGetAllPermissions_GetByID(RoleID, OperationID, PageSize
                            , PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
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

        private void AddNewPermissions(int RoleID, long OperationID, bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
                                                , bool pPrint)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewPermissions(RoleID, OperationID, FullControl
                                                    , pView
                                                    , pAdd
                                                    , pUpdate
                                                    , pDelete
                                                    , pReport
                                                    , pPrint, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewPermissions(asyncResult);
                                if (results == true)
                                {
                                    allPermission.PageIndex = 0;
                                    GetAllPermissions_GetByID(SelectedRole.RoleID, 0, allPermission.PageSize, allPermission.PageIndex, "", true);
                                    Globals.ShowMessage("Thêm mới Permission thành công!", "");
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

        private void UpdatePermissions(long PermissionItemID, bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
                                                , bool pPrint)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePermissionsFull(PermissionItemID, FullControl
                                                    , pView
                                                    , pAdd
                                                    , pUpdate
                                                    , pDelete
                                                    , pReport
                                                    , pPrint, Globals.DispatchCallback((asyncResult) =>
                                                    {
                                                        try
                                                        {
                                                            var results = contract.EndUpdatePermissionsFull(asyncResult);
                                                            if (results == true)
                                                            {
                                                                allPermission.PageIndex = 0;
                                                                GetAllPermissions_GetByID(SelectedRole.RoleID, 0, allPermission.PageSize, allPermission.PageIndex, "", true);
                                                                Globals.ShowMessage(eHCMSResources.Z1757_G1_CNhatPermission, "");
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

        private void DeletePermissions(long PermissionItemID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeletePermissions(PermissionItemID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeletePermissions(asyncResult);
                                if (results == true)
                                {
                                    allPermission.PageIndex = 0;
                                    GetAllPermissions_GetByID(SelectedRole.RoleID, 0, allPermission.PageSize, allPermission.PageIndex, "", true);

                                    Globals.ShowMessage(eHCMSResources.Z1758_G1_XoaPermissionThCong, "");
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
