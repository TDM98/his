using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_MauSapHetHanDung : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_MauSapHetHanDung()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsMauSapHetHanDung1.EnforceConstraints = false;
            sp_MauSapHetHanDungTableAdapter.Fill(dsMauSapHetHanDung1.sp_MauSapHetHanDung
                , Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
                , Convert.ToInt64(V_MedProductType.Value));
        }

        private void XRpt_ThuocSapHetHanSD_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
