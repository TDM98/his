using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptInOutStockDrugDeptAddictiveNew : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInOutStockDrugDeptAddictiveNew()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptInOutStockDrugDeptAddictiveNew_BeforePrint);
        }

        public void FillData()
        {

            spRpt_DrugDept_InOutStocks_AddictiveTableAdapter.Fill(dsInOutStocksAddictive1.spRpt_DrugDept_InOutStocks_Addictive
                                                                , this.Year.Value as int?,Convert.ToInt64(this.RefGenDrugCatID.Value),Convert.ToInt64(this.V_MedProductType.Value));
        }

        private void XRptInOutStockDrugDeptAddictiveNew_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //spRpt_DrugDept_InOutStocks_AddictiveTableAdapter.Fill(dsInOutStocksAddictive1.spRpt_DrugDept_InOutStocks_Addictive, this.Year.Value as int?,Convert.ToInt64(this.RefGenDrugCatID.Value),Convert.ToInt64(this.V_MedProductType.Value));
            FillData();
        }
    }
}
