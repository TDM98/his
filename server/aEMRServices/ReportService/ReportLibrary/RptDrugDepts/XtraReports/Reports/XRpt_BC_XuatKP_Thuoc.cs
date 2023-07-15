using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRpt_BC_XuatKP_Thuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BC_XuatKP_Thuoc()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(ds_BC_XuatKhoaPhong1.spRptOutStocks_Drug_ByV_MedProductType, spRptOutStocks_Drug_ByV_MedProductTypeTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value.ToString()), Convert.ToDateTime(parToDate.Value.ToString()), Convert.ToInt32(Quarter.Value), Convert.ToInt32(Month.Value),
                Convert.ToInt32(Year.Value), Convert.ToInt32(flag.Value), Convert.ToInt64(V_MedProductType.Value), Convert.ToInt64(OutStoreID.Value), Convert.ToInt64(InStoreID.Value)
            }, int.MaxValue);
            //dsGeneralInOutStatistics1.EnforceConstraints = false;
            //spGeneralInOutStatisticsTableAdapter.Fill(dsGeneralInOutStatistics1.spGeneralInOutStatistics, Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString()), Convert.ToInt64(V_MedProductType.Value));
            //if (dsGeneralInOutStatistics1.spGeneralInOutStatistics.Count == 0)
            //{
            //    dsGeneralInOutStatistics1.spGeneralInOutStatistics.Rows.Add(new object[10] { 0, 0, 0, 0, 0, 0, 0, "", "","" });
            //}
        }
        
        private void XRpt_BC_XuatKP_Thuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
