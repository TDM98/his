using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Windows.Media;
using Service.Core.Common;
using aEMR.Controls;
using aEMR.Common.Utilities;
using aEMR.CommonTasks;
using eHCMSLanguage;
using aEMR.Common.Printing;
using aEMR.Common.PagedCollectionView;
using Microsoft.Win32;
using Castle.Windsor;
using System.Windows.Data;
/*
* 20181006 #001 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
*                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
*/
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IEstimationPharmacy)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EstimationPharmacyViewModel : Conductor<object>, IEstimationPharmacy
        , IHandle<PharmacyCloseSearchEstimationEvent>, IHandle<PharmacySupplierToEstimationEvent>
        , IHandle<PharmaceuticalCompanyToEstimationEvent>

    {
        public string TitleForm { get; set; }

        #region Indicator Member
        private bool _isLoadingGetRefGenericDrugCategory = false;
        public bool isLoadingGetRefGenericDrugCategory
        {
            get { return _isLoadingGetRefGenericDrugCategory; }
            set
            {
                if (_isLoadingGetRefGenericDrugCategory != value)
                {
                    _isLoadingGetRefGenericDrugCategory = value;
                    NotifyOfPropertyChange(() => isLoadingGetRefGenericDrugCategory);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator = false;
        public bool isLoadingFullOperator
        {
            get { return _isLoadingFullOperator; }
            set
            {
                if (_isLoadingFullOperator != value)
                {
                    _isLoadingFullOperator = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingCheckExists = false;
        public bool isLoadingCheckExists
        {
            get { return _isLoadingCheckExists; }
            set
            {
                if (_isLoadingCheckExists != value)
                {
                    _isLoadingCheckExists = value;
                    NotifyOfPropertyChange(() => isLoadingCheckExists);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingGetID = false;
        public bool isLoadingGetID
        {
            get { return _isLoadingGetID; }
            set
            {
                if (_isLoadingGetID != value)
                {
                    _isLoadingGetID = value;
                    NotifyOfPropertyChange(() => isLoadingGetID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingGetMonth = false;
        public bool isLoadingGetMonth
        {
            get { return _isLoadingGetMonth; }
            set
            {
                if (_isLoadingGetMonth != value)
                {
                    _isLoadingGetMonth = value;
                    NotifyOfPropertyChange(() => isLoadingGetMonth);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDelete = false;
        public bool isLoadingDelete
        {
            get { return _isLoadingDelete; }
            set
            {
                if (_isLoadingDelete != value)
                {
                    _isLoadingDelete = value;
                    NotifyOfPropertyChange(() => isLoadingDelete);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _IsHIStorage = false;
        public bool IsHIStorage
        {
            get { return _IsHIStorage; }
            set
            {
                if(_IsHIStorage != value)
                {
                    _IsHIStorage = value;
                    NotifyOfPropertyChange(() => IsHIStorage);
                }
            }
        }


        public bool IsLoading
        {
            get { return (isLoadingGetRefGenericDrugCategory || isLoadingFullOperator || isLoadingCheckExists || isLoadingGetID || isLoadingSearch || isLoadingGetMonth || isLoadingDelete); }
        }

        private bool _IsComplete = true;
        public bool IsComplete
        {
            get { return _IsComplete; }
            set
            {
                if(_IsComplete != value)
                {
                    _IsComplete = value;
                    NotifyOfPropertyChange(() => IsComplete);
                }
            }
        }
        #endregion

        private enum DataGridCol
        {
            ColMultiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3,
            HoatChat = 4,
            HangSX = 5,
            QCDongGoi = 6,
            SLQC = 7,
            SLTon = 8,
            T1 = 9,
            T2 = 10,
            T3 = 11,
            T4 = 12,
            TBX = 13,
            DoUuTien = 14,
            HeSoNhan = 15,
            SLLyThuyet = 16,
            SLTinhTheoQC = 17,
            SLDieuChinh = 18,
            DVT = 19,
            XuatT12 = 20,
            ThangHienTai = 21
        }

        private long V_MedProductType = 11001;//11001 : thuoc , 11002 : y cu ,11003 : hoa chat 
        [ImportingConstructor]
        public EstimationPharmacyViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);

            EstimationDetailsDeleted = new ObservableCollection<PharmacyEstimationForPODetail>();

            SearchCriteria = new RequestSearchCriteria();

            CurrentMonthDisplay = new MonthDisplay();

            InittializeEstimationForPO();
            GetAllSupplier();
            GetAllPharmaceuticalCompany();
            CurrentRefGenericDrugDetail = new DataEntities.RefGenericDrugDetail();

            authorization();

            RefGenericDrugDetail = new PagedSortableCollectionView<DataEntities.RefGenericDrugDetail>();
            RefGenericDrugDetail.OnRefresh += new EventHandler<RefreshEventArgs>(RefGenericDrugDetail_OnRefresh);
            RefGenericDrugDetail.PageSize = Globals.PageSize;
         
        }

        void RefGenericDrugDetail_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenericDrugDetail_Auto(BrandName, RefGenericDrugDetail.PageIndex, RefGenericDrugDetail.PageSize);
        }


        #region 1. Properties Member

        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    if (_AllChecked)
                    {
                        AllCheckedfc();
                    }
                    else
                    {
                        UnCheckedfc();
                    }
                }
            }
        }

        private string BrandName;

        private int _CDC;
        public int CDC
        {
            get
            {
                return _CDC;
            }
            set
            {
                if (_CDC != value)
                {
                    if (CheckIntMoreThanZeroValid(value))
                    {
                        _CDC = value;
                        NotifyOfPropertyChange(() => CDC);
                    }
                    else
                    {
                        Globals.ShowMessage(eHCMSResources.Z1599_G1_CDCKgHopLe, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
        }
        private int _HeSoNhan;
        public int HeSoNhan
        {
            get
            {
                return _HeSoNhan;
            }
            set
            {
                if (_HeSoNhan != value)
                {
                    if (CheckIntMoreThanZeroValid(value))
                    {
                        _HeSoNhan = value;
                        NotifyOfPropertyChange(() => HeSoNhan);
                        CountEstimateAll();
                    }
                    else
                    {
                        Globals.ShowMessage(eHCMSResources.Z1600_G1_HSoNhanKgHopLe, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
        }
        private bool CheckIntMoreThanZeroValid(int value)
        {
            if (value >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private ObservableCollection<PharmacyEstimationForPO> _PharmacyEstimationForPOCbx;
        public ObservableCollection<PharmacyEstimationForPO> PharmacyEstimationForPOCbx
        {
            get
            {
                return _PharmacyEstimationForPOCbx;
            }
            set
            {
                if (_PharmacyEstimationForPOCbx != value)
                {
                    _PharmacyEstimationForPOCbx = value;
                    NotifyOfPropertyChange(() => PharmacyEstimationForPOCbx);
                }

            }
        }

        private PharmacyEstimationForPO _CurrentPharmacyEstimationForPO;
        public PharmacyEstimationForPO CurrentPharmacyEstimationForPO
        {
            get
            {
                return _CurrentPharmacyEstimationForPO;
            }
            set
            {
                if (_CurrentPharmacyEstimationForPO != value)
                {
                    _CurrentPharmacyEstimationForPO = value;
                    NotifyOfPropertyChange(() => CurrentPharmacyEstimationForPO);
                }
            }
        }

        private bool _IsFirstMonth = true;
        public bool IsFirstMonth
        {
            get
            {
                return _IsFirstMonth;
            }
            set
            {
                _IsFirstMonth = value;
                if (_IsFirstMonth)
                {
                    IsAdditionFirstMonth = !_IsFirstMonth;
                    IsModifyMonth = !_IsFirstMonth;
                    IsFirstYear = !_IsFirstMonth;
                    IsModifyYear = !_IsFirstMonth;
                }
                NotifyOfPropertyChange(() => IsFirstMonth);
            }
        }

        private bool _IsAdditionFirstMonth;
        public bool IsAdditionFirstMonth
        {
            get
            {
                return _IsAdditionFirstMonth;
            }
            set
            {
                _IsAdditionFirstMonth = value;
                if (_IsAdditionFirstMonth)
                {
                    IsFirstMonth = !_IsAdditionFirstMonth;
                    IsModifyMonth = !_IsAdditionFirstMonth;
                    IsFirstYear = !_IsAdditionFirstMonth;
                    IsModifyYear = !_IsAdditionFirstMonth;
                }
                NotifyOfPropertyChange(() => IsAdditionFirstMonth);
            }
        }

        private bool _IsModifyMonth;
        public bool IsModifyMonth
        {
            get
            {
                return _IsModifyMonth;
            }
            set
            {
                _IsModifyMonth = value;
                if (_IsModifyMonth)
                {
                    IsFirstMonth = !_IsModifyMonth;
                    IsAdditionFirstMonth = !_IsModifyMonth;
                    IsFirstYear = !_IsModifyMonth;
                    IsModifyYear = !_IsModifyMonth;
                }
                NotifyOfPropertyChange(() => IsModifyMonth);
            }
        }

        private bool _IsFirstYear;
        public bool IsFirstYear
        {
            get
            {
                return _IsFirstYear;
            }
            set
            {
                _IsFirstYear = value;
                if (_IsFirstYear)
                {
                    IsFirstMonth = !_IsFirstYear;
                    IsAdditionFirstMonth = !_IsFirstYear;
                    IsModifyMonth = !_IsFirstYear;
                    IsModifyYear = !_IsFirstYear;
                }
                NotifyOfPropertyChange(() => IsFirstYear);
            }
        }

        private bool _IsModifyYear;
        public bool IsModifyYear
        {
            get
            {
                return _IsModifyYear;
            }
            set
            {
                _IsModifyYear = value;
                if (_IsModifyYear)
                {
                    IsFirstMonth = !_IsModifyYear;
                    IsAdditionFirstMonth = !_IsModifyYear;
                    IsModifyMonth = !_IsModifyYear;
                    IsFirstYear = !_IsModifyYear;
                }
                NotifyOfPropertyChange(() => IsModifyYear);
            }
        }

        private Visibility _VisibilityAdd = Visibility.Collapsed;
        public Visibility VisibilityAdd
        {
            get
            {
                return _VisibilityAdd;
            }
            set
            {
                _VisibilityAdd = value;
                NotifyOfPropertyChange(() => VisibilityAdd);
            }
        }

        private PharmacyEstimationForPODetail _CurrentPharmacyEstimationForPoDetail;
        public PharmacyEstimationForPODetail CurrentPharmacyEstimationForPoDetail
        {
            get
            {
                return _CurrentPharmacyEstimationForPoDetail;
            }
            set
            {
                _CurrentPharmacyEstimationForPoDetail = value;
                NotifyOfPropertyChange(() => CurrentPharmacyEstimationForPoDetail);
            }
        }

        private ObservableCollection<PharmacyEstimationForPODetail> EstimationDetailsDeleted;

        private MonthDisplay _CurrentMonthDisplay;
        public MonthDisplay CurrentMonthDisplay
        {
            get
            {
                return _CurrentMonthDisplay;
            }
            set
            {
                if (_CurrentMonthDisplay != value)
                {
                    _CurrentMonthDisplay = value;
                    NotifyOfPropertyChange(() => CurrentMonthDisplay);
                }
            }
        }

        private RequestSearchCriteria _SearchCriteria;
        public RequestSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mDuTru_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDuTruThuoc,
                                               (int)oPharmacyEx.mDuTru_Tim, (int)ePermission.mView);
            mDuTru_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDuTruThuoc,
                                               (int)oPharmacyEx.mDuTru_ThongTin, (int)ePermission.mView);
            mDuTru_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDuTruThuoc,
                                               (int)oPharmacyEx.mDuTru_PhieuMoi, (int)ePermission.mView);
            mDuTru_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDuTruThuoc,
                                               (int)oPharmacyEx.mDuTru_Luu, (int)ePermission.mView);
            mDuTru_ReportIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDuTruThuoc,
                                               (int)oPharmacyEx.mDuTru_ReportIn, (int)ePermission.mView);
            mDuTru_Xoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDuTruThuoc,
                                               (int)oPharmacyEx.mDuTru_Xoa, (int)ePermission.mView);
        }
        #region checking account

        private bool _mDuTru_Tim = true;
        private bool _mDuTru_ThongTin = true;
        private bool _mDuTru_PhieuMoi = true;
        private bool _mDuTru_Luu = true;
        private bool _mDuTru_ReportIn = true;
        private bool _mDuTru_Xoa = true;

        public bool mDuTru_Xoa
        {
            get
            {
                return _mDuTru_Xoa;
            }
            set
            {
                if (_mDuTru_Xoa == value)
                    return;
                _mDuTru_Xoa = value;
            }
        }

        public bool mDuTru_Tim
        {
            get
            {
                return _mDuTru_Tim;
            }
            set
            {
                if (_mDuTru_Tim == value)
                    return;
                _mDuTru_Tim = value;
            }
        }
        public bool mDuTru_ThongTin
        {
            get
            {
                return _mDuTru_ThongTin;
            }
            set
            {
                if (_mDuTru_ThongTin == value)
                    return;
                _mDuTru_ThongTin = value;
            }
        }
        public bool mDuTru_PhieuMoi
        {
            get
            {
                return _mDuTru_PhieuMoi;
            }
            set
            {
                if (_mDuTru_PhieuMoi == value)
                    return;
                _mDuTru_PhieuMoi = value;
            }
        }
        public bool mDuTru_Luu
        {
            get
            {
                return _mDuTru_Luu;
            }
            set
            {
                if (_mDuTru_Luu == value)
                    return;
                _mDuTru_Luu = value;
            }
        }
        public bool mDuTru_ReportIn
        {
            get
            {
                return _mDuTru_ReportIn;
            }
            set
            {
                if (_mDuTru_ReportIn == value)
                    return;
                _mDuTru_ReportIn = value;
            }
        }
        //private bool _bEdit = true;
        //private bool _bAdd = true;
        //private bool _bDelete = true;
        //private bool _bView = true;
        //private bool _bPrint = true;
        //private bool _bReport = true;

        //public bool bEdit
        //{
        //    get
        //    {
        //        return _bEdit;
        //    }
        //    set
        //    {
        //        if (_bEdit == value)
        //            return;
        //        _bEdit = value;
        //    }
        //}
        //public bool bAdd
        //{
        //    get
        //    {
        //        return _bAdd;
        //    }
        //    set
        //    {
        //        if (_bAdd == value)
        //            return;
        //        _bAdd = value;
        //    }
        //}
        //public bool bDelete
        //{
        //    get
        //    {
        //        return _bDelete;
        //    }
        //    set
        //    {
        //        if (_bDelete == value)
        //            return;
        //        _bDelete = value;
        //    }
        //}
        //public bool bView
        //{
        //    get
        //    {
        //        return _bView;
        //    }
        //    set
        //    {
        //        if (_bView == value)
        //            return;
        //        _bView = value;
        //    }
        //}
        //public bool bPrint
        //{
        //    get
        //    {
        //        return _bPrint;
        //    }
        //    set
        //    {
        //        if (_bPrint == value)
        //            return;
        //        _bPrint = value;
        //    }
        //}
        //public bool bReport
        //{
        //    get
        //    {
        //        return _bReport;
        //    }
        //    set
        //    {
        //        if (_bReport == value)
        //            return;
        //        _bReport = value;
        //    }
        //}
        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void InittializeEstimationForPO()
        {
            CurrentPharmacyEstimationForPO = null;
            CurrentPharmacyEstimationForPO = new PharmacyEstimationForPO();
            CurrentPharmacyEstimationForPO.DateOfEstimation = Globals.ServerDate.Value;
            CurrentPharmacyEstimationForPO.StaffID = GetStaffLogin().StaffID;
            CurrentPharmacyEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.FIRSTMONTH;
            CurrentPharmacyEstimationForPO.EstimationDetails = new ObservableCollection<PharmacyEstimationForPODetail>();
            CVS_PCVEstimationDetails = new CollectionViewSource { Source = CurrentPharmacyEstimationForPO.EstimationDetails };
            CV_PCVEstimationDetails = (CollectionView)CVS_PCVEstimationDetails.View;
            NotifyOfPropertyChange(() => CV_PCVEstimationDetails);
            SetRadioButtonCompleted();
            EstimationDetailsDeleted.Clear();
            HideShowColumnDelete();
            VisibilityAdd = Visibility.Collapsed;
        }

        private void CountEstimateAll()
        {
            if (CurrentPharmacyEstimationForPO.EstimationDetails != null)
            {
                foreach (PharmacyEstimationForPODetail p in CurrentPharmacyEstimationForPO.EstimationDetails)
                {
                    p.NumberOfEstimatedMonths_F = HeSoNhan;
                    p.EstimatedQty_F = (p.NumberOfEstimatedMonths_F * p.OutAverageQty) - p.RemainQty;
                    if (p.RefGenMedProductDetails != null && p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault() != 0)
                    {
                        if (p.EstimatedQty_F > 0)
                        {
                            p.PackageQty = (int)Math.Ceiling((double)p.EstimatedQty_F / (double)p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault());
                        }
                        else
                        {
                            p.PackageQty = 0;
                        }
                        p.AdjustedQty = (int)Math.Ceiling((double)p.PackageQty * p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault());
                        if (p.EntityState == EntityState.PERSITED || p.EntityState == EntityState.MODIFIED)
                        {
                            p.EntityState = EntityState.MODIFIED;
                        }
                    }
                }
            }
        }

        public bool CheckDeleted(object item)
        {
            PharmacyEstimationForPODetailExt temp = item as PharmacyEstimationForPODetailExt;
            if (temp != null && temp.EntityState == EntityState.DETACHED)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void grdRequestDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            if (CheckDeleted(e.Row.DataContext))
            {
                e.Row.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void PharmacyEstimationForPO_FullOperator()
        {
            this.ShowBusyIndicator();
            //isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyEstimationForPO_FullOperator(V_MedProductType, CurrentPharmacyEstimationForPO, IsHIStorage, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            PharmacyEstimationForPO EstimateOut;
                            long results = contract.EndPharmacyEstimationForPO_FullOperator(out EstimateOut, asyncResult);
                            EstimationDetailsDeleted.Clear();
                            SetRadioButtonCompleted();
                            if (results >= 0)
                            {
                                CurrentPharmacyEstimationForPO = EstimateOut;
                                LoadDataGrid();
                            }
                            else
                            {
                                InittializeEstimationForPO();
                            }
                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SetRadioButtonCompleted()
        {
            if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
            {
                IsFirstMonth = true;
            }
            else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH)
            {
                IsAdditionFirstMonth = true;
            }
            else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
            {
                IsModifyMonth = true;
            }
            else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
            {
                IsFirstYear = true;
            }
            else
            {
                IsModifyYear = true;
            }
            ShowOrHideAddProduct();

        }

        private void ShowOrHideAddProduct()
        {
            if (CurrentPharmacyEstimationForPO.PharmacyEstimatePoID > 0)
            {
                if (CurrentPharmacyEstimationForPO.IsOrder.GetValueOrDefault())
                {
                    VisibilityAdd = Visibility.Collapsed;
                }
                else
                {
                    VisibilityAdd = Visibility.Visible;
                }
            }
        }

        //public void btnSave(object sender, RoutedEventArgs e)
        //{
        //    if (grdEstimateDetails != null && grdEstimateDetails.IsValid)
        //    {
        //        bool isAdd = true;
        //        if (EstimationDetailsDeleted.Count > 0)
        //        {
        //            if (CurrentPharmacyEstimationForPO.EstimationDetails == null)
        //            {
        //                CurrentPharmacyEstimationForPO.EstimationDetails = new ObservableCollection<PharmacyEstimationForPODetail>();
        //            }
        //            for (int i = 0; i < EstimationDetailsDeleted.Count; i++)
        //            {
        //                CurrentPharmacyEstimationForPO.EstimationDetails.Add(EstimationDetailsDeleted[i]);
        //            }
        //        }
        //        if (CurrentPharmacyEstimationForPO.EstimationDetails != null && CurrentPharmacyEstimationForPO.EstimationDetails.Count > 0)
        //        {
        //            for (int i = 0; i < CurrentPharmacyEstimationForPO.EstimationDetails.Count; i++)
        //            {
        //                if (CurrentPharmacyEstimationForPO.EstimationDetails[i].RefGenMedProductDetails != null && CurrentPharmacyEstimationForPO.EstimationDetails[i].RefGenMedProductDetails.DrugID > 0)
        //                {
        //                    if (CurrentPharmacyEstimationForPO.EstimationDetails[i].AdjustedQty <= 0)
        //                    {
        //                        isAdd = false;
        //                        if (MessageBox.Show("1 số thuốc có SL dự trù <=0." + Environment.NewLine + "Bạn có muốn chương trình tự xóa những thuốc này không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //                        {
        //                            AutoDeleteEstimationDetailsZero();
        //                        }
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            isAdd = false;
        //            MessageBox.Show(eHCMSResources.K0430_G1_NhapDLieu);
        //        }
        //        if (isAdd)
        //        {
        //            PharmacyEstimationForPO_FullOperator();
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe);
        //    }
        //}


        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (CurrentPharmacyEstimationForPO == null || CurrentPharmacyEstimationForPO.EstimationDetails == null || CurrentPharmacyEstimationForPO.EstimationDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.K0433_G1_NhapHgCanDuTru);
                return;
            }

            if (CurrentPharmacyEstimationForPO.EstimationDetails.Any(x => x.AdjustedQty <= 0))
            {
                if (MessageBox.Show(eHCMSResources.A0821_G1_Msg_ConfTuDongXoaHg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    AutoDeleteEstimationDetailsZero();
                }
            }

            if (EstimationDetailsDeleted.Count > 0)
            {
                for (int i = 0; i < EstimationDetailsDeleted.Count; i++)
                {
                    CurrentPharmacyEstimationForPO.EstimationDetails.Add(EstimationDetailsDeleted[i]);
                }
            }

            PharmacyEstimationForPO_FullOperator();

        }


        private void AutoDeleteEstimationDetailsZero()
        {
            if (CurrentPharmacyEstimationForPO.EstimationDetails != null)
            {
                ObservableCollection<PharmacyEstimationForPODetail> lst = CurrentPharmacyEstimationForPO.EstimationDetails.DeepCopy();
                var items = lst.Where(x => x.AdjustedQty <= 0);
                if (items != null)
                {
                    foreach (PharmacyEstimationForPODetail p in items)
                    {
                        if (p.EntityState != EntityState.NEW)
                        {
                            p.EntityState = EntityState.DETACHED;
                            EstimationDetailsDeleted.Add(p);
                        }
                        //CurrentPharmacyEstimationForPO.EstimationDetails.Remove(p);
                    }
                    CurrentPharmacyEstimationForPO.EstimationDetails = lst.Where(x => x.RefGenMedProductDetails != null && x.AdjustedQty > 0).ToObservableCollection();
                    LoadDataGrid();
                }
            }
        }

        private void DeleteEstimationDetails(object item)
        {
            if (CurrentPharmacyEstimationForPO.EstimationDetails != null)
            {
                PharmacyEstimationForPODetail p = (PharmacyEstimationForPODetail)item;
                if (p.EntityState != EntityState.NEW)
                {
                    p.EntityState = EntityState.DETACHED;
                    EstimationDetailsDeleted.Add(p);
                }
                CurrentPharmacyEstimationForPO.EstimationDetails.Remove(p);
               //CurrentPharmacyEstimationForPO.EstimationDetails = CurrentPharmacyEstimationForPO.EstimationDetails.ToObservableCollection();
                LoadDataGrid();
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (grdEstimateDetails != null && grdEstimateDetails.SelectedItem !=null)
                {
                    DeleteEstimationDetails(grdEstimateDetails.SelectedItem);
                }

            }
        }

        private void DisplayMonth()
        {
            if (CurrentMonthDisplay == null)
            {
                CurrentMonthDisplay = new MonthDisplay();
            }
            DateTime Date = Globals.ServerDate.Value;
            if (CurrentPharmacyEstimationForPO != null)
            {
                Date = CurrentPharmacyEstimationForPO.DateOfEstimation;
            }

            int month = Date.Month;
            CurrentMonthDisplay.CurrentT = eHCMSResources.Z1410_G1_SLgXuatT + month.ToString();
            if (month > 4)
            {
                CurrentMonthDisplay.T1 = eHCMSResources.T0748_G1_T.ToUpper() + (month - 4).ToString();
                CurrentMonthDisplay.T2 = eHCMSResources.T0748_G1_T.ToUpper() + (month - 3).ToString();
                CurrentMonthDisplay.T3 = eHCMSResources.T0748_G1_T.ToUpper() + (month - 2).ToString();
                CurrentMonthDisplay.T4 = eHCMSResources.T0748_G1_T.ToUpper() + (month - 1).ToString();
            }
            else
            {
                if (month == 1)
                {
                    CurrentMonthDisplay.T1 = eHCMSResources.Z1312_G1_Thang9;
                    CurrentMonthDisplay.T2 = eHCMSResources.Z1312_G1_Thang10;
                    CurrentMonthDisplay.T3 = eHCMSResources.Z1312_G1_Thang11;
                    CurrentMonthDisplay.T4 = eHCMSResources.Z1312_G1_Thang12;
                }
                else if (month == 2)
                {
                    CurrentMonthDisplay.T1 = eHCMSResources.Z1312_G1_Thang10;
                    CurrentMonthDisplay.T2 = eHCMSResources.Z1312_G1_Thang11;
                    CurrentMonthDisplay.T3 = eHCMSResources.Z1312_G1_Thang12;
                    CurrentMonthDisplay.T4 = eHCMSResources.Z1312_G1_Thang1;
                }
                else if (month == 3)
                {
                    CurrentMonthDisplay.T1 = eHCMSResources.Z1312_G1_Thang11;
                    CurrentMonthDisplay.T2 = eHCMSResources.Z1312_G1_Thang12;
                    CurrentMonthDisplay.T3 = eHCMSResources.Z1312_G1_Thang1;
                    CurrentMonthDisplay.T4 = eHCMSResources.T0758_G1_T2;
                }
                else
                {
                    CurrentMonthDisplay.T1 = eHCMSResources.Z1312_G1_Thang12;
                    CurrentMonthDisplay.T2 = eHCMSResources.Z1312_G1_Thang1;
                    CurrentMonthDisplay.T3 = eHCMSResources.T0758_G1_T2;
                    CurrentMonthDisplay.T4 = eHCMSResources.T0759_G1_T3;
                }
            }
            NotifyOfPropertyChange(() => CurrentMonthDisplay);
            HideOrShowColumnDataGrid();
            DisplayColumnDataGrid();
            HideShowColumnDelete();
        }

        public void grdEstimateDetails_Loaded(object sender, RoutedEventArgs e)
        {
            grdEstimateDetails = sender as DataGrid;
            DisplayMonth();
        }

        private void DisplayColumnDataGrid()
        {
            if (grdEstimateDetails != null)
            {
                grdEstimateDetails.Columns[(int)DataGridCol.T1].Header = CurrentMonthDisplay.T1;
                grdEstimateDetails.Columns[(int)DataGridCol.T2].Header = CurrentMonthDisplay.T2;
                grdEstimateDetails.Columns[(int)DataGridCol.T3].Header = CurrentMonthDisplay.T3;
                grdEstimateDetails.Columns[(int)DataGridCol.T4].Header = CurrentMonthDisplay.T4;
                grdEstimateDetails.Columns[(int)DataGridCol.ThangHienTai].Header = CurrentMonthDisplay.CurrentT;
            }
        }

        private void HideOrShowColumnDataGrid()
        {
            if (grdEstimateDetails != null)
            {
                if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH || CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH || CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.T1].Visibility = Visibility.Visible;
                    grdEstimateDetails.Columns[(int)DataGridCol.T2].Visibility = Visibility.Visible;
                    grdEstimateDetails.Columns[(int)DataGridCol.T3].Visibility = Visibility.Visible;
                    grdEstimateDetails.Columns[(int)DataGridCol.T4].Visibility = Visibility.Visible;
                }
                else
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.T1].Visibility = Visibility.Collapsed;
                    grdEstimateDetails.Columns[(int)DataGridCol.T2].Visibility = Visibility.Collapsed;
                    grdEstimateDetails.Columns[(int)DataGridCol.T3].Visibility = Visibility.Collapsed;
                    grdEstimateDetails.Columns[(int)DataGridCol.T4].Visibility = Visibility.Collapsed;

                }
            }
        }

        public void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPharmacyEstimationForPO != null && CurrentPharmacyEstimationForPO.EstimationDetails != null && CurrentPharmacyEstimationForPO.EstimationDetails.Count > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0829_G1_Msg_ConfDoiLoaiDuTru, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
                    {
                        IsFirstMonth = true;
                    }
                    else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH)
                    {
                        IsAdditionFirstMonth = true;
                    }
                    else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
                    {
                        IsModifyMonth = true;
                    }
                    else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
                    {
                        IsFirstYear = true;
                    }
                    else
                    {
                        IsModifyYear = true;
                    }
                    return;
                }
            }


            CurrentPharmacyEstimationForPO = null;
            CurrentPharmacyEstimationForPO = new PharmacyEstimationForPO();
            CurrentPharmacyEstimationForPO.DateOfEstimation = Globals.GetCurServerDateTime();
            CurrentPharmacyEstimationForPO.StaffID = GetStaffLogin().StaffID;
            CurrentPharmacyEstimationForPO.EstimationDetails = new ObservableCollection<PharmacyEstimationForPODetail>();
            CVS_PCVEstimationDetails = new CollectionViewSource { Source = CurrentPharmacyEstimationForPO.EstimationDetails };
            CV_PCVEstimationDetails = (CollectionView)CVS_PCVEstimationDetails.View;
            NotifyOfPropertyChange(() => CV_PCVEstimationDetails);
            EstimationDetailsDeleted.Clear();
            VisibilityAdd = Visibility.Collapsed;

            if (IsFirstMonth)
            {
                CurrentPharmacyEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.FIRSTMONTH;
            }
            else if (IsAdditionFirstMonth)
            {
                CurrentPharmacyEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH;
            }
            else if (IsModifyMonth)
            {
                CurrentPharmacyEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.MODIFYMONTH;

            }
            else if (IsFirstYear)
            {
                CurrentPharmacyEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.FIRSTYEAR;
            }
            else
            {
                CurrentPharmacyEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.MODIFYYEAR;
            }
        }
        DataGrid grdEstimateDetails = null;

        private void PharmacyEstimationForPoDetail_ByParentID(long PharmacyEstimateID)
        {
            this.ShowBusyIndicator();
            SearchKey = "";
            //isLoadingGetID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyEstimationForPODetail_ByParentID(PharmacyEstimateID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndPharmacyEstimationForPODetail_ByParentID(asyncResult);
                            CurrentPharmacyEstimationForPO.EstimationDetails = results.ToObservableCollection();
                            EstimationDetailsDeleted.Clear();
                            LoadDataGrid();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingGetID = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyEstimationForPO_Search(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            //isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyEstimationForPO_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, IsHIStorage, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndPharmacyEstimationForPO_Search(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IEstimationPharmacySearch> onInitDlg = delegate (IEstimationPharmacySearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.PharmacyEstimationForPOList.Clear();
                                        proAlloc.PharmacyEstimationForPOList.TotalItemCount = Total;
                                        proAlloc.PharmacyEstimationForPOList.PageIndex = 0;
                                        proAlloc.IsHIStorage = IsHIStorage;
                                        foreach (PharmacyEstimationForPO p in results)
                                        {
                                            proAlloc.PharmacyEstimationForPOList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IEstimationPharmacySearch>(onInitDlg);
                                }
                                else
                                {
                                    //lay thang gia tri
                                    CurrentPharmacyEstimationForPO = results.FirstOrDefault();
                                    DisplayMonth();
                                    SetRadioButtonCompleted();
                                    PharmacyEstimationForPoDetail_ByParentID(CurrentPharmacyEstimationForPO.PharmacyEstimatePoID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            PharmacyEstimationForPO_Search(0, Globals.PageSize);
        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                PharmacyEstimationForPO_Search(0, Globals.PageSize);
            }
        }

        private void GetEstimationForMonthPharmacy()
        {
            this.ShowBusyIndicator();
            //isLoadingGetMonth = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetEstimationForMonthPharmacy(V_MedProductType, CurrentPharmacyEstimationForPO.DateOfEstimation, CurrentPharmacyEstimationForPO.V_EstimateType, IsHIStorage, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetEstimationForMonthPharmacy(asyncResult);
                            CurrentPharmacyEstimationForPO.EstimationDetails = results.ToObservableCollection();
                            if (CurrentPharmacyEstimationForPO.EstimationDetails != null && CurrentPharmacyEstimationForPO.EstimationDetails.Count > 0)
                            {
                                for (int i = 0; i < CurrentPharmacyEstimationForPO.EstimationDetails.Count; i++)
                                {
                                    CurrentPharmacyEstimationForPO.EstimationDetails[i].EntityState = EntityState.NEW;
                                }
                            }
                            LoadDataGrid();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingGetMonth = false;
                            //  Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool CheckedExists = false;
        private void PharmacyEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation)
        {
            this.ShowBusyIndicator();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //isLoadingCheckExists = true;
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyEstimationForPO_CheckExists(V_EstimateType, DateOfEstimation, IsHIStorage, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            CheckedExists = contract.EndPharmacyEstimationForPO_CheckExists(asyncResult);
                            if (CheckedExists)
                            {
                                if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0548_G1_DaCoDuTruDauTh, eHCMSResources.G0442_G1_TBao);
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0549_G1_DaCoDuTruDauNam, eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            else
                            {
                                DisplayMonth();
                                GetEstimationForMonthPharmacy();
                                VisibilityAdd = Visibility.Visible;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingCheckExists = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnOK(object sender, RoutedEventArgs e)
        {
            if (CurrentPharmacyEstimationForPO != null && CurrentPharmacyEstimationForPO.DateOfEstimation != null)
            {
                if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH || CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
                {
                    PharmacyEstimationForPO_CheckExists(CurrentPharmacyEstimationForPO.V_EstimateType.GetValueOrDefault(), CurrentPharmacyEstimationForPO.DateOfEstimation);
                }
                else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH)
                {
                    DisplayMonth();
                    GetEstimationForMonthPharmacy();
                    VisibilityAdd = Visibility.Visible;
                }
                else
                {
                    DisplayMonth();
                    VisibilityAdd = Visibility.Visible;
                }
               
            }
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0143_G1_Msg_ConfTaoMoiPhDuTru, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                InittializeEstimationForPO();
            }
        }

        private void GetPackageQty(object item)
        {
            ////hien ko dung nua
            //PharmacyEstimationForPODetail p = (PharmacyEstimationForPODetail)item;
            //if (p.RefGenMedProductDetails != null)
            //{
            //    p.PackageQty = Math.Round((double)p.AdjustedQty / p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1), 2);
            //}
            //if (p.EntityState == EntityState.PERSITED || p.EntityState == EntityState.MODIFIED)
            //{
            //    p.EntityState = EntityState.MODIFIED;
            //}
        }

        private void CountEstimate(object item, bool IsGrid)
        {
            PharmacyEstimationForPODetail p = (PharmacyEstimationForPODetail)item;
            if (p != null)
            {
                if (IsGrid)
                {
                    if (PreparingCellForEdit != "" && p.NumberOfEstimatedMonths_F == Convert.ToInt32(PreparingCellForEdit))
                    {
                        return;
                    }
                }
                p.EstimatedQty_F = (p.NumberOfEstimatedMonths_F * p.OutAverageQty) - p.RemainQty;
                if (p.RefGenMedProductDetails != null && p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault() != 0)
                {
                    if (p.EstimatedQty_F > 0)
                    {
                        //p.PackageQty = (int)Math.Ceiling((double)p.EstimatedQty / (double)p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault());
                        p.PackageQty = Math.Ceiling(CalQtyPackaging(p.EstimatedQty_F, p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1)));
                    }
                    else
                    {
                        p.PackageQty = 0;
                    }
                    
                    //p.AdjustedQty = (int)Math.Ceiling((double)p.PackageQty * p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault());
                    p.AdjustedQty = CalAdjustedQty(p.PackageQty, p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));

                    p.TotalPrice = p.AdjustedQty * p.UnitPrice;

                    if (p.EntityState == EntityState.PERSITED || p.EntityState == EntityState.MODIFIED)
                    {
                        p.EntityState = EntityState.MODIFIED;
                    }
                }

            }
        }

        private void ChangeEntityState(object item)
        {
            PharmacyEstimationForPODetail p = item as PharmacyEstimationForPODetail;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }
            }
        }


        private string PreparingCellForEdit = "";
        public void grdEstimateDetails_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            TextBox tbl = grdEstimateDetails.CurrentColumn.GetCellContent(grdEstimateDetails.SelectedItem) as TextBox;
            if (tbl != null)
            {
                PreparingCellForEdit = tbl.Text;
            }
        }

        public void grdEstimateDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (grdEstimateDetails != null)
            {
                if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH || CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH || CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
                {
                    ChangeEntityState(grdEstimateDetails.SelectedItem);
                }
            }
            PharmacyEstimationForPODetail item = e.Row.DataContext as PharmacyEstimationForPODetail;
            //▼====== #001
            //string CurrentColumnName = e.Column.GetValue(FrameworkElement.NameProperty).ToString();

            //if (CurrentColumnName == "colNumOfEstimatedMonth")
            if (e.Column.Equals(grdEstimateDetails.GetColumnByName("colNumOfEstimatedMonth")))
            {
                if (txtNumberOfEstimatedMonths != null && !string.IsNullOrEmpty(txtNumberOfEstimatedMonths.Text))
                {
                    BindingExpression binding = txtNumberOfEstimatedMonths.GetBindingExpression(AxTextBox.TextProperty);
                    binding.UpdateSource();
                }
                CountEstimate(item, true);
            }

            //if (CurrentColumnName == "colQtyPackaging" && item.RefGenMedProductDetails != null)
            if (e.Column.Equals(grdEstimateDetails.GetColumnByName("colQtyPackaging")) && item.RefGenMedProductDetails != null)
            {
                item.AdjustedQty = CalAdjustedQty(item.PackageQty, item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));
                item.TotalPrice = item.AdjustedQty * item.UnitPrice;
            }

            //if (CurrentColumnName == "colAdjustedQty" && item.RefGenMedProductDetails != null)
            if (e.Column.Equals(grdEstimateDetails.GetColumnByName("colAdjustedQty")) && item.RefGenMedProductDetails != null)
            {
                item.PackageQty = CalQtyPackaging(item.AdjustedQty, item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));
                item.TotalPrice = item.AdjustedQty * item.UnitPrice;
            }
            //▲====== #001
        }

        private int CalAdjustedQty(double _qtyPackaging, int _unitPackaging)
        {
            return (int)Math.Round(_qtyPackaging * _unitPackaging, 0);
        }

        private double CalQtyPackaging(double _adjustedQty, int _unitPackaging)
        {
            return (double)_adjustedQty / (_unitPackaging > 0 ? _unitPackaging : 1);
        }

        public void txtQtyPackaging_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentPharmacyEstimationForPoDetail == null || CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails == null)
            {
                return;
            }

            //KMx: Nếu text có dấu ",". VD: 150,000. thì phải bỏ dấu "," đi, nếu không sẽ không TryParse được (03/11/2014 17:26).
            string txt = "";
            txt = (sender as TextBox).Text.Replace(",", "");

            if (!string.IsNullOrEmpty(txt))
            {

                double qtyPackaging = 0;
                double.TryParse(txt, out qtyPackaging);

                CurrentPharmacyEstimationForPoDetail.PackageQty = qtyPackaging;
                CurrentPharmacyEstimationForPoDetail.AdjustedQty = CalAdjustedQty(qtyPackaging, CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));
                CurrentPharmacyEstimationForPoDetail.TotalPrice = CurrentPharmacyEstimationForPoDetail.AdjustedQty * CurrentPharmacyEstimationForPoDetail.UnitPrice;
            }
        }


        public void txtAdjustedQty_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentPharmacyEstimationForPoDetail == null || CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails == null)
            {
                return;
            }

            //KMx: Nếu text có dấu ",". VD: 150,000. thì phải bỏ dấu "," đi, nếu không sẽ không TryParse được (03/11/2014 17:26).
            string txt = "";
            txt = (sender as TextBox).Text.Replace(",", "");

            if (!string.IsNullOrEmpty(txt))
            {

                int adjustedQty = 0;
                int.TryParse(txt, out adjustedQty);

                CurrentPharmacyEstimationForPoDetail.AdjustedQty = adjustedQty;
                CurrentPharmacyEstimationForPoDetail.PackageQty = CalQtyPackaging(adjustedQty, CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));
                CurrentPharmacyEstimationForPoDetail.TotalPrice = CurrentPharmacyEstimationForPoDetail.AdjustedQty * CurrentPharmacyEstimationForPoDetail.UnitPrice;
            }
        }


        private void PharmacyEstimationForPODetail_DrugID(long DrugID, string DrugCode)
        {
            this.ShowBusyIndicator();
            //isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyEstimationForPODetail_ByDrugID(CurrentPharmacyEstimationForPO.DateOfEstimation, DrugID, DrugCode, CurrentPharmacyEstimationForPO.V_EstimateType, IsHIStorage, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            CurrentPharmacyEstimationForPoDetail = contract.EndPharmacyEstimationForPODetail_ByDrugID(asyncResult);
                            if (CurrentPharmacyEstimationForPoDetail != null)
                            {
                                CurrentPharmacyEstimationForPoDetail.EntityState = EntityState.NEW;
                                if (DrugID == 0)
                                {
                                    CurrentRefGenericDrugDetail = CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails;
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                            }
                            txt = "";
                            txtHSN = "";
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingFullOperator = false;
                            IsComplete = true;
                            if (AxHSN != null)
                            {
                                AxHSN.Focus();
                            }
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyEstimationForPODelete(long PharmacyEstimatePoID)
        {
            this.ShowBusyIndicator();
            //isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyEstimationForPO_Delete(PharmacyEstimatePoID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndPharmacyEstimationForPO_Delete(asyncResult);
                            InittializeEstimationForPO();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);
                }
            });
            t.Start();
        }

        public void btnDeletePhieu()
        {
            if (MessageBox.Show(eHCMSResources.Z1411_G1_CoChacMuonXoaPhDuTru, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CurrentPharmacyEstimationForPO != null)
                {
                    PharmacyEstimationForPODelete(CurrentPharmacyEstimationForPO.PharmacyEstimatePoID);
                }
            }
        }

        #region IHandle<PharmacyCloseSearchEstimationEvent> Members

        public void Handle(PharmacyCloseSearchEstimationEvent message)
        {
            if (message != null)
            {
                CurrentPharmacyEstimationForPO = message.SelectedEstimation as PharmacyEstimationForPO;
                DisplayMonth();
                SetRadioButtonCompleted();
                PharmacyEstimationForPoDetail_ByParentID(CurrentPharmacyEstimationForPO.PharmacyEstimatePoID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            if (CurrentPharmacyEstimationForPO == null)
            {
                MessageBox.Show(eHCMSResources.Z1412_G1_ChonPhDuTruXemIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = CurrentPharmacyEstimationForPO.PharmacyEstimatePoID;
            DialogView.SupplierID = SelectedSupplier != null && SelectedSupplier.SupplierID > 0 ? SelectedSupplier.SupplierID : 0;
            DialogView.PCOID = SelectedPharmaceuticalCompany != null && SelectedPharmaceuticalCompany.PCOID > 0 ? SelectedPharmaceuticalCompany.PCOID : 0;
            DialogView.LyDo = eHCMSResources.Z0558_G1_BangDuTruThuoc;
            if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
            {
                DialogView.LyDo += string.Format(" {0} ", eHCMSResources.G0039_G1_Th.ToUpper()) + CurrentPharmacyEstimationForPO.DateOfEstimation.Month.ToString() + "/" + CurrentPharmacyEstimationForPO.DateOfEstimation.Year.ToString();
            }
            else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH || CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
            {
                DialogView.LyDo += string.Format(" {0} ", eHCMSResources.Z0561_G1_BoSungTh) + CurrentPharmacyEstimationForPO.DateOfEstimation.Month.ToString() + "/" + CurrentPharmacyEstimationForPO.DateOfEstimation.Year.ToString();
            }
            else if (CurrentPharmacyEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
            {
                DialogView.LyDo += string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + CurrentPharmacyEstimationForPO.DateOfEstimation.Year.ToString();
            }
            else
            {
                DialogView.LyDo += string.Format(" {0} ", eHCMSResources.Z0562_G1_BoSungNam) + CurrentPharmacyEstimationForPO.DateOfEstimation.Year.ToString();
            }
            DialogView.eItem = ReportName.PHARMACY_ESTIMATTION;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        public void btnPrint()
        {
            this.ShowBusyIndicator();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetEstimationPharmacyInPdfFormat(CurrentPharmacyEstimationForPO.PharmacyEstimatePoID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetEstimationPharmacyInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            Globals.EventAggregator.Publish(results);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }
        #endregion

        #region Paging Member
        private string _SearchKey;
        public string SearchKey
        {
            get { return _SearchKey; }
            set
            {
                _SearchKey = value;
                NotifyOfPropertyChange(() => SearchKey);
            }
        }

        private int _PCVPageSize = 15;
        public int PCVPageSize
        {
            get
            {
                return _PCVPageSize;
            }
            set
            {
                if (_PCVPageSize != value)
                {
                    _PCVPageSize = value;
                    NotifyOfPropertyChange(() => PCVPageSize);
                }
            }
        }

        private Visibility _VisibilityPaging = Visibility.Collapsed;
        public Visibility VisibilityPaging
        {
            get
            {
                return _VisibilityPaging;
            }
            set
            {
                if (_VisibilityPaging != value)
                {
                    _VisibilityPaging = value;
                    NotifyOfPropertyChange(() => VisibilityPaging);
                }
            }
        }

        private CollectionViewSource CVS_PCVEstimationDetails = null;
        public CollectionView CV_PCVEstimationDetails
        {
            get; set;
        }

        DataPager pagerStockTakes = null;
        public void pagerStockTakes_Loaded(object sender, RoutedEventArgs e)
        {
            pagerStockTakes = sender as DataPager;
        }

        CheckBox PagingChecked;
        public void Paging_Checked(object sender, RoutedEventArgs e)
        {
            //avtivate datapager
            PagingChecked = sender as CheckBox;
            pagerStockTakes.Source = grdEstimateDetails.ItemsSource.Cast<object>().ToObservableCollection();
            VisibilityPaging = Visibility.Visible;
        }

        public void Paging_Unchecked(object sender, RoutedEventArgs e)
        {
            //deavtivate datapager
            pagerStockTakes.Source = null;
            VisibilityPaging = Visibility.Collapsed;

            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            CVS_PCVEstimationDetails = null;
            if (CurrentPharmacyEstimationForPO != null && CurrentPharmacyEstimationForPO.EstimationDetails != null)
            {
                CVS_PCVEstimationDetails = new CollectionViewSource { Source = CurrentPharmacyEstimationForPO.EstimationDetails };
                CV_PCVEstimationDetails = (CollectionView)CVS_PCVEstimationDetails.View;
            }
            NotifyOfPropertyChange(() => CV_PCVEstimationDetails);

            btnFilter();
            if (PagingChecked != null && PagingChecked.IsChecked.GetValueOrDefault())
            {
                pagerStockTakes.Source = grdEstimateDetails.ItemsSource.Cast<object>().ToObservableCollection();
            }
        }

        public void cbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                PCVPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
            }
        }

        public void btnFilter()
        {
            if (CV_PCVEstimationDetails != null)
            {
                CV_PCVEstimationDetails.Filter = null;
                CV_PCVEstimationDetails.Filter = new Predicate<object>(DoFilter);
            }
        }

        //Callback method
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search
            PharmacyEstimationForPODetail emp = o as PharmacyEstimationForPODetail;
            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if (emp.RefGenMedProductDetails == null)
                {
                    return false;
                }

                if (emp.RefGenMedProductDetails.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void SearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchKey = (sender as TextBox).Text;
                btnFilter();
            }
        }

        #endregion

        private string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    PharmacyEstimationForPODetail_DrugID(0, txt);
                }
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                return _VisibilityName;
            }
            set
            {
                _VisibilityName = value;
                _VisibilityCode = !_VisibilityName;
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }

        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            VisibilityName = true;
        }

        AxTextBox AxHSN = null;
        public void HeSoNhan_Loaded(object sender, RoutedEventArgs e)
        {
            AxHSN = sender as AxTextBox;
        }

        private string txtHSN = "";
        public void AxHSN_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtHSN != (sender as AxTextBox).Text)
            {
                txtHSN = (sender as AxTextBox).Text;
                if (!string.IsNullOrEmpty(txtHSN))
                {
                    try
                    {
                        CurrentPharmacyEstimationForPoDetail.NumberOfEstimatedMonths_F = Convert.ToInt32(txtHSN);
                    }
                    catch
                    {

                    }
                    CountEstimate(CurrentPharmacyEstimationForPoDetail, false);
                }
            }
        }

        AutoCompleteBox auto;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            auto = sender as AutoCompleteBox;
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (CurrentPharmacyEstimationForPoDetail != null && CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails != null)
            {
                if (e.Parameter == CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails.BrandName)
                {
                    return;
                }

            }
            BrandName = e.Parameter;
            //tim theo ten
            RefGenericDrugDetail.PageIndex = 0;
            GetRefGenericDrugDetail_Auto(e.Parameter, RefGenericDrugDetail.PageIndex, RefGenericDrugDetail.PageSize);
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (CurrentPharmacyEstimationForPoDetail != null && CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails != null && CurrentRefGenericDrugDetail != null)
            {
                if (CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails.DrugID == CurrentRefGenericDrugDetail.DrugID)
                {
                    return;
                }
            }
            if (auto.SelectedItem != null && IsComplete)
            {
                if (CurrentPharmacyEstimationForPoDetail == null)
                {
                    CurrentPharmacyEstimationForPoDetail = new PharmacyEstimationForPODetail();
                }
                CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails = CurrentRefGenericDrugDetail;//C auto.SelectedItem as RefGenericDrugDetail;
                IsComplete = false;
                PharmacyEstimationForPODetail_DrugID(CurrentRefGenericDrugDetail.DrugID, "");
            }
        }

        public void AddItem()
        {
            if (CurrentPharmacyEstimationForPoDetail == null || CurrentPharmacyEstimationForPoDetail.RefGenMedProductDetails == null)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0410_G1_ChonThuoc), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentPharmacyEstimationForPoDetail.AdjustedQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0564_G1_SLgDuTruPhaiLonHon0, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentPharmacyEstimationForPO == null)
            {
                CurrentPharmacyEstimationForPO = new PharmacyEstimationForPO();
            }
            if (CurrentPharmacyEstimationForPO.EstimationDetails == null)
            {
                CurrentPharmacyEstimationForPO.EstimationDetails = new ObservableCollection<PharmacyEstimationForPODetail>();
            }
            var chk = CurrentPharmacyEstimationForPO.EstimationDetails.Where(x => x.RefGenMedProductDetails != null && x.RefGenMedProductDetails.DrugID == CurrentPharmacyEstimationForPoDetail.DrugID);
            if (chk != null && chk.Count() > 0)
            {
                Globals.ShowMessage(eHCMSResources.K0053_G1_ThuocDaTonTai, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //GetPackageQty(CurrentPharmacyEstimationForPoDetail);
            CurrentPharmacyEstimationForPO.EstimationDetails.Insert(0, CurrentPharmacyEstimationForPoDetail);
            LoadDataGrid();
            CurrentPharmacyEstimationForPoDetail = new PharmacyEstimationForPODetail();
            CurrentRefGenericDrugDetail = null;

            if (VisibilityName)
            {
                if (auto != null)
                {
                    auto.Text = "";
                    auto.Focus();
                }
            }
            else
            {
                if (tbx != null)
                {
                    tbx.Text = "";
                    tbx.Focus();
                }
            }
            
        }

        #region AutoGenMedProduct Member

        private RefGenericDrugDetail _CurrentRefGenericDrugDetail;
        public RefGenericDrugDetail CurrentRefGenericDrugDetail
        {
            get
            {
                return _CurrentRefGenericDrugDetail;
            }
            set
            {
                if (_CurrentRefGenericDrugDetail != value)
                {
                    _CurrentRefGenericDrugDetail = value;
                    NotifyOfPropertyChange(() => CurrentRefGenericDrugDetail);
                }
            }
        }

        private PagedSortableCollectionView<RefGenericDrugDetail> _RefGenericDrugDetail;
        public PagedSortableCollectionView<RefGenericDrugDetail> RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                if (_RefGenericDrugDetail != value)
                {
                    _RefGenericDrugDetail = value;
                    NotifyOfPropertyChange(() => RefGenericDrugDetail);
                }
            }
        }

        private void GetRefGenericDrugDetail_Auto(string BrandName, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDrugGenericDetails_RefAutoPaging(null,BrandName, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndSearchRefDrugGenericDetails_RefAutoPaging(out Total, asyncResult);
                            RefGenericDrugDetail.Clear();
                            RefGenericDrugDetail.TotalItemCount = Total;
                            foreach (RefGenericDrugDetail p in results)
                            {
                                RefGenericDrugDetail.Add(p);
                            }
                            auto.ItemsSource = RefGenericDrugDetail;
                            auto.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                           
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #endregion

        private void HideShowColumnDelete()
        {
            if (grdEstimateDetails != null)
            {
                if (CurrentPharmacyEstimationForPO.CanSave && mDuTru_Xoa)
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    grdEstimateDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    grdEstimateDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AllCheckedfc()
        {
            if (CurrentPharmacyEstimationForPO != null && CurrentPharmacyEstimationForPO.EstimationDetails != null && CurrentPharmacyEstimationForPO.EstimationDetails.Count > 0)
            {
                for (int i = 0; i < CurrentPharmacyEstimationForPO.EstimationDetails.Count; i++)
                {
                    CurrentPharmacyEstimationForPO.EstimationDetails[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (CurrentPharmacyEstimationForPO != null && CurrentPharmacyEstimationForPO.EstimationDetails != null && CurrentPharmacyEstimationForPO.EstimationDetails.Count > 0)
            {
                for (int i = 0; i < CurrentPharmacyEstimationForPO.EstimationDetails.Count; i++)
                {
                    CurrentPharmacyEstimationForPO.EstimationDetails[i].Checked = false;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (CurrentPharmacyEstimationForPO != null && CurrentPharmacyEstimationForPO.EstimationDetails != null && CurrentPharmacyEstimationForPO.EstimationDetails.Count > 0)
            {
                var items = CurrentPharmacyEstimationForPO.EstimationDetails.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (PharmacyEstimationForPODetail p in items)
                        {
                            if (p.EntityState != EntityState.NEW)
                            {
                                p.EntityState = EntityState.DETACHED;
                                EstimationDetailsDeleted.Add(p);
                            }
                        }
                        CurrentPharmacyEstimationForPO.EstimationDetails = CurrentPharmacyEstimationForPO.EstimationDetails.Where(x => x.Checked == false).ToObservableCollection();
                        LoadDataGrid();
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
                }
            }
        }




        private ObservableCollection<Supplier> _Suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get
            {
                return _Suppliers;
            }
            set
            {
                if (_Suppliers != value)
                {
                    _Suppliers = value;
                }
                NotifyOfPropertyChange(() => Suppliers);
            }
        }

        private Supplier _SelectedSupplier;
        public Supplier SelectedSupplier
        {
            get
            {
                return _SelectedSupplier;
            }
            set
            {
                if (_SelectedSupplier != value)
                {
                    _SelectedSupplier = value;
                }
                NotifyOfPropertyChange(() => SelectedSupplier);
            }
        }

        private ObservableCollection<PharmaceuticalCompany> _PharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanies
        {
            get
            {
                return _PharmaceuticalCompanies;
            }
            set
            {
                if (_PharmaceuticalCompanies != value)
                {
                    _PharmaceuticalCompanies = value;
                }
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
            }
        }

        private PharmaceuticalCompany _SelectedPharmaceuticalCompany;
        public PharmaceuticalCompany SelectedPharmaceuticalCompany
        {
            get
            {
                return _SelectedPharmaceuticalCompany;
            }
            set
            {
                if (_SelectedPharmaceuticalCompany != value)
                {
                    _SelectedPharmaceuticalCompany = value;
                }
                NotifyOfPropertyChange(() => SelectedPharmaceuticalCompany);
            }
        }

        private void GetAllSupplier()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSupplier_ByPCOIDNotPaging(null, (long)AllLookupValues.V_SupplierType.CUNGCAP_THIETBI_YTE, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetSupplier_ByPCOIDNotPaging(asyncResult);
                            Suppliers = results.ToObservableCollection();
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

            });

            t.Start();
        }


        public void GetAllPharmaceuticalCompany()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPharmaceuticalCompanyCbx(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPharmaceuticalCompanyCbx(asyncResult);
                            PharmaceuticalCompanies = results.ToObservableCollection();
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

            });

            t.Start();

        }

        public void Supplier_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox axSupplier = sender as AutoCompleteBox;
            if (axSupplier == null || Suppliers == null)
            {
                return;
            }
            axSupplier.ItemsSource = Suppliers.Where(x => StringUtil.RemoveSign4VietnameseString(x.SupplierName).ToUpper().Contains(StringUtil.RemoveSign4VietnameseString(e.Parameter.ToUpper())) || (x.SupplierCode != null && x.SupplierCode.ToUpper().Contains(e.Parameter.ToUpper())));
            axSupplier.PopulateComplete();
        }

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            Action<ISuppliers> onInitDlg = delegate (ISuppliers proAlloc)
            {
                proAlloc.IsChildWindow = true;
                proAlloc.ePharmacySupplierEvent = eFirePharmacySupplierEvent.EstimationPharmacy;
            };
            GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg);
        }

        public void btnSearchNSX()
        {
            Action<IPharmacieucalCompany> onInitDlg = delegate (IPharmacieucalCompany proAlloc)
            {
                proAlloc.IsChildWindow = true;
                proAlloc.ePharmacieucalCompany = eFirePharmacieucalCompanyEvent.EstimationPharmacy;
            };
            GlobalsNAV.ShowDialog<IPharmacieucalCompany>(onInitDlg);
        }

        public void Handle(PharmacySupplierToEstimationEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedSupplier = message.SelectedSupplier as Supplier;
            }
        }

        public void Handle(PharmaceuticalCompanyToEstimationEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedPharmaceuticalCompany = message.SelectedPharmaceuticalCompany as PharmaceuticalCompany;
            }
        }


        public void btnExportExcel()
        {
            if (CurrentPharmacyEstimationForPO == null || CurrentPharmacyEstimationForPO.PharmacyEstimatePoID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0379_G1_ChonPhDuTruDeXuatExcel, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "Excel xls (*.xls)|*.xls",
                //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }

            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.DU_TRU;
            RptParameters.EstimatePoID = CurrentPharmacyEstimationForPO.PharmacyEstimatePoID;
            RptParameters.Show = "DuTru";

            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
        }

        AxTextBox txtNumberOfEstimatedMonths = null;
        public void txtNumberOfEstimatedMonths_Loaded(object sender, RoutedEventArgs e)
        {
            txtNumberOfEstimatedMonths = sender as AxTextBox;
        }

    }
}
