using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptToDieuTriNgoaiTru_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptToDieuTriNgoaiTru_SubReport()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRptToDieuTriNgoaiTru1.EnforceConstraints = false;
            spRptToDieuTriNgoaiTru_SubTableAdapter.Fill(dsRptToDieuTriNgoaiTru1.spRptToDieuTriNgoaiTru_Sub, PtRegDetailID.Value as long?);
        }
    }
}
