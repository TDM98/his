using System;
using System.Data;
using AxLogging;
using eHCMS.Services.Core;
using eHCMSLanguage;

/*
 * 20181026 TNHX #001: [0004198] Update report to show data from parameter which sent from client
 * 20181026 TNHX #002: [BM0002176] Change report from "Hoa don" to "Bien lai thu tien". Disable function "In Hoa Don"
 */
namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutPatientReceipt_V4_InNhiet : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceipt_V4_InNhiet()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceipt HAM KHOI TAO");
            InitializeComponent();
        }

        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (Convert.ToBoolean(parIsGenericPayment.Value))
            {
                //▼====: #001
                sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.ClearBeforeFill = true;
                // quyen loi BHYT
                xrLabel2.ExpressionBindings.Clear();
                xrLabel2.Text = null;
                // receipt number
                xrLabel9.ExpressionBindings.Clear();
                xrLabel9.Text = parGenPaymtCode.Value.ToString();
                //full name
                xrLabel33.ExpressionBindings.Clear();
                xrLabel33.Text = parGenPaymtCustName.Value.ToString();
                // patient code
                xrLabel35.ExpressionBindings.Clear();
                xrLabel35.Text = parPatientCode != null ? parPatientCode.Value.ToString() : "";
                // age
                xrLabel4.ExpressionBindings.Clear();
                xrLabel4.Text = parGenPaymtCustDOB.Value.ToString();
                // gender
                xrLabel7.ExpressionBindings.Clear();
                xrLabel7.Text = null;
                // patient address
                xrLabel41.ExpressionBindings.Clear();
                xrLabel41.Text = parGenPaymtCustAddr.Value.ToString();
                // STT
                xrLabel17.ExpressionBindings.Clear();
                xrLabel17.Text = null;
                //[Parameters.parTotalPTPaymentAfterVAT!#,#]
                xrLabel22.ExpressionBindings.Clear();
                xrLabel22.Text = null;
                //[Parameters].[parTotalPatientPaymentInWords]
                xrLabel20.ExpressionBindings.Clear();
                xrLabel20.Text = null;
                // staff
                xrLabel31.ExpressionBindings.Clear();
                xrLabel31.Text = parStaffName.Value.ToString();
                //// total amount
                //xrTableCell6.ExpressionBindings.Clear();
                //xrTableCell6.Text = null;
                //// total Hi amount
                //xrTableCell7.ExpressionBindings.Clear();
                //xrTableCell7.Text = null;
                // total patien before VAT
                xrTableCell16.ExpressionBindings.Clear();
                xrTableCell16.Text = null;

                //xrTableCell9.ExpressionBindings.Clear();
                //xrTableCell9.Text = parGenPaymtReason.Value.ToString();
                //xrTableCell11.ExpressionBindings.Clear();
                //xrTableCell11.Text = parTotalAmount.Value.ToString();
                //xrTableCell12.ExpressionBindings.Clear();
                //xrTableCell12.Text = parTotalAmount.Value.ToString();
                //xrTableCell13.ExpressionBindings.Clear();
                //xrTableCell13.Text = parTotalAmount.Value.ToString();
                //xrTableCell14.ExpressionBindings.Clear();
                //xrTableCell14.Text = parTotalAmount.Value.ToString();

                // du lieu chua biet bo voa dau
                //this.xrLabel4.Text = this.parPaymentDate != null ? this.parPaymentDate.Value.ToString() : "";
                //this.xrLabel6.Text = this.parGenPaymtOrgName != null ? this.parGenPaymtOrgName.Value.ToString() : "";

                decimal TotalPTPaymentBeforeVAT = 0;
                decimal TotalPTPaymentAfterVAT = 0;

                TotalPTPaymentBeforeVAT = Convert.ToDecimal(parTotalPTPaymentBeforeVAT.Value.ToString());
                TotalPTPaymentBeforeVAT = Math.Round(TotalPTPaymentBeforeVAT, 0);
                TotalPTPaymentAfterVAT = Convert.ToDecimal(parTotalPTPaymentAfterVAT.Value.ToString());
                TotalPTPaymentAfterVAT = Math.Round(TotalPTPaymentAfterVAT, 0);
                Parameters["parTotalAmount"].Value = TotalPTPaymentBeforeVAT.ToString("N0");
                //Parameters["parTotalPatientPayment"].Value = PaymentAmount.ToString("N0") + string.Format(" {0}",  eHCMSResources.G1616_G1_VND.ToLower());

                Parameters["parTotalPTPaymentBeforeVAT"].Value = TotalPTPaymentBeforeVAT.ToString("N0");
                Parameters["parTotalPTPaymentAfterVAT"].Value = TotalPTPaymentAfterVAT.ToString("N0");
                if (String.IsNullOrEmpty(parVATPercent.Value.ToString()))
                {
                    Parameters["parVATPercent"].Value = "-";
                }
                else
                {
                    Parameters["parVATPercent"].Value = parVATPercent.Value.ToString();
                }
                if (String.IsNullOrEmpty(parVATAmount.Value.ToString()))
                {
                    Parameters["parVATAmount"].Value = "-";
                }
                else
                {
                    decimal VATAmount = 0;
                    VATAmount = Convert.ToDecimal(parVATAmount.Value.ToString());
                    Parameters["parVATAmount"].Value = VATAmount.ToString("N0");
                }

                NumberToLetterConverter converterforGenPay = new NumberToLetterConverter();
                decimal temp1 = 0;
                string readAmount = "";
                if (TotalPTPaymentAfterVAT < 0)
                {
                    temp1 = 0 - TotalPTPaymentAfterVAT;
                    readAmount = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp1 = TotalPTPaymentAfterVAT;
                    readAmount = "";
                }
                Parameters["parTotalPatientPaymentInWords"].Value = readAmount + converterforGenPay.Convert(temp1.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());

                
                xrLabel7.Text = Parameters["parBHYTString"].Value.ToString();
                xrTableCell16.Text = Parameters["parTotalPTPaymentBeforeVAT"].Value.ToString();
                xrLabel22.Text = Parameters["parTotalPTPaymentBeforeVAT"].Value.ToString();
                //xrTableCell14.Text = Parameters["parTotalPTPaymentBeforeVAT"].Value.ToString();
                //xrTableCell12.Text = Parameters["parVATAmount"].Value.ToString();
                //xrTableCell11.Text = Parameters["parVATAmount"].Value.ToString();
                //string totalBHYT = Convert.ToDecimal(Convert.ToDouble(Parameters["parVATAmount"].Value.ToString()) - Convert.ToDouble(Parameters["parTotalPTPaymentAfterVAT"].Value.ToString())).ToString("N0");
                //xrTableCell13.Text = totalBHYT;
                //xrTableCell7.Text = totalBHYT;
                xrLabel20.Text = Parameters["parTotalPatientPaymentInWords"].Value.ToString();
                //▲====: #001
                return;
            }
            //▼====: #002
            parPaymentDate.Value = DateTime.Now;
            outPatientReceipt1.EnforceConstraints = false;
            sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.ClearBeforeFill = true;
            sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDTableAdapter.Fill(outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID, Convert.ToInt64(param_PaymentID.Value), Convert.ToInt64(pOutPtCashAdvanceID.Value));
            decimal totalBN = 0;
            decimal totalBH = 0;
            decimal totalAmount = 0;
            string totalBHString = "";
            string quyenLoiAndDoiTuong = "";
            if (outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID != null && outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows.Count > 0)
            {
                if (outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["ChungTuGoc"].ToString() == "")
                {
                    xrLabel37.Visible = false;
                }
                foreach (DataRow row in outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows)
                {
                    totalAmount += (decimal)row["Amount"];
                    totalBN += (decimal)row["PatientAmount"];
                    totalBH += (decimal)row["Amount"] - (decimal)row["PatientAmount"] - (decimal)row["DiscountAmount"];
                }
                string HICode = outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["HICardNo"].ToString();
                if (!string.IsNullOrEmpty(HICode))
                {
                    quyenLoiAndDoiTuong = HICode.Substring(2, 1) + " - " + (Convert.ToDecimal(outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentID.Rows[0]["PtInsuranceBenefit"].ToString()) * 100).ToString("N0") + "%";
                }
            }
            if (totalBH != 0)
            {
                totalBHString = totalBH.ToString("N0");
            }
            else
            {
                quyenLoiAndDoiTuong = "Viện phí";
            }

            totalBN = Math.Round(totalBN, 0);

            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (totalBN < 0)
            {
                temp = 0 - totalBN;
                prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = totalBN;
                prefix = "";
            }

            Parameters["parTotalPTPaymentBeforeVAT"].Value = totalBN.ToString("N0");
            Parameters["parTotalPTPaymentAfterVAT"].Value = Parameters["parTotalPTPaymentBeforeVAT"].Value;
            Parameters["parVATPercent"].Value = "-";
            Parameters["parVATAmount"].Value = "-";
            Parameters["parTotalHIPayment"].Value = totalBHString;
            Parameters["parTotalAmount"].Value = totalAmount.ToString("N0");
            Parameters["parBHYTString"].Value = quyenLoiAndDoiTuong;
            Parameters["parTotalPatientPaymentInWords"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            //▲====: #002
        }

        void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //if (GetCurrentColumnValue("IsDeleted") != null && GetCurrentColumnValue("IsDeleted") != DBNull.Value && GetCurrentColumnValue("IsDeleted").ToString() == "True")
            //{
            //    xrLine1.Visible = true;
            //}
            //else
            //{
            //    xrLine1.Visible = false;
            //}
        }
    }
}
