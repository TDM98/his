using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptDoanhThuTongHop : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoanhThuTongHop()
        {
            InitializeComponent();
        }

        private void XRptDoanhThuTongHop_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            this.dsDoanhThuTongHop1.EnforceConstraints = false;
            this.spRptDoanhThuTongHopTableAdapter.Fill(this.dsDoanhThuTongHop1.spRptDoanhThuTongHop, Convert.ToDateTime(this.parFromDate.Value), Convert.ToDateTime(this.parToDate.Value), Convert.ToInt64(this.paraDeptLocID.Value), Convert.ToInt64(this.paraDeptID.Value));
        }
    }
}
