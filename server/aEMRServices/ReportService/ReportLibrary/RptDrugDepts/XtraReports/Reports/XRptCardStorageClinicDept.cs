using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;


namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptCardStorageClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptCardStorageClinicDept()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            spRpt_ClinicDept_CardStorageTableAdapter.Fill(dsCardStorageClinicDept1.spRpt_ClinicDept_CardStorage, Convert.ToInt64(this.StoreID.Value), Convert.ToInt64(this.GenMedProductID.Value), Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt64(V_MedProductType.Value), Convert.ToBoolean(this.ViewBefore20150331.Value));
        }
        private void XRptCardStorage_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (dsCardStorageClinicDept1.spRpt_ClinicDept_CardStorage != null && dsCardStorageClinicDept1.spRpt_ClinicDept_CardStorage.Rows.Count > 0)
            {
                this.Parameters["StockFinal"].Value = Convert.ToDecimal(dsCardStorageClinicDept1.spRpt_ClinicDept_CardStorage.Rows[dsCardStorageClinicDept1.spRpt_ClinicDept_CardStorage.Rows.Count - 1]["QtyStocks"]);
            }
        }
    }
}
