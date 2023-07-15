using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_6_Hinh_New_SubReport_V2_C2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_6_Hinh_New_SubReport_V2_C2()
        {
            InitializeComponent();
        }

        private void XRpt_PCLImagingResult_New_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_6_Hinh_Sub1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_New_6_Hinh_SubTableAdapter.Fill(dsXRpt_PCLImagingResult_New_6_Hinh_Sub1.spXRpt_PCLImagingResult_New_6_Hinh_Sub, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value), 2);
        }
    }
}
