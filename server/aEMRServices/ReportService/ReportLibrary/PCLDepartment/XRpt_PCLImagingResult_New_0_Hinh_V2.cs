using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_0_Hinh_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_0_Hinh_V2()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_V21.EnforceConstraints = false;
            spXRpt_PCLImagingResult_NewTableAdapter1.Fill(dsXRpt_PCLImagingResult_New_V21.spXRpt_PCLImagingResult_New, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));
            spXRpt_PCLImagingResult_New_SubTableAdapter1.Fill(dsXRpt_PCLImagingResult_New_V21.spXRpt_PCLImagingResult_New_Sub, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));

            if (IsEmptyValue(dsXRpt_PCLImagingResult_New_V21.spXRpt_PCLImagingResult_New.Rows[0]["Suggest"].ToString()))
            {
                xrTable4.Visible = false;
            }
        }

        private void XRpt_PCLImagingResult_New_0_Hinh_V2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_PCLImagingResult_New_0_Hinh_V2_SubReport)((XRSubreport)sender).ReportSource).PCLImgResultID.Value = Convert.ToInt64(PCLImgResultID.Value);
            ((XRpt_PCLImagingResult_New_0_Hinh_V2_SubReport)((XRSubreport)sender).ReportSource).V_PCLRequestType.Value = Convert.ToInt64(V_PCLRequestType.Value);
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
            //((XRpt_PCLImagingResult_New_SubReport)((XRSubreport)sender).ReportSource).parHospitalAddress.Value = parHospitalAddress.Value;
        }
    }
}
