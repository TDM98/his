using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes
{
    public partial class XRptDrugDeptStockTakes_Get_Thuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptStockTakes_Get_Thuoc()
        {
            InitializeComponent();
        }

        private void XRptDrugDeptStockTakes_Get_Thuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            this.dsRptDrugDeptStockTakes_Get1.EnforceConstraints = false;
            this.spRptDrugDeptStockTakes_GetTableAdapter.Fill(this.dsRptDrugDeptStockTakes_Get1.spRptDrugDeptStockTakes_Get, Convert.ToInt64(this.ID.Value));
        }

    }
}
