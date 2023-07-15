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
using System.Collections.Generic;

namespace aEMR.CommonTasks
{
    public class LoadRefMedicalServiceTypeTask:IResult
    {
        public Exception Error { get; private set; }

        private ObservableCollection<RefMedicalServiceType> _RefMedicalServiceTypes;

        public ObservableCollection<RefMedicalServiceType> RefMedicalServiceTypes
        {
            get { return _RefMedicalServiceTypes; }
        }

        public LoadRefMedicalServiceTypeTask()
        {
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
                        var outTypes = new List<long>
                                           {
                                               (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NGOAITRU,
                                               (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU_NGOAITRU,
                                               //(long) AllLookupValues.V_RefMedicalServiceInOutOthers.HANHCHANH_NGOAITRU
                                           };
                        contract.BeginGetMedicalServiceTypesByInOutType(outTypes,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    _RefMedicalServiceTypes = new ObservableCollection<RefMedicalServiceType>
                                        (contract.EndGetMedicalServiceTypesByInOutType(asyncResult));
                                }
                                catch (Exception ex)
                                {
                                    _RefMedicalServiceTypes = null;
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
                finally
                {
                    
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
