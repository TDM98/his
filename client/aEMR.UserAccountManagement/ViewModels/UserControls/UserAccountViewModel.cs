using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
using System.Collections.Generic;

/*
 * #001 20180922 TNHX: Apply BusyIndicator, refactor code, fix error [#0000075]
 */
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUserAccount)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserAccountViewModel : Conductor<object>, IUserAccount
        , IHandle<allStaffChangeEvent>
    {
        private long groupID = 0;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UserAccountViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            _allUserAccount = new PagedSortableCollectionView<UserAccount>();
            _allUserAccount.PageIndex = 0;
            GetAllGroupByGroupID(0);
            allUserAccount.PageSize = 1000;
            GetAllUserAccountPaging(allUserAccount.PageSize, allUserAccount.PageIndex, "", true);
            //20190318 TBL: Ham nay khong thay lam gi, de lai se lam cho chuong trinh cham
            //GetAllUserName(1000, allUserAccount.PageIndex, "", true); 
            curUserAccount = new UserAccount();
            curUserAccount.Staff = new Staff();
            GetAllRefStaffCategories();
            _allUserAccount.OnRefresh += new EventHandler<RefreshEventArgs>(_allUserAccount_OnRefresh);
        }

        void _allUserAccount_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllUserAccountPaging(allUserAccount.PageSize, allUserAccount.PageIndex, "", true);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //_allUserAccount = new PagedSortableCollectionView<UserAccount>();
            //_allUserAccount.PageIndex = 0;
            //allUserAccount.PageSize = 1000;
            //GetAllUserAccountPaging(allUserAccount.PageSize, allUserAccount.PageIndex, "", true);
            //curUserAccount = new UserAccount();
            //curUserAccount.Staff = new Staff();
            //==== 20161206 CMN End.
            _allUserAccount.OnRefresh += new EventHandler<RefreshEventArgs>(_allUserAccount_OnRefresh);
            Globals.EventAggregator.Subscribe(this);
            allUserGroup = new ObservableCollection<UserGroup>();
        }
        
        #region Properties
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
                allUserGroup=new ObservableCollection<UserGroup>();
                groupID = SelectedGroup.GroupID;
                GetAllUserGroupGetByID(0, groupID, 1000, 0, "", true);
            }
        }

        private PagedSortableCollectionView<UserAccount> _allUserAccount;
        public PagedSortableCollectionView<UserAccount> allUserAccount
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

        private ObservableCollection<UserAccount> _UserAccounts;
        public ObservableCollection<UserAccount> UserAccounts
        {
            get
            {
                return _UserAccounts;
            }
            set
            {
                if (_UserAccounts == value)
                    return;
                _UserAccounts = value;
                NotifyOfPropertyChange(() => UserAccounts);
            }
        }

        private PagedSortableCollectionView<UserAccount> _allUserName;
        public PagedSortableCollectionView<UserAccount> allUserName
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
                //SelectedUserAccount.AccountName                  
            }
        }

        private UserAccount _curUserAccount;
        public UserAccount curUserAccount
        {
            get
            {
                return _curUserAccount;
            }
            set
            {
                if (_curUserAccount == value)
                    return;
                _curUserAccount = value;
                NotifyOfPropertyChange(() => curUserAccount);
            }
        }

        private ObservableCollection<RefStaffCategory> _allRefStaffCategory;
        public ObservableCollection<RefStaffCategory> allRefStaffCategory
        {
            get
            {
                return _allRefStaffCategory;
            }
            set
            {
                if (_allRefStaffCategory == value)
                    return;
                _allRefStaffCategory = value;
                NotifyOfPropertyChange(() => allRefStaffCategory);
            }
        }

        private RefStaffCategory _SelectedRefStaffCategory;
        public RefStaffCategory SelectedRefStaffCategory
        {
            get
            {
                return _SelectedRefStaffCategory;
            }
            set
            {
                if (_SelectedRefStaffCategory == value)
                    return;
                _SelectedRefStaffCategory = value;
                NotifyOfPropertyChange(() => SelectedRefStaffCategory);
                if (SelectedRefStaffCategory != null)
                {
                    GetAllStaff(SelectedRefStaffCategory.StaffCatgID);    
                }                
            }
        }

        private ObservableCollection<Staff> _allStaff;
        public ObservableCollection<Staff> allStaff
        {
            get
            {
                return _allStaff;
            }
            set
            {
                if (_allStaff == value)
                    return;
                _allStaff = value;
                NotifyOfPropertyChange(() => allStaff);
            }
        }

        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff == value)
                    return;
                _SelectedStaff = value;
            }
        }

        private ObservableCollection<UserGroup> _allOldUserGroup;
        public ObservableCollection<UserGroup> allOldUserGroup
        {
            get
            {
                return _allOldUserGroup;
            }
            set
            {
                if (_allOldUserGroup == value)
                    return;
                _allOldUserGroup = value;
                NotifyOfPropertyChange(() => allOldUserGroup);
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

        private string _txtUserAccountName;
        public string txtUserAccountName
        {
            get
            {
                return _txtUserAccountName;
            }
            set
            {
                if (_txtUserAccountName == value)
                    return;
                _txtUserAccountName = value;
                NotifyOfPropertyChange(() => txtUserAccountName);
            }
        }

        private ObservableCollection<UserGroup> _allUserGroup;
        public ObservableCollection<UserGroup> allUserGroup
        {
            get
            {
                return _allUserGroup;
            }
            set
            {
                if (_allUserGroup == value)
                    return;
                _allUserGroup = value;
                NotifyOfPropertyChange(() => allUserGroup);
            }
        }

        /*▼====: #001*/
        public PasswordBox TxtPassword { get; set; }

        public PasswordBox TxtConfirm { get; set; }
        #endregion

        public void TxtPassword_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPassword = sender as PasswordBox;
        }
        public void TxtConfirm_Loaded(object sender, RoutedEventArgs e)
        {
            TxtConfirm = sender as PasswordBox;
        }
        /*▲====: #001*/

        public bool CheckValid(object temp)
        {
            UserAccount p = temp as UserAccount;
            if (p == null)
            {
                return false;
            }
            //return p.Validate();
            return true;
        }

        public bool CheckAccount(string Name)
        {
            if(Name.Contains(" "))
            {
                MessageBox.Show("Account Name không được chứa khoảng trắng.");
                return false;
            }
            foreach (var UserName in allUserAccount)
            {
                if (UserName.AccountName.ToUpper().Equals(Name.ToUpper()))
                {
                    MessageBox.Show("Account Name này đã tồn tại.\n (Account Name Không phân biệt chữ hoa và chữ thường!)");
                    return false;
                }
            }
            return true;
        }

        private bool Validate(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            //Se lay ngay tren server.
            DateTime today = DateTime.Now.Date;
            if (curUserAccount.AccountPassword != curUserAccount.AccountPasswordConfirm)
            {
                System.ComponentModel.DataAnnotations.ValidationResult item = new System.ComponentModel.DataAnnotations.ValidationResult("Confirm password sai!", new string[] { "Thông Báo!" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        public void butSearch()
        {
            PagingLinq(allUserAccount.PageIndex, allUserAccount.PageSize);
        }
        
        private void PagingLinq(int pIndex, int pPageSize)
        {
            var ResultAll = from p in allUserAccount.ToObservableCollection()
                            select p;
            List<UserAccount> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
            ShowItemsOnList(Items);
        }

        private void ShowItemsOnList(List<UserAccount> ObjCollect)
        {
            if (txtUserAccountName == null)
            {
                return;
            }
            if (UserAccounts != null)
            {
                UserAccounts.Clear();
            }
            foreach (UserAccount item in ObjCollect)
            {
                if (item.AccountName.ToUpper().Contains(txtUserAccountName.ToUpper()) || (item.Staff != null && Globals.RemoveVietnameseString(item.Staff.FullName).ToUpper().Contains(Globals.RemoveVietnameseString(txtUserAccountName).ToUpper())))
                {
                    UserAccounts.Add(item);
                }
            }
        }

        public void butSave()
        {
            /*▼====: #001*/
            if (curUserAccount.AccountName == null || !CheckAccount(curUserAccount.AccountName))
            {
                MessageBox.Show(eHCMSResources.Z2042_G1_NhapTenNguoiDung);
                return;
            }
            if (TxtPassword.Password != TxtConfirm.Password)
            {
                MessageBox.Show(eHCMSResources.Z2041_G1_XacNhanMKKhongDung);
                return;
            }
            /*▲====: #001*/
            if (CheckValid(curUserAccount))
            {
                ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
                
                if (!Validate(out validationResults))
                {
                    //var errorVm = Globals.GetViewModel<IValidationError>();
                    //errorVm.SetErrors(validationResults);
                    //Globals.ShowDialog(errorVm as Conductor<object>);

                    Action<IValidationError> onInitDlg = (errorVm) =>
                    {
                        errorVm.SetErrors(validationResults);
                    };
                    GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);

                    return;
                }
                if (curUserAccount.Staff == null)
                {
                    curUserAccount.Staff = new Staff();
                }
                string AccountName = curUserAccount.AccountName;
                string AccountPassword = TxtPassword.Password;
                if(Globals.isEncrypt)
                {
                    AccountName = EncryptExtension.Encrypt(curUserAccount.AccountName.ToUpper(),
                                                                      Globals.AxonKey, Globals.AxonPass);
                    AccountPassword = EncryptExtension.Encrypt(TxtPassword.Password,
                                                                      Globals.AxonKey, Globals.AxonPass);    
                }
                
                AddNewUserAccount(curUserAccount.Staff.StaffID, AccountName, AccountPassword, true);
                butClear();
            }
        }

        public void butClear()
        {
            curUserAccount = new UserAccount();
            TxtConfirm.Clear();
            TxtPassword.Clear();
        }

        public void lnkDeleteClick(object sender,RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteUserAccount((int) SelectedUserAccount.AccountID);
            }
        }

        public void lnkEditClick(object sender, RoutedEventArgs e)
        {
            //var cwdUserAccountUpdate = Globals.GetViewModel<IcwdUserAccountUpdate>();
            //cwdUserAccountUpdate.SelectedUserAccount= SelectedUserAccount;
            //cwdUserAccountUpdate.allUserName = allUserName.ToObservableCollection();
            //var instance = cwdUserAccountUpdate as Conductor<object>;
            //this.ActivateItem(cwdUserAccountUpdate);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IcwdUserAccountUpdate> onInitDlg = (cwdUserAccountUpdate) =>
            {
                cwdUserAccountUpdate.SelectedUserAccount = SelectedUserAccount;
                cwdUserAccountUpdate.allUserName = allUserAccount.ToObservableCollection();
                this.ActivateItem(cwdUserAccountUpdate);
            };
            GlobalsNAV.ShowDialog<IcwdUserAccountUpdate>(onInitDlg);
        }

        public void txtUserNameLostFocus(object sender)
        {
            CheckAccount(((TextBox)sender).Text);
        }

        public void txtUserAccountNameLostFocus(TextBox sender)
        {
            txtUserAccountName = sender.Text;
            butSearch();
        }

        public void txtUserNameKeyUp(object sender,KeyEventArgs e)
        {
            //if ((long)e.Key > 127)
            //    e.Handled = true;
        }

        public bool CheckExist()
        {
            if (allOldUserGroup != null)
            {
                foreach (var UG in allOldUserGroup)
                {
                    if (UG.AccountID == SelectedUserAccount.AccountID && UG.GroupID == SelectedGroup.GroupID)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1760_G1_ThemMoiNgDungVaoNhom, "");
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CheckExist(ObservableCollection<UserGroup> lstUG, UserGroup uG)
        {
            if (lstUG != null)
            {
                foreach (var UG in lstUG)
                {
                    if (UG.AccountID == uG.UserAccount.AccountID && UG.GroupID == uG.Group.GroupID)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1760_G1_ThemMoiNgDungVaoNhom, "");
                        return false;
                    }
                }
            }

            return true;
        }

        public void btAddChoose()
        {
            if (SelectedGroup != null && SelectedUserAccount != null)
            {
                if (CheckExist())
                {
                    UserGroup UG = new UserGroup();
                    UG.Group = new Group();
                    UG.UserAccount = new UserAccount();
                    UG.Group = SelectedGroup;
                    UG.UserAccount = SelectedUserAccount;
                    UG.GroupID = SelectedGroup.GroupID;
                    UG.AccountID = SelectedUserAccount.AccountID;
                    if (CheckExist(allUserGroup, UG))
                    {
                        allUserGroup.Add(UG);
                    }
                }
            }
        }

        public void butSaveUserGroup()
        {
            if (allUserGroup != null && allUserGroup.Count > 0)
            {
                for (int i = 0; i < allUserGroup.Count; i++)
                {
                    AddNewUserGroup(allUserGroup[i].UserAccount.AccountID, allUserGroup[i].Group.GroupID);
                }
            }
        }

        public void lnkDeleteUGClick(object sender, RoutedEvent e)
        {
            allOldUserGroup.Remove(selectedUserGroup);
            allUserGroup.Remove(selectedUserGroup);
        }

        public void btUndo()
        {
            if (allUserGroup.Count > 0)
            {
                allOldUserGroup.Remove(allUserGroup[allUserGroup.Count - 1]);
                allUserGroup.RemoveAt(allUserGroup.Count - 1);    
            }
        }

        #region method
        /*▼====: #001*/
        private void GetAllUserAccountPaging(int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllUserAccountsPaging(null, PageSize, PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var results = contract.EndGetAllUserAccountsPaging(out int Total, asyncResult);
                                 if (results != null)
                                 {
                                     if (allUserAccount == null)
                                     {
                                         allUserAccount = new PagedSortableCollectionView<UserAccount>();
                                     }
                                     else
                                     {
                                         allUserAccount.Clear();
                                     }
                                     if (!Globals.isEncrypt)
                                     {
                                         foreach (var p in results)
                                         {
                                             allUserAccount.Add(p);
                                         }
                                     }
                                     else
                                     {
                                         foreach (var p in results)
                                         {
                                             try
                                             {
                                                 {
                                                     p.AccountName = EncryptExtension.Decrypt(p.AccountName, Globals.AxonKey, Globals.AxonPass);
                                                     if (p.AccountName == "")
                                                     {
                                                         //p.AccountName = "Chưa Encrypt";
                                                         continue;
                                                     }
                                                 }
                                             }
                                             catch
                                             {
                                                 p.AccountName = "Chưa Encrypt";
                                             }
                                             allUserAccount.Add(p);
                                         }
                                     }
                                     UserAccounts = allUserAccount.ToObservableCollection();
                                     NotifyOfPropertyChange(() => UserAccounts);
                                     allUserAccount.TotalItemCount = Total;
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

        private void GetAllUserName(int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllUserAccountsPaging(null, 1000, PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllUserAccountsPaging(out int Total, asyncResult);
                                if (results != null)
                                {
                                    if (allUserName == null)
                                    {
                                        allUserName = new PagedSortableCollectionView<UserAccount>();
                                    }
                                    else
                                    {
                                        allUserName.Clear();
                                    }
                                    if (!Globals.isEncrypt)
                                    {
                                        foreach (var p in results)
                                        {
                                            allUserName.Add(p);
                                        }
                                    }
                                    else
                                    {
                                        foreach (var p in results)
                                        {
                                            try
                                            {
                                                {
                                                    p.AccountName = EncryptExtension.Decrypt(p.AccountName, Globals.AxonKey, Globals.AxonPass);
                                                    if (p.AccountName == "")
                                                    {
                                                    //p.AccountName = "Chưa Encrypt";
                                                    continue;
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                p.AccountName = "Chưa Encrypt";
                                            }
                                            allUserName.Add(p);
                                        }
                                    }

                                    allUserName.TotalItemCount = Total;
                                    NotifyOfPropertyChange(() => allUserName);
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

        private void GetAllRefStaffCategories()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRefStaffCategories(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllRefStaffCategories(asyncResult);
                                if (results != null)
                                {
                                    allRefStaffCategory = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allRefStaffCategory);
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

        private void GetAllStaff(long StaffCatgID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaff(StaffCatgID, Globals.DispatchCallback((asyncResult) =>
                        {
                             try
                             {
                                 var results = contract.EndGetAllStaff(asyncResult);
                                 if (results != null)
                                 {
                                     allStaff = results.ToObservableCollection();
                                     NotifyOfPropertyChange(() => allStaff);
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

        private void AddNewUserAccount(long StaffID, string AccountName, string AccountPassword, bool IsActivated)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewUserAccount(StaffID, AccountName, AccountPassword, IsActivated, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewUserAccount(asyncResult);
                                allUserAccount.PageIndex = 0;
                                GetAllUserAccountPaging(allUserAccount.PageSize, allUserAccount.PageIndex, "", true);
                                Globals.ShowMessage(eHCMSResources.Z1761_G1_ThemMoiUserAcc, "");
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

        private void DeleteUserAccount(int AccountID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteUserAccount(AccountID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteUserAccount(asyncResult);
                                allUserAccount.PageIndex = 0;
                                GetAllUserAccountPaging(allUserAccount.PageSize, allUserAccount.PageIndex, "", true);
                                Globals.ShowMessage(eHCMSResources.Z1762_G1_XoaUserAcc, "");
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

        private void GetAllUserGroupGetByID(long AccountID, long GroupID, int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllUserGroupGetByID(AccountID, GroupID, PageSize
                            , PageIndex, OrderBy, CountTotal, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetAllUserGroupGetByID(out int Total, asyncResult);
                                    if (results != null)
                                    {
                                        allOldUserGroup = results.ToObservableCollection();
                                        NotifyOfPropertyChange(() => allOldUserGroup);
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

        private void AddNewUserGroup(long AccountID, int GroupID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewUserGroup(AccountID, GroupID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewUserGroup(asyncResult);
                                if (results == true)
                                {
                                    allUserGroup = new ObservableCollection<UserGroup>();
                                    Globals.ShowMessage(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK, "");
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

        #region Handle
        public void Handle(allStaffChangeEvent obj)
        {
            if(obj != null)
            {
                GetAllUserAccountPaging(allUserAccount.PageSize, allUserAccount.PageIndex, "", true);   
            }
        }
        #endregion
    }
}
