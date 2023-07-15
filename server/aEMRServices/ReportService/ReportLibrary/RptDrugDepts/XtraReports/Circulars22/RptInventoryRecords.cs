using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Circulars22
{
    public partial class RptInventoryRecords : DevExpress.XtraReports.UI.XtraReport
    {
        public RptInventoryRecords()
        {
            InitializeComponent();
        }

        private void RptInventoryRecords_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            ReportSqlProvider.Instance.ReaderIntoSchema(dsInventoryRecords1.spRpt_Circulars22_InventoryRecords
                , spRpt_Circulars22_InventoryRecordsTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
                , Convert.ToInt64(V_MedProductType.Value)
            }, int.MaxValue);
        }
    }
}
