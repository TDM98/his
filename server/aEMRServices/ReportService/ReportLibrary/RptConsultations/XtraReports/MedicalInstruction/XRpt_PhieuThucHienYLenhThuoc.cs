using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRpt_PhieuThucHienYLenhThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PhieuThucHienYLenhThuoc()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsPhieuThucHienYLenhThuoc1.EnforceConstraints = false;
            spRptGetExecuteDrugTableAdapter.Fill(dsPhieuThucHienYLenhThuoc1.spRptGetExecuteDrug, PtRegistrationID.Value as long?);
        }
    }
}
