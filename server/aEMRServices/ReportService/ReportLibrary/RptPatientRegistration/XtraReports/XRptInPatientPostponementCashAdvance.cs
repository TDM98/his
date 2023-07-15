using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration.XtraReports
{
    public partial class XRptInPatientPostponementCashAdvance : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInPatientPostponementCashAdvance()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptInPatientPostponementCashAdvance_BeforePrint);
        }

        public void FillData()
        {
            spRpt_InPatientPostponementCashAdvanceTableAdapter.Fill(dsInPatientPostponementCashAdvance1.spRpt_InPatientPostponementCashAdvance
                                                                , Convert.ToDateTime(this.FromDate.Value)
                                                                , Convert.ToDateTime(this.ToDate.Value)
                                                                , Convert.ToInt16(this.Quarter.Value)
                                                                , Convert.ToInt16(this.Month.Value)
                                                                , Convert.ToInt16(this.Year.Value)
                                                                , Convert.ToInt16(this.flag.Value)
                                                                , Convert.ToInt32(this.DeptID.Value));

        }

        private void XRptInPatientPostponementCashAdvance_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
