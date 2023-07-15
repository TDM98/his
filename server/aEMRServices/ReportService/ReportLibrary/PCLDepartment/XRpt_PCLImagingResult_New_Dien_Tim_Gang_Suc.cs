using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_New_Dien_Tim_Gang_SucTableAdapter.Fill(dsXRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc1.spXRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));
           
        }

        private void XRpt_PCLImagingResult_New_Test_Nhanh_Cov_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
