using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_InTheKCBVaccine : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_InTheKCBVaccine()
        {
            InitializeComponent();
        }

        private void XRpt_InTheKCBVaccine_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
        }
    }
}
