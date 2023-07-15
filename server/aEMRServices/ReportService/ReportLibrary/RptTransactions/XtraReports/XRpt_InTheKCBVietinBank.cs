using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_InTheKCBVietinBank : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_InTheKCBVietinBank()
        {
            InitializeComponent();
        }

        private void XRpt_InTheKCBVietinBank_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
        }
    }
}
