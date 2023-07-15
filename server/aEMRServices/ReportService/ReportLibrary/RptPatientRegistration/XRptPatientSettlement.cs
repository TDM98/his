using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using AxLogging;
using DataEntities;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPatientSettlement : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientSettlement()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceipt HAM KHOI TAO");  

            InitializeComponent();
            PrintingSystem.ShowPrintStatusDialog = false;
        }
        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsPatientSettlement1.EnforceConstraints = false;

            sp_Rpt_PatientSettlementTableAdapter.ClearBeforeFill = true;
            sp_Rpt_PatientSettlementTableAdapter.Fill(dsPatientSettlement1.sp_Rpt_PatientSettlement, Convert.ToInt64(this.param_ID.Value), Convert.ToByte(this.param_flag.Value));


            decimal totalBN = 0;
            decimal totalBH = 0;
            decimal totalAmount = 0;

            string AmountString = "";
            string BHYTString="";


            if (dsPatientSettlement1.sp_Rpt_PatientSettlement != null && dsPatientSettlement1.sp_Rpt_PatientSettlement.Rows.Count > 0)
            {
                totalBN = (decimal)dsPatientSettlement1.sp_Rpt_PatientSettlement.Rows[0]["TotalPatientPayment"];
                totalBH = (decimal)dsPatientSettlement1.sp_Rpt_PatientSettlement.Rows[0]["TotalHIPayment"];
                totalAmount = totalBN + totalBH;

                string HICode = dsPatientSettlement1.sp_Rpt_PatientSettlement.Rows[0]["HICardNo"].ToString();
                if(!string.IsNullOrEmpty(HICode))
                {
                    BHYTString = string.Format(eHCMSResources.Z1489_G1_SoBHYT0BHChi1, HICode, (Convert.ToDecimal(dsPatientSettlement1.sp_Rpt_PatientSettlement.Rows[0]["PtInsuranceBenefit"].ToString()) * 100).ToString("N0"));
                }
            }
            if (totalBH != 0)
            {
                AmountString = totalBN.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower() + "(BH: " + totalBH.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower() + " /" + totalAmount.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower() + " )";
            }
            else
            {
                AmountString = totalBN.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower();
                BHYTString = "";
            }
            totalBN = Math.Round(totalBN, 0);

            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (totalBN < 0)
            {
                temp = 0 - totalBN;
                prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = totalBN;
                prefix = "";
            }

            Parameters["parame_BHYTString"].Value = BHYTString;
            Parameters["parame_AmountString"].Value = AmountString;
            this.Parameters["param_AmountInWords"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

    }
}
