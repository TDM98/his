using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common;


namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IcwdUserAccountUpdate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdUserAccountUpdateViewModel : Conductor<object>, IcwdUserAccountUpdate
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public cwdUserAccountUpdateViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void  OnActivate()
        {
 	         base.OnActivate();
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
            }
        }

        private ObservableCollection<UserAccount> _allUserName;
        public ObservableCollection<UserAccount> allUserName
        {
            get
            {
                return _allUserName;
            }
            set
            {
                if (_allUserName == value)
                    return;
                _allUserName = value;
                NotifyOfPropertyChange(() => allUserName);
            }
        }

        public void butUpdate()
        {
            if (!CheckAccount(SelectedUserAccount.AccountName))
            {
                return;
            }
            //if (CheckValid(SelectedUserAccount))
            {
                string AccountName = SelectedUserAccount.AccountName;
                string AccountPassword = SelectedUserAccount.AccountPassword;
                if (Globals.isEncrypt)
                {
                    AccountName = EncryptExtension.Encrypt(SelectedUserAccount.AccountName.ToUpper(),
                                                                      Globals.AxonKey, Globals.AxonPass);
                    AccountPassword = EncryptExtension.Encrypt(SelectedUserAccount.AccountPassword,
                                                                      Globals.AxonKey, Globals.AxonPass);
                }
                UpdateUserAccount((int)SelectedUserAccount.AccountID
                                , (long)SelectedUserAccount.StaffID
                                , AccountName
                                , null
                                , true);
            }

            

            Globals.EventAggregator.Publish(new allStaffChangeEvent { });
            TryClose();
        }

        public void txtUserNameLostFocus(object sender)
        {
            //CheckAccount(((TextBox)sender).Text);
        }
        public void txtUserNameKeyUp(object sender, KeyEventArgs e)
        {
            //CheckAccount(((TextBox)sender).Text);
        }
        public bool CheckAccount(string Name)
        {
            if (Name.Contains(" "))
            {
                MessageBox.Show("Account Name không được chứa khoảng trắng.");
                return false;
            }
            foreach (var UserName in allUserName)
            {
                if (UserName.AccountName.ToUpper().Equals(Name.ToUpper()))
                {
                    MessageBox.Show("Account Name này đã tồn tại.\n (Account Name Không phân biệt chữ hoa và chữ thường!)");
                    return false;
                }
            }
            return true;
        }

        //public void ShowPwdBtn()
        //{
        //    MessageBox.Show("Hello");
        //}

        public void OnAccNameClickShowPwd(object source, RoutedEventArgs args)
        {
            if (SelectedUserAccount != null)
            {
                string strDecryptedPwd = EncryptExtension.Decrypt(SelectedUserAccount.AccountPassword, Globals.AxonKey, Globals.AxonPass);
                MessageBox.Show(strDecryptedPwd);
            }
        }

        public void butCancel()
        {
            TryClose();
        }
        public void butResetPass()
        {
            string AccountPass = EncryptExtension.Encrypt("1",
                                                                Globals.AxonKey, Globals.AxonPass);

            UpdateUserAccount((int)SelectedUserAccount.AccountID
                                , 1
                                , ""
                                , AccountPass
                                , true);
        }
        private void UpdateUserAccount(int AccountID, long StaffID, string AccountName, string AccountPassword, bool IsActivated)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateUserAccount(AccountID, StaffID, AccountName, AccountPassword, IsActivated, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndUpdateUserAccount(asyncResult);
                            if (results == true)
                            {
                                //Globals.ShowMessage("Thêm thành công!", eHCMSResources.T0432_G1_Error);
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
    }
}

