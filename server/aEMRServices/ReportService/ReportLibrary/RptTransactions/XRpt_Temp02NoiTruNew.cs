using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp02NoiTruNew_V1 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp02NoiTruNew_V1()
        {
            InitializeComponent();
        }

        decimal totalTamUng = 0;
        decimal totalNguonKhac = 0;

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

        }

        private void XRpt_Temp02NoiTruNew_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        #region Tính tổng BH trả của riêng group DVKTC.

        decimal HIPaymentForHiTechService = 0;

        //KMx: Mỗi khi chuyển group (dịch vụ, thuốc, pcl) thì gọi hàm này.
        private void CellHIPaymentForHighTechService_SummaryReset(object sender, EventArgs e)
        {
            HIPaymentForHiTechService = 0;
        }

        //KMx: Khi load mỗi dòng trong group thì gọi hàm này.
        private void CellHIPaymentForHighTechService_SummaryRowChanged(object sender, EventArgs e)
        {
            //KMx: Chỉ có DVKTC mới cộng BH.
            bool isHighTechServiceBill = Convert.ToBoolean(DetailReport.GetCurrentColumnValue("IsHighTechServiceBill"));
            if (isHighTechServiceBill)
            {
                HIPaymentForHiTechService += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("HealthInsuranceRebate"));
            }
        }

        //KMx: Khi load hết các dòng trong 1 group thì gọi hàm này.
        private void CellHIPaymentForHighTechService_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
            e.Result = Math.Round(HIPaymentForHiTechService, MidpointRounding.AwayFromZero);
            e.Handled = true;
        }

        //KMx: Chỉ in dòng BH của DVKTC (05/12/2015 16:20).
        private void xrTableRowSumHIHighTechSerice_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (HIPaymentForHiTechService <= 0)
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region Tính tổng BH trả của từng group và của toàn report.
        decimal totalCP = 0;
        decimal totalBNTra = 0;
        decimal totalBHTra = 0;

        decimal amount = 0;
        decimal hiRebate = 0;

        private void CellHIPayment_SummaryReset(object sender, EventArgs e)
        {
            amount = 0;
            hiRebate = 0;
        }

        private void CellHIPayment_SummaryRowChanged(object sender, EventArgs e)
        {
            amount += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("TotalAmount"));
            hiRebate += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("HealthInsuranceRebate"));
        }

        private void CellHIPayment_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {
            bool isHighTechServiceBill = Convert.ToBoolean(DetailReport.GetCurrentColumnValue("IsHighTechServiceBill"));
            decimal MaxHIPay = Convert.ToDecimal(DetailReport.GetCurrentColumnValue("MaxHIPay"));

            if (isHighTechServiceBill && MaxHIPay > 0 && Math.Round(hiRebate, MidpointRounding.AwayFromZero) > MaxHIPay)
            {
                hiRebate = MaxHIPay;
            }
            else
            {
                hiRebate = Math.Round(hiRebate, MidpointRounding.AwayFromZero);
            }

            totalCP += Math.Round(amount, MidpointRounding.AwayFromZero);
            totalBHTra += hiRebate;

            e.Result = hiRebate;

            e.Handled = true;
        }
        #endregion

        #region Tính tổng BN trả của từng group.
        decimal sumTotalAmount = 0;
        decimal sumHIRebate = 0;

        private void CellPtPayment_SummaryReset(object sender, EventArgs e)
        {
            // Reset the result each time a group is printed. 
            sumTotalAmount = 0;
            sumHIRebate = 0;
        }

        private void CellPtPayment_SummaryRowChanged(object sender, EventArgs e)
        {
            // Calculate a summary. 
            sumTotalAmount += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("TotalAmount"));
            sumHIRebate += Convert.ToDecimal(DetailReport.GetCurrentColumnValue("HealthInsuranceRebate"));
        }

        private void CellPtPayment_SummaryGetResult(object sender, SummaryGetResultEventArgs e)
        {

            bool isHighTechServiceBill = Convert.ToBoolean(DetailReport.GetCurrentColumnValue("IsHighTechServiceBill"));
            decimal MaxHIPay = Convert.ToDecimal(DetailReport.GetCurrentColumnValue("MaxHIPay"));

            if (isHighTechServiceBill && MaxHIPay > 0 && Math.Round(sumHIRebate, MidpointRounding.AwayFromZero) > MaxHIPay)
            {
                sumHIRebate = MaxHIPay;
            }
            else
            {
                sumHIRebate = Math.Round(sumHIRebate, MidpointRounding.AwayFromZero);
            }

            e.Result = Math.Round(sumTotalAmount, MidpointRounding.AwayFromZero) - sumHIRebate;

            e.Handled = true;
        }
        #endregion


        private void xrTableCellSum_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            bool isHighTechServiceBill = Convert.ToBoolean(DetailReport.GetCurrentColumnValue("IsHighTechServiceBill"));

            string strSum = DetailReport.GetCurrentColumnValue("IdxNoiTru").ToString();

            if (!string.IsNullOrEmpty(DetailReport.GetCurrentColumnValue("ChildIdxNoiTru").ToString()))
            {
                strSum += "." + DetailReport.GetCurrentColumnValue("ChildIdxNoiTru").ToString();
            }

            if (isHighTechServiceBill && hiRebate > 0)
            {
                ((XRTableCell)sender).Text = string.Format(eHCMSResources.R0082_G1_BHYTTToanToiDa, strSum);
            }
            else
            {
                ((XRTableCell)sender).Text = string.Format(eHCMSResources.Z1351_G1_Cong0, strSum);
            }
        }

        decimal totalBNPhaiDongThem = 0;
        
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
                this.Parameters["PaymentTypeName"].Value = string.Format("{0} :",  eHCMSResources.Z1377_G1_SoTienBNDongBoSung);
            }
            else
            {
                this.Parameters["PaymentTypeName"].Value = string.Format("{0} :",  eHCMSResources.Z1378_G1_SoTienHoanTraBN);
            }


            System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp1 = 0;
            string prefix1 = "";
            if (totalCP < 0)
            {
                temp1 = 0 - totalCP;
                prefix1 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
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
                prefix2 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp2 = totalBNTra;
                prefix2 = "";
            }
            this.Parameters["ReadMoneyBNTra"].Value = prefix2 + converter.Convert(temp2.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            decimal temp3 = 0;
            string prefix3 = "";
            if (totalBNTra < 0)
            {
                temp3 = 0 - totalBHTra;
                prefix3 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
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
                prefix4 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
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
                prefix5 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp5 = totalNguonKhac;
                prefix5 = "";
            }
            this.Parameters["ReadMoneyNguonKhac"].Value = prefix5 + converter.Convert(temp5.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());


            decimal temp6 = Math.Abs(totalBNPhaiDongThem);
            this.Parameters["ReadMoneyBNPhaiDongThem"].Value = converter.Convert(temp6.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());

        }
    }
}