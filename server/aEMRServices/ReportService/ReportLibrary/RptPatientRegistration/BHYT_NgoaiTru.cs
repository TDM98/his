using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class BHYT_NgoaiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public BHYT_NgoaiTru()
        {
            InitializeComponent();
            BeforePrint += BHYT_NgoaiTru_BeforePrint;
            PrintingSystem.ShowPrintStatusDialog = false;
        }
        void BHYT_NgoaiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        public void FillData()
        {
            this.eHCMS_DB_DEVDataSet11.EnforceConstraints = false;

            spRpt_HealthInsurance_ConfirmHiItemTableAdapter.ClearBeforeFill = true;
            //var registrationID = Parameters["param_RegistrationID"].Value as int?;
            long registrationID = Convert.ToInt32(this.param_RegistrationID.Value);
            if (registrationID <= 0)
                return;
            spRpt_HealthInsurance_ConfirmHiItemTableAdapter.Fill(eHCMS_DB_DEVDataSet11.spRpt_HealthInsurance_ConfirmHiItem, registrationID, Convert.ToInt32(this.parServiceRecID.Value));
            if (eHCMS_DB_DEVDataSet11.spRpt_HealthInsurance_ConfirmHiItem == null || eHCMS_DB_DEVDataSet11.spRpt_HealthInsurance_ConfirmHiItem.Rows.Count <= 0)
                return;

            var firstRow = eHCMS_DB_DEVDataSet11.spRpt_HealthInsurance_ConfirmHiItem.Rows[0];
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
