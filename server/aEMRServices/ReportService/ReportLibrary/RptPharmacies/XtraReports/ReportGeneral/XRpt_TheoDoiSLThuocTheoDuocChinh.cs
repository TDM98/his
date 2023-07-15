using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_TheoDoiSLThuocTheoDuocChinh : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TheoDoiSLThuocTheoDuocChinh()
        {
            InitializeComponent();
           
        }
        public void FillData()
        {
            dsTheoDoiSoLuongThuocTheoDuocChinh1.Clear();
            spRpt_InOutStocks_DrugClassTableAdapter.Fill(dsTheoDoiSoLuongThuocTheoDuocChinh1.spRpt_InOutStocks_DrugClass, Convert.ToInt32(Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToInt64(this.DrugClassID.Value),Convert.ToInt64(this.RefGenDrugCatID_1.Value), Convert.ToInt64(this.StoreID.Value));
            if (dsTheoDoiSoLuongThuocTheoDuocChinh1.spRpt_InOutStocks_DrugClass.Rows.Count == 0)
            {
                dsTheoDoiSoLuongThuocTheoDuocChinh1.spRpt_InOutStocks_DrugClass.Rows.Add(new object[11] { 0, 0, 0, 0, 0, 0, 0,"","","","" });
            }
        }

        private void XRpt_TheoDoiSLThuocTheoDuocChinh_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
