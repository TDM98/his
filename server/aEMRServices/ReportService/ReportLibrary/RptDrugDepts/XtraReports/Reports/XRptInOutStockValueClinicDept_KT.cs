using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockValueClinicDept_KT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValueClinicDept_KT()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStocksClinicDept_KT1.spRpt_ClinicDept_InOutStocks_ByStoreID_KT
                , spRpt_ClinicDept_InOutStocks_ByStoreID_KTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value.ToString())
                , Convert.ToDateTime(ToDate.Value.ToString())
                , Convert.ToInt64(V_MedProductType.Value)
                , Convert.ToInt64(RefGenDrugCatID_1.Value)
                , Convert.ToInt64(DrugDeptProductGroupReportTypeID.Value)
            }, int.MaxValue);
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
