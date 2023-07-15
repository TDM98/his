using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class BaoCaoChiTietVienPhi_PK_Mau02 : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoChiTietVienPhi_PK_Mau02()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            spRpt_BaoCaoChiTietVienPhi_PKTableAdapter.Fill(dsBaoCaoChiTietVienPhiPKMau021.spRpt_BaoCaoChiTietVienPhi_PK, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value));
        }

        private void BaoCaoChiTietVienPhi_PK_Mau02_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
