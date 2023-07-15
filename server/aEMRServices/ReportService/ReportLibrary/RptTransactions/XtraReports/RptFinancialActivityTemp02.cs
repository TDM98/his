using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Linq;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class RptFinancialActivityTemp02 : DevExpress.XtraReports.UI.XtraReport
    {
        public RptFinancialActivityTemp02()
        {
            InitializeComponent();
        }
        private void RptHISTotalPayments_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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

            DateTime mFromDate = Convert.ToDateTime(this.FromDate.Value);
            DateTime mToDate = Convert.ToDateTime(this.ToDate.Value);
            spRptFinancialActivityTemp02TableAdapter.Fill(dsHISTotalPayments1.spRptFinancialActivityTemp02, mFromDate, mToDate);

            DataSchema.dsFinancialActivityTemp02TableAdapters.spGetRptFinancialActivityTemp02OutAmountTableAdapter spGetRptFinancialActivityTemp02OutAmountTableAdapter = new DataSchema.dsFinancialActivityTemp02TableAdapters.spGetRptFinancialActivityTemp02OutAmountTableAdapter();
            spGetRptFinancialActivityTemp02OutAmountTableAdapter.Fill(dsHISTotalPayments1.spGetRptFinancialActivityTemp02OutAmount, mFromDate, mToDate);
        }
    }
}
