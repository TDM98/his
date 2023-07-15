using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_CLVT_Hai_Ham : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_CLVT_Hai_Ham()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_CLVT_Hai_Ham1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_New_CLVT_Hai_HamTableAdapter.Fill(dsXRpt_PCLImagingResult_New_CLVT_Hai_Ham1.spXRpt_PCLImagingResult_New_CLVT_Hai_Ham, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));
        }

        private void XRpt_PCLImagingResult_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
