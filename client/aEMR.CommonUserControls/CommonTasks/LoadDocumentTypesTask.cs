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
    public class LoadDocumentTypesTask:IResult
    {
        public Exception Error { get; private set; }

        private ObservableCollection<Lookup> _documentTypes;

        public ObservableCollection<Lookup> DocumentTypes
        {
            get { return _documentTypes; }
        }

        public LoadDocumentTypesTask()
        {
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

                        contract.BeginGetAllDocumentTypeOnHold(
                                           Globals.DispatchCallback((asyncResult) =>
                                           {
                                               try
                                               {
                                                   var allDocTypes = contract.EndGetAllDocumentTypeOnHold(asyncResult);

                                                   if (allDocTypes != null)
                                                   {
                                                       _documentTypes = new ObservableCollection<Lookup>(allDocTypes);
                                                   }
                                                   else
                                                   {
                                                       _documentTypes = null;
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
