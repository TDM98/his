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
    public class RefDepartmentReqCashAdv_DeptIDTask : IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<RefDepartmentReqCashAdv> _RefDepartmentReqCashAdvList;
        public ObservableCollection<RefDepartmentReqCashAdv> RefDepartmentReqCashAdvList
        {
            get { return _RefDepartmentReqCashAdvList; }
        }

        private long _DeptID;
        public RefDepartmentReqCashAdv_DeptIDTask(long DeptID)
        {
            _DeptID = DeptID;
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

                        contract.BeginRefDepartmentReqCashAdv_DeptID(_DeptID,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefDepartmentReqCashAdv> allItems = new ObservableCollection<RefDepartmentReqCashAdv>();
                            try
                            {
                                allItems = contract.EndRefDepartmentReqCashAdv_DeptID(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _RefDepartmentReqCashAdvList = new ObservableCollection<RefDepartmentReqCashAdv>(allItems);

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
