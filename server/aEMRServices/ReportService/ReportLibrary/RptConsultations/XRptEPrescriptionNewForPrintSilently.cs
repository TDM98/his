using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptEPrescriptionNewForPrintSilently : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionNewForPrintSilently()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            this.dsPrescriptionNew1.EnforceConstraints = false;
            this.spPrescriptions_RptHeaderByIssueIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptHeaderByIssueID, Convert.ToInt32(this.parIssueID.Value));
            this.spPrescriptions_RptViewByPrescriptIDTableAdapter.Fill(this.dsPrescriptionNew1.spPrescriptions_RptViewByPrescriptID, Convert.ToInt32(this.parIssueID.Value), true, false);
        }

        private void XRptEPrescriptionNewForPrintSilently_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
