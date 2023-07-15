using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes
{
    public partial class XRptPhieuKiemKeThuoc_ClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuKiemKeThuoc_ClinicDept()
        {
            InitializeComponent();
        }

        private void XRptPhieuKiemKeThuoc_ClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            this.dsClinicDeptStockTake1.EnforceConstraints = false;
            this.spClinicDeptStockTakeDetails_LoadTableAdapter.Fill(this.dsClinicDeptStockTake1.spClinicDeptStockTakeDetails_Load, Convert.ToInt32(this.ID.Value));
            this.spClinicDeptStockTakes_IDTableAdapter.Fill(this.dsClinicDeptStockTake1.spClinicDeptStockTakes_ID, Convert.ToInt32(this.ID.Value));
        }

    }
}
