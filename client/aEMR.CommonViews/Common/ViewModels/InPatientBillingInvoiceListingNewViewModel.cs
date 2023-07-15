using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
/*
 * 20180829 #001 TTM: 
 * 20191102 #002 TNHX:  BM 0017411: Add new func ReCalBillingInvoices with choose MedPrice & PCLPrice, Check if IsValidBill = 2 => using new func + Refactor code
 * 20200204 #003 TTM:   BM 0019630: Fix tự động tích tính BH cho bill khi tính lại. Chỉ tích khi được người sử dụng đồng ý và bill phải thuộc trường hợp: Bill không có bảo hiểm chi trả nhưng trong bill lại có dịch vụ thuộc danh mục bảo hiểm.
 * 20200704 #004 TTM:   BM 0039324: Gỡ điều kiện kiểm tra nếu thay đổi quyền lợi thì bắt buộc tính bằng tính lại bill bảng giá.
 * 20210929 #005 TNHX: Bỏ hỏi tự động tích BH => không tự động tích BH khi tính lại bill
 * 20220802 #006 BLQ: Tự động tích bảo hiểm khi tính lại bill
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientBillingInvoiceListingNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientBillingInvoiceListingNewViewModel : Conductor<object>, IInPatientBillingInvoiceListingNew
        , IHandle<SelectPriceListForRecalcBillInvoiceEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        IEventAggregator _eventArg;

        [ImportingConstructor]
        public InPatientBillingInvoiceListingNewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _eventArg = eventArg;
            eventArg.Subscribe(this);
            //var detailsVm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
            //InvoiceDetailsContent = detailsVm;
            //InvoiceDetailsContent.CanDelete = false;
            //InvoiceDetailsContent.ShowDeleteColumn = false;
            //if (_showInfoColumn)
            //{
            //    var detailsVm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
            //    InvoiceDetailsContent = detailsVm;
            //    InvoiceDetailsContent.CanDelete = false;
            //    InvoiceDetailsContent.ShowDeleteColumn = false;
            //    InvoiceDetailsContent.ShowHIAppliedColumn = false;
            //}
            //else
            //{
            //    InvoiceDetailsContent = null;
            //}
            Authorization();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
            //CMN: No need to do this, it's child control in all same code
            ////▼====== #003
            //var homevm = Globals.GetViewModel<IHome>();
            //homevm.OutstandingTaskContent = null;
            //homevm.IsExpandOST = false;
            ////▲====== #003
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var currentview = view as IInPatientBillingInvoiceListingNewView;
            if (currentview != null)
            {
                //currentview.ShowEditColumn(_showEditColumn);
                //currentview.ShowInfoColumn(_showInfoColumn);
                //currentview.ShowRecalcHiColumn(_showRecalcHiColumn);

                currentview.ShowColumn("colEdit", _showEditColumn);
                currentview.ShowColumn("colShowDetails", _showInfoColumn);
                currentview.ShowColumn("colRecalcHi", _showRecalcHiColumn);
                currentview.ShowColumn("colRecalcHiWithPriceList", _showRecalcHiWithPriceListColumn);
                currentview.ShowColumn("colCheckItem", _showCheckItemColumn);
                currentview.ShowColumn("colPrintBill", _showPrintBillColumn);
                currentview.ShowColumn("colSupportInfo", _showSupportHpl);
            }
        }

        private bool _mTickSelect = true;
        public bool mTickSelect
        {
            get { return _mTickSelect; }
            set
            {
                if (_mTickSelect != value)
                {
                    _mTickSelect = value;
                    NotifyOfPropertyChange(() => mTickSelect);
                }
            }
        }

        private bool _mExpanderDetail = true;
        public bool mExpanderDetail
        {
            get { return _mExpanderDetail; }
            set
            {
                if (_mExpanderDetail != value)
                {
                    _mExpanderDetail = value;
                    NotifyOfPropertyChange(() => mExpanderDetail);
                }
            }
        }

        private bool _showEditColumn = true;
        public bool ShowEditColumn
        {
            get { return _showEditColumn; }
            set
            {
                if (_showEditColumn != value)
                {
                    _showEditColumn = value;
                    NotifyOfPropertyChange(() => ShowEditColumn);

                    var view = this.GetView() as IInPatientBillingInvoiceListingNewView;
                    if (view != null)
                    {
                        //view.ShowEditColumn(_showEditColumn);
                        view.ShowColumn("colEdit", _showEditColumn);
                    }
                }
            }
        }

        private bool _showHIAppliedColumn = false;
        public bool ShowHIAppliedColumn
        {
            get { return _showHIAppliedColumn; }
            set
            {
                if (_showHIAppliedColumn != value)
                {
                    _showHIAppliedColumn = value;
                    NotifyOfPropertyChange(() => ShowHIAppliedColumn);

                    //if (InvoiceDetailsContent != null)
                    //{
                    //    InvoiceDetailsContent.ShowHIAppliedColumn = ShowHIAppliedColumn;
                    //}
                }
            }
        }

        private bool _showInfoColumn = true;
        public bool ShowInfoColumn
        {
            get { return _showInfoColumn; }
            set
            {
                if (_showInfoColumn != value)
                {
                    _showInfoColumn = value;
                    NotifyOfPropertyChange(() => ShowInfoColumn);

                    //var view = GetView() as IInPatientBillingInvoiceListingView;
                    //if (view != null)
                    //{
                    //    view.ShowInfoColumn(_showInfoColumn);
                    //}
                    //if(_showInfoColumn)
                    //{
                    //    var detailsVm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
                    //    InvoiceDetailsContent = detailsVm;
                    //    InvoiceDetailsContent.CanDelete = false;
                    //    InvoiceDetailsContent.ShowDeleteColumn = false;
                    //}
                    //else
                    //{
                    //    InvoiceDetailsContent = null;
                    //}
                }
            }
        }

        private bool _showRecalcHiColumn = true;
        public bool ShowRecalcHiColumn
        {
            get { return _showRecalcHiColumn; }
            set
            {
                if (_showRecalcHiColumn != value)
                {
                    _showRecalcHiColumn = value;
                    NotifyOfPropertyChange(() => ShowRecalcHiColumn);

                    var view = GetView() as IInPatientBillingInvoiceListingView;
                    if (view != null)
                    {
                        view.ShowRecalcHiColumn(_showRecalcHiColumn);
                    }
                }
            }
        }

        private bool _showRecalcHiWithPriceListColumn = true;
        public bool ShowRecalcHiWithPriceListColumn
        {
            get { return _showRecalcHiWithPriceListColumn; }
            set
            {
                if (_showRecalcHiWithPriceListColumn != value)
                {
                    _showRecalcHiWithPriceListColumn = value;
                    NotifyOfPropertyChange(() => ShowRecalcHiWithPriceListColumn);

                    var view = GetView() as IInPatientBillingInvoiceListingView;
                    if (view != null)
                    {
                        view.ShowRecalcHiWithPriceListColumn(_showRecalcHiWithPriceListColumn);
                    }
                }
            }
        }

        private bool _showCheckItemColumn;
        public bool ShowCheckItemColumn
        {
            get { return _showCheckItemColumn; }
            set
            {
                if (_showCheckItemColumn != value)
                {
                    _showCheckItemColumn = value;
                    NotifyOfPropertyChange(() => ShowCheckItemColumn);
                }
            }
        }

        private bool _showPrintBillColumn;
        public bool ShowPrintBillColumn
        {
            get { return _showPrintBillColumn; }
            set
            {
                if (_showPrintBillColumn != value)
                {
                    _showPrintBillColumn = value;
                    NotifyOfPropertyChange(() => ShowPrintBillColumn);
                }
            }
        }

        private bool _showSupportHpl;
        public bool ShowSupportHpl
        {
            get { return _showSupportHpl; }
            set
            {
                if (_showSupportHpl != value)
                {
                    _showSupportHpl = value;
                    NotifyOfPropertyChange(() => ShowSupportHpl);
                }
            }
        }

        //▼====== #001: Bổ sung HIBenefit để truyền xuống cho InPatientBillingInvoiceDetailsListingViewModel kiểm tra xem bệnh nhân có BHYT hay không
        private double? _HIBenefit;
        public double? HIBenefit
        {
            get { return _HIBenefit; }
            set
            {
                if (_HIBenefit != value)
                {
                    _HIBenefit = value;
                    NotifyOfPropertyChange(() => HIBenefit);
                }
            }
        }

        //▲====== #001
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

        //private IInPatientBillingInvoiceDetailsListing _invoiceDetailsContent;
        //public IInPatientBillingInvoiceDetailsListing InvoiceDetailsContent
        //{
        //    get { return _invoiceDetailsContent; }
        //    set { _invoiceDetailsContent = value; 
        //    NotifyOfPropertyChange(()=>InvoiceDetailsContent);}
        //}

        private ObservableCollection<InPatientBillingInvoice> _billingInvoices;
        public ObservableCollection<InPatientBillingInvoice> BillingInvoices
        {
            get { return _billingInvoices; }
            set
            {
                if (_billingInvoices != value)
                {
                    _billingInvoices = value;

                    SumTotalPrice();

                    NotifyOfPropertyChange(() => BillingInvoices);
                    //if (InvoiceDetailsContent != null)
                    //{
                    //    InvoiceDetailsContent.BillingInvoice = null;
                    //    InvoiceDetailsContent.ResetView(); 
                    //}
                }
            }
        }

        private bool _ReplaceMaxHIPay;

        public bool ReplaceMaxHIPay
        {
            get { return _ReplaceMaxHIPay; }
            set
            {
                _ReplaceMaxHIPay = value;
                NotifyOfPropertyChange(() => ReplaceMaxHIPay);
            }
        }

        //private InPatientBillingInvoice _beingViewedItem;
        //public InPatientBillingInvoice BeingViewedItem
        //{
        //    get { return _beingViewedItem; }
        //    set
        //    {
        //        _beingViewedItem = value;
        //        if (InvoiceDetailsContent != null)
        //        {
        //            InvoiceDetailsContent.BillingInvoice = _beingViewedItem;  
        //        }
        //        NotifyOfPropertyChange(()=>BeingViewedItem);
        //        NotifyOfPropertyChange(() => DetailItemTitle);
        //    }
        //}

        //public string DetailItemTitle
        //{
        //    get
        //    {
        //        if(_beingViewedItem != null)
        //        {
        //            return string.Format("{0} ", eHCMSResources.Z1240_G1_DSDVBill) + _beingViewedItem.BillingInvNum;
        //        }
        //        return string.Empty;
        //    }
        //}

        public bool ShowAll
        {
            get;
            set;
        }

        public string GroupBy
        {
            get;
            set;
        }

        private void AllCheckedfc()
        {
            if (BillingInvoices == null || BillingInvoices.Count < 0)
            {
                return;
            }
            foreach (InPatientBillingInvoice item in BillingInvoices)
            {
                item.IsChecked = true;
            }
        }

        private void UnCheckedfc()
        {
            if (BillingInvoices == null || BillingInvoices.Count < 0)
            {
                return;
            }
            foreach (InPatientBillingInvoice item in BillingInvoices)
            {
                item.IsChecked = false;
            }
        }

        private DataGridRowDetailsVisibilityMode _dataGridRowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;

        public DataGridRowDetailsVisibilityMode DataGridRowDetailsVisibilityMode
        {
            get { return _dataGridRowDetailsVisibilityMode; }
            set
            {
                _dataGridRowDetailsVisibilityMode = value;
                NotifyOfPropertyChange(() => DataGridRowDetailsVisibilityMode);
            }
        }

        public void GetBillingInvoiceDetails(InPatientBillingInvoice inv)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientBillingInvoiceDetails(inv.InPatientBillingInvID, false, false, false,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    List<PatientRegistrationDetail> regDetails;
                                    List<PatientPCLRequest> pclRequestList;
                                    List<OutwardDrugClinicDeptInvoice> allInPatientInvoices;

                                    contract.EndGetInPatientBillingInvoiceDetails(out regDetails, out pclRequestList, out allInPatientInvoices, asyncResult);

                                    var tempInv = asyncResult.AsyncState as InPatientBillingInvoice;
                                    if (tempInv != null)
                                    {
                                        tempInv.RegistrationDetails = regDetails != null ? regDetails.ToObservableCollection() : null;

                                        tempInv.PclRequests = pclRequestList != null ? pclRequestList.ToObservableCollection() : null;

                                        tempInv.OutwardDrugClinicDeptInvoices = allInPatientInvoices != null ? allInPatientInvoices.ToObservableCollection() : null;
                                    }

                                    //KMx: Show pop-up chi tiết Bill (13/09/2014 11:12).
                                    //var vm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
                                    //vm.CurrentSupportFunds = tempInv.IsHighTechServiceBill && SupportFund_ForHighTechServiceBill != null && SupportFund_ForHighTechServiceBill.Count() > 0 ?
                                    //    SupportFund_ForHighTechServiceBill.Where(x=>x.BillingInvID == tempInv.InPatientBillingInvID).ToObservableCollection() : null;
                                    //vm.CanDelete = false;
                                    //vm.ShowDeleteColumn = false;
                                    //vm.ShowHIAppliedColumn = ShowHIAppliedColumn;
                                    //vm.BillingInvoice = tempInv;
                                    //vm.CurentRegistration = this.CurentRegistration;
                                    //vm.ResetView();

                                    //Globals.ShowDialog(vm as Conductor<object>);
                                    void onInitDlg(IInPatientBillingInvoiceDetailsListing Alloc)
                                    {
                                        Alloc.CurrentSupportFunds = tempInv.IsHighTechServiceBill && SupportFund_ForHighTechServiceBill != null && SupportFund_ForHighTechServiceBill.Count() > 0 ?
                                            SupportFund_ForHighTechServiceBill.Where(x => x.BillingInvID == tempInv.InPatientBillingInvID).ToObservableCollection() : null;
                                        Alloc.CanDelete = false;
                                        Alloc.ShowDeleteColumn = false;
                                        Alloc.ShowHIAppliedColumn = ShowHIAppliedColumn;
                                        Alloc.BillingInvoice = tempInv;
                                        //▼====== #001: Truyền HIBenefit cho Popup xem chi tiết để thực hiện kiểm tra quyền lợi BHYT của bệnh nhân
                                        Alloc.HIBenefit = this.HIBenefit;
                                        //▲====== #001
                                        Alloc.CurentRegistration = this.CurentRegistration;
                                        Alloc.IsNewCreateBill = IsNewCreateBill;
                                        Alloc.ResetView();
                                    }
                                    GlobalsNAV.ShowDialog<IInPatientBillingInvoiceDetailsListing>(onInitDlg);

                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), inv);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    //Globals.HideBusy();
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });

            t.Start();
        }

        public void RemoveItemCmd(object source, object eventArgs)
        {
            if (Globals.IsLockRegistration(RegLockFlag, eHCMSResources.G2773_G1_XoaDV))
            {
                return;
            }
            var sender = source as Button;
            if (sender != null)
            {
                var ctx = sender.DataContext as MedRegItemBase;
                if (ctx != null && BillingInvoices != null)
                {
                    if (ctx.ID > 0)
                    {
                        return;
                    }
                    foreach (var inv in BillingInvoices)
                    {
                        if (ctx is PatientRegistrationDetail)
                        {
                            var item = (PatientRegistrationDetail)ctx;
                            if (inv.RegistrationDetails != null)
                            {
                                inv.RegistrationDetails.Remove(item);
                            }
                        }
                        else if (ctx is PatientPCLRequestDetail)
                        {
                            var item = (PatientPCLRequestDetail)ctx;
                            if (inv.PclRequests != null)
                            {
                                foreach (var request in inv.PclRequests)
                                {
                                    if (request.PatientPCLRequestIndicators != null
                                        && request.PatientPCLRequestIndicators.Contains(item))
                                    {
                                        request.PatientPCLRequestIndicators.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (ctx is OutwardDrugClinicDept)
                        {
                            var item = (OutwardDrugClinicDept)ctx;
                            if (inv.OutwardDrugClinicDeptInvoices != null)
                            {
                                foreach (var clinicDeptInv in inv.OutwardDrugClinicDeptInvoices)
                                {
                                    if (clinicDeptInv.OutwardDrugClinicDepts != null
                                        && clinicDeptInv.OutwardDrugClinicDepts.Contains(item))
                                    {
                                        clinicDeptInv.OutwardDrugClinicDepts.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        public List<long> GetSelectedIds()
        {
            var ids = new List<long>();
            if (BillingInvoices != null)
            {
                ids.AddRange(from inv in BillingInvoices where inv.IsChecked select inv.InPatientBillingInvID);
            }
            return ids;
        }

        public List<InPatientBillingInvoice> GetSelectedItems()
        {
            List<InPatientBillingInvoice> items = null;
            if (BillingInvoices != null)
            {
                items = BillingInvoices.Where(inv => inv.IsChecked).ToList();
            }
            return items;
        }

        //public void RefreshDetailsView(InPatientBillingInvoice inv)
        //{
        //    if (InvoiceDetailsContent != null)
        //    {
        //        InvoiceDetailsContent.BillingInvoice = inv;
        //        InvoiceDetailsContent.ResetView(); 
        //    }
        //}

        private Button ShowBillingInvoiceDetail { get; set; }
        public void ShowBillingInvoiceDetail_Loaded(object sender)
        {
            ShowBillingInvoiceDetail = (Button)sender;
            ShowBillingInvoiceDetail.Visibility = Globals.convertVisibility(mDangKyNoiTru_XemChiTiet);
        }

        public void CheckBox_Loaded(object sender)
        {
            ((CheckBox)sender).Visibility = Globals.convertVisibility(mTickSelect);
        }

        public void ShowBillingInvoiceDetailCmd(InPatientBillingInvoice source, object eventArgs)
        {
            GetBillingInvoiceDetails(source);

            //KMx: Dùng cho Expander, nhưng bây giờ chuyển sang dùng Pop-up khi xem chi tiết Bill
            //BeingViewedItem = source;
            //if (source != null
            //      && (source.RegistrationDetails == null || source.RegistrationDetails.Count == 0)
            //      && (source.PclRequests == null || source.PclRequests.Count == 0)
            //      && (source.OutwardDrugClinicDeptInvoices == null || source.OutwardDrugClinicDeptInvoices.Count == 0)
            //      && source.InPatientBillingInvID > 0)
            //{
            //    GetBillingInvoiceDetails(source, RefreshDetailsView);
            //}
            //else
            //{
            //    RefreshDetailsView(source);
            //}
            //var view = GetView() as IInPatientBillingInvoiceListingView;
            //if (view != null)
            //{
            //    view.ExpandDetailsInfo();
            //}
        }

        //KMx: Khi click xem chi tiết thì hiện pop-up lên. Không dùng Expander nữa, vì khó nhìn (13/09/2014 10:10).
        //public void ShowBillingInvoiceDetailCmd(InPatientBillingInvoice source, object eventArgs)
        //{
        //    BeingViewedItem = source;
        //    if (source != null
        //          && (source.RegistrationDetails == null || source.RegistrationDetails.Count == 0)
        //          && (source.PclRequests == null || source.PclRequests.Count == 0)
        //          && (source.OutwardDrugClinicDeptInvoices == null || source.OutwardDrugClinicDeptInvoices.Count == 0)
        //          && source.InPatientBillingInvID > 0)
        //    {
        //        GetBillingInvoiceDetails(source, RefreshDetailsView);
        //    }
        //    else
        //    {
        //        RefreshDetailsView(source);
        //    }
        //    var view = GetView() as IInPatientBillingInvoiceListingView;
        //    if (view != null)
        //    {
        //        view.ExpandDetailsInfo();
        //    }
        //}
        //▼====: #002
        public void RecalcHiCmd(InPatientBillingInvoice billInv, object eventArgs)
        {
            if (Globals.IsLockRegistration(RegLockFlag, eHCMSResources.G1293_G1_TinhLaiBills.ToLower()))
            {
                return;
            }
            if (billInv.BillingInvIsFinalized && (CurentRegistration == null || CurentRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU))
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0257_G1_Msg_InfoKhTheTinhLaiBillDaQToan));
                return;
            }
            if (billInv.BillingInvIsFinalized && (CurentRegistration == null || (CurentRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU && CurentRegistration.PtInsuranceBenefit != billInv.HIBenefit)))
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0257_G1_Msg_InfoKhTheTinhLaiBillDaQToan));
                return;
            }
            GetBillingInvoiceDetailsForRecal(billInv, false, false);           
        }
        public void RecalcAllHiCmd()
        {
            if(BillingInvoices!= null && BillingInvoices.Count > 0)
            {
                //IsLoading = true;
                this.ShowBusyIndicator();
            }
            foreach (var item in BillingInvoices)
            {
                if (Globals.IsLockRegistration(RegLockFlag, eHCMSResources.G1293_G1_TinhLaiBills.ToLower()))
                {
                    return;
                }
                if (item.BillingInvIsFinalized && (CurentRegistration == null || CurentRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU))
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0257_G1_Msg_InfoKhTheTinhLaiBillDaQToan));
                    return;
                }
                if (item.BillingInvIsFinalized && (CurentRegistration == null || (CurentRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU && CurentRegistration.PtInsuranceBenefit != item.HIBenefit)))
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0257_G1_Msg_InfoKhTheTinhLaiBillDaQToan));
                    return;
                }
                bool IsRecalcAllHiDone;
                if (item.Equals(BillingInvoices.LastOrDefault()))
                {
                    IsRecalcAllHiDone = true;
                }
                else
                {
                    IsRecalcAllHiDone = false;
                }
                GetBillingInvoiceDetailsForRecal(item, false, false,false, IsRecalcAllHiDone);
            }
            this.HideBusyIndicator();
        }

        public void RecalcHiWithPriceListCmd(InPatientBillingInvoice billInv, object eventArgs)
        {
            if (Globals.IsLockRegistration(RegLockFlag, eHCMSResources.G1293_G1_TinhLaiBills.ToLower()))
            {
                return;
            }
            if (billInv.BillingInvIsFinalized && (CurentRegistration == null || CurentRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU))
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0257_G1_Msg_InfoKhTheTinhLaiBillDaQToan));
                return;
            }
            if (billInv.BillingInvIsFinalized && (CurentRegistration == null || (CurentRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU && CurentRegistration.PtInsuranceBenefit != billInv.HIBenefit)))
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0257_G1_Msg_InfoKhTheTinhLaiBillDaQToan));
                return;
            }

            GetBillingInvoiceDetailsForRecal(billInv, true, false);
        }
        //▲====: #002

        private Button hplEditBillingInvoice { get; set; }
        public void EditBillingInvoice_Loaded(object sender)
        {
            hplEditBillingInvoice = (Button)sender;
            hplEditBillingInvoice.Visibility = Globals.convertVisibility(mDangKyNoiTru_SuaDV);
        }

        public void EditBillingInvoiceCmd(InPatientBillingInvoice billInv, object eventArgs)
        {
            if (Globals.IsLockRegistration(RegLockFlag, "chỉnh sửa bill"))
            {
                return;
            }
            if (billInv.BillingInvIsFinalized)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0258_G1_Msg_InfoKhTheSuaBillDaQToan));
                return;
            }
            else
            {
                Globals.EventAggregator.Publish(new EditItem<InPatientBillingInvoice> { Item = billInv });
            }
        }

        public void PrintBillCmd(InPatientBillingInvoice source, object eventArgs)
        {
            if (source == null || source.InPatientBillingInvID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0653_G1_Msg_InfoKhCoHDDeIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            //var reportVm = Globals.GetViewModel<IBillingInvoiceDetailsReport>();
            //reportVm.InPatientBillingInvID = source.InPatientBillingInvID;
            //Globals.ShowDialog(reportVm as Conductor<object>);
            void onInitDlg(IBillingInvoiceDetailsReport Alloc)
            {
                Alloc.InPatientBillingInvID = source.InPatientBillingInvID;
            }
            GlobalsNAV.ShowDialog<IBillingInvoiceDetailsReport>(onInitDlg, null, false, true);
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region bool properties

        private bool _mDangKyNoiTru_SuaDV = true;
        private bool _mDangKyNoiTru_XemChiTiet = true;

        public bool mDangKyNoiTru_SuaDV
        {
            get
            {
                return _mDangKyNoiTru_SuaDV;
            }
            set
            {
                if (_mDangKyNoiTru_SuaDV == value)
                    return;
                _mDangKyNoiTru_SuaDV = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_SuaDV);
            }
        }

        public bool mDangKyNoiTru_XemChiTiet
        {
            get
            {
                return _mDangKyNoiTru_XemChiTiet;
            }
            set
            {
                if (_mDangKyNoiTru_XemChiTiet == value)
                    return;
                _mDangKyNoiTru_XemChiTiet = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_XemChiTiet);
            }
        }
        #endregion

        private bool _bShowTotalPrice;
        public bool bShowTotalPrice
        {
            get
            {
                return _bShowTotalPrice;
            }
            set
            {
                if (_bShowTotalPrice == value)
                    return;
                _bShowTotalPrice = value;
                NotifyOfPropertyChange(() => bShowTotalPrice);
            }
        }

        private decimal _SumAmount;
        public decimal SumAmount
        {
            get
            {
                return _SumAmount;
            }
            set
            {
                if (_SumAmount == value)
                    return;
                _SumAmount = value;
                NotifyOfPropertyChange(() => SumAmount);
            }
        }

        private decimal _SumTotalHIPayment;
        public decimal SumTotalHIPayment
        {
            get
            {
                return _SumTotalHIPayment;
            }
            set
            {
                if (_SumTotalHIPayment == value)
                    return;
                _SumTotalHIPayment = value;
                NotifyOfPropertyChange(() => SumTotalHIPayment);
            }
        }

        private decimal _SumTotalPatientPayment;
        public decimal SumTotalPatientPayment
        {
            get
            {
                return _SumTotalPatientPayment;
            }
            set
            {
                if (_SumTotalPatientPayment == value)
                    return;
                _SumTotalPatientPayment = value;
                NotifyOfPropertyChange(() => SumTotalPatientPayment);
            }
        }

        private void SumTotalPrice()
        {
            if (BillingInvoices == null || BillingInvoices.Count <= 0)
            {
                SumAmount = 0;
                SumTotalHIPayment = 0;
                SumTotalPatientPayment = 0;
                return;
            }

            //SumAmount = MathExt.Round(BillingInvoices.Sum(x => x.TotalInvoicePrice), MidpointRounding.AwayFromZero);
            //SumTotalHIPayment = MathExt.Round(BillingInvoices.Sum(x => x.TotalHIPayment), MidpointRounding.AwayFromZero);
            //SumTotalPatientPayment = SumAmount - SumTotalHIPayment;
        }

        private int _RegLockFlag = 0;
        public int RegLockFlag
        {
            get
            {
                return _RegLockFlag;
            }
            set
            {
                _RegLockFlag = value;
                NotifyOfPropertyChange(() => RegLockFlag);
            }
        }

        private ObservableCollection<CharitySupportFund> _CurrentSupportFund;
        public ObservableCollection<CharitySupportFund> CurrentSupportFund
        {
            get
            {
                return _CurrentSupportFund;
            }
            set
            {
                _CurrentSupportFund = value;
                NotifyOfPropertyChange(() => CurrentSupportFund);
            }
        }

        private ObservableCollection<CharitySupportFund> _SupportFund_ForHighTechServiceBill;
        public ObservableCollection<CharitySupportFund> SupportFund_ForHighTechServiceBill
        {
            get
            {
                return _SupportFund_ForHighTechServiceBill;
            }
            set
            {
                _SupportFund_ForHighTechServiceBill = value;
                NotifyOfPropertyChange(() => SupportFund_ForHighTechServiceBill);
            }
        }

        public void ShowPopupCharitySupport(InPatientBillingInvoice billInv, object eventArgs)
        {
            //if (Globals.IsLockRegistration(RegLockFlag, eHCMSResources.G1293_G1_TinhLaiBills.ToLower()))
            //{
            //    return;
            //}
            if (billInv == null || billInv.InPatientBillingInvID <= 0)
            {
                return;
            }
            if (billInv.BillingInvIsFinalized)
            {
                MessageBox.Show(string.Format("Không thể cập nhật thông tin quỹ hỗ trợ cho bill đã quyết toán!"));
                return;
            }
            //var vm = Globals.GetViewModel<ICharitySupportFund>();
            //vm.SelectedBillingInv = billInv;
            //vm.PtRegistrationID = billInv.PtRegistrationID;
            //vm.IsHighTechServiceBill = billInv.IsHighTechServiceBill;
            //vm.GetAllCharityOrganization();
            //vm.GetCharitySupportFunds();
            //Globals.ShowDialog(vm as Conductor<object>, false);

            void onInitDlg(ICharitySupportFund Alloc)
            {
                Alloc.SelectedBillingInv = billInv;
                Alloc.PtRegistrationID = billInv.PtRegistrationID;
                Alloc.IsHighTechServiceBill = billInv.IsHighTechServiceBill;
                Alloc.GetAllCharityOrganization();
                Alloc.GetCharitySupportFunds();
            }
            GlobalsNAV.ShowDialog<ICharitySupportFund>(onInitDlg);
        }
        private PatientRegistration _CurentRegistration;
        public PatientRegistration CurentRegistration
        {
            get
            {
                return _CurentRegistration;
            }
            set
            {
                if (_CurentRegistration != value)
                {
                    _CurentRegistration = value;
                    NotifyOfPropertyChange(() => CurentRegistration);
                }
            }
        }

        private long _SelectedMedPriceListID = 0;
        public long SelectedMedPriceListID
        {
            get
            {
                return _SelectedMedPriceListID;
            }
            set
            {
                if (_SelectedMedPriceListID != value)
                {
                    _SelectedMedPriceListID = value;
                    NotifyOfPropertyChange(() => SelectedMedPriceListID);
                }
            }
        }

        private long _SelectedPCLPriceListID = 0;
        public long SelectedPCLPriceListID
        {
            get
            {
                return _SelectedPCLPriceListID;
            }
            set
            {
                if (_SelectedPCLPriceListID != value)
                {
                    _SelectedPCLPriceListID = value;
                    NotifyOfPropertyChange(() => SelectedPCLPriceListID);
                }
            }
        }

        private bool _DoRecalcHiWithPriceList = false;
        public bool DoRecalcHiWithPriceList
        {
            get
            {
                return _DoRecalcHiWithPriceList;
            }
            set
            {
                if (_DoRecalcHiWithPriceList != value)
                {
                    _DoRecalcHiWithPriceList = value;
                    NotifyOfPropertyChange(() => DoRecalcHiWithPriceList);
                }
            }
        }

        public void Handle(SelectPriceListForRecalcBillInvoiceEvent message)
        {
            if (message != null)
            {
                SelectedMedPriceListID = message.MedServiceItemPriceListID;
                SelectedPCLPriceListID = message.PCLExamTypePriceListID;
                DoRecalcHiWithPriceList = message.DoRecalcHiWithPriceList;
            }
        }

        private readonly bool ReCalBillingInv = true;

        public void GetBillingInvoiceDetailsForRecal(InPatientBillingInvoice inv, bool WithPriceList = true, bool IsRecalSecondTime = false
            , bool IsPassCheckNonBlockValidPCLExamDate = false, bool IsRecalcAllHiDone = true)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientBillingInvoiceDetails(inv.InPatientBillingInvID, false, IsRecalSecondTime, IsPassCheckNonBlockValidPCLExamDate,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    contract.EndGetInPatientBillingInvoiceDetails(out List<PatientRegistrationDetail> regDetails
                                        , out List<PatientPCLRequest> pclRequestList
                                        , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices, asyncResult);
                                    if (asyncResult.AsyncState is InPatientBillingInvoice tempInv)
                                    {
                                        tempInv.RegistrationDetails = regDetails?.ToObservableCollection();
                                        tempInv.PclRequests = pclRequestList?.ToObservableCollection();
                                        tempInv.OutwardDrugClinicDeptInvoices = allInPatientInvoices?.ToObservableCollection();

                                        if (WithPriceList)
                                        {
                                            RecalcHiWithPriceListAfterLoadBill(tempInv, IsPassCheckNonBlockValidPCLExamDate);
                                        }
                                        else RecalcHiAfterLoadBill(tempInv, IsRecalSecondTime, IsPassCheckNonBlockValidPCLExamDate, IsRecalcAllHiDone);
                                    }
                                }
                                //catch (FaultException<AxException> fault)
                                //{
                                //    error = new AxErrorEventArgs(fault);
                                //}
                                catch (Exception ex)
                                {
                                    IsLoading = false;
                                    error = new AxErrorEventArgs(ex);
                                    if (!IsRecalSecondTime && ex.Message.Contains("19090601"))
                                    {
                                        if (MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                                        {
                                            GetBillingInvoiceDetailsForRecal(inv, WithPriceList, true, true);
                                        }
                                        else
                                        {
                                            GetBillingInvoiceDetailsForRecal(inv, WithPriceList, true, false);
                                        }
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    }
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), inv);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    this.HideBusyIndicator();
                }
                finally
                {
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });

            t.Start();
        }

        private void RecalcHiWithPriceListAfterLoadBill(InPatientBillingInvoice billInv, bool IsPassCheckNonBlockValidPCLExamDate = false)
        {
            if (billInv == null || ((billInv.RegistrationDetails == null && billInv.PclRequests == null) ||
                (billInv.RegistrationDetails != null && billInv.RegistrationDetails.Count() == 0 && billInv.PclRequests != null && billInv.PclRequests.Count() == 0)))
            {
                MessageBox.Show(eHCMSResources.Z2911_G1_ChiDanhChoBillCoDVCLS);
                return;
            }

            // TNHX: Reset PriceListID
            SelectedMedPriceListID = 0;
            SelectedPCLPriceListID = 0;
            DoRecalcHiWithPriceList = false;
            // TNHX: Popup choosePriceList View
            void onInitDlg(IInPatientChoosePriceList Alloc)
            {
                Alloc.RecalBillingInvoice = billInv;
            }
            GlobalsNAV.ShowDialog<IInPatientChoosePriceList>(onInitDlg, null, false, true, Globals.GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize());

            bool IsUsePriceByNewCer = false;
            if (CurentRegistration != null && CurentRegistration.AdmissionInfo != null
                && (CurentRegistration.AdmissionInfo.MedServiceItemPriceListID > 0 || CurentRegistration.AdmissionInfo.PCLExamTypePriceListID > 0))
            {
                if (MessageBox.Show(string.Format("{0}?", eHCMSResources.Z2791_G1_SuDungBangGiaMoi), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    IsUsePriceByNewCer = true;
                }
            }

            if (!DoRecalcHiWithPriceList)
            {
                //MessageBox.Show(eHCMSResources.Z2900_G1_Chon1BangGia);
                return;
            }
            //▼===== #003
            bool IsAutoCheckCountHI = false;
            //▼====: #005
            //if (AutoCheckCountHI(billInv) && billInv.TotalHIPayment == 0)
            //{
            //    if (MessageBox.Show(string.Format("{0}", eHCMSResources.Z2975_G1_HoiTuDongCheck), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        IsAutoCheckCountHI = true;
            //    }
            //}
            //▲===== #003
            //▲====: #005

            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRecalcInPatientBillingInvoiceWithPriceList(Globals.LoggedUserAccount.StaffID, billInv, IsUsePriceByNewCer, IsAutoCheckCountHI, ReplaceMaxHIPay
                            , SelectedMedPriceListID, SelectedPCLPriceListID, ReCalBillingInv
                            , Globals.DispatchCallback(asyncResult =>
                           {
                               try
                               {
                                   contract.EndRecalcInPatientBillingInvoiceWithPriceList(asyncResult);
                                   MessageBox.Show(eHCMSResources.A0474_G1_Msg_InfoTinhLaiBillOK);
                                   Globals.EventAggregator.Publish(new ReloadRegisAfterRecalcBillInvoiceEvent() { Item = billInv });
                               }
                               //catch (FaultException<AxException> fault)
                               //{
                               //    error = new AxErrorEventArgs(fault);
                               //}
                               catch (Exception ex)
                               {
                                   error = new AxErrorEventArgs(ex);
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
                    error = new AxErrorEventArgs(ex);
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });

            t.Start();
        }

        private void RecalcHiAfterLoadBill(InPatientBillingInvoice billInv, bool IsNotCheckInvalid, bool IsPassCheckNonBlockValidPCLExamDate, bool IsRecalcAllHiDone = true)
        {
            //▼===== #004
            //if (billInv.IsValidForBill > 1)
            //{
            //    if (billInv == null || ((billInv.RegistrationDetails == null && billInv.PclRequests == null) ||
            //    ((billInv.RegistrationDetails != null && billInv.RegistrationDetails.Count() > 0) || (billInv.PclRequests != null && billInv.PclRequests.Count() > 0))))
            //    {
            //        MessageBox.Show(eHCMSResources.Z2900_G1_DkyDoiDoiTuongTinhLaiBill);
            //        return;
            //    }
            //}
            //▲===== #004
            bool IsUsePriceByNewCer = false;
            if (CurentRegistration != null && CurentRegistration.AdmissionInfo != null
                && (CurentRegistration.AdmissionInfo.MedServiceItemPriceListID > 0 || CurentRegistration.AdmissionInfo.PCLExamTypePriceListID > 0))
            {
                if (MessageBox.Show(string.Format("{0}?", eHCMSResources.Z2791_G1_SuDungBangGiaMoi), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    IsUsePriceByNewCer = true;
                }
            }
            //▼===== #003
            //▼====: #006
            bool IsAutoCheckCountHI = true;
            //▲====: #006
            //▼====: #005
            //if (AutoCheckCountHI(billInv) && billInv.TotalHIPayment == 0)
            //{
            //    if(MessageBox.Show(string.Format("{0}", eHCMSResources.Z2975_G1_HoiTuDongCheck), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        IsAutoCheckCountHI = true;
            //    }
            //}
            //▲===== #003
            //▲====: #005
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRecalcInPatientBillingInvoice(Globals.LoggedUserAccount.StaffID, billInv, IsUsePriceByNewCer, IsAutoCheckCountHI, ReplaceMaxHIPay, ReCalBillingInv
                            , IsNotCheckInvalid
                            , IsPassCheckNonBlockValidPCLExamDate
                            , Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    contract.EndRecalcInPatientBillingInvoice(asyncResult);
                                    if (IsRecalcAllHiDone)
                                    {
                                        MessageBox.Show(eHCMSResources.A0474_G1_Msg_InfoTinhLaiBillOK);
                                        Globals.EventAggregator.Publish(new ReloadRegisAfterRecalcBillInvoiceEvent() { Item = billInv });
                                    }
                                }
                                //catch (FaultException<AxException> fault)
                                //{
                                //    error = new AxErrorEventArgs(fault);
                                //}
                                catch (Exception ex)
                                {
                                    IsLoading = false;
                                    error = new AxErrorEventArgs(ex);
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    if (IsRecalcAllHiDone)
                                    {
                                        IsLoading = false;
                                        this.HideBusyIndicator();
                                    }
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });

            t.Start();
        }
        private bool _IsNewCreateBill = false;
        public bool IsNewCreateBill
        {
            get
            {
                return _IsNewCreateBill;
            }
            set
            {
                if (_IsNewCreateBill != value)
                {
                    _IsNewCreateBill = value;
                    NotifyOfPropertyChange(() => IsNewCreateBill);
                }
            }
        }
        //▼===== #003
        public bool AutoCheckCountHI(InPatientBillingInvoice billInv)
        {
            //Lấy danh sách Dịch vụ, CLS, thuốc y cụ ... Không cần kiểm tra billInv == null vì trước hàm này đã kiểm tra rồi.
            ObservableCollection<PatientRegistrationDetail> ListPatientRegistrationDetails = billInv.RegistrationDetails;
            ObservableCollection<PatientPCLRequest> ListPatientPCLRequest = billInv.PclRequests;
            ObservableCollection<OutwardDrugClinicDeptInvoice> ListOutwardDrug = billInv.OutwardDrugClinicDeptInvoices;
            //Kiểm tra dịch vụ thuộc danh mục bảo hiểm.
            if (ListPatientRegistrationDetails != null && ListPatientRegistrationDetails.Count > 0)
            {
                foreach(var detail in ListPatientRegistrationDetails)
                {
                    if (detail.HIAllowedPrice > 0 && !detail.IsCountHI)
                    {
                        return true;
                    }
                }
            }
            //Kiểm tra CLS thuộc danh mục BH
            if (ListPatientPCLRequest != null && ListPatientPCLRequest.Count > 0)
            {
                foreach (var invoice in ListPatientPCLRequest)
                {
                    if (invoice.PatientPCLRequestIndicators != null && invoice.PatientPCLRequestIndicators.Count > 0)
                    {
                        foreach (var detail in invoice.PatientPCLRequestIndicators)
                        {
                            if (detail.HIAllowedPrice > 0 && !detail.IsCountHI)
                            {
                                return true;
                            }
                        }

                    }
                }
            }
            //Kiểm tra tồn tại thuốc thuộc danh mục bảo hiểm.
            if (ListOutwardDrug != null && ListOutwardDrug.Count > 0)
            {
                foreach (var invoice in ListOutwardDrug)
                {
                    if (invoice.OutwardDrugClinicDepts != null && invoice.OutwardDrugClinicDepts.Count > 0)
                    {
                        foreach (var detail in invoice.OutwardDrugClinicDepts)
                        {
                            if (detail.HIAllowedPrice > 0 && !detail.IsCountHI)
                            {
                                return true;
                            }
                        }

                    }
                }
            }
            return false;
        }
        //▲===== #003
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }
    }
}
