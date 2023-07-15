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
    public partial class XRptInwardChemicalSupplier : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardChemicalSupplier()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spRpt_InwardChemical_ByIDTableAdapter.Fill(dsInwardChemicalSupplier1.spRpt_InwardChemical_ByID, Convert.ToInt64(this.InvID.Value));
            spRpt_InwardChemicalInvoice_ByIDTableAdapter.Fill(dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID, Convert.ToInt64(this.InvID.Value));
            spInwardChemicalMedDeptInvoice_GetListCostTableAdapter.Fill(dsInwardChemicalSupplier1.spInwardChemicalMedDeptInvoice_GetListCost, Convert.ToInt64(this.InvID.Value));
            decimal total = 0;
            string strCurrencyName = "";
            if (dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID != null && dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID.Rows[0]["TotalHaveDicounting"])
                        + Convert.ToDecimal(dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID.Rows[0]["DifferenceValue"]);
                try
                {
                    strCurrencyName = dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID.Rows[0]["CurrencyName"].ToString();
                }
                catch { }
            }
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
          
        }

        private void XRptInwardChemicalSupplier_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
