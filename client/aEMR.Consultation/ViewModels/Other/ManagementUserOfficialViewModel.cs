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
/*
 * 20210724 #001 TNHX: init view
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IManagementUserOfficial)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ManagementUserOfficialViewModel : ViewModelBase, IManagementUserOfficial
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ManagementUserOfficialViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            Innitailize();
        }

        public void Innitailize()
        {
            curManagementUserOfficial = new ManagementUserOfficial();
            AucHoldLoginDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            AucHoldLoginDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
            AucHoldLoginDoctor.IsDoctorOnly = true;
            AucHoldLoginDoctor.ShowStopUsing = true;

            AucHoldOfficialDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            AucHoldOfficialDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
            AucHoldOfficialDoctor.IsDoctorOnly = true;
            AucHoldOfficialDoctor.ShowStopUsing = true;

            allManagementUserOfficial = new PagedSortableCollectionView<ManagementUserOfficial>();
            allManagementUserOfficial.PageSize = 10;
            allManagementUserOfficial.PageIndex = 0;
            allManagementUserOfficial.OnRefresh += new EventHandler<RefreshEventArgs>(allManagementUserOfficial_OnRefresh);

            FromDateContent = Globals.GetViewModel<IMinHourDateControl>();
            FromDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            ToDateContent = Globals.GetViewModel<IMinHourDateControl>();
            ToDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
        }

        void allManagementUserOfficial_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetManagementUserOfficialPaging(AucHoldLoginDoctor.StaffID, AucHoldOfficialDoctor.StaffID
                , allManagementUserOfficial.PageIndex
                , allManagementUserOfficial.PageSize, true);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        private IMinHourDateControl _FromDateContent;
        public IMinHourDateControl FromDateContent
        {
            get { return _FromDateContent; }
            set
            {
                _FromDateContent = value;
                NotifyOfPropertyChange(() => FromDateContent);
            }
        }

        private IMinHourDateControl _ToDateContent;
        public IMinHourDateControl ToDateContent
        {
            get { return _ToDateContent; }
            set
            {
                _ToDateContent = value;
                NotifyOfPropertyChange(() => ToDateContent);
            }
        }

        private ManagementUserOfficial _selectedManagementUserOfficial;
        public ManagementUserOfficial SelectedManagementUserOfficial
        {
            get
            {
                return _selectedManagementUserOfficial;
            }
            set
            {
                if (_selectedManagementUserOfficial == value)
                    return;
                _selectedManagementUserOfficial = value;
                NotifyOfPropertyChange(() => SelectedManagementUserOfficial);
            }
        }

        private IAucHoldConsultDoctor _aucHoldLoginDoctor;
        public IAucHoldConsultDoctor AucHoldLoginDoctor
        {
            get
            {
                return _aucHoldLoginDoctor;
            }
            set
            {
                if (_aucHoldLoginDoctor != value)
                {
                    _aucHoldLoginDoctor = value;
                    NotifyOfPropertyChange(() => AucHoldLoginDoctor);
                }
            }
        }

        private IAucHoldConsultDoctor _aucHoldOfficialDoctor;
        public IAucHoldConsultDoctor AucHoldOfficialDoctor
        {
            get
            {
                return _aucHoldOfficialDoctor;
            }
            set
            {
                if (_aucHoldOfficialDoctor != value)
                {
                    _aucHoldOfficialDoctor = value;
                    NotifyOfPropertyChange(() => AucHoldOfficialDoctor);
                }
            }
        }

        private PagedSortableCollectionView<ManagementUserOfficial> _allManagementUserOfficial;
        public PagedSortableCollectionView<ManagementUserOfficial> allManagementUserOfficial
        {
            get
            {
                return _allManagementUserOfficial;
            }
            set
            {
                if (_allManagementUserOfficial == value)
                    return;
                _allManagementUserOfficial = value;
                NotifyOfPropertyChange(() => allManagementUserOfficial);
            }
        }

        private ManagementUserOfficial _curManagementUserOfficial;
        public ManagementUserOfficial curManagementUserOfficial
        {
            get
            {
                return _curManagementUserOfficial;
            }
            set
            {
                if (_curManagementUserOfficial == value)
                    return;
                _curManagementUserOfficial = value;
                NotifyOfPropertyChange(() => curManagementUserOfficial);
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

        public AllLookupValues.PatientFindBy PatientFindBy { get; set; }
        private bool _bIsAllChecked;
        public bool bIsAllChecked
        {
            get { return _bIsAllChecked; }
            set
            {
                _bIsAllChecked = value;
                _bIsNoiTruChecked = !_bIsAllChecked;
                _bIsNgoaiTruChecked = !_bIsAllChecked;
                NotifyOfPropertyChange(() => bIsAllChecked);
            }
        }

        private bool _bIsNgoaiTruChecked = true;
        public bool bIsNgoaiTruChecked
        {
            get { return _bIsNgoaiTruChecked; }
            set
            {
                _bIsNgoaiTruChecked = value;
                _bIsNoiTruChecked = !_bIsNgoaiTruChecked;
                _bIsAllChecked = !_bIsNgoaiTruChecked;
                NotifyOfPropertyChange(() => bIsNgoaiTruChecked);
            }
        }

        private bool _bIsNoiTruChecked;
        public bool bIsNoiTruChecked
        {
            get { return _bIsNoiTruChecked; }
            set
            {
                _bIsNoiTruChecked = value;
                _bIsNgoaiTruChecked = !_bIsNoiTruChecked;
                _bIsAllChecked = !_bIsNoiTruChecked;
                NotifyOfPropertyChange(() => bIsNoiTruChecked);
            }
        }

        private void SetPatientFindBy(AllLookupValues.PatientFindBy PatientFindByUpdate)
        {
            PatientFindBy = PatientFindByUpdate;
        }

        public void rdoNgoaiTru_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy(AllLookupValues.PatientFindBy.NGOAITRU);
        }

        public void rdoNoiTru_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy(AllLookupValues.PatientFindBy.NOITRU);
        }

        public void rdoAll_Click(object sender, RoutedEventArgs e)
        {
            SetPatientFindBy(AllLookupValues.PatientFindBy.CAHAI);
        }

        public void butUpdate()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();
            //UpdateManagementUserOfficial();
        }

        public void butAddNew()
        {
            if (AucHoldLoginDoctor.StaffID == 0)
            {
                Globals.ShowMessage("Chưa chọn bác sĩ chính thức", eHCMSResources.T0432_G1_Error);
                return;
            }
            if (AucHoldOfficialDoctor.StaffID == 0)
            {
                Globals.ShowMessage("Chưa chọn bác sĩ sử dụng", eHCMSResources.T0432_G1_Error);
                return;
            }
            if (FromDateContent.DateTime > ToDateContent.DateTime)
            {
                Globals.ShowMessage("Từ ngày phải trước đến ngày", eHCMSResources.T0432_G1_Error);
                return;
            }
            if (AucHoldLoginDoctor.StaffID != 0)
            {
                ManagementUserOfficial curUserOfficialHitory = new ManagementUserOfficial
                {
                    LoginUserID = AucHoldLoginDoctor.StaffID,
                    UserOfficialID = AucHoldOfficialDoctor.StaffID,
                    FromDate = FromDateContent.DateTime.GetValueOrDefault(),
                    ToDate = ToDateContent.DateTime.GetValueOrDefault(),
                    StaffID = (long)Globals.LoggedUserAccount.StaffID,
                    PatientFindBy = (int)PatientFindBy
                };
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
                            contract.BeginAddManagementUserOfficial(curUserOfficialHitory, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndAddManagementUserOfficial(asyncResult);
                                    if (results > 0)
                                    {
                                        MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                        curUserOfficialHitory.ManagementUserOfficialID = results;
                                        if (Globals.DoctorAccountBorrowed == null)
                                        {
                                            Globals.DoctorAccountBorrowed = new UserAccount();
                                            Globals.DoctorAccountBorrowed.Staff = new Staff { StaffID = AucHoldLoginDoctor.StaffID, FullName = AucHoldLoginDoctor.StaffName };
                                            Globals.DoctorAccountBorrowed.StaffID = AucHoldLoginDoctor.StaffID;
                                        }
                                        else
                                        {
                                            Globals.DoctorAccountBorrowed.Staff = new Staff { StaffID = AucHoldLoginDoctor.StaffID, FullName = AucHoldLoginDoctor.StaffName };
                                            Globals.DoctorAccountBorrowed.StaffID = AucHoldLoginDoctor.StaffID;
                                        }
                                    }
                                    GetManagementUserOfficialPaging(AucHoldLoginDoctor.StaffID, AucHoldOfficialDoctor.StaffID, allManagementUserOfficial.PageIndex
                                        , allManagementUserOfficial.PageSize, true);
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
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteManagementUserOfficial();
            }
        }

        public void lnkRefreshClick(object sender, RoutedEventArgs e)
        {
            allManagementUserOfficial.PageSize = 10;
            allManagementUserOfficial.PageIndex = 0;
            GetManagementUserOfficialPaging(AucHoldLoginDoctor.StaffID, AucHoldOfficialDoctor.StaffID, allManagementUserOfficial.PageIndex
                , allManagementUserOfficial.PageSize, true);
        }

        private void DeleteManagementUserOfficial()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteManagementUserOfficial(SelectedManagementUserOfficial.ManagementUserOfficialID
                            , (long)Globals.LoggedUserAccount.StaffID
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteManagementUserOfficial(asyncResult);
                                if (results == true)
                                {
                                    MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                }
                                GetManagementUserOfficialPaging(AucHoldLoginDoctor.StaffID, AucHoldOfficialDoctor.StaffID
                                    , allManagementUserOfficial.PageIndex
                                    , allManagementUserOfficial.PageSize, true);
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

        private void AddManagementUserOfficial()
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
                        contract.BeginAddManagementUserOfficial(curManagementUserOfficial, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddManagementUserOfficial(asyncResult);
                                if (results > 0)
                                {
                                    MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                }
                                GetManagementUserOfficialPaging(AucHoldLoginDoctor.StaffID, AucHoldOfficialDoctor.StaffID
                                    , allManagementUserOfficial.PageIndex
                                    , allManagementUserOfficial.PageSize, true);
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

        private void GetManagementUserOfficialPaging(long LoginUserID, long UserOfficialID
            , int PageIndex, int PageSize, bool bCountTotal)
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
                        contract.BeginGetManagementUserOfficialPaging(LoginUserID, UserOfficialID, PageIndex
                            , PageSize, bCountTotal
                            , Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var results = contract.EndGetManagementUserOfficialPaging(out int total, asyncResult);
                                 allManagementUserOfficial.Clear();
                                 if (results != null)
                                 {
                                     foreach (var p in results)
                                     {
                                         allManagementUserOfficial.Add(p);
                                     }
                                 }
                                 allManagementUserOfficial.TotalItemCount = total;
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

        public void grdManagementUserOfficial_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            ManagementUserOfficial item = e.Row.DataContext as ManagementUserOfficial;
            if (item == null)
            {
                return;
            }
        }
    }
}
