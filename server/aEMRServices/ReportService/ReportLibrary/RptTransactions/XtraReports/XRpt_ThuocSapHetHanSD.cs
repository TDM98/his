using System;
namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_ThuocSapHetHanSD : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ThuocSapHetHanSD()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsThuocSapHetHanSD1.EnforceConstraints = false;
            sp_ThuocSapHetHanSDTableAdapter.Fill(dsThuocSapHetHanSD1.sp_ThuocSapHetHanSD
                , Convert.ToInt64(StoreID.Value)
                , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
                , Convert.ToInt64(V_MedProductType.Value));
            //spOutwardDrug_ByIDInvoiceTableAdapter.Fill((DataSource as DataSchema.dsXuatNoiBo).spOutwardDrug_ByIDInvoice, Convert.ToInt64(OutiID.Value));
            //spOutwardDrugInvoices_ByIDVisitorTableAdapter.Fill((DataSource as DataSchema.dsXuatNoiBo).spOutwardDrugInvoices_ByIDVisitor, Convert.ToInt64(OutiID.Value));
        }

        private void XRpt_ThuocSapHetHanSD_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
