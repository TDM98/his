using System;
namespace eHCMS.ReportLib.RptPharmacies.XtraReports.OutInternal
{
    public partial class XRpt_XuatNoiBo_KT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_XuatNoiBo_KT()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spOutwardDrug_ByIDInvoiceTableAdapter.Fill((DataSource as DataSchema.dsXuatNoiBo).spOutwardDrug_ByIDInvoice, Convert.ToInt64(OutiID.Value));
            spOutwardDrugInvoices_ByIDVisitorTableAdapter.Fill((DataSource as DataSchema.dsXuatNoiBo).spOutwardDrugInvoices_ByIDVisitor, Convert.ToInt64(OutiID.Value));
        }

        private void XRpt_XuatNoiBo_KT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
