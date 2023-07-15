using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPatientInfo : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientInfo()
        {
            InitializeComponent();
        }

        public void SetParametersForSubReport(object sender)
        {
            ((XRptPatientInfo_SubReport)((XRSubreport)sender).ReportSource).PatientCode.Value = this.PatientCode.Value.ToString();
            ((XRptPatientInfo_SubReport)((XRSubreport)sender).ReportSource).FileCodeNumber.Value = this.FileCodeNumber.Value.ToString();
            ((XRptPatientInfo_SubReport)((XRSubreport)sender).ReportSource).PatientName.Value = this.PatientName.Value.ToString();
            ((XRptPatientInfo_SubReport)((XRSubreport)sender).ReportSource).DOB.Value = this.DOB.Value.ToString();
            ((XRptPatientInfo_SubReport)((XRSubreport)sender).ReportSource).Age.Value = this.Age.Value.ToString();
            ((XRptPatientInfo_SubReport)((XRSubreport)sender).ReportSource).Gender.Value = this.Gender.Value.ToString();

            DateTime admissionDate = Convert.ToDateTime(this.AdmissionDate.Value);
            if (admissionDate.Year > 1970)
            {
                ((XRptPatientInfo_SubReport)((XRSubreport)sender).ReportSource).AdmissionDate.Value = admissionDate;
            }
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport5_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport7_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport11_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport13_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport14_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport15_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport16_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport17_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport18_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport19_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport20_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport21_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport22_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport23_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport24_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport25_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport26_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport27_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport28_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport29_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport30_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport31_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport32_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport33_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport34_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport35_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport36_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport37_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport38_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport39_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport40_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport41_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport42_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport43_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport44_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport45_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport46_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport47_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport48_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport49_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport50_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport51_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport52_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport53_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport54_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport55_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport56_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport57_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport58_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport59_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport60_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport61_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport62_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport63_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport64_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport65_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

    }
}
