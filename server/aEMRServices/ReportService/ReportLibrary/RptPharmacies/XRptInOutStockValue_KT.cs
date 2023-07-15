using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptInOutStockValue_KT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockValue_KT()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsInOutStockValue_KT1.spRpt_InOutStockValue_KT, spRpt_InOutStockValue_KTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString())
            }, int.MaxValue);
        }

        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
