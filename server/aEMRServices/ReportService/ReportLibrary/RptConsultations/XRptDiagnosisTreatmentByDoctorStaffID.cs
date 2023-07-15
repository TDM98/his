using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptDiagnosisTreatmentByDoctorStaffID : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDiagnosisTreatmentByDoctorStaffID()
        {
            InitializeComponent();
        }

        private void XRptDiagnosisTreatmentByDoctorStaffID_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            this.dsRpt_DiagnosisTreatmentByDoctorStaffID1.EnforceConstraints = false;
            this.spRpt_DiagnosisTreatmentByDoctorStaffIDTableAdapter.Fill(this.dsRpt_DiagnosisTreatmentByDoctorStaffID1.spRpt_DiagnosisTreatmentByDoctorStaffID, Convert.ToInt32(this.parDoctorStaffID.Value), Convert.ToDateTime(this.parFromDate.Value), Convert.ToDateTime(this.parToDate.Value));
        }

    }
}
