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
    public class PrintPclItemsSilently : IResult
    {
        private PatientRegistration _registration;
        private PatientPCLRequest _pclRequest;
        private List<long> _patientPclRequestDetailsList;
        public PrintPclItemsSilently(PatientRegistration registration, PatientPCLRequest pclRequest, List<long> patientPclRequestDetailsList)
        {
            this._pclRequest = pclRequest;
            this._registration = registration;
            this._patientPclRequestDetailsList = patientPclRequestDetailsList;
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
                            contract.BeginGetPclItemsReportInPdfFormat(_registration.Patient.FullName, _registration.Patient.PatientStreetAddress, _registration.Patient.GenderString, "", _pclRequest.Diagnosis, _pclRequest.PatientPCLReqID, _patientPclRequestDetailsList,
                                            Globals.DispatchCallback((asyncResult) =>
                                            {
                                                try
                                                {
                                                    var data = contract.EndGetPclItemsReportInPdfFormat(asyncResult);
                                                    var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, data, ActiveXPrintType.ByteArray);
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
