using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;

namespace aEMR.CommonTasks
{
    public class SearchGetDrugForSellVisitorByPrescriptIDTask:IResult
    {
        public Exception Error { get; private set; }

        public ObservableCollection<GetDrugForSellVisitor> DrugForSellVisitorList { get; set; }

        private long? PrescriptID = null;
        private byte HI;
        private bool IsHIPatient = false;
        private long StoreID;

        public SearchGetDrugForSellVisitorByPrescriptIDTask(long? _PrescriptID , byte _HI, bool _IsHIPatient, long _StoreID)
        {
            PrescriptID = _PrescriptID;
            HI = _HI;
            IsHIPatient = _IsHIPatient;
            StoreID = _StoreID;         
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        bool bCanceled = false;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugForSellVisitorAutoComplete_ForPrescriptionByID(HI, IsHIPatient, StoreID, PrescriptID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var resList = contract.EndGetDrugForSellVisitorAutoComplete_ForPrescriptionByID(asyncResult);
                                    if (resList != null)
                                    {
                                        DrugForSellVisitorList = resList.ToObservableCollection();                                    
                                    }
                                    else
                                    {
                                        DrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    bCanceled = true;
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
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
                    this.HideBusyIndicator();
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
