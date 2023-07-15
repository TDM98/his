using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.Managers.PharmacySellingPriceList
{
    public partial class XRptPharmacySellingPriceList_Detail : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPharmacySellingPriceList_Detail()
        {
            InitializeComponent();
        }

        private void XRptPharmacySellingPriceList_Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            this.dsPharmacySellingPriceList_Detail1.EnforceConstraints = false;
            this.spPharmacySellingPriceList_DetailTableAdapter.Fill(this.dsPharmacySellingPriceList_Detail1.spPharmacySellingPriceList_Detail, Convert.ToInt32(this.parPharmacySellingPriceListID.Value));
        }

    }
}
