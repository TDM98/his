using System;
using System.Collections.Generic;
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
    public class PCLRequest_ViewResults_SearchTask:IResult
    {
        public Exception Error { get; private set; }

        private readonly PatientPCLRequestSearchCriteria _searchCriteria;
        private readonly int _pageIndex;
        private readonly int _pageSize;
        private readonly bool _countTotal;
        public bool CountTotal
        {
            get
            {
                return _countTotal;
            }
        }

        public int TotalItemCount { get; set; }
        private IList<PatientPCLRequest> _PatientPclRequestList;
        public IList<PatientPCLRequest> PatientPclRequestList
        {
            get
            {
                return _PatientPclRequestList;
            }
          
        }
        public PCLRequest_ViewResults_SearchTask(PatientPCLRequestSearchCriteria searchCriteria, int pageIndex, int pageSize, bool countTotal)
        {
            _pageIndex = pageIndex;
            _pageSize = pageSize;
            _searchCriteria = searchCriteria;
            _countTotal = countTotal;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPatientPCLRequest_ViewResult_SearchPagingNew(_searchCriteria, _pageIndex, _pageSize, "", _countTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            bool bOK = false;

                            try
                            {
                                _PatientPclRequestList = client.EndPatientPCLRequest_ViewResult_SearchPagingNew(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (Exception innerEx)
                            {
                                Error = innerEx;
                            }
                            finally
                            {
                                if (bOK)
                                {
                                    if (_countTotal)
                                    {
                                        TotalItemCount = Total;
                                    }
                                }
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
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
