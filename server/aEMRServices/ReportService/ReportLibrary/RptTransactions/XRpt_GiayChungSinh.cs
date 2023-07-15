using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_GiayChungSinh : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_GiayChungSinh()
        {
            InitializeComponent();
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_GiayChungSinhSubReport)((XRSubreport)sender).ReportSource).BirthCertificateID.Value = Convert.ToInt32(BirthCertificateID.Value);
            ((XRpt_GiayChungSinhSubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_GiayChungSinhSubReport)((XRSubreport)sender).ReportSource).BirthCertificateID.Value = Convert.ToInt32(BirthCertificateID.Value);
            ((XRpt_GiayChungSinhSubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
        }
    }
}
