namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptHosClientContractPatientSummary : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptHosClientContractPatientSummary()
        {
            InitializeComponent();
        }
        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptHosClientContractPatientSummaryTableAdapter.Fill(dsHosClientContractPatientSummary1.spRptHosClientContractPatientSummary, HosContractPtID.Value as long?);
        }
    }
}