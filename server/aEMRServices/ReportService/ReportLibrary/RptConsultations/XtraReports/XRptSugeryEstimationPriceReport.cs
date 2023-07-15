using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptSugeryEstimationPriceReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSugeryEstimationPriceReport()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            this.spGetSugeryEstimationPriceReportTableAdapter.Fill(this.dsSugeryEstimationPriceReport1.spGetSugeryEstimationPriceReport, Convert.ToInt32(this.pConsultingDiagnosysID.Value));
            if (this.dsSugeryEstimationPriceReport1.spGetSugeryEstimationPriceReport.Rows.Count > 0)
            {
                if (Convert.ToBoolean(this.dsSugeryEstimationPriceReport1.spGetSugeryEstimationPriceReport.Rows[0]["ClosedSugery"]) == true)
                {
                    lbClosedSugery.BackColor = Color.Gray;
                    lbOpenedSugery.BackColor = Color.Transparent;
                }
                else
                {
                    lbClosedSugery.BackColor = Color.Transparent;
                    lbOpenedSugery.BackColor = Color.Gray;
                }
            }
        }
        private void XRptSugeryEstimationPriceReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}