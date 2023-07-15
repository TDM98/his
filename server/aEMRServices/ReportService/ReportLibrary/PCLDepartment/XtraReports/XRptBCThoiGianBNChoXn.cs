using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.PCLDepartment.XtraReports
{
    public partial class XRptBCThoiGianBNChoXn : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBCThoiGianBNChoXn()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            //dsBCThoiGianBNChoXN1.EnforceConstraints = false;
            //spRpt_BCThoiGianBNChoXNTableAdapter.Fill(dsBCThoiGianBNChoXN1.spRpt_BCThoiGianBNChoXN, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCThoiGianBNChoXN1.spRpt_BCThoiGianBNChoXN, spRpt_BCThoiGianBNChoXNTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value)
            }, int.MaxValue);
        }

        private void XRptBCThoiGianBNChoXn_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
