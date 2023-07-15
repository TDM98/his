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
    public class LoadMedRecTemplateTask : IResult
    {
        public Exception Error { get; private set; }

        private ObservableCollection<MedicalRecordTemplate> _RefMedRecTemplate;

        public ObservableCollection<MedicalRecordTemplate> RefMedRecTemplate
        {
            get { return _RefMedRecTemplate; }
        }

        public LoadMedRecTemplateTask()
        {
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllMedRecTemplates(Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetAllMedRecTemplates(asyncResult);
                                    if (results != null)
                                    {
                                        _RefMedRecTemplate = new ObservableCollection<MedicalRecordTemplate>(results);
                                    }
                                    else
                                    {
                                        _RefMedRecTemplate = null;
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
