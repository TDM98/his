using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward
{
    public partial class XRptPhieuSaoThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuSaoThuoc()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            sp_PhieuSaoThuoc_DetailsTableAdapter.Fill(dsPhieuSaoThuoc1.sp_PhieuSaoThuoc_Details, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value), Convert.ToInt64(parIntPtDiagDrInstructionID.Value));
            sp_PhieuSaoThuoc_HeaderTableAdapter.Fill(dsPhieuSaoThuoc1.sp_PhieuSaoThuoc_Header, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value), Convert.ToInt64(parIntPtDiagDrInstructionID.Value));
        }

        private void XRXRptPhieuSaoThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
