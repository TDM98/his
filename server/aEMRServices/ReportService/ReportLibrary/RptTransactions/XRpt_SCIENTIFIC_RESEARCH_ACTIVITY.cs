using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_SCIENTIFIC_RESEARCH_ACTIVITY : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_SCIENTIFIC_RESEARCH_ACTIVITY()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spBAOCAO_CDTTableAdapter.Fill(ds_SCIENTIFIC_RESEARCH_ACTIVITY1.spBAOCAO_CDT, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value));
        }
        public void SetParametersForSubReport(object sender)
        {
            ((XRpt_SCIENTIFIC_RESEARCH_ACTIVITY_SubReport)((XRSubreport)sender).ReportSource).FromDate.Value = Convert.ToDateTime(this.FromDate.Value);

            ((XRpt_SCIENTIFIC_RESEARCH_ACTIVITY_SubReport)((XRSubreport)sender).ReportSource).ToDate.Value = Convert.ToDateTime(this.ToDate.Value);
    

        }
        private void XRpt_SCIENTIFIC_RESEARCH_ACTIVITY_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            this.xrLabel14.Text = "( Kỳ hạn báo cáo : " + Convert.ToDateTime(this.ToDate.Value).Month + " tháng )";
            this.xrLabel15.Text = "( Kỳ hạn báo cáo : " + Convert.ToDateTime(this.ToDate.Value).Month + " tháng )";
            this.xrLabel16.Text = "" + Convert.ToDateTime(this.ToDate.Value).Year + "";
            this.xrLabel17.Text = "" + Convert.ToDateTime(this.ToDate.Value).Year + "";
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }
    }
}
