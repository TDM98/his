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
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptBangKeChungTuThanhToan)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BangKeChungTuThanhToanViewModel : Conductor<object>, IDrugDeptBangKeChungTuThanhToan
        , IHandle<DrugDeptCloseSearchSupplierEvent>, IHandle<DrugDeptCloseSearchSupplierDrugDeptPaymentReqEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        #region Indicator Member
        private bool _isLoadingSupplierPayment = false;
        public bool isLoadingSupplierPayment
        {
            get { return _isLoadingSupplierPayment; }
            set
            {
                if (_isLoadingSupplierPayment != value)
                {
                    _isLoadingSupplierPayment = value;
                    NotifyOfPropertyChange(() => isLoadingSupplierPayment);
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

     
      

        private bool _isLoadingPaymentMode = false;
        public bool isLoadingPaymentMode
        {
            get { return _isLoadingPaymentMode; }
            set
            {
                if (_isLoadingPaymentMode != value)
                {
                    _isLoadingPaymentMode = value;
                    NotifyOfPropertyChange(() => isLoadingPaymentMode);
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

        private bool _isLoadingDetailID = false;
        public bool isLoadingDetailID
        {
            get { return _isLoadingDetailID; }
            set
            {
                if (_isLoadingDetailID != value)
                {
                    _isLoadingDetailID = value;
                    NotifyOfPropertyChange(() => isLoadingDetailID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingID = false;
        public bool isLoadingID
        {
            get { return _isLoadingID; }
            set
            {
                if (_isLoadingID != value)
                {
                    _isLoadingID = value;
                    NotifyOfPropertyChange(() => isLoadingID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public bool IsLoading
        {
            get { return (isLoadingSupplierPayment || isLoadingFullOperator  || isLoadingSearch || isLoadingDetailID || isLoadingID); }
        }

        #endregion

        public long V_SupplierType = 7200;

        [ImportingConstructor]
        public BangKeChungTuThanhToanViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            authorization();
            SupplierCriteria = new SupplierSearchCriteria();
            SupplierCriteria.V_SupplierType = V_SupplierType;

            Suppliers = new PagedSortableCollectionView<DrugDeptSupplier>();
            Suppliers.OnRefresh += Suppliers_OnRefresh;
            Suppliers.PageSize = Globals.PageSize;

            CurrentSupplierDrugDeptPaymentReqs = new SupplierDrugDeptPaymentReqs();
            CurrentSupplierDrugDeptPaymentReqs.RequestedDate = DateTime.Now;
            CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom = DateTime.Now.AddDays(-30);
            CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateTo = DateTime.Now;
            CurrentSupplierDrugDeptPaymentReqs.StaffName = GetStaffLogin().FullName;
            CurrentSupplierDrugDeptPaymentReqs.StaffID = GetStaffLogin().StaffID;

            Coroutine.BeginExecute(GetPaymentMode());

            SearchCriteria = new InwardInvoiceSearchCriteria();
            SearchCriteriaOld = new RequestSearchCriteria();
        }

        private void RefeshPhieu()
        {
            CurrentSupplierDrugDeptPaymentReqs.RequestedDate = DateTime.Now;
            //KMx: Khi user bấm phiếu mới thì không set lại SupplierInvDateFrom, SupplierInvDateTo. Vì 1 lần làm thanh toán cho rất nhiều NCC, mắc công user chọn lại (Thùy Khoa Dược yêu cầu) (22/10/2015 15:19).
            //CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom = DateTime.Now.AddDays(-30);
            //CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateTo = DateTime.Now;
            CurrentSupplierDrugDeptPaymentReqs.StaffName = GetStaffLogin().FullName;
            CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID = 0;
            CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier = null;
            CurrentSupplierDrugDeptPaymentReqs.SupplierID = 0;
            CurrentSupplierDrugDeptPaymentReqs.V_PaymentMode = 0;
            CurrentSupplierDrugDeptPaymentReqs.V_PaymentReqStatus = 0;
            CurrentSupplierDrugDeptPaymentReqs.V_PaymentReqStatusName = "";
            CurrentSupplierDrugDeptPaymentReqs.SupplierAccountNum = "";
            CurrentSupplierDrugDeptPaymentReqs.SupplierBank = "";
            CurrentSupplierDrugDeptPaymentReqs.SequenceNum = "";
            CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = null;
            CurrentSupplierDrugDeptPaymentReqs.StaffID = GetStaffLogin().StaffID;
            InvoiceByReqID = null;
        }

        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }
        public void authorization()
        {
        }
        #region checking account

        private bool _mTim = true;
        private bool _mThemMoi = true;
        private bool _mChinhSua = true;
        private bool _mXemInBK = true;
        private bool _mXemInPDNTT = true;

        
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
        public bool mChinhSua
        {
          get
          {
            return _mChinhSua;
          }
          set
          {
            if (_mChinhSua == value)
              return;
            _mChinhSua = value;
            NotifyOfPropertyChange(() => mChinhSua);
          }
        }
        public bool mXemInBK
        {
          get
          {
            return _mXemInBK;
          }
          set
          {
            if (_mXemInBK == value)
              return;
            _mXemInBK = value;
            NotifyOfPropertyChange(() => mXemInBK);
          }
        }
        public bool mXemInPDNTT
        {
          get
          {
            return _mXemInPDNTT;
          }
          set
          {
            if (_mXemInPDNTT == value)
              return;
            _mXemInPDNTT = value;
            NotifyOfPropertyChange(() => mXemInPDNTT);
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
        #region Auto for Supplier

        private SupplierSearchCriteria _SupplierCriteria;
        public SupplierSearchCriteria SupplierCriteria
        {
            get
            {
                return _SupplierCriteria;
            }
            set
            {
                _SupplierCriteria = value;
                NotifyOfPropertyChange(() => SupplierCriteria);
            }
        }

        private PagedSortableCollectionView<DrugDeptSupplier> _Suppliers;
        public PagedSortableCollectionView<DrugDeptSupplier> Suppliers
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

        private void SearchSupplierAuto(int PageIndex, int PageSize)
        {
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptSupplier_SearchAutoPaging(SupplierCriteria, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListUnits = contract.EndDrugDeptSupplier_SearchAutoPaging(out totalCount, asyncResult);
                            if (ListUnits != null)
                            {
                                Suppliers.Clear();
                                Suppliers.TotalItemCount = totalCount;
                                foreach (DrugDeptSupplier p in ListUnits)
                                {
                                    Suppliers.Add(p);
                                }
                                NotifyOfPropertyChange(() => Suppliers);
                            }
                            auSupplier.ItemsSource = Suppliers;
                            auSupplier.PopulateComplete();
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

        AutoCompleteBox auSupplier;
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            auSupplier = sender as AutoCompleteBox;
            SupplierCriteria.SupplierName = e.Parameter;
            Suppliers.PageIndex = 0;
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            //var proAlloc = Globals.GetViewModel<ISupplierProduct>();
            //proAlloc.IsChildWindow = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            void onInitDlg(ISupplierProduct proAlloc)
            {
                proAlloc.IsChildWindow = true;
            }
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (CurrentSupplierDrugDeptPaymentReqs != null)
            {
                if ((sender as AutoCompleteBox).SelectedItem != null)
                {
                    CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier = (sender as AutoCompleteBox).SelectedItem as DrugDeptSupplier;
                    CurrentSupplierDrugDeptPaymentReqs.SupplierBank = CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier.BankName;
                    CurrentSupplierDrugDeptPaymentReqs.SupplierAccountNum = CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier.AccountNumber;
                    CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = null;
                    btnOK();
                }
                else
                {
                    CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier = null;
                    CurrentSupplierDrugDeptPaymentReqs.SupplierBank = "";
                    CurrentSupplierDrugDeptPaymentReqs.SupplierAccountNum = "";
                    CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = null;
                }
            }
        }


        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        #region Properties member
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

        private SupplierDrugDeptPaymentReqs CurrentSupplierDrugDeptPaymentReqCopy;

        private SupplierDrugDeptPaymentReqs _CurrentSupplierDrugDeptPaymentReqs;
        public SupplierDrugDeptPaymentReqs CurrentSupplierDrugDeptPaymentReqs
        {
            get
            {
                return _CurrentSupplierDrugDeptPaymentReqs;
            }
            set
            {
                if (_CurrentSupplierDrugDeptPaymentReqs != value)
                {
                    _CurrentSupplierDrugDeptPaymentReqs = value;
                    NotifyOfPropertyChange(() => CurrentSupplierDrugDeptPaymentReqs);
                }

            }
        }

        private InwardDrugMedDeptInvoice _CurrentInwardDrugMedDeptInvoice;
        public InwardDrugMedDeptInvoice CurrentInwardDrugMedDeptInvoice
        {
            get
            {
                return _CurrentInwardDrugMedDeptInvoice;
            }
            set
            {
                if (_CurrentInwardDrugMedDeptInvoice != value)
                {
                    _CurrentInwardDrugMedDeptInvoice = value;
                    NotifyOfPropertyChange(() => CurrentInwardDrugMedDeptInvoice);
                }

            }
        }

        IList<Lookup> _PaymentModes;
        public IList<Lookup> PaymentModes
        {
            get
            {
                return _PaymentModes;
            }
            set
            {
                if (_PaymentModes != value)
                {
                    _PaymentModes = value;
                    NotifyOfPropertyChange(() => PaymentModes);
                }
            }
        }

        private InwardInvoiceSearchCriteria _searchCriteria;
        public InwardInvoiceSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private RequestSearchCriteria _SearchCriteriaOld;
        public RequestSearchCriteria SearchCriteriaOld
        {
            get
            {
                return _SearchCriteriaOld;
            }
            set
            {
                _SearchCriteriaOld = value;
                NotifyOfPropertyChange(() => SearchCriteriaOld);
            }
        }

        private string _bCount;
        public string bCount
        {
            get
            {
                return _bCount;
            }
            set
            {
                _bCount = value;
                NotifyOfPropertyChange(()=>bCount);
            }
        }

        private decimal _SumMoney;
        public decimal SumMoney
        {
            get
            {
                return _SumMoney;
            }
            set
            {
                _SumMoney = value;
                NotifyOfPropertyChange(() => SumMoney);
            }
        }

        private string _ReadMoney;
        public string ReadMoney
        {
            get
            {
                return _ReadMoney;
            }
            set
            {
                _ReadMoney = value;
                NotifyOfPropertyChange(() => ReadMoney);
            }
        }

        #endregion

        private IEnumerator<IResult> GetPaymentMode()
        {
            isLoadingPaymentMode = true;
            var paymentTypeTask = new LoadLookupListTask(LookupValues.PAYMENT_MODE, false, false);
            yield return paymentTypeTask;
            PaymentModes = paymentTypeTask.LookupList;
            isLoadingPaymentMode = false;
            yield break;
        }

        private void SupplierDrugDeptPaymentReqs_Details()
        {
            isLoadingSupplierPayment = true;
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierDrugDeptPaymentReqs_Details(SearchCriteria,V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSupplierDrugDeptPaymentReqs_Details(asyncResult);
                          
                            CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = results.ToObservableCollection();
                            if (CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices == null)
                            {
                                CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = new ObservableCollection<InwardDrugMedDeptInvoice>();
                            }
                            if ( InvoiceByReqID != null)
                            {
                                foreach (InwardDrugMedDeptInvoice p in InvoiceByReqID)
                                {
                                    if (p.SupplierID == CurrentSupplierDrugDeptPaymentReqs.SupplierID)
                                    {
                                        CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices.Add(p);
                                    }
                                }
                            }

                            SumPrice();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSupplierPayment = false;
                           // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SupplierDrugDeptPaymentReqs_Save()
        {
            isLoadingFullOperator = true;
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierDrugDeptPaymentReqs_Save(CurrentSupplierDrugDeptPaymentReqs, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SupplierDrugDeptPaymentReqs OutPaymentReqs;
                            var results = contract.EndSupplierDrugDeptPaymentReqs_Save(out OutPaymentReqs,asyncResult);
                            CurrentSupplierDrugDeptPaymentReqs = OutPaymentReqs;
                            try
                            {
                                InvoiceByReqID = OutPaymentReqs.InwardDrugMedDeptInvoices.DeepCopy();
                            }
                            catch
                            {
                                InvoiceByReqID = null;
                            }
                            SumPrice();
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

        private void SupplierDrugDeptPaymentReqs_ID(long ID)
        {
            isLoadingID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierDrugDeptPaymentReqs_ID(ID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            CurrentSupplierDrugDeptPaymentReqs = contract.EndSupplierDrugDeptPaymentReqs_ID(asyncResult);
                            CurrentSupplierDrugDeptPaymentReqCopy = CurrentSupplierDrugDeptPaymentReqs.DeepCopy();
                            SupplierDrugDeptPaymentReqs_DetailsByReqID(ID);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private ObservableCollection<InwardDrugMedDeptInvoice> InvoiceByReqID = null;

        private void SupplierDrugDeptPaymentReqs_DetailsByReqID(long ID)
        {
            isLoadingDetailID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierDrugDeptPaymentReqs_DetailsByReqID(ID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var results = contract.EndSupplierDrugDeptPaymentReqs_DetailsByReqID(asyncResult);
                            InvoiceByReqID = results.ToObservableCollection().DeepCopy();
                            if (CurrentSupplierDrugDeptPaymentReqs != null)
                            {
                                CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = results.ToObservableCollection();
                            }
                            SumPrice();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetailID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void FromDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom != null && CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateTo != null)
            {
                if (CurrentSupplierDrugDeptPaymentReqCopy == null)
                {
                    CurrentSupplierDrugDeptPaymentReqCopy = new SupplierDrugDeptPaymentReqs();
                }
                if (CurrentSupplierDrugDeptPaymentReqCopy != null && CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier != null && CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom.GetValueOrDefault().ToString("dd/MM/yyyy") != CurrentSupplierDrugDeptPaymentReqCopy.SupplierInvDateFrom.GetValueOrDefault().ToString("dd/MM/yyyy"))
                {
                    btnOK();
                }
            }
            else
            {
                CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = null;
            }
        }

        public void ToDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom != null && CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateTo != null)
            {
                if (CurrentSupplierDrugDeptPaymentReqCopy == null)
                {
                    CurrentSupplierDrugDeptPaymentReqCopy = new SupplierDrugDeptPaymentReqs();
                }
                if (CurrentSupplierDrugDeptPaymentReqCopy != null && CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier != null && CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom.GetValueOrDefault().ToString("dd/MM/yyyy") !=CurrentSupplierDrugDeptPaymentReqCopy.SupplierInvDateFrom.GetValueOrDefault().ToString("dd/MM/yyyy"))
                {
                    btnOK();
                }
            }
            else
            {
                CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = null;
            }
        }

        public void btnOK()
        {
            if (CurrentSupplierDrugDeptPaymentReqs != null && CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier != null)
            {
                if (CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom != null & CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateTo != null)
                {
                    SearchCriteria.SupplierID = CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier.SupplierID;
                    SearchCriteria.FromDate = CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom;
                    SearchCriteria.ToDate = CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateTo;
                    CurrentSupplierDrugDeptPaymentReqCopy = CurrentSupplierDrugDeptPaymentReqs.DeepCopy();
                    SupplierDrugDeptPaymentReqs_Details();
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.A0885_G1_Msg_InfoNhapTuNgDenNg2, eHCMSResources.G0442_G1_TBao);
                }
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0923_G1_ChonNCCTToan, eHCMSResources.G0442_G1_TBao);
            }
        }

        private void SumPrice()
        {
            bCount = string.Format("({0} {1})", 0, eHCMSResources.P0360_G1_Ph); 
            SumMoney = 0;
            if (CurrentSupplierDrugDeptPaymentReqs != null && CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices != null)
            {
                bCount = string.Format("({0} {1})", CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices.Count.ToString(), eHCMSResources.P0360_G1_Ph);
                for (int i = 0; i < CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices.Count; i++)
                {
                    SumMoney += CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices[i].TotalPrice;
                }
            }
            eHCMS.Services.Core.NumberToLetterConverter converter = new eHCMS.Services.Core.NumberToLetterConverter();
            ReadMoney = converter.Convert(SumMoney.ToString(), ',', eHCMSResources.Z0871_G1_Le) + string.Format(" {0}", eHCMSResources.Z0872_G1_Dong);
        }

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                if (CurrentSupplierDrugDeptPaymentReqs != null)
                {
                    CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
                    CurrentSupplierDrugDeptPaymentReqs.SupplierBank = CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier.BankName;
                    CurrentSupplierDrugDeptPaymentReqs.SupplierAccountNum = CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier.AccountNumber;
                    CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = null;
                    btnOK();
                }
            }
        }

        #endregion

        public void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void btnSave()
        {
            if (CheckDataBeforeSave())
            {
                CurrentSupplierDrugDeptPaymentReqs.V_MedProductType = V_MedProductType;
                SupplierDrugDeptPaymentReqs_Save();
            }
        }

        private bool CheckDataBeforeSave()
        {
            string StrError = "";
            if (CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateFrom == null)
            {
                StrError += string.Format("{0}.", eHCMSResources.A0883_G1_Msg_InfoNhapTuNg) + Environment.NewLine;
            }
            if (CurrentSupplierDrugDeptPaymentReqs.SupplierInvDateTo == null)
            {
                StrError += string.Format("{0}.", eHCMSResources.A0867_G1_Msg_InfoNhapDenNg) + Environment.NewLine;
            }
            if (CurrentSupplierDrugDeptPaymentReqs.SelectedSupplier == null)
            {
                StrError += eHCMSResources.Z1292_G1_ChuaChonNCC + Environment.NewLine;
            }
            if (CurrentSupplierDrugDeptPaymentReqs.V_PaymentMode == 0)
            {
                StrError += eHCMSResources.Z1293_G1_ChuaChonHinhThucTToan + Environment.NewLine;
            }
            if (CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices == null || CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices.Count==0)
            {
                StrError += string.Format("{0}.", eHCMSResources.Z0926_G1_KgCoHDonTToan) + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(StrError))
            {
                Globals.ShowMessage(StrError,eHCMSResources.T0074_G1_I);
                return false;
            }
            return true;
        }

        public void btnDelete()
        {
            SupplierDrugDeptPaymentReqs_Delete(CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID);
        }

        private void SupplierDrugDeptPaymentReqs_Delete(long ID)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierDrugDeptPaymentReqs_Delete(ID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndSupplierDrugDeptPaymentReqs_Delete(asyncResult);
                            Globals.ShowMessage(eHCMSResources.K2825_G1_DaXoa, eHCMSResources.G0442_G1_TBao);
                            RefeshPhieu();
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
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplierDrugDeptPaymentReqs != null && CurrentSupplierDrugDeptPaymentReqs.CanSave)
            {
                CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices.Remove(CurrentInwardDrugMedDeptInvoice);
                CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices = CurrentSupplierDrugDeptPaymentReqs.InwardDrugMedDeptInvoices.ToObservableCollection();
                SumPrice();
            }
            else
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0920_G1_PhKgDuocXoaSua),eHCMSResources.G0442_G1_TBao);
            }
        }

        public void HyperlinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0126_G1_Msg_ConfChuyenThanhPhChoDuyet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SupplierDrugDeptPaymentReqs_UpdateStatus(CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID, (long)AllLookupValues.V_PaymentReqStatus.WAITING_APPROVED,null);
            }

        }
        public void hblDaDuyet(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0130_G1_Msg_ConfPhDaDuyet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SupplierDrugDeptPaymentReqs_UpdateStatus(CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID, (long)AllLookupValues.V_PaymentReqStatus.APPROVED,GetStaffLogin().StaffID);
            }

        }
        private void SupplierDrugDeptPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus,long? StaffID)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierDrugDeptPaymentReqs_UpdateStatus(ID, V_PaymentReqStatus,StaffID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndSupplierDrugDeptPaymentReqs_UpdateStatus(asyncResult);
                            Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0826_G1_CNhatTThaiThCong), eHCMSResources.G0442_G1_TBao);
                            SupplierDrugDeptPaymentReqs_ID(ID);
                           // SupplierDrugDeptPaymentReqs_DetailsByReqID(ID);
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

        public void btnNew()
        {
            RefeshPhieu();
        }

        public void tbx_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(SearchCriteriaOld !=null)
                {
                    SearchCriteriaOld.Code = (sender as TextBox).Text;
                }
                SearchSupplierDrugDeptPaymentReqs(0, Globals.PageSize);
            }
        }

        public void btnSearch()
        {
            SearchSupplierDrugDeptPaymentReqs(0,Globals.PageSize);
        }

        private void SearchSupplierDrugDeptPaymentReqs(int PageIndex, int PageSize)
        {
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierDrugDeptPaymentReqs_Search(SearchCriteriaOld,V_MedProductType, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndSupplierDrugDeptPaymentReqs_Search(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    //var proAlloc = Globals.GetViewModel<IDrugDeptBangKeChungTuThanhToanSearch>();
                                    //proAlloc.SearchCriteria = SearchCriteriaOld.DeepCopy();
                                    //proAlloc.V_MedProductType = V_MedProductType;
                                    //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                    //{
                                    //    proAlloc.strHienThi = eHCMSResources.Z0930_G1_TimBKeCTuTToanThuoc.ToUpper();
                                    //}
                                    //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                                    //{
                                    //    proAlloc.strHienThi = eHCMSResources.Z0931_G1_TimBKeCTuTToanYCu.ToUpper();
                                    //}
                                    //else
                                    //{
                                    //    proAlloc.strHienThi = eHCMSResources.Z0932_G1_TimBKeCTuTToanHChat.ToUpper();
                                    //}
                                    
                                    //proAlloc.SupplierDrugDeptPaymentReqList.Clear();
                                    //proAlloc.SupplierDrugDeptPaymentReqList.TotalItemCount = Total;
                                    //proAlloc.SupplierDrugDeptPaymentReqList.PageIndex = 0;
                                    //foreach (SupplierDrugDeptPaymentReqs p in results)
                                    //{
                                    //    proAlloc.SupplierDrugDeptPaymentReqList.Add(p);
                                    //}
                                    //var instance = proAlloc as Conductor<object>;
                                    //Globals.ShowDialog(instance, (o) => { });

                                    Action<IDrugDeptBangKeChungTuThanhToanSearch> onInitDlg = (proAlloc) =>
                                    {
                                        proAlloc.SearchCriteria = SearchCriteriaOld.DeepCopy();
                                        proAlloc.V_MedProductType = V_MedProductType;
                                        if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                                        {
                                            proAlloc.strHienThi = eHCMSResources.Z0930_G1_TimBKeCTuTToanThuoc.ToUpper();
                                        }
                                        else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                                        {
                                            proAlloc.strHienThi = eHCMSResources.Z0931_G1_TimBKeCTuTToanYCu.ToUpper();
                                        }
                                        else
                                        {
                                            proAlloc.strHienThi = eHCMSResources.Z0932_G1_TimBKeCTuTToanHChat.ToUpper();
                                        }

                                        proAlloc.SupplierDrugDeptPaymentReqList.Clear();
                                        proAlloc.SupplierDrugDeptPaymentReqList.TotalItemCount = Total;
                                        proAlloc.SupplierDrugDeptPaymentReqList.PageIndex = 0;
                                        foreach (SupplierDrugDeptPaymentReqs p in results)
                                        {
                                            proAlloc.SupplierDrugDeptPaymentReqList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IDrugDeptBangKeChungTuThanhToanSearch>(onInitDlg);
                                }
                                else
                                {
                                    CurrentSupplierDrugDeptPaymentReqs = results.FirstOrDefault();
                                    CurrentSupplierDrugDeptPaymentReqCopy = CurrentSupplierDrugDeptPaymentReqs.DeepCopy();
                                    SupplierDrugDeptPaymentReqs_DetailsByReqID(CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0863_G1_KgTimThay,eHCMSResources.G0442_G1_TBao);
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

        #region IHandle<DrugDeptCloseSearchSupplierDrugDeptPaymentReqEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierDrugDeptPaymentReqEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentSupplierDrugDeptPaymentReqs = message.SelectedPaymentReq as SupplierDrugDeptPaymentReqs;
                CurrentSupplierDrugDeptPaymentReqCopy = CurrentSupplierDrugDeptPaymentReqs.DeepCopy();
                SupplierDrugDeptPaymentReqs_DetailsByReqID(CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            //var proAlloc = Globals.GetViewModel<IDrugDeptReportDocumentPreview>();
            //proAlloc.ID = CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID;
            //proAlloc.eItem = ReportName.DRUGDEPT_BANGKECHUNGTUTHANHTOAN;
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0778_G1_BKeCTuTToanThuoc.ToUpper();
            //}
            //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0779_G1_BKeCTuTToanYCu.ToUpper();
            //}
            //else
            //{
            //    proAlloc.LyDo = string.Format("{0} ", eHCMSResources.Z0780_G1_BKeCTuTToanHChat.ToUpper());
            //}

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID;
                proAlloc.eItem = ReportName.DRUGDEPT_BANGKECHUNGTUTHANHTOAN;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z0778_G1_BKeCTuTToanThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z0779_G1_BKeCTuTToanYCu.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = string.Format("{0} ", eHCMSResources.Z0780_G1_BKeCTuTToanHChat.ToUpper());
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreviewRequest()
        {
            //var proAlloc = Globals.GetViewModel<IDrugDeptReportDocumentPreview>();
            //proAlloc.ID = CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID;
            //proAlloc.eItem = ReportName.DRUGDEPT_PHIEUDENGHITHANHTOAN;
            //if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0933_G1_PhDNghiTToanThuoc.ToUpper();
            //}
            //else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0934_G1_PhDNghiTToanYCu.ToUpper();
            //}
            //else
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0935_G1_PhDNghiTToanHChat.ToUpper();
            //}

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = CurrentSupplierDrugDeptPaymentReqs.DrugDeptSupplierPaymentReqID;
                proAlloc.eItem = ReportName.DRUGDEPT_PHIEUDENGHITHANHTOAN;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z0933_G1_PhDNghiTToanThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z0934_G1_PhDNghiTToanYCu.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z0935_G1_PhDNghiTToanHChat.ToUpper();
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }


        #endregion
    }
}
