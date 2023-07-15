using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockValueClinicDept_KT_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValueClinicDept_KT_V2()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            dsInOutStocksClinicDept_KT_V21.EnforceConstraints = false;
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStocksClinicDept_KT_V21.spRpt_ClinicDept_InOutStocks_ByStoreID_KT_V2
                , spRpt_ClinicDept_InOutStocks_ByStoreID_KT_V2TableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value.ToString())
                , Convert.ToDateTime(ToDate.Value.ToString())
                , Convert.ToInt64(V_MedProductType.Value)
                , Convert.ToInt64(RefGenDrugCatID_1.Value)
                , Convert.ToInt64(DrugDeptProductGroupReportTypeID.Value)
            }, int.MaxValue);
        }

        private void XRptInOutStocksV2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
