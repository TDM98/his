using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptToDieuTriNgoaiTru_Summary_ForMaxillofacial : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptToDieuTriNgoaiTru_Summary_ForMaxillofacial()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsToDieuTriNgoaiTru_ForMaxillofacial1.EnforceConstraints = false;
            spRptToDieuTriNgoaiTru_ForMaxillofacialTableAdapter.Fill(dsToDieuTriNgoaiTru_ForMaxillofacial1.spRptToDieuTriNgoaiTru_ForMaxillofacial, OutPtTreatmentProgramID.Value as long?);
        }
        
        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptToDieuTriNgoaiTru_SubReport_ForMaxillofacial)((XRSubreport)sender).ReportSource).Parameters["PtRegDetailID"].Value = GetCurrentColumnValue("PtRegDetailID") as long?;
        }
       
    }
}