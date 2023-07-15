using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Printing;
using aEMR.Common;


namespace aEMR.CommonTasks
{
    public class PrintPatientCashAdvanceSilently : IResult
    {
        private long _PaymentID;
        private int _FindPatient;
        public PrintPatientCashAdvanceSilently(long paymentID,int findPatient)
        {
            this._PaymentID = paymentID;
            this._FindPatient = findPatient;
        }
        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                Exception serviceCallEx = null;
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        try
                        {
                            contract.BeginGetPatientCashAdvanceReportInPdfFormat(_PaymentID,_FindPatient,
                                            Globals.DispatchCallback((asyncResult) =>
                                            {
                                                try
                                                {
                                                    var data = contract.EndGetPatientCashAdvanceReportInPdfFormat(asyncResult);
                                                    var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_HOA_DON, data, ActiveXPrintType.ByteArray);
                                                    Globals.EventAggregator.Publish(printEvt);
                                                }
                                                catch (Exception ex)
                                                {
                                                    serviceCallEx = ex;
                                                }

                                                Completed(this, new ResultCompletionEventArgs
                                                {
                                                    Error = serviceCallEx,
                                                    WasCancelled = false
                                                });

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
                    serviceCallEx = ex;
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = serviceCallEx,
                        WasCancelled = false
                    });
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
