using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptCardStorageDrugDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptCardStorageDrugDept()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsCardStorageDrugDept1.EnforceConstraints = false;
            spRpt_DrugDept_CardStorageTableAdapter.Fill(dsCardStorageDrugDept1.spRpt_DrugDept_CardStorage,Convert.ToInt64(this.StoreID.Value), Convert.ToInt64(this.GenMedProductID.Value), Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt64(V_MedProductType.Value));
        }
        private void XRptCardStorage_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (dsCardStorageDrugDept1.spRpt_DrugDept_CardStorage != null && dsCardStorageDrugDept1.spRpt_DrugDept_CardStorage.Rows.Count > 0)
            {
                this.Parameters["StockFinal"].Value = Convert.ToDecimal(dsCardStorageDrugDept1.spRpt_DrugDept_CardStorage.Rows[dsCardStorageDrugDept1.spRpt_DrugDept_CardStorage.Rows.Count - 1]["QtyStocks"]);
            }
        }
    }
}
