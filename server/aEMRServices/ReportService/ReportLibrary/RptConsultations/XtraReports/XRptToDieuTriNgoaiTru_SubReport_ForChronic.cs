using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptToDieuTriNgoaiTru_SubReport_ForChronic : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptToDieuTriNgoaiTru_SubReport_ForChronic()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsToDieuTriNgoaiTru_ForChronic1.EnforceConstraints = false;
            spRptToDieuTriNgoaiTru_Sub_ForChronicTableAdapter1.Fill(dsToDieuTriNgoaiTru_ForChronic1.spRptToDieuTriNgoaiTru_Sub_ForChronic, (long)PtRegDetailID.Value);
        }

    }
}