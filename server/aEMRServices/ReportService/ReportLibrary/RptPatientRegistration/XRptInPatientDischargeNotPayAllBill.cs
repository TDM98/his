using System;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptInPatientDischargeNotPayAllBill: DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInPatientDischargeNotPayAllBill()
        {
            InitializeComponent();
            BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptInPatientDischargeNotPayAllBill_BeforePrint);
        }

        public void FillData()
        {
            spRptInPatientDischargedNotPaymentAllBillTableAdapter.Fill(dsInPatientDischargeNotPayAllBill1.spRptInPatientDischargedNotPaymentAllBill
                                                                , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(DeptID.Value));
        }

        private void XRptInPatientDischargeNotPayAllBill_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}