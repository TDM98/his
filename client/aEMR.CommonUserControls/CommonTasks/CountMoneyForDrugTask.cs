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

namespace aEMR.CommonTasks
{
    public class CountMoneyForDrugTask:IResult
    {
        public Exception Error { get; private set; }

        private decimal _AmountPaid;
        public decimal AmountPaid
        {
            get { return _AmountPaid; }
        }

        private readonly long _OutiID;
        public long OutiID
        {
            get
            {
                return _OutiID;
            }
        }

        private readonly long _V_TranRefType;
        public long V_TranRefType
        {
            get
            {
                return _V_TranRefType;
            }
        }
        public CountMoneyForDrugTask(long outiID, long v_TranRefType)
        {
            _OutiID = outiID;
            _V_TranRefType = v_TranRefType;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCountMoneyForDrug(_OutiID,_V_TranRefType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndCountMoneyForDrug(out _AmountPaid, asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                }
                                finally
                                {
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = false
                                    });
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = null,
                        WasCancelled = false
                    });
                }
            });
            t.Start();
        }
     
        public event EventHandler<ResultCompletionEventArgs> Completed;
        
    }
}
