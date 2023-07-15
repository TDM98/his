using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadPatientPaymentAccountListTask:IResult
    {
        public Exception Error { get; private set; }
        private ObservableCollection<PatientPaymentAccount> _PatientPaymentAccountList;
        public ObservableCollection<PatientPaymentAccount> PatientPaymentAccountList
        {
            get { return _PatientPaymentAccountList; }
        }
        public LoadPatientPaymentAccountListTask()
        {
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    this.ShowBusyIndicator();
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllPatientPaymentAccounts(
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PatientPaymentAccount> allItems = new ObservableCollection<PatientPaymentAccount>();
                            try
                            {
                                allItems = contract.EndGetAllPatientPaymentAccounts(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                _PatientPaymentAccountList = new ObservableCollection<PatientPaymentAccount>(allItems);
                           
                                Completed(this, new ResultCompletionEventArgs
                                {
                                    Error = null,
                                    WasCancelled = false
                                });

                                this.HideBusyIndicator();
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
