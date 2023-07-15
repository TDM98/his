using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class RptTreatmentStatistics : DevExpress.XtraReports.UI.XtraReport
    {
        public RptTreatmentStatistics()
        {
            InitializeComponent();
        }
        private void RptTreatmentStatistics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            xrLabelTitle.Text = string.Format("HOẠT ĐỘNG ĐIỀU TRỊ NĂM {0}", Convert.ToDateTime(this.ToDate.Value).Year);
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
            this.spGetTreatmentStatisticsTableAdapter.Fill(this.dsTreatmentStatistics1.spGetTreatmentStatistics, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToBoolean(this.IsSpecialist.Value));
            if (Convert.ToBoolean(this.IsSpecialist.Value))
            {
                lbReportType.Text = "Biểu 03.2-ĐTCK";
            }
        }
        private void lbSpecialistTitle_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (!Convert.ToBoolean(this.IsSpecialist.Value))
            {
                e.Cancel = true;
            }
        }
    }
}
