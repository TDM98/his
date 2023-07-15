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
using Castle.Core.Logging;
using Castle.Windsor;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;
/*
 * 20180828 #001 TTM: Fix loi khong goi Delagate Action
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICashAdvanceBill)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CashAdvanceBillViewModel : Conductor<object>, ICashAdvanceBill
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CashAdvanceBillViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private ObservableCollection<PatientCashAdvance> _patientCashAdvanceList;

        public ObservableCollection<PatientCashAdvance> PatientCashAdvanceList
        {
            get { return _patientCashAdvanceList; }
            set
            {
                if (_patientCashAdvanceList != value)
                {
                    _patientCashAdvanceList = value;
                    NotifyOfPropertyChange(() => PatientCashAdvanceList);
                }
            }
        }


        public void GetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType)
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetCashAdvanceBill(PtRegistrationID, V_RegistrationType,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var regItem = contract.EndGetCashAdvanceBill(asyncResult);

                                    if (regItem != null)
                                    {
                                        PatientCashAdvanceList = regItem.ToObservableCollection();
                                    }

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
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        public void PrintBillCmd(PatientCashAdvance source, object eventArgs)
        {
            //KMx: Đóng tiền bill in report giống thu tiền tạm ứng (23/12/2014 17:53).
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.ID = source.PtCashAdvanceID;

            //proAlloc.eItem = ReportName.REGISTRATION_CASH_ADVANCE_BILL_INPT;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PaymentID = source.PtCashAdvanceID;
            //proAlloc.FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;

            //proAlloc.eItem = ReportName.PATIENTCASHADVANCE_REPORT;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });
            //▼====== #001
            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.PaymentID = source.PtCashAdvanceID;
                proAlloc.FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;

                proAlloc.eItem = ReportName.PATIENTCASHADVANCE_REPORT;
            };

            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            //▲====== #001
        }

    }

}