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
    public partial class XRptOutPatientReceipt_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceipt_V2()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceipt HAM KHOI TAO");
            InitializeComponent();
            PrintingSystem.ShowPrintStatusDialog = false;
        }
        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (Convert.ToBoolean(this.parIsGenericPayment.Value))
            {
                this.xrLabel1.DataBindings.Clear();
                this.xrLabel2.DataBindings.Clear();
                this.xrLabel7.DataBindings.Clear();
                this.xrLabel8.DataBindings.Clear();
                this.xrLabel9.DataBindings.Clear();
                this.xrLabel10.DataBindings.Clear();
                this.xrLabel11.DataBindings.Clear();
                this.xrLabel12.DataBindings.Clear();
                this.xrLabel13.DataBindings.Clear();
                this.xrLabel17.DataBindings.Clear();
                this.xrLabel18.DataBindings.Clear();
                this.xrLabel19.DataBindings.Clear();

                //Xóa dữ liệu mặc định
                this.xrLabel1.Text = null;
                this.xrLabel5.Text = null;
                this.xrLabel2.Text = null;
                this.xrLabel7.Text = null;
                this.xrLabel8.Text = null;
                this.xrLabel9.Text = null;
                this.xrLabel10.Text = null;
                this.xrLabel11.Text = null;
                this.xrLabel12.Text = null;
                this.xrLabel13.Text = null;
                this.xrLabel14.Text = null;
                this.xrLabel15.Text = null;
                this.xrLabel16.Text = null;
                this.xrLabel17.Text = null;
                this.xrLabel18.Text = null;
                this.xrLabel19.Text = null;

                this.xrLabel8.Text = this.parGenPaymtReason.Value .ToString();
                this.xrLabel9.Text = this.parGenPaymtCode.Value.ToString();
                this.xrLabel10.Text = this.parGenPaymtCustAddr.Value.ToString();
                this.xrLabel6.Text = this.parGenPaymtOrgName != null ? this.parGenPaymtOrgName.Value.ToString() : "";
                this.xrLabel11.Text = this.parGenPaymtCustName.Value.ToString();
                this.xrLabel12.Text = "ĐT: " + this.parGenPaymtCustPhone.Value.ToString();
                this.xrLabel5.Text = eHCMSResources.N0036_G1_NSinh + ": " + this.parGenPaymtCustDOB.Value.ToString();
                this.xrLabel13.Text = this.parStaffName.Value.ToString();

                decimal PaymentAmount = 0;
                PaymentAmount = Convert.ToDecimal(this.parPaymentAmount.Value.ToString());
                PaymentAmount = Math.Round(PaymentAmount, 0);
                Parameters["parTotalAmount"].Value = PaymentAmount.ToString("N0") + " " + eHCMSResources.G1616_G1_VND.ToLower();
                Parameters["parTotalPatientPayment"].Value = PaymentAmount.ToString("N0") + string.Format(" {0}",  eHCMSResources.G1616_G1_VND.ToLower());

                NumberToLetterConverter converterforGenPay = new NumberToLetterConverter();
                decimal temp1 = 0;
                string readAmount = "";
                if (PaymentAmount < 0)
                {
                    temp1 = 0 - PaymentAmount;
                    readAmount = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp1 = PaymentAmount;
                    readAmount = "";
                }
                Parameters["parTotalPatientPaymentInWords"].Value = readAmount + converterforGenPay.Convert(temp1.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
                return;
            }
            this.parPaymentDate.Value = DateTime.Now;
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
            Parameters["parTotalPatientPayment"].Value = totalBN.ToString("N0") + string.Format(" {0}",  eHCMSResources.G1616_G1_VND.ToLower());
            Parameters["parTotalHIPayment"].Value = totalBHString;
            Parameters["parTotalAmount"].Value = totalAmount.ToString("N0") + eHCMSResources.G1616_G1_VND.ToLower();

            Parameters["parBHYTString"].Value = BHYTString;

            Parameters["parTotalPatientPaymentInWords"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

    }
}
