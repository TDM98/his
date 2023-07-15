using eHCMSLanguage;
using System;
using System.Threading;
using System.Windows;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;



namespace aEMR.CommonTasks
{
    public class ExportToExcelAllItemsPriceListTask : IResult
    {
        public Exception Error { get; private set; }
        long PriceListID { get; set; }
        int PriceListType { get; set; }
        string StoreName { get; set; }
        string ShowTitle { get; set; }
        Microsoft.Win32.SaveFileDialog objSFD { get; set; }
        public ExportToExcelAllItemsPriceListTask(long _PriceListID, int _PriceListType, Microsoft.Win32.SaveFileDialog _objSFD)
        {
            objSFD = _objSFD;
            PriceListID = _PriceListID;
            PriceListType = _PriceListType;
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
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bHasError = false;
                        contract.BeginExportToExcelAllItemsPriceList(PriceListID, PriceListType, StoreName, ShowTitle,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndExportToExcelAllItemsPriceList(asyncResult);
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
