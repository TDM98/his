using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptDoctorInstruction_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoctorInstruction_SubReport()
        {
            InitializeComponent();
        }

        private void XRptDoctorInstruction_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptMedicalInstructionTableAdapter.Fill(dsRptMedicalInstruction1.spRptMedicalInstruction, IntPtDiagDrInstructionID.Value as long?, 0
                , PrescriptID.Value as long?, null, null);
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
    }
}
