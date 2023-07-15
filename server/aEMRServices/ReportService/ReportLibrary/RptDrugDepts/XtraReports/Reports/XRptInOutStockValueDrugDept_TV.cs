using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockValueDrugDept_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValueDrugDept_TV()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStocksDrugDept1.spRpt_DrugDept_InOutStocks, spRpt_DrugDept_InOutStocksTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value.ToString())
                , Convert.ToDateTime(ToDate.Value.ToString())
                , Convert.ToInt64(V_MedProductType.Value), true
                , Convert.ToInt64(RefGenDrugCatID_1.Value)
                , Convert.ToInt64(DrugDeptProductGroupReportTypeID.Value)
                , 0
                , BidID.Value as long?
                , true
            }, int.MaxValue);
            //if (Convert.ToInt16(BidID.Value) == 0)
            //{
            //    GroupHeader1.Visible = true;
            //    GroupHeader2.Visible = false;
            //} else
            //{
            //    GroupHeader1.Visible = false;
            //    GroupHeader2.Visible = true;
            //}
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
