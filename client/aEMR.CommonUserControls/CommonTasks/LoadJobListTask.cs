using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;

namespace aEMR.CommonTasks
{
    public class LoadJobListTask : IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefJob> _RefJobList;
        public ObservableCollection<RefJob> RefJobList
        {
            get { return _RefJobList; }
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllJobs(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllJobs(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allJob130 = allItems.ToList();
                                    _RefJobList = Globals.allJob130.ToObservableCollection();
                                }
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
                                this.HideBusyIndicator();
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
