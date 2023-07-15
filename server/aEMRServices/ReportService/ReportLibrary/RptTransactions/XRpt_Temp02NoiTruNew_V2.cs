using System;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
/*
 * 20171024 #001 CMN: Added TotalPatientPaymentPaidAmount
*/
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp02NoiTruNew : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp02NoiTruNew()
        {
            InitializeComponent();
        }

        decimal totalTamUng = 0;
        decimal totalNguonKhac = 0;
        bool IsHICard_FiveYearsCont_NoPaid = false;

        public void FillData()
        {
            string DefaultNumFilmsRecvStr = null;
            spRpt_CreateTemp02NoiTruNewTableAdapter.Fill(dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew
                , Convert.ToInt64(PtRegistrationID.Value), Convert.ToDateTime(FromDate.Value)
                , Convert.ToDateTime(ToDate.Value), Convert.ToInt32(DeptID.Value)
                , Convert.ToBoolean(ViewByDate.Value), Convert.ToBoolean(PrintNoChargeItem.Value)
                , 0, 24003, Convert.ToInt64(TransactionID.Value), ref DefaultNumFilmsRecvStr, false, false, false, false);
            spHealthInsuranceHistory_InPtTableAdapter.Fill(dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt
                , Convert.ToInt64(this.PtRegistrationID.Value), Convert.ToDateTime(this.DischargeDate.Value)
                , 24003, 0, 0, false, false);
            spRpt_GetPatientCashAdvanceTableAdapter.Fill(dsTemp02NoiTruNew1.spRpt_GetPatientCashAdvance, Convert.ToInt64(this.PtRegistrationID.Value), 24003);
            spRpt_GetCharitySupportFundTableAdapter.Fill(dsTemp02NoiTruNew1.spRpt_GetCharitySupportFund, Convert.ToInt64(this.PtRegistrationID.Value), 0, 24003);

            if (dsTemp02NoiTruNew1.spRpt_GetPatientCashAdvance != null && dsTemp02NoiTruNew1.spRpt_GetPatientCashAdvance.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemp02NoiTruNew1.spRpt_GetPatientCashAdvance.Rows.Count; i++)
                {
                    totalTamUng += Convert.ToDecimal(dsTemp02NoiTruNew1.spRpt_GetPatientCashAdvance.Rows[i]["PaymentAmount"]);
                }
            }

            if (dsTemp02NoiTruNew1.spRpt_GetCharitySupportFund != null && dsTemp02NoiTruNew1.spRpt_GetCharitySupportFund.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemp02NoiTruNew1.spRpt_GetCharitySupportFund.Rows.Count; i++)
                {
                    totalNguonKhac += Convert.ToDecimal(dsTemp02NoiTruNew1.spRpt_GetCharitySupportFund.Rows[i]["AmountValue"]);
                }
            }

            totalTamUng = Math.Round(totalTamUng, MidpointRounding.AwayFromZero);
            totalNguonKhac = Math.Round(totalNguonKhac, MidpointRounding.AwayFromZero);

            if (dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt != null && dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Rows.Count > 0)
            {
                IsHICard_FiveYearsCont_NoPaid = Convert.ToBoolean(dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Rows[0]["IsHICard_FiveYearsCont_NoPaid"]);
                /*▼====: #001*/
                if (dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Columns["TotalPatientPaymentPaidAmount"] != null && dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Rows[0]["TotalPatientPaymentPaidAmount"] != DBNull.Value)
                    gTotalPatientPaymentPaidAmount = Convert.ToDecimal(dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Rows[0]["TotalPatientPaymentPaidAmount"]);
                /*▲====: #001*/
            }
        }

        private void XRpt_Temp02NoiTruNew_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        decimal totalCP = 0;
        decimal totalBNTra = 0;
        decimal totalBHTra = 0;
        decimal totalBNPhaiDongThem = 0;
        /*▼====: #001*/
        decimal? gTotalPatientPaymentPaidAmount = null;
        /*▲====: #001*/

        private void ReportFooter1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            totalBNTra = totalCP - totalBHTra;

            totalBNPhaiDongThem = totalBNTra - (totalTamUng + totalNguonKhac);

            this.Parameters["TotalAmount"].Value = totalCP;
            this.Parameters["TotalHIPayment"].Value = totalBHTra;
            this.Parameters["TotalPatientPayment"].Value = totalBNTra;
            this.Parameters["TotalPatientCashAdvance"].Value = totalTamUng;
            this.Parameters["TotalCharitySupport"].Value = totalNguonKhac;
            this.Parameters["TotalPatientExtraPayment"].Value = Math.Abs(totalBNPhaiDongThem);

            if (totalBNPhaiDongThem > 0)
            {
                this.Parameters["PaymentTypeName"].Value = string.Format("{0} :", eHCMSResources.Z1377_G1_SoTienBNDongBoSung);
            }
            else
            {
                this.Parameters["PaymentTypeName"].Value = string.Format("{0} :", eHCMSResources.Z1378_G1_SoTienHoanTraBN);
            }
            /*▼====: #001*/
            if (gTotalPatientPaymentPaidAmount > 0)
            {
                this.Parameters["TotalPatientPaymentPaidName"].Value = "Số tiền đã trả bệnh nhân:";
            }
            else
            {
                this.Parameters["TotalPatientPaymentPaidName"].Value = null;
            }
            /*▲====: #001*/

            System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp1 = 0;
            string prefix1 = "";
            if (totalCP < 0)
            {
                temp1 = 0 - totalCP;
                prefix1 = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp1 = totalCP;
                prefix1 = "";
            }
            this.Parameters["ReadMoneyTongCP"].Value = prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            decimal temp2 = 0;
            string prefix2 = "";
            if (totalBNTra < 0)
            {
                temp2 = 0 - totalBNTra;
                prefix2 = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp2 = totalBNTra;
                prefix2 = "";
            }
            this.Parameters["ReadMoneyBNTra"].Value = prefix2 + converter.Convert(temp2.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            decimal temp3 = 0;
            string prefix3 = "";
            if (totalBHTra < 0)
            {
                temp3 = 0 - totalBHTra;
                prefix3 = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp3 = totalBHTra;
                prefix3 = "";
            }
            this.Parameters["ReadMoneyBHTra"].Value = prefix3 + converter.Convert(temp3.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());

            decimal temp4 = 0;
            string prefix4 = "";
            if (totalTamUng < 0)
            {
                temp4 = 0 - totalTamUng;
                prefix4 = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp4 = totalTamUng;
                prefix4 = "";
            }
            this.Parameters["ReadMoneyTamUng"].Value = prefix4 + converter.Convert(temp4.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());

            decimal temp5 = 0;
            string prefix5 = "";
            if (totalNguonKhac < 0)
            {
                temp5 = 0 - totalNguonKhac;
                prefix5 = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp5 = totalNguonKhac;
                prefix5 = "";
            }
            this.Parameters["ReadMoneyNguonKhac"].Value = prefix5 + converter.Convert(temp5.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());

            /*▼====: #001*/
            if (gTotalPatientPaymentPaidAmount > 0)
            {
                totalBNPhaiDongThem += gTotalPatientPaymentPaidAmount.GetValueOrDefault(0);
                this.Parameters["TotalPatientExtraPayment"].Value = Math.Abs(totalBNPhaiDongThem);
            }
            /*▲====: #001*/

            decimal temp6 = Math.Abs(totalBNPhaiDongThem);
            this.Parameters["ReadMoneyBNPhaiDongThem"].Value = converter.Convert(temp6.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            /*▼====: #001*/
            if (gTotalPatientPaymentPaidAmount > 0)
            {
                this.Parameters["ReadTotalPatientPaymentPaid"].Value = converter.Convert(gTotalPatientPaymentPaidAmount.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
                this.Parameters["TotalPatientPaymentPaidAmount"].Value = gTotalPatientPaymentPaidAmount;
                xrLine1.Visible = true;
                xrLabel87.Visible = true;
            }
            else
            {
                xrLine1.Visible = false;
                xrLabel87.Visible = false;
            }
            /*▲====: #001*/
        }

        #region Tính tổng BH trả của riêng group DVKTC.
        private void CellHIPaymentForHighTechService_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
            e.Result = Math.Round(HiTechTotalHIPayment, MidpointRounding.AwayFromZero);
            totalBHTra = ReportTotalHIPayment;
            e.Handled = true;
        }
        private void xrTableRowSumHIHighTechSerice_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (HiTechTotalHIPayment <= 0)
            {
                e.Cancel = true;
            }
        }
        #endregion
        #region Lấy cột MaxHIPay
        decimal SixMothAddedValue = 0;
        decimal HIRebate = 0;
        decimal PatientPayment = 0;
        decimal ReportTotalHIPayment = 0;
        decimal HiTechTotalHIPayment = 0;
        private void MaxHIPay_GetResult(object sender, SummaryGetResultEventArgs e)
        {
            decimal MaxHIPay = 0;
            if (DetailReport.GetCurrentColumnValue("MaxHIPay") != null && Convert.ToDecimal(DetailReport.GetCurrentColumnValue("MaxHIPay")) > 0)
            {
                MaxHIPay = Convert.ToDecimal(DetailReport.GetCurrentColumnValue("MaxHIPay"));
                if (MaxHIPay > HIRebate + SixMothAddedValue)
                    MaxHIPay = HIRebate;
            }
            else
            {
                MaxHIPay = HIRebate;
                if (SixMothAddedValue > 0)
                    MaxHIPay += SixMothAddedValue;
            }
            ReportTotalHIPayment += MaxHIPay;
            if (DetailReport.GetCurrentColumnValue("IsHighTechServiceBill") != null && Convert.ToBoolean(DetailReport.GetCurrentColumnValue("IsHighTechServiceBill")))
            {
                HiTechTotalHIPayment += MaxHIPay;
            }
            e.Result = MaxHIPay;
            e.Handled = true;
        }
        private void MaxHIPay_Reset(object sender, EventArgs e)
        {
            SixMothAddedValue = 0;
            HIRebate = 0;
            PatientPayment = 0;
            OverHIPayment = 0;
        }
        private void MaxHIPay_RowChanged(object sender, EventArgs e)
        {
            if (SixMothAddedValue == 0 && DetailReport.GetCurrentColumnValue("SixMothAddedValue") != null && DetailReport.GetCurrentColumnValue("Group1") != null && DetailReport.GetCurrentColumnValue("Group1").ToString() == "b")
                SixMothAddedValue = Convert.ToDecimal(DetailReport.GetCurrentColumnValue("SixMothAddedValue"));
            if (DetailReport.GetCurrentColumnValue("HealthInsuranceRebate") != null)
                HIRebate += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("HealthInsuranceRebate"));
            if (DetailReport.GetCurrentColumnValue("Amount") != null)
                PatientPayment += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("Amount"));
            if (DetailReport.GetCurrentColumnValue("TotalAmount") != null)
                totalCP += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("TotalAmount"));
        }
        #endregion

        private void HIRebate_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (DetailReport.GetCurrentColumnValue("Group1") == null && DetailReport.GetCurrentColumnValue("Group1").ToString() != "b")
                e.Cancel = false;
        }
        private void MaxHI_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (DetailReport.GetCurrentColumnValue("Group1") != null && DetailReport.GetCurrentColumnValue("Group1").ToString() == "b")
            {
                if ((((XRTableCell)sender).Text == "0" || string.IsNullOrEmpty(((XRTableCell)sender).Text)) || Convert.ToDecimal(((XRTableCell)sender).Text) > HIRebate + SixMothAddedValue)
                    ((XRTableCell)sender).Text = string.Format("{0:#,#}", HIRebate);
            }
        }
        decimal SumaryPatientPayment = 0;
        private void PatientPayment_GetResult(object sender, SummaryGetResultEventArgs e)
        {
            PatientPayment += OverHIPayment;
            SumaryPatientPayment += PatientPayment;
            e.Result = PatientPayment;
            e.Handled = true;
        }
        private void SumaryPatientPayment_Reset(object sender, EventArgs e)
        {
            SumaryPatientPayment = 0;
            HiTechTotalHIPayment = 0;
        }
        private void SumaryPatientPayment_GetResult(object sender, SummaryGetResultEventArgs e)
        {
            totalBNTra += SumaryPatientPayment;
            e.Result = SumaryPatientPayment;
            e.Handled = true;
        }
        private void SumHIAmount_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (DetailReport.GetCurrentColumnValue("Group1") == null || string.IsNullOrEmpty(DetailReport.GetCurrentColumnValue("Group1").ToString()) || DetailReport.GetCurrentColumnValue("Group1").ToString() != "b")
                e.Cancel = true;
        }

        private void RowNote_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (DetailReport.GetCurrentColumnValue("IsHighTechServiceBill") != null || !Convert.ToBoolean(DetailReport.GetCurrentColumnValue("IsHighTechServiceBill")))
                e.Cancel = true;
        }
        decimal OverHIPayment = 0;
        private void MaxPatientPay_GetResult(object sender, SummaryGetResultEventArgs e)
        {
            decimal MaxHIPay = 0;
            if (DetailReport.GetCurrentColumnValue("MaxHIPay") != null && Convert.ToDecimal(DetailReport.GetCurrentColumnValue("MaxHIPay")) > 0
                && DetailReport.GetCurrentColumnValue("Group1") != null && DetailReport.GetCurrentColumnValue("Group1").ToString() == "b")
            {
                MaxHIPay = Convert.ToDecimal(DetailReport.GetCurrentColumnValue("MaxHIPay"));
                if (MaxHIPay < HIRebate + SixMothAddedValue)
                    OverHIPayment = HIRebate - MaxHIPay;
            }
            e.Result = OverHIPayment;
            e.Handled = true;
        }

        private void MaxHIPaid_GetResult(object sender, SummaryGetResultEventArgs e)
        {
            decimal MaxHIPay = 0;
            if (DetailReport.GetCurrentColumnValue("Group1") != null && DetailReport.GetCurrentColumnValue("Group1").ToString() == "b")
            {
                if (DetailReport.GetCurrentColumnValue("MaxHIPay") != null && Convert.ToDecimal(DetailReport.GetCurrentColumnValue("MaxHIPay")) > 0)
                    MaxHIPay = Convert.ToDecimal(DetailReport.GetCurrentColumnValue("MaxHIPay"));

                if (MaxHIPay < HIRebate + SixMothAddedValue && MaxHIPay > 0)
                    e.Result = null;
                else
                {
                    e.Result = -1 * SixMothAddedValue;
                    PatientPayment -= SixMothAddedValue;
                }
            }
            else
                e.Result = null;
            e.Handled = true;
        }

        private void HIRebate_GetResult(object sender, SummaryGetResultEventArgs e)
        {
            e.Result = HIRebate;
            e.Handled = true;
        }

        private void FiveYearRow_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (DetailReport.GetCurrentColumnValue("Group1") == null || string.IsNullOrEmpty(DetailReport.GetCurrentColumnValue("Group1").ToString()) || DetailReport.GetCurrentColumnValue("Group1").ToString() != "b")
                e.Cancel = true;
            else if (SixMothAddedValue == 0)
                e.Cancel = true;
        }
    }
}