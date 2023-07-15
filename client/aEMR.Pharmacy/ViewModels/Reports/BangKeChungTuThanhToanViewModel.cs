using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
//using eHCMS.Services.Core;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IBangKeChungTuThanhToan)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BangKeChungTuThanhToanViewModel : Conductor<object>, IBangKeChungTuThanhToan
        , IHandle<PharmacyCloseSearchSupplierEvent>, IHandle<PharmacyCloseSearchSupplierPharmacyPaymentReqEvent>
    {
        public string TitleForm { get; set; }        
        public long V_SupplierType = 7200;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public BangKeChungTuThanhToanViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            Authorization();
            SupplierCriteria = new SupplierSearchCriteria
            {
                V_SupplierType = V_SupplierType
            };

            Suppliers = new PagedSortableCollectionView<Supplier>();
            Suppliers.OnRefresh += Suppliers_OnRefresh;
            Suppliers.PageSize = Globals.PageSize;

            CurrentSupplierPharmacyPaymentReqs = new SupplierPharmacyPaymentReqs
            {
                RequestedDate = Globals.ServerDate.Value,
                SupplierInvDateFrom = Globals.ServerDate.Value.AddDays(-30),
                SupplierInvDateTo = Globals.ServerDate.Value,
                StaffName = GetStaffLogin().FullName,
                StaffID = GetStaffLogin().StaffID
            };

            GetPaymentMode();

            SearchCriteria = new InwardInvoiceSearchCriteria();
            SearchCriteriaOld = new RequestSearchCriteria();
        }

        private void RefeshPhieu()
        {
            CurrentSupplierPharmacyPaymentReqs.RequestedDate = Globals.ServerDate.Value;
            CurrentSupplierPharmacyPaymentReqs.SupplierInvDateFrom = Globals.ServerDate.Value.AddDays(-30);
            CurrentSupplierPharmacyPaymentReqs.SupplierInvDateTo = Globals.ServerDate.Value;
            CurrentSupplierPharmacyPaymentReqs.StaffName = GetStaffLogin().FullName;
            CurrentSupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID = 0;
            CurrentSupplierPharmacyPaymentReqs.SelectedSupplier = null;
            CurrentSupplierPharmacyPaymentReqs.SupplierID = 0;
            CurrentSupplierPharmacyPaymentReqs.V_PaymentMode = 0;
            CurrentSupplierPharmacyPaymentReqs.V_PaymentReqStatus = 0;
            CurrentSupplierPharmacyPaymentReqs.V_PaymentReqStatusName = "";
            CurrentSupplierPharmacyPaymentReqs.SupplierAccountNum = "";
            CurrentSupplierPharmacyPaymentReqs.SupplierBank = "";
            CurrentSupplierPharmacyPaymentReqs.SequenceNum = "";
            CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = null;
            CurrentSupplierPharmacyPaymentReqs.StaffID = GetStaffLogin().StaffID;
        }
        //protected override void OnDeactivate(bool close)
        //{
        //    Globals.EventAggregator.Unsubscribe(this);
        //    //base.OnDeactivate(close);
        //    CurrentSupplierPharmacyPaymentReqs = null;
        //    CurrentInwardDrugInvoice = null;
        //    PaymentModes = null;
        //    SearchCriteria = null;
        //    SearchCriteriaOld = null;
        //    SupplierCriteria = null;
        //    Suppliers = null;
        //}

        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCNhap_BangKeChungTuThanhToan,
                                               (int)oPharmacyEx.mBCNhap_BangKeChungTuThanhToan_Tim, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCNhap_BangKeChungTuThanhToan,
                                               (int)oPharmacyEx.mBCNhap_BangKeChungTuThanhToan_ChinhSua, (int)ePermission.mView);
            bInBangKe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCNhap_BangKeChungTuThanhToan,
                                               (int)oPharmacyEx.mBCNhap_BangKeChungTuThanhToan_InBangKe, (int)ePermission.mView);
            bInDNTT = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCNhap_BangKeChungTuThanhToan,
                                               (int)oPharmacyEx.mBCNhap_BangKeChungTuThanhToan_InDNTT, (int)ePermission.mView);
        }
        #region checking account

        private bool _bTim = true;
        private bool _bChinhSua = true;
        private bool _bInBangKe = true;
        private bool _bInDNTT = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }

        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }

        public bool bInBangKe
        {
            get
            {
                return _bInBangKe;
            }
            set
            {
                if (_bInBangKe == value)
                    return;
                _bInBangKe = value;
            }
        }

        public bool bInDNTT
        {
            get
            {
                return _bInDNTT;
            }
            set
            {
                if (_bInDNTT == value)
                    return;
                _bInDNTT = value;
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

        private PagedSortableCollectionView<Supplier> _Suppliers;
        public PagedSortableCollectionView<Supplier> Suppliers
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
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchSupplierAutoPaging(SupplierCriteria, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ListUnits = contract.EndSearchSupplierAutoPaging(out int totalCount, asyncResult);
                                if (ListUnits != null)
                                {
                                    Suppliers.Clear();
                                    Suppliers.TotalItemCount = totalCount;
                                    foreach (Supplier p in ListUnits)
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
            //var proAlloc = Globals.GetViewModel<ISuppliers>();
            //proAlloc.IsChildWindow = true;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ISuppliers> onInitDlg = (proAlloc) =>
            {
                proAlloc.IsChildWindow = true;
            };
            GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg);
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (CurrentSupplierPharmacyPaymentReqs != null)
            {
                if ((sender as AutoCompleteBox).SelectedItem != null)
                {
                    CurrentSupplierPharmacyPaymentReqs.SelectedSupplier = (sender as AutoCompleteBox).SelectedItem as Supplier;
                    CurrentSupplierPharmacyPaymentReqs.SupplierBank = CurrentSupplierPharmacyPaymentReqs.SelectedSupplier.BankName;
                    CurrentSupplierPharmacyPaymentReqs.SupplierAccountNum = CurrentSupplierPharmacyPaymentReqs.SelectedSupplier.AccountNumber;
                    CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = null;
                    btnOK();
                }
                else
                {
                    CurrentSupplierPharmacyPaymentReqs.SelectedSupplier = null;
                    CurrentSupplierPharmacyPaymentReqs.SupplierBank = "";
                    CurrentSupplierPharmacyPaymentReqs.SupplierAccountNum = "";
                    CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = null;
                }
            }
        }

        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        #region Properties member

        private SupplierPharmacyPaymentReqs CurrentSupplierPharmacyPaymentReqCopy;
        private SupplierPharmacyPaymentReqs _CurrentSupplierPharmacyPaymentReqs;
        public SupplierPharmacyPaymentReqs CurrentSupplierPharmacyPaymentReqs
        {
            get
            {
                return _CurrentSupplierPharmacyPaymentReqs;
            }
            set
            {
                if (_CurrentSupplierPharmacyPaymentReqs != value)
                {
                    _CurrentSupplierPharmacyPaymentReqs = value;
                    NotifyOfPropertyChange(() => CurrentSupplierPharmacyPaymentReqs);
                }
            }
        }

        private InwardDrugInvoice _CurrentInwardDrugInvoice;
        public InwardDrugInvoice CurrentInwardDrugInvoice
        {
            get
            {
                return _CurrentInwardDrugInvoice;
            }
            set
            {
                if (_CurrentInwardDrugInvoice != value)
                {
                    _CurrentInwardDrugInvoice = value;
                    NotifyOfPropertyChange(() => CurrentInwardDrugInvoice);
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
                NotifyOfPropertyChange(() => bCount);
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

        private void GetPaymentMode()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.PAYMENT_MODE, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PaymentModes = contract.EndGetAllLookupValuesByType(asyncResult);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void SupplierPharmacyPaymentReqs_Details()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierPharmacyPaymentReqs_Details(SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSupplierPharmacyPaymentReqs_Details(asyncResult);
                                CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = results.ToObservableCollection();
                                SumPrice();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void SupplierPharmacyPaymentReqs_Save()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierPharmacyPaymentReqs_Save(CurrentSupplierPharmacyPaymentReqs, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                SupplierPharmacyPaymentReqs OutPaymentReqs;
                                var results = contract.EndSupplierPharmacyPaymentReqs_Save(out OutPaymentReqs, asyncResult);
                                CurrentSupplierPharmacyPaymentReqs = OutPaymentReqs;
                                SumPrice();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void SupplierPharmacyPaymentReqs_ID(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierPharmacyPaymentReqs_ID(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentSupplierPharmacyPaymentReqs = contract.EndSupplierPharmacyPaymentReqs_ID(asyncResult);
                                CurrentSupplierPharmacyPaymentReqCopy = CurrentSupplierPharmacyPaymentReqs.DeepCopy();
                                SupplierPharmacyPaymentReqs_DetailsByReqID(ID);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void SupplierPharmacyPaymentReqs_DetailsByReqID(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierPharmacyPaymentReqs_DetailsByReqID(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSupplierPharmacyPaymentReqs_DetailsByReqID(asyncResult);
                                if (CurrentSupplierPharmacyPaymentReqs != null)
                                {
                                    CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = results.ToObservableCollection();
                                }
                                SumPrice();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void FromDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplierPharmacyPaymentReqs.SupplierInvDateFrom != null && CurrentSupplierPharmacyPaymentReqs.SupplierInvDateTo != null)
            {
                if (CurrentSupplierPharmacyPaymentReqCopy == null)
                {
                    CurrentSupplierPharmacyPaymentReqCopy = new SupplierPharmacyPaymentReqs();
                }
                if (CurrentSupplierPharmacyPaymentReqCopy != null && CurrentSupplierPharmacyPaymentReqs.SelectedSupplier != null && CurrentSupplierPharmacyPaymentReqs.SupplierInvDateFrom.GetValueOrDefault().ToString("dd/MM/yyyy") != CurrentSupplierPharmacyPaymentReqCopy.SupplierInvDateFrom.GetValueOrDefault().ToString("dd/MM/yyyy"))
                {
                    btnOK();
                }
            }
            else
            {
                CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = null;
            }
        }

        public void ToDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplierPharmacyPaymentReqs.SupplierInvDateFrom != null && CurrentSupplierPharmacyPaymentReqs.SupplierInvDateTo != null)
            {
                if (CurrentSupplierPharmacyPaymentReqCopy == null)
                {
                    CurrentSupplierPharmacyPaymentReqCopy = new SupplierPharmacyPaymentReqs();
                }
                if (CurrentSupplierPharmacyPaymentReqCopy != null && CurrentSupplierPharmacyPaymentReqs.SelectedSupplier != null && CurrentSupplierPharmacyPaymentReqs.SupplierInvDateFrom.GetValueOrDefault().ToString("dd/MM/yyyy") != CurrentSupplierPharmacyPaymentReqCopy.SupplierInvDateFrom.GetValueOrDefault().ToString("dd/MM/yyyy"))
                {
                    btnOK();
                }
            }
            else
            {
                CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = null;
            }
        }

        public void btnOK()
        {
            if (CurrentSupplierPharmacyPaymentReqs != null && CurrentSupplierPharmacyPaymentReqs.SelectedSupplier != null)
            {
                if (CurrentSupplierPharmacyPaymentReqs.SupplierInvDateFrom != null & CurrentSupplierPharmacyPaymentReqs.SupplierInvDateTo != null)
                {
                    SearchCriteria.SupplierID = CurrentSupplierPharmacyPaymentReqs.SelectedSupplier.SupplierID;
                    SearchCriteria.FromDate = CurrentSupplierPharmacyPaymentReqs.SupplierInvDateFrom;
                    SearchCriteria.ToDate = CurrentSupplierPharmacyPaymentReqs.SupplierInvDateTo;
                    CurrentSupplierPharmacyPaymentReqCopy = CurrentSupplierPharmacyPaymentReqs.DeepCopy();
                    SupplierPharmacyPaymentReqs_Details();
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
            bCount = string.Format("( 0 {0})", eHCMSResources.P0360_G1_Ph);
            SumMoney = 0;
            if (CurrentSupplierPharmacyPaymentReqs != null && CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices != null)
            {
                bCount = "( " + CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices.Count.ToString() + string.Format(" {0} )", eHCMSResources.P0360_G1_Ph);
                for (int i = 0; i < CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices.Count; i++)
                {
                    SumMoney += Math.Round(CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices[i].TotalPrice, 0);
                }
            }
            eHCMS.Services.Core.NumberToLetterConverter converter = new eHCMS.Services.Core.NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (SumMoney < 0)
            {
                temp = 0 - SumMoney;
                prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am);
            }
            else
            {
                temp = SumMoney;
                prefix = "";
            }
            ReadMoney = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le) + string.Format(" {0}", eHCMSResources.Z0872_G1_Dong);
        }

        #region IHandle<PharmacyCloseSearchSupplierEvent> Members

        public void Handle(PharmacyCloseSearchSupplierEvent message)
        {
            if (message != null && IsActive)
            {
                if (CurrentSupplierPharmacyPaymentReqs != null)
                {
                    CurrentSupplierPharmacyPaymentReqs.SelectedSupplier = message.SelectedSupplier as Supplier;
                    CurrentSupplierPharmacyPaymentReqs.SupplierBank = CurrentSupplierPharmacyPaymentReqs.SelectedSupplier.BankName;
                    CurrentSupplierPharmacyPaymentReqs.SupplierAccountNum = CurrentSupplierPharmacyPaymentReqs.SelectedSupplier.AccountNumber;
                    CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = null;
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
                SupplierPharmacyPaymentReqs_Save();
            }
        }

        private bool CheckDataBeforeSave()
        {
            string StrError = "";
            if (CurrentSupplierPharmacyPaymentReqs.SupplierInvDateFrom == null)
            {
                StrError += "Bạn chưa nhập từ ngày." + Environment.NewLine;
            }
            if (CurrentSupplierPharmacyPaymentReqs.SupplierInvDateTo == null)
            {
                StrError += "Bạn chưa nhập đến ngày." + Environment.NewLine;
            }
            if (CurrentSupplierPharmacyPaymentReqs.SelectedSupplier == null)
            {
                StrError += eHCMSResources.Z1292_G1_ChuaChonNCC + Environment.NewLine;
            }
            if (CurrentSupplierPharmacyPaymentReqs.V_PaymentMode == 0)
            {
                StrError += eHCMSResources.Z1293_G1_ChuaChonHinhThucTToan + Environment.NewLine;
            }
            if (CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices == null || CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices.Count == 0)
            {
                StrError += eHCMSResources.Z0926_G1_KgCoHDonTToan + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(StrError))
            {
                Globals.ShowMessage(StrError, eHCMSResources.T0074_G1_I);
                return false;
            }
            return true;
        }

        public void btnDelete()
        {
            SupplierPharmacyPaymentReqs_Delete(CurrentSupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID);
        }

        private void SupplierPharmacyPaymentReqs_Delete(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierPharmacyPaymentReqs_Delete(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndSupplierPharmacyPaymentReqs_Delete(asyncResult);
                                Globals.ShowMessage(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao);
                                RefeshPhieu();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSupplierPharmacyPaymentReqs != null && CurrentSupplierPharmacyPaymentReqs.CanSave)
            {
                CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices.Remove(CurrentInwardDrugInvoice);
                CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices = CurrentSupplierPharmacyPaymentReqs.InwardDrugInvoices.ToObservableCollection();
                SumPrice();
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0920_G1_PhKgDuocXoaSua, eHCMSResources.G0442_G1_TBao);
            }
        }

        public void HyperlinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0126_G1_Msg_ConfChuyenThanhPhChoDuyet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SupplierPharmacyPaymentReqs_UpdateStatus(CurrentSupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID, (long)AllLookupValues.V_PaymentReqStatus.WAITING_APPROVED, null);
            }
        }

        public void hblDaDuyet(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0130_G1_Msg_ConfPhDaDuyet, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SupplierPharmacyPaymentReqs_UpdateStatus(CurrentSupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID, (long)AllLookupValues.V_PaymentReqStatus.APPROVED, GetStaffLogin().StaffID);
            }
        }

        private void SupplierPharmacyPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierPharmacyPaymentReqs_UpdateStatus(ID, V_PaymentReqStatus, StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                             try
                             {
                                 contract.EndSupplierPharmacyPaymentReqs_UpdateStatus(asyncResult);
                                 Globals.ShowMessage(eHCMSResources.A0282_G1_Msg_InfoCNhatStatusOK, eHCMSResources.G0442_G1_TBao);
                                 SupplierPharmacyPaymentReqs_ID(ID);
                                // SupplierPharmacyPaymentReqs_DetailsByReqID(ID);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                if (SearchCriteriaOld != null)
                {
                    SearchCriteriaOld.Code = (sender as TextBox).Text;
                }
                SearchSupplierPharmacyPaymentReqs(0, Globals.PageSize);
            }
        }

        public void btnSearch()
        {
            SearchSupplierPharmacyPaymentReqs(0, Globals.PageSize);
        }

        private void SearchSupplierPharmacyPaymentReqs(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierPharmacyPaymentReqs_Search(SearchCriteriaOld, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSupplierPharmacyPaymentReqs_Search(out int Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        IBangKeChungTuThanhToanSearch DialogView = Globals.GetViewModel<IBangKeChungTuThanhToanSearch>();
                                        DialogView.SearchCriteria = SearchCriteriaOld.DeepCopy();
                                        DialogView.SupplierPharmacyPaymentReqList.Clear();
                                        DialogView.SupplierPharmacyPaymentReqList.TotalItemCount = Total;
                                        DialogView.SupplierPharmacyPaymentReqList.PageIndex = 0;
                                        foreach (SupplierPharmacyPaymentReqs p in results)
                                        {
                                            DialogView.SupplierPharmacyPaymentReqList.Add(p);
                                        }
                                        GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
                                    }
                                    else
                                    {
                                        CurrentSupplierPharmacyPaymentReqs = results.FirstOrDefault();
                                        CurrentSupplierPharmacyPaymentReqCopy = CurrentSupplierPharmacyPaymentReqs.DeepCopy();
                                        SupplierPharmacyPaymentReqs_DetailsByReqID(CurrentSupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID);
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
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        #region IHandle<PharmacyCloseSearchSupplierPharmacyPaymentReqEvent> Members

        public void Handle(PharmacyCloseSearchSupplierPharmacyPaymentReqEvent message)
        {
            if (message != null && IsActive)
            {
                CurrentSupplierPharmacyPaymentReqs = message.SelectedPaymentReq as SupplierPharmacyPaymentReqs;
                CurrentSupplierPharmacyPaymentReqCopy = CurrentSupplierPharmacyPaymentReqs.DeepCopy();
                SupplierPharmacyPaymentReqs_DetailsByReqID(CurrentSupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = CurrentSupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID;
            DialogView.eItem = ReportName.PHARMACY_BANGKECHUNGTUTHANHTOAN;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }


        public void btnPreviewRequest()
        {
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = CurrentSupplierPharmacyPaymentReqs.PharmacySupplierPaymentReqID;
            DialogView.eItem = ReportName.PHARMACY_PHIEUDENGHITHANHTOAN;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        #endregion
    }
}
