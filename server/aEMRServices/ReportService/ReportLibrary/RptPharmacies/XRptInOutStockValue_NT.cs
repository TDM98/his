using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptInOutStockValue_NT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValue_NT()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStocks_NT1.spRpt_InOutStocks_NT, spRpt_InOutStocks_NTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString())
            }, int.MaxValue);
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
