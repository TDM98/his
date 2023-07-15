using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptDoctorInstruction_Summary_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoctorInstruction_Summary_V2()
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
            spRptMedicalInstruction_V2TableAdapter.Fill(dsRptMedicalInstruction1.spRptMedicalInstruction_V2, IntPtDiagDrInstructionID.Value as long?, PtRegistrationID.Value as long?, mFromDate, mToDate);
        }
    }
}
