using eHCMSLanguage;
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
using System.Collections.Generic;
using aEMR.Common;
using System.Windows.Media;
using aEMR.CommonTasks;
using aEMR.Controls;
using Castle.Windsor;
using aEMR.DrugDept.Views;
using System.Windows.Data;
using aEMR.Common.BaseModel;
/*
* 20181006 #001 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
*                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc.
* 20190604 #002 TTM:   BM 0011781: Thêm dự trù cho các kho mới
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IEstimationDrugDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EstimationDrugDeptViewModel : ViewModelBase, IEstimationDrugDept
        , IHandle<DrugDeptCloseSearchEstimationEvent>
    {
        //private enum DataGridCol
        //{
        //    ColMutiDelete = 0,
        //    ColDelete = 1,
        //    MaThuoc = 2,
        //    TenThuoc = 3,
        //    HangSX = 4,
        //    QCDongGoi = 5,
        //    SLQC = 6,
        //    SLTon = 7,
        //    T1 = 8,
        //    T2 = 9,
        //    T3 = 10,
        //    T4 = 11,
        //    TBX = 12,
        //    DoUuTien = 13,
        //    HeSoNhan = 14,
        //    SLLyThuyet = 15,
        //    SLTinhTheoQC = 16,
        //    SLDieuChinh = 17,
        //    DVT = 18,
        //    XuatT12 = 19,
        //    ThangHienTai = 20
        //}

        private enum DataGridCol
        {
            ColMutiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3,
            TenHoatChat = 4,
            NongDo = 5,
            QCDongGoi = 6,
            DVT = 7,
            DuongDung = 8,
            DonGia = 9,
            SLXuat = 10,
            SLTonCuoi = 11,
            SLLyThuyet = 12,
            SLDieuChinh = 13,
            ThanhTien = 14,
            SLThauConLai = 15,
            DotThau = 16,
            TenNCC = 17,
            Note = 18
        }

        [ImportingConstructor]
        public EstimationDrugDeptViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Authorization();
            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new RequestSearchCriteria();
            EstimationDetailsDeleted = new ObservableCollection<DrugDeptEstimationForPoDetail>();

            CurrentMonthDisplay = new MonthDisplay();
            CurrentDrugDeptEstimationForPO = new DrugDeptEstimationForPO();
            CurrentDrugDeptEstimationForPO.DateOfEstimation = DateTime.Now;

            InittializeEstimationForPO();

            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            RefGenericDrugDetail = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenericDrugDetail.OnRefresh += new EventHandler<RefreshEventArgs>(RefGenericDrugDetail_OnRefresh);
            RefGenericDrugDetail.PageSize = Globals.PageSize;
            //LoadValidBidFromSupplierID(0);

            CommonGlobals.GetAllPositionInHospital(this);

            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
        }

        void RefGenericDrugDetail_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenericDrugDetail_Auto(BrandName, RefGenericDrugDetail.PageIndex, RefGenericDrugDetail.PageSize);
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account
        private bool _mTim = true;
        private bool _mThemMoi = true;
        private bool _mXoa = true;
        private bool _mXemIn = true;
        public bool mTim
        {
            get
            {
                return _mTim;
            }
            set
            {
                if (_mTim == value)
                    return;
                _mTim = value;
                NotifyOfPropertyChange(() => mTim);
            }
        }
        public bool mThemMoi
        {
            get
            {
                return _mThemMoi;
            }
            set
            {
                if (_mThemMoi == value)
                    return;
                _mThemMoi = value;
                NotifyOfPropertyChange(() => mThemMoi);
            }
        }
        public bool mXoa
        {
            get
            {
                return _mXoa;
            }
            set
            {
                if (_mXoa == value)
                    return;
                _mXoa = value;
                NotifyOfPropertyChange(() => mXoa);
            }
        }
        public bool mXemIn
        {
            get
            {
                return _mXemIn;
            }
            set
            {
                if (_mXemIn == value)
                    return;
                _mXemIn = value;
                NotifyOfPropertyChange(() => mXemIn);
            }
        }
        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }
        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion

        #region binding visibilty
        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }
        #endregion

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

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                    NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1);
                }

            }
        }

        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return V_MedProductType == (long)AllLookupValues.MedProductType.THUOC; }
        }

        private int _CDC;
        public int CDC
        {
            get
            {
                return _CDC;
            }
            set
            {
                _CDC = value;
                NotifyOfPropertyChange(() => CDC);
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
                    _HeSoNhan = value;
                    NotifyOfPropertyChange(() => HeSoNhan);
                    if (_HeSoNhan != 0)
                    {
                        CountEstimateAll();
                    }
                }
            }
        }

        private ObservableCollection<DrugDeptEstimationForPO> _DrugDeptEstimationForPOCbx;
        public ObservableCollection<DrugDeptEstimationForPO> DrugDeptEstimationForPOCbx
        {
            get
            {
                return _DrugDeptEstimationForPOCbx;
            }
            set
            {
                if (_DrugDeptEstimationForPOCbx != value)
                {
                    _DrugDeptEstimationForPOCbx = value;
                    NotifyOfPropertyChange(() => DrugDeptEstimationForPOCbx);
                }
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

        private DrugDeptEstimationForPO _CurrentDrugDeptEstimationForPO;
        public DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO
        {
            get
            {
                return _CurrentDrugDeptEstimationForPO;
            }
            set
            {
                if (_CurrentDrugDeptEstimationForPO != value)
                {
                    _CurrentDrugDeptEstimationForPO = value;
                    NotifyOfPropertyChange(() => CurrentDrugDeptEstimationForPO);
                }
                if (_CurrentDrugDeptEstimationForPO != null)
                {
                    IsOrder = _CurrentDrugDeptEstimationForPO.IsOrder;
                }
                else
                {
                    IsOrder = true;
                }
                NotifyOfPropertyChange(() => IsOrder);
            }
        }

        private DrugDeptEstimationForPoDetail _CurrentDrugDeptEstimationForPoDetail;
        public DrugDeptEstimationForPoDetail CurrentDrugDeptEstimationForPoDetail
        {
            get
            {
                return _CurrentDrugDeptEstimationForPoDetail;
            }
            set
            {
                _CurrentDrugDeptEstimationForPoDetail = value;
                NotifyOfPropertyChange(() => CurrentDrugDeptEstimationForPoDetail);
            }
        }

        private ObservableCollection<DrugDeptEstimationForPoDetail> EstimationDetailsDeleted;

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

        private PagedSortableCollectionView<DrugDeptEstimationForPoDetail> _DrugDeptEstimationForPoDetails;
        public PagedSortableCollectionView<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetails
        {
            get
            {
                return _DrugDeptEstimationForPoDetails;
            }
            set
            {
                if (_DrugDeptEstimationForPoDetails != value)
                {
                    _DrugDeptEstimationForPoDetails = value;
                }
                NotifyOfPropertyChange(() => DrugDeptEstimationForPoDetails);
            }
        }

        private bool? _IsOrder;
        public bool? IsOrder
        {
            get
            {
                return _IsOrder;
            }
            set
            {
                _IsOrder = value;
                NotifyOfPropertyChange(() => IsOrder);
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

        private bool _IsForeign;
        public bool IsForeign
        {
            get
            {
                return _IsForeign;
            }
            set
            {
                _IsForeign = value;
                if (_IsForeign)
                {
                    IsTrongNuoc = !_IsForeign;
                    IsAll = !_IsForeign;
                }
                NotifyOfPropertyChange(() => IsForeign);
            }
        }

        private bool _IsTrongNuoc = true;
        public bool IsTrongNuoc
        {
            get
            {
                return _IsTrongNuoc;
            }
            set
            {
                _IsTrongNuoc = value;
                if (_IsTrongNuoc)
                {
                    IsForeign = !_IsTrongNuoc;
                    IsAll = !_IsTrongNuoc;
                }
                NotifyOfPropertyChange(() => IsTrongNuoc);
            }
        }

        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }
        private ObservableCollection<Bid> _BidCollection;
        public ObservableCollection<Bid> BidCollection
        {
            get
            {
                return _BidCollection;
            }
            set
            {
                _BidCollection = value;
                NotifyOfPropertyChange(() => BidCollection);
            }
        }
        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void InittializeEstimationForPO()
        {
            CurrentDrugDeptEstimationForPO = new DrugDeptEstimationForPO
            {
                DateOfEstimation = DateTime.Now,
                StaffID = GetStaffLogin().StaffID
            };
            SetDefultRefGenericDrugCategory();
            CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.FIRSTMONTH;
            SetRadioButtonCompleted();
            EstimationDetailsDeleted.Clear();
            HideShowColumnDelete();
            CV_PCVEstimationDetails = null;
            NotifyOfPropertyChange(() => CV_PCVEstimationDetails);
            VisibilityAdd = Visibility.Collapsed;
            IsEnableCanOK = true;
            IsEnableDeletePhieu = false;
        }

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, true);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            yield break;
        }

        private void SetDefultRefGenericDrugCategory()
        {
            if (CurrentDrugDeptEstimationForPO != null && RefGenericDrugCategory_1s != null)
            {
                CurrentDrugDeptEstimationForPO.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }

        private void CountEstimateAll()
        {
            if (CurrentDrugDeptEstimationForPO.EstimationDetails != null)
            {
                foreach (DrugDeptEstimationForPoDetail p in CurrentDrugDeptEstimationForPO.EstimationDetails)
                {
                    p.NumberOfEstimatedMonths_F = HeSoNhan;
                    p.EstimatedQty_F = (p.NumberOfEstimatedMonths_F * p.OutAverageQty) - p.RemainQty;
                    if (p.RefGenMedProductDetails != null && p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault() != 0)
                    {
                        if (p.EstimatedQty_F > 0)
                        {
                            p.QtyPackaging = Math.Round(p.EstimatedQty_F / p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(), 2);
                        }
                        else
                        {
                            p.QtyPackaging = 0;
                        }
                        p.AdjustedQty = (int)Math.Ceiling(p.QtyPackaging * p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault());
                        if (p.EntityState == Service.Core.Common.EntityState.PERSITED || p.EntityState == Service.Core.Common.EntityState.MODIFIED)
                        {
                            p.EntityState = Service.Core.Common.EntityState.MODIFIED;
                        }
                    }
                }
            }
        }

        public bool CheckDeleted(object item)
        {
            DrugDeptEstimationForPoDetail temp = item as DrugDeptEstimationForPoDetail;
            if (temp != null && temp.EntityState == Service.Core.Common.EntityState.DETACHED)
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

        private bool CheckedExists = false;

        private void DrugDeptEstimationForPO_FullOperator()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPO_FullOperator(V_MedProductType, CurrentDrugDeptEstimationForPO, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                DrugDeptEstimationForPO EstimateOut;
                                long results = contract.EndDrugDeptEstimationForPO_FullOperator(out EstimateOut, asyncResult);
                                EstimationDetailsDeleted.Clear();
                                SetRadioButtonCompleted();
                                if (results >= 0)
                                {
                                    CurrentDrugDeptEstimationForPO = EstimateOut;
                                    LoadDataGrid();
                                }
                                else
                                {
                                    InittializeEstimationForPO();
                                }
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK));
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

        private void DrugDeptEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPO_CheckExists(V_EstimateType, DateOfEstimation, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CheckedExists = contract.EndDrugDeptEstimationForPO_CheckExists(asyncResult);
                                if (CheckedExists)
                                {
                                    if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
                                    {
                                        //Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0548_G1_DaCoDuTruDauTh), eHCMSResources.G0442_G1_TBao);
                                        Globals.ShowMessage(string.Format("{0}!", "Đã có dự trù ngày trong khoản thời gian đã chọn"), eHCMSResources.G0442_G1_TBao);
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0549_G1_DaCoDuTruDauNam), eHCMSResources.G0442_G1_TBao);
                                    }
                                }
                                else
                                {
                                    DisplayMonth();
                                    //GetEstimationForMonthDrugDept();
                                    GetEstimationDrugDept_V2();
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

        private void SetRadioButtonCompleted()
        {
            if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
            {
                IsFirstMonth = true;
                ReadOnlyColumnTrue();
                //VisibilityAdd = Visibility.Collapsed;
            }
            else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH)
            {
                IsAdditionFirstMonth = true;
                ReadOnlyColumnTrue();
            }
            else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
            {
                IsModifyMonth = true;
                ReadOnlyColumnFalse();

            }
            else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
            {
                IsFirstYear = true;
                ReadOnlyColumnTrue();
                // VisibilityAdd = Visibility.Collapsed;
            }
            else
            {
                IsModifyYear = true;
                ReadOnlyColumnFalse();
                //if (CurrentDrugDeptEstimationForPO.IsOrder.GetValueOrDefault())
                //{
                //    VisibilityAdd = Visibility.Collapsed;
                //}
                //else
                //{
                //    VisibilityAdd = Visibility.Visible;
                //}
            }
            ShowOrHideAddProduct();
        }

        private void ShowOrHideAddProduct()
        {
            if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.DrugDeptEstimatePoID > 0)
            {
                if (CurrentDrugDeptEstimationForPO.IsOrder.GetValueOrDefault())
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
        //    bool isAdd = true;
        //    if (CurrentDrugDeptEstimationForPO.EstimationDetails != null && CurrentDrugDeptEstimationForPO.EstimationDetails.Count > 0)
        //    {
        //        if (EstimationDetailsDeleted.Count > 0)
        //        {
        //            if (CurrentDrugDeptEstimationForPO.EstimationDetails == null)
        //            {
        //                CurrentDrugDeptEstimationForPO.EstimationDetails = new ObservableCollection<DrugDeptEstimationForPoDetail>();
        //            }
        //            for (int i = 0; i < EstimationDetailsDeleted.Count; i++)
        //            {
        //                CurrentDrugDeptEstimationForPO.EstimationDetails.Add(EstimationDetailsDeleted[i]);
        //            }
        //        }

        //        for (int i = 0; i < CurrentDrugDeptEstimationForPO.EstimationDetails.Count; i++)
        //        {
        //            if (CurrentDrugDeptEstimationForPO.EstimationDetails[i].RefGenMedProductDetails != null && CurrentDrugDeptEstimationForPO.EstimationDetails[i].RefGenMedProductDetails.GenMedProductID > 0)
        //            {
        //                if (CurrentDrugDeptEstimationForPO.EstimationDetails[i].AdjustedQty <= 0)
        //                {
        //                    isAdd = false;
        //                    if (MessageBox.Show("1 số mặt hàng có SL dự trù <= 0." + Environment.NewLine + eHCMSResources.Z0551_G1_CoMuonChTrTuDongXoaMH, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //                    {
        //                        AutoDeleteEstimationDetailsZero();
        //                    }
        //                    //break;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        isAdd = false;
        //        MessageBox.Show(eHCMSResources.K0433_G1_NhapHgCanDuTru);
        //    }
        //    if (isAdd)
        //    {
        //        DrugDeptEstimationForPO_FullOperator();
        //    }
        //}

        private bool CheckEstimationDetails()
        {
            ObservableCollection<DrugDeptEstimationForPoDetail> EstimationDetailList = new ObservableCollection<DrugDeptEstimationForPoDetail>();
            string strMessage = "";
            //if (CurrentDrugDeptEstimationForPO.EstimationDetails.Any(x => Globals.ServerConfigSection.MedDeptElements.UseBidDetailOnInward && x.BidDetailID > 0 && x.BidRemainingQty < x.AdjustedQty))
            //{
            //    if (MessageBox.Show(eHCMSResources.Z2736_G1_SoLuongCNTKoHopLe + Environment.NewLine + eHCMSResources.I0940_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //    {
            //        foreach (var eItem in CurrentDrugDeptEstimationForPO.EstimationDetails.Where(x => Globals.ServerConfigSection.MedDeptElements.UseBidDetailOnInward && x.BidDetailID > 0 && x.BidRemainingQty < x.AdjustedQty))
            //        {
            //            eItem.AdjustedQty = eItem.BidRemainingQty;
            //        }
            //    }
            //    return false;
            //}
            if (CurrentDrugDeptEstimationForPO.IsTrongNuoc == true)
            {
                strMessage = eHCMSResources.G1800_G1_TrongNuoc.ToLower();
                EstimationDetailList = CurrentDrugDeptEstimationForPO.EstimationDetails.Where(x => x.RefGenMedProductDetails != null && x.RefGenMedProductDetails.SupplierMain != null && x.RefGenMedProductDetails.SupplierMain.IsForeign == true).ToObservableCollection();
            }
            if (CurrentDrugDeptEstimationForPO.IsForeign == true)
            {
                strMessage = eHCMSResources.N0145_G1_NgoaiNuoc.ToLower();
                EstimationDetailList = CurrentDrugDeptEstimationForPO.EstimationDetails.Where(x => x.RefGenMedProductDetails != null && x.RefGenMedProductDetails.SupplierMain != null && x.RefGenMedProductDetails.SupplierMain.IsForeign.GetValueOrDefault() == false).ToObservableCollection();
            }
            if (EstimationDetailList != null && EstimationDetailList.Count > 0)
            {
                string BrandName = "";

                int nCount = 0;

                foreach (DrugDeptEstimationForPoDetail item in EstimationDetailList)
                {
                    if (nCount < 15)
                    {
                        BrandName += item.RefGenMedProductDetails.BrandName + Environment.NewLine;
                        nCount++;
                    }
                    else
                    {
                        BrandName += "..." + Environment.NewLine;
                        break;
                    }
                }

                if (MessageBox.Show(string.Format(eHCMSResources.Z1309_G1_I, strMessage, BrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (CurrentDrugDeptEstimationForPO == null || CurrentDrugDeptEstimationForPO.EstimationDetails == null || CurrentDrugDeptEstimationForPO.EstimationDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.K0433_G1_NhapHgCanDuTru);
                return;
            }
            if (CurrentDrugDeptEstimationForPO.EstimationDetails.Any(x => x.AdjustedQty <= 0))
            {
                if (MessageBox.Show(eHCMSResources.Z0550_G1_MatHangCoSLgNhoHon0 + Environment.NewLine + eHCMSResources.Z0551_G1_CoMuonChTrTuDongXoaMH, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    AutoDeleteEstimationDetailsZero();
                }
            }
            if (!CheckEstimationDetails())
            {
                return;
            }
            if (EstimationDetailsDeleted.Count > 0)
            {
                for (int i = 0; i < EstimationDetailsDeleted.Count; i++)
                {
                    CurrentDrugDeptEstimationForPO.EstimationDetails.Add(EstimationDetailsDeleted[i]);
                }
            }
            DrugDeptEstimationForPO_FullOperator();
        }

        private void AutoDeleteEstimationDetailsZero()
        {
            if (CurrentDrugDeptEstimationForPO.EstimationDetails != null)
            {
                ObservableCollection<DrugDeptEstimationForPoDetail> lst = CurrentDrugDeptEstimationForPO.EstimationDetails.DeepCopy();
                var items = lst.Where(x => x.AdjustedQty <= 0);
                if (items != null)
                {
                    foreach (DrugDeptEstimationForPoDetail p in items)
                    {
                        if (p.EntityState != Service.Core.Common.EntityState.NEW)
                        {
                            p.EntityState = Service.Core.Common.EntityState.DETACHED;
                            EstimationDetailsDeleted.Add(p);
                        }
                        //CurrentDrugDeptEstimationForPO.EstimationDetails.Remove(p);
                    }
                    CurrentDrugDeptEstimationForPO.EstimationDetails = lst.Where(x => x.AdjustedQty > 0).ToObservableCollection();
                    LoadDataGrid();
                }
            }
        }

        private void DeleteEstimationDetails(object item)
        {
            if (CurrentDrugDeptEstimationForPO.EstimationDetails != null)
            {
                DrugDeptEstimationForPoDetail p = (DrugDeptEstimationForPoDetail)item;
                if (p.EntityState != Service.Core.Common.EntityState.NEW)
                {
                    p.EntityState = Service.Core.Common.EntityState.DETACHED;
                    EstimationDetailsDeleted.Add(p);
                }
                CurrentDrugDeptEstimationForPO.EstimationDetails.Remove(p);
                LoadDataGrid();
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (grdEstimateDetails != null && grdEstimateDetails.SelectedItem != null)
                {
                    DeleteEstimationDetails(grdEstimateDetails.SelectedItem);
                }
            }
        }

        private void DisplayMonth()
        {
            DateTime Date = CurrentDrugDeptEstimationForPO.DateOfEstimation;
            int month = Date.Month;
            CurrentMonthDisplay.CurrentT = eHCMSResources.Z0622_G1_SLgXuatT + month.ToString();
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
            //HideOrShowColumnDataGrid();
            //DisplayColumnDataGrid();
            HideShowColumnDelete();
        }

        DataGrid grdEstimateDetails = null;
        public void grdEstimateDetails_Loaded(object sender, RoutedEventArgs e)
        {
            grdEstimateDetails = sender as DataGrid;
            DisplayMonth();
        }

        //Không dùng nữa 20210831
        //private void HideOrShowColumnDataGrid()
        //{
        //    if (grdEstimateDetails != null)
        //    {
        //        if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH || CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH || CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
        //        {
        //            grdEstimateDetails.Columns[(int)DataGridCol.T1].Visibility = Visibility.Visible;
        //            grdEstimateDetails.Columns[(int)DataGridCol.T2].Visibility = Visibility.Visible;
        //            grdEstimateDetails.Columns[(int)DataGridCol.T3].Visibility = Visibility.Visible;
        //            grdEstimateDetails.Columns[(int)DataGridCol.T4].Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            grdEstimateDetails.Columns[(int)DataGridCol.T1].Visibility = Visibility.Collapsed;
        //            grdEstimateDetails.Columns[(int)DataGridCol.T2].Visibility = Visibility.Collapsed;
        //            grdEstimateDetails.Columns[(int)DataGridCol.T3].Visibility = Visibility.Collapsed;
        //            grdEstimateDetails.Columns[(int)DataGridCol.T4].Visibility = Visibility.Collapsed;
        //        }
        //    }
        //}

        private void HideShowColumnDelete()
        {
            if (grdEstimateDetails != null)
            {
                if (CurrentDrugDeptEstimationForPO.CanOK)
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    grdEstimateDetails.Columns[(int)DataGridCol.ColMutiDelete].Visibility = Visibility.Collapsed;
                }
                else
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    grdEstimateDetails.Columns[(int)DataGridCol.ColMutiDelete].Visibility = Visibility.Visible;
                }
            }
        }

        //Không dùng nữa 20210831
        //private void DisplayColumnDataGrid()
        //{
        //    if (grdEstimateDetails != null)
        //    {
        //        grdEstimateDetails.Columns[(int)DataGridCol.T1].Header = CurrentMonthDisplay.T1;
        //        grdEstimateDetails.Columns[(int)DataGridCol.T2].Header = CurrentMonthDisplay.T2;
        //        grdEstimateDetails.Columns[(int)DataGridCol.T3].Header = CurrentMonthDisplay.T3;
        //        grdEstimateDetails.Columns[(int)DataGridCol.T4].Header = CurrentMonthDisplay.T4;
        //        grdEstimateDetails.Columns[(int)DataGridCol.ThangHienTai].Header = CurrentMonthDisplay.CurrentT;
        //    }
        //}

        //KMx: Test
        //private void ResetView()
        //{
        //    CurrentDrugDeptEstimationForPO = null;
        //    CurrentDrugDeptEstimationForPO = new DrugDeptEstimationForPO();
        //    CurrentDrugDeptEstimationForPO.DateOfEstimation = Globals.GetCurServerDateTime();
        //    CurrentDrugDeptEstimationForPO.StaffID = GetStaffLogin().StaffID;
        //    EstimationDetailsDeleted.Clear();
        //    VisibilityAdd = Visibility.Collapsed;
        //    LoadDataGrid();

        //    if (IsFirstMonth)
        //    {
        //        CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.FIRSTMONTH;
        //        ReadOnlyColumnTrue();
        //    }
        //    else if (IsAdditionFirstMonth)
        //    {
        //        CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH;
        //        ReadOnlyColumnTrue();
        //    }
        //    else if (IsModifyMonth)
        //    {
        //        CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.MODIFYMONTH;
        //        ReadOnlyColumnFalse();
        //    }
        //    else if (IsFirstYear)
        //    {
        //        CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.FIRSTYEAR;
        //        ReadOnlyColumnTrue();
        //    }
        //    else
        //    {
        //        CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.MODIFYYEAR;
        //        ReadOnlyColumnFalse();
        //    }

        //}

        public void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.EstimationDetails != null && CurrentDrugDeptEstimationForPO.EstimationDetails.Count > 0)
            {
                if (MessageBox.Show(eHCMSResources.Z0555_G1_I, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
                    {
                        IsFirstMonth = true;
                    }
                    else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH)
                    {
                        IsAdditionFirstMonth = true;
                    }
                    else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
                    {
                        IsModifyMonth = true;
                    }
                    else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
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

            CurrentDrugDeptEstimationForPO = null;
            CurrentDrugDeptEstimationForPO = new DrugDeptEstimationForPO
            {
                DateOfEstimation = Globals.GetCurServerDateTime(),
                StaffID = GetStaffLogin().StaffID
            };
            EstimationDetailsDeleted.Clear();
            VisibilityAdd = Visibility.Collapsed;
            LoadDataGrid();

            if (IsFirstMonth)
            {
                CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.FIRSTMONTH;
                ReadOnlyColumnTrue();
            }
            else if (IsAdditionFirstMonth)
            {
                CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH;
                ReadOnlyColumnTrue();
            }
            else if (IsModifyMonth)
            {
                CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.MODIFYMONTH;
                ReadOnlyColumnFalse();
            }
            else if (IsFirstYear)
            {
                CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.FIRSTYEAR;
                ReadOnlyColumnTrue();
            }
            else
            {
                CurrentDrugDeptEstimationForPO.V_EstimateType = (long)AllLookupValues.V_EstimateType.MODIFYYEAR;
                ReadOnlyColumnFalse();
            }
        }

        public void RadioButtonForeign_Click(object sender, RoutedEventArgs e)
        {
            //if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.EstimationDetails != null && CurrentDrugDeptEstimationForPO.EstimationDetails.Count > 0)
            //{
            //    if (MessageBox.Show("Chương trình sẽ tự động xóa hàng bên dưới, bạn có đồng ý đổi không?", eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            //    {
            //        CurrentDrugDeptEstimationForPO.IsForeign = !CurrentDrugDeptEstimationForPO.IsForeign;
            //        CurrentDrugDeptEstimationForPO.IsTrongNuoc = !CurrentDrugDeptEstimationForPO.IsTrongNuoc;
            //        return;
            //    }

            //    CurrentDrugDeptEstimationForPO.EstimationDetails.Clear();
            //    LoadDataGrid();
            //}
            if (CurrentDrugDeptEstimationForPO != null)
            {
                if(IsForeign)
                {
                    CurrentDrugDeptEstimationForPO.IsForeign = true;
                }
                else if(IsTrongNuoc)
                {
                    CurrentDrugDeptEstimationForPO.IsForeign = false;
                }
                else
                {
                    CurrentDrugDeptEstimationForPO.IsForeign = null;
                }
            }
        }

        private void ReadOnlyColumnFalse()
        {
            if (grdEstimateDetails != null)
            {
                if (IsOrder.GetValueOrDefault())
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                }
                else
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                }
                //grdEstimateDetails.Columns[(int)DataGridCol.TenThuoc].IsReadOnly = false;
                //grdEstimateDetails.Columns[(int)DataGridCol.TenThuoc].CellStyle = null;
                //grdEstimateDetails.Columns[(int)DataGridCol.MaThuoc].IsReadOnly = false;
                //grdEstimateDetails.Columns[(int)DataGridCol.MaThuoc].CellStyle = null;
            }
        }

        private void ReadOnlyColumnTrue()
        {
            if (grdEstimateDetails != null)
            {
                if (IsOrder.GetValueOrDefault())
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                }
                else
                {
                    grdEstimateDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                }
                //grdEstimateDetails.Columns[(int)DataGridCol.TenThuoc].IsReadOnly = true;
                //grdEstimateDetails.Columns[(int)DataGridCol.TenThuoc].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyLeft"];
                //grdEstimateDetails.Columns[(int)DataGridCol.MaThuoc].IsReadOnly = true;
                //grdEstimateDetails.Columns[(int)DataGridCol.MaThuoc].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyLeft"];
            }
        }

        private void DrugDeptEstimationForPoDetail_ByParentID(long DrugDeptEstimateID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            SearchKey = "";
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPoDetail_ByParentID(DrugDeptEstimateID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptEstimationForPoDetail_ByParentID(asyncResult);
                                CurrentDrugDeptEstimationForPO.EstimationDetails = results.ToObservableCollection();
                                EstimationDetailsDeleted.Clear();

                                //KMx: Tính SLĐG dưới Database luôn, không cần lên đây tính (19/11/2014 14:49).
                                //tinh lai so luong dong goi
                                //GetPackageQtyAll();
                                LoadDataGrid();
                                IsEnableDeletePhieu = true;
                                IsEnableCanOK = false;
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

        private void DrugDeptEstimationForPO_Search(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPO_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndDrugDeptEstimationForPO_Search(out Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        this.HideBusyIndicator();
                                        void onInitDlg(IEstimationDrugDeptSearch proAlloc)
                                        {
                                            proAlloc.SearchCriteria = SearchCriteria;
                                            proAlloc.V_MedProductType = V_MedProductType;
                                            //▼====== #002
                                            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z0556_G1_TimPhDuTru.ToUpper();
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z0616_G1_TimPhDuTruYCu.ToUpper();
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z3227_G1_TimPhDuTruDDuong.ToUpper();
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z0617_G1_TimPhDuTruHChat.ToUpper();
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2707_G1_TimPhDuTruVTYTTH.ToUpper();
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2708_G1_TimPhDuTruVaccine.ToUpper();
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2709_G1_TimPhDutruBlood.ToUpper();
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2710_G1_TimPhDuTruVPP.ToUpper();
                                            }
                                            else if (V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2711_G1_TimPhDuTruVTTH.ToUpper();
                                            }
                                            else
                                            {
                                                proAlloc.strHienThi = eHCMSResources.Z2712_G1_TimPhDuTruThanhTrung.ToUpper();
                                            }
                                            //▲====== #002

                                            proAlloc.DrugDeptEstimationForPOList.Clear();
                                            proAlloc.DrugDeptEstimationForPOList.TotalItemCount = Total;
                                            proAlloc.DrugDeptEstimationForPOList.PageIndex = 0;
                                            foreach (DrugDeptEstimationForPO p in results)
                                            {
                                                proAlloc.DrugDeptEstimationForPOList.Add(p);
                                            }
                                        }
                                        GlobalsNAV.ShowDialog<IEstimationDrugDeptSearch>(onInitDlg);
                                    }
                                    else
                                    {
                                        //lay thang gia tri
                                        this.HideBusyIndicator();
                                        ChangeValue(CurrentDrugDeptEstimationForPO.RefGenDrugCatID_1, results.FirstOrDefault().RefGenDrugCatID_1);
                                        CurrentDrugDeptEstimationForPO = results.FirstOrDefault();
                                        HideShowColumnDelete();
                                        SetRadioButtonCompleted();
                                        DrugDeptEstimationForPoDetail_ByParentID(CurrentDrugDeptEstimationForPO.DrugDeptEstimatePoID);
                                    }
                                }

                                else
                                {
                                    Globals.ShowMessage("Chưa có phiếu dự trù nào", eHCMSResources.G0442_G1_TBao);
                                    return;
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

        public void btnSearch()
        {
            DrugDeptEstimationForPO_Search(0, Globals.PageSize);
        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }

        private void GetEstimationForMonthDrugDept()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetEstimationForMonth(V_MedProductType, CurrentDrugDeptEstimationForPO, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetEstimationForMonth(asyncResult);
                                CurrentDrugDeptEstimationForPO.EstimationDetails = results.ToObservableCollection();
                                if (CurrentDrugDeptEstimationForPO.EstimationDetails != null)
                                {
                                    for (int i = 0; i < CurrentDrugDeptEstimationForPO.EstimationDetails.Count; i++)
                                    {
                                        CurrentDrugDeptEstimationForPO.EstimationDetails[i].EntityState = Service.Core.Common.EntityState.NEW;
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

        public void btnOK(object sender, RoutedEventArgs e)
        {
            if (CheckDatetime())
            {
                if (((EstimationDrugDeptView)GetView()).dpkEstimateDate.SelectedDate != null)
                {
                    if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH || CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
                    {
                        DrugDeptEstimationForPO_CheckExists(CurrentDrugDeptEstimationForPO.V_EstimateType.GetValueOrDefault(), CurrentDrugDeptEstimationForPO.DateOfEstimation);
                    }
                    else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH)
                    {
                        DisplayMonth();
                        //GetEstimationForMonthDrugDept();
                        GetEstimationDrugDept_V2();
                        VisibilityAdd = Visibility.Visible;
                    }
                    else
                    {
                        DisplayMonth();
                        VisibilityAdd = Visibility.Visible;
                    }
                }
            }
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            InittializeEstimationForPO();
        }

        private void CountEstimate(object item, bool IsGrid)
        {
            DrugDeptEstimationForPoDetail p = (DrugDeptEstimationForPoDetail)item;
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
                        //p.QtyPackaging = Math.Round((double)p.EstimatedQty / (double)p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(), 2);
                        p.QtyPackaging = Math.Ceiling(CalQtyPackaging(p.EstimatedQty_F, p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1)));
                    }
                    else
                    {
                        p.QtyPackaging = 0;
                    }

                    //p.AdjustedQty = (int)Math.Ceiling(p.QtyPackaging * p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault());
                    p.AdjustedQty = CalAdjustedQty(p.QtyPackaging, p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));

                    p.TotalPrice = p.AdjustedQty * p.UnitPrice;

                    if (p.EntityState == Service.Core.Common.EntityState.PERSITED || p.EntityState == Service.Core.Common.EntityState.MODIFIED)
                    {
                        p.EntityState = Service.Core.Common.EntityState.MODIFIED;
                    }
                }
            }
        }

        private void ChangeEntityState(object item)
        {
            DrugDeptEstimationForPoDetail p = item as DrugDeptEstimationForPoDetail;
            if (p != null)
            {
                if (p.EntityState == Service.Core.Common.EntityState.PERSITED)
                {
                    p.EntityState = Service.Core.Common.EntityState.MODIFIED;
                }
            }
        }

        private void GetPackageQty(object item)
        {
            ////hien khong su dung ham nay nua
            //DrugDeptEstimationForPoDetail p = (DrugDeptEstimationForPoDetail)item;
            //if (p.RefGenMedProductDetails != null && p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1) > 0)
            //{
            //    p.QtyPackaging = Math.Round((double)p.AdjustedQty / p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1), 2);
            //}
            //if (p.EntityState == EntityState.PERSITED || p.EntityState == EntityState.MODIFIED)
            //{
            //    p.EntityState = EntityState.MODIFIED;
            //}
        }

        //KMx: Tính số lượng đóng gói dưới database luôn, không cần lên đây tính (17/11/2014 17:11).
        //private void GetPackageQtyAll()
        //{
        //    if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.EstimationDetails != null)
        //    {
        //        foreach (DrugDeptEstimationForPoDetail p in CurrentDrugDeptEstimationForPO.EstimationDetails)
        //        {
        //            if (p.RefGenMedProductDetails != null && p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1) > 0)
        //            {
        //                p.QtyPackaging = Math.Round((double)p.AdjustedQty / p.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1), 2);
        //            }
        //        }
        //    }
        //}

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
            //if (e.Column.DisplayIndex == (int)DataGridCol.HeSoNhan)
            //{
            //    CountEstimate(e.Row.DataContext, true);
            //}
            //if (e.Column.DisplayIndex == (int)DataGridCol.SLDieuChinh)
            //{
            //    GetPackageQty(e.Row.DataContext);
            //}
            if (grdEstimateDetails != null)
            {
                if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH || CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH || CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
                {
                    ChangeEntityState(grdEstimateDetails.SelectedItem);
                }
            }

            DrugDeptEstimationForPoDetail item = e.Row.DataContext as DrugDeptEstimationForPoDetail;

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
                item.AdjustedQty = CalAdjustedQty(item.QtyPackaging, item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));
                item.TotalPrice = item.AdjustedQty * item.UnitPrice;
            }

            //if (CurrentColumnName == "colAdjustedQty" && item.RefGenMedProductDetails != null)
            if (e.Column.Equals(grdEstimateDetails.GetColumnByName("colAdjustedQty")) && item.RefGenMedProductDetails != null)
            {
                item.QtyPackaging = CalQtyPackaging(item.AdjustedQty, item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));
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
            return _adjustedQty / (_unitPackaging > 0 ? _unitPackaging : 1);
        }

        public void txtQtyPackaging_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentDrugDeptEstimationForPoDetail == null || CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails == null)
            {
                return;
            }

            //KMx: Nếu text có dấu ",". VD: 150,000. thì phải bỏ dấu "," đi, nếu không sẽ không TryParse được (19/11/2014 11:45).
            string txt = "";
            txt = (sender as TextBox).Text.Replace(",", "");

            if (!string.IsNullOrEmpty(txt))
            {
                double qtyPackaging = 0;
                double.TryParse(txt, out qtyPackaging);

                CurrentDrugDeptEstimationForPoDetail.QtyPackaging = qtyPackaging;
                CurrentDrugDeptEstimationForPoDetail.AdjustedQty = CalAdjustedQty(qtyPackaging, CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));
                CurrentDrugDeptEstimationForPoDetail.TotalPrice = CurrentDrugDeptEstimationForPoDetail.AdjustedQty * CurrentDrugDeptEstimationForPoDetail.UnitPrice;
            }
        }

        public void txtAdjustedQty_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentDrugDeptEstimationForPoDetail == null || CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails == null)
            {
                return;
            }

            //KMx: Nếu text có dấu ",". VD: 150,000. thì phải bỏ dấu "," đi, nếu không sẽ không TryParse được (19/11/2014 11:45).
            string txt = "";
            txt = (sender as TextBox).Text.Replace(",", "");

            if (!string.IsNullOrEmpty(txt))
            {
                int adjustedQty = 0;
                int.TryParse(txt, out adjustedQty);

                CurrentDrugDeptEstimationForPoDetail.AdjustedQty = adjustedQty;
                CurrentDrugDeptEstimationForPoDetail.QtyPackaging = CalQtyPackaging(adjustedQty, CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1));
                CurrentDrugDeptEstimationForPoDetail.TotalPrice = CurrentDrugDeptEstimationForPoDetail.AdjustedQty * CurrentDrugDeptEstimationForPoDetail.UnitPrice;
            }
        }

        private void DrugDeptEstimationForPODelete(long DrugDeptEstimatePoID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPO_Delete(DrugDeptEstimatePoID, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDrugDeptEstimationForPO_Delete(asyncResult);
                                InittializeEstimationForPO();
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

        public void btnDeletePhieu()
        {
            if (MessageBox.Show(string.Format(eHCMSResources.Z0557_G1_CoChacMuonXoa, eHCMSResources.P0370_G1_PhDuTru), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CurrentDrugDeptEstimationForPO != null)
                {
                    DrugDeptEstimationForPODelete(CurrentDrugDeptEstimationForPO.DrugDeptEstimatePoID);
                }
            }
        }

        #region IHandle<DrugDeptCloseSearchEstimationEvent> Members
        public void Handle(DrugDeptCloseSearchEstimationEvent message)
        {
            if (IsActive && message != null)
            {
                DrugDeptEstimationForPO temp = message.SelectedEstimation as DrugDeptEstimationForPO;
                if (temp != null)
                {
                    ChangeValue(CurrentDrugDeptEstimationForPO.RefGenDrugCatID_1, temp.RefGenDrugCatID_1);
                    CurrentDrugDeptEstimationForPO = temp;
                    HideShowColumnDelete();
                    SetRadioButtonCompleted();
                    DrugDeptEstimationForPoDetail_ByParentID(CurrentDrugDeptEstimationForPO.DrugDeptEstimatePoID);
                }
            }
        }
        #endregion

        #region printing member
        private long _estimationCodeBegin;
        public long EstimationCodeBegin
        {
            get
            {
                return _estimationCodeBegin;
            }
            set
            {
                _estimationCodeBegin = value;
                NotifyOfPropertyChange(() => EstimationCodeBegin);
            }
        }

        private long _estimationCodeEnd;
        public long EstimationCodeEnd
        {
            get
            {
                return _estimationCodeEnd;
            }
            set
            {
                _estimationCodeEnd = value;
                NotifyOfPropertyChange(() => EstimationCodeEnd);
            }
        }

        public void btnPreview()
        {
            void onInitDlg(IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentDrugDeptEstimationForPO.DrugDeptEstimatePoID;

                proAlloc.V_MedProductType = V_MedProductType;
                //▼======= #002
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z0558_G1_BangDuTruThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z0559_G1_BangDuTruYCu.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = eHCMSResources.Z3226_G1_BangDuTruDDuong.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                {
                    proAlloc.LyDo = eHCMSResources.Z0560_G1_BangDuTruHChat.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                {
                    proAlloc.LyDo = eHCMSResources.Z2701_G1_BangDuTruVTYTTH.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
                {
                    proAlloc.LyDo = eHCMSResources.Z2702_G1_BangDuTruVaccine.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
                {
                    proAlloc.LyDo = eHCMSResources.Z2703_G1_BangDuTruBlood.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                {
                    proAlloc.LyDo = eHCMSResources.Z2704_G1_BangDuTruVPP.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
                {
                    proAlloc.LyDo = eHCMSResources.Z2705_G1_BangDuTruVTTH.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z2706_G1_BangDuTruThanhTrung.ToUpper();
                }
                //▲======== #002
                if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
                {
                    proAlloc.LyDo += string.Format(" {0} ", eHCMSResources.G0039_G1_Th.ToUpper()) + CurrentDrugDeptEstimationForPO.DateOfEstimation.Month.ToString() + "/" + CurrentDrugDeptEstimationForPO.DateOfEstimation.Year.ToString();
                }
                else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH || CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
                {
                    proAlloc.LyDo += string.Format(" {0} ", eHCMSResources.Z0561_G1_BoSungTh.ToUpper()) + CurrentDrugDeptEstimationForPO.DateOfEstimation.Month.ToString() + "/" + CurrentDrugDeptEstimationForPO.DateOfEstimation.Year.ToString();
                }
                else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
                {
                    proAlloc.LyDo += string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + CurrentDrugDeptEstimationForPO.DateOfEstimation.Year.ToString();
                }
                else
                {
                    proAlloc.LyDo += string.Format(" {0} ", eHCMSResources.Z0562_G1_BoSungNam.ToUpper()) + CurrentDrugDeptEstimationForPO.DateOfEstimation.Year.ToString();
                }
                if (LoaiIn == 1)
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_ESTIMATION;
                }
                else if (LoaiIn == 2)
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_ESTIMATIONKETOAN;
                }
                else
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_ESTIMATIONTHUKHO;
                }
                proAlloc.StaffFullName = GetStaffLogin().FullName;
            }
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnMergePreview()
        {
            void onInitDlg(IDrugDeptReportDocumentPreview proAlloc)
            {
                //▼======= #002
                proAlloc.ID = CurrentDrugDeptEstimationForPO.DrugDeptEstimatePoID;

                proAlloc.V_MedProductType = V_MedProductType;

                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z0558_G1_BangDuTruThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z0559_G1_BangDuTruYCu.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = eHCMSResources.Z3226_G1_BangDuTruDDuong.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                {
                    proAlloc.LyDo = eHCMSResources.Z0560_G1_BangDuTruHChat.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                {
                    proAlloc.LyDo = eHCMSResources.Z2701_G1_BangDuTruVTYTTH.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
                {
                    proAlloc.LyDo = eHCMSResources.Z2702_G1_BangDuTruVaccine.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
                {
                    proAlloc.LyDo = eHCMSResources.Z2703_G1_BangDuTruBlood.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                {
                    proAlloc.LyDo = eHCMSResources.Z2704_G1_BangDuTruVPP.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
                {
                    proAlloc.LyDo = eHCMSResources.Z2705_G1_BangDuTruVTTH.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z2706_G1_BangDuTruThanhTrung.ToUpper();
                }
                //▲======== #002

                if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTMONTH)
                {
                    proAlloc.LyDo += string.Format(" {0} ", eHCMSResources.G0039_G1_Th.ToUpper()) + CurrentDrugDeptEstimationForPO.DateOfEstimation.Month.ToString() + "/" + CurrentDrugDeptEstimationForPO.DateOfEstimation.Year.ToString();
                }
                else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.ADDITION_FIRSTMONTH || CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.MODIFYMONTH)
                {
                    proAlloc.LyDo += string.Format(" {0} ", eHCMSResources.Z0561_G1_BoSungTh.ToUpper()) + CurrentDrugDeptEstimationForPO.DateOfEstimation.Month.ToString() + "/" + CurrentDrugDeptEstimationForPO.DateOfEstimation.Year.ToString();
                }
                else if (CurrentDrugDeptEstimationForPO.V_EstimateType == (long)AllLookupValues.V_EstimateType.FIRSTYEAR)
                {
                    proAlloc.LyDo += string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + CurrentDrugDeptEstimationForPO.DateOfEstimation.Year.ToString();
                }
                else
                {
                    proAlloc.LyDo += string.Format(" {0} ", eHCMSResources.Z0562_G1_BoSungNam.ToUpper()) + CurrentDrugDeptEstimationForPO.DateOfEstimation.Year.ToString();
                }

                proAlloc.EstimationCodeBegin = EstimationCodeBegin;
                proAlloc.EstimationCodeEnd = EstimationCodeEnd;
                proAlloc.eItem = ReportName.DRUGDEPT_ESTIMATION;
            }
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }
        public void btnPrint()
        {
            MessageBox.Show(eHCMSResources.Z0563_G1_ChuaLam);
        }
        #endregion

        private void ChangeValue(long value1, long value2)
        {
            if (value1 != value2)
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
        }

        private bool flag = true;
        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag)
            {
                CurrentDrugDeptEstimationForPoDetail = new DrugDeptEstimationForPoDetail();
                if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.EstimationDetails != null)
                {
                    CurrentDrugDeptEstimationForPO.EstimationDetails.Clear();
                }
                LoadDataGrid();
            }
            flag = true;
        }

        private int LoaiIn = 1;
        public void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            LoaiIn = 1;
        }

        public void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            LoaiIn = 2;
        }

        public void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            LoaiIn = 3;
        }

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
            if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.EstimationDetails != null)
            {
                CVS_PCVEstimationDetails = new CollectionViewSource { Source = CurrentDrugDeptEstimationForPO.EstimationDetails };
                CV_PCVEstimationDetails = (CollectionView)CVS_PCVEstimationDetails.View;
                NotifyOfPropertyChange(() => CV_PCVEstimationDetails);
            }

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
            DrugDeptEstimationForPoDetail emp = o as DrugDeptEstimationForPoDetail;
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

        #region Add Item Member
        private void DrugDeptEstimationForPoDetail_GenMedProductID(long GenMedProductID, string Code)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPoDetail_GenMedProductID(GenMedProductID, Code, CurrentDrugDeptEstimationForPO.DateOfEstimation, CurrentDrugDeptEstimationForPO.V_EstimateType, V_MedProductType, CurrentDrugDeptEstimationForPO.RefGenDrugCatID_1, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentDrugDeptEstimationForPoDetail = contract.EndDrugDeptEstimationForPoDetail_GenMedProductID(asyncResult);
                                if (CurrentDrugDeptEstimationForPoDetail != null)
                                {
                                    CurrentDrugDeptEstimationForPoDetail.EntityState = Service.Core.Common.EntityState.NEW;
                                    if (GenMedProductID == 0)
                                    {
                                        CurrentRefGenericDrugDetail = CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails;
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
                                if (AxHSN != null)
                                {
                                    AxHSN.Focus();
                                }
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

        private string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);

                    //DrugDeptEstimationForPoDetail_GenMedProductID(0, Code);
                    DrugDeptEstimationForPoDetail_GenMedProductID_V2(0, Code);
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

        public void Code_Checked(object sender, RoutedEventArgs e)
        {
            VisibilityName = false;
        }

        public void Name_Checked(object sender, RoutedEventArgs e)
        {
            VisibilityName = true;
        }

        AxTextBox AxHSN = null;
        public void HeSoNhan_Loaded(object sender, RoutedEventArgs e)
        {
            AxHSN = sender as AxTextBox;
        }

        private string txtHSN = "";
        public void AxHSN_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtHSN != (sender as AxTextBox).Text)
            {
                txtHSN = (sender as AxTextBox).Text;
                if (!string.IsNullOrEmpty(txtHSN))
                {
                    try
                    {
                        CurrentDrugDeptEstimationForPoDetail.NumberOfEstimatedMonths_F = Convert.ToInt32(txtHSN);
                    }
                    catch
                    {

                    }
                    CountEstimate(CurrentDrugDeptEstimationForPoDetail, false);
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
            if (CurrentDrugDeptEstimationForPoDetail != null && CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails != null)
            {
                if (e.Parameter == CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails.BrandName)
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
            if (CurrentDrugDeptEstimationForPoDetail != null && CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails != null && CurrentRefGenericDrugDetail != null)
            {
                if (CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails.GenMedProductID == CurrentRefGenericDrugDetail.GenMedProductID)
                {
                    return;
                }
            }
            if (auto.SelectedItem != null)
            {
                if (CurrentDrugDeptEstimationForPoDetail == null)
                {
                    CurrentDrugDeptEstimationForPoDetail = new DrugDeptEstimationForPoDetail();
                }
                CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails = CurrentRefGenericDrugDetail;//C auto.SelectedItem as RefGenericDrugDetail;
                //DrugDeptEstimationForPoDetail_GenMedProductID(CurrentRefGenericDrugDetail.GenMedProductID, "");
                DrugDeptEstimationForPoDetail_GenMedProductID_V2(CurrentRefGenericDrugDetail.GenMedProductID, "");
            }
        }

        public void AddItem()
        {
            if (CurrentDrugDeptEstimationForPoDetail == null || CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails == null)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0410_G1_ChonThuoc), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentDrugDeptEstimationForPoDetail.AdjustedQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0564_G1_SLgDuTruPhaiLonHon0, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentDrugDeptEstimationForPO == null)
            {
                CurrentDrugDeptEstimationForPO = new DrugDeptEstimationForPO();
            }
            if (CurrentDrugDeptEstimationForPO.EstimationDetails == null)
            {
                CurrentDrugDeptEstimationForPO.EstimationDetails = new ObservableCollection<DrugDeptEstimationForPoDetail>();
            }

            // TxD 08/02/2015: Requested by Ds. Thu Van to take the following check out and allow for duplication of Item.
            //                  If this removed checking cause any issue anywhere else then it needs to be reviewed
            var chk = CurrentDrugDeptEstimationForPO.EstimationDetails.Where(x => x.RefGenMedProductDetails != null && x.RefGenMedProductDetails.GenMedProductID == CurrentDrugDeptEstimationForPoDetail.GenMedProductID
                                                                                                                    && x.RefGenMedProductDetails.SupplierMain.SupplierID == CurrentDrugDeptEstimationForPoDetail.SupplierID);
            if (chk != null && chk.Count() > 0)
            {
                Globals.ShowMessage("Thuốc này đã tồn tại!", eHCMSResources.G0442_G1_TBao);
                return;
            }

            //GetPackageQty(CurrentDrugDeptEstimationForPoDetail);

            CurrentDrugDeptEstimationForPO.EstimationDetails.Insert(0, CurrentDrugDeptEstimationForPoDetail);
            LoadDataGrid();
            CurrentDrugDeptEstimationForPoDetail = new DrugDeptEstimationForPoDetail();
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
        #endregion

        #region AutoGenMedProduct Member
        private RefGenMedProductDetails _CurrentRefGenericDrugDetail;
        public RefGenMedProductDetails CurrentRefGenericDrugDetail
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

        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenericDrugDetail;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenericDrugDetail
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
            long? RefGenDrugCatID_1 = null;
            if (CurrentDrugDeptEstimationForPO != null)
            {
                RefGenDrugCatID_1 = CurrentDrugDeptEstimationForPO.RefGenDrugCatID_1;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SearchAutoPaging(false, BrandName, null, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total;
                            var results = contract.EndRefGenMedProductDetails_SearchAutoPaging(out Total, asyncResult);
                            RefGenericDrugDetail.Clear();
                            RefGenericDrugDetail.TotalItemCount = Total;
                            foreach (RefGenMedProductDetails p in results)
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

        private void AllCheckedfc()
        {
            if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.EstimationDetails != null && CurrentDrugDeptEstimationForPO.EstimationDetails.Count > 0)
            {
                for (int i = 0; i < CurrentDrugDeptEstimationForPO.EstimationDetails.Count; i++)
                {
                    CurrentDrugDeptEstimationForPO.EstimationDetails[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.EstimationDetails != null && CurrentDrugDeptEstimationForPO.EstimationDetails.Count > 0)
            {
                for (int i = 0; i < CurrentDrugDeptEstimationForPO.EstimationDetails.Count; i++)
                {
                    CurrentDrugDeptEstimationForPO.EstimationDetails[i].Checked = false;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (CurrentDrugDeptEstimationForPO != null && CurrentDrugDeptEstimationForPO.EstimationDetails != null && CurrentDrugDeptEstimationForPO.EstimationDetails.Count > 0)
            {
                var items = CurrentDrugDeptEstimationForPO.EstimationDetails.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (DrugDeptEstimationForPoDetail p in items)
                        {
                            if (p.EntityState != Service.Core.Common.EntityState.NEW)
                            {
                                p.EntityState = Service.Core.Common.EntityState.DETACHED;
                                EstimationDetailsDeleted.Add(p);
                            }
                        }
                        CurrentDrugDeptEstimationForPO.EstimationDetails = CurrentDrugDeptEstimationForPO.EstimationDetails.Where(x => x.Checked == false).ToObservableCollection();
                        LoadDataGrid();
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
                }
            }
        }

        AxTextBox txtNumberOfEstimatedMonths = null;
        public void txtNumberOfEstimatedMonths_Loaded(object sender, RoutedEventArgs e)
        {
            txtNumberOfEstimatedMonths = sender as AxTextBox;
        }

        #region Methods
        //private void LoadValidBidFromSupplierID(long SupplierID)
        //{
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
        //        {
        //            var mContract = mServiceFactory.ServiceInstance;
        //            try
        //            {
        //                mContract.BeginGetInUsingBidCollectionFromSupplierID(SupplierID, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        var mReturnValues = mContract.EndGetInUsingBidCollectionFromSupplierID(asyncResult);
        //                        if (mReturnValues == null || mReturnValues.Count == 0)
        //                        {
        //                            BidCollection = new ObservableCollection<Bid>();
        //                        }
        //                        else
        //                        {
        //                            BidCollection = mReturnValues.ToObservableCollection();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                this.HideBusyIndicator();
        //            }
        //        }
        //    });
        //    t.Start();
        //}
        #endregion

        private void GetEstimationDrugDept_V2()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetEstimation_V2(V_MedProductType, FromDate, ToDate, CurrentDrugDeptEstimationForPO, false, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetEstimation_V2(asyncResult);
                                CurrentDrugDeptEstimationForPO.EstimationDetails = results.ToObservableCollection();
                                if (CurrentDrugDeptEstimationForPO.EstimationDetails != null)
                                {
                                    for (int i = 0; i < CurrentDrugDeptEstimationForPO.EstimationDetails.Count; i++)
                                    {
                                        CurrentDrugDeptEstimationForPO.EstimationDetails[i].EntityState = Service.Core.Common.EntityState.NEW;
                                    }
                                }
                                LoadDataGrid();
                                IsEnableCanOK = false;
                                IsEnableDeletePhieu = false;
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

        private DateTime _FromDate;
        public DateTime FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime _ToDate;
        public DateTime ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                if(CurrentDrugDeptEstimationForPO != null)
                {
                    CurrentDrugDeptEstimationForPO.DateOfEstimation = _ToDate;

                }
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        private bool CheckDatetime()
        {
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0366_G1_ChonNgThCanXem);
                return false;
            }
            if (FromDate > ToDate)
            {
                MessageBox.Show(eHCMSResources.A0857_G1_Msg_InfoNgThangKhHopLe2);
                return false;
            }
            return true;
        }

        private void DrugDeptEstimationForPoDetail_GenMedProductID_V2(long GenMedProductID, string Code)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPoDetail_GenMedProductID_V2(GenMedProductID, Code, FromDate, ToDate, V_MedProductType, CurrentDrugDeptEstimationForPO.RefGenDrugCatID_1, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentDrugDeptEstimationForPoDetail = contract.EndDrugDeptEstimationForPoDetail_GenMedProductID_V2(asyncResult);
                                if (CurrentDrugDeptEstimationForPoDetail != null)
                                {
                                    CurrentDrugDeptEstimationForPoDetail.EntityState = Service.Core.Common.EntityState.NEW;
                                    if (GenMedProductID == 0)
                                    {
                                        CurrentRefGenericDrugDetail = CurrentDrugDeptEstimationForPoDetail.RefGenMedProductDetails;
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
                                if (AxHSN != null)
                                {
                                    AxHSN.Focus();
                                }
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

        private bool _IsEnableCanOK = true;
        public bool IsEnableCanOK
        {
            get
            {
                return _IsEnableCanOK;
            }
            set
            {
                _IsEnableCanOK = value;
                NotifyOfPropertyChange(() => IsEnableCanOK);
            }
        }

        private bool _IsEnableDeletePhieu = false;
        public bool IsEnableDeletePhieu
        {
            get
            {
                return _IsEnableDeletePhieu;
            }
            set
            {
                _IsEnableDeletePhieu = value;
                NotifyOfPropertyChange(() => IsEnableDeletePhieu);
            }
        }

        private bool _IsAll;
        public bool IsAll
        {
            get
            {
                return _IsAll;
            }
            set
            {
                _IsAll = value;
                if (_IsAll)
                {
                    IsForeign = !_IsAll;
                    IsTrongNuoc = !_IsAll;
                }
                NotifyOfPropertyChange(() => IsAll);
            }
        }
    }
}
