using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class RptFinancialActivityTemp03 : DevExpress.XtraReports.UI.XtraReport
    {
        public RptFinancialActivityTemp03()
        {
            InitializeComponent();
        }
        private void RptFinancialActivityTemp03_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DateTime mFromDate = Convert.ToDateTime(this.FromDate.Value);
            DateTime mToDate = Convert.ToDateTime(this.ToDate.Value);
            xrTitle.Text = string.Format("HOẠT ĐỘNG TÀI CHÍNH NĂM {0}", mToDate.Year);
            switch (mToDate.Month)
            {
                case 1:
                case 2:
                case 3:
                    lbSubTitle.Text = "(Kỳ hạn báo cáo: 3 tháng)";
                    break;
                case 4:
                case 5:
                case 6:
                    lbSubTitle.Text = "(Kỳ hạn báo cáo: 6 tháng)";
                    break;
                case 7:
                case 8:
                case 9:
                    lbSubTitle.Text = "(Kỳ hạn báo cáo: 9 tháng)";
                    break;
                case 10:
                case 11:
                case 12:
                    lbSubTitle.Text = "(Kỳ hạn báo cáo: 12 tháng)";
                    break;
            }
            spFinancialActivityTemp03TableAdapter.Fill(dsFinancialActivityTemp031.spFinancialActivityTemp03, mFromDate, mToDate);
        }
    }
}
