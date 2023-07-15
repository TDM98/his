using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Managers.DrugDeptSellingPriceList
{
    public partial class XRptDrugDeptSellingPriceList_Detail : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptSellingPriceList_Detail()
        {
            InitializeComponent();
        }

        private void XRptDrugDeptSellingPriceList_Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsDrugDeptSellingPriceList_Detail1.spDrugDeptSellingPriceList_Detail, spDrugDeptSellingPriceList_DetailTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt32(parDrugDeptSellingPriceListID.Value)
            }, int.MaxValue);
        }
    }
}
