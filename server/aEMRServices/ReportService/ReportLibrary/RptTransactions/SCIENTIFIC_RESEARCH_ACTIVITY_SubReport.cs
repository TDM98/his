using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_SCIENTIFIC_RESEARCH_ACTIVITY_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_SCIENTIFIC_RESEARCH_ACTIVITY_SubReport()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spBAOCAO_NCKHOAHOCTableAdapter.Fill(xRpt_SCIENTIFIC_RESEARCH_ACTIVITY_SubReport1.spBAOCAO_NCKHOAHOC, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value));
        }
        private void XRpt_SCIENTIFIC_RESEARCH_ACTIVITY_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
           
        }
           

    }
}
