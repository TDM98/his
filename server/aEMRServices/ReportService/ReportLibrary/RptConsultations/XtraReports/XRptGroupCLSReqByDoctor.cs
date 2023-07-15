using System;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptGroupCLSReqByDoctor : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptGroupCLSReqByDoctor()
        {
            InitializeComponent();
        }

        private void XRptGroupCLSReqByDoctor_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsGroupCLSReqByDoctor1.EnforceConstraints = false;
            spRpt_GroupCLSReqByDoctorTableAdapter.Fill(dsGroupCLSReqByDoctor1.spRpt_GroupCLSReqByDoctor, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(V_PatientFindBy.Value));
        }
    }
}
