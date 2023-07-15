using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.StockTakes
{
    public partial class XRptPhieuKiemKeHoaChat : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuKiemKeHoaChat()
        {
            InitializeComponent();
        }

        private void XRptPhieuKiemKeHoaChat_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsRptDrugDeptStockTakes_Get1.EnforceConstraints = false;
            spRptDrugDeptStockTakes_GetTableAdapter.Fill(dsRptDrugDeptStockTakes_Get1.spRptDrugDeptStockTakes_Get, Convert.ToInt32(ID.Value));
            spDrugDeptStockTakes_IDTableAdapter.Fill(dsRptDrugDeptStockTakes_Get1.spDrugDeptStockTakes_ID, Convert.ToInt32(ID.Value));
        }
    }
}
