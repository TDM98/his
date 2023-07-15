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
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof (IInPatientBillingInvoiceListing)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientBillingInvoiceListingViewModel : Conductor<object>, IInPatientBillingInvoiceListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientBillingInvoiceListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //var detailsVm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
            //InvoiceDetailsContent = detailsVm;
            //InvoiceDetailsContent.CanDelete = false;
            //InvoiceDetailsContent.ShowDeleteColumn = false;

            if (_showInfoColumn)
            {
                var detailsVm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
                InvoiceDetailsContent = detailsVm;
                InvoiceDetailsContent.CanDelete = false;
                InvoiceDetailsContent.ShowDeleteColumn = false;
                InvoiceDetailsContent.ShowHIAppliedColumn = false;
            }
            else
            {
                InvoiceDetailsContent = null;
            }
            authorization();
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var currentview = view as IInPatientBillingInvoiceListingView;
            if (currentview != null)
            {
                currentview.ShowEditColumn(_showEditColumn);
                currentview.ShowInfoColumn(_showInfoColumn);
                currentview.ShowRecalcHiColumn(_showRecalcHiColumn);
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

                    var view = this.GetView() as IInPatientBillingInvoiceListingView;
                    if (view != null)
                    {
                        view.ShowEditColumn(_showEditColumn);
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

                    if (InvoiceDetailsContent != null)
                    {
                        InvoiceDetailsContent.ShowHIAppliedColumn = ShowHIAppliedColumn;
                    }
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

                    var view = GetView() as IInPatientBillingInvoiceListingView;
                    if (view != null)
                    {
                        view.ShowInfoColumn(_showInfoColumn);
                    }
                    if(_showInfoColumn)
                    {
                        var detailsVm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
                        InvoiceDetailsContent = detailsVm;
                        InvoiceDetailsContent.CanDelete = false;
                        InvoiceDetailsContent.ShowDeleteColumn = false;
                    }
                    else
                    {
                        InvoiceDetailsContent = null;
                    }
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

        private IInPatientBillingInvoiceDetailsListing _invoiceDetailsContent;
        public IInPatientBillingInvoiceDetailsListing InvoiceDetailsContent
        {
            get { return _invoiceDetailsContent; }
            set { _invoiceDetailsContent = value; 
            NotifyOfPropertyChange(()=>InvoiceDetailsContent);}
        }

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

        private ObservableCollection<InPatientBillingInvoice> _billingInvoices;

        public ObservableCollection<InPatientBillingInvoice> BillingInvoices
        {
            get { return _billingInvoices; }
            set 
            {
                if (_billingInvoices != value)
                {
                    _billingInvoices = value;
                    NotifyOfPropertyChange(() => BillingInvoices);
                    if (InvoiceDetailsContent != null)
                    {
                        InvoiceDetailsContent.BillingInvoice = null;
                        InvoiceDetailsContent.ResetView(); 
                    }
                }
            }
        }

        private InPatientBillingInvoice _beingViewedItem;
        public InPatientBillingInvoice BeingViewedItem
        {
            get { return _beingViewedItem; }
            set
            {
                _beingViewedItem = value;
                if (InvoiceDetailsContent != null)
                {
                    InvoiceDetailsContent.BillingInvoice = _beingViewedItem;  
                }
                NotifyOfPropertyChange(()=>BeingViewedItem);
                NotifyOfPropertyChange(() => DetailItemTitle);
            }
        }

        public string DetailItemTitle
        {
            get
            {
                if(_beingViewedItem != null)
                {
                    return string.Format("{0} ", eHCMSResources.Z1240_G1_DSDVBill) + _beingViewedItem.BillingInvNum;
                }
                return string.Empty;
            }
        }

        

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

        public void GetBillingInvoiceDetails(InPatientBillingInvoice inv, Action<InPatientBillingInvoice> callback = null)
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

                                    if (callback != null)
                                    {
                                        callback(tempInv);
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                            }), inv);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
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
            var sender = source as Button;
            if(sender != null)
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
        public void RefreshDetailsView(InPatientBillingInvoice inv)
        {
            if (InvoiceDetailsContent != null)
            {
                InvoiceDetailsContent.BillingInvoice = inv;
                InvoiceDetailsContent.ResetView(); 
            }
        }

        private Button ShowBillingInvoiceDetail { get; set; }
        public void ShowBillingInvoiceDetail_Loaded(object sender)
        {
            ShowBillingInvoiceDetail = (Button) sender;
            ShowBillingInvoiceDetail.Visibility = Globals.convertVisibility(mDangKyNoiTru_XemChiTiet);
        }
        
        public void CheckBox_Loaded(object sender)
        {
            ((CheckBox) sender).Visibility = Globals.convertVisibility(mTickSelect);
        }
        public void ShowBillingInvoiceDetailCmd(InPatientBillingInvoice source, object eventArgs)
        {
            BeingViewedItem = source;
            if (source != null
                  && (source.RegistrationDetails == null || source.RegistrationDetails.Count == 0)
                  && (source.PclRequests == null || source.PclRequests.Count == 0)
                  && (source.OutwardDrugClinicDeptInvoices == null || source.OutwardDrugClinicDeptInvoices.Count == 0)
                  && source.InPatientBillingInvID > 0)
            {
                GetBillingInvoiceDetails(source, RefreshDetailsView);
            }
            else
            {
                RefreshDetailsView(source);
            }
            var view = GetView() as IInPatientBillingInvoiceListingView;
            if (view != null)
            {
                view.ExpandDetailsInfo();
            }
        }
        public void RecalcHiCmd(InPatientBillingInvoice source, object eventArgs)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRecalcInPatientBillingInvoice(Globals.LoggedUserAccount.StaffID, source, false,false, false, false, false, false,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    contract.EndRecalcInPatientBillingInvoice(asyncResult);
                                    MessageBox.Show(eHCMSResources.A0474_G1_Msg_InfoTinhLaiBillOK);
                                    Globals.EventAggregator.Publish(new ItemChanged<InPatientBillingInvoice, IInPatientBillingInvoiceListing> { Item = source, Source = this });
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
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
        
        private Button hplEditBillingInvoice { get; set; }
        public void EditBillingInvoice_Loaded(object sender)
        {
            hplEditBillingInvoice = (Button) sender;
            hplEditBillingInvoice.Visibility = Globals.convertVisibility(mDangKyNoiTru_SuaDV);
        }

        public void EditBillingInvoiceCmd(InPatientBillingInvoice source, object eventArgs)
        {
            
            if(source.PaidTime != null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0259_G1_Msg_KhTheSuaBillDaTraTien));
            }
            else
            {
                Globals.EventAggregator.Publish(new EditItem<InPatientBillingInvoice> { Item = source });   
            }
        }

        public void authorization()
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
    }

}