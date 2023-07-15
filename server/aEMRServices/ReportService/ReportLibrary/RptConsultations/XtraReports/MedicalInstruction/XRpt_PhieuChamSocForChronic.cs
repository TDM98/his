using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRpt_PhieuChamSocForChronic : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PhieuChamSocForChronic()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRptPhieuChamSoc1.EnforceConstraints = false;
            spRptPhieuChamSocTableAdapter.Fill(dsRptPhieuChamSoc1.spRptPhieuChamSoc, PtRegistrationID.Value as long?, (long)V_RegistrationType.Value, false);
        }
    }
}
