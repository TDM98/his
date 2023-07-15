using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardVTYTTHSupplier : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardVTYTTHSupplier()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardDrugVTYTTH_ByIDTableAdapter.Fill(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTH_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardDrugVTYTTHInvoice_ByIDTableAdapter.Fill(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID, Convert.ToInt64(InvID.Value));
            spInwardVTYTTHInvoice_GetListCostTableAdapter.Fill(dsInwardVTYTTHSupplier1.spInwardVTYTTHInvoice_GetListCost, Convert.ToInt64(InvID.Value));
            decimal total = 0;
            string strCurrencyName = "";
            if (dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID != null && dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID.Rows[0]["TotalHaveDicounting"])
                        + Convert.ToDecimal(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID.Rows[0]["DifferenceValue"]);
                try
                {
                    strCurrencyName = dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID.Rows[0]["CurrencyName"].ToString();
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
            Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), strCurrencyName);          
        }

        private void XRptInwardVTYTTHSupplier_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
