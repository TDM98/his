using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_0_Hinh : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_0_Hinh()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_NewTableAdapter.Fill(dsXRpt_PCLImagingResult_New1.spXRpt_PCLImagingResult_New, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));

        }

        private void XRpt_PCLImagingResult_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_PCLImagingResult_New_0_Hinh_SubReport_V2)((XRSubreport)sender).ReportSource).PCLImgResultID.Value = Convert.ToInt64(PCLImgResultID.Value);
            ((XRpt_PCLImagingResult_New_0_Hinh_SubReport_V2)((XRSubreport)sender).ReportSource).V_PCLRequestType.Value = Convert.ToInt64(V_PCLRequestType.Value);
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalAddress.Value = parHospitalAddress.Value;
        }
    }
}
