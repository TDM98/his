using System;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;

namespace aEMR.CommonTasks
{
    public class SaveGenMedProductTask:IResult
    {
        public Exception Error { get; private set; }

        private OutwardDrugMedDeptInvoice Invoice { get; set; }
        private OutwardDrugMedDeptInvoice _SavedOutwardInvoice = null;
        public OutwardDrugMedDeptInvoice SavedOutwardInvoice 
        {
            get
            {
                return _SavedOutwardInvoice;
            }
            set
            {
                _SavedOutwardInvoice = value;
            }
        }

        private long StaffID { get; set; }

        public SaveGenMedProductTask(OutwardDrugMedDeptInvoice _Invoice)
        {
            Invoice = _Invoice;
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {                                
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        bool AlreadyShowErrorMessage = false;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveOutwardInvoice(Invoice,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var res = contract.EndSaveOutwardInvoice(out _SavedOutwardInvoice, asyncResult);
                                    if (res && _SavedOutwardInvoice != null)
                                    {
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogError(fault.ToString());
                                    Globals.ShowMessage(fault.ToString(), eHCMSResources.T0432_G1_Error);
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = true
                                    });
                                }
                                catch (Exception ex)
                                {
                                    AlreadyShowErrorMessage = true;
                                    ClientLoggerHelper.LogError(ex.ToString());
                                    //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = true
                                    });
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                    bool bCanceled = false;
                                    if (_SavedOutwardInvoice == null)
                                    {
                                        bCanceled = true;
                                        if (!AlreadyShowErrorMessage)
                                        {
                                            ClientLoggerHelper.LogError(eHCMSResources.Z1719_G1_PhXuatThuocLuuKgThCong);
                                            Globals.ShowMessage(eHCMSResources.Z1583_G1_PhXuatThuocLuuKgThCong, eHCMSResources.T0432_G1_Error);
                                        }
                                    }
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = bCanceled
                                    });
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Error = ex;
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = null,
                        WasCancelled = true
                    });
                }
            });

            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
        
    }
}
