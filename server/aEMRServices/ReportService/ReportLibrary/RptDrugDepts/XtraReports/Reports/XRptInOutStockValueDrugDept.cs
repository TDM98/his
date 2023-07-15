using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockValueDrugDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValueDrugDept()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            dsInOutStocksDrugDept1.Clear();
            spRpt_DrugDept_InOutStocksTableAdapter.Fill(dsInOutStocksDrugDept1.spRpt_DrugDept_InOutStocks
                , Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value.ToString())
                , Convert.ToDateTime(ToDate.Value.ToString())
                , Convert.ToInt64(V_MedProductType.Value), true
                , Convert.ToInt64(RefGenDrugCatID_1.Value)
                , Convert.ToInt64(DrugDeptProductGroupReportTypeID.Value), 0
                , null, true);
            if (dsInOutStocksDrugDept1.spRpt_DrugDept_InOutStocks.Count == 0)
            {
                dsInOutStocksDrugDept1.spRpt_DrugDept_InOutStocks.Rows.Add(new object[10] { 0, 0, 0, 0, 0, 0, 0, "", "","" });
            }
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
