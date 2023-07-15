using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptEPrescriptionNew_InPt : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionNew_InPt()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            this.dsPrescriptionNew_InPt1.EnforceConstraints = false;
            this.spPrescriptions_RptHeaderByIssueID_InPtTableAdapter.Fill(this.dsPrescriptionNew_InPt1.spPrescriptions_RptHeaderByIssueID_InPt, Convert.ToInt32(this.parIssueID.Value));
            this.spPrescriptions_RptViewByPrescriptID_InPtTableAdapter.Fill(this.dsPrescriptionNew_InPt1.spPrescriptions_RptViewByPrescriptID_InPt, Convert.ToInt32(this.parIssueID.Value), false, false);
        }

        private void XRptEPrescriptionNew_InPt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
