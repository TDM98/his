using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_SOXETNGHIEM : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_SOXETNGHIEM()
        {
            InitializeComponent();
        }

        private void XRpt_SOXETNGHIEM_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        public void FillData()
        {
            spSoXetNghiemTableAdapter.Fill(ds_SoXetNghiem1.spSoXetNghiem, Convert.ToDateTime(this.ToDate.Value), Convert.ToDateTime(this.FromDate.Value));
        }

    }
}
