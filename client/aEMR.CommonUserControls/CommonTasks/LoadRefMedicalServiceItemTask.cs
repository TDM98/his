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
using aEMR.Common;
namespace aEMR.CommonTasks
{
    public class LoadRefMedicalServiceItemTask:IResult
    {
        public Exception Error { get; private set; }

        public RefMedicalServiceType RefMedServiceType;

        private ObservableCollection<RefMedicalServiceItem> _RefMedicalServiceItems;

        public ObservableCollection<RefMedicalServiceItem> RefMedicalServiceItems
        {
            get { return _RefMedicalServiceItems; }
        }

        public LoadRefMedicalServiceItemTask(RefMedicalServiceType _RefMedServiceType)
        {
            RefMedServiceType = _RefMedServiceType;
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
                        long? serviceTypeID = null;
                        if (RefMedServiceType != null)
                        {
                            serviceTypeID = RefMedServiceType.MedicalServiceTypeID;
                        }
                        contract.BeginGetAllMedicalServiceItemsByType(serviceTypeID, null,null,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<RefMedicalServiceItem> allItem = new ObservableCollection<RefMedicalServiceItem>();
                                try
                                {
                                    _RefMedicalServiceItems =new ObservableCollection<RefMedicalServiceItem>(contract.EndGetAllMedicalServiceItemsByType(asyncResult));                                    
                                }
                                catch (Exception ex)
                                {
                                    _RefMedicalServiceItems = null;
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
                            }), RefMedServiceType);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
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
