using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_Sieu_Am_Tim : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_Sieu_Am_Tim()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_Sieu_Am_Tim1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_New_Sieu_Am_TimTableAdapter.Fill(dsXRpt_PCLImagingResult_New_Sieu_Am_Tim1.spXRpt_PCLImagingResult_New_Sieu_Am_Tim, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));
        }

        private void XRpt_PCLImagingResult_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

      
        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_PCLImagingResult_New_6_Hinh_SubReport_Image)((XRSubreport)sender).ReportSource).PCLImgResultID.Value = Convert.ToInt64(PCLImgResultID.Value);
            ((XRpt_PCLImagingResult_New_6_Hinh_SubReport_Image)((XRSubreport)sender).ReportSource).V_PCLRequestType.Value = Convert.ToInt64(V_PCLRequestType.Value);
        }
    }
}
