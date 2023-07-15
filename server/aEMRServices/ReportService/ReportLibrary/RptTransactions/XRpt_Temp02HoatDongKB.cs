using System;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp02HoatDongKB : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp02HoatDongKB()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spBaoCao_HoatDongKBTableAdapter.Fill(dsTemp02HoatDongKB1.spBaoCao_HoatDongKB, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value));
        }
        private void XRpt_Temp02HoatDongKB_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            switch (Convert.ToDateTime(this.ToDate.Value).Month)
            {
                case 1:
                case 2:
                case 3:
                    xrLabel1.Text = "(Kỳ hạn báo cáo: 3 tháng)";
                    break;
                case 4:
                case 5:
                case 6:
                    xrLabel1.Text = "(Kỳ hạn báo cáo: 6 tháng)";
                    break;
                case 7:
                case 8:
                case 9:
                    xrLabel1.Text = "(Kỳ hạn báo cáo: 9 tháng)";
                    break;
                case 10:
                case 11:
                case 12:
                    xrLabel1.Text = "(Kỳ hạn báo cáo: 12 tháng)";
                    break;
            }
            //this.xrLabel1.Text = "( Kỳ hạn báo cáo : " + Convert.ToDateTime(this.ToDate.Value).Month + " tháng )";
            this.xrLabel9.Text = "" + Convert.ToDateTime(this.ToDate.Value).Year + "";
        }
    }
}