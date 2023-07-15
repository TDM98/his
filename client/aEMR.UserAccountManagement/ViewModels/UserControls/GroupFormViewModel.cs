using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
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
/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IGroupForm)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class GroupFormViewModel : Conductor<object>, IGroupForm
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GroupFormViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            allGroup = new PagedSortableCollectionView<Group>();
            allGroup.PageIndex = 0;
        }

        void allGroup_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllGroupAllPaging(0, _allGroup.PageSize, _allGroup.PageIndex, "", true);
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            allGroup.OnRefresh += allGroup_OnRefresh;
            GetAllGroupAllPaging(0, allGroup.PageSize, allGroup.PageIndex, "", true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();    
        }

        #region property

        private PagedSortableCollectionView<Group> _allGroup;
        public PagedSortableCollectionView<Group> allGroup
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
            }
        }
        #endregion

        public string txtName { get; set; }
        public string txtDescription { get; set; }

        public void butSave()
        {
            AddNewGroup(txtName, txtDescription); 
        }
        public void lnkDeleteClick(object sender, RoutedEvent e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteGroup(SelectedGroup.GroupID);    
            }
            
        }
        public void lnkUpdateClick(object sender, RoutedEvent e)
        {
            //var cwdModuleEnumVM = Globals.GetViewModel<IcwdGroupUpdate>();
            //cwdModuleEnumVM.SelectedGroup = SelectedGroup;
            //var instance = cwdModuleEnumVM as Conductor<object>;
            //this.ActivateItem(cwdModuleEnumVM);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IcwdGroupUpdate> onInitDlg = (cwdModuleEnumVM) =>
            {
                cwdModuleEnumVM.SelectedGroup = SelectedGroup;
                var instance = cwdModuleEnumVM as Conductor<object>;
                this.ActivateItem(cwdModuleEnumVM);
            };
            GlobalsNAV.ShowDialog<IcwdGroupUpdate>();

        }

        #region method
        /*▼====: #001*/
        private void GetAllGroupAllPaging(long GroupID, int PageSize, int PageIndex, string OrderBy,
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
                        contract.BeginGetAllGroupAllPaging(GroupID, PageSize, PageIndex, OrderBy,
                                 CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndGetAllGroupAllPaging(out Total, asyncResult);
                                if (results != null)
                                {
                                    if (allGroup == null)
                                    {
                                        allGroup = new PagedSortableCollectionView<Group>();
                                    }
                                    else
                                    {
                                        allGroup.Clear();
                                    }
                                    foreach (var p in results)
                                    {
                                        allGroup.Add(p);
                                    }
                                    allGroup.TotalItemCount = Total;
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

        private void DeleteGroup(int GroupID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteGroup(GroupID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteGroup(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.T0432_G1_Error);
                                    allGroup.PageIndex = 0;
                                    Globals.EventAggregator.Publish(new allGroupChangeEvent { });
                                    GetAllGroupAllPaging(0, _allGroup.PageSize, _allGroup.PageIndex, "", true);
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
        private void AddNewGroup(string GroupName, string Description)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewGroup(GroupName, Description, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewGroup(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.A1027_G1_Msg_InfoThemOK, eHCMSResources.T0432_G1_Error);
                                    txtName = "";
                                    txtDescription = "";
                                    NotifyOfPropertyChange(() => txtName);
                                    NotifyOfPropertyChange(() => txtDescription);
                                    allGroup.PageIndex = 0;
                                    Globals.EventAggregator.Publish(new allGroupChangeEvent { });
                                    GetAllGroupAllPaging(0, _allGroup.PageSize, _allGroup.PageIndex, "", true);
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
