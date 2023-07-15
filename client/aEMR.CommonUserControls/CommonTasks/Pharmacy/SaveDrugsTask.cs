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
using eHCMSLanguage;
using aEMR.Common;
using System.Collections.Generic;
using System.Linq;

namespace aEMR.CommonTasks
{
    public class SaveDrugsTask : IResult
    {
        public Exception Error { get; private set; }

        private OutwardDrugInvoice Invoice { get; set; }
        private OutwardDrugInvoice _SavedOutwardInvoice = null;
        public OutwardDrugInvoice SavedOutwardInvoice
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

        public List<OutwardDrug> SavedOutwardDrugs { get; set; }

        private long StaffID { get; set; }

        public SaveDrugsTask(OutwardDrugInvoice _Invoice, long _StaffID)
        {
            Invoice = _Invoice;
            StaffID = _StaffID;
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    //using (var serviceFactory = new CommonServiceClient())
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        bool AlreadyShowErrorMessage = false;
                        var contract = serviceFactory.ServiceInstance;
                        //contract.BeginSaveDrugs(StaffID, Globals.DeptLocation.DeptLocationID, null, Invoice,
                        //Globals.DispatchCallback((asyncResult) =>
                        contract.BeginSaveDrugs_Pst_V2(StaffID, Globals.DeptLocation.DeptLocationID, null, Invoice,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    IList<OutwardDrug> mSavedOutwardDrugs = new List<OutwardDrug>();
                                    //var res = contract.EndSaveDrugs(out _SavedOutwardInvoice, asyncResult);
                                    var res = contract.EndSaveDrugs_Pst_V2(out _SavedOutwardInvoice, out mSavedOutwardDrugs, asyncResult);
                                    SavedOutwardDrugs = mSavedOutwardDrugs == null ? null : mSavedOutwardDrugs.ToList();
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogError(fault.ToString());
                                    Globals.ShowMessage(fault.ToString(), eHCMSResources.T0432_G1_Error);
                                    if (Completed != null)
                                    {
                                        Completed(this, new ResultCompletionEventArgs
                                        {
                                            Error = null,
                                            WasCancelled = true
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AlreadyShowErrorMessage = true;
                                    ClientLoggerHelper.LogError(ex.ToString());
                                    //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    if (Completed != null)
                                    {
                                        Completed(this, new ResultCompletionEventArgs
                                        {
                                            Error = null,
                                            WasCancelled = true
                                        });
                                    }
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
                                    if (Completed != null)
                                    {
                                        Completed(this, new ResultCompletionEventArgs
                                        {
                                            Error = null,
                                            WasCancelled = bCanceled
                                        });
                                    }
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
                    if (Completed != null)
                    {
                        Completed(this, new ResultCompletionEventArgs
                        {
                            Error = null,
                            WasCancelled = true
                        });
                    }
                }
            });

            t.Start();
        }
        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}