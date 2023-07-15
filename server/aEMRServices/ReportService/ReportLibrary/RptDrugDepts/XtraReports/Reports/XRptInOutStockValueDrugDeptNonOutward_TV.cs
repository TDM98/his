using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockValueDrugDeptNonOutward_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValueDrugDeptNonOutward_TV()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStocksDrugDept_NonExport1.spRpt_DrugDept_InOutStocks_ByStoreID
                , spRpt_DrugDept_InOutStocks_ByStoreIDTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value.ToString())
                , Convert.ToDateTime(ToDate.Value.ToString())
                , Convert.ToInt64(V_MedProductType.Value), true
                , Convert.ToInt64(RefGenDrugCatID_1.Value)
                , Convert.ToInt64(DrugDeptProductGroupReportTypeID.Value)
                , 0
                , BidID.Value as long?
                , 1
            }, int.MaxValue);
        }

        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
