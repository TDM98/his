using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;

namespace aEMR.CommonTasks
{
    public class OutwardDrug_GetMaxDayBuyInsuranceTask:IResult
    {
        public Exception Error { get; private set; }

        public DateTime NgayBHGanNhat { get; set; }

        private long PatientID { get; set; }
        private long outiID { get; set; }

        public OutwardDrug_GetMaxDayBuyInsuranceTask(long _PatientID, long _outiID)
        {
            PatientID=PatientID;
            outiID = _outiID;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    bool bCanceled = false;
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrug_GetMaxDayBuyInsurance(PatientID, outiID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    NgayBHGanNhat = contract.EndOutwardDrug_GetMaxDayBuyInsurance(asyncResult).GetValueOrDefault();                                    
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogError(fault.ToString());
                                    bCanceled = true;
                                }
                                finally
                                {
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = bCanceled
                                    });
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Error = ex;
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = null,
                        WasCancelled = true
                    });
                }
            });

            t.Start();
        }


        public event EventHandler<ResultCompletionEventArgs> Completed;
        
    }
}
