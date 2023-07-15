using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.Managers.PharmacySellingPriceList
{
    public partial class XRptPharmacySellingPriceList_AutoCreate : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPharmacySellingPriceList_AutoCreate()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            this.dsPharmacySellingPriceList_AutoCreate1.EnforceConstraints = false;
            string str= Convert.ToString(this.parResult);
            str = "";
            this.spPharmacySellingPriceList_AutoCreateTableAdapter.Fill(this.dsPharmacySellingPriceList_AutoCreate1.spPharmacySellingPriceList_AutoCreate, ref str);
        }

        private void XRptPharmacySellingPriceList_AutoCreate_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        

    }
}
