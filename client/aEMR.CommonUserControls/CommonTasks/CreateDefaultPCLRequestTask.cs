using System;
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
    /// <summary>
    /// Tạo PCL Request mặc định (gói CLS) tương ứng với một dịch vụ
    /// </summary>
    public class CreateDefaultPCLRequestTask:IResult
    {
        public Exception Error { get; private set; }
        private long _medServiceID;
        private PatientPCLRequest _pclRequest;
        public PatientPCLRequest PatientPCLRequest
        {
            get { return _pclRequest; }
        }
        private PatientPCLRequest _externalPclRequest;
        public PatientPCLRequest ExternalPclRequest
        {
            get { return _externalPclRequest; }
        }
        private long? _reqFromDeptLocID;
        private long? _pclDeptLocID;
        public CreateDefaultPCLRequestTask(long medServiceID, long? pclDeptLocID = null, long? reqFromDeptLocID = null)
        {
            _medServiceID = medServiceID;
            _pclDeptLocID = pclDeptLocID;
            _reqFromDeptLocID = reqFromDeptLocID;
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

                        contract.BeginCreateNewPCLRequest(_medServiceID,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            PatientPCLRequest item = null;
                            PatientPCLRequest externalRequest;
                            try
                            {
                                contract.EndCreateNewPCLRequest(out item,out externalRequest, asyncResult);
                                _pclRequest = item;
                                _pclRequest.PCLDeptLocID = _pclDeptLocID;
                                _pclRequest.ReqFromDeptLocID = _reqFromDeptLocID;

                                _externalPclRequest = externalRequest;
                                if(_externalPclRequest != null)
                                {
                                    _externalPclRequest.ReqFromDeptLocID = _reqFromDeptLocID;    
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
