using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptHoatDongPhongDangKyExt : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptHoatDongPhongDangKyExt()
        {
            InitializeComponent();
        }
        private void XRptHoatDongPhongDangKyExt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            this.dsHoatDongPhongDangKyExt1.EnforceConstraints = false;
            this.spRptHoatDongPhongDangKy_NewTableAdapter.Fill(this.dsHoatDongPhongDangKyExt1.spRptHoatDongPhongDangKy_New
                , Convert.ToDateTime(this.parFromDate.Value)
                , Convert.ToDateTime(this.parToDate.Value)
                , Convert.ToInt64(this.parDeptLocID.Value)
                );
        }
    }
}
