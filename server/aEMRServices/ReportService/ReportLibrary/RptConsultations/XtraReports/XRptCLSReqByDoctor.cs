using System;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptCLSReqByDoctor : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptCLSReqByDoctor()
        {
            InitializeComponent();
        }

        private void XRptCLSReqByDoctor_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsCLSReqByDoctor1.EnforceConstraints = false;
            spRpt_CLSReqByDoctorTableAdapter.Fill(dsCLSReqByDoctor1.spRpt_CLSReqByDoctor, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(V_PatientFindBy.Value));
        }
    }
}
