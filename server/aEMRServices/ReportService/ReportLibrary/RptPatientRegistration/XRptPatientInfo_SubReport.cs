using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPatientInfo_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientInfo_SubReport()
        {
            InitializeComponent();
        }

        private void XRptPatientInfo_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //this.Parameters["DOB_Gender"].Value = this.DOB.Value.ToString() + " - " + this.Gender.Value.ToString();
            if (!string.IsNullOrEmpty(this.Age.Value.ToString()))
            {
                this.Parameters["DOB_Gender"].Value = this.Gender.Value.ToString() + " - " + this.DOB.Value.ToString() + " (" + this.Age.Value.ToString() + ")";
            }
            else
            {
                this.Parameters["DOB_Gender"].Value = this.Gender.Value.ToString() + " - " + this.DOB.Value.ToString();
            }
            this.Parameters["FileAndPatientCode"].Value = this.PatientCode.Value.ToString() + (this.FileCodeNumber.Value.ToString() == this.PatientCode.Value.ToString() ? "" : " - " + this.FileCodeNumber.Value.ToString());
        }

        private void xrLabel1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int CharLimit_Font8 = 19;
            int CharLimit_Font7 = 22;
            int CharLimit_Font6 = 26;

            int Name_Length = this.PatientName.Value.ToString().Length;

            if (Name_Length <= CharLimit_Font8)
            {
                ((XRLabel)sender).Font = new Font("Times New Roman", 8, FontStyle.Bold);
            }
            else if (Name_Length > CharLimit_Font8 && Name_Length <= CharLimit_Font7)
            {
                ((XRLabel)sender).Font = new Font("Times New Roman", 7, FontStyle.Bold);
            }
            else if (Name_Length > CharLimit_Font7 && Name_Length <= CharLimit_Font6)
            {
                ((XRLabel)sender).Font = new Font("Times New Roman", 6, FontStyle.Bold);
            }
            else
            {
                ((XRLabel)sender).Font = new Font("Times New Roman", 5, FontStyle.Bold);
            }
        }

    }
}
