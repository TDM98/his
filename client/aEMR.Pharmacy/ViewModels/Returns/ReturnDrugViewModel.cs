using System.Collections.Generic;
using System.ComponentModel.Composition;
using aEMR.CommonTasks;
using aEMR.DataContracts;
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
using aEMR.Common.Printing;
using eHCMSLanguage;
using aEMR.Common.Converters;
using Castle.Windsor;
using aEMR.Controls;

/*
 * 20181010 #001 TBL: BM 0002160. Fix tra hang khi so luong tra lon hon so luong
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IReturnDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReturnDrugViewModel : Conductor<object>, IReturnDrug
        , IHandle<PharmacyCloseSearchReturnEvent>, IHandle<PharmacyCloseSearchReturnInvoiceEvent>
        , IHandle<PharmacyCloseSearchVisitorEvent>
        , IHandle<PayForRegistrationCompleted>
         , IHandle<PharmacyPayEvent>
    {
        public string TitleForm { get; set; }

        #region Indicator Member

        private bool _isLoadingAddTrans = false;
        public bool isLoadingAddTrans
        {
            get { return _isLoadingAddTrans; }
            set
            {
                if (_isLoadingAddTrans != value)
                {
                    _isLoadingAddTrans = value;
                    NotifyOfPropertyChange(() => isLoadingAddTrans);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

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

        private bool _isLoadingOutSatus = false;
        public bool isLoadingOutSatus
        {
            get { return _isLoadingOutSatus; }
            set
            {
                if (_isLoadingOutSatus != value)
                {
                    _isLoadingOutSatus = value;
                    NotifyOfPropertyChange(() => isLoadingOutSatus);
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

        private bool _isLoadingInfoPatient = false;
        public bool isLoadingInfoPatient
        {
            get { return _isLoadingInfoPatient; }
            set
            {
                if (_isLoadingInfoPatient != value)
                {
                    _isLoadingInfoPatient = value;
                    NotifyOfPropertyChange(() => isLoadingInfoPatient);
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

        private bool _isLoadingDetail = false;
        public bool isLoadingDetail
        {
            get { return _isLoadingDetail; }
            set
            {
                if (_isLoadingDetail != value)
                {
                    _isLoadingDetail = value;
                    NotifyOfPropertyChange(() => isLoadingDetail);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }




        public bool IsLoading
        {
            get { return (isLoadingAddTrans || isLoadingGetStore || isLoadingFullOperator || isLoadingInfoPatient || isLoadingOutSatus || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
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
        [ImportingConstructor]
        public ReturnDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            eventArg.Subscribe(this);
            SearchCriteria = new SearchOutwardInfo();
            SearchCriteria.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED;
            //SearchCriteria.V_OutDrugInvStatus = 0;
            SearchCriteriaReturn = new SearchOutwardInfo();

            SelectedOutwardInfo = new OutwardDrugInvoice();
            SelectedOutwardInfo.OutDate = Globals.ServerDate.Value;
            SelectedOutwardInfo.TypID = -1;
            //SelectedOutwardInfo.TypID = (long)AllLookupValues.RefOutputType.HOANTRATHUOC;

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            LoadOutStatus();
            StaffName = GetStaffLogin().FullName;
        }

        //protected override void OnActivate()
        //{
        //}
        //protected override void OnDeactivate(bool close)
        //{
        //    SearchCriteria = null;
        //    SearchCriteriaReturn = null;
        //    SelectedOutwardInfo = null;
        //    Globals.EventAggregator.Unsubscribe(this);
        //}

        #region Properties Member

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

        private bool _IsEnabled = false;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }

        private OutwardDrugInvoice _SelectedOutwardInfo;
        public OutwardDrugInvoice SelectedOutwardInfo
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
                    NotifyOfPropertyChange(() => CanPayCmd);
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

        private SearchOutwardInfo _searchCriteria;
        public SearchOutwardInfo SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private SearchOutwardInfo _searchCriteriaReturn;
        public SearchOutwardInfo SearchCriteriaReturn
        {
            get { return _searchCriteriaReturn; }
            set
            {
                if (_searchCriteriaReturn != value)
                    _searchCriteriaReturn = value;
                NotifyOfPropertyChange(() => SearchCriteriaReturn);
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

        private Visibility _Visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                _Visibility = value;
                if (_Visibility == Visibility.Collapsed)
                {
                    IsVisibility = Visibility.Visible;
                }
                else
                {
                    IsVisibility = Visibility.Collapsed;
                }
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        private Visibility _IsVisibility;
        public Visibility IsVisibility
        {
            get
            {
                return _IsVisibility;
            }
            set
            {
                if (_IsVisibility != value)
                {
                    _IsVisibility = value;
                }
                NotifyOfPropertyChange(() => IsVisibility);
            }
        }

        private Patient _patientInfo;
        public Patient PatientInfo
        {
            get
            {
                return _patientInfo;
            }
            set
            {
                if (_patientInfo != value)
                {
                    _patientInfo = value;
                    NotifyOfPropertyChange(() => PatientInfo);
                }
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

        private bool? _bFlagStoreHI;
        public bool? bFlagStoreHI
        {
            get
            {
                return _bFlagStoreHI;
            }
            set
            {
                if (_bFlagStoreHI != value)
                {
                    _bFlagStoreHI = value;
                    NotifyOfPropertyChange(() => bFlagStoreHI);
                }
            }
        }

        private bool _bFlagPaidTime;
        public bool bFlagPaidTime
        {
            get
            {
                return _bFlagPaidTime;
            }
            set
            {
                if (_bFlagPaidTime != value)
                {
                    _bFlagPaidTime = value;
                    NotifyOfPropertyChange(() => bFlagPaidTime);
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

            mTraHang_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mTraHang,
                                               (int)oPharmacyEx.mTraHang_Tim, (int)ePermission.mView);
            mTraHang_TimPhieuTraHangCu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mTraHang,
                                               (int)oPharmacyEx.mTraHang_TimPhieuTraHangCu, (int)ePermission.mView);
            mTraHang_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mTraHang,
                                               (int)oPharmacyEx.mTraHang_ThongTin, (int)ePermission.mView);
            mTraHang_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mTraHang,
                                               (int)oPharmacyEx.mTraHang_Luu, (int)ePermission.mView);
            mTraHang_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mTraHang,
                                               (int)oPharmacyEx.mTraHang_TraTien, (int)ePermission.mView);
            mTraHang_ReportIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mTraHang,
                                               (int)oPharmacyEx.mTraHang_ReportIn, (int)ePermission.mView);
        }
        #region checking account

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

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            if (bFlagStoreHI == true)
            {
                StoreCbx = (paymentTypeTask.LookupList.Where(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_HIDRUGs)).ToObservableCollection<RefStorageWarehouseLocation>();
            }
            else if (bFlagStoreHI == false)
            {
                StoreCbx = (paymentTypeTask.LookupList.Where(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_EXTERNAL)).ToObservableCollection<RefStorageWarehouseLocation>();
            }
            SetDefaultForStore();
            isLoadingGetStore = false;
            yield break;
        }

        private const string ALLITEMS = "[All]";
        private void LoadOutStatus()
        {
            isLoadingOutSatus = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_OutDrugInvStatus, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllLookupValuesByType(asyncResult);
                            OutStatus = results.ToObservableCollection();
                            Lookup item = new Lookup();
                            item.LookupID = 0;
                            item.ObjectValue = ALLITEMS;
                            OutStatus.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingOutSatus = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        int findPatient = 0;
        private IEnumerator<IResult> DoGetInfoPatient()
        {
            isLoadingInfoPatient = true;
            long? PtRegistrationID = null;
            long? PatientID = null;
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.SelectedPrescription != null)
            {
                PtRegistrationID = SelectedOutwardInfo.PtRegistrationID;
                PatientID = SelectedOutwardInfo.SelectedPrescription.PatientID;
            }
            var paymentTypeTask = new LoadPatientInfoByRegistrationTask(PtRegistrationID, PatientID,findPatient);
            yield return paymentTypeTask;
            PatientInfo = paymentTypeTask.CurrentPatient;
            try
            {
                if (!PatientInfo.AgeOnly.GetValueOrDefault())
                {
                    PatientInfo.DOBText = PatientInfo.DOB.GetValueOrDefault().ToString("dd/MM/yyyy");
                }
                PatientInfo.LatestRegistration = paymentTypeTask.CurrentPatient.LatestRegistration;
                PatientInfo.CurrentHealthInsurance = paymentTypeTask.CurrentPatient.CurrentHealthInsurance;
                PatientInfo.CurrentClassification = paymentTypeTask.CurrentPatient.CurrentClassification;
                if (!SelectedOutwardInfo.IsHICount.GetValueOrDefault())
                    PatientInfo.LatestRegistration.PtInsuranceBenefit = 0;

            }
            catch
            {
            }
            isLoadingInfoPatient = false;
            yield break;

        }

        private void LoadPatientInfoInvoice()
        {
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.PtRegistrationID != null && SelectedOutwardInfo.SelectedPrescription != null)
            {
                Coroutine.BeginExecute(DoGetInfoPatient());
            }
        }

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

        private void GetOutWardDrugInvoiceReturnByID(long OutiID, bool showPaymentWindow = false)
        {
            isLoadingGetID = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutWardDrugInvoiceByID(OutiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            OutwardDrugInvoice temp = SelectedOutwardInfo;
                            SelectedOutwardInfo = contract.EndGetOutWardDrugInvoiceByID(asyncResult);
                            CountMoney();
                            IsEnabled = false;

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
                            isLoadingGetID = false;
                            //Globals.IsBusy = false;
                        }

                    }), showPaymentWindow);
                }

            });

            t.Start();
        }

        private void AddOutwardDrugReturn()
        {     
            this.ShowBusyIndicator();
            //isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            long OutiID = 0;
            SelectedOutwardInfo.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.RETURN;

            var t = new Thread(() =>
            {
                //using (var serviceFactory = new CommonServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginAddOutwardDrugReturn(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),null, SelectedOutwardInfo, SelectedOutwardInfo.OutwardDrugs.ToList(), Globals.DispatchCallback((asyncResult) =>
                    contract.BeginAddOutwardDrugReturn_Pst(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), null, SelectedOutwardInfo, SelectedOutwardInfo.OutwardDrugs.ToList(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            //contract.EndAddOutwardDrugReturn(out OutiID, asyncResult);
                            contract.EndAddOutwardDrugReturn_Pst(out OutiID, asyncResult);
                            if (IsChildWindow)
                            {
                                //phat su kien de form o duoi load lai trang thai phieu
                                Globals.EventAggregator.Publish(new PharmacyCloseFormReturnEvent { });
                            }
                            this.GetOutWardDrugInvoiceReturnByID(OutiID, true);
                            HideColumnDataGrid(false);

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void AddOutwardDrugReturnVisitor()
        {
            long OutiID = 0;
            //isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginAddOutWardDrugInvoiceReturnVisitor(SelectedOutwardInfo, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginAddOutWardDrugInvoiceReturnVisitor_Pst(SelectedOutwardInfo, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            //contract.EndAddOutWardDrugInvoiceReturnVisitor(out OutiID, asyncResult);
                            contract.EndAddOutWardDrugInvoiceReturnVisitor_Pst(out OutiID, asyncResult);
                            if (IsChildWindow)
                            {
                                //phat su kien de form o duoi load lai trang thai phieu
                                Globals.EventAggregator.Publish(new PharmacyCloseFormReturnEvent { });
                            }
                            HideColumnDataGrid(false);
                            if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON)
                            {
                                this.GetOutWardDrugInvoiceReturnByID(OutiID, false);
                            }
                            else
                            {
                                this.GetOutWardDrugInvoiceReturnByID(OutiID, true);
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool CheckPhieuMuon()
        {
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.OutwardDrugs !=null)
            {
                foreach (var item in SelectedOutwardInfo.OutwardDrugs)
                {
                    if (item.OutQuantity != item.OutQuantityReturn)
                    {
                        MessageBox.Show(eHCMSResources.K0269_G1_PhMuonPhaiTraHetThuoc);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckExpriry(ObservableCollection<OutwardDrug> TempCopy)
        {
            string results= "";
            foreach (var item in TempCopy)
            {
                if (eHCMS.Services.Core.AxHelper.CompareDate(Globals.ServerDate.Value, item.InExpiryDate) == 1)
                {
                    results += item.GetDrugForSellVisitor.BrandName + ",";
                }
            }
            if (!string.IsNullOrEmpty(results))
            {
                results += Environment.NewLine + "Đã hết hạn dùng.Bạn có cho phép trả không?";
                if (MessageBox.Show(results, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private ObservableCollection<OutwardDrug> TempCopy;
        public void btnSave()
        {
            if (GridInward == null || !GridInward.IsValid())
            {
                MessageBox.Show(eHCMSResources.A0976_G1_Msg_InfoSLgTraKhHopLe);
                /*▼====: #001*/
                return;
                /*▲====: #001*/
            }
            TempCopy = SelectedOutwardInfo.OutwardDrugs.DeepCopy();
            ObservableCollection<OutwardDrug> items = SelectedOutwardInfo.OutwardDrugs.Where(x => x.OutQuantityReturn > 0).ToObservableCollection();
            if (CheckExpriry(items))
            {
                if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON)
                {
                    //neu la phieu muon thuoc thi bat buoc phai tra het,chu khong duoc tra lac nhac
                    if (!CheckPhieuMuon())
                    {
                        return;
                    }
                }

                //if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.BANTHEOTOA && Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) != 1)
                // Txd 25/05/2014 Replaced ConfigList
                if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.BANTHEOTOA && Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent != 1)
                {
                    AddOutwardDrugReturn();
                }
                else
                {
                    AddOutwardDrugReturnVisitor();
                }
            }
        }

        //public void GridInward_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    CountMoney();
        //    if (SumTotalPriceReturn > 0)
        //    {
        //        IsEnabled = true;
        //    }
        //    else
        //    {
        //        IsEnabled = false;
        //    }
        //}

        public void GridInward_CurrentCellChanged(object sender, EventArgs e)
        {
            CountMoney();
            if (SumTotalPriceReturn > 0)
            {
                IsEnabled = true;
            }
            else
            {
                IsEnabled = false;
            }
        }

        #region search member

        public void btnSearch()
        {
            SearchOutwardInfo(0, 20);
        }

        public void btnSearchAdvance()
        {
            Action<ICollectionDrugSearch> onInitDlg = delegate (ICollectionDrugSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.bFlagStoreHI = bFlagStoreHI;
                proAlloc.bFlagPaidTime = bFlagPaidTime;
                proAlloc.pageTitle = eHCMSResources.G1669_G1_TraThuocTimPh;
            };
            GlobalsNAV.ShowDialog<ICollectionDrugSearch>(onInitDlg);
        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchOutwardInfo(0, 20);
            }
        }

        public void Search_KeyUp_MaPhieuXuat(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.OutInvID = (sender as TextBox).Text;
                }
                SearchOutwardInfo(0, 20);
            }
        }

        public void Search_KeyUp_HICardCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.HICardCode = (sender as TextBox).Text;
                }
                SearchOutwardInfo(0, 20);
            }
        }

        TextBox SearchMaPhieuXuatTextBox;
        public void SearchMaPhieuXuatTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            SearchMaPhieuXuatTextBox = (TextBox)sender;
            SearchMaPhieuXuatTextBox.Focus();
        }

        private void SearchOutwardInfo(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            //isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            //KMx: Nếu tìm kiếm mà không theo tiêu chí nào hết thì phải giới hạn ngày (08/08/2014 09:51).
            if (SearchCriteria == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(SearchCriteria.PatientCode) &&  string.IsNullOrEmpty(SearchCriteria.CustomerName) && string.IsNullOrEmpty(SearchCriteria.HICardCode) && string.IsNullOrEmpty(SearchCriteria.OutInvID))
            {
                SearchCriteria.fromdate = Globals.GetCurServerDateTime();
                SearchCriteria.todate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.fromdate = null;
                SearchCriteria.todate = null;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutWardDrugInvoiceSearchAllByStatus(SearchCriteria, PageIndex, PageSize, true, bFlagStoreHI, bFlagPaidTime, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total = 0;
                            var results = contract.EndGetOutWardDrugInvoiceSearchAllByStatus(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<ICollectionDrugSearch> onInitDlg = delegate (ICollectionDrugSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.OutwardInfoList.Clear();
                                        proAlloc.OutwardInfoList.TotalItemCount = Total;
                                        proAlloc.OutwardInfoList.PageIndex = 0;
                                        proAlloc.OutwardInfoList.PageSize = 20;
                                        proAlloc.bFlagPaidTime = bFlagPaidTime;
                                        proAlloc.bFlagStoreHI = bFlagStoreHI;
                                        proAlloc.pageTitle = eHCMSResources.G1669_G1_TraThuocTimPh;
                                        foreach (OutwardDrugInvoice p in results)
                                        {
                                            proAlloc.OutwardInfoList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<ICollectionDrugSearch>(onInitDlg);
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
                            this.HideBusyIndicator();
                            //isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #endregion

        public void GetCurrentInvoiceInfo(OutwardDrugInvoice Current)
        {
            IsReturnCost = false;
            SelectedOutwardInfo = Current;
            if (SelectedOutwardInfo.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)
            {
                Globals.ShowMessage(eHCMSResources.Z1691_G1_BNChuaNhanThuoc, eHCMSResources.G0442_G1_TBao);
            }
            if (SelectedOutwardInfo.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                Globals.ShowMessage(eHCMSResources.Z1692_G1_PhXuatDaDcHuy, eHCMSResources.G0442_G1_TBao);
            }
            if (SelectedOutwardInfo.PrescriptID != null && SelectedOutwardInfo.PrescriptID > 0)
            {
                Visibility = Visibility.Visible;
                LoadPatientInfoInvoice();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
            HideColumnDataGrid(true);
            GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID);
        }

        private void GetOutwardDrugDetailsByOutwardInvoice(long OutiID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutwardDrugDetailsByOutwardInvoice(OutiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardDrugDetailsByOutwardInvoice(asyncResult);
                            SelectedOutwardInfo.OutwardDrugs = results.ToObservableCollection();
                            CopyInvoiceFirst();
                            //KMx: Nhà thuốc yêu cầu để mặc định trả hàng theo giá vốn (15/02/2014 09:46)
                            //20190306 TBL: Theo anh Tuan keu khong tra theo gia von va an luon nen set False
                            IsReturnCost = false;
                            CountMoney();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void CountMoney()
        {
            if (SelectedOutwardInfo == null || SelectedOutwardInfo.OutwardDrugs == null)
            {
                return;
            }

            SumTotalPriceReturn = 0;
            SumTotalPriceHIReturn = 0;
            SumTotalPricePatientReturn = 0;

            SelectedOutwardInfo.TotalInvoicePrice = 0;
            SelectedOutwardInfo.TotalHIPayment = 0;
            SelectedOutwardInfo.TotalCoPayment = 0;
            SelectedOutwardInfo.TotalPatientPayment = 0;
            SelectedOutwardInfo.TotalPriceDifference = 0;

            //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;
            if (!onlyRoundResultForOutward)
            {
                //KMx: Nếu load phiếu trả thuốc cũ thì vào cái if này. Nếu load phiếu xuất để trả thuốc thì vào else (02/08/2014 16:01).
                if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.HOANTRATHUOC)
                {
                    foreach (OutwardDrug p in SelectedOutwardInfo.OutwardDrugs)
                    {
                        SumTotalPriceReturn = SumTotalPriceReturn + p.TotalPrice;
                        SumTotalPriceHIReturn = SumTotalPriceHIReturn + p.TotalHIPayment;

                        SelectedOutwardInfo.TotalPriceDifference += p.PriceDifference * p.OutQuantity;
                    }
                }
                else
                {
                    foreach (OutwardDrug p in SelectedOutwardInfo.OutwardDrugs)
                    {
                        //p.TotalCoPayment > 0 tức là bệnh nhân được hưởng bảo hiểm < 100%
                        if (p.TotalCoPayment > 0)
                        {
                            p.OutHIRebateReturn = (decimal)(Math.Floor((double)(p.HIAllowedPrice.GetValueOrDefault() * (decimal)p.HIBenefit.GetValueOrDefault() * p.OutQuantityReturn)));
                        }
                        //Bệnh nhân được hưởng bảo hiểm 100%
                        else
                        {
                            p.OutHIRebateReturn = (decimal)(Math.Floor((double)p.HIAllowedPrice.GetValueOrDefault() * p.OutQuantityReturn));
                        }
                        p.TotalHIPayment = (decimal)(Math.Floor((double)p.OutHIRebateReturn.GetValueOrDefault(0)));
                        //KMx: Tại sao lại thay đổi p.TotalCoPayment (bệnh nhân đồng chi trả với bảo hiểm)?
                        //     TotalCoPayment không được thay đổi.
                        //if (p.TotalHIPayment > 0)
                        //{
                        //    p.TotalCoPayment =(decimal)(Math.Ceiling((double)(p.TotalPriceReturn - p.TotalHIPayment - (p.PriceDifference * p.OutQuantityReturn))));
                        //}
                        p.PatientReturn = p.TotalPriceReturn - p.OutHIRebateReturn;

                        SumTotalPriceReturn = SumTotalPriceReturn + p.TotalPriceReturn;
                        SumTotalPriceHIReturn = SumTotalPriceHIReturn + p.OutHIRebateReturn.GetValueOrDefault();

                        SelectedOutwardInfo.TotalPriceDifference += p.PriceDifference * p.OutQuantityReturn;
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
            else
            {
                decimal totalHIAllowedPrice = 0;

                //KMx: Nếu load phiếu trả thuốc cũ thì vào cái if này. Nếu load phiếu xuất để trả thuốc thì vào else (02/08/2014 16:01).
                if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.HOANTRATHUOC)
                {
                    foreach (OutwardDrug p in SelectedOutwardInfo.OutwardDrugs)
                    {
                        SumTotalPriceReturn = SumTotalPriceReturn + p.TotalPrice;
                        SumTotalPriceHIReturn = SumTotalPriceHIReturn + p.TotalHIPayment;

                        //SelectedOutwardInfo.TotalPriceDifference += p.PriceDifference * p.OutQuantity;
                        totalHIAllowedPrice += p.HIAllowedPrice.GetValueOrDefault() * p.OutQuantity;
                    }
                }
                else
                {
                    foreach (OutwardDrug p in SelectedOutwardInfo.OutwardDrugs)
                    {
                        p.OutHIRebateReturn = p.HIAllowedPrice.GetValueOrDefault() * (decimal)p.HIBenefit.GetValueOrDefault() * p.OutQuantityReturn;

                        p.TotalHIPayment = p.OutHIRebateReturn.GetValueOrDefault(0);

                        p.PatientReturn = p.TotalPriceReturn - p.OutHIRebateReturn;

                        p.TotalCoPayment = p.HIAllowedPrice.GetValueOrDefault() * p.OutQuantityReturn - p.TotalHIPayment;

                        p.PriceDifference = p.OutPrice - p.HIAllowedPrice.GetValueOrDefault();

                        SumTotalPriceReturn = SumTotalPriceReturn + p.TotalPriceReturn;
                        SumTotalPriceHIReturn = SumTotalPriceHIReturn + p.OutHIRebateReturn.GetValueOrDefault();

                        //SelectedOutwardInfo.TotalPriceDifference += p.PriceDifference * p.OutQuantityReturn;
                        totalHIAllowedPrice += p.HIAllowedPrice.GetValueOrDefault() * p.OutQuantityReturn;
                    }
                }
                SumTotalPriceReturn = MathExt.Round(SumTotalPriceReturn, aEMR.Common.Converters.MidpointRounding.AwayFromZero);
                SumTotalPriceHIReturn = MathExt.Round(SumTotalPriceHIReturn, aEMR.Common.Converters.MidpointRounding.AwayFromZero);

                SumTotalPricePatientReturn = SumTotalPriceReturn - SumTotalPriceHIReturn;

                SelectedOutwardInfo.TotalInvoicePrice = SumTotalPriceReturn;
                SelectedOutwardInfo.TotalHIPayment = SumTotalPriceHIReturn;
                if (SelectedOutwardInfo.TotalHIPayment > 0)
                {
                    SelectedOutwardInfo.TotalCoPayment = totalHIAllowedPrice - SumTotalPriceHIReturn;
                }
                SelectedOutwardInfo.TotalPatientPayment = SumTotalPricePatientReturn;
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

        private void GetOutWardDrugInvoiceSearchReturn(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            //isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            //KMx: Nếu tìm kiếm mà không theo tiêu chí nào hết thì phải giới hạn ngày (08/08/2014 09:51).
            if (SearchCriteriaReturn == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(SearchCriteriaReturn.OutInvID))
            {
                SearchCriteriaReturn.fromdate = Globals.GetCurServerDateTime().AddDays(-1);
                SearchCriteriaReturn.todate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteriaReturn.fromdate = null;
                SearchCriteriaReturn.todate = null;
            }


            int Total = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceSearchReturn(SearchCriteriaReturn, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutWardDrugInvoiceSearchReturn(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo popup tim kiem
                                    Action<IReturnDrugSearchInvoice> onInitDlg = delegate (IReturnDrugSearchInvoice proAlloc)
                                    {
                                        proAlloc.SearchCriteriaReturn = SearchCriteriaReturn.DeepCopy();
                                        proAlloc.OutwardDrugInvoices.Clear();
                                        proAlloc.OutwardDrugInvoices.TotalItemCount = Total;
                                        proAlloc.OutwardDrugInvoices.PageIndex = 0;
                                        proAlloc.OutwardDrugInvoices.PageSize = 20;
                                        foreach (OutwardDrugInvoice p in results)
                                        {
                                            proAlloc.OutwardDrugInvoices.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IReturnDrugSearchInvoice>(onInitDlg);
                                }
                                else
                                {
                                    SelectedOutwardInfo = new OutwardDrugInvoice();
                                    SelectedOutwardInfo = results.FirstOrDefault();

                                    //KMx: Nếu chỉ có 1 kết quả thì phải set lại như bên dưới, nếu không vẫn còn lưu lại thông tin của phiếu trước.
                                    if (SelectedOutwardInfo.PrescriptID != null && SelectedOutwardInfo.PrescriptID > 0)
                                    {
                                        Visibility = Visibility.Visible;
                                        LoadPatientInfoInvoice();
                                    }
                                    else
                                    {
                                        Visibility = Visibility.Collapsed;
                                    }

                                    //doi lai\
                                    HideColumnDataGrid(false);
                                    GetOutWardDrugInvoiceReturnByID(SelectedOutwardInfo.ReturnID.GetValueOrDefault());
                                    ////count return 

                                    //GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.ReturnID.GetValueOrDefault());
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
                            this.HideBusyIndicator();
                            //isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearchReturn()
        {
            GetOutWardDrugInvoiceSearchReturn(0, 20);
        }

        public void btnSearchReturnAdvance()
        {
            //mo popup tim kiem
            Action<IReturnDrugSearchInvoice> onInitDlg = delegate (IReturnDrugSearchInvoice proAlloc)
            {
                proAlloc.SearchCriteriaReturn = SearchCriteriaReturn.DeepCopy();
            };
            GlobalsNAV.ShowDialog<IReturnDrugSearchInvoice>(onInitDlg);
        }

        public void Search_KeyUp_Return(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteriaReturn != null)
                {
                    SearchCriteriaReturn.OutInvID = (sender as TextBox).Text;
                }
                btnSearchReturn();
            }
        }

        #region IHandle<PharmacyCloseSearchReturnEvent> Members

        public void Handle(PharmacyCloseSearchReturnEvent message)
        {

            if (message != null)
            {
                SelectedOutwardInfo = message.SelectedInvoice as OutwardDrugInvoice;
                HideColumnDataGrid(true);
                GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID);
            }
        }

        #endregion

        #region IHandle<PharmacyCloseSearchReturnInvoiceEvent> Members

        public void Handle(PharmacyCloseSearchReturnInvoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardInfo = message.SelectedInvoice as OutwardDrugInvoice;
                if (SelectedOutwardInfo.PrescriptID != null && SelectedOutwardInfo.PrescriptID > 0)
                {
                    Visibility = Visibility.Visible;
                    LoadPatientInfoInvoice();
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
                HideColumnDataGrid(false);
                GetOutWardDrugInvoiceReturnByID(SelectedOutwardInfo.ReturnID.GetValueOrDefault());
                //GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.ReturnID.GetValueOrDefault());
                IsEnabled = false;
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutwardInfo.outiID;
                if (SelectedOutwardInfo.DrugInvoice != null && SelectedOutwardInfo.DrugInvoice.IsHICount.GetValueOrDefault())
                {
                    proAlloc.eItem = ReportName.PHARMACY_TRATHUOCBH;
                }
                else
                {
                    proAlloc.eItem = ReportName.PHARMACY_TRATHUOC;
                }
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
        }
        public void btnPrint()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetReturnDrugInPdfFormat(SelectedOutwardInfo.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetReturnDrugInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            Globals.EventAggregator.Publish(results);
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
        #endregion

        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                SelectedOutwardInfo = null;
                SelectedOutwardInfo = new OutwardDrugInvoice();
                SelectedOutwardInfo.OutDate = Globals.ServerDate.Value;
            }
        }

        #region IHandle<PharmacyCloseSearchVisitorEvent> Members

        public void Handle(PharmacyCloseSearchVisitorEvent message)
        {
            if (message != null && this.IsActive)
            {
                GetCurrentInvoiceInfo(message.SelectedOutwardInfo as OutwardDrugInvoice);
            }
        }

        #endregion

        public bool CanPayCmd
        {
            get
            {
                bool isXNB_ChoMuon = false;
                if (SelectedOutwardInfo != null && SelectedOutwardInfo.DrugInvoice != null && SelectedOutwardInfo.DrugInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_CHOMUON)
                {
                    isXNB_ChoMuon = true;
                }
                return SelectedOutwardInfo != null && SelectedOutwardInfo.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE && !isXNB_ChoMuon;
            }
        }

        private void ShowFormCountMoney()
        {
            //var proAlloc = Globals.GetViewModel<ISimplePayPharmacy>();
            //proAlloc.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
            //if (SelectedOutwardInfo.TypID == (long)AllLookupValues.RefOutputType.HOANTRATHUOC)
            //{
            //    proAlloc.TotalPayForSelectedItem = 0;
            //    proAlloc.TotalPaySuggested = -SelectedOutwardInfo.TotalPatientPayment.DeepCopy();
            //}
            //else
            //{
            //    proAlloc.TotalPayForSelectedItem = SelectedOutwardInfo.TotalPatientPayment.DeepCopy();
            //    proAlloc.TotalPaySuggested = SelectedOutwardInfo.TotalPatientPayment.DeepCopy();
            //}
            //proAlloc.StartCalculating();
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ISimplePayPharmacy> onInitDlg = (proAlloc) =>
            {
                proAlloc.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
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
            // ShowFormCountMoney();
            if (SelectedOutwardInfo != null
                && SelectedOutwardInfo.ReturnID.HasValue
                && SelectedOutwardInfo.DrugInvoice != null                
                //&& SelectedOutwardInfo.DrugInvoice.PtRegistrationID > 0 && (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) != 1))
                // Txd 25/05/2014 Replaced ConfigList
                && SelectedOutwardInfo.DrugInvoice.PtRegistrationID > 0 && Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent != 1)
            {

                if (SelectedOutwardInfo.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.SAVE)
                {
                    MessageBox.Show(eHCMSResources.A0712_G1_Msg_InfoKhTheTinhTien);
                    return;
                }
                PayForRegistration(SelectedOutwardInfo.DrugInvoice.PtRegistrationID.Value);
            }
            else
            {
                if (SelectedOutwardInfo.DrugInvoice != null && SelectedOutwardInfo.DrugInvoice.PtRegistrationID > 0)
                {
                    SelectedOutwardInfo.PtRegistrationID = SelectedOutwardInfo.DrugInvoice.PtRegistrationID;
                }
                ShowFormCountMoney();
            }
        }



        private IEnumerator<IResult> DoPayForRegistration(long regID)
        {
            var loadRegInfoTask = new LoadRegistrationInfoTask(regID);
            yield return loadRegInfoTask;

            if (loadRegInfoTask.Error == null)
            {
                //var vm2 = Globals.GetViewModel<ISimplePay2>();
                //vm2.Registration = loadRegInfoTask.Registration;
                //vm2.PayForSelectedItemOnly = true;
                //vm2.RegistrationDetails = null;
                //vm2.PclRequests = null;

                //vm2.DrugInvoices = new List<OutwardDrugInvoice>() { SelectedOutwardInfo };
                //vm2.StartCalculating();
                //Globals.ShowDialog(vm2 as Conductor<object>);

                var vm = Globals.GetViewModel<ISimplePay>();
                vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                vm.Registration = loadRegInfoTask.Registration;
                vm.FormMode = PaymentFormMode.PAY;

                vm.RegistrationDetails = null;
                vm.PclRequests = null;

                vm.DrugInvoices = new List<OutwardDrugInvoice>() { SelectedOutwardInfo };
                vm.StartCalculating();

                if (vm.TotalPayForSelectedItem != vm.TotalPaySuggested)
                {
                    Action<ISimplePay> onInitDlg = delegate (ISimplePay _vm)
                    {
                        _vm = vm;
                    };
                    GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
                }
                else
                {
                    Action<ISimplePay2> onInitDlg = delegate (ISimplePay2 vm2)
                    {
                        vm2.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                        vm2.Registration = loadRegInfoTask.Registration;

                        vm2.RegistrationDetails = null;
                        vm2.PclRequests = null;

                        vm2.DrugInvoices = new List<OutwardDrugInvoice>() { SelectedOutwardInfo };
                        vm2.StartCalculating();
                    };
                    GlobalsNAV.ShowDialog<ISimplePay2>(onInitDlg);
                }
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
            }
            yield break;
        }

        public void PayForRegistration(long registrationID)
        {
            if (registrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0712_G1_Msg_InfoKhTheTinhTien);
                return;
            }
            //Coroutine.BeginExecute(DoPayForRegistration(registrationID));
            Coroutine.BeginExecute(DoRefund(SelectedOutwardInfo.DrugInvoice.PtRegistrationID.Value, SelectedOutwardInfo.outiID));
            //GetOutWardDrugInvoiceReturnByID(SelectedOutwardInfo.outiID);
          
        }

        public IEnumerator<IResult> DoRefund(long registrationID, long outiID)
        {
            //Neu co dich vu nao o trang thai hoan tien ma chua hoan tien thi hoan tien luon.
            var loadRegInfoTask = new LoadRegistrationInfoTask(registrationID);
            yield return loadRegInfoTask;
            Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
            {
                vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                vm.Registration = loadRegInfoTask.Registration;
                vm.FormMode = PaymentFormMode.PAY;

                List<PatientRegistrationDetail> lsRegDetails = null;
                var lsPclRequests = new List<PatientPCLRequest>();
                if (loadRegInfoTask.Registration.PatientRegistrationDetails != null)
                {
                    lsRegDetails = loadRegInfoTask.Registration.PatientRegistrationDetails.Where(item => item.PaidTime != null && item.RefundTime == null &&
                                                                                                   item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                                                                   && item.RecordState == RecordState.UNCHANGED).ToList();
                }
                if (loadRegInfoTask.Registration.PCLRequests != null)
                {
                    foreach (var request in loadRegInfoTask.Registration.PCLRequests)
                    {
                        //Neu co 1 request detail bi delete va chua hoan tien thi add nguyen cai request nay luon.
                        if (request.PatientPCLRequestIndicators != null)
                        {
                            if (request.PaidTime != null && request.RefundTime == null &&
                                request.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL && request.RecordState == RecordState.UNCHANGED)
                            {
                                lsPclRequests.Add(request);
                                continue;
                            }

                            if (request.PatientPCLRequestIndicators.Any(requestDetail => requestDetail.PaidTime != null && requestDetail.RefundTime == null &&
                                                                                         requestDetail.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                                                         && requestDetail.RecordState == RecordState.UNCHANGED))
                            {
                                lsPclRequests.Add(request);
                            }
                        }
                    }
                }

                vm.RegistrationDetails = lsRegDetails;
                vm.PclRequests = lsPclRequests;
                vm.DrugInvoices = new List<OutwardDrugInvoice>() { SelectedOutwardInfo };
                vm.StartCalculating();
            };
            GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);

            GetOutWardDrugInvoiceReturnByID(SelectedOutwardInfo.outiID);
        }

        public void Handle(PayForRegistrationCompleted message)
        {
            if (this.GetView() != null && message != null)
            {
                if (SelectedOutwardInfo != null
                && SelectedOutwardInfo.ReturnID.HasValue
                && SelectedOutwardInfo.DrugInvoice != null
                && SelectedOutwardInfo.DrugInvoice.PtRegistrationID > 0
                && message.Registration != null
                && message.Registration.PtRegistrationID == SelectedOutwardInfo.DrugInvoice.PtRegistrationID)
                {
                    GetOutWardDrugInvoiceReturnByID(SelectedOutwardInfo.outiID);
                    btnPreview();
                }
            }
        }

        #region IsReturnCost Member

        private bool _IsReturnCost;
        public bool IsReturnCost
        {
            get { return _IsReturnCost; }
            set
            {
                if (_IsReturnCost != value)
                {
                    _IsReturnCost = value;
                    NotifyOfPropertyChange(() => IsReturnCost);
                    if (_IsReturnCost)
                    {
                        GetPriceCost();
                    }
                    else
                    {
                        GetPriceSell();
                    }
                }
            }
        }

        private ObservableCollection<OutwardDrug> OutwardDrugDetailCopy;

        private void CopyInvoiceFirst()
        {
            if (SelectedOutwardInfo != null)
            {
                OutwardDrugDetailCopy = SelectedOutwardInfo.OutwardDrugs.DeepCopy();
            }
            else
            {
                OutwardDrugDetailCopy = null;
            }
        }

        private void GetPriceCost()
        {
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.OutwardDrugs != null)
            {
                foreach (OutwardDrug item in SelectedOutwardInfo.OutwardDrugs)
                {
                    if (item.HIAllowedPrice > 0)
                    {
                        item.OutPrice = item.HIAllowedPrice.GetValueOrDefault();

                    }
                    else
                    {
                        if (item.InwardDrug != null)
                        {
                            item.OutPrice = item.InwardDrug.InBuyingPriceActual;
                        }
                    }
                    item.InvoicePrice = item.OutPrice;
                    item.OutQuantityReturn = 0;
                }
                CountMoney();
            }

        }

        private void GetPriceSell()
        {
            if (SelectedOutwardInfo != null)
            {
                SelectedOutwardInfo.OutwardDrugs = OutwardDrugDetailCopy.DeepCopy();
                CountMoney();
            }
        }
        #endregion


        private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionForDrugPayTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID);
            yield return paymentTypeTask;
            isLoadingGetStore = false;
            yield break;
        }

        private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
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
                        GetOutWardDrugInvoiceReturnByID(SelectedOutwardInfo.outiID);
                    });
                }
                else
                {
                    Coroutine.BeginExecute(AddTransactionVisitor(message.CurPatientPayment, SelectedOutwardInfo), null, (o, e) =>
                    {
                        btnPreview();
                        GetOutWardDrugInvoiceReturnByID(SelectedOutwardInfo.outiID);
                    });
                }
            }
        }
    }
}
