using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRpt_XacNhan_DieuTri_Covid : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_XacNhan_DieuTri_Covid()
        {
            InitializeComponent();
        }

        private void XRpt_XacNhan_DieuTri_Covid_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_XacNhan_DieuTri_Covid1.EnforceConstraints = false;
            spXRpt_XacNhan_DieuTri_CovidTableAdapter.Fill(dsXRpt_XacNhan_DieuTri_Covid1.spXRpt_XacNhan_DieuTri_Covid,Convert.ToInt64(InPatientAdmDisDetailID.Value));
        }
    }
}
