using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptHoatDongPhongDangKy : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptHoatDongPhongDangKy()
        {
            InitializeComponent();
        }
        private void XRptHoatDongPhongDangKy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsHoatDongPhongDangKy1.EnforceConstraints = false;
            spRptHoatDongPhongDangKyTableAdapter.Fill(dsHoatDongPhongDangKy1.spRptHoatDongPhongDangKy
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                , Convert.ToInt64(parDeptLocID.Value)
                );
        }
    }
}
