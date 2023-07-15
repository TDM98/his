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
using System.IO;

namespace aEMR.CommonTasks
{
    public class LoadPatientPCLImagingResults_ByIDTask:IResult
    {
        public Exception Error { get; private set; }

        private long _ptPCLReqID;
        private long _PCLExamTypeID;
        /*▼====: #001*/
        private long V_PCLRequestType = 25001;
        /*▲====: #001*/

        private PatientPCLImagingResult _ObjPatientPCLImagingResult;

        public PatientPCLImagingResult ObjPatientPCLImagingResult
        {
            get { return _ObjPatientPCLImagingResult; }
            set
            {
                if (_ObjPatientPCLImagingResult != value)
                {
                    _ObjPatientPCLImagingResult = value;
                }
            }
        }

        public LoadPatientPCLImagingResults_ByIDTask(long ReqID, long ExamTypeID, long V_PCLRequestType = 25001)
        {
            _ptPCLReqID = ReqID;
            _PCLExamTypeID = ExamTypeID;
            /*▼====: #001*/
            this.V_PCLRequestType = V_PCLRequestType == 0 ? 25001 : V_PCLRequestType;
            /*▲====: #001*/
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsImportClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        /*▼====: #001*/
                        //contract.BeginGetPatientPCLImagingResults_ByID(_ptPCLReqID,_PCLExamTypeID, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginGetPatientPCLImagingResults_ByID_V2(_ptPCLReqID, _PCLExamTypeID, V_PCLRequestType, Globals.DispatchCallback((asyncResult) =>
                        /*▲====: #001*/
                        {
                            try
                            {
                                ObjPatientPCLImagingResult = contract.EndGetPatientPCLImagingResults_ByID_V2(asyncResult);
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

    public class LoadPatientPCLImagingResults_ByIDExtTask : IResult
    {
        public Exception Error { get; private set; }

        private PCLResultFileStorageDetailSearchCriteria p;
        private PatientPCLImagingResult _ObjPatientPCLImagingResult;

        public PatientPCLImagingResult ObjPatientPCLImagingResult
        {
            get { return _ObjPatientPCLImagingResult; }
            set
            {
                if (_ObjPatientPCLImagingResult != value)
                {
                    _ObjPatientPCLImagingResult = value;
                }
            }
        }

        public LoadPatientPCLImagingResults_ByIDExtTask(  PCLResultFileStorageDetailSearchCriteria _p)
        {
            p=_p;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsImportClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientPCLImagingResults_ByIDExt(p,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    ObjPatientPCLImagingResult = contract.EndGetPatientPCLImagingResults_ByIDExt(asyncResult);
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

    public class LoadPCLResultFileStoreDetailsTask : IResult
    {
        public Exception Error { get; private set; }
        private long _PatientID;
        private long? _PCLRequestID;
        private long? _PCLExamTypeID;
        private long V_PCLRequestType = 25001;

        private IList<PCLResultFileStorageDetail> _ObjGetPCLResultFileStoreDetails;
        public IList<PCLResultFileStorageDetail> ObjGetPCLResultFileStoreDetails
        {
            get { return _ObjGetPCLResultFileStoreDetails; }
            set
            {
                _ObjGetPCLResultFileStoreDetails = value;
            }
        }

        public LoadPCLResultFileStoreDetailsTask(long patientID, long? pclRequestID, long? PCLExamTypeID, long V_PCLRequestType = 25001)
        {
            _PatientID = patientID;
            _PCLRequestID = pclRequestID;
            _PCLExamTypeID = PCLExamTypeID;
            /*▼====: #001*/
            this.V_PCLRequestType = V_PCLRequestType == 0 ? 25001 : V_PCLRequestType;
            /*▲====: #001*/
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
                        /*▼====: #001*/
                        //contract.BeginGetPCLResultFileStoreDetails(_PatientID,_PCLRequestID,null,_PCLExamTypeID
                        contract.BeginGetPCLResultFileStoreDetails_V2(_PatientID,_PCLRequestID,null,_PCLExamTypeID, V_PCLRequestType,
                        /*▲====: #001*/
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    ObjGetPCLResultFileStoreDetails = contract.EndGetPCLResultFileStoreDetails_V2(asyncResult);
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

    public class LoadPCLResultFileStoreDetailsExtTask : IResult
    {
        public Exception Error { get; private set; }
        private PCLResultFileStorageDetailSearchCriteria p;
        
        private IList<PCLResultFileStorageDetail> _ObjGetPCLResultFileStoreDetails;
        public IList<PCLResultFileStorageDetail> ObjGetPCLResultFileStoreDetails
        {
            get { return _ObjGetPCLResultFileStoreDetails; }
            set
            {
                _ObjGetPCLResultFileStoreDetails = value;
            }
        }

        public LoadPCLResultFileStoreDetailsExtTask(PCLResultFileStorageDetailSearchCriteria _p)
        {
            p = _p;
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

                        contract.BeginGetPCLResultFileStoreDetailsExt(p,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    ObjGetPCLResultFileStoreDetails = contract.EndGetPCLResultFileStoreDetailsExt(asyncResult);
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
