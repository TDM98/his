using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRpt_XuatNoiBo : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_XuatNoiBo()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spOutwardDrug_ByIDInvoiceTableAdapter.Fill((this.DataSource as DataSchema.dsXuatNoiBo).spOutwardDrug_ByIDInvoice, Convert.ToInt64(this.OutiID.Value));
            spOutwardDrugInvoices_ByIDVisitorTableAdapter.Fill((this.DataSource as DataSchema.dsXuatNoiBo).spOutwardDrugInvoices_ByIDVisitor, Convert.ToInt64(this.OutiID.Value));
        }

        private void XRpt_XuatNoiBo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
