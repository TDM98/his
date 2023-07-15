using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRpt_BKXuatThuocTheoBN : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BKXuatThuocTheoBN()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBKXuatThuocTheoBN1.spRpt_BKXuatThuocTheoBN, spRpt_BKXuatThuocTheoBNTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString())
            }, int.MaxValue);
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
