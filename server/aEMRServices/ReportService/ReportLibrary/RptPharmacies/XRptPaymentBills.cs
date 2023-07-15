using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptPaymentBills : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPaymentBills()
        {
            InitializeComponent();

            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptPatientPayment_BeforePrint);
            this.PrintingSystem.ShowPrintStatusDialog = false;
        }


        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            spRpt_Patients_GetReceiptDrugVisistorTableAdapter.ClearBeforeFill = true;
            spRpt_Patients_GetReceiptDrugVisistorTableAdapter.Fill(dsReceiptDrugVisistor1.spRpt_Patients_GetReceiptDrugVisistor, Convert.ToInt64(this.param_PaymentID.Value), Convert.ToInt64(this.OutiID.Value), Convert.ToInt64(this.V_TranRefType.Value));
            if (dsReceiptDrugVisistor1.spRpt_Patients_GetReceiptDrugVisistor != null && dsReceiptDrugVisistor1.spRpt_Patients_GetReceiptDrugVisistor.Rows.Count > 0)
            {
                decimal? total = dsReceiptDrugVisistor1.spRpt_Patients_GetReceiptDrugVisistor.Rows[0]["PayAmount"] as decimal?;
                if (total.HasValue)
                {
                    System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                    NumberToLetterConverter converter = new NumberToLetterConverter();
                    decimal temp = 0;
                    string prefix = "";
                    if (total < 0)
                    {
                        temp = 0 - Math.Round(total.GetValueOrDefault());
                        prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                    }
                    else
                    {
                        temp = Math.Round(total.GetValueOrDefault());
                        prefix = "";
                    }
                    this.Parameters["param_AmountInWords"].Value = prefix + converter.Convert(temp.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
                }
            }
        }
    }
}
