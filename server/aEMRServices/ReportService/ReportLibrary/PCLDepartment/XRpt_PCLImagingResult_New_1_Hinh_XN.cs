using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_1_Hinh_XN : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_1_Hinh_XN()
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
            
            if (parHospitalCode.Value.ToString() == "96160")
            {
                if (Convert.ToBoolean(IsBilingual.Value))
                    xrLabel9.Text = "Trưởng Phòng Xét Nghiệm<br><size=10><i>Head of Laboratory</i></size>";
                else
                    xrLabel9.Text = "Trưởng Phòng Xét Nghiệm";
            }
            else
            {
                if (Convert.ToBoolean(IsBilingual.Value))
                    xrLabel9.Text = "Trưởng Khoa Xét Nghiệm<br><size=10><i>Head of Laboratory Department</i></size>";
                else
                    xrLabel9.Text = "Trưởng Khoa Xét Nghiệm";
            }
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
            ((XRpt_PCLImagingResult_New_1_Hinh_XN_SubReport)((XRSubreport)sender).ReportSource).PCLImgResultID.Value = Convert.ToInt64(PCLImgResultID.Value);
            ((XRpt_PCLImagingResult_New_1_Hinh_XN_SubReport)((XRSubreport)sender).ReportSource).V_PCLRequestType.Value = Convert.ToInt64(V_PCLRequestType.Value);
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalAddress.Value = parHospitalAddress.Value;
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_PCLImagingResult_New_1_Hinh_XN_SubReport_Image)((XRSubreport)sender).ReportSource).PCLImgResultID.Value = Convert.ToInt64(PCLImgResultID.Value);
            ((XRpt_PCLImagingResult_New_1_Hinh_XN_SubReport_Image)((XRSubreport)sender).ReportSource).V_PCLRequestType.Value = Convert.ToInt64(V_PCLRequestType.Value);
        }
    }
}
