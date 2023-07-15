using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class BaoCaoChiTietVienPhi : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoChiTietVienPhi()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            spRpt_BaoCaoChiTietVienPhiTableAdapter.Fill(dsBaoCaoChiTietVienPhiMau041.spRpt_BaoCaoChiTietVienPhi, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value));
          
        }

        private void BaoCaoChiTietVienPhi_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
