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
    public class PCLExamTypeServiceTarget_CheckedTask : IResult
    {
        public Exception Error { get; private set; }
        private bool _Result;
        public bool Result
        {
            get { return _Result; }
            set { _Result = value; }
        }

        private long _PCLExamTypeID;
        private DateTime _Date;
        public PCLExamTypeServiceTarget_CheckedTask(long PCLExamTypeID, DateTime Date)
        {
            _PCLExamTypeID = PCLExamTypeID;
            _Date = Date;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPCLExamTypeServiceTarget_Checked(_PCLExamTypeID, _Date, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefOutputType> allItems = new ObservableCollection<RefOutputType>();
                            try
                            {
                                bool result = contract.EndPCLExamTypeServiceTarget_Checked(asyncResult);
                                Result = result;
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

    public class PCLExamTypeServiceTarget_CheckedAppointmentTask : IResult
    {
        public Exception Error { get; private set; }
        private bool _Result;
        public bool Result
        {
            get { return _Result; }
            set { _Result = value; }
        }

        private long _PCLExamTypeID;
        private DateTime _Date;
        public PCLExamTypeServiceTarget_CheckedAppointmentTask(long PCLExamTypeID, DateTime Date)
        {
            _PCLExamTypeID = PCLExamTypeID;
            _Date = Date;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPCLExamTypeServiceTarget_Checked_Appointment(_PCLExamTypeID, _Date, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefOutputType> allItems = new ObservableCollection<RefOutputType>();
                            try
                            {
                                bool result = contract.EndPCLExamTypeServiceTarget_Checked_Appointment(asyncResult);
                                Result = result;
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
