using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Managers.DrugDeptSellingPriceList
{
    public partial class XRptDrugDeptSellingPriceListYCuHoaChat_Detail : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptSellingPriceListYCuHoaChat_Detail()
        {
            
            InitializeComponent();
        }

        private void FillData()
        {
            this.dsDrugDeptSellingPriceList_Detail1.EnforceConstraints = false;
            this.spDrugDeptSellingPriceList_DetailTableAdapter.Fill(this.dsDrugDeptSellingPriceList_Detail1.spDrugDeptSellingPriceList_Detail,Convert.ToInt32(this.parDrugDeptSellingPriceListID.Value));
        }

        private void XRptDrugDeptSellingPriceList_Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
