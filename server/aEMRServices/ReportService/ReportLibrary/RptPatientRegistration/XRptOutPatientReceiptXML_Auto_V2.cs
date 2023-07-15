using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutPatientReceiptXML_Auto_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceiptXML_Auto_V2()
        {
            InitializeComponent();
        }
        private void XtraReport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter.Fill(outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXml, this.param_PaymentID.Value);
        }
        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptOutPatientReceiptAuto_V2)((XRSubreport)sender).ReportSource).param_PaymentID.Value = Convert.ToInt32(GetCurrentColumnValue("PaymentID"));
        }
    }
}
