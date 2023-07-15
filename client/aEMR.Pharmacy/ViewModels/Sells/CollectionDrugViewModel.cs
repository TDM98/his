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
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Common.Printing;
using aEMR.Common;
using System.Collections.Generic;
using aEMR.CommonTasks;
using eHCMSLanguage;
using Castle.Windsor;
using System.Text;
/*
* 20181103 #001 TNHX: [BM0005214] Update report PhieuNhanThuoc base RefApplicationConfig.MixedHIPharmacyStores
*/
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ICollectionDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CollectionDrugViewModel : Conductor<object>, ICollectionDrug
        , IHandle<PharmacyCloseSearchVisitorEvent>
         , IHandle<PharmacyPayEvent>
         , IHandle<PayForRegistrationCompleted>
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

        private bool _isLoadingOutStatus = false;
        public bool isLoadingOutStatus
        {
            get { return _isLoadingOutStatus; }
            set
            {
                if (_isLoadingOutStatus != value)
                {
                    _isLoadingOutStatus = value;
                    NotifyOfPropertyChange(() => isLoadingOutStatus);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingCount = false;
        public bool isLoadingCount
        {
            get { return _isLoadingCount; }
            set
            {
                if (_isLoadingCount != value)
                {
                    _isLoadingCount = value;
                    NotifyOfPropertyChange(() => isLoadingCount);
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
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingCount || isLoadingInfoPatient || isLoadingOutStatus || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion
        [ImportingConstructor]
        public CollectionDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            authorization();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            LoadOutStatus();
            GetStaffLogin();

            RefeshData();
            SetDefaultForStore();

        }

        private void RefeshData()
        {
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugInvoice();
            SelectedOutInvoice.OutDate = Globals.ServerDate.Value;


            SearchCriteria = null;
            SearchCriteria = new SearchOutwardInfo();
            SearchCriteria.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;

            OutwardDrugsCopy = null;
            PatientInfo = null;
        }

        //protected override void OnActivate()
        //{

        //}

        //protected override void OnDeactivate(bool close)
        //{
        //    SelectedOutInvoice = null;
        //    SearchCriteria = null;
        //    SearchCriteria = null;
        //    OutwardDrugsCopy = null;
        //    PatientInfo = null;

        //    StoreCbx = null;
        //    OutStatus = null;

        //}

        private void SetDefaultForStore()
        {
            if (StoreCbx != null)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
                SearchCriteria.StoreID = StoreCbx.FirstOrDefault().StoreID;
            }
        }

        #region Properties Member

        public string TitleForm { get; set; }
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

        private Staff GetStaffLogin()
        {
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            return Globals.LoggedUserAccount.Staff;
        }

        private SearchOutwardInfo _SearchCriteria;
        public SearchOutwardInfo SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private ObservableCollection<OutwardDrug> OutwardDrugsCopy;

        private OutwardDrugInvoice _SelectedOutInvoice;
        public OutwardDrugInvoice SelectedOutInvoice
        {
            get
            {
                return _SelectedOutInvoice;
            }
            set
            {
                if (_SelectedOutInvoice != value)
                {
                    _SelectedOutInvoice = value;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoice);
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

        private bool? _bFlagStoreHI;
        public bool? bFlagStoreHI
        {
            get
            {
                return _bFlagStoreHI;
            }
            set
            {
                if(_bFlagStoreHI != value)
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

            mNhanThuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhanThuoc,
                                               (int)oPharmacyEx.mNhanThuoc_Tim, (int)ePermission.mView);
            mNhanThuoc_ThongTin = mNhanThuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhanThuoc,
                                               (int)oPharmacyEx.mNhanThuoc_ThongTin, (int)ePermission.mView);
            mNhanThuoc_DaLayThuoc = mNhanThuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhanThuoc,
                                               (int)oPharmacyEx.mNhanThuoc_DaLayThuoc, (int)ePermission.mView);
            mNhanThuoc_HuyPhieuXuat = mNhanThuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhanThuoc,
                                               (int)oPharmacyEx.mNhanThuoc_HuyPhieuXuat, (int)ePermission.mView);
            mNhanThuoc_TraHang = mNhanThuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhanThuoc,
                                               (int)oPharmacyEx.mNhanThuoc_TraHang, (int)ePermission.mView);
            mNhanThuoc_ReportIn = mNhanThuoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhanThuoc,
                                               (int)oPharmacyEx.mNhanThuoc_ReportIn, (int)ePermission.mView);
        }

        #region checking account

        private bool _mNhanThuoc_Tim = true;
        private bool _mNhanThuoc_ThongTin = true;
        private bool _mNhanThuoc_DaLayThuoc = true;
        private bool _mNhanThuoc_HuyPhieuXuat = true;
        private bool _mNhanThuoc_TraHang = true;
        private bool _mNhanThuoc_ReportIn = true;

        public bool mNhanThuoc_Tim
        {
            get
            {
                return _mNhanThuoc_Tim;
            }
            set
            {
                if (_mNhanThuoc_Tim == value)
                    return;
                _mNhanThuoc_Tim = value;
            }
        }
        public bool mNhanThuoc_ThongTin
        {
            get
            {
                return _mNhanThuoc_ThongTin;
            }
            set
            {
                if (_mNhanThuoc_ThongTin == value)
                    return;
                _mNhanThuoc_ThongTin = value;
            }
        }
        public bool mNhanThuoc_DaLayThuoc
        {
            get
            {
                return _mNhanThuoc_DaLayThuoc;
            }
            set
            {
                if (_mNhanThuoc_DaLayThuoc == value)
                    return;
                _mNhanThuoc_DaLayThuoc = value;
            }
        }
        public bool mNhanThuoc_HuyPhieuXuat
        {
            get
            {
                return _mNhanThuoc_HuyPhieuXuat;
            }
            set
            {
                if (_mNhanThuoc_HuyPhieuXuat == value)
                    return;
                _mNhanThuoc_HuyPhieuXuat = value;
            }
        }
        public bool mNhanThuoc_TraHang
        {
            get
            {
                return _mNhanThuoc_TraHang;
            }
            set
            {
                if (_mNhanThuoc_TraHang == value)
                    return;
                _mNhanThuoc_TraHang = value;
            }
        }
        public bool mNhanThuoc_ReportIn
        {
            get
            {
                return _mNhanThuoc_ReportIn;
            }
            set
            {
                if (_mNhanThuoc_ReportIn == value)
                    return;
                _mNhanThuoc_ReportIn = value;
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
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            if (bFlagStoreHI == true)
            {
                StoreCbx = (paymentTypeTask.LookupList.Where(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_HIDRUGs)).ToObservableCollection<RefStorageWarehouseLocation>();
            }
            else if(bFlagStoreHI == false)
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
            isLoadingOutStatus = true;
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
                            isLoadingOutStatus = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        int findPatient = 0;
        private IEnumerator<IResult> DoGetInfoPatientInvoice()
        {
            isLoadingInfoPatient = true;
            long? PtRegistrationID = null;
            long? PatientID = null;
            if (SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null)
            {
                PtRegistrationID = SelectedOutInvoice.PtRegistrationID;
                PatientID = SelectedOutInvoice.SelectedPrescription.PatientID;
            }
            var paymentTypeTask = new LoadPatientInfoByRegistrationTask(PtRegistrationID, PatientID, findPatient);
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
                if (!SelectedOutInvoice.IsHICount.GetValueOrDefault())
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
            if (SelectedOutInvoice != null)
            {
                Coroutine.BeginExecute(DoGetInfoPatientInvoice());
            }
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private void CalcOutwardDrugInvoice(long RegistrationID, OutwardDrugInvoice Inv)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginCalcOutwardDrugInvoice(RegistrationID, Inv, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            OutwardDrugInvoice OutInvoice;
                            PayableSum payableSum;
                            decimal TotalPaid;
                            contract.EndCalcOutwardDrugInvoice(out OutInvoice, out payableSum, out TotalPaid, asyncResult);

                            SelectedOutInvoice = OutInvoice;


                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public int GetClassicationPaytient()
        {
            if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null)
            {
                return 1;
            }
            else
            {
                //0:benh nhan thong thuong,1:benh nhan bao hiem
                return 0;
            }

        }

        private bool CheckValid()
        {
            bool result = true;
            if (SelectedOutInvoice != null)
            {
                if (SelectedOutInvoice.OutwardDrugs == null)
                {
                    return false;
                }
                foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
                {
                    if (item.Validate() == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private void GetOutWardDrugInvoiceVisitorByID(long OutwardID)
        {
            isLoadingGetID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceVisitorByID(OutwardID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedOutInvoice = contract.EndGetOutWardDrugInvoiceVisitorByID(asyncResult);
                            if (SelectedOutInvoice != null)
                            {
                                SelectedOutInvoice.OutwardDrugs = OutwardDrugsCopy;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void OutWardDrugInvoice_UpdateStatus(OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingFullOperator = true;
            //   Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginOutWardDrugInvoice_UpdateStatus(InvoiceDrug, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int results = contract.EndOutWardDrugInvoice_UpdateStatus(asyncResult);
                            if (results > 0)
                            {
                                Globals.ShowMessage(eHCMSResources.A0923_G1_Msg_InfoPhKhTonTai, eHCMSResources.G0442_G1_TBao);
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.A0282_G1_Msg_InfoCNhatStatusOK, eHCMSResources.G0442_G1_TBao);
                                InvoiceDrug.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED;
                                InvoiceDrug.CalculateState();
                                //InvoiceDrug.OutDrugInvStatus = "Đã lấy thuốc";
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

        public void btnSaveStatus(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                if (MessageBox.Show(eHCMSResources.Z1063_G1_GiaoThuocChoBNNayChua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED;
                    OutWardDrugInvoice_UpdateStatus(SelectedOutInvoice);
                }
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1704_G1_PhXuatKgHopLe, eHCMSResources.G0442_G1_TBao);
            }
        }

        public void btnCancel(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0116_G1_Msg_ConfHuyPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //if (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) == 1)

                // Txd 25/05/2014 Replaced ConfigList
                if (Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent == 1)
                {
                    CancalOutwardInvoiceVisitor();
                }
                else
                {
                    if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANTHEOTOA)
                    {
                        CancalOutwardInvoice();
                    }
                    else
                    {
                        CancalOutwardInvoiceVisitor();
                    }
                }
            }
        }

        private void CountMoneyForVisitorPharmacy(long outiID, bool bThuTien)
        {
            isLoadingCount = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginCountMoneyForVisitorPharmacy(outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            decimal AmountPaid = 0;

                            bool value = contract.EndCountMoneyForVisitorPharmacy(out AmountPaid, asyncResult);
                            //goi ham tinh tien
                            Action<ISimplePayPharmacy> onInitDlg = delegate (ISimplePayPharmacy proAlloc)
                            {
                                proAlloc.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                                proAlloc.TotalPayForSelectedItem = SelectedOutInvoice.TotalInvoicePrice.DeepCopy();
                                proAlloc.TotalPaySuggested = SelectedOutInvoice.TotalInvoicePrice;
                            };
                            GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
                        }

                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingCount = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void ShowFormCountMoney()
        {
            Action<ISimplePayPharmacy> onInitDlg = delegate (ISimplePayPharmacy proAlloc)
            {
                proAlloc.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                {
                    proAlloc.TotalPayForSelectedItem = 0;
                    proAlloc.TotalPaySuggested = -AmountPaided;
                }
                else
                {
                    proAlloc.TotalPayForSelectedItem = SelectedOutInvoice.TotalInvoicePrice.DeepCopy();
                    proAlloc.TotalPaySuggested = SelectedOutInvoice.TotalInvoicePrice.DeepCopy() - AmountPaided;
                }
                proAlloc.StartCalculating();
            };
            GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
        }

        private void CancalOutwardInvoiceVisitor()
        {
            isLoadingFullOperator = true;
            SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.CANCELED;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginOutWardDrugInvoiceVisitor_Cancel(SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginOutWardDrugInvoiceVisitor_Cancel_Pst(SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long TransItemID = 0;
                            //var results = contract.EndOutWardDrugInvoiceVisitor_Cancel(out TransItemID, asyncResult);
                            var results = contract.EndOutWardDrugInvoiceVisitor_Cancel_Pst(out TransItemID, asyncResult);
                            if (TransItemID > 0)
                            {
                                Coroutine.BeginExecute(DoGetAmountPaided(SelectedOutInvoice.outiID, SelectedOutInvoice));
                            }
                            OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
                            GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddTransactionVisitor(payment, InvoiceDrug, GetStaffLogin().StaffID, Globals.DeptLocation.DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long PaymentID = 0;
                            bool value = contract.EndAddTransactionVisitor(out PaymentID, asyncResult);
                            if (value)
                            {
                                btnPreview();
                                GetOutWardDrugInvoiceVisitorByID(InvoiceDrug.outiID);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddTransactionHoanTien(payment, InvoiceDrug, GetStaffLogin().StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long PaymentID = 0;
                            bool value = contract.EndAddTransactionHoanTien(out PaymentID, asyncResult);
                            if (value)
                            {
                                GetOutWardDrugInvoiceVisitorByID(InvoiceDrug.outiID);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #region IHandle<PharmacyPayEvent> Members

        public void Handle(PharmacyPayEvent message)
        {
            //thu tien
            if (this.IsActive && message != null)
            {
                if (message.CurPatientPayment != null && message.CurPatientPayment.PayAmount < 0)
                {
                    AddTransactionHoanTien(message.CurPatientPayment, SelectedOutInvoice);
                }
                else
                {
                    AddTransactionVisitor(message.CurPatientPayment, SelectedOutInvoice);
                }
            }
        }

        #endregion

        private void GetRegistrationInfo(long PtRegistrationID)
        {
            isLoadingInfoPatient = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRegistrationInfo(PtRegistrationID, 0, false, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var RegistrationInfo = contract.EndGetRegistrationInfo(asyncResult);

                            //Neu invoice nay chua tra tien thi thoi. Khong thoi tien lai.
                            if (SelectedOutInvoice != null && SelectedOutInvoice.PaidTime == null)
                            {
                                MessageBox.Show("Phiếu xuất này chưa  trả tiền nên không hoàn tiền lại.");
                                return;
                            }
                            //hien thi form tinh tien cho viec huy thuoc
                            var vm = Globals.GetViewModel<ISimplePay>();
                            vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                            vm.Registration = RegistrationInfo;
                            vm.FormMode = PaymentFormMode.PAY;
                            if (RegistrationInfo == null)
                            {
                                MessageBox.Show(eHCMSResources.A0380_G1_Msg_InfoChuaChonDK);
                                return;
                            }

                            vm.RegistrationDetails = null;
                            vm.PclRequests = null;

                            vm.DrugInvoices = new List<OutwardDrugInvoice>() { SelectedOutInvoice };
                            vm.StartCalculating();

                            if (vm.TotalPayForSelectedItem != vm.TotalPaySuggested)
                            {
                                GlobalsNAV.ShowDialog_V3<ISimplePay>(vm);
                            }
                            else
                            {
                                Action<ISimplePay2> onInitDlg = delegate (ISimplePay2 vm2)
                                {
                                    vm2.Registration = RegistrationInfo;
                                    vm2.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                                    vm2.RegistrationDetails = null;
                                    vm2.PclRequests = null;

                                    vm2.DrugInvoices = new List<OutwardDrugInvoice>() { SelectedOutInvoice };
                                    vm2.StartCalculating();
                                };
                                GlobalsNAV.ShowDialog<ISimplePay2>(onInitDlg);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingInfoPatient = false;
                            //Globals.IsBusy = false;

                        }

                    }), null);

                }

            });

            t.Start();

        }

        private void GetRegistrationInfo_InPt(long PtRegistrationID)
        {
            isLoadingGetID = true;

            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            //KMx: Bộ 5 Properties ở dưới phải bằng true hết thì mới lấy PayableSum được (18/09/2014 11:04).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetRegistrationDetails = true;
            LoadRegisSwitch.IsGetPCLRequests = true;
            LoadRegisSwitch.IsGetDrugInvoices = true;
            LoadRegisSwitch.IsGetPatientTransactions = true;
            LoadRegisSwitch.IsGetDrugClinicDeptInvoices = true;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRegistrationInfo_InPt(PtRegistrationID, 1, LoadRegisSwitch, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var RegistrationInfo = contract.EndGetRegistrationInfo_InPt(asyncResult);
                            var vm = Globals.GetViewModel<ISimplePay>();
                            vm.Registration = RegistrationInfo;
                            vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                            vm.FormMode = PaymentFormMode.PAY;
                            if (RegistrationInfo == null)
                            {
                                MessageBox.Show(eHCMSResources.A0380_G1_Msg_InfoChuaChonDK);
                                return;
                            }

                            vm.RegistrationDetails = null;
                            vm.PclRequests = null;
                            if (RegistrationInfo.DrugInvoices == null)
                            {
                                RegistrationInfo.DrugInvoices = new ObservableCollection<OutwardDrugInvoice>();
                            }

                            vm.DrugInvoices = RegistrationInfo.DrugInvoices.Where(item => item.outiID == SelectedOutInvoice.outiID && (item.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
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
                                    vm2.Registration = RegistrationInfo;
                                    vm2.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                                    vm2.RegistrationDetails = null;
                                    vm2.PclRequests = null;
                                    vm2.ObjectState = SelectedOutInvoice.outiID;

                                    vm2.DrugInvoices = RegistrationInfo.DrugInvoices.Where(item => item.outiID == SelectedOutInvoice.outiID && (item.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
                                    vm2.StartCalculating();
                                };
                                GlobalsNAV.ShowDialog<ISimplePay2>(onInitDlg);
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

                    }), null);

                }

            });

            t.Start();

        }

        private void CancalOutwardInvoice()
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new CommonServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginCancelOutwardDrugInvoice(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID,
                    //null, SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginCancelOutwardDrugInvoice_Pst(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID,
                        null, SelectedOutInvoice, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            //var results = contract.EndCancelOutwardDrugInvoice(asyncResult);
                            var results = contract.EndCancelOutwardDrugInvoice_Pst(asyncResult);
                            if (results)
                            {
                                SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.CANCELED;
                            }
                            GetRegistrationInfo(SelectedOutInvoice.SelectedPrescription.PtRegistrationID.GetValueOrDefault());
                            //Globals.ShowMessage("Đã Hủy Phiếu", eHCMSResources.G0442_G1_TBao);
                            //RefeshData();
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
                            if (results != null)
                            {
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                if (SelectedOutInvoice.OutwardDrugs != null)
                                {
                                    OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
                                }
                                SumTotalPrice();
                                //StartCalculatingInvoice();
                            }
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
            if (string.IsNullOrEmpty(SearchCriteria.PatientCode) && string.IsNullOrEmpty(SearchCriteria.CustomerName) && string.IsNullOrEmpty(SearchCriteria.HICardCode) && string.IsNullOrEmpty(SearchCriteria.OutInvID))
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
                                        proAlloc.pageTitle = eHCMSResources.N0192_G1_NhanThuoc_TimPh;
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

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            SearchOutwardInfo(0, 20);
        }

        public void btnSearchAdvance(object sender, RoutedEventArgs e)
        {
            Action<ICollectionDrugSearch> onInitDlg = delegate (ICollectionDrugSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.bFlagStoreHI = bFlagStoreHI;
                proAlloc.bFlagPaidTime = bFlagPaidTime;
                proAlloc.pageTitle = eHCMSResources.N0192_G1_NhanThuoc_TimPh;
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
        private void SumTotalPrice()
        {
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.TotalInvoicePrice = 0;
                if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        SelectedOutInvoice.TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalPrice;
                    }
                }
            }
        }

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;
        }

        public void GridInward_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        public void btnReturn()
        {
            Action<IReturnDrug> onInitDlg = delegate (IReturnDrug proAlloc)
            {
                if (proAlloc.SearchCriteria == null)
                {
                    proAlloc.SearchCriteria = new DataEntities.SearchOutwardInfo();
                }
                proAlloc.SearchCriteria.ID = SelectedOutInvoice.outiID;
                proAlloc.SearchCriteria.OutInvID = SelectedOutInvoice.OutInvID;
                proAlloc.btnSearch();
            };
            GlobalsNAV.ShowDialog<IReturnDrug>(onInitDlg);
        }

        #region printing member
        /*▼====: #001*/
        public void btnPreview()
        {
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = SelectedOutInvoice.outiID;
            // Add condition for won't throw exception in case "Toa khong thuoc"
            if (SelectedOutInvoice != null && (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANTHEOTOA || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE))
            {
                if (Globals.ServerConfigSection.CommonItems.MixedHIPharmacyStores)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Root>");
                    sb.Append("<IDList>");

                    if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
                    {
                        foreach (var item in SelectedOutInvoice.OutwardDrugs.Select(x => x.outiID).Distinct())
                        {
                            sb.AppendFormat("<ID>{0}</ID>", item);
                        }
                    }
                    sb.Append("</IDList>");
                    sb.Append("</Root>");

                    DialogView.PatientID = PatientInfo != null ? PatientInfo.PatientID : 0;
                    DialogView.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC_SUMMARY;
                    DialogView.ListID = sb.ToString();
                }
                else
                {
                    if (SelectedOutInvoice.IsHICount.GetValueOrDefault())
                    {
                        DialogView.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC_BH;
                    }
                    else
                    {
                        DialogView.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC;
                    }
                }
            }
            else
            {
                DialogView.eItem = ReportName.PHARMACY_XUATNOIBO;
            }
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        /*▲====: #001*/

        private void PrintSilient()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetCollectionDrugInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetCollectionDrugInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, "A5");
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

        private void Print_XuatNoiBo()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutwardInternalInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardInternalInPdfFormat(asyncResult);
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

        public void btnPrint()
        {
            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANTHEOTOA || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
            {

                PrintSilient();
            }
            else
            {
                Print_XuatNoiBo();
            }
        }
        #endregion

        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                RefeshData();
            }
        }

        public void GetCurrentInvoiceInfo(OutwardDrugInvoice Current)
        {
            SelectedOutInvoice = Current;
            if (SelectedOutInvoice.PrescriptID != null && SelectedOutInvoice.PrescriptID > 0)
            {
                Visibility = Visibility.Visible;
                LoadPatientInfoInvoice();
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
            GetOutwardDrugDetailsByOutwardInvoice(SelectedOutInvoice.outiID);
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

        public void btnCollectMultiDrug()
        {
            Action<ICollectionMultiDrug> onInitDlg = delegate (ICollectionMultiDrug proAlloc)
            {
                proAlloc.StoreID = StoreID;
            };
            GlobalsNAV.ShowDialog<ICollectionMultiDrug>(onInitDlg);
        }

        public void Handle(PharmacyCloseCollectionMultiEvent message)
        {
            if (message != null)
            {
                if (SelectedOutInvoice != null && SelectedOutInvoice.outiID > 0)
                {
                    RefeshData();
                }
            }
        }
        public void btnHoanTien()
        {
            //if ((Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) == 1) || SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.BANTHEOTOA)

            // Txd 25/05/2014 Replaced ConfigList
            if ((Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent == 1) || SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.BANTHEOTOA)
            {
                LoadAndPayMoneyForService(SelectedOutInvoice, true);
                //ShowFormCountMoney();
            }
            else
            {
                Coroutine.BeginExecute(DoRefund(SelectedOutInvoice.PtRegistrationID.Value, SelectedOutInvoice.outiID));
                GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);
            }
        }
        #region COROUTINES
        private void LoadAndPayMoneyForService(OutwardDrugInvoice Invoice, bool IsDepent = false)
        {
            //long PtRegistrationID = Invoice.PtRegistrationID.Value;
            Coroutine.BeginExecute(DoGetAmountPaided(Invoice.outiID, Invoice, IsDepent));
        }
        public IEnumerator<IResult> DoRefund(long registrationID, long outiID)
        {
            var loadRegInfoTask = new LoadRegistrationInfoTask(registrationID);
            yield return loadRegInfoTask;
            if (loadRegInfoTask.Error == null && loadRegInfoTask.Registration != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
                    {
                        vm.Registration = loadRegInfoTask.Registration;
                        vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                        vm.FormMode = PaymentFormMode.PAY;

                        OutwardDrugInvoice tempInv = null;

                        if (vm.Registration.DrugInvoices != null)
                        {
                            foreach (var inv in vm.Registration.DrugInvoices)
                            {
                                if (inv.outiID == outiID
                                    && inv.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED
                                    && inv.PaidTime != null && inv.RefundTime == null)
                                {
                                    tempInv = inv;
                                    break;
                                }
                            }
                        }

                        if (tempInv != null)
                        {
                            vm.DrugInvoices = new List<OutwardDrugInvoice> { tempInv };
                        }

                        vm.StartCalculating();
                    };
                    GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
                });
            }
            else
            {
                MessageBox.Show("Hãy kiểm tra lại. Không thể lấy thông tin đăng ký để trả tiền lại cho Bệnh nhân");
            }
            yield break;
        }
        #endregion

        private bool IsBenhNhanNoiTru()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null)
            {
                if (SelectedOutInvoice.SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private decimal AmountPaided = 0;
        private IEnumerator<IResult> DoGetAmountPaided(long outiID, OutwardDrugInvoice Invoice, bool IsDepent = false)
        {
            var paymentTypeTask = new CalcMoneyPaidedForDrugInvoiceTask(outiID);
            yield return paymentTypeTask;
            AmountPaided = paymentTypeTask.Amount;
            if (!IsDepent)
            {
                if (SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.SelectedPrescription.PatientID > 0)
                {
                    if (IsBenhNhanNoiTru())
                    {
                        GetRegistrationInfo_InPt(Invoice.PtRegistrationID.Value);
                    }
                    else
                    {
                        GetRegistrationInfo(Invoice.PtRegistrationID.Value);
                    }
                }
                else
                { ShowFormCountMoney(); }
            }
            else
            {
                ShowFormCountMoney();
            }
            yield break;
        }

        public void Handle(PayForRegistrationCompleted message)
        {
            if (message != null && this.IsActive)
            {
                GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);
            }
        }
    }
}
