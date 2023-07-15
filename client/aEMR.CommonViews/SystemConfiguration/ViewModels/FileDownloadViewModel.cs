using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using Caliburn.Micro;
using eHCMSCommon.Utilities;
using aEMR.Common;
using aEMR.ViewContracts;
using Microsoft.Win32;

namespace aEMR.SystemConfiguration.ViewModels
{
    [Export(typeof(IFileDownload)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FileDownloadViewModel : Conductor<object>, IFileDownload
    {
        private int _progressPercentage;
        public int ProgressPercentage
        {
            get
            {
                return _progressPercentage;
            }
            private set
            {
                _progressPercentage = value;
                NotifyOfPropertyChange(() => ProgressPercentage);
            }
        }

        private string _downloadStatus = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2882_G1_DangTaiDLieu);
        public string DownloadStatus
        {
            get
            {
                return _downloadStatus;
            }
            set
            {
                _downloadStatus = value;
                NotifyOfPropertyChange(() => DownloadStatus);
            }
        }
        
        private bool _showDownloadProgressPanel;
        public bool ShowDownloadProgressPanel
        {
            get
            {
                return _showDownloadProgressPanel;
            }
            set
            {
                _showDownloadProgressPanel = value;
                NotifyOfPropertyChange(() => ShowDownloadProgressPanel);
            }
        }
        
        public void DownloadCmd()
        {
            Download();
        }
        public void Download()
        {
            var client = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.OpenReadCompleted += client_OpenReadCompleted;

            string root = Helpers.GetRootPath();
            string printServerPath = root + "printing.cab";
            try
            {
                ProgressPercentage = 0;
                ShowDownloadProgressPanel = true;
                client.OpenReadAsync(new Uri(printServerPath, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }
        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
           //Fire event here
            ProgressPercentage = 100;

            if (e.Error == null)
            {
                DownloadStatus = string.Format("{0}.", eHCMSResources.Z1153_G1_Completed);
                data = new byte[e.Result.Length];
                e.Result.Read(data, 0, (int)e.Result.Length);

                //if (App.Current.IsRunningOutOfBrowser)
                //{
                //    if (App.Current.HasElevatedPermissions)
                //    {
                //        SaveCmd();
                //    }
                //}
                SaveCmd();
            }
            else
            {
                DownloadStatus = string.Format("{0}.", eHCMSResources.Z1154_G1_Failed);
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }
        private byte[] data = null;
        public void SaveCmd()
        {
            try
            {
                var sd = new SaveFileDialog();

                sd.DefaultExt = ".cab";
                sd.Filter = "Cab Files|*.cab";

                bool? dialogResult = sd.ShowDialog();

                if (data != null)
                {
                    if (dialogResult == true)
                    {

                        using (var fs = sd.OpenFile())
                        {
                            fs.Write(data, 0, data.Length);

                            fs.Close();

                            MessageBox.Show(eHCMSResources.A0804_G1_Msg_InfoLuuOK);

                        }
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z0970_G1_NoDataReceived);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(eHCMSResources.T0432_G1_Error + ": " + ex.ToString());
                throw;
            }
        }
        public void CancelCmd()
        {
            ShowDownloadProgressPanel = false;
        }
    }
}
