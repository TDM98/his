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
/*
 * 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
 *                      gọi về Service tốn thời gian.
 */
namespace aEMR.CommonTasks
{
    public class LoadLookupBehavingTask:IResult
    {
        public Exception Error { get; private set; }

        private ObservableCollection<Lookup> _RefBehaving;

        public ObservableCollection<Lookup> RefBehaving
        {
            get { return _RefBehaving; }
        }

        public LoadLookupBehavingTask()
        {
        }

        public void Execute(ActionExecutionContext context)
        {
            //▼====== #001
            _RefBehaving = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.BEHAVING))
                {
                    _RefBehaving.Add(tmpLookup);
                }
            }
            //▲====== #001
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ePMRsServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetLookupBehaving(Globals.DispatchCallback((asyncResult) =>
        //                {

        //                    try
        //                    {
        //                        var results = contract.EndGetLookupBehaving(asyncResult);
        //                        if (results != null)
        //                            {
        //                                _RefBehaving = new ObservableCollection<Lookup>(results);
        //                            }
        //                            else
        //                            {
        //                                _RefBehaving = null;
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Error = ex;
        //                        }
        //                        finally
        //                        {
        //                            Completed(this, new ResultCompletionEventArgs
        //                            {
        //                                Error = null,
        //                                WasCancelled = false
        //                            });
        //                        }
        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Error = ex;
        //            Completed(this, new ResultCompletionEventArgs
        //            {
        //                Error = null,
        //                WasCancelled = false
        //            });
        //        }
        //    });
        //    t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
