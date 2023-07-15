using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_HeSoAnToanBan : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_HeSoAnToanBan()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            baoCao_HeSoAnToanBanTableAdapter.Fill((this.DataSource as DataSchema.ReportGeneral.dsHeSoAnToanBan).BaoCao_HeSoAnToanBan,Convert.ToInt64(this.StoreID.Value));
        }

        private void XRpt_HeSoAnToanBan_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
