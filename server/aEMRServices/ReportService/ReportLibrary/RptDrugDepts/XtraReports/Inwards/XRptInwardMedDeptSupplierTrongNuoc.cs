using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardMedDeptSupplierTrongNuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardMedDeptSupplierTrongNuoc()
        {
            InitializeComponent();
            //xem lai dung chung nhu the nao?
            // Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN",false);
           // CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        public void FillData()
        {
            spRpt_InwardDrugMedDept_ByIDTableAdapter.Fill(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardDrugMedDeptInvoice_ByIDTableAdapter.Fill(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID, Convert.ToInt64(InvID.Value));
            decimal total = 0;
            decimal VATDifference = 0;
            decimal DiscountingOnProduct = 0;
            decimal DiscountingInvoice = 0;
            decimal TotalNotVAT = 0;
            decimal CustomTax = 0;
            decimal DifferenceValue = 0;
            string strCurrencyName = "";
            if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID != null && dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["TotalNotVAT"])
                        + Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["DifferenceValue"])
                        - Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["Discounting"])
                        - Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["DiscountingOnProduct"]);

                VATDifference = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["TotalVATDifferenceAmount"]);
                TotalNotVAT = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["TotalNotVAT"]);
                DiscountingOnProduct = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["DiscountingOnProduct"]);
                DiscountingInvoice = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["DiscountingInvoice"]);
                CustomTax = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["CustomTax"]);
                DifferenceValue = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["DifferenceValue"]);

                VATDifference = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["TotalVATDifferenceAmount"]);
                if (Convert.ToString(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["DotThau"]) == "")
                {
                    xrLabel78.Visible = false;
                    xrLabel79.Visible = false;
                }
                try
                {
                    strCurrencyName = dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["CurrencyName"].ToString();
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

            if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID != null && dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count > 0)
            {
                Row = dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count;
                for (int i = 0; i < dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count; i++)
                {
                    decimal DiscountingRow = 0;
                    decimal Discounting = 0;
                    CountRow++;
                    Qty = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["InQuantity"]);
                    InBuyingPrice = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["InBuyingPrice"]);
                    Discounting = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["Discounting"]);
                    if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["VAT"] != DBNull.Value)
                    {
                        VAT = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["VAT"]);
                    }
                    else
                    {
                        VAT = 0;
                    }
                    TotalPriceNotVAT = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["TotalPriceNotVAT"]);
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
            if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID != null && dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count; i++)
                {
                    if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["VAT"] != DBNull.Value)
                    {
                        VATForEqual = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["VAT"]);
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
                    if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count == ncount)
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

        private void XRptInwardMedDeptSupplierTrongNuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
