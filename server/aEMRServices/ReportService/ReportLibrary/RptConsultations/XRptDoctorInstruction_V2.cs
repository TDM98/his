namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptDoctorInstruction_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoctorInstruction_V2()
        {
            InitializeComponent();
        }
        private void XRptDoctorInstruction_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //spRptMedicalInstructionTableAdapter.Fill(dsRptMedicalInstruction1.spRptMedicalInstruction, IntPtDiagDrInstructionID.Value as long?, 0, null, null);
            //spRptGetIntravenousPlan_InPtTableAdapter.Fill(dsRptMedicalInstruction1.spRptGetIntravenousPlan_InPt, IntPtDiagDrInstructionID.Value as long?, 0);
            sp_GetAllPCLItemsByInstructionIDTableAdapter.Fill(dsRptMedicalInstruction1.sp_GetAllPCLItemsByInstructionID, IntPtDiagDrInstructionID.Value as long?);
            sp_GetAllRegistrationItemsByInstructionIDTableAdapter.Fill(dsRptMedicalInstruction1.sp_GetAllRegistrationItemsByInstructionID, IntPtDiagDrInstructionID.Value as long?);
        }
    }
}
