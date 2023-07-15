using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class RptFinancialActivityTemp01 : DevExpress.XtraReports.UI.XtraReport
    {
        public RptFinancialActivityTemp01()
        {
            InitializeComponent();
        }
        private void RptFinancialActivityTemp01_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            xrTitle.Text = string.Format("HOẠT ĐỘNG TÀI CHÍNH NĂM {0}", Convert.ToDateTime(this.ToDate.Value).Year);
            switch (Convert.ToDateTime(this.ToDate.Value).Month)
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
            this.spFinancialActivityTemp01TableAdapter.Fill(this.dsFinancialActivityTemp011.spFinancialActivityTemp01, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value));
        }
    }
}
