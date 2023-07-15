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

namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(ILoginHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginHistoryViewModel : Conductor<object>, ILoginHistory
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LoginHistoryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            GetUserLogHisGByHostName();
            GetUserLogHisGByUserAccount();
            _searchUserLoginHistory=new UserLoginHistory();
            _allUserLoginHistory=new PagedSortableCollectionView<UserLoginHistory>();
            _allUserLoginHistory.PageIndex = 0;
            _allUserLoginHistory.PageSize = 15;
            _allUserLoginHistory.OnRefresh += new EventHandler<RefreshEventArgs>(_allUserLoginHistory_OnRefresh);

        }

        void _allUserLoginHistory_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllUserLoginHistoryPaging(searchUserLoginHistory, allUserLoginHistory.PageSize, allUserLoginHistory.PageIndex, "", true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //_allUserLoginHistory = new PagedSortableCollectionView<UserLoginHistory>();
            //_allUserLoginHistory.PageIndex = 0;
            //_allUserLoginHistory.PageSize = 15;
            //_allUserLoginHistory.OnRefresh += new EventHandler<RefreshEventArgs>(_allUserLoginHistory_OnRefresh);
            //==== 20161206 CMN End.
        }

        private bool _IsLoading1 = false;
        public bool IsLoading1
        {
            get { return _IsLoading1; }
            set
            {
                if (_IsLoading1 != value)
                {
                    _IsLoading1 = value;
                    NotifyOfPropertyChange(() => IsLoading1);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsLoading2 = false;
        public bool IsLoading2
        {
            get { return _IsLoading2; }
            set
            {
                if (_IsLoading2 != value)
                {
                    _IsLoading2 = value;
                    NotifyOfPropertyChange(() => IsLoading2);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsLoading3 = false;
        public bool IsLoading3
        {
            get { return _IsLoading3; }
            set
            {
                if (_IsLoading3 != value)
                {
                    _IsLoading3 = value;
                    NotifyOfPropertyChange(() => IsLoading3);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        
        private bool _IsLoading = false;
        public bool IsLoading
        {
            get
            {
                return _IsLoading1 || _IsLoading2 || _IsLoading3 ;
            }
            set
            {
                //if (_IsLoading != value)
                {
                    _IsLoading = _IsLoading1 || _IsLoading2 || _IsLoading3 ;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

#region properties

        private Nullable<DateTime> _logDate;//=DateTime.Now;
        public Nullable<DateTime> logDate
        {
            get
            {
                return _logDate;
            }
            set
            {
                if (_logDate == value)
                    return;
                _logDate = value;
                NotifyOfPropertyChange(() => logDate);
                if (!chkChoice)
                {
                    searchUserLoginHistory.HostName = "";
                    searchUserLoginHistory.AccountID = 0;
                }
                searchUserLoginHistory.LogDateTime = logDate;
                allUserLoginHistory.PageIndex = 0;
                GetAllUserLoginHistoryPaging(searchUserLoginHistory, allUserLoginHistory.PageSize, allUserLoginHistory.PageIndex, "", true);
            }
        }

        private ObservableCollection<UserLoginHistory> _allAccount;
        public ObservableCollection<UserLoginHistory> allAccount
        {
            get
            {
                return _allAccount;
            }
            set
            {
                if (_allAccount == value)
                    return;
                _allAccount = value;
                NotifyOfPropertyChange(() => allAccount);
            }
        }
        private ObservableCollection<UserLoginHistory> _allHostName;
        public ObservableCollection<UserLoginHistory> allHostName
        {
            get
            {
                return _allHostName;
            }
            set
            {
                if (_allHostName == value)
                    return;
                _allHostName = value;
                NotifyOfPropertyChange(() => allHostName);
            }
        }

        private UserLoginHistory _SelectedHostName;
        public UserLoginHistory SelectedHostName
        {
            get
            {
                return _SelectedHostName;
            }
            set
            {
                if (_SelectedHostName == value)
                    return;
                _SelectedHostName = value;
                NotifyOfPropertyChange(() => SelectedHostName);
                searchUserLoginHistory.HostName = SelectedHostName.HostName;
                if (!chkChoice)
                {
                    searchUserLoginHistory.LogDateTime = null;
                    searchUserLoginHistory.AccountID = 0;
                }
                
                allUserLoginHistory.PageIndex = 0;
                GetAllUserLoginHistoryPaging(searchUserLoginHistory, allUserLoginHistory.PageSize, allUserLoginHistory.PageIndex, "", true);
            }
        }
        private UserLoginHistory _SelectedAccount;
        public UserLoginHistory SelectedAccount
        {
            get
            {
                return _SelectedAccount;
            }
            set
            {
                if (_SelectedAccount == value)
                    return;
                _SelectedAccount = value;
                NotifyOfPropertyChange(() => SelectedAccount);
                searchUserLoginHistory.AccountID = SelectedAccount.AccountID;
                if (!chkChoice)
                {
                    searchUserLoginHistory.HostName = "";
                    searchUserLoginHistory.LogDateTime = null;
                }
                allUserLoginHistory.PageIndex = 0;
                GetAllUserLoginHistoryPaging(searchUserLoginHistory, allUserLoginHistory.PageSize, allUserLoginHistory.PageIndex, "", true);
            }
        }

        private UserLoginHistory _searchUserLoginHistory;
        public UserLoginHistory searchUserLoginHistory
        {
            get
            {
                return _searchUserLoginHistory;
            }
            set
            {
                if (_searchUserLoginHistory == value)
                    return;
                _searchUserLoginHistory = value;
                NotifyOfPropertyChange(() => searchUserLoginHistory);
            }
        }

        private PagedSortableCollectionView<UserLoginHistory> _allUserLoginHistory;
        public PagedSortableCollectionView<UserLoginHistory> allUserLoginHistory
        {
            get
            {
                return _allUserLoginHistory;
            }
            set
            {
                if (_allUserLoginHistory == value)
                    return;
                _allUserLoginHistory = value;
                NotifyOfPropertyChange(() => allUserLoginHistory);
            }
        }
        

#endregion

        public bool chkChoice { get; set; }

        public void lnkDeleteClick(object sender, RoutedEvent e)
        {
            //DeleteGroupRoles(SelectedGroupRole.GroupRoleID);
        }
#region method
        private void GetUserLogHisGByUserAccount()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading1 = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUserLogHisGByUserAccount(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetUserLogHisGByUserAccount(asyncResult);
                            if (results != null)
                            {
                                allAccount = new ObservableCollection<UserLoginHistory>();
                                UserLoginHistory ulh=new UserLoginHistory();
                                ulh.UserAccount=new UserAccount();
                                ulh.AccountID = -1;
                                ulh.UserAccount.AccountName = "--Hãy chọn một tài khoản--";
                                SelectedAccount = ulh;
                                allAccount.Add(ulh);
                                if (!Globals.isEncrypt)
                                {
                                    foreach (var p in results)
                                    {
                                        allAccount.Add(p);
                                    }
                                }
                                else
                                {
                                    foreach (var p in results)
                                    {
                                        try
                                        {
                                            //if (Globals.isEncrypt)
                                            {
                                                p.UserAccount.AccountName = EncryptExtension.Decrypt(p.UserAccount.AccountName
                                                , Globals.AxonKey, Globals.AxonPass);
                                                if (p.UserAccount.AccountName == "")
                                                {
                                                    continue;
                                                }
                                            }
                                            allAccount.Add(p);
                                        }
                                        catch { }
                                    }
                                }
                                
                                NotifyOfPropertyChange(() => allAccount);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading1 = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void GetUserLogHisGByHostName()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading2 = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUserLogHisGByHostName(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetUserLogHisGByHostName(asyncResult);
                            if (results != null)
                            {
                                allHostName = new ObservableCollection<UserLoginHistory>();
                                UserLoginHistory ulh=new UserLoginHistory();
                                ulh.HostName = "";
                                ulh.tempHostName = "--Hãy chọn một host--";
                                SelectedHostName = ulh;
                                allHostName.Add(ulh);    
                                foreach (var p in results)
                                {
                                    try
                                    {
                                        //if(Globals.isEncrypt)
                                        //{
                                        //    p.UserAccount.AccountName = EncryptExtension.Decrypt(p.UserAccount.AccountName
                                        //        , Globals.AxonKey, Globals.AxonPass);    
                                        //}
                                        p.tempHostName = p.HostName;
                                        allHostName.Add(p);    
                                    }
                                    catch{}
                                }
                                NotifyOfPropertyChange(() => allHostName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;\
                            IsLoading2 = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void GetAllUserLoginHistoryPaging(UserLoginHistory ulh
                                        , int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading3 = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllUserLoginHistoryPaging(ulh, PageSize, PageIndex, OrderBy, CountTotal
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var results = contract.EndGetAllUserLoginHistoryPaging(out total, asyncResult);
                            if (results != null)
                            {
                                if (allUserLoginHistory == null)
                                {
                                    allUserLoginHistory = new PagedSortableCollectionView<UserLoginHistory>();
                                }
                                else
                                {
                                    allUserLoginHistory.Clear();
                                }
                                foreach (var p in results)
                                {
                                    try
                                    {
                                        if(Globals.isEncrypt)
                                        {
                                            p.UserAccount.AccountName = EncryptExtension.Decrypt(p.UserAccount.AccountName
                                                , Globals.AxonKey, Globals.AxonPass);    
                                        }
                                        
                                    }
                                    catch
                                    {
                                        p.UserAccount.AccountName = "Chưa Encrypt";
                                    }
                                    allUserLoginHistory.Add(p);
                                }
                                allUserLoginHistory.TotalItemCount = total;
                                NotifyOfPropertyChange(() => allUserLoginHistory);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading3 = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        
#endregion

    }
}
