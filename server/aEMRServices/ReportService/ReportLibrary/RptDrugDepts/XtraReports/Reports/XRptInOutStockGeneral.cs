using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockGeneral : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockGeneral()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsGeneralInOutStatistics1.spGeneralInOutStatistics, spGeneralInOutStatisticsTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString()), Convert.ToInt64(V_MedProductType.Value)
            }, int.MaxValue);
            //dsGeneralInOutStatistics1.EnforceConstraints = false;
            //spGeneralInOutStatisticsTableAdapter.Fill(dsGeneralInOutStatistics1.spGeneralInOutStatistics, Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString()), Convert.ToInt64(V_MedProductType.Value));
            //if (dsGeneralInOutStatistics1.spGeneralInOutStatistics.Count == 0)
            //{
            //    dsGeneralInOutStatistics1.spGeneralInOutStatistics.Rows.Add(new object[10] { 0, 0, 0, 0, 0, 0, 0, "", "","" });
            //}
        }
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
