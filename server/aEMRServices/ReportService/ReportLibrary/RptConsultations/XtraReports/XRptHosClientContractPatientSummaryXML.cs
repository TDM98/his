using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptHosClientContractPatientSummaryXML : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptHosClientContractPatientSummaryXML()
        {
            InitializeComponent();
        }
        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (HosContractPtIDCollection.Value == null)
            {
                return;
            }
            long[] CurrentHosContractPtIDCollection;
            try
            {
                CurrentHosContractPtIDCollection = HosContractPtIDCollection.Value.ToString().Split('|').Select(x => Convert.ToInt64(x)).ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            DataTable HosContractTB = new DataTable();
            HosContractTB.Columns.Add("HosContractPtID", typeof(long));
            foreach (var Item in CurrentHosContractPtIDCollection)
            {
                HosContractTB.Rows.Add(new object[] { Item });
            }
            this.DataSource = HosContractTB;
        }
        private void RptHosClientContractPatientSummary_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptHosClientContractPatientSummary)((XRSubreport)sender).ReportSource).Parameters["HosContractPtID"].Value = GetCurrentColumnValue("HosContractPtID") as long?;
        }
    }
}