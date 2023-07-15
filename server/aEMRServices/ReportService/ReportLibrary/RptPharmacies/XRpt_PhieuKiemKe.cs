using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRpt_PhieuKiemKe : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PhieuKiemKe()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spPharmacyStockTakes_IDTableAdapter.Fill((this.DataSource as DataSchema.dsPhieuKiemKe).spPharmacyStockTakes_ID, Convert.ToInt64(this.ID.Value));
            spPharmacyStockTakesDetails_LoadTableAdapter.Fill((this.DataSource as DataSchema.dsPhieuKiemKe).spPharmacyStockTakesDetails_Load, Convert.ToInt64(this.ID.Value));
        }

        private void XRpt_PhieuKiemKe_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
