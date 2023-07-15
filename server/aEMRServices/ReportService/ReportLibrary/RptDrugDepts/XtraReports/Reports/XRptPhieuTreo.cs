using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptPhieuTreo : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuTreo()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsRptPhieuTreo1.spRptPhieuTreo
                 , spRptPhieuTreoTableAdapter.Adapter.GetFillParameters(), new object[] {
                    Convert.ToDateTime(FromDate.Value.ToString()),
                    Convert.ToDateTime(ToDate.Value.ToString()),
                    Convert.ToInt32(Quarter.Value),
                    Convert.ToInt32(Month.Value),
                    Convert.ToInt32(Year.Value),
                    Convert.ToByte(Flag.Value),
                    Convert.ToInt32(StoreID.Value)
             }, int.MaxValue);
        }
        private void XRptPhieuTreo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}