using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptDoanhThuTongHopNoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoanhThuTongHopNoiTru()
        {
            InitializeComponent();
        }

        private void XRptDoanhThuTongHopNoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            this.dsDoanhThuTongHopNoiTru1.EnforceConstraints = false;
            this.spRptDoanhThu_NoiTru_TongHopTableAdapter.Fill(this.dsDoanhThuTongHopNoiTru1.spRptDoanhThu_NoiTru_TongHop, Convert.ToDateTime(this.parFromDate.Value), Convert.ToDateTime(this.parToDate.Value),Convert.ToInt32(this.parDeptID.Value));
        }
    }
}
