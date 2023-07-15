using eHCMSLanguage;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using aEMR.CommonTasks;
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
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Common;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using aEMR.Controls;
/*
 * 20200131 #001 TBL: BM 0019642: Fix lỗi số lượng trả lớn hơn số lượng xuất
 */

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptReturn)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReturnDrugViewModel : Conductor<object>, IDrugDeptReturn
        , IHandle<DrugDeptCloseSearchOutMedDeptInvoiceEvent>
        , IHandle<PharmacyPayEvent>
    {

        #region Indicator Member

        private bool _isLoadingGetStore = false;
        public bool isLoadingGetStore
        {
            get { return _isLoadingGetStore; }
            set
            {
                if (_isLoadingGetStore != value)
                {
                    _isLoadingGetStore = value;
                    NotifyOfPropertyChange(() => isLoadingGetStore);
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

        private bool _isLoadingInvoiceDetailD = false;
        public bool isLoadingInvoiceDetailD
        {
            get { return _isLoadingInvoiceDetailD; }
            set
            {
                if (_isLoadingInvoiceDetailD != value)
                {
                    _isLoadingInvoiceDetailD = value;
                    NotifyOfPropertyChange(() => isLoadingInvoiceDetailD);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingInvoiceID = false;
        public bool isLoadingInvoiceID
        {
            get { return _isLoadingInvoiceID; }
            set
            {
                if (_isLoadingInvoiceID != value)
                {
                    _isLoadingInvoiceID = value;
                    NotifyOfPropertyChange(() => isLoadingInvoiceID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingReturnID = false;
        public bool isLoadingReturnID
        {
            get { return _isLoadingReturnID; }
            set
            {
                if (_isLoadingReturnID != value)
                {
                    _isLoadingReturnID = value;
                    NotifyOfPropertyChange(() => isLoadingReturnID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingOutstatus = false;
        public bool isLoadingOutstatus
        {
            get { return _isLoadingOutstatus; }
            set
            {
                if (_isLoadingOutstatus != value)
                {
                    _isLoadingOutstatus = value;
                    NotifyOfPropertyChange(() => isLoadingOutstatus);
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

        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingInvoiceDetailD || isLoadingReturnID || isLoadingOutstatus || isLoadingSearch); }
        }

        #endregion
        public enum DataGridCol
        {
            QTYRETUNRED = 6,
            RETURN = 7,
            TotalPriceReturn = 9,
            TotalPrice = 10,
            HIPayReturn = 11,
            HIPay = 12,
            PatientReturn = 13,
            PatientPay = 14
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ReturnDrugViewModel(IWindsorContainer container, INavigationService service, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = service;
            _salePosCaching = salePosCaching;
            _logger = container.Resolve<ILogger>();
            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);
            SearchCriteria = new MedDeptInvoiceSearchCriteria();
            // SearchCriteria.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED;
            SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO;

            SearchCriteriaReturn = new MedDeptInvoiceSearchCriteria();
            SearchCriteriaReturn.TypID = (long)AllLookupValues.RefOutputType.HOANTRATHUOC;

            SelectedOutwardInfo = new OutwardDrugMedDeptInvoice();
            Coroutine.BeginExecute(DoLoadOutStatus());

            StaffName = GetStaffLogin().FullName;
            SetDefaultForStore();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Coroutine.BeginExecute(DoGetStore_DrugDept());
        }

        #region Properties Member

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
                }

            }
        }

        private bool _IsChildWindow = false;
        public bool IsChildWindow
        {
            get
            {
                return _IsChildWindow;
            }
            set
            {
                if (_IsChildWindow != value)
                {
                    _IsChildWindow = value;
                    NotifyOfPropertyChange(() => IsChildWindow);
                }
            }
        }


        private ObservableCollection<Lookup> _OutStatus;
        public ObservableCollection<Lookup> OutStatus
        {
            get
            {
                return _OutStatus;
            }
            set
            {
                if (_OutStatus != value)
                {
                    _OutStatus = value;
                    NotifyOfPropertyChange(() => OutStatus);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        private OutwardDrugMedDeptInvoice _SelectedOutwardInfo;
        public OutwardDrugMedDeptInvoice SelectedOutwardInfo
        {
            get
            {
                return _SelectedOutwardInfo;
            }
            set
            {
                if (_SelectedOutwardInfo != value)
                {
                    _SelectedOutwardInfo = value;
                    NotifyOfPropertyChange(() => SelectedOutwardInfo);

                    NotifyOfPropertyChange(() => ShowInvoiceInfo);
                    NotifyOfPropertyChange(() => ShowReturnInvoiceInfo);
                }
            }
        }

        public bool ShowInvoiceInfo
        {
            get
            {
                return SelectedOutwardInfo == null || !SelectedOutwardInfo.ReturnID.HasValue;
            }
        }
        public bool ShowReturnInvoiceInfo
        {
            get
            {
                return !ShowInvoiceInfo;
            }
        }

        private MedDeptInvoiceSearchCriteria _searchCriteria;
        public MedDeptInvoiceSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private MedDeptInvoiceSearchCriteria _searchCriteriaReturn;
        public MedDeptInvoiceSearchCriteria SearchCriteriaReturn
        {
            get { return _searchCriteriaReturn; }
            set
            {
                if (_searchCriteriaReturn != value)
                    _searchCriteriaReturn = value;
                NotifyOfPropertyChange(() => SearchCriteriaReturn);
            }
        }

        private int _QtyReturn;
        public int QtyReturn
        {
            get { return _QtyReturn; }
            set
            {
                if (_QtyReturn != value)
                    _QtyReturn = value;
                NotifyOfPropertyChange(() => QtyReturn);
            }
        }


        private decimal _SumTotalPriceReturn;
        public decimal SumTotalPriceReturn
        {
            get { return _SumTotalPriceReturn; }
            set
            {
                if (_SumTotalPriceReturn != value)
                    _SumTotalPriceReturn = value;
                NotifyOfPropertyChange(() => SumTotalPriceReturn);
            }
        }

        private decimal _SumTotalPriceHIReturn;
        public decimal SumTotalPriceHIReturn
        {
            get { return _SumTotalPriceHIReturn; }
            set
            {
                if (_SumTotalPriceHIReturn != value)
                    _SumTotalPriceHIReturn = value;
                NotifyOfPropertyChange(() => SumTotalPriceHIReturn);
            }
        }

        private decimal _SumTotalPricePatientReturn;
        public decimal SumTotalPricePatientReturn
        {
            get { return _SumTotalPricePatientReturn; }
            set
            {
                if (_SumTotalPricePatientReturn != value)
                    _SumTotalPricePatientReturn = value;
                NotifyOfPropertyChange(() => SumTotalPricePatientReturn);
            }
        }

        private long _StoreID;
        public long StoreID
        {
            get { return _StoreID; }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }
        #endregion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mTim = true;
        private bool _mLuu = true;
        private bool _mTraTien = true;
        private bool _mIn = true;

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
        public bool mLuu
        {
            get
            {
                return _mLuu;
            }
            set
            {
                if (_mLuu == value)
                    return;
                _mLuu = value;
                NotifyOfPropertyChange(() => mLuu);
            }
        }
        public bool mTraTien
        {
            get
            {
                return _mTraTien;
            }
            set
            {
                if (_mTraTien == value)
                    return;
                _mTraTien = value;
                NotifyOfPropertyChange(() => mTraTien);
            }
        }
        public bool mIn
        {
            get
            {
                return _mIn;
            }
            set
            {
                if (_mIn == value)
                    return;
                _mIn = value;
                NotifyOfPropertyChange(() => mIn);
            }
        }


        private bool _mTraHang_Tim = true;
        private bool _mTraHang_TimPhieuTraHangCu = true;
        private bool _mTraHang_ThongTin = true;
        private bool _mTraHang_Luu = true;
        private bool _mTraHang_TraTien = true;
        private bool _mTraHang_ReportIn = true;

        public bool mTraHang_Tim
        {
            get
            {
                return _mTraHang_Tim;
            }
            set
            {
                if (_mTraHang_Tim == value)
                    return;
                _mTraHang_Tim = value;
            }
        }
        public bool mTraHang_TimPhieuTraHangCu
        {
            get
            {
                return _mTraHang_TimPhieuTraHangCu;
            }
            set
            {
                if (_mTraHang_TimPhieuTraHangCu == value)
                    return;
                _mTraHang_TimPhieuTraHangCu = value;
            }
        }
        public bool mTraHang_ThongTin
        {
            get
            {
                return _mTraHang_ThongTin;
            }
            set
            {
                if (_mTraHang_ThongTin == value)
                    return;
                _mTraHang_ThongTin = value;
            }
        }
        public bool mTraHang_Luu
        {
            get
            {
                return _mTraHang_Luu;
            }
            set
            {
                if (_mTraHang_Luu == value)
                    return;
                _mTraHang_Luu = value;
            }
        }
        public bool mTraHang_TraTien
        {
            get
            {
                return _mTraHang_TraTien;
            }
            set
            {
                if (_mTraHang_TraTien == value)
                    return;
                _mTraHang_TraTien = value;
            }
        }
        public bool mTraHang_ReportIn
        {
            get
            {
                return _mTraHang_ReportIn;
            }
            set
            {
                if (_mTraHang_ReportIn == value)
                    return;
                _mTraHang_ReportIn = value;
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

        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            SetDefaultForStore();
            isLoadingGetStore = false;
            yield break;
        }

        private IEnumerator<IResult> DoLoadOutStatus()
        {
            isLoadingOutstatus = true;
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_OutDrugInvStatus, false, true);
            yield return paymentTypeTask;
            OutStatus = paymentTypeTask.LookupList;
            SetDefaultForStore();
            isLoadingOutstatus = false;
            yield break;
        }

        private const string ALLITEMS = "[All]";

        private void SetDefaultForStore()
        {
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
                SearchCriteria.StoreID = StoreCbx.FirstOrDefault().StoreID;
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        public void GridInward_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public void GridInward_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid GridInward = sender as DataGrid;
            if (GridInward.Columns[(int)DataGridCol.RETURN].Visibility == Visibility.Visible)
            {
                if (GridInward.SelectedItem != null)   //ensure we have current item
                {
                    //call begin edit
                    int idx = (int)DataGridCol.RETURN;
                    GridInward.CurrentColumn = GridInward.Columns[idx];
                    GridInward.BeginEdit();
                    TextBox obj = GridInward.Columns[idx].GetCellContent(GridInward.SelectedItem) as TextBox;
                    if (obj != null)
                    {
                        obj.Focus();
                    }
                }
            }
        }

        public void GridInward_KeyUp(object sender, KeyEventArgs e)
        {
            DataGrid GridInward = sender as DataGrid;
            if (GridInward.Columns[(int)DataGridCol.RETURN].Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Enter || e.Key == Key.Tab || e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
                {
                    if (GridInward.SelectedItem != null)   //ensure we have current item
                    {
                        //call begin edit
                        GridInward.BeginEdit();
                        int idx = (int)DataGridCol.RETURN;
                        TextBox obj = GridInward.Columns[idx].GetCellContent(GridInward.SelectedItem) as TextBox;
                        if (obj != null)
                        {
                            obj.Focus();
                        }
                    }
                }
            }
        }

        DataGrid GridInward;
        public void GridInward_Loaded(object sender, RoutedEventArgs e)
        {
            GridInward = sender as DataGrid;
        }

        private void GetCurrentInvoiceInfo(OutwardDrugMedDeptInvoice Current)
        {
            SelectedOutwardInfo = Current;
            SelectedOutwardInfo.Notes = "";
            //if (SelectedOutwardInfo.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)
            //{
            //    Globals.ShowMessage("Bệnh nhân này chưa nhận thuốc nên không thể thực hiện việc trả thuốc được", "Lưu ý");
            //}
            if (SelectedOutwardInfo.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                Globals.ShowMessage(eHCMSResources.Z1298_G1_PhXuatDaHuy, eHCMSResources.G0442_G1_TBao);
            }
            HideColumnDataGrid(true);
            GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID);
        }

        private void GetCurrentInvoiceReturnInfo(OutwardDrugMedDeptInvoice Current)
        {
            SelectedOutwardInfo = Current;
            if (SelectedOutwardInfo.ReturnID.GetValueOrDefault(0) > 0)
            {
                GetOutwardDrugMedDeptInvoiceByID(SelectedOutwardInfo.outiID);
            }
            HideColumnDataGrid(false);
            GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID);
        }

        private void GetOutwardDrugDetailsByOutwardInvoice(long OutiID,bool showPaymentWindow = false)
        {
            isLoadingInvoiceDetailD = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutwardDrugMedDeptDetailByInvoice(OutiID, V_MedProductType, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardDrugMedDeptDetailByInvoice(asyncResult);
                            SelectedOutwardInfo.OutwardDrugMedDepts = results.ToObservableCollection();
                            CountMoney();
                            var bShowPayment = (bool)asyncResult.AsyncState;
                            if (bShowPayment)
                            {
                                PayCmd();
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingInvoiceDetailD = false;
                            //Globals.IsBusy = false;
                        }

                    }), showPaymentWindow);

                }

            });

            t.Start();
        }

        private void CountMoney()
        {
            SumTotalPriceReturn = 0;
            SumTotalPriceHIReturn = 0;
            SumTotalPricePatientReturn = 0;
            QtyReturn = 0;
            if (SelectedOutwardInfo != null)
            {
                SelectedOutwardInfo.TotalInvoicePrice = 0;
                SelectedOutwardInfo.TotalCoPayment = 0;
                SelectedOutwardInfo.TotalHIPayment = 0;
                SelectedOutwardInfo.TotalPriceDifference = 0;
                SelectedOutwardInfo.TotalPatientPayment = 0;

                if (SelectedOutwardInfo.OutwardDrugMedDepts != null)
                {
                    if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.HOANTRATHUOC)
                    {
                        foreach (OutwardDrugMedDept p in SelectedOutwardInfo.OutwardDrugMedDepts)
                        {
                            SumTotalPriceReturn = SumTotalPriceReturn + (p.OutPrice * p.OutQuantity);
                            SumTotalPriceHIReturn = SumTotalPriceHIReturn + p.TotalHIPayment;

                            SelectedOutwardInfo.TotalPriceDifference += p.PriceDifference * p.OutQuantity;
                        }
                    }
                    else
                    {
                        foreach (OutwardDrugMedDept p in SelectedOutwardInfo.OutwardDrugMedDepts)
                        {
                            QtyReturn = QtyReturn + p.OutQuantityReturn;
                            if (p.TotalCoPayment > 0)
                            {
                                p.OutHIRebateReturn = p.HIAllowedPrice.GetValueOrDefault() * (decimal)p.HIBenefit.GetValueOrDefault() * p.OutQuantityReturn;
                            }
                            else
                            {
                                p.OutHIRebateReturn = p.HIAllowedPrice.GetValueOrDefault() * p.OutQuantityReturn;
                            }
                            p.TotalHIPayment = p.OutHIRebateReturn.GetValueOrDefault(0);
                            if (p.TotalHIPayment > 0)
                            {
                                p.TotalCoPayment = (p.OutQuantityReturn * p.OutPrice) - p.TotalHIPayment - (p.PriceDifference * p.OutQuantityReturn);
                            }
                            SelectedOutwardInfo.TotalPriceDifference += p.PriceDifference * p.OutQuantityReturn;

                            p.PatientReturn = p.TotalPriceReturn - p.OutHIRebateReturn;

                            SumTotalPriceReturn = SumTotalPriceReturn + p.TotalPriceReturn;
                            SumTotalPriceHIReturn = SumTotalPriceHIReturn + p.OutHIRebateReturn.GetValueOrDefault();

                        }

                    }
                    SumTotalPricePatientReturn = SumTotalPriceReturn - SumTotalPriceHIReturn;
                    SelectedOutwardInfo.TotalInvoicePrice = SumTotalPriceReturn;
                    SelectedOutwardInfo.TotalHIPayment = SumTotalPriceHIReturn;
                    if (SelectedOutwardInfo.TotalHIPayment > 0)
                    {
                        SelectedOutwardInfo.TotalCoPayment = SumTotalPriceReturn - SumTotalPriceHIReturn - SelectedOutwardInfo.TotalPriceDifference;
                    }
                    SelectedOutwardInfo.TotalPatientPayment = SumTotalPricePatientReturn;

                }
            }
        }

        private void HideColumnDataGrid(bool value)
        {
            if (GridInward != null)
            {
                if (!value)
                {
                    GridInward.Columns[(int)DataGridCol.QTYRETUNRED].Visibility = Visibility.Collapsed;
                    GridInward.Columns[(int)DataGridCol.RETURN].Visibility = Visibility.Collapsed;
                    GridInward.Columns[(int)DataGridCol.TotalPriceReturn].Visibility = Visibility.Collapsed;
                    GridInward.Columns[(int)DataGridCol.TotalPrice].Visibility = Visibility.Visible;
                    GridInward.Columns[(int)DataGridCol.HIPayReturn].Visibility = Visibility.Collapsed;
                    GridInward.Columns[(int)DataGridCol.HIPay].Visibility = Visibility.Visible;
                    GridInward.Columns[(int)DataGridCol.PatientReturn].Visibility = Visibility.Collapsed;
                    GridInward.Columns[(int)DataGridCol.PatientPay].Visibility = Visibility.Visible;
                }
                else
                {
                    GridInward.Columns[(int)DataGridCol.QTYRETUNRED].Visibility = Visibility.Visible;
                    GridInward.Columns[(int)DataGridCol.RETURN].Visibility = Visibility.Visible;
                    GridInward.Columns[(int)DataGridCol.TotalPriceReturn].Visibility = Visibility.Visible;
                    GridInward.Columns[(int)DataGridCol.TotalPrice].Visibility = Visibility.Collapsed;
                    GridInward.Columns[(int)DataGridCol.HIPayReturn].Visibility = Visibility.Visible;
                    GridInward.Columns[(int)DataGridCol.HIPay].Visibility = Visibility.Collapsed;
                    GridInward.Columns[(int)DataGridCol.PatientReturn].Visibility = Visibility.Visible;
                    GridInward.Columns[(int)DataGridCol.PatientPay].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void GetOutwardDrugMedDeptInvoiceByID(long OutiID, bool showPaymentWindow = false)
        {
            isLoadingInvoiceID = true;
            //   Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugMedDeptInvoice(OutiID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedOutwardInfo = contract.EndGetOutwardDrugMedDeptInvoice(asyncResult);
                            HideColumnDataGrid(false);
                            GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID,showPaymentWindow);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingInvoiceID = false;
                            // Globals.IsBusy = false;
                        }

                    }), showPaymentWindow);
                }

            });

            t.Start();
        }


        private void AddOutwardDrugReturn()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginOutWardDrugMedDeptInvoiceReturn_Insert(SelectedOutwardInfo, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutiID = 0;
                            contract.EndOutWardDrugMedDeptInvoiceReturn_Insert(out OutiID, asyncResult);
                            if (IsChildWindow)
                            {
                                ////phat su kien de form o duoi load lai trang thai phieu
                                //Globals.EventAggregator.Publish(new DrugDeptCloseFormReturnEvent { });
                            }

                            HideColumnDataGrid(false);
                            if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON)
                            {
                                this.GetOutwardDrugMedDeptInvoiceByID(OutiID, false);
                            }
                            else
                            {
                                //KMx: Hiện tại phiếu xuất đã trả tiền rồi thì không cho trả hàng. Nếu không sẽ bị lỗi. Khi nào Khoa Dược yêu cầu thì sẽ sửa lại (24/12/2014 15:11).
                                //this.GetOutwardDrugMedDeptInvoiceByID(OutiID, true);
                                this.GetOutwardDrugMedDeptInvoiceByID(OutiID, false);
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSave()
        {
            /*▼====: #001*/
            if (GridInward == null || !GridInward.IsValid())
            {
                MessageBox.Show(eHCMSResources.A0976_G1_Msg_InfoSLgTraKhHopLe);
                return;
            }
            /*▲====: #001*/
            if (QtyReturn > 0)
            {
                AddOutwardDrugReturn();
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0442_G1_NhapSLggTra);
            }
        }


        //public void GridInward_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    CountMoney();
        //}

        public void GridInward_CurrentCellChanged(object sender, EventArgs e)
        {
            CountMoney();
        }
        #region search Invoice member

        public void btnSearch()
        {
            MedDeptInvoiceSearchCriteria(0, Globals.PageSize);
        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CustomerName = (sender as TextBox).Text;
                }
                MedDeptInvoiceSearchCriteria(0, Globals.PageSize);
            }
        }

        public void Search_KeyUp_MaPhieuXuat(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CodeInvoice = (sender as TextBox).Text;
                }
                MedDeptInvoiceSearchCriteria(0, Globals.PageSize);
            }
        }

        private void MedDeptInvoiceSearchCriteria(int PageIndex, int PageSize)
        {
            if (SearchCriteria != null)
            {
                SearchCriteria.V_MedProductType = V_MedProductType;
            }
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugMedDeptInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total = 0;
                            var results = contract.EndOutwardDrugMedDeptInvoice_SearchByType(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IXuatNoiBoSearch> onInitDlg = delegate (IXuatNoiBoSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                        {
                                            proAlloc.strHienThi = eHCMSResources.Z1299_G1_TimPhXuatThuoc;
                                        }
                                        else if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                        {
                                            proAlloc.strHienThi = eHCMSResources.Z1300_G1_TimPhXuatYCu;
                                        }
                                        else
                                        {
                                            proAlloc.strHienThi = eHCMSResources.Z1301_G1_TimPhXuatHChat;
                                        }
                                        proAlloc.OutwardMedDeptInvoiceList.Clear();
                                        proAlloc.OutwardMedDeptInvoiceList.TotalItemCount = Total;
                                        proAlloc.OutwardMedDeptInvoiceList.PageIndex = 0;
                                        foreach (OutwardDrugMedDeptInvoice p in results)
                                        {
                                            proAlloc.OutwardMedDeptInvoiceList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IXuatNoiBoSearch>(onInitDlg);
                                }
                                else
                                {
                                    GetCurrentInvoiceInfo(results.FirstOrDefault());
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #endregion

        #region Search Invoice Return Member
        public void Search_KeyUp_Return(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteriaReturn != null)
                {
                    SearchCriteriaReturn.CodeInvoice = (sender as TextBox).Text;
                }
                btnSearchReturn();
            }
        }

        public void btnSearchReturn()
        {
            MedDeptInvoiceSearchCriteria_Return(0, Globals.PageSize);
        }
        private void MedDeptInvoiceSearchCriteria_Return(int PageIndex, int PageSize)
        {
            if (SearchCriteriaReturn != null)
            {
                SearchCriteriaReturn.V_MedProductType = V_MedProductType;
            }
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugMedDeptInvoice_SearchByType(SearchCriteriaReturn, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total = 0;
                            var results = contract.EndOutwardDrugMedDeptInvoice_SearchByType(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IXuatNoiBoSearch> onInitDlg = delegate (IXuatNoiBoSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteriaReturn.DeepCopy();
                                        if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                        {
                                            proAlloc.strHienThi = "TÌM PHIẾU TRẢ THUỐC";
                                        }
                                        else if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                        {
                                            proAlloc.strHienThi = "TÌM PHIẾU TRẢ Y CỤ";
                                        }
                                        else
                                        {
                                            proAlloc.strHienThi = "TÌM PHIẾU TRẢ HÓA CHẤT";
                                        }
                                        proAlloc.OutwardMedDeptInvoiceList.Clear();
                                        proAlloc.OutwardMedDeptInvoiceList.TotalItemCount = Total;
                                        proAlloc.OutwardMedDeptInvoiceList.PageIndex = 0;
                                        foreach (OutwardDrugMedDeptInvoice p in results)
                                        {
                                            proAlloc.OutwardMedDeptInvoiceList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IXuatNoiBoSearch>(onInitDlg);
                                }
                                else
                                {
                                    GetCurrentInvoiceReturnInfo(results.FirstOrDefault());
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        #endregion

        #region printing member

        public void btnPreview()
        {
            Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutwardInfo.outiID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z1302_G1_PhTraThuoc;
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z1303_G1_PhTraYCu;
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z1304_G1_PhTraHChat;
                }
                proAlloc.eItem = ReportName.DRUGDEPT_RETURN_MEDDEPT;
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }
        #endregion

        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                SelectedOutwardInfo = null;
                SelectedOutwardInfo = new OutwardDrugMedDeptInvoice();
            }
        }

        #region IHandle<DrugDeptCloseSearchOutMedDeptInvoiceEvent> Members

        public void Handle(DrugDeptCloseSearchOutMedDeptInvoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                OutwardDrugMedDeptInvoice temp = message.SelectedOutMedDeptInvoice as OutwardDrugMedDeptInvoice;
                if (temp != null)
                {
                    if (temp.ReturnID == null || temp.ReturnID == 0)
                    {
                        GetCurrentInvoiceInfo(temp);
                    }
                    else
                    {
                        GetCurrentInvoiceReturnInfo(temp);
                    }
                }

            }
        }

        #endregion


        private void ShowFormCountMoney()
        {
            Action<ISimplePayPharmacy> onInitDlg = delegate (ISimplePayPharmacy proAlloc)
            {
                if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.HOANTRATHUOC)
                {
                    proAlloc.TotalPayForSelectedItem = 0;
                    proAlloc.TotalPaySuggested = -SelectedOutwardInfo.TotalPatientPayment.DeepCopy();
                }
                else
                {
                    proAlloc.TotalPayForSelectedItem = SelectedOutwardInfo.TotalPatientPayment.DeepCopy();
                    proAlloc.TotalPaySuggested = SelectedOutwardInfo.TotalPatientPayment.DeepCopy();
                }
                proAlloc.StartCalculating();
            };
            GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
        }

        public void PayCmd()
        {
            ShowFormCountMoney();
        }


        #region PharmacyPayEvent
        private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionMedDeptForDrugPayTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            isLoadingGetStore = false;
            yield break;
        }

        private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionMedDeptForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            isLoadingGetStore = false;
            yield break;
        }

        public void Handle(PharmacyPayEvent message)
        {
            if (this.IsActive && message != null)
            {
                if (message.CurPatientPayment != null && message.CurPatientPayment.PayAmount < 0)
                {
                    Coroutine.BeginExecute(AddTransactionHoanTien(message.CurPatientPayment, SelectedOutwardInfo), null, (o, e) =>
                    {
                        GetOutwardDrugMedDeptInvoiceByID(SelectedOutwardInfo.outiID);
                    });
                }
                else
                {
                    Coroutine.BeginExecute(AddTransactionVisitor(message.CurPatientPayment, SelectedOutwardInfo), null, (o, e) =>
                    {
                        btnPreview();
                        GetOutwardDrugMedDeptInvoiceByID(SelectedOutwardInfo.outiID);
                    });
                }
            }
        }
      
        #endregion
    }
}
