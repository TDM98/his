using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptHISSummaryFollowByICD_New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptHISSummaryFollowByICD_New()
        {
            InitializeComponent();
        }

        private void XRptHISSummaryFollowByICD_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
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
            DateTime mStartDate = Convert.ToDateTime(FromDate.Value);
            DateTime mEndDate = Convert.ToDateTime(ToDate.Value);
            Year.Value = mStartDate.Year;
            xrTitle.Text = string.Format("TÌNH HÌNH BỆNH TẬT, TỬ VONG TẠI BỆNH VIỆN THEO ICD NĂM {0}", mEndDate.Year);
            spGetHISSummaryByICD_NewTableAdapter.Fill(dsGetHISSummaryByICD_New1.spGetHISSummaryByICD_New, mStartDate, mEndDate, false);
        }
    }
}
