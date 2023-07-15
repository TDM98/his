using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward
{
    public partial class XRptKKPhieuCongKhaiThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptKKPhieuCongKhaiThuoc()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            sp_KKPhieuCongKhaiThuoc_DetailsTableAdapter1.Fill(kkPhieuCongKhaiThuoc1.sp_KKPhieuCongKhaiThuoc_Details, Convert.ToInt64(parIntPtDiagDrInstructionID.Value));
            sp_KKPhieuCongKhaiThuoc_HeaderTableAdapter.Fill(kkPhieuCongKhaiThuoc1.sp_KKPhieuCongKhaiThuoc_Header, Convert.ToInt64(parIntPtDiagDrInstructionID.Value));
        }

        private void XRXRptKKPhieuCongKhaiThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
