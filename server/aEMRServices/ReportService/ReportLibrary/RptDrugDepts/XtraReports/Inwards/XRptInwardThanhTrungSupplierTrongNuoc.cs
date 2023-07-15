using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardThanhTrungSupplierTrongNuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardThanhTrungSupplierTrongNuoc()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardThanhTrung_ByIDTableAdapter.Fill(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardThanhTrungInvoice_ByIDTableAdapter.Fill(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID, Convert.ToInt64(InvID.Value));
            decimal total = 0;
            decimal VATDifference = 0;
            decimal DiscountingOnProduct = 0;
            decimal DiscountingInvoice = 0;
            decimal TotalNotVAT = 0;
            decimal CustomTax = 0;
            decimal DifferenceValue = 0;
            string strCurrencyName = "";
            if (dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID != null && dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["TotalHaveDicounting"])
                        + Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["DifferenceValue"])
                        - Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["Discounting"])
                        - Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["DiscountingOnProduct"]);

                VATDifference = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["TotalVATDifferenceAmount"]);
                TotalNotVAT = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["TotalNotVAT"]);
                DiscountingOnProduct = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["DiscountingOnProduct"]);
                DiscountingInvoice = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["DiscountingInvoice"]);
                CustomTax = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["CustomTax"]);
                DifferenceValue = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["DifferenceValue"]);

                VATDifference = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["TotalVATDifferenceAmount"]);
                try
                {
                    strCurrencyName = dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["CurrencyName"].ToString();
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
            if (dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID != null && dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows.Count > 0)
            {
                Row = dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows.Count;
                for (int i = 0; i < dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows.Count; i++)
                {
                    decimal DiscountingRow = 0;
                    decimal Discounting = 0;
                    CountRow++;
                    Qty = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["InQuantity"]);
                    InBuyingPrice = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["InBuyingPrice"]);
                    if (dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["VAT"] != DBNull.Value)
                    {
                        VAT = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["VAT"]);
                    }
                    else
                    {
                        VAT = 0;
                    }
                    TotalPriceNotVAT = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["TotalPriceNotVAT"]);
                    Discounting = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["Discounting"]);
                    if (CountRow == Row) //Nếu chỉ có 1 dòng hoặc là dòng cuối cùng
                    {
                        if (DiscountingInvoice > 0)
                        {
                            DiscountingRow = DiscountingInvoice - SumDiscountingRow;
                        }
                        SumDiscountingRow = SumDiscountingRow + DiscountingRow;
                        TotalVAT = TotalVAT + ((TotalPriceNotVAT - Discounting - DiscountingRow) * VAT);
                    }
                    else
                    {
                        if (DiscountingInvoice > 0)
                        {
                            DiscountingRow = (TotalPriceNotVAT - Discounting) * (DiscountingInvoice / (TotalNotVAT - DiscountingOnProduct));
                        }
                        SumDiscountingRow = SumDiscountingRow + DiscountingRow;
                        TotalVAT = TotalVAT + ((TotalPriceNotVAT - Discounting - DiscountingRow) * VAT);
                    }
                }
            }
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
                prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = total;
                prefix = "";
            }
            Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), strCurrencyName);

            decimal? Vat = null;
            decimal? VATForEqual = null;
            int ncount = 0;
            if (dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID != null && dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows.Count; i++)
                {
                    if (dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["VAT"] != DBNull.Value)
                    {
                        VATForEqual = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows[i]["VAT"]);
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
                    if (dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID.Rows.Count == ncount)
                    {
                        //this.Parameters["parVATorTong"].Value = "Không thuế VAT";
                        xrLabel16.Visible = false;
                        xrLabel60.Visible = false;
                        xrLabel59.Visible = false;
                        xrLabel58.Visible = false;
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

        private void XRptInwardThanhTrungSupplierTrongNuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
