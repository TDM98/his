using System;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;

namespace aEMR.CommonTasks
{
    public class LoadCurrentDateTask:IResult
    {
        private Exception _error;
        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
        }
        public LoadCurrentDateTask()
        {
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                _currentDate = contract.EndGetDate(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                _error = ex;
                            }
                            finally
                            {
                                Completed(this, new ResultCompletionEventArgs
                                {
                                    Error = _error,
                                    WasCancelled = false
                                });
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    _error = ex;
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
