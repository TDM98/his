using aEMR.Common.BaseModel;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IHtmlReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HtmlReportViewModel : ViewModelBase, IHtmlReport
    {
        private WebBrowser WbMain;
        public void WbMain_Loaded(object sender)
        {
            WbMain = sender as WebBrowser;
        }
        public void NavigateToString(string BodyContent)
        {
            if(WbMain == null)
            {
                return;
            }
            WbMain.NavigateToString(BodyContent);
        }
        public void PrintPreview()
        {
            mshtml.IHTMLDocument2 WbDocument = WbMain.Document as mshtml.IHTMLDocument2;
            WbDocument.execCommand("print", true, null);
        }
    }
}