using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptToDieuTriNgoaiTru_SubReport_ForMaxillofacial : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptToDieuTriNgoaiTru_SubReport_ForMaxillofacial()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsToDieuTriNgoaiTru_ForMaxillofacial1.EnforceConstraints = false;
            spRptToDieuTriNgoaiTru_Sub_ForMaxillofacialTableAdapter.Fill(dsToDieuTriNgoaiTru_ForMaxillofacial1.spRptToDieuTriNgoaiTru_Sub_ForMaxillofacial, (long)PtRegDetailID.Value);
        }

    }
}