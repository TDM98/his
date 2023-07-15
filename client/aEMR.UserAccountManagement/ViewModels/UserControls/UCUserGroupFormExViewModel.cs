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
using aEMR.Common.Utilities;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUCUserGroupFormEx)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCUserGroupFormExViewModel : Conductor<object>, IUCUserGroupFormEx
        , IHandle<allGroupChangeEvent>
        , IHandle<GroupChangeCompletedEvent>
    {
        private long groupID = 0;
        private long userAccountID = 0;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UCUserGroupFormExViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            /*▼====: #001*/
            //GetAllUserAccount(0);
            //GetAllGroupByGroupID(0);
            /*▲====: #001*/
            _allUserGroupEx = new PagedSortableCollectionView<UserGroup>();
            allUserGroupEx.OnRefresh += new EventHandler<RefreshEventArgs>(allUserGroupEx_OnRefresh);
        }

        void allUserGroupEx_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllUserGroupGetByID(userAccountID, groupID, allUserGroupEx.PageSize, allUserGroupEx.PageIndex
                , "", true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //_allUserGroupEx = new PagedSortableCollectionView<UserGroup>();
            //allUserGroupEx.OnRefresh += new EventHandler<RefreshEventArgs>(allUserGroupEx_OnRefresh);
            //==== 20161206 CMN End.
        }
       
        #region property

        private ObservableCollection<UserAccount> _allUserAccount;
        public ObservableCollection<UserAccount> allUserAccount
        {
            get
            {
                return _allUserAccount;
            }
            set
            {
                if (_allUserAccount == value)
                    return;
                _allUserAccount = value;
                NotifyOfPropertyChange(() => allUserAccount);
            }
        }

        private UserAccount _SelectedUserAccount;
        public UserAccount SelectedUserAccount
        {
            get
            {
                return _SelectedUserAccount;
            }
            set
            {
                if (_SelectedUserAccount == value)
                    return;
                _SelectedUserAccount = value;
                NotifyOfPropertyChange(() => SelectedUserAccount);
                userAccountID=SelectedUserAccount.AccountID;
                if (userAccountID>0)
                {
                    allUserGroupEx.PageIndex = 0;
                    groupID = 0;
                    GetAllUserGroupGetByID(userAccountID, groupID, allUserGroupEx.PageSize, allUserGroupEx.PageIndex
                        , "", true);
                    SelectedGroup = allGroup[0];
                }
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
                    allUserGroupEx.PageIndex = 0;
                    userAccountID = 0;
                    GetAllUserGroupGetByID(userAccountID, groupID, allUserGroupEx.PageSize, allUserGroupEx.PageIndex
                        , "", true);
                    SelectedUserAccount = allUserAccount[0];
                }
                   
            }
        }

        private PagedSortableCollectionView<UserGroup> _allUserGroupEx;
        public PagedSortableCollectionView<UserGroup> allUserGroupEx
        {
            get
            {
                return _allUserGroupEx;
            }
            set
            {
                if (_allUserGroupEx == value)
                    return;
                _allUserGroupEx = value;
                NotifyOfPropertyChange(() => allUserGroupEx);
            }
        }

        private UserGroup _selectedUserGroup;
        public UserGroup selectedUserGroup
        {
            get
            {
                return _selectedUserGroup;
            }
            set
            {
                if (_selectedUserGroup == value)
                    return;
                _selectedUserGroup = value;
                NotifyOfPropertyChange(() => selectedUserGroup);
            }
        }
        #endregion

        public void lnkDeleteClick(object sender, RoutedEvent e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteUserGroup((int) selectedUserGroup.UGID);
            }
        }
        public void Handle(allGroupChangeEvent obj)
        {
            GetAllGroupByGroupID(0);
        }
        public void Handle(GroupChangeCompletedEvent obj)
        {
            GetAllGroupByGroupID(0);
        }

        #region method
        public ObservableCollection<UserAccount> sortUserAccount(ObservableCollection<UserAccount> allUserAccount)
        {
            for (int i = 0; i < allUserAccount.Count-1; i++)
            {
                for (int j = i+1; j < allUserAccount.Count; j++)
                {
                    if (String.Compare(allUserAccount[i].AccountName , allUserAccount[j].AccountName)>0)
                    {
                        UserAccount temp = allUserAccount[i];
                        allUserAccount[i] = allUserAccount[j];
                        allUserAccount[j] = temp;
                    }
                }
            }

            return allUserAccount;
        }

        /*▼====: #001*/
        private void GetAllUserAccount(long AccountID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllUserAccount(AccountID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllUserAccount(asyncResult);
                                if (results != null)
                                {
                                    allUserAccount = results.ToObservableCollection();

                                    foreach (var p in allUserAccount)
                                    {
                                        try
                                        {
                                            p.AccountName = EncryptExtension.Decrypt(p.AccountName, Globals.AxonKey, Globals.AxonPass);
                                            if (p.AccountName == "")
                                            {
                                                p.AccountName = "Chưa Encrypt";
                                                continue;
                                            }
                                        }
                                        catch
                                        {
                                            p.AccountName = "Chưa Encrypt";
                                        }
                                    }
                                    allUserAccount = sortUserAccount(allUserAccount);
                                    UserAccount usTemp = new UserAccount();
                                    usTemp.AccountID = -1;
                                    usTemp.AccountName = "-----Chọn Một User-----";
                                    allUserAccount.Insert(0, usTemp);

                                    NotifyOfPropertyChange(() => allUserAccount);
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

        private void GetAllUserGroupGetByID(long AccountID, long GroupID
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
                        contract.BeginGetAllUserGroupGetByID(AccountID, GroupID
                                , PageSize, PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
                         {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndGetAllUserGroupGetByID(out Total, asyncResult);
                                if (results != null)
                                {
                                    if (allUserGroupEx == null)
                                    {
                                        allUserGroupEx = new PagedSortableCollectionView<UserGroup>();
                                    }
                                    else
                                    {
                                        allUserGroupEx.Clear();
                                    }

                                    foreach (var p in results)
                                    {
                                        try
                                        {
                                            if (Globals.isEncrypt)
                                            {
                                                p.UserAccount.AccountName = EncryptExtension.Decrypt(p.UserAccount.AccountName, Globals.AxonKey, Globals.AxonPass);
                                            }
                                            allUserGroupEx.Add(p);
                                        }
                                        catch
                                        { }
                                    //allUserGroupEx.Add(p);
                                }
                                    allUserGroupEx.TotalItemCount = Total;
                                    NotifyOfPropertyChange(() => allUserGroupEx);
                                //SelectedGroup = allGroup[0];
                                //SelectedUserAccount = allUserAccount[0];
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
        
        private void DeleteUserGroup(int UGID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteUserGroup(UGID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteUserGroup(asyncResult);
                                if (results == true)
                                {
                                    Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.T0432_G1_Error);
                                    allUserGroupEx.PageIndex = 0;
                                    GetAllUserGroupGetByID(userAccountID, groupID, allUserGroupEx.PageSize, allUserGroupEx.PageIndex
                                        , "", true);
                                    Globals.EventAggregator.Publish(new DeleteUserGroupCompletedEvent { });
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
