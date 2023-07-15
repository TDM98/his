using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_ThongKeSoLuongThe : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ThongKeSoLuongThe()
        {
            InitializeComponent();
        }

        private void XRpt_ThongKeSoLuongThe_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_ThongKeSoLuongThe1.EnforceConstraints = false;
            spXRpt_ThongKeSoLuongTheTableAdapter.Fill(dsXRpt_ThongKeSoLuongThe1.spXRpt_ThongKeSoLuongThe
                , Convert.ToDateTime(parFromDate.Value));
        }
    }
}
