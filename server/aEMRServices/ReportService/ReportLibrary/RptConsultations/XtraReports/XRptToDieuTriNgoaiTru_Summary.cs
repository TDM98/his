using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptToDieuTriNgoaiTru_Summary : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptToDieuTriNgoaiTru_Summary()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {           
            spRptToDieuTriNgoaiTruTableAdapter.Fill(dsRptToDieuTriNgoaiTru1.spRptToDieuTriNgoaiTru, OutPtTreatmentProgramID.Value as long?);
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
                ((XRptToDieuTriNgoaiTru_SubReport)((XRSubreport)sender).ReportSource).Parameters["PtRegDetailID"].Value = GetCurrentColumnValue("PtRegDetailID") as long?;
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
                ((XRptToDieuTriNgoaiTru_CLS_SubReport)((XRSubreport)sender).ReportSource).Parameters["PtRegDetailID"].Value = GetCurrentColumnValue("PtRegDetailID") as long?;
        }
        //private void DetailSubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        //{
        //    ((XRptDoctorInstruction_SubReport)((XRSubreport)sender).ReportSource).Parameters["IntPtDiagDrInstructionID"].Value = GetCurrentColumnValue("IntPtDiagDrInstructionID") as long?;
        //}
        //private void DetailDrugSubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        //{
        //    ((XRptDoctorInstructionDetail_SubReport)((XRSubreport)sender).ReportSource).Parameters["IntPtDiagDrInstructionID"].Value = GetCurrentColumnValue("IntPtDiagDrInstructionID") as long?;
        //}
    }
}