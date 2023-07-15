using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class RptHISStaffsInfo : DevExpress.XtraReports.UI.XtraReport
    {
        public RptHISStaffsInfo()
        {
            InitializeComponent();
        }
        private void RptHISStaffsInfo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int mMonth = Convert.ToInt32(this.Month.Value);
            int mYear = Convert.ToInt32(this.Year.Value);
            xrTitle.Text = string.Format("TÌNH HÌNH CÁN BỘ, CÔNG CHỨC, VIÊN CHỨC THÁNG {0} NĂM {1}", mMonth, this.Year.Value);
            switch (mMonth)
            {
                case 1:
                case 2:
                case 3:
                    xrLabelSubTitle.Text = string.Format("(Kỳ hạn báo cáo: {0} tháng)", 3);
                    break;
                case 4:
                case 5:
                case 6:
                    xrLabelSubTitle.Text = string.Format("(Kỳ hạn báo cáo: {0} tháng)", 6);
                    break;
                case 7:
                case 8:
                case 9:
                    xrLabelSubTitle.Text = string.Format("(Kỳ hạn báo cáo: {0} tháng)", 9);
                    break;
                case 10:
                case 11:
                case 12:
                    xrLabelSubTitle.Text = string.Format("(Kỳ hạn báo cáo: {0} tháng)", 12);
                    break;
            }
            if (mMonth < 12)
            {
                mMonth = mMonth + 1;
            }
            else
            {
                mYear = mYear + 1;
                mMonth = 1;
            }
            spHISStaffInfosTableAdapter.Fill(dsHISStaffsInfo1.spHISStaffInfos, new DateTime(mYear, mMonth, 1), false);
        }
        private void xrTableRowDetail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (GetCurrentColumnValue("Group2Name").ToString() == "I"
                || GetCurrentColumnValue("Group2Name").ToString() == "II"
                || GetCurrentColumnValue("Group2Name").ToString() == "III"
                || GetCurrentColumnValue("Group2Name").ToString() == "IV")
            {
                xrTableCellGroup2DeptName.Font = new Font("Arial", 8.25f, FontStyle.Bold);
                xrTableCellGroup2Name.Font = new Font("Arial", 8.25f, FontStyle.Bold);
            }
            else if (GetCurrentColumnValue("Group2Name").ToString() == "2.1"
                || GetCurrentColumnValue("Group2Name").ToString() == "2.2"
                || GetCurrentColumnValue("Group2Name").ToString() == "2.3"
                || GetCurrentColumnValue("Group2Name").ToString() == "2.4")
            {
                xrTableCellGroup2DeptName.Font = new Font("Arial", 8.25f, FontStyle.Italic);
                xrTableCellGroup2Name.Font = new Font("Arial", 8.25f, FontStyle.Italic);
            }
            else
            {
                xrTableCellGroup2DeptName.Font = new Font("Arial", 8.25f, FontStyle.Regular);
                xrTableCellGroup2Name.Font = new Font("Arial", 8.25f, FontStyle.Regular);
            }
        }
    }
}
