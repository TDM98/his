using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class GiayHenTaiKhamBHYT_InPt : DevExpress.XtraReports.UI.XtraReport
    {
        public GiayHenTaiKhamBHYT_InPt()
        {
            InitializeComponent();
            BeforePrint += GiayHenTaiKhamBHYT_InPt_BeforePrint;
        }
        void GiayHenTaiKhamBHYT_InPt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        public void FillData()
        {
            this.dsGiayHenTaiKhamBHYT_InPt1.EnforceConstraints = false;


            var registrationID = Parameters["param_RegistrationID"].Value as int?;
            var AppointmentID = Parameters["param_AppointmentID"].Value as int?;
            
            spRpt_HealthInsurance_ConfirmHiItem_InPtTableAdapter1.ClearBeforeFill = true;
            spRptPatientApptServiceDetails_DetailsTableAdapter1.ClearBeforeFill = true;
            spRpt_HealthInsurance_ConfirmHiItem_InPtTableAdapter1.Fill(dsGiayHenTaiKhamBHYT_InPt1.spRpt_HealthInsurance_ConfirmHiItem_InPt, registrationID, Convert.ToInt32(this.parServiceRecID.Value));
            spRptPatientApptServiceDetails_DetailsTableAdapter1.Fill(dsGiayHenTaiKhamBHYT_InPt1.spRptPatientApptServiceDetails_Details,
                AppointmentID);


            var firstRow = dsGiayHenTaiKhamBHYT_InPt1.spRpt_HealthInsurance_ConfirmHiItem_InPt.Rows[0];
            var yob = string.Empty;
            if (firstRow["DOB"] != DBNull.Value)
            {
                yob = ((DateTime)firstRow["DOB"]).Year.ToString();
            }
            Parameters["param_YOB"].Value = yob;

            var strBenefit = string.Empty;
            if (firstRow["HiBenefit"] != DBNull.Value)
            {
                double d;
                if (double.TryParse(firstRow["HiBenefit"].ToString(), out d))
                {
                    strBenefit = (Math.Round(d * 100)).ToString() + "%";
                }
            }

            Parameters["param_Benefit"].Value = strBenefit;
            var gender = firstRow["Gender"].ToString().ToUpper();
            gender = gender == "M" ? eHCMSResources.K0785_G1_1Nam : eHCMSResources.K0816_G1_2Nu;
            var patientName = firstRow["PatientName"] + ", " + gender;
            
            Parameters["param_PatientName"].Value = patientName;
        }
    }
}
