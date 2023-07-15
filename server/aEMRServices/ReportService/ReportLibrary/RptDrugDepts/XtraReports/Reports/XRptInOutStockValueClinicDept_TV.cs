using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockValueClinicDept_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValueClinicDept_TV()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStockValueClinicDept1.spRpt_ClinicDept_InOutStockValue
                , spRpt_ClinicDept_InOutStockValueTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value.ToString())
                , Convert.ToDateTime(ToDate.Value.ToString())
                , Convert.ToInt64(V_MedProductType.Value)
                , Convert.ToInt64(RefGenDrugCatID_1.Value)
                , Convert.ToInt64(DrugDeptProductGroupReportTypeID.Value)
                , 0
            }, int.MaxValue);
        }

        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
