using System;
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
    public class PrintRegisteredServiceSilently : IResult
    {
        private PatientRegistration _registration;
        private PatientRegistrationDetail _regDetails;
        public PrintRegisteredServiceSilently(PatientRegistration registration, PatientRegistrationDetail regDetails)
        {
            this._regDetails = regDetails;
            this._registration = registration;
        }
        public void Execute(ActionExecutionContext context)
        {
            //Goi ham in report XRptRegisteredService
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
                            contract.BeginGetRegisteredServiceReportInPdfFormat(_registration.Patient.FullName, _registration.Patient.PatientStreetAddress, _registration.Patient.GenderString,
                                            _regDetails.DeptLocation.Location.LocationName, _registration.PtRegistrationCode, _regDetails.RefMedicalServiceItem.MedServiceName
                                            ,_regDetails.ServiceSeqNumString,
                                            Globals.DispatchCallback((asyncResult) =>
                                            {
                                                var data = contract.EndGetRegisteredServiceReportInPdfFormat(asyncResult);

                                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, data, ActiveXPrintType.ByteArray);
                                                Globals.EventAggregator.Publish(printEvt);

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
