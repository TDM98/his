using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
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
using System.Collections.ObjectModel;
using eHCMSCommon.Utilities;
using System.Linq;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAucHoldUserAccount)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AucHoldUserAccountViewModel : Conductor<object>, IAucHoldUserAccount
        
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AucHoldUserAccountViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);
            GetAllUserName(1000, 0, "", true);
        }

        private long _AccountID;
        public long AccountID
        {
            get { return _AccountID; }
            set
            {
                _AccountID = value;
                NotifyOfPropertyChange(() => AccountID);
            }
        }

        private bool _IsProcessing;
        public bool IsProcessing
        {
            get { return _IsProcessing; }
            set
            {
                _IsProcessing = value;
                NotifyOfPropertyChange(() => IsProcessing);
            }
        }

        #region nhan vien
        private ObservableCollection<UserAccount> _UserAccount;
        public ObservableCollection<UserAccount> UserAccount
        {
            get
            {
                return _UserAccount;
            }
            set
            {
                if (_UserAccount != value)
                {
                    _UserAccount = value;
                    NotifyOfPropertyChange(() => UserAccount);
                }
            }
        }

        private ObservableCollection<UserAccount> _allNewUserAccount;
        public ObservableCollection<UserAccount> allNewUserAccount
        {
            get
            {
                return _allNewUserAccount;
            }
            set
            {
                if (_allNewUserAccount == value)
                    return;
                _allNewUserAccount = value;
                NotifyOfPropertyChange(() => allNewUserAccount);
            }
        }

        //private void SearchUserAccount(string SearchKeys)
        //{
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new UserAccountsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetAllUserGroupGetByID(SearchKeys, 0, Globals.PageSize, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    var results = contract.EndSearchStaffFullName(asyncResult);
        //                    if (results != null)
        //                    {
        //                        UserAccount = new ObservableCollection<Staff>();
        //                        foreach (Staff p in results)
        //                        {
        //                            UserAccount.Add(p);
        //                        }
        //                        NotifyOfPropertyChange(() => UserAccount);
        //                    }
        //                    AucHoldUserAccount.ItemsSource = UserAccount;
        //                    AucHoldUserAccount.PopulateComplete();
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}
        private void PagingLinq(string AccountName)
        {
            allNewUserAccount = UserAccount.Where(item => item.AccountName.ToUpper().Contains(AccountName.ToUpper())).ToObservableCollection();
            AucHoldUserAccount.ItemsSource = allNewUserAccount;
            AucHoldUserAccount.PopulateComplete();
        }
        public void setDefault() 
        {
            AccountID = 0;
            AucHoldUserAccount.Text = "";
        }
        

        AutoCompleteBox AucHoldUserAccount;

        public void AucHoldUserAccount_Loaded(object sender, RoutedEventArgs e)
        {
            AucHoldUserAccount = sender as AutoCompleteBox;
        }

        public void AucHoldUserAccount_Populating(object sender, PopulatingEventArgs e)
        {
            PagingLinq(e.Parameter);
            //GetAllUserAccountsPaging(e.Parameter, 1000,0, "", true);
        }

        public void AucHoldUserAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AucHoldUserAccount.SelectedItem != null)
            {
                AccountID = (AucHoldUserAccount.SelectedItem as UserAccount).AccountID;
            }
            else
            {
                AccountID = 0;
            }
        }

        private void GetAllUserName(int PageSize, int PageIndex, string OrderBy,
                            bool CountTotal)
        {
            IsProcessing = true;
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllUserAccountsPaging(null,10000, PageIndex, OrderBy,
                             CountTotal, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     int Total = 0;
                                     UserAccount = new ObservableCollection<UserAccount>();
                                     var results = contract.EndGetAllUserAccountsPaging(out Total, asyncResult);
                                     if (results != null && results.Count>0)
                                     {
                                         foreach (var p in results)
                                         {
                                             try
                                             {
                                                 {
                                                     p.AccountName = EncryptExtension.Decrypt(p.AccountName, Globals.AxonKey, Globals.AxonPass);
                                                     if (p.AccountName == "")
                                                     {
                                                         //p.AccountName = eHCMSResources.Z1162_G1_ChuaEncrypt;
                                                         continue;
                                                     }
                                                 }
                                             }
                                             catch
                                             {
                                                 p.AccountName = eHCMSResources.Z1162_G1_ChuaEncrypt;
                                             }
                                             UserAccount.Add(p);
                                         }

                                         NotifyOfPropertyChange(() => UserAccount);
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                     Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                 }
                                 finally
                                 {
                                     //Globals.IsBusy = false;
                                     IsProcessing = false;
                                     Globals.EventAggregator.Publish(new GetAllUserNameCompletedEvent());
                                 }

                             }), null);

                }

            });

            t.Start();
        }

        private void GetAllUserAccountsPaging(string AccountName, int PageSize, int PageIndex, string OrderBy,
                            bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllUserAccountsPaging(EncryptExtension.Encrypt(AccountName, Globals.AxonKey, Globals.AxonPass), 10000, PageIndex, OrderBy,
                             CountTotal, Globals.DispatchCallback((asyncResult) =>
                             {
                                 try
                                 {
                                     int Total = 0;
                                     allNewUserAccount = new ObservableCollection<UserAccount>();
                                     var results = contract.EndGetAllUserAccountsPaging(out Total, asyncResult);
                                     if (results != null && results.Count > 0)
                                     {
                                         foreach (var p in results)
                                         {
                                             try
                                             {
                                                 {
                                                     p.AccountName = EncryptExtension.Decrypt(p.AccountName, Globals.AxonKey, Globals.AxonPass);
                                                     if (p.AccountName == "")
                                                     {
                                                         continue;
                                                     }
                                                 }
                                             }
                                             catch
                                             {
                                                 p.AccountName = eHCMSResources.Z1162_G1_ChuaEncrypt;
                                             }
                                             allNewUserAccount.Add(p);
                                         }

                                         NotifyOfPropertyChange(() => UserAccount);
                                     }
                                 }
                                 catch (Exception ex)
                                 {
                                     Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                 }
                                 finally
                                 {
                                     //Globals.IsBusy = false;
                                     Globals.EventAggregator.Publish(new GetAllUserNameCompletedEvent());
                                 }

                             }), null);

                }

            });

            t.Start();
        }

        #endregion
    }
}
