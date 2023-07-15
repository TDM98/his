using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptTransactions.XtraReports.Accountant
{
    public partial class XRpt_HangTonNhieu : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_HangTonNhieu()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCHangTonNhieu1.spRpt_KT_BCHangTonNhieu_ByStoreID, spRpt_KT_BCHangTonNhieu_ByStoreIDTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString()), Convert.ToInt64(V_MedProductType.Value)
                , Convert.ToInt64(DrugDeptProductGroupReportTypeID.Value)
            }, int.MaxValue);
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
