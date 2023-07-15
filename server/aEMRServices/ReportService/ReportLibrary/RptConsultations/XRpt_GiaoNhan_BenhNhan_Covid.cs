using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRpt_GiaoNhan_BenhNhan_Covid : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_GiaoNhan_BenhNhan_Covid()
        {
            InitializeComponent();
        }

        private void XRpt_XacNhan_DieuTri_Covid_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_GiaoNhan_BenhNhan_Covid1.EnforceConstraints = false;
            spXRpt_GiaoNhan_BenhNhan_CovidTableAdapter.Fill(dsXRpt_GiaoNhan_BenhNhan_Covid1.spXRpt_GiaoNhan_BenhNhan_Covid,Convert.ToInt64(InPatientAdmDisDetailID.Value));
        }
    }
}
