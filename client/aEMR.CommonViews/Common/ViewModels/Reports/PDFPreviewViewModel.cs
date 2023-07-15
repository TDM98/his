using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System;
using System.Windows.Media.Imaging;
using DataEntities;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.ReportModel.ReportModels;
using aEMR.Infrastructure;
using Castle.Windsor;
using DevExpress.ReportServer.Printing;
using System.Windows;
using System.Linq;
/*
* 20221129 #001 TNHX: Thêm màn hình kết quả PDF chữ ký số
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPDFPreviewView)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PDFPreviewViewModel : Conductor<object>, IPDFPreviewView
    {
        [ImportingConstructor]
        public PDFPreviewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            try
            {
                Uri uri = new Uri(DigitalSignatureResultPath);
                PDFViewerSource = uri;
            }
           catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Uri _PDFViewerSource;
        public Uri PDFViewerSource
        {
            get { return _PDFViewerSource; }
            set
            {
                if (_PDFViewerSource != value)
                {
                    _PDFViewerSource = value;
                    NotifyOfPropertyChange(() => PDFViewerSource);
                }
            }
        }

        private string _DigitalSignatureResultPath;
        public string DigitalSignatureResultPath
        {
            get { return _DigitalSignatureResultPath; }
            set
            {
                _DigitalSignatureResultPath = value;
                NotifyOfPropertyChange(() => DigitalSignatureResultPath);
            }
        }

        public void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            DigitalSignatureResultPath = "";
        }
    }
}
