using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRpt_PCLImagingResult_New_Realtime_PCR_Cov : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PCLImagingResult_New_Realtime_PCR_Cov()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_PCLImagingResult_New_Realtime_PCR_Cov1.EnforceConstraints = false;
            spXRpt_PCLImagingResult_New_Realtime_PCR_CovTableAdapter.Fill(dsXRpt_PCLImagingResult_New_Realtime_PCR_Cov1.spXRpt_PCLImagingResult_New_Realtime_PCR_Cov
                , Convert.ToInt64(PCLImgResultID.Value), Convert.ToInt64(V_PCLRequestType.Value));
            
            if (dsXRpt_PCLImagingResult_New_Realtime_PCR_Cov1.spXRpt_PCLImagingResult_New_Realtime_PCR_Cov != null 
                && dsXRpt_PCLImagingResult_New_Realtime_PCR_Cov1.spXRpt_PCLImagingResult_New_Realtime_PCR_Cov.Rows.Count > 0)
            {
                if (dsXRpt_PCLImagingResult_New_Realtime_PCR_Cov1.spXRpt_PCLImagingResult_New_Realtime_PCR_Cov.Rows[0]["FileNameExportPDF"] != null)
                {
                    ExportOptions.PrintPreview.DefaultFileName = dsXRpt_PCLImagingResult_New_Realtime_PCR_Cov1.spXRpt_PCLImagingResult_New_Realtime_PCR_Cov.Rows[0]["FileNameExportPDF"].ToString();
                }
                //if (dsXRpt_PCLImagingResult_New_Realtime_PCR_Cov1.spXRpt_PCLImagingResult_New_Realtime_PCR_Cov.Rows[0]["PublicLinkResulfPDF"] != null)
                //{
                //    ExportOptions.PrintPreview.SaveMode = DevExpress.XtraPrinting.SaveMode.UsingDefaultPath;
                //    ExportOptions.PrintPreview.DefaultDirectory = dsXRpt_PCLImagingResult_New_Realtime_PCR_Cov1.spXRpt_PCLImagingResult_New_Realtime_PCR_Cov.Rows[0]["PublicLinkResulfPDF"].ToString();
                //}
            }
        }

        private void XRpt_PCLImagingResult_New_Test_Nhanh_Cov_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            if (parHospitalCode.Value.ToString() == "96160")
            {
                xrLabel9.Text = "TRƯỞNG PHÒNG XÉT NGHIỆM";
            }
            else
            {
                xrLabel9.Text = "TRƯỞNG KHOA XÉT NGHIỆM";
            }
        }
    }
}
