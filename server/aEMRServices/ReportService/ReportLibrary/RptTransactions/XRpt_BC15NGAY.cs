using System;
using System.Windows.Forms;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BC15NGAY : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BC15NGAY()
        {
            InitializeComponent();
        }

        private void XRpt_BC15NGAY_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            try
            {
                FillData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void FillData()
        {
            spBAOCAO15DAYTableAdapter.Fill(dsBaoCao15Day1.spBAOCAO15DAY, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(DeptID.Value), Convert.ToInt32(SearchID.Value));
            DateTime FromDate = Convert.ToDateTime(this.FromDate.Value);
            DateTime ToDate = Convert.ToDateTime(this.ToDate.Value);
            xrLabel8.Text = " TỪ NGÀY " + FromDate.Day + " / " + FromDate.Month + " ĐẾN " + ToDate.Day + " / " + ToDate.Month + " / " + ToDate.Year;
            var data = dsBaoCao15Day1.spBAOCAO15DAY;
            if (data != null && data.Rows.Count > 0)
            {
                if (data.Rows[0]["DateNV"] != DBNull.Value && Convert.ToDateTime(data.Rows[0]["DateNV"]) > DateTime.MinValue)
                {
                    DateTime NGAYNV = Convert.ToDateTime(data.Rows[0]["DateNV"]);
                    if ((FromDate <= NGAYNV) && (NGAYNV <= ToDate))
                    {
                        FromDate = NGAYNV;
                    }
                }
            }

            ngay1.Text = "" + FromDate.Day + "/" + FromDate.Month + "";
            ngay2.Text = "" + FromDate.AddDays(1).Day + "/" + FromDate.AddDays(1).Month + "";
            ngay3.Text = "" + FromDate.AddDays(2).Day + "/" + FromDate.AddDays(2).Month + "";
            ngay4.Text = "" + FromDate.AddDays(3).Day + "/" + FromDate.AddDays(3).Month + "";
            ngay5.Text = "" + FromDate.AddDays(4).Day + "/" + FromDate.AddDays(4).Month + "";
            ngay6.Text = "" + FromDate.AddDays(5).Day + "/" + FromDate.AddDays(5).Month + "";
            ngay7.Text = "" + FromDate.AddDays(6).Day + "/" + FromDate.AddDays(6).Month + "";
            ngay8.Text = "" + FromDate.AddDays(7).Day + "/" + FromDate.AddDays(7).Month + "";
            ngay9.Text = "" + FromDate.AddDays(8).Day + "/" + FromDate.AddDays(8).Month + "";
            ngay10.Text = "" + FromDate.AddDays(9).Day + "/" + FromDate.AddDays(9).Month + "";
            ngay11.Text = "" + FromDate.AddDays(10).Day + "/" + FromDate.AddDays(10).Month + "";
            ngay12.Text = "" + FromDate.AddDays(11).Day + "/" + FromDate.AddDays(11).Month + "";
            ngay13.Text = "" + FromDate.AddDays(12).Day + "/" + FromDate.AddDays(12).Month + "";
            ngay14.Text = "" + FromDate.AddDays(13).Day + "/" + FromDate.AddDays(13).Month + "";
            ngay15.Text = "" + FromDate.AddDays(14).Day + "/" + FromDate.AddDays(14).Month + "";
        }
    }
}
