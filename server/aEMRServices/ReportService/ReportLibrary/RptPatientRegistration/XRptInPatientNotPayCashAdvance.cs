using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptInPatientNotPayCashAdvance : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInPatientNotPayCashAdvance()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptInPatientNotPayCashAdvance_BeforePrint);
        }

        public void FillData()
        {
            spRpt_InPatientNotPayCashAdvanceTableAdapter.Fill(dsInPatientNotPayCashAdvance1.spRpt_InPatientNotPayCashAdvance
                                                                , Convert.ToDateTime(this.FromDate.Value)
                                                                , Convert.ToDateTime(this.ToDate.Value)
                                                                , Convert.ToInt16(this.Quarter.Value)
                                                                , Convert.ToInt16(this.Month.Value)
                                                                , Convert.ToInt16(this.Year.Value)
                                                                , Convert.ToInt16(this.flag.Value)
                                                                , Convert.ToInt32(this.DeptID.Value));

        }

        private void XRptInPatientNotPayCashAdvance_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
