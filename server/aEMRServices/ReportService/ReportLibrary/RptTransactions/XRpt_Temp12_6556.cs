using System;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
using System.Linq;
using System.Data;
using System.Drawing;
/*
* 20171024 #001 CMN: Added TotalPatientPaymentPaidAmount
*/
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp12_6556 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp12_6556()
        {
            InitializeComponent();
        }

        decimal totalTamUng = 0;
        decimal totalNguonKhac = 0;
        bool IsHICard_FiveYearsCont_NoPaid = false;
        bool IsPayDrug;
        bool IsOutPtTreatment; //20191226 TBL: Cờ xác nhận BN điều trị ngoại trú
        public void FillData()
        {
            if (RegistrationType.Value == null || RegistrationType.Value.Equals(0))
            {
                RegistrationType.Value = 24003;
            }
            if (Convert.ToInt32(RegistrationType.Value) == 24001)
            {
                tcRegistrationTypeCode.Text = "1";
                LBTitle.Text = "BẢNG KÊ CHI PHÍ KHÁM BỆNH NGOẠI TRÚ";
                //DeptName.Value = "KHOA KHÁM BỆNH";
            }
            dsTemp02NoiTruNew1.EnforceConstraints = false;
            string DefaultNumFilmsRecvStr = null;
            spRpt_CreateTemp02NoiTruNewTableAdapter.Fill(dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew
                , Convert.ToInt64(PtRegistrationID.Value), Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
                , Convert.ToInt32(DeptID.Value), Convert.ToBoolean(ViewByDate.Value), Convert.ToBoolean(PrintNoChargeItem.Value)
                , 1, Convert.ToInt64(RegistrationType.Value), Convert.ToInt64(TransactionID.Value)
                , ref DefaultNumFilmsRecvStr, false, true, false, false);
            spHealthInsuranceHistory_InPtTableAdapter.Fill(dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt
                , Convert.ToInt64(PtRegistrationID.Value), Convert.ToDateTime(DischargeDate.Value)
                , Convert.ToInt64(RegistrationType.Value), Convert.ToInt64(TransactionID.Value)
                , Convert.ToInt64(DeptID.Value), Convert.ToBoolean(IsKHTHView.Value), true);
            spRpt_GetPatientCashAdvanceTableAdapter.Fill(dsTemp02NoiTruNew1.spRpt_GetPatientCashAdvance, Convert.ToInt64(PtRegistrationID.Value), Convert.ToInt64(RegistrationType.Value));
            spRpt_GetCharitySupportFundTableAdapter.Fill(dsTemp02NoiTruNew1.spRpt_GetCharitySupportFund, Convert.ToInt64(PtRegistrationID.Value), 0, Convert.ToInt64(RegistrationType.Value));
            //20191121 TBL: Lấy tất cả bill lên
            if (Convert.ToInt32(RegistrationType.Value) == 24003)
            {
                sp_GetAllInPatientBillingInvoicesTableAdapter.Fill(dsTemp02NoiTruNew1.sp_GetAllInPatientBillingInvoices, Convert.ToInt64(PtRegistrationID.Value), Convert.ToInt32(DeptID.Value), null, null);
            }
            if (string.IsNullOrEmpty(DefaultNumFilmsRecvStr))
            {
                txtDefaultNumFilmsRecvStr.Visible = false;
            }
            else
            {
                txtDefaultNumFilmsRecvStr.Visible = true;
                txtDefaultNumFilmsRecvStr.Text = string.Format("(Tôi đã nhận {0})", DefaultNumFilmsRecvStr);
            }

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
                if (dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Columns["DeptName"] != null && dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Rows[0]["DeptName"] != DBNull.Value && DeptName.Value.ToString() == "")
                {
                    DeptName.Value = dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Rows[0]["DeptName"];
                }
                if (dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Columns["Apply4210_5149"] != null && dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Rows[0]["Apply4210_5149"] != DBNull.Value)
                {
                    bool Apply4210_5149 = Convert.ToBoolean(dsTemp02NoiTruNew1.spHealthInsuranceHistory_InPt.Rows[0]["Apply4210_5149"]);
                    if (Apply4210_5149)
                    {
                        xrLabel21.Text = "+ Quỹ BHYT thanh toán theo giá dịch vụ y tế:";
                        xrLabel21.WidthF = 234F;
                        xrLabel43.LocationF = new Point(234, 64);
                        if (Convert.ToInt32(RegistrationType.Value) == 24001)
                        {
                            xrLabel13.Text = "+ Quỹ BHYT thanh toán theo định suất: Thanh toán vào cuối kỳ (cuối quý).";
                        }
                        else
                        {
                            xrLabel13.Text = "+ Quỹ BHYT thanh toán theo DRG: Thanh toán vào cuối kỳ (cuối quý).";
                        }
                        xrLabel13.Visible = true;
                        xrLine6.Visible = true;
                        xrLine17.Visible = true;
                    }
                    else
                    {
                        xrLabel22.LocationF = new Point(0, 86);
                        xrLabel35.LocationF = new Point(136, 86);
                        xrLabel86.LocationF = new Point(725, 86);

                        xrLabel30.LocationF = new Point(0, 108);
                        xrLabel36.LocationF = new Point(34, 108);
                        xrLabel31.LocationF = new Point(194, 108);
                        xrLabel39.LocationF = new Point(725, 108);

                        xrLabel25.LocationF = new Point(0, 130);
                        xrLabel91.LocationF = new Point(34, 130);
                        xrLabel92.LocationF = new Point(194, 130);
                        xrLabel93.LocationF = new Point(725, 130);

                        xrLabel74.LocationF = new Point(0, 152);
                        xrLabel62.LocationF = new Point(75, 152);
                        xrLabel76.LocationF = new Point(725, 152);

                        xrLabel42.LocationF = new Point(0, 174);
                    }
                }
                else
                {
                    xrLabel22.LocationF = new Point(0, 86);
                    xrLabel35.LocationF = new Point(136, 86);
                    xrLabel86.LocationF = new Point(725, 86);

                    xrLabel30.LocationF = new Point(0, 108);
                    xrLabel36.LocationF = new Point(34, 108);
                    xrLabel31.LocationF = new Point(194, 108);
                    xrLabel43.LocationF = new Point(725, 108);

                    xrLabel25.LocationF = new Point(0, 130);
                    xrLabel91.LocationF = new Point(34, 130);
                    xrLabel93.LocationF = new Point(194, 130);
                    xrLabel43.LocationF = new Point(725, 130);

                    xrLabel74.LocationF = new Point(0, 152);
                    xrLabel62.LocationF = new Point(75, 152);
                    xrLabel76.LocationF = new Point(725, 152);

                    xrLabel42.LocationF = new Point(0, 174);
                }
            }

            if (dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew != null && dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Count > 0 && Convert.ToInt32(RegistrationType.Value) == 24001)
            {
                for (int i = 0; i < dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Count; i++)
                {
                    if (dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Columns["IsPayDrug"] != null && dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows[0]["IsPayDrug"] != DBNull.Value)
                    {
                        IsPayDrug = Convert.ToBoolean(dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows[i]["IsPayDrug"]);
                        break;
                    }
                }
            }

            if (dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew != null && dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Count > 0 && Convert.ToInt32(RegistrationType.Value) == 24001)
            {
                for (int i = 0; i < dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Count; i++)
                {
                    if (dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Columns["IsOutPtTreatment"] != null && dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows[0]["IsOutPtTreatment"] != DBNull.Value)
                    {
                        IsOutPtTreatment = Convert.ToBoolean(dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows[i]["IsOutPtTreatment"]);
                        break;
                    }
                }
            }

            //20191121 TBL: Tiền BN trả bây giờ sẽ lấy từ tổng tất cả các bill chứ không lấy từ chi tiết bill nữa
            if (dsTemp02NoiTruNew1.sp_GetAllInPatientBillingInvoices != null && dsTemp02NoiTruNew1.sp_GetAllInPatientBillingInvoices.Rows.Count > 0 && Convert.ToInt32(RegistrationType.Value) == 24003)
            {
                for (int i = 0; i < dsTemp02NoiTruNew1.sp_GetAllInPatientBillingInvoices.Rows.Count; i++)
                {
                    if (dsTemp02NoiTruNew1.sp_GetAllInPatientBillingInvoices.Columns["TotalPatientPayment"] != null && dsTemp02NoiTruNew1.sp_GetAllInPatientBillingInvoices.Rows[i]["TotalPatientPayment"] != DBNull.Value)
                    {
                        totalBNTra += Convert.ToDecimal(dsTemp02NoiTruNew1.sp_GetAllInPatientBillingInvoices.Rows[i]["TotalPatientPayment"]);
                    }
                }
            }

            if (!IsPayDrug && Convert.ToInt32(RegistrationType.Value) == 24001)
            {
                LBTitle.Text = "TỔNG KẾT CHI PHÍ";
            }
            else if (IsOutPtTreatment && Convert.ToInt32(RegistrationType.Value) == 24001)
            {
                tcRegistrationTypeCode.Text = "2";
                LBTitle.Text = "BẢNG KÊ CHI PHÍ ĐIỀU TRỊ NGOẠI TRÚ";
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
        decimal TotalPtPmtWithoutCoPay = 0;

        private string ReadMoneyToString(decimal aMoney, string aMoneyPrefix = "")
        {
            NumberToLetterConverter mConverter = new NumberToLetterConverter();
            System.Globalization.CultureInfo mCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            decimal aTempMoney = 0;
            if (aMoney < 0)
            {
                aTempMoney = 0 - aMoney;
                aMoneyPrefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                aTempMoney = aMoney;
                aMoneyPrefix = "";
            }
            return aMoneyPrefix + mConverter.Convert(aTempMoney.ToString(), mCultureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }
        private void ReportFooter1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            decimal t = totalCP - Math.Floor(totalCP);
            if (t == (decimal)0.5)
            {
                totalCP += (decimal)0.5;
            }
            else
            {
                totalCP = Math.Round(totalCP, 0, MidpointRounding.AwayFromZero);
            }
            decimal TotalPatientCoPayAmount = 0;
            if (dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Count > 0)
            {
                TotalPatientCoPayAmount = dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Cast<DataRow>().Where(x => x["TotalPatientCoPayAmount"] != null && x["TotalPatientCoPayAmount"] != DBNull.Value).Sum(x => Convert.ToDecimal(x["TotalPatientCoPayAmount"]));
            }
            if (dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Count > 0)
            {
                TotalPtPmtWithoutCoPay = dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Cast<DataRow>().Where(x => x["Amount"] != null && x["Amount"] != DBNull.Value).Sum(x => Convert.ToDecimal(x["Amount"]));
            }
            if (Convert.ToInt32(RegistrationType.Value) == 24001)
            {
                totalBNTra = totalCP - totalBHTra;
            }
            //TotalPtPmtWithoutCoPay = Math.Round(totalBNTra, 0, MidpointRounding.AwayFromZero) - TotalPatientCoPayAmount;

            //20191225 TBL: Theo ý anh Tuân là Người bệnh trả = Cùng trả + Các khoảng khác để tránh trường hợp theo cách tính cũ thì Cùng trả = Người bệnh trả - Các khoảng khác ra số âm do Người bệnh trả đã làm tròn
            //TotalPatientCoPayAmount = Math.Round(totalBNTra, 0, MidpointRounding.AwayFromZero) - TotalPtPmtWithoutCoPay;
            totalBNTra = TotalPatientCoPayAmount + TotalPtPmtWithoutCoPay;

            //totalBNPhaiDongThem = totalBNTra - (totalTamUng + totalNguonKhac);
            totalBNPhaiDongThem = TotalPtPmtWithoutCoPay;

            Parameters["TotalAmount"].Value = totalCP;
            Parameters["TotalHIPayment"].Value = totalBHTra;
            Parameters["TotalPatientPayment"].Value = Math.Ceiling(totalBNTra);
            Parameters["TotalPatientCashAdvance"].Value = totalTamUng;
            Parameters["TotalCharitySupport"].Value = totalNguonKhac;
            Parameters["TotalPatientExtraPayment"].Value = Math.Abs(totalBNPhaiDongThem);
            Parameters["TotalPatientCoPayPayment"].Value = TotalPatientCoPayAmount;
            Parameters["PtPmtWithoutCoPay"].Value = TotalPtPmtWithoutCoPay;
            Parameters["ReadMoneyCoPayment"].Value = ReadMoneyToString(TotalPatientCoPayAmount);

            if (totalBNPhaiDongThem > 0)
            {
                Parameters["PaymentTypeName"].Value = string.Format("{0}:", eHCMSResources.Z1377_G1_SoTienBNDongBoSung);
            }
            else
            {
                Parameters["PaymentTypeName"].Value = string.Format("{0}:", eHCMSResources.Z1378_G1_SoTienHoanTraBN);
            }

            Parameters["PaymentTypeName"].Value = "Các khoảng phải trả khác:";

            /*▼====: #001*/
            if (gTotalPatientPaymentPaidAmount > 0)
            {
                Parameters["TotalPatientPaymentPaidName"].Value = "Số tiền đã trả bệnh nhân:";
            }
            else
            {
                Parameters["TotalPatientPaymentPaidName"].Value = null;
            }
            /*▲====: #001*/

            Parameters["ReadMoneyTongCP"].Value = ReadMoneyToString(totalCP);
            Parameters["ReadMoneyBNTra"].Value = ReadMoneyToString(Math.Ceiling(totalBNTra));
            Parameters["ReadMoneyBHTra"].Value = ReadMoneyToString(totalBHTra);
            Parameters["ReadMoneyTamUng"].Value = ReadMoneyToString(totalTamUng);
            Parameters["ReadMoneyNguonKhac"].Value = ReadMoneyToString(totalNguonKhac);
            decimal ChiphiBH = 0;
            if (dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Count > 0)
            {
                ChiphiBH = dsTemp02NoiTruNew1.spRpt_CreateTemp02NoiTruNew.Rows.Cast<DataRow>().Where(x => x["TotalHIAllowedAmount"] != null && x["TotalHIAllowedAmount"] != DBNull.Value).Sum(x => Convert.ToDecimal(x["TotalHIAllowedAmount"]));
            }
            Parameters["ChiphiBH"].Value = ChiphiBH;
            Parameters["ReadMoneyChiphiBH"].Value = ReadMoneyToString(ChiphiBH);

            /*▼====: #001*/
            if (gTotalPatientPaymentPaidAmount > 0)
            {
                totalBNPhaiDongThem += gTotalPatientPaymentPaidAmount.GetValueOrDefault(0);
                Parameters["TotalPatientExtraPayment"].Value = Math.Abs(totalBNPhaiDongThem);
            }
            /*▲====: #001*/

            this.Parameters["ReadMoneyBNPhaiDongThem"].Value = ReadMoneyToString(Math.Abs(totalBNPhaiDongThem));
            /*▼====: #001*/
            if (gTotalPatientPaymentPaidAmount > 0)
            {
                Parameters["ReadTotalPatientPaymentPaid"].Value = ReadMoneyToString(gTotalPatientPaymentPaidAmount.GetValueOrDefault(0));
                Parameters["TotalPatientPaymentPaidAmount"].Value = gTotalPatientPaymentPaidAmount;
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
            if (DetailReport.GetCurrentColumnValue("HealthInsuranceRebate") != null && DetailReport.GetCurrentColumnValue("HealthInsuranceRebate") != DBNull.Value)
            {
                HIRebate += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("HealthInsuranceRebate"));
            }
            if (DetailReport.GetCurrentColumnValue("Amount") != null && DetailReport.GetCurrentColumnValue("Amount") != DBNull.Value)
            {
                PatientPayment += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("Amount"));
            }
            if (DetailReport.GetCurrentColumnValue("TotalAmount") != null && DetailReport.GetCurrentColumnValue("TotalAmount") != DBNull.Value)
            {
                totalCP += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("TotalAmount"));
            }
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
            if (Convert.ToInt32(RegistrationType.Value) == 24001)
            {
                totalBNTra += SumaryPatientPayment;
            }

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
        private void ReportFooter2_AfterPrint(object sender, EventArgs e)
        {
            PageHeaderLine.Visible = false;
        }
        private void DetailCashAdvance_AfterPrint(object sender, EventArgs e)
        {
            PageHeaderLine.Visible = false;
        }
        private void DetailCashAdvance_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            PageHeaderLine.Visible = false;
        }

        private void GroupHeader3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (DetailReport.GetCurrentColumnValue("calHienThi") == null)
            {
                xrTable3.Visible = false;

            }
            else
            {
                xrTable3.Visible = true;
            }

        }
    }
}
