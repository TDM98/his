using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_Test_Nhanh_Cov : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_Test_Nhanh_Cov()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_Test_Nhanh_Cov1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_New_Test_Nhanh_CovTableAdapter.Fill(dsXRpt_PCLImagingResult_New_Test_Nhanh_Cov1.spXRpt_PCLImagingResult_New_Test_Nhanh_Cov, Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));
            if (dsXRpt_PCLImagingResult_New_Test_Nhanh_Cov1.spXRpt_PCLImagingResult_New_Test_Nhanh_Cov != null
                && dsXRpt_PCLImagingResult_New_Test_Nhanh_Cov1.spXRpt_PCLImagingResult_New_Test_Nhanh_Cov.Rows.Count > 0)
            {
                if (dsXRpt_PCLImagingResult_New_Test_Nhanh_Cov1.spXRpt_PCLImagingResult_New_Test_Nhanh_Cov.Rows[0]["FileNameExportPDF"] != null)
                {
                    ExportOptions.PrintPreview.DefaultFileName = dsXRpt_PCLImagingResult_New_Test_Nhanh_Cov1.spXRpt_PCLImagingResult_New_Test_Nhanh_Cov.Rows[0]["FileNameExportPDF"].ToString();
                }
                //if (dsXRpt_PCLImagingResult_New_Test_Nhanh_Cov1.spXRpt_PCLImagingResult_New_Test_Nhanh_Cov.Rows[0]["PublicLinkResulfPDF"] != null)
                //{
                //    ExportOptions.PrintPreview.SaveMode = DevExpress.XtraPrinting.SaveMode.UsingDefaultPath;
                //    ExportOptions.PrintPreview.DefaultDirectory = dsXRpt_PCLImagingResult_New_Test_Nhanh_Cov1.spXRpt_PCLImagingResult_New_Test_Nhanh_Cov.Rows[0]["PublicLinkResulfPDF"].ToString();
                //}
            }
            //if(Convert.ToInt64(parHospitalCode.Value) == 95076)
            //{
            //    lblLanhDao.Visible = false;
            //    lblTruongKhoa.LocationF = new PointF(516, 40);
            //    picChuKyTruongKhoa.LocationF = new PointF(580, 70);
            //}
           
        }

        private void XRpt_PCLImagingResult_New_Test_Nhanh_Cov_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            if (parHospitalCode.Value.ToString() == "96160")
            {
                if (Convert.ToBoolean(IsBilingual.Value))
                    lblTruongKhoa.Text = "TRƯỞNG PHÒNG XÉT NGHIỆM<br><size=10><i>Head of Laboratory</i></size>";
                else
                    lblTruongKhoa.Text = "TRƯỞNG PHÒNG XÉT NGHIỆM";
            }
            else
            {
                if (Convert.ToBoolean(IsBilingual.Value))
                    lblTruongKhoa.Text = "TRƯỞNG KHOA XÉT NGHIỆM<br><size=10><i>Head of Laboratory Department</i></size>";
                else
                    lblTruongKhoa.Text = "TRƯỞNG KHOA XÉT NGHIỆM";
            }
        }
    }
}
