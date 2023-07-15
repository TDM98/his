using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_6_Hinh_1_New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_6_Hinh_1_New()
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

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_PCLImagingResult_New_6_Hinh_SubReport_V2)((XRSubreport)sender).ReportSource).PCLImgResultID.Value = Convert.ToInt64(PCLImgResultID.Value);
            ((XRpt_PCLImagingResult_New_6_Hinh_SubReport_V2)((XRSubreport)sender).ReportSource).V_PCLRequestType.Value = Convert.ToInt64(V_PCLRequestType.Value);
        }

        private void XRpt_PCLImagingResult_New_6_Hinh_1_New_AfterPrint(object sender, EventArgs e)
        {
            XRpt_PCLImagingResult_New_6_Hinh_New_SubReport_Image report2 = new XRpt_PCLImagingResult_New_6_Hinh_New_SubReport_Image();
            report2.PCLImgResultID.Value = Convert.ToInt64(PCLImgResultID.Value);
            report2.V_PCLRequestType.Value = Convert.ToInt64(V_PCLRequestType.Value);
            report2.parDepartmentOfHealth.Value = Convert.ToString(parDepartmentOfHealth.Value);
            report2.parHospitalName.Value = Convert.ToString(parHospitalName.Value);
            report2.parHospitalAddress.Value = Convert.ToString(parHospitalAddress.Value);
            report2.CreateDocument();

            (sender as XtraReport).Pages.AddRange(report2.Pages);
        }
    }
}
