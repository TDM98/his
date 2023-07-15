using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptDrugDeptWatchOutQty : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptWatchOutQty()
        {
            InitializeComponent();
        }
        public void FillData()
        {
             spRpt_DrugDeptWatchOutQtyTableAdapter.Fill(dsDrugDeptWatchOutQty1.spRpt_DrugDeptWatchOutQty,
                Convert.ToInt32(this.V_MedProductType.Value), 
                Convert.ToDateTime(this.FromDate.Value),
                Convert.ToDateTime(this.ToDate.Value),
                Convert.ToInt32(this.Quarter.Value),
                Convert.ToInt32(this.Month.Value),
                Convert.ToInt32(this.Year.Value),
                Convert.ToByte(this.Flag.Value));
        }

        private void XRptDrugDeptWatchOutQty_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
