using eHCMSLanguage;
using System;
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
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IRoleForm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoleFormViewModel: Conductor<object>, IRoleForm
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RoleFormViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            allRole = new PagedSortableCollectionView<Role>();
        }
        void allRole_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllRolesAllPaging(0, allRole.PageSize, allRole.PageIndex, "", true);
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            allRole.OnRefresh += allRole_OnRefresh;
            GetAllRolesAllPaging(0, allRole.PageSize, allRole.PageIndex, "", true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //allRole.PageIndex = 0;
            //GetAllRolesAllPaging(0, allRole.PageSize, allRole.PageIndex, "", true);
            //==== 20161206 CMN End.
        }
        
        #region property

        private PagedSortableCollectionView<Role> _allRole;
        public PagedSortableCollectionView<Role> allRole
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
            }
        }
        #endregion

        public string txtName { get; set; }
        public string txtDescription { get; set; }

        public void butSave()
        {
            AddNewRoles(txtName, txtDescription);
            
        }
        public void lnkDeleteClick(object sender, RoutedEvent e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không? Xóa Role này sẽ xóa các Permision có liên quan.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteRole(SelectedRole.RoleID);
            }
        }
        public void lnkUpdateClick(object sender,RoutedEvent e)
        {
            //var cwdModuleEnumVM = Globals.GetViewModel<IcwdRoleUpdate>();
            //cwdModuleEnumVM.SelectedRole = SelectedRole;
            //var instance = cwdModuleEnumVM as Conductor<object>;
            //this.ActivateItem(cwdModuleEnumVM);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IcwdRoleUpdate> onInitDlg = (cwdModuleEnumVM) =>
            {
                cwdModuleEnumVM.SelectedRole = SelectedRole;
                this.ActivateItem(cwdModuleEnumVM);
            };
            GlobalsNAV.ShowDialog<IcwdRoleUpdate>(onInitDlg);
        }

        #region method
        /*▼====: #001*/
        private void GetAllRolesAllPaging(long RoleID, int PageSize, int PageIndex, string OrderBy,
                             bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRolesAllPaging(RoleID, PageSize, PageIndex, OrderBy,
                                 CountTotal, Globals.DispatchCallback((asyncResult) =>
                                 {
                                     try
                                     {
                                         int Total = 0;
                                         var results = contract.EndGetAllRolesAllPaging(out Total, asyncResult);
                                         if (results != null)
                                         {
                                             if (allRole == null)
                                             {
                                                 allRole = new PagedSortableCollectionView<Role>();
                                             }
                                             else
                                             {
                                                 allRole.Clear();
                                             }
                                             allRole.TotalItemCount = Total;
                                             foreach (var p in results)
                                             {
                                                 allRole.Add(p);
                                             }
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

        private void DeleteRole(int RoleID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRoles(RoleID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteRoles(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.T0432_G1_Error);
                                    allRole.PageIndex = 0;
                                    Globals.EventAggregator.Publish(new allRoleChangeEvent { });
                                    GetAllRolesAllPaging(0, allRole.PageSize, allRole.PageIndex, "", true);
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

        private void AddNewRoles(string GroupName, string Description)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewRoles(GroupName, Description, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewRoles(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.A1027_G1_Msg_InfoThemOK, eHCMSResources.T0432_G1_Error);
                                    txtName = "";
                                    txtDescription = "";
                                    allRole.PageIndex = 0;
                                    NotifyOfPropertyChange(() => txtName);
                                    NotifyOfPropertyChange(() => txtDescription);
                                    Globals.EventAggregator.Publish(new allRoleChangeEvent { });
                                    GetAllRolesAllPaging(0, allRole.PageSize, allRole.PageIndex, "", true);
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
