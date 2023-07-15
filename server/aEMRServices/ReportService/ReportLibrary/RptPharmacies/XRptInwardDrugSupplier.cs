using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptInwardDrugSupplier : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardDrugSupplier()
        {
            InitializeComponent();
            FillDataInit();
        }

        public void FillDataInit()
        {
            spInwardDrugInvoices_ByIDTableAdapter.Fill((DataSource as DataSchema.dsInwardDrugSupplier).spInwardDrugInvoices_ByID, 0);
            spRpt_InwardDrugDetails_ByIDTableAdapter.Fill((DataSource as DataSchema.dsInwardDrugSupplier).spRpt_InwardDrugDetails_ByID, 0);
        }

        public void FillData()
        {
            spInwardDrugInvoices_ByIDTableAdapter.Fill((DataSource as DataSchema.dsInwardDrugSupplier).spInwardDrugInvoices_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardDrugDetails_ByIDTableAdapter.Fill((DataSource as DataSchema.dsInwardDrugSupplier).spRpt_InwardDrugDetails_ByID, Convert.ToInt64(InvID.Value));
            decimal total = 0;
            decimal VATDifference = 0;
            decimal DiscountingOnProduct = 0;
            decimal DiscountingInvoice = 0;
            decimal TotalNotVAT = 0;
            decimal CustomTax = 0;
            decimal DifferenceValue = 0;
            string strCurrencyName = "";
            if (dsInwardDrugSupplier1.spInwardDrugInvoices_ByID != null && dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["TotalNotVAT"])
                       - Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["DiscountingOnProduct"])
                       - Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["DiscountingInvoice"])
                       + Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["CustomTax"])
                       + Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["VATValueInvoice"])
                       + Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["TotalVATDifferenceAmount"]);
                       //+ Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["DifferenceValue"]);
                VATDifference = Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["TotalVATDifferenceAmount"]);
                TotalNotVAT = Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["TotalNotVAT"]);
                DiscountingOnProduct = Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["DiscountingOnProduct"]);
                DiscountingInvoice = Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["DiscountingInvoice"]);
                CustomTax = Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["CustomTax"]);
                DifferenceValue = Convert.ToDecimal(dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["DifferenceValue"]);
                try
                {
                    strCurrencyName = dsInwardDrugSupplier1.spInwardDrugInvoices_ByID.Rows[0]["CurrencyName"].ToString();
                }
                catch { }
            }

            decimal TotalVAT = 0;
            decimal Qty = 0;
            decimal InBuyingPrice = 0;
            decimal VAT = 0;
            decimal TotalPriceNotVAT = 0;
            int Row = 1;
            int CountRow = 0;
            decimal SumDiscountingRow = 0;
            if (dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID != null && dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows.Count > 0)
            {
                Row = dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows.Count;
                for (int i = 0; i < dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows.Count; i++)
                {
                    decimal DiscountingRow = 0;
                    decimal Discounting = 0;
                    CountRow++;
                    Qty = Convert.ToDecimal(dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows[i]["InQuantity"]);
                    InBuyingPrice = Convert.ToDecimal(dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows[i]["InBuyingPrice"]);
                    Discounting = Convert.ToDecimal(dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows[i]["Discounting"]);
                    if (dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows[i]["VAT"] != DBNull.Value)
                    {
                        VAT = Convert.ToDecimal(dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows[i]["VAT"]);
                    }
                    else
                    {
                        VAT = 0;
                    }
                    TotalPriceNotVAT = Convert.ToDecimal(dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows[i]["TotalPriceNotVAT"]);
                    //20200324 TBL: BM 0027036: phải trừ đi CK của từng dòng để tính VAT chính xác
                    if (CountRow == Row) //Nếu chỉ có 1 dòng hoặc là dòng cuối cùng
                    {
                        if (DiscountingInvoice > 0)
                        {
                            DiscountingRow = DiscountingInvoice - SumDiscountingRow;
                        }
                        //▼====== 2020328 TTM: Xoá điều kiện này vì set DiscountingRow = Discounting thì khi trừ tính vAT sẽ tính 2 lần
                        //                     Vì cả 2 thằng trong trường hợp set này đều là Chiết khấu dòng.
                        //else
                        //{
                        //    DiscountingRow = Discounting;
                        //}
                        //▲===== 
                        SumDiscountingRow = SumDiscountingRow + DiscountingRow;
                        TotalVAT = TotalVAT + ((TotalPriceNotVAT - Discounting - DiscountingRow) * VAT);
                    }
                    else
                    {
                        if (DiscountingInvoice > 0)
                        {
                            DiscountingRow = (TotalPriceNotVAT - Discounting) * (DiscountingInvoice / (TotalNotVAT - DiscountingOnProduct));
                        }
                        //▼====== 2020328 TTM: Xoá điều kiện này vì set DiscountingRow = Discounting thì khi trừ tính vAT sẽ tính 2 lần
                        //                     Vì cả 2 thằng trong trường hợp set này đều là Chiết khấu dòng.
                        //else
                        //{
                        //    DiscountingRow = Discounting;
                        //}
                        //▲===== 
                        SumDiscountingRow = SumDiscountingRow + DiscountingRow;
                        TotalVAT = TotalVAT + ((TotalPriceNotVAT - Discounting - DiscountingRow) * VAT);
                    }
                }
            }
            //20200326 TBL: BM 0027036: Fix lỗi tổng tiền viết bằng chữ không đúng với Tiền thanh toán khi có điều chỉnh Tổng tiền VAT thực tế
            //total = total + TotalVAT;
            total = TotalNotVAT - DiscountingInvoice - DiscountingOnProduct + CustomTax + TotalVAT + DifferenceValue;
            this.Parameters["parTotalVatWithOutDifference"].Value = TotalVAT.ToString();
            this.Parameters["parTotalVAT"].Value = (TotalVAT + VATDifference).ToString();

            if (strCurrencyName.ToUpper() == eHCMSResources.G1616_G1_VND.ToUpper() || strCurrencyName.ToUpper() == eHCMSResources.G1616_G1_VND.ToUpper() || strCurrencyName == "")
            {
                total = Math.Round(total, 0);
            }
            else
            {
                total = Math.Round(total, 2);
            }
            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (total < 0)
            {
                temp = 0 - total;
                prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = total;
                prefix = "";
            }
            this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), strCurrencyName);

            decimal? Vat = null;
            decimal? VATForEqual = null;
            int ncount = 0;
            if (dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID != null && dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows.Count; i++)
                {
                    if (dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows[i]["VAT"] != DBNull.Value)
                    {
                        VATForEqual = Convert.ToDecimal(dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows[i]["VAT"]);
                    }
                    else
                    {
                        VATForEqual = null;
                    }
                    if (i == 0)
                    {
                        Vat = VATForEqual;
                    }
                    if (VATForEqual == null)
                    {
                        ncount++;
                    }
                    if (dsInwardDrugSupplier1.spRpt_InwardDrugDetails_ByID.Rows.Count == ncount)
                    {
                        //this.Parameters["parVATorTong"].Value = "Không thuế VAT";
                        xrLabel16.Visible = false;
                        xrLabel19.Visible = false;
                        xrLabel65.Visible = false;
                        xrLabel51.Visible = false;
                    }
                    if (Vat != VATForEqual)
                    {
                        this.Parameters["parVATorTong"].Value = "Tổng VAT";
                        break;
                    }
                }
            }
            this.Parameters["parVATShow"].Value = Vat;
        }

        private void XRptInwardDrugSupplier_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
