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
    public partial class XRptOutPatientReceiptAuto_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceiptAuto_V2()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceipt HAM KHOI TAO");  

            InitializeComponent();
            PrintingSystem.ShowPrintStatusDialog = false;
        }
        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            outPatientReceipt1.EnforceConstraints = false;
            sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.ClearBeforeFill=true;
            sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.Fill(outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID, Convert.ToInt64(this.param_PaymentID.Value), null);
            decimal totalBN = 0;
            decimal totalBH = 0;
            decimal totalAmount = 0;
            string strservices = "";
            string totalBHString = "";
            string BHYTString = "";
            if (outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID != null && outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows.Count > 0)
            {
                foreach (DataRow row in outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows)
                {
                    if (!string.IsNullOrEmpty(strservices))
                    {
                        strservices += ", ";
                    }
                    strservices += row["ServiceName"].ToString();

                    totalAmount += (decimal)row["Amount"];
                    totalBN += (decimal)row["PatientAmount"];
                    totalBH += (decimal)row["Amount"] - (decimal)row["PatientAmount"];

                }
                string HICode = outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["HICardNo"].ToString();
                if(!string.IsNullOrEmpty(HICode))
                {
                    BHYTString = string.Format(eHCMSResources.Z1489_G1_SoBHYT0BHChi1, HICode, (Convert.ToDecimal(outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["PtInsuranceBenefit"].ToString()) * 100).ToString("N0"));
                }
                Parameters["parame_SequenceNumberString"].Value = AxHelper.GetSequenceNumber((byte)outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["ServiceSeqNumType"],(int)outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["ServiceSeqNum"]);
            }
            if (totalBH != 0)
            {
                totalBHString = string.Format("{0}: ", eHCMSResources.R0072_G1_BHYTChi) + totalBH.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower() + "/ " + totalAmount.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower();
            }
            else
            {
                totalBHString = "";
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

            Parameters["parServiceString"].Value = strservices;
            Parameters["parTotalPatientPayment"].Value = totalBN.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower();
            Parameters["parTotalHIPayment"].Value = totalBHString;
            Parameters["parTotalAmount"].Value = totalAmount.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower();

            Parameters["parBHYTString"].Value = BHYTString;

            Parameters["parTotalPatientPaymentInWords"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

    }
}
