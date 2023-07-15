using System.Windows.Controls;
using System.Windows.Media;
using aEMR.Controls;
using Microsoft.Web.WebView2.Core;

namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{
    
    public partial class ExamInformationPatientsView : UserControl
    {
        public ExamInformationPatientsView()
        {
            InitializeComponent();
        }

        //private void WbMain_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        //{
        //    WbMain.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        //    WbMain.CoreWebView2.Settings.UserAgent = "MY-AGENT";
        //    WbMain.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        //    WbMain.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
        //    WbMain.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;
        //}
        //private void CoreWebView2_WebResourceRequested(object sender, CoreWebView2WebResourceRequestedEventArgs e)
        //{
        //    e.Request.Headers.SetHeader("Authorization", "Bearer 192|Rx5gmHJPR32oy1WXhkeQ8rHZrb2y0KqUqhEMpEMa");
        //}
    }
}
