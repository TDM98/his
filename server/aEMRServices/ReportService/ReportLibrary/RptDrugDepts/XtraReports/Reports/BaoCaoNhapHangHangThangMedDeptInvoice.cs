using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class BaoCaoNhapHangHangThangMedDeptInvoice_V1 : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoNhapHangHangThangMedDeptInvoice_V1()
        {
            InitializeComponent();
            //FillData();
        }

        private void FillData()
        {
            baoCao_NhapHangHangThangMedDeptInvoiceTableAdapter.Fill(dsBaoCaoNhapHangHangThangMedDeptInvoice1.BaoCao_NhapHangHangThangMedDeptInvoice, Convert.ToInt32(this.StoreID.Value), Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.Flag.Value),Convert.ToInt32(this.V_MedProductType.Value));
        }
        private void BaoCaoNhapHangHangThangMedDeptInvoice_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
