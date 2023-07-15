
using System;
using System.Threading;
using System.Windows;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using DataEntities;
using eHCMSLanguage;

namespace aEMR.CommonTasks
{
    public static class ExportToExcelGeneric
    {
        public static Exception Error { get; private set; }
        static ReportParameters RptParameters;
        static Microsoft.Win32.SaveFileDialog objSFD { get; set; }
        static object VM;
        private static SeachPtRegistrationCriteria SeachPtRegistrationCriteria { get; set; }
        public static void Action(ReportParameters _RptParameters, Microsoft.Win32.SaveFileDialog _objSFD, object _vm)
        {
            VM = _vm;
            objSFD = _objSFD;
            RptParameters = _RptParameters;
            string newGuid = Guid.NewGuid().ToString();
            RptParameters.StoreName = newGuid;
            RptParameters.ShowTitle = objSFD.SafeFileName;
            if (_RptParameters.IsExportToCSV)
            {
                ExportToCSV();
            }
            else
            {
                if (Globals.ServerConfigSection.CommonItems.ApplyNewFuncExportExcel 
                    && RptParameters.reportName != ReportName.OutwardDrugsByStaffStatistic 
                    && RptParameters.reportName != ReportName.OutwardDrugsByStaffStatisticDetails 
                    && RptParameters.reportName != ReportName.OutwardDrugClinicDeptsByStaffStatisticDetails_TP)
                {
                    ExportToExcel_New(objSFD.FileName);
                }
                else
                {
                    ExportToExcel();
                }
            }
        }
        public static void Action(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, Microsoft.Win32.SaveFileDialog _objSFD, object _vm)
        {
            VM = _vm;
            objSFD = _objSFD;
            SeachPtRegistrationCriteria = aSeachPtRegistrationCriteria.EntityDeepCopy();
            SeachPtRegistrationCriteria.IsExportData = true;
            SeachPtRegistrationCriteria.SafeFileName = objSFD.SafeFileName;
            if (SeachPtRegistrationCriteria.IsHasInvoice)
            {
                ExportToExcelEInvoiceData();
            }
            else
            {
                ExportToExcelRegistrationsData();
            }
        }
        public static void ExportHospitalClientContractExcel(long HosClientContractID, Microsoft.Win32.SaveFileDialog _objSFD, object _vm)
        {
            VM = _vm;
            objSFD = _objSFD;
            ExportHospitalClientContractDetails_Excel(HosClientContractID);
        }
        public static void ExportHospitalClientContract_ReulstExcel(long HosClientContractID, Microsoft.Win32.SaveFileDialog _objSFD, object _vm)
        {
            VM = _vm;
            objSFD = _objSFD;
            ExportHospitalClientContractResultDetails_Excel(HosClientContractID);
        }
        public static void ExportToExcel_New(string filePath)
        {
            VM.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportToExcellAllGeneric_New(RptParameters, filePath, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportToExcellAllGeneric_New(asyncResult);
                                if (results != null)
                                {
                                    SaveStreamToExcel(results, objSFD);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                VM.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    VM.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public static void ExportToExcel()
        {
            VM.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportToExcellAllGeneric(RptParameters, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportToExcellAllGeneric(asyncResult);
                                SaveStreamToExcel(results, objSFD);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                VM.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    VM.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public static void ExportToCSV()
        {
            VM.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportToCSVAllGeneric(RptParameters, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportToCSVAllGeneric(asyncResult);
                                SaveStreamToExcel(results, objSFD);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                VM.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    VM.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public static void ExportToExcelEInvoiceData()
        {
            VM.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportEInvoiceToExcel(SeachPtRegistrationCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportEInvoiceToExcel(asyncResult);
                                SaveStreamToExcel(results, objSFD);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                VM.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    VM.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public static void ExportToExcelRegistrationsData()
        {
            VM.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportRegistrationsData(SeachPtRegistrationCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportRegistrationsData(asyncResult);
                                SaveStreamToExcel(results, objSFD);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                VM.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    VM.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public static void ExportHospitalClientContractDetails_Excel(long HosClientContractID)
        {
            VM.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportHospitalClientContractDetails_Excel(HosClientContractID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportHospitalClientContractDetails_Excel(asyncResult);
                                SaveStreamToExcel(results, objSFD);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                VM.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    VM.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public static void ExportHospitalClientContractResultDetails_Excel(long HosClientContractID)
        {
            VM.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportHospitalClientContractResultDetails_Excel(HosClientContractID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportHospitalClientContractResultDetails_Excel(asyncResult);
                                SaveStreamToExcel(results, objSFD);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                VM.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    VM.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private static void SaveStreamToExcel(byte[] Result, Microsoft.Win32.SaveFileDialog saveFileDialog)
        {
            if (Result != null)
            {
                var myStream = saveFileDialog.OpenFile();
                myStream.Write(Result, 0, Result.Length);
                myStream.Close();
                myStream.Dispose();
                MessageBox.Show(eHCMSResources.A0804_G1_Msg_InfoLuuOK);
            }

        }
    }

    //KMx: A.Tuấn yêu cầu chuyển task thành static class (07/08/2014 11:10).
    //public class ExportToExcellAllGenericTask : IResult
    //{
    //    public Exception Error { get; private set; }
    //    ReportParameters RptParameters;
    //    SaveFileDialog objSFD { get; set; }
    //    public ExportToExcellAllGenericTask(ReportParameters _RptParameters, SaveFileDialog _objSFD)
    //    {
    //        objSFD = _objSFD;
    //        RptParameters = _RptParameters;
    //        string newGuid = Guid.NewGuid().ToString();
    //        RptParameters.StoreName = newGuid;
    //        RptParameters.ShowTitle = objSFD.SafeFileName;
    //    }

    //    public void Execute(ActionExecutionContext context)
    //    {
    //        this.ShowBusyIndicator();
    //        var t = new Thread(() =>
    //        {
    //            try
    //            {
    //                using (var serviceFactory = new TransactionServiceClient())
    //                {
    //                    var contract = serviceFactory.ServiceInstance;
    //                    bool bHasError = false;
    //                    contract.BeginExportToExcellAllGeneric(RptParameters,
    //                        Globals.DispatchCallback((asyncResult) =>
    //                        {
    //                            try
    //                            {
    //                                var results = contract.EndExportToExcellAllGeneric(asyncResult);
    //                                SaveStreamToExcel(results, objSFD);
    //                            }
    //                            catch (Exception ex)
    //                            {
    //                                Error = ex;
    //                                bHasError = true;
    //                                MessageBox.Show(ex.Message);
    //                            }
    //                            finally
    //                            {                                    
    //                                Completed(this, new ResultCompletionEventArgs
    //                                {
    //                                    Error = null,
    //                                    WasCancelled = bHasError
    //                                });
    //                                this.HideBusyIndicator();
    //                            }

    //                        }), null);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                MessageBox.Show(ex.Message);
    //                Error = ex;
    //                Completed(this, new ResultCompletionEventArgs
    //                {
    //                    Error = null,
    //                    WasCancelled = true
    //                });
    //                this.HideBusyIndicator();
    //            }
    //        });
    //        t.Start();
    //    }
    //    private void SaveStreamToExcel(byte[] Result, SaveFileDialog saveFileDialog)
    //    {
    //        if (Result != null)
    //        {
    //            var myStream = saveFileDialog.OpenFile();
    //            myStream.Write(Result, 0, Result.Length);
    //            myStream.Close();
    //            myStream.Dispose();
    //            MessageBox.Show(eHCMSResources.A0804_G1_Msg_InfoLuuOK);                
    //        }

    //    }
    //    public event EventHandler<ResultCompletionEventArgs> Completed;
    //}
}
