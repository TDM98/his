using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockClinicDept()
        {
            InitializeComponent();
           
        }
        public void FillData()
        {
            spRpt_ClinicDept_InOutStocksTableAdapter.Fill(dsInOutStocksClinicDept1.spRpt_ClinicDept_InOutStocks, Convert.ToInt64(this.StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(this.ToDate.Value.ToString()), Convert.ToInt64(this.V_MedProductType.Value), Convert.ToBoolean(this.ViewBefore20150331.Value), Convert.ToInt32(this.RefGenDrugCatID_1.Value), Convert.ToInt64(this.DrugDeptProductGroupReportTypeID.Value));
            if (dsInOutStocksClinicDept1.spRpt_ClinicDept_InOutStocks.Count == 0)
            {
                dsInOutStocksClinicDept1.spRpt_ClinicDept_InOutStocks.Rows.Add(new object[10] { 0, 0, 0, 0, 0, 0, 0, "", "", "" });
            }
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
