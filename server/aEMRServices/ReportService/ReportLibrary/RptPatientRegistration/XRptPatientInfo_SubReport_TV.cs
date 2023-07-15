using System.Drawing;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPatientInfo_SubReport_TV : XtraReport
    {
        public XRptPatientInfo_SubReport_TV()
        {
            InitializeComponent();
        }

        private void XRptPatientInfo_SubReport_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //this.Parameters["DOB_Gender"].Value = this.DOB.Value.ToString() + " - " + this.Gender.Value.ToString();
            if (!string.IsNullOrEmpty(Age.Value.ToString()))
            {
                Parameters["DOB_Gender"].Value = Age.Value.ToString() + " ( " + Gender.Value.ToString() + " )";
                //if (DOB.Value.ToString().Length > 4)
                //{
                //    Parameters["DOB_Age"].Value = DOB.Value.ToString();
                //} else
                Parameters["DOB_Age"].Value = DOB.Value.ToString() + " (" + Age.Value.ToString() + ")";
            }
            else
            {
                Parameters["DOB_Gender"].Value = DOB.Value.ToString() + " ( " + Gender.Value.ToString() + " )";
                //if (DOB.Value.ToString().Length > 4)
                //{
                //    Parameters["DOB_Age"].Value = DOB.Value.ToString();
                //}
                //else 
                Parameters["DOB_Age"].Value = DOB.Value.ToString() + " (" + Age.Value.ToString() + ")";
            }
            Parameters["FileAndPatientCode"].Value = PatientCode.Value.ToString() + (FileCodeNumber.Value.ToString() == PatientCode.Value.ToString() ? "" : " - " + FileCodeNumber.Value.ToString());
        }

        private void xrLabel2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //int CharLimit_Font8 = 45;

            //int Address_Length = PatientFullAddress.Value.ToString().Length;

            //if (Address_Length > CharLimit_Font8)
            //{
            //    xrLabel2.Font = new Font("Times New Roman", 6F);
            //}

            //int CharLimit_Font9 = 20;
            //int FullName_Length = PatientName.Value.ToString().Length;

            //if (FullName_Length > CharLimit_Font9)
            //{
            //    xrLabel1.Font = new Font("Times New Roman", 5F);
            //}
        }
    }
}
