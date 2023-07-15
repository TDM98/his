using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptTransactions.XtraReports.Accountant
{
    public partial class XRptClinicDeptCardStorage : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptClinicDeptCardStorage()
        {
            InitializeComponent();
        }

        //private void FillData()
        //{
        //    spRpt_ClinicDept_CardStorage_KTTableAdapter.Fill(dsClinicDept_CardStorage_KT1.spRpt_ClinicDept_CardStorage_KT
        //        , Convert.ToInt64(StoreID.Value), Convert.ToInt64(GenMedProductID.Value)
        //        , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
        //        , Convert.ToInt64(V_MedProductType.Value)
        //        );
        //}

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsClinicDept_CardStorage_KT1.spRpt_ClinicDept_CardStorage_KT, spRpt_ClinicDept_CardStorage_KTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToInt64(GenMedProductID.Value), Convert.ToDateTime(FromDate.Value)
                , Convert.ToDateTime(ToDate.Value), Convert.ToInt64(V_MedProductType.Value)
            }, int.MaxValue);
        }

        private void XRptCardStorage_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (dsClinicDept_CardStorage_KT1.spRpt_ClinicDept_CardStorage_KT != null && dsClinicDept_CardStorage_KT1.spRpt_ClinicDept_CardStorage_KT.Rows.Count > 0)
            {
                Parameters["StockFinal"].Value = Convert.ToDecimal(dsClinicDept_CardStorage_KT1.spRpt_ClinicDept_CardStorage_KT.Rows[dsClinicDept_CardStorage_KT1.spRpt_ClinicDept_CardStorage_KT.Rows.Count - 1]["QtyStocks"]);
            }
        }
    }
}
