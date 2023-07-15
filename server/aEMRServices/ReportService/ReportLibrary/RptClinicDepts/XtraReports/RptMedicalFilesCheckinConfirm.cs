using System;
using DevExpress.XtraReports.UI;
namespace eHCMS.ReportLib.RptClinicDepts.XtraReports
{
    public partial class RptMedicalFilesCheckinConfirm : DevExpress.XtraReports.UI.XtraReport
    {
        public RptMedicalFilesCheckinConfirm()
        {
            InitializeComponent();
        }
        private void RptMedicalFilesCheckoutConfirm_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            if (this.StartDate != null && Convert.ToDateTime(this.StartDate.Value) == DateTime.MinValue)
                this.StartDate = null;
            if (this.EndDate != null && Convert.ToDateTime(this.EndDate.Value) == DateTime.MinValue)
                this.EndDate = null;
            spGetMedicalFileFromRegistrationTableAdapter.Fill(dsMedicalFilesList1.spGetMedicalFileFromRegistration, 0, -1, this.StartDate == null ? null : (DateTime?)Convert.ToDateTime(this.StartDate.Value), this.EndDate == null ? null : (DateTime?)Convert.ToDateTime(this.EndDate.Value), true, true, this.Registrations == null ? null : this.Registrations.Value.ToString(), Convert.ToInt32(this.PtRegistrationID.Value), Convert.ToInt32(this.MedicalFileStorageCheckID.Value), 0);
        }
        string mReceiveName = "";
        private void ReceiveName_RowChanged(object sender, EventArgs e)
        {
            mReceiveName = GetCurrentColumnValue("ReceiveName").ToString();
        }
        private void ReceiveName_GetResult(object sender, SummaryGetResultEventArgs e)
        {
            e.Result = mReceiveName;
            e.Handled = true;
        }
        private void ReceiveName_Reset(object sender, EventArgs e)
        {
            mReceiveName = "";
        }
    }
}