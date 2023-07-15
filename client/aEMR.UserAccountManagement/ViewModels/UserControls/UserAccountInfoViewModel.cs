using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using System.ComponentModel;
using aEMR.Common.Utilities;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

/*
 * #001 20180921 TNHX: Apply BusyIndicator, Fix Change password, remove unnecessary code, refactor code
 */
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IUserAccountInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserAccountInfoViewModel : Conductor<object>, IUserAccountInfo, IHandle<allStaffChangeEvent>
    {
        private long groupID = 0;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UserAccountInfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            /*▼====: #001*/
            //_allUserAccount =new PagedSortableCollectionView<UserAccount>();
            //_allUserAccount.PageIndex = 0;

            //GetAllUserName(1000, allUserAccount.PageIndex, "", true);
            //curUserAccount.Staff.SStreetAddress
            
            //GetAllRefStaffCategories();
            //_allUserAccount.OnRefresh += new EventHandler<Common.Collections.RefreshEventArgs>(_allUserAccount_OnRefresh);
            curUserAccount = Globals.LoggedUserAccount;
            if (curUserAccount.Staff != null)
                GetStaffByID((long)curUserAccount.StaffID);
            /*▲====: #001*/
        }

        void _allUserAccount_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //_allUserAccount = new PagedSortableCollectionView<UserAccount>();
            //_allUserAccount.PageIndex = 0;
            
            //_allUserAccount.OnRefresh+=new EventHandler<RefreshEventArgs>(_allUserAccount_OnRefresh);
            //allUserGroup = new ObservableCollection<UserGroup>();
            //==== 20161206 CMN End.
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
                NotifyOfPropertyChange(()=>curUserAccount);
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

#endregion
        
        public bool CheckValid(object temp)
        {
            UserAccount p = temp as UserAccount;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }

        public bool CheckAccount(string Name)
        {
            if(Name.Contains(" "))
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
        
        public void butClear()
        {
            curUserAccount=new UserAccount();
        }
        
        public void txtUserNameLostFocus(object sender)
        {
            CheckAccount(((TextBox)sender).Text);
        }

        public void txtUserNameKeyUp(object sender,KeyEventArgs e)
        {
            //if ((long)e.Key > 127)
            //    e.Handled = true;
        }

        public bool CheckExist()
        {
            if (allOldUserGroup!= null)
            {
                foreach (var UG in allOldUserGroup)
                {
                    if (UG.AccountID == SelectedUserAccount.AccountID
                        && UG.GroupID == SelectedGroup.GroupID)
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
                    if (UG.AccountID == uG.UserAccount.AccountID
                        && UG.GroupID == uG.Group.GroupID)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1760_G1_ThemMoiNgDungVaoNhom, "");
                        return false;
                    }
                }
            }

            return true;
        }

        public object PerImage { get; set; }

        public void PersonImageLoaded(object sender, RoutedEventArgs e)
        {
            PerImage = sender as Image;
            if(curUserAccount.Staff.PImage!=null)
            {
                ImageLoaded();
            }
            else
            {
                Uri uri = new Uri("/eHCMSCal;component/Assets/Images/Anonymous.png", UriKind.Relative);
                ImageSource img = new BitmapImage(uri);
                ((Image)PerImage).SetValue(Image.SourceProperty, img);    
            }
        }

        public void ImageLoaded()
        {
            Image image = new Image();
            BitmapImage bitmapimage = new BitmapImage();
            MemoryStream stream = new MemoryStream(curUserAccount.Staff.PImage);
            //bitmapimage.SetSource(stream);
            //((Image)PerImage).Source = bitmapimage;
        }

        public void btAddChoose()
        {
            if (SelectedGroup!=null
                && SelectedUserAccount!=null)
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
       
        public void lnkDeleteUGClick(object sender, RoutedEvent e)
        {
            allOldUserGroup.Remove(selectedUserGroup);
            allUserGroup.Remove(selectedUserGroup);
        }

        public void btUndo()
        {
            if (allUserGroup.Count>0)
            {
                allOldUserGroup.Remove(allUserGroup[allUserGroup.Count - 1]);
                allUserGroup.RemoveAt(allUserGroup.Count-1);    
            }
        }

        public void butUpdate()
        {
            //var StaffVM = Globals.GetViewModel<IStaffEdit>();
            //StaffVM.curStaff = Common.ObjectCopier.DeepCopy(curUserAccount.Staff);
            //var instance = StaffVM as Conductor<object>;

            //this.ActivateItem(StaffVM);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IStaffEdit> onInitDlg = (StaffVM) =>
            {
                StaffVM.curStaff = Common.ObjectCopier.DeepCopy(curUserAccount.Staff);
                this.ActivateItem(StaffVM);
            };
            GlobalsNAV.ShowDialog<IStaffEdit>(onInitDlg);
        }

        /*▼====: #001*/
        public void TxtOldPassword_Loaded(object sender, RoutedEventArgs e)
        {
            TxtOldPassword = sender as PasswordBox;
        }
        public PasswordBox TxtOldPassword { get; set; }

        public void TxtPassword_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPassword = sender as PasswordBox;
        }
        public PasswordBox TxtPassword { get; set; }

        public void TxtConfirm_Loaded(object sender, RoutedEventArgs e)
        {
            TxtConfirm = sender as PasswordBox;
        }
        public PasswordBox TxtConfirm { get; set; }

        public void butSave()
        {
            string AccountPassword = Common.EncryptExtension.Encrypt(TxtOldPassword.Password, Globals.AxonKey, Globals.AxonPass);
            if (AccountPassword != curUserAccount.AccountPassword)
            {
                MessageBox.Show(eHCMSResources.A0877_G1_Msg_InfoSaiPassCu);
                return;
            }
            if (TxtPassword.Password != TxtConfirm.Password)
            {
                MessageBox.Show(eHCMSResources.K0464_G1_MatKhauSai);
            }

            string AccountPass = Common.EncryptExtension.Encrypt(TxtPassword.Password,
                                                                Globals.AxonKey, Globals.AxonPass);

            UpdateUserAccount((int)curUserAccount.AccountID
                            , (long)curUserAccount.StaffID
                            , ""
                            , AccountPass
                            , true);
        }

        #region method   
        private void GetStaffByID(long StaffID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetStaffByID(StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                curUserAccount.Staff = contract.EndGetStaffByID(asyncResult);
                                NotifyOfPropertyChange(() => curUserAccount.Staff);
                                if (curUserAccount.Staff.PImage != null)
                                {
                                    ImageLoaded();
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

        private void UpdateUserAccount(int AccountID, long StaffID, string AccountName, string AccountPassword, bool IsActivated)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                                curUserAccount.AccountPassword = AccountPassword;
                                    TxtConfirm.Password = "";
                                    TxtOldPassword.Password = "";
                                    TxtPassword.Password = "";
                                    NotifyOfPropertyChange(() => TxtConfirm);
                                    NotifyOfPropertyChange(() => TxtOldPassword);
                                    NotifyOfPropertyChange(() => TxtPassword);
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
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

        public void Handle(allStaffChangeEvent obj)
        {
            //if (obj != null)
            {
                GetStaffByID((long)curUserAccount.StaffID);
            }
        }
    }
}
