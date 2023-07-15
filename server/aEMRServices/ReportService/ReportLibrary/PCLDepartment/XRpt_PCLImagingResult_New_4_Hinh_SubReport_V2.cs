using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_4_Hinh_SubReport_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_4_Hinh_SubReport_V2()
        {
            InitializeComponent();
        }

        private void XRpt_PCLImagingResult_New_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_Sub1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_New_SubTableAdapter.Fill(dsXRpt_PCLImagingResult_New_Sub1.spXRpt_PCLImagingResult_New_Sub, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));
        }
    }
}
