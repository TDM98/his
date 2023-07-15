using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptToDieuTriNgoaiTru_CLS_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptToDieuTriNgoaiTru_CLS_SubReport()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRptToDieuTriNgoaiTru1.EnforceConstraints = false;
            spRptPCLRequestForDieuTriNgoaiTruTableAdapter.Fill(dsRptToDieuTriNgoaiTru1.spRptPCLRequestForDieuTriNgoaiTru, PtRegDetailID.Value as long?);
            if (dsRptToDieuTriNgoaiTru1.spRptPCLRequestForDieuTriNgoaiTru.Count == 0)
            {
                ReportHeader.Visible = false;
            }
        }
    }
}
