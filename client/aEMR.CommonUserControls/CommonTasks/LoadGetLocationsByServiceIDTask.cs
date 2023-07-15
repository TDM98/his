using System;
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
    public class LoadGetLocationsByServiceIDTask:IResult
    {
        public Exception Error { get; private set; }

        private ObservableCollection<DeptLocation> _DeptLocations;

        public ObservableCollection<DeptLocation> DeptLocations
        {
            get { return _DeptLocations; }
        }

        public long MedServiceID;

        public LoadGetLocationsByServiceIDTask(long _MedServiceID)
        {
            MedServiceID = _MedServiceID;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetLocationsByServiceID(MedServiceID,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    _DeptLocations =new ObservableCollection<DeptLocation>(contract.EndGetLocationsByServiceID(asyncResult));
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
