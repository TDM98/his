using System;
using System.Drawing.Printing;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPatientPaymentByStaff : XtraReport
    {
        public XRptPatientPaymentByStaff()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsPaymentReceiptByStaffDetails1.EnforceConstraints = false;
            spReportPaymentReceiptByStaffDetailsTableAdapter1.Fill(
                dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaffDetails,
                Convert.ToInt32(RepPaymentRecvID.Value));
            //this.spReportPaymentReceiptByStaffDetailsTableAdapter.Fill(this.dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaffDetails, Convert.ToInt32(this.parIssueID.Value));
        }

        private void XRptPatientPaymentByStaff_BeforePrint(object sender, PrintEventArgs e)
        {
            FillData();
        }
    }
}