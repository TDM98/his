using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_GiayNghiViecKhongHuongBaoHiemSubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_GiayNghiViecKhongHuongBaoHiemSubReport()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_GiayNghiViecKhongHuongBaoHiem1.EnforceConstraints = false;
            spGetVacationInsuranceCertificates_ByIDTableAdapter.Fill(dsXRpt_GiayNghiViecKhongHuongBaoHiem1.spGetVacationInsuranceCertificates_ByID, Convert.ToInt64(VacationInsuranceCertificateID.Value));
            if (Convert.ToBoolean(dsXRpt_GiayNghiViecKhongHuongBaoHiem1.spGetVacationInsuranceCertificates_ByID.Rows[0]["IsPrenatal"]))
            {
                xrLabel19.Text = null;
                xrLabel20.Text = null;
                xrLabel21.Text = null;
                xrLabel36.Text = null;
                xrLabel38.Text = null;
                xrLabel40.Text = null;
                xrLabel41.Text = null;
            }
            //xrLabel4.Text = "Số seri: " + parHospitalCode.Value + 
            //    Convert.ToDateTime(dsXRpt_GiayNghiViecKhongHuongBaoHiem1.spGetVacationInsuranceCertificates_ByID.Rows[0]["CreatedDate"]).Year.ToString().Substring(2, 2) + 
            //    Convert.ToInt32(dsXRpt_GiayNghiViecKhongHuongBaoHiem1.spGetVacationInsuranceCertificates_ByID.Rows[0]["SeriNumber"]).ToString("D5");
        }
        private void XRpt_GiayNghiViecKhongHuongBaoHiemSubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
