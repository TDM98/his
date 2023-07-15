using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptReportCancelInvoiceDetails : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptReportCancelInvoiceDetails()
        {
            InitializeComponent();
        }

        private void XRptReportCancelInvoiceDetails_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsPaymentReceiptByStaffDetails1.EnforceConstraints = false;
            spReportPaymentReceiptByStaff_HeaderTableAdapter.Fill(
                  dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaff_Header,
                  Convert.ToInt32(RepPaymentRecvID.Value));
            spReportPaymentReceiptByStaffDetails_ByIDTableAdapter.Fill(
                dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaffDetails_ByID,
                Convert.ToInt32(RepPaymentRecvID.Value));
        }

    }
}
