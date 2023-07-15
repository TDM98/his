using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions.XtraReports.Accountant
{
    public partial class XRptDrugDeptCardStorage : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptCardStorage()
        {
            InitializeComponent();
        }

        //private void FillData()
        //{
        //    dsDrugDept_CardStorage_KT1.EnforceConstraints = false;
        //    spRpt_DrugDept_CardStorage_KTTableAdapter.Fill(dsDrugDept_CardStorage_KT1.spRpt_DrugDept_CardStorage_KT
        //        , Convert.ToInt64(StoreID.Value), Convert.ToInt64(GenMedProductID.Value)
        //        , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(V_MedProductType.Value));
        //}

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsDrugDept_CardStorage_KT1.spRpt_DrugDept_CardStorage_KT, spRpt_DrugDept_CardStorage_KTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToInt64(GenMedProductID.Value), Convert.ToDateTime(FromDate.Value)
                , Convert.ToDateTime(ToDate.Value), Convert.ToInt64(V_MedProductType.Value)
            }, int.MaxValue);
        }

        private void XRptCardStorage_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (dsDrugDept_CardStorage_KT1.spRpt_DrugDept_CardStorage_KT != null && dsDrugDept_CardStorage_KT1.spRpt_DrugDept_CardStorage_KT.Rows.Count > 0)
            {
                Parameters["StockFinal"].Value = Convert.ToDecimal(dsDrugDept_CardStorage_KT1.spRpt_DrugDept_CardStorage_KT.Rows[dsDrugDept_CardStorage_KT1.spRpt_DrugDept_CardStorage_KT.Rows.Count - 1]["QtyStocks"]);
            }
        }
    }
}
