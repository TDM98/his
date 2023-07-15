using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptDoctorInstruction: DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoctorInstruction()
        {
            InitializeComponent();
        }

        private void XRptDoctorInstruction_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            //this.dsRpt_DiagnosisTreatmentByDoctorStaffID1.EnforceConstraints = false;
            this.spRptInstruction_ParentTableAdapter.Fill(this.dsDoctorInstruction1.spRptInstruction_Parent, Convert.ToInt32(parInstructionID));
        }

    }
}
