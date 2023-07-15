using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptToDieuTriNgoaiTru_Summary_ForChronic : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptToDieuTriNgoaiTru_Summary_ForChronic()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsToDieuTriNgoaiTru_ForChronic1.EnforceConstraints = false;
            spRptToDieuTriNgoaiTru_ForChronicTableAdapter.Fill(dsToDieuTriNgoaiTru_ForChronic1.spRptToDieuTriNgoaiTru_ForChronic, OutPtTreatmentProgramID.Value as long?);
        }
        
        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptToDieuTriNgoaiTru_SubReport_ForChronic)((XRSubreport)sender).ReportSource).Parameters["PtRegDetailID"].Value = GetCurrentColumnValue("PtRegDetailID") as long?;
        }
       
    }
}