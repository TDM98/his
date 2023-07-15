using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Castle.Windsor;
using DataEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Data;

/*
 * 20191228 TBL: BM 0021779: Thêm nút xem chi tiết bill của bệnh ngoại trú
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IOutPatientBillingManage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPatientBillingManageViewModel : ViewModelBase, IOutPatientBillingManage
    {
        [ImportingConstructor]
        public OutPatientBillingManageViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
        }
        #region Properties
        private CollectionViewSource ItemColectionViewSource;
        public CollectionView ItemColectionView { get; set; }
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
        #endregion
        #region Methods
        public void UpdateItemList(IList<InPatientBillingInvoice> aItemCollection)
        {
            if (aItemCollection == null)
            {
                aItemCollection = new ObservableCollection<InPatientBillingInvoice>();
            }
            ItemColectionViewSource = new CollectionViewSource { Source = aItemCollection };
            ItemColectionView = (CollectionView)ItemColectionViewSource.View;
            ItemColectionView.Refresh();
            NotifyOfPropertyChange(() => ItemColectionView);
        }
        #endregion
        #region Events
        public void RemoveBillingCmd(object sender, object eventArgs)
        {
            var mSelectedItem = sender as FrameworkElement;
            if (mSelectedItem != null && mSelectedItem.DataContext != null)
            {
                Globals.EventAggregator.Publish(new RemoveItem<InPatientBillingInvoice>() { Item = mSelectedItem.DataContext as InPatientBillingInvoice });
            }
        }
        //▼====: #001
        public void ShowBillingInvoiceDetailCmd(InPatientBillingInvoice source, object eventArgs)
        {
            GetBillingInvoiceDetails(source);
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
                                    void onInitDlg(IInPatientBillingInvoiceDetailsListing Alloc)
                                    {
                                        Alloc.CurrentSupportFunds = tempInv.IsHighTechServiceBill && SupportFund_ForHighTechServiceBill != null && SupportFund_ForHighTechServiceBill.Count() > 0 ?
                                            SupportFund_ForHighTechServiceBill.Where(x => x.BillingInvID == tempInv.InPatientBillingInvID).ToObservableCollection() : null;
                                        Alloc.CanDelete = false;
                                        Alloc.ShowDeleteColumn = false;
                                        Alloc.ShowHIAppliedColumn = false;
                                        Alloc.BillingInvoice = tempInv;
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
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });

            t.Start();
        }
        //▲====: #001
        #endregion
    }
}