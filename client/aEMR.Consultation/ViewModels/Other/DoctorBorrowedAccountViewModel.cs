using eHCMSLanguage;
using System;
using System.Net;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Common.BaseModel;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.Controls;
using System.Linq;
using System.Collections.Generic;
/*
* 20210713 #001 TNHX: 260 Truyền biến để hiển thị chọn bsi mượn
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IDoctorBorrowedAccount)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DoctorBorrowedAccountViewModel : ViewModelBase, IDoctorBorrowedAccount
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DoctorBorrowedAccountViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            Innitailize();
        }

        public void Innitailize()
        {
            CurUserOfficialHistory = new UserOfficialHistory
            {
                LoggedAccountID = (long)Globals.LoggedUserAccount.StaffID,
                LoggedHistoryID = Globals.LoggedUserAccount.LoggedHistoryID
            };
            ListManagementUserOfficial = new ObservableCollection<ManagementUserOfficial>();
            ListManagementUserOfficialID = "";
            allUserOfficialHistory = new PagedSortableCollectionView<UserOfficialHistory>();
            allUserOfficialHistory.PageSize = 100;
            allUserOfficialHistory.PageIndex = 0;
            allUserOfficialHistory.OnRefresh += new EventHandler<RefreshEventArgs>(allUserOfficialHistory_OnRefresh);
            GetManagementUserOfficialPaging((long)Globals.LoggedUserAccount.StaffID, 0
                , 0 , 1000, true);
        }

        void allUserOfficialHistory_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetUserOfficialHistoryPaging((long)Globals.LoggedUserAccount.StaffID, allUserOfficialHistory.PageIndex
                , allUserOfficialHistory.PageSize, true);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        #region Properties
        private int _PatientFindBy = 2;
        public int PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                if (_PatientFindBy == value)
                    return;
                _PatientFindBy = value;
                NotifyOfPropertyChange(() => PatientFindBy);
            }
        }

        AxAutoComplete cboUserOfficial { get; set; }
        
        private List<Staff> _AllStaffs;
        public List<Staff> AllStaffs
        {
            get
            {
                return _AllStaffs;
            }
            set
            {
                if (_AllStaffs == value)
                    return;
                _AllStaffs = value;
                NotifyOfPropertyChange(() => AllStaffs);
            }
        }

        private PagedSortableCollectionView<UserOfficialHistory> _allUserOfficialHistory;
        public PagedSortableCollectionView<UserOfficialHistory> allUserOfficialHistory
        {
            get
            {
                return _allUserOfficialHistory;
            }
            set
            {
                if (_allUserOfficialHistory == value)
                    return;
                _allUserOfficialHistory = value;
                NotifyOfPropertyChange(() => allUserOfficialHistory);
            }
        }

        private UserOfficialHistory _CurUserOfficialHistory;
        public UserOfficialHistory CurUserOfficialHistory
        {
            get
            {
                return _CurUserOfficialHistory;
            }
            set
            {
                if (_CurUserOfficialHistory == value)
                    return;
                _CurUserOfficialHistory = value;
                NotifyOfPropertyChange(() => CurUserOfficialHistory);
            }
        }

        private bool _IsPopupView;
        public bool IsPopupView
        {
            get
            {
                return _IsPopupView;
            }
            set
            {
                if (_IsPopupView == value)
                    return;
                _IsPopupView = value;
                NotifyOfPropertyChange(() => IsPopupView);
            }
        }

        private string ListManagementUserOfficialID { get; set; }
        
        private ObservableCollection<ManagementUserOfficial> _listManagementUserOfficial;
        public ObservableCollection<ManagementUserOfficial> ListManagementUserOfficial
        {
            get
            {
                return _listManagementUserOfficial;
            }
            set
            {
                if (_listManagementUserOfficial == value)
                    return;
                _listManagementUserOfficial = value;
                NotifyOfPropertyChange(() => ListManagementUserOfficial);
            }
        }
        #endregion

        #region Methods
        public void butAddNew()
        {
            if (CurUserOfficialHistory.OfficialAccountID != 0)
            {                
                if (IsPopupView)
                {
                    this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                }
                else
                {
                    this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
                }
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new UserManagementServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginAddUserOfficialHistory(CurUserOfficialHistory, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndAddUserOfficialHistory(asyncResult);
                                    if (results > 0)
                                    {
                                        MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                        CurUserOfficialHistory.UOHistoryID = results;
                                        if (Globals.DoctorAccountBorrowed == null)
                                        {
                                            Globals.DoctorAccountBorrowed = new UserAccount();
                                            Globals.DoctorAccountBorrowed.Staff = new Staff { StaffID = CurUserOfficialHistory.OfficialAccountID, FullName = CurUserOfficialHistory.OfficialAccount.FullName };
                                            Globals.DoctorAccountBorrowed.StaffID = CurUserOfficialHistory.OfficialAccountID;
                                        }
                                        else
                                        {
                                            Globals.DoctorAccountBorrowed.Staff = new Staff { StaffID = CurUserOfficialHistory.OfficialAccountID, FullName = CurUserOfficialHistory.OfficialAccount.FullName };
                                            Globals.DoctorAccountBorrowed.StaffID = CurUserOfficialHistory.OfficialAccountID;
                                        }
                                    }
                                    GetUserOfficialHistoryPaging((long)Globals.LoggedUserAccount.StaffID, allUserOfficialHistory.PageIndex
                                        , allUserOfficialHistory.PageSize, true);
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    if (IsPopupView)
                                    {
                                        this.DlgHideBusyIndicator();
                                    }
                                    else
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }
                            }), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (IsPopupView)
                        {
                            this.DlgHideBusyIndicator();
                        }
                        else
                        {
                            this.HideBusyIndicator();
                        }
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    }
                });

                t.Start();
            }
            else
            {
                Globals.ShowMessage("Chưa chọn Bác sĩ", eHCMSResources.T0432_G1_Error);
            }
        }

        public void butCancel()
        {
            TryClose();
        }

        public void lnkDeleteClick(object sender, RoutedEventArgs e)
        {
            //if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //    DeleteUserOfficialHistory();
            //}
        }

        public void lnkRefreshClick(object sender, RoutedEventArgs e)
        {
            allUserOfficialHistory.PageSize = 10;
            allUserOfficialHistory.PageIndex = 0;
            GetUserOfficialHistoryPaging((long)Globals.LoggedUserAccount.StaffID, allUserOfficialHistory.PageIndex
                , allUserOfficialHistory.PageSize, true);
        }

        private void GetUserOfficialHistoryPaging(long LoggedStaffID, int PageIndex, int PageSize, bool bCountTotal)
        {
            if (IsPopupView)
            {
                this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetUserOfficialHistoryPaging(LoggedStaffID, PageIndex, PageSize, bCountTotal
                            , Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var results = contract.EndGetUserOfficialHistoryPaging(out int total, asyncResult);
                                 allUserOfficialHistory.Clear();
                                 if (results != null)
                                 {
                                     foreach (var p in results)
                                     {
                                         allUserOfficialHistory.Add(p);
                                     }
                                 }
                                 allUserOfficialHistory.TotalItemCount = total;
                             }
                             catch (Exception ex)
                             {
                                 Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                             }
                             finally
                             {
                                 if (IsPopupView)
                                 {
                                     this.DlgHideBusyIndicator();
                                 }
                                 else
                                 {
                                     this.HideBusyIndicator();
                                 }
                             }
                         }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    if (IsPopupView)
                    {
                        this.DlgHideBusyIndicator();
                    }
                    else
                    {
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }

        //▼====: #001
        public void grdUserOfficialHistory_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            UserOfficialHistory item = e.Row.DataContext as UserOfficialHistory;
            if (item == null)
            {
                return;
            }
            if (item.LoggedHistoryID != Globals.LoggedUserAccount.LoggedHistoryID)
            {
                e.Row.Background = new SolidColorBrush(Colors.LightGray);
            }
        }
        //▲====: #001

        private void GetManagementUserOfficialPaging(long LoginUserID, long UserOfficialID
            , int PageIndex, int PageSize, bool bCountTotal)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetManagementUserOfficialPaging(LoginUserID, UserOfficialID, PageIndex
                            , PageSize, bCountTotal
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetManagementUserOfficialPaging(out int total, asyncResult);
                                    ListManagementUserOfficial.Clear();
                                    if (results != null)
                                    {
                                        ListManagementUserOfficial = results.ToObservableCollection();
                                        foreach (var item in results)
                                        {
                                            if ((item.PatientFindBy == 2 || item.PatientFindBy == PatientFindBy)
                                                && item.FromDate < Globals.GetCurServerDateTime()
                                                && item.ToDate > Globals.GetCurServerDateTime()
                                                && !item.IsDeleted)
                                            {
                                                ListManagementUserOfficialID += item.UserOfficialID + ";";
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void UserOfficial_Loaded(object sender, RoutedEventArgs e)
        {
            cboUserOfficial = (AxAutoComplete)sender;
        }

        public void UserOfficial_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)
                && ListManagementUserOfficialID.Contains(x.StaffID.ToString())
                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText));
            (sender as AxAutoComplete).PopulateComplete();
        }

        public void UserOfficial_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {

            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                CurUserOfficialHistory.OfficialAccountID = 0;
                return;
            }
            CurUserOfficialHistory.OfficialAccountID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            CurUserOfficialHistory.OfficialAccount = ((sender as AxAutoComplete).SelectedItem as Staff);
        }
        #endregion
    }
}
