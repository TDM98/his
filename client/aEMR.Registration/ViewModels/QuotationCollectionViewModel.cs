using aEMR.Common.Collections;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Ax.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using static aEMR.Registration.ViewModels.QuotationViewModel;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IQuotationCollection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class QuotationCollectionViewModel : Conductor<object>, IQuotationCollection
    {
        #region Properties
        private ObservableCollection<InPatientBillingInvoice> _QuotationCollection;
        public ObservableCollection<InPatientBillingInvoice> QuotationCollection
        {
            get
            {
                return _QuotationCollection;
            }
            set
            {
                if (_QuotationCollection == value)
                {
                    return;
                }
                _QuotationCollection = value;
                NotifyOfPropertyChange(() => QuotationCollection);
            }
        }
        private InPatientBillingInvoice _SelectedQuotation;
        public InPatientBillingInvoice SelectedQuotation
        {
            get
            {
                return _SelectedQuotation;
            }
            set
            {
                if (_SelectedQuotation == value)
                {
                    return;
                }
                _SelectedQuotation = value;
                NotifyOfPropertyChange(() => SelectedQuotation);
            }
        }
        public bool IsHasSelected { get; set; } = false;
        private short _CurrentViewCase = (short)ViewCase.Template;
        public short CurrentViewCase
        {
            get
            {
                return _CurrentViewCase;
            }
            set
            {
                if (_CurrentViewCase == value)
                {
                    return;
                }
                _CurrentViewCase = value;
                NotifyOfPropertyChange(() => CurrentViewCase);
                NotifyOfPropertyChange(() => IsQuotationViewCase);
            }
        }
        public bool IsQuotationViewCase
        {
            get
            {
                return CurrentViewCase == (short)ViewCase.Quotation;
            }
        }
        #endregion
        #region Events
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetQuotationCollection(CurrentViewCase, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedCollection = contract.EndGetQuotationCollection(asyncResult);
                                if (GettedCollection == null)
                                {
                                    QuotationCollection = new ObservableCollection<InPatientBillingInvoice>();
                                }
                                QuotationCollection = GettedCollection.ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public void gvQuotationCollection_DblClick(object sender, Common.EventArgs<object> e)
        {
            IsHasSelected = true;
            TryClose();
        }
        public void RemoveItemCmd(InPatientBillingInvoice aQuotation, object eventArgs)
        {
            if (aQuotation == null)
            {
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRemoveQuotation(aQuotation.InPatientBillingInvID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndRemoveQuotation(asyncResult);
                                QuotationCollection.Remove(aQuotation);
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public void gvQuotationCollection_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsQuotationViewCase)
            {
                (sender as ReadOnlyDataGrid).GetColumnByName("clPatientCode").Visibility = Visibility.Collapsed;
                (sender as ReadOnlyDataGrid).GetColumnByName("clPatientName").Visibility = Visibility.Collapsed;
                (sender as ReadOnlyDataGrid).GetColumnByName("clHIBenefit").Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}