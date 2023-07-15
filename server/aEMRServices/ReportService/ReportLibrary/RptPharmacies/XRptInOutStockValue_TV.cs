using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptInOutStockValue_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValue_TV()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStockValue1.spRpt_InOutStockValue, spRpt_InOutStockValueTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString())
            }, int.MaxValue);
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
