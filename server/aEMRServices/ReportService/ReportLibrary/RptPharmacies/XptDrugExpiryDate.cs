using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XptDrugExpiryDate : DevExpress.XtraReports.UI.XtraReport
    {
        public XptDrugExpiryDate()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            spLoadDrugExpiryDateTableAdapter.Fill((this.DataSource as DataSchema.dsDrugExpiryDate).spLoadDrugExpiryDate,Convert.ToInt64(StoreID.Value),Convert.ToInt16(Type.Value));
        }
        private void XptDrugExpiryDate_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
