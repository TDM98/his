using System;
using System.Data;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptInPtAndrologyRecord : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInPtAndrologyRecord()
        {
            InitializeComponent();
        }
        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptInPtAndrologyRecordTableAdapter.Fill(dsInPtAndrologyRecord1.spRptInPtAndrologyRecord, PtRegistrationID.Value as long?, V_RegistrationType.Value as long?);
            spRptInPtAndrologyRecordPCLDetailsTableAdapter.Fill(dsInPtAndrologyRecord1.spRptInPtAndrologyRecordPCLDetails, PtRegistrationID.Value as long?, V_RegistrationType.Value as long?);
            spRptInPtAndrologyRecordPrescriptionDetailsTableAdapter.Fill(dsInPtAndrologyRecord1.spRptInPtAndrologyRecordPrescriptionDetails, PtRegistrationID.Value as long?, V_RegistrationType.Value as long?);
            if (dsInPtAndrologyRecord1.spRptInPtAndrologyRecordPrescriptionDetails.Rows == null
                || dsInPtAndrologyRecord1.spRptInPtAndrologyRecordPrescriptionDetails.Rows.Count == 0
                || !dsInPtAndrologyRecord1.spRptInPtAndrologyRecordPrescriptionDetails.Rows.Cast<DataRow>().Any(x => x["V_DrugType"] != null && x["V_DrugType"] != DBNull.Value && x["V_DrugType"].ToString() == "53203"))
            {
                DetailReport5.Visible = false;
            }
            else
            {
                DetailReport5.Visible = true;
            }
        }
    }
}