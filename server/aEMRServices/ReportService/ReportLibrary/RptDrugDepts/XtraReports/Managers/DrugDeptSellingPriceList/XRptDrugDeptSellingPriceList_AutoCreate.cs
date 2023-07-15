using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Managers.DrugDeptSellingPriceList
{
    public partial class XRptDrugDeptSellingPriceList_AutoCreate : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptSellingPriceList_AutoCreate()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            this.dsDrugDeptSellingPriceList_AutoCreate1.EnforceConstraints = false;
            string str= Convert.ToString(this.parResult);
            str = "";
            this.spDrugDeptSellingPriceList_AutoCreateTableAdapter.Fill(this.dsDrugDeptSellingPriceList_AutoCreate1.spDrugDeptSellingPriceList_AutoCreate, Convert.ToInt32(this.parV_MedProductType.Value), ref str);
        }

        private void XRptDrugDeptSellingPriceList_AutoCreate_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        

    }
}
