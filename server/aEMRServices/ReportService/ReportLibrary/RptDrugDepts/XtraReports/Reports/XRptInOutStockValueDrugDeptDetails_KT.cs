using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockValueDrugDeptDetails_KT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValueDrugDeptDetails_KT()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsDrugDept_InOutStocks_Details1.spRpt_DrugDept_InOutStocks_Details_ByStoreID, spRpt_DrugDept_InOutStocks_Details_ByStoreIDTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value.ToString())
                , Convert.ToDateTime(ToDate.Value.ToString())
                , Convert.ToInt64(V_MedProductType.Value), true
                , Convert.ToInt64(RefGenDrugCatID_1.Value)
                , Convert.ToInt64(DrugDeptProductGroupReportTypeID.Value)
                , 0
                , BidID.Value as long?
            }, int.MaxValue);
        }

        private void XRptInOutStockValueDrugDeptDetails_KT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
