using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class BaoCaoTheoDoiCongNo : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoTheoDoiCongNo()
        {
            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            baoCao_TheoDoiCongNoTableAdapter.Fill(dsTheoDoiCongNo1.BaoCao_TheoDoiCongNo,Convert.ToInt64(this.StoreID.Value), Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.Flag.Value), Convert.ToByte(this.Type.Value));
        }
        private void BaoCaoTheoDoiCongNo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
