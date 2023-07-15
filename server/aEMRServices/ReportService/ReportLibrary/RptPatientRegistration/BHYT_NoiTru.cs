using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class BHYT_NoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public BHYT_NoiTru()
        {
            InitializeComponent();
            BeforePrint += BHYT_NoiTru_BeforePrint;
        }
        void BHYT_NoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        public void FillData()
        {
            this.dsBHYT_NoiTru1.EnforceConstraints = false;

            spRpt_HealthInsurance_ConfirmHiItem_InPtTableAdapter.ClearBeforeFill = true;
            var registrationID = Parameters["param_RegistrationID"].Value as int?;
            if (!registrationID.HasValue || registrationID.Value <= 0)
                return;
            spRpt_HealthInsurance_ConfirmHiItem_InPtTableAdapter.Fill(dsBHYT_NoiTru1.spRpt_HealthInsurance_ConfirmHiItem_InPt, registrationID, Convert.ToInt32(this.parServiceRecID.Value));
            if (dsBHYT_NoiTru1.spRpt_HealthInsurance_ConfirmHiItem_InPt == null || dsBHYT_NoiTru1.spRpt_HealthInsurance_ConfirmHiItem_InPt.Rows.Count <= 0)
                return;

            var firstRow = dsBHYT_NoiTru1.spRpt_HealthInsurance_ConfirmHiItem_InPt.Rows[0];
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
