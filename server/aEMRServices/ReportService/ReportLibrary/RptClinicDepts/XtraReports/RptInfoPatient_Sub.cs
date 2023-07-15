using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptClinicDepts.XtraReports
{
    public partial class RptInfoPatient_Sub : DevExpress.XtraReports.UI.XtraReport
    {
        public RptInfoPatient_Sub()
        {
            InitializeComponent();
        }

        private void RptInfoPatient_Sub_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            this.Parameters["DOB"].Value = this.DOB.Value.ToString();
            this.Parameters["PatientCode"].Value = this.PatientCode.Value.ToString();
            this.Parameters["PatientName"].Value = this.PatientName.Value.ToString();
            this.Parameters["FileCodeNumber"].Value = this.FileCodeNumber.Value.ToString();
        }

    }
}
