using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_1_Hinh_SubReport_Image : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_1_Hinh_SubReport_Image()
        {
            InitializeComponent();
        }

        private void XRpt_PCLImagingResult_New_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_SubReport_Image1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_New_Sub_ImageTableAdapter.Fill(dsXRpt_PCLImagingResult_New_SubReport_Image1.spXRpt_PCLImagingResult_New_Sub_Image, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));
        }
    }
}
