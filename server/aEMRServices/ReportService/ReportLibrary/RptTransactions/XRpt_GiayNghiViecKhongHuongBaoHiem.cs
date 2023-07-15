using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_GiayNghiViecKhongHuongBaoHiem : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_GiayNghiViecKhongHuongBaoHiem()
        {
            InitializeComponent();
        }
        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_GiayNghiViecKhongHuongBaoHiemSubReport)((XRSubreport)sender).ReportSource).VacationInsuranceCertificateID.Value = Convert.ToInt32(VacationInsuranceCertificateID.Value);
            ((XRpt_GiayNghiViecKhongHuongBaoHiemSubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
            ((XRpt_GiayNghiViecKhongHuongBaoHiemSubReport)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value;
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_GiayNghiViecKhongHuongBaoHiemSubReport)((XRSubreport)sender).ReportSource).VacationInsuranceCertificateID.Value = Convert.ToInt32(VacationInsuranceCertificateID.Value);
            ((XRpt_GiayNghiViecKhongHuongBaoHiemSubReport)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value;
            ((XRpt_GiayNghiViecKhongHuongBaoHiemSubReport)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value;
        }
    }
}
