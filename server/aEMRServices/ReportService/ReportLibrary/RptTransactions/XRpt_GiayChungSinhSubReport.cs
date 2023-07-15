using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_GiayChungSinhSubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_GiayChungSinhSubReport()
        {
            InitializeComponent();
        }

        private void XRpt_GiayChungSinhSubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_GiayChungSinh1.EnforceConstraints = false;
            spGetBirthCertificates_ByIDTableAdapter.Fill(dsXRpt_GiayChungSinh1.spGetBirthCertificates_ByID, Convert.ToInt64(BirthCertificateID.Value));
        }
    }
}
