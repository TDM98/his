using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class BaoCaoTheoDoiCongNoMedDept : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoTheoDoiCongNoMedDept()
        {
            InitializeComponent();
            //FillData();
        }

        private void FillData()
        {
            baoCao_DrugDept_TheoDoiCongNo_BaseDepotTableAdapter.Fill(dsTheoDoiCongNoMedDept1.BaoCao_DrugDept_TheoDoiCongNo_BaseDepot, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.Flag.Value), Convert.ToByte(this.Type.Value), Convert.ToInt64(this.V_MedProductType.Value));
        }
        private void BaoCaoTheoDoiCongNo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
