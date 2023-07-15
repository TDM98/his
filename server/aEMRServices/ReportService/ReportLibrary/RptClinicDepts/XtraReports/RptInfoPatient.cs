using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptClinicDepts.XtraReports
{
    public partial class RptInfoPatient : DevExpress.XtraReports.UI.XtraReport
    {
        public RptInfoPatient()
        {
            InitializeComponent();
        }
        public void SetParametersForSubReport(object sender)
        {
            ((RptInfoPatient_Sub)((XRSubreport)sender).ReportSource).Parameters["FileCodeNumber"].Value = this.FileCodeNumber.Value.ToString();
            ((RptInfoPatient_Sub)((XRSubreport)sender).ReportSource).Parameters["PatientName"].Value = this.PatientName.Value.ToString();
            ((RptInfoPatient_Sub)((XRSubreport)sender).ReportSource).Parameters["DOB"].Value = this.DOB.Value.ToString();
            ((RptInfoPatient_Sub)((XRSubreport)sender).ReportSource).Parameters["PatientCode"].Value = this.PatientCode.Value.ToString();
        }

        private void xrSubreport1_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport5_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }


    }
}
