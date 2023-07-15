
using System;
using System.Threading;
using System.Windows;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;

namespace aEMR.CommonTasks
{
    public class ExportToExcelBangKeChiTietKhamBenhTask : IResult
    {
        public Exception Error { get; private set; }
        SeachPtRegistrationCriteria SearchCriteria;
        string StoreName { get; set; }
        string ShowTitle { get; set; }
        Microsoft.Win32.SaveFileDialog objSFD { get; set; }
        public ExportToExcelBangKeChiTietKhamBenhTask(SeachPtRegistrationCriteria _SearchCriteria, Microsoft.Win32.SaveFileDialog _objSFD)
        {
            objSFD = _objSFD;
            SearchCriteria = _SearchCriteria;
            StoreName = Guid.NewGuid().ToString();
            ShowTitle = objSFD.SafeFileName;
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            { 
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bHasError = false;
                        contract.BeginExportToExcelBangKeChiTietKhamBenh(SearchCriteria, StoreName, ShowTitle,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndExportToExcelBangKeChiTietKhamBenh(asyncResult);
                                    SaveStreamToExcel(results, objSFD);
                                }
                                catch (Exception ex)
                                {
                                    Error = ex;
                                    bHasError = true;
                                }
                                finally
                                {
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = bHasError
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
                        WasCancelled = true
                    });
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void SaveStreamToExcel(byte[] Result, Microsoft.Win32.SaveFileDialog saveFileDialog)
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
        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
