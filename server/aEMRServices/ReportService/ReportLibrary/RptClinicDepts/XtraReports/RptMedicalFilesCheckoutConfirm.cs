using System;
namespace eHCMS.ReportLib.RptClinicDepts.XtraReports
{
    public partial class RptMedicalFilesCheckoutConfirm : DevExpress.XtraReports.UI.XtraReport
    {
        public RptMedicalFilesCheckoutConfirm()
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
            spGetMedicalFileFromRegistrationTableAdapter.Fill(dsMedicalFilesList1.spGetMedicalFileFromRegistration, 0, -1, this.StartDate == null ? null : (DateTime?)Convert.ToDateTime(this.StartDate.Value), this.EndDate == null ? null : (DateTime?)Convert.ToDateTime(this.EndDate.Value), false, true, this.Registrations == null ? null : this.Registrations.Value.ToString(), Convert.ToInt32(this.PtRegistrationID.Value), Convert.ToInt32(this.MedicalFileStorageCheckID.Value), 0);
        }
    }
}
