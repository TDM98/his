using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptTransactions.XtraReports.Accountant
{
    public partial class XRptPharmacyCardStorage : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPharmacyCardStorage()
        {
            InitializeComponent();
        }

        //private void FillData()
        //{
        //    spRpt_Pharmacy_CardStorage_KTTableAdapter.Fill(dsPharmacy_CardStorage_KT1.spRpt_Pharmacy_CardStorage_KT
        //        , Convert.ToInt64(StoreID.Value), Convert.ToInt64(GenMedProductID.Value)
        //        , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value));
        //}

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsPharmacy_CardStorage_KT1.spRpt_Pharmacy_CardStorage_KT, spRpt_Pharmacy_CardStorage_KTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToInt64(GenMedProductID.Value), Convert.ToDateTime(FromDate.Value)
                , Convert.ToDateTime(ToDate.Value)
            }, int.MaxValue);
        }

        private void XRptPharmacyCardStorage_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (dsPharmacy_CardStorage_KT1.spRpt_Pharmacy_CardStorage_KT != null && dsPharmacy_CardStorage_KT1.spRpt_Pharmacy_CardStorage_KT.Rows.Count > 0)
            {
                Parameters["StockFinal"].Value = Convert.ToDecimal(dsPharmacy_CardStorage_KT1.spRpt_Pharmacy_CardStorage_KT.Rows[dsPharmacy_CardStorage_KT1.spRpt_Pharmacy_CardStorage_KT.Rows.Count - 1]["QtyStocks"]);
            }
        }
    }
}
