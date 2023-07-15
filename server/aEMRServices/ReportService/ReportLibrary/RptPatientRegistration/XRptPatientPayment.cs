using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DataEntities;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public sealed partial class XRptPatientPayment : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPayment()
        {
            InitializeComponent();

            BeforePrint += XRptPatientPayment_BeforePrint;
            PrintingSystem.ShowPrintStatusDialog = false;
        }


        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            adapterGetReceipt.ClearBeforeFill = true;
            var paymentID = Parameters["param_PaymentID"].Value as int?;
            var FindPatient = Parameters["FindPatient"].Value as int?;
            //Danh cho noi tru
            var CashAdvanceID = Parameters["param_CashAdvanceID"].Value as int?;

            if (!paymentID.HasValue || paymentID.Value <= 0) 
                return;
            adapterGetReceipt.Fill(dsPayment1.spRpt_Patients_GetReceipt, paymentID, FindPatient, CashAdvanceID);
            if (dsPayment1.spRpt_Patients_GetReceipt == null || dsPayment1.spRpt_Patients_GetReceipt.Rows.Count <= 0)
                return;
            var total = dsPayment1.spRpt_Patients_GetReceipt.Rows[0]["PayAmount"] as decimal?;
            if(total.HasValue)
            {
                var cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                var converter = new NumberToLetterConverter();

                Parameters["param_AmountInWords"].Value = converter.Convert(total.Value.ToString("0.00"), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());

                var creditOrDebit = (short)dsPayment1.spRpt_Patients_GetReceipt.Rows[0]["CreditOrDebit"];
                if(creditOrDebit < 0)
                {
                    Parameters["param_PaymentReason"].Value = eHCMSResources.Z1339_G1_HoanTienVPhi;
                    Parameters["param_Name"].Value = eHCMSResources.R0495_G1_phCHI.ToUpper();
                }
                else
                {
                    var paymentType = dsPayment1.spRpt_Patients_GetReceipt.Rows[0]["V_PaymentType"] as long?;
                    if (paymentType.HasValue && paymentType.Value == (long)AllLookupValues.PaymentType.TAM_UNG)
                    {
                        Parameters["param_PaymentReason"].Value = eHCMSResources.Z1340_G1_UngTienVPhi;
                    }
                    else
                    {
                        Parameters["param_PaymentReason"].Value = eHCMSResources.Z1341_G1_TraTienVPhi;
                    }
                    Parameters["param_Name"].Value = eHCMSResources.R0511_G1_phThu.ToUpper();
                }
            }
            else
            {
                Parameters["param_Name"].Value = eHCMSResources.R0511_G1_phThu.ToUpper();
            }
        }
    }
}
