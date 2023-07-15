using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using System.Threading;
using System.Globalization;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardMedDeptSupplier : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardMedDeptSupplier()
        {
            InitializeComponent();
            //xem lai dung chung nhu the nao?
            // Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN",false);
           // CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        public void FillData()
        {
            spRpt_InwardDrugMedDept_ByIDTableAdapter.Fill(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID, Convert.ToInt64(this.InvID.Value));
            spRpt_InwardDrugMedDeptInvoice_ByIDTableAdapter.Fill(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID, Convert.ToInt64(this.InvID.Value));
            spInwardDrugMedDeptInvoice_GetListCostTableAdapter.Fill(dsInwardMedDeptSupplier1.spInwardDrugMedDeptInvoice_GetListCost, Convert.ToInt64(this.InvID.Value));
            decimal total = 0;
            string strCurrencyName = "";
            if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID != null && dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["TotalHaveDicounting"])
                        + Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID.Rows[0]["DifferenceValue"]);
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
            if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID != null && dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count; i++)
                {
                    Qty = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["InQuantity"]);
                    InBuyingPrice = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["InBuyingPrice"]);
                    VAT = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["VAT"]);
                    TotalVAT = TotalVAT + (Qty * InBuyingPrice * VAT);
                }
            }
            total = total + TotalVAT;
            this.Parameters["parTotalVAT"].Value = TotalVAT.ToString();

            if (strCurrencyName.ToUpper() == eHCMSResources.G1616_G1_VND.ToUpper() || strCurrencyName.ToUpper() == eHCMSResources.G1616_G1_VND.ToUpper() || strCurrencyName == "")
            {
                total = Math.Round(total, 0);
            }
            else
            {
                total = Math.Round(total, 4);
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
            this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), strCurrencyName);

            decimal VatOnRow = 0;
            if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID != null && dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        VatOnRow = Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["VAT"]);
                    }
                    if (VatOnRow != Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["VAT"]))
                    {
                        this.Parameters["parVATorTong"].Value = "Tổng VAT";
                        break;
                    }
                }
            }
            this.Parameters["parVATShow"].Value = VatOnRow.ToString();
        }

        private void XRptInwardMedDeptSupplier_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
