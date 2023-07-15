using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Threading;
using System.Windows;
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
using aEMR.Common;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPaymentUpdateNote)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentUpdateNoteViewModel : Conductor<object>, IPaymentUpdateNote
    {
        #region Properties

        private PatientTransactionPayment _CurPaymentInfo;
        public PatientTransactionPayment CurPaymentInfo
        {
            get
            {
                return _CurPaymentInfo;
            }
            set
            {
                if (_CurPaymentInfo != value)
                {
                    _CurPaymentInfo = value;
                    NotifyOfPropertyChange(()=>CurPaymentInfo);
                }
            }

        }
        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PaymentUpdateNoteViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }

        public void btnSave()
        {
            if (string.IsNullOrEmpty(CurPaymentInfo.ManualReceiptNumber))
            {
                MessageBox.Show(eHCMSResources.A0887_G1_Msg_InfoNhapBienLaiHuy);
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        try
                        {
                            contract.BeginPatientTransactionPayment_UpdateID(CurPaymentInfo,
                                            Globals.DispatchCallback(asyncResult =>
                                            {
                                                try
                                                {
                                                    var data = contract.EndPatientTransactionPayment_UpdateID(asyncResult);
                                                    TryClose();
                                                    //phat su kien de load lai bill
                                                    Globals.EventAggregator.Publish(new TranPaymentUpdateEvent { TransactionID=CurPaymentInfo.TransactionID.GetValueOrDefault(0)});
                                                }
                                                catch (Exception ex)
                                                {
                                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                                    MessageBox.Show(eHCMSResources.A0694_G1_Msg_InfoKhTheLayDataInHD);
                                                }
                                            }), null);
                        }
                        catch (Exception innerEx)
                        {
                            ClientLoggerHelper.LogInfo(innerEx.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        public void btnClose()
        {
            TryClose();
        }
    }
}
