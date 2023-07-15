using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptDoctorInstruction_Summary : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoctorInstruction_Summary()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRptMedicalInstruction1.EnforceConstraints = false;
            DateTime? mFromDate = null;
            DateTime? mToDate = null;
            if (FromDate.Value != null && Convert.ToDateTime(FromDate.Value) > DateTime.MinValue)
            {
                mFromDate = FromDate.Value as DateTime?;
            }
            if (ToDate.Value != null && Convert.ToDateTime(ToDate.Value) > DateTime.MinValue)
            {
                mToDate = ToDate.Value as DateTime?;
            }
            spRptMedicalInstructionTableAdapter.Fill(dsRptMedicalInstruction1.spRptMedicalInstruction, IntPtDiagDrInstructionID.Value as long?, PtRegistrationID.Value as long?
                , PrescriptID.Value as long?, mFromDate, mToDate);
        }

        private void DetailSubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptDoctorInstruction_SubReport)((XRSubreport)sender).ReportSource).Parameters["IntPtDiagDrInstructionID"].Value = GetCurrentColumnValue("IntPtDiagDrInstructionID") as long?;
            ((XRptDoctorInstruction_SubReport)((XRSubreport)sender).ReportSource).Parameters["PrescriptID"].Value = GetCurrentColumnValue("PrescriptID") as long?;
        }

        private void DetailDrugSubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptDoctorInstructionDetail_SubReport)((XRSubreport)sender).ReportSource).Parameters["IntPtDiagDrInstructionID"].Value = GetCurrentColumnValue("IntPtDiagDrInstructionID") as long?;
            ((XRptDoctorInstructionDetail_SubReport)((XRSubreport)sender).ReportSource).Parameters["PrescriptID"].Value = GetCurrentColumnValue("PrescriptID") as long?;
        }
    }
}
