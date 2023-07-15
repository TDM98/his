using eHCMSLanguage;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using System.Windows;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IHISPaymentHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HISPaymentHistoryViewModel : Conductor<object>, IHISPaymentHistory
    {
        #region Properties
        private DateTime _gStartDate = DateTime.Now.AddMonths(-1);
        public DateTime gStartDate
        {
            get
            {
                return _gStartDate;
            }
            set
            {
                _gStartDate = value;
                NotifyOfPropertyChange("gStartDate");
            }
        }
        private DateTime _gEndDate = DateTime.Now;
        public DateTime gEndDate
        {
            get
            {
                return _gEndDate;
            }
            set
            {
                _gEndDate = value;
                NotifyOfPropertyChange("gEndDate");
            }
        }
        private ObservableCollection<HOSPayment> _gHOSPaymentCollection;
        public ObservableCollection<HOSPayment> gHOSPaymentCollection
        {
            get
            {
                return _gHOSPaymentCollection;
            }
            set
            {
                if (_gHOSPaymentCollection != value)
                {
                    _gHOSPaymentCollection = value;
                    NotifyOfPropertyChange("gHOSPaymentCollection");
                }
            }
        }
        #endregion
        #region Events
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public HISPaymentHistoryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            
        }

        public void HISPaymentEdit()
        {
            //var HISPaymentEditVM = Globals.GetViewModel<IHISPaymentEdit>();
            //var instance = HISPaymentEditVM as Conductor<object>;
            //this.ActivateItem(HISPaymentEditVM);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IHISPaymentEdit> onInitDlg = (HISPaymentEditVM) =>
            {
                this.ActivateItem(HISPaymentEditVM);
            };
            GlobalsNAV.ShowDialog<IHISPaymentEdit>(onInitDlg);
        }
        public void SearchCmd()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetHOSPayments(gStartDate, gEndDate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            gHOSPaymentCollection = new ObservableCollection<HOSPayment>(contract.EndGetHOSPayments(asyncResult));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public void hplEdit_Click(object aSelectedItem)
        {
            if (aSelectedItem != null && aSelectedItem is HOSPayment)
            {
                //var HISPaymentEditVM = Globals.GetViewModel<IHISPaymentEdit>();
                //HISPaymentEditVM.gHOSPayment = (aSelectedItem as HOSPayment).EntityDeepCopy();
                //var instance = HISPaymentEditVM as Conductor<object>;
                //this.ActivateItem(HISPaymentEditVM);
                //Globals.ShowDialog(instance, (o) => { });

                Action<IHISPaymentEdit> onInitDlg = (HISPaymentEditVM) =>
                {
                    HISPaymentEditVM.gHOSPayment = (aSelectedItem as HOSPayment).EntityDeepCopy();
                    this.ActivateItem(HISPaymentEditVM);
                };
                GlobalsNAV.ShowDialog<IHISPaymentEdit>(onInitDlg);
            }
        }
        public void hplDelete_Click(object aSelectedItem)
        {
            if (aSelectedItem != null && aSelectedItem is HOSPayment)
            {
                if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    this.ShowBusyIndicator();
                    var t = new Thread(() =>
                    {
                        using (var serviceFactory = new TransactionServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginDeleteHOSPayment((aSelectedItem as HOSPayment).HOSPaymentID, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndDeleteHOSPayment(asyncResult))
                                    {
                                        MessageBox.Show(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        SearchCmd();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                        }
                    });
                    t.Start();
                }
            }
        }
        #endregion
    }
}
