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
    public partial class XRptOutPatientReceiptAuto : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceiptAuto()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceiptAuto HAM KHOI TAO");  

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
            string AmountString = "";
            string BHYTString="";
            if (outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID != null && outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows.Count > 0)
            {
                foreach (DataRow row in outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows)
                {
                    if (!string.IsNullOrEmpty(strservices))
                    {
                        strservices += ", ";
                    }
                    strservices += row["ServiceName"].ToString();// string.Format("{0} [{1}]", row["ServiceName"], ((decimal)row["PatientAmount"]).ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower());
                    //if (!string.IsNullOrEmpty(row["SpecialNote"].ToString()))
                    //{
                    //    strservices += " - " + row["SpecialNote"].ToString();
                    //}
                    totalBH += (decimal)row["Amount"] - (decimal)row["PatientAmount"];
                    totalAmount += (decimal)row["Amount"];
                    totalBN += (decimal)row["PatientAmount"];

                }
                string HICode=outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["HICardNo"].ToString();
                if(!string.IsNullOrEmpty(HICode))
                {
                    BHYTString = string.Format(eHCMSResources.Z1489_G1_SoBHYT0BHChi1, HICode, (Convert.ToDecimal(outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["PtInsuranceBenefit"].ToString()) * 100).ToString("N0"));
                }
                Parameters["parame_SequenceNumberString"].Value = AxHelper.GetSequenceNumber((byte)outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["ServiceSeqNumType"],(int)outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["ServiceSeqNum"]);
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
            Parameters["parame_ServiceString"].Value = strservices;
            this.Parameters["param_AmountInWords"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

    }
}
