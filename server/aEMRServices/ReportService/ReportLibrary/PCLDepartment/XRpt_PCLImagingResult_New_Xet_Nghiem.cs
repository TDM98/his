using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_Xet_Nghiem : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_Xet_Nghiem()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_NewTableAdapter.Fill(dsXRpt_PCLImagingResult_New1.spXRpt_PCLImagingResult_New, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));

        }

        private void XRpt_PCLImagingResult_New_Test_Nhanh_Cov_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_PCLImagingResult_New_Xet_Nghiem_SubReport_V2)((XRSubreport)sender).ReportSource).PCLImgResultID.Value = Convert.ToInt64(PCLImgResultID.Value);
            ((XRpt_PCLImagingResult_New_Xet_Nghiem_SubReport_V2)((XRSubreport)sender).ReportSource).V_PCLRequestType.Value = Convert.ToInt64(V_PCLRequestType.Value);
        }
    }
}
