using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardVTTHSupplier : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardVTTHSupplier()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardVTTH_ByIDTableAdapter.Fill(dsInwardVTTHSupplier1.spRpt_InwardVTTH_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardVTTHInvoice_ByIDTableAdapter.Fill(dsInwardVTTHSupplier1.spRpt_InwardVTTHInvoice_ByID, Convert.ToInt64(InvID.Value));
            spInwardVTTHMedDeptInvoice_GetListCostTableAdapter.Fill(dsInwardVTTHSupplier1.spInwardVTTHMedDeptInvoice_GetListCost, Convert.ToInt64(InvID.Value));
            decimal total = 0;
            string strCurrencyName = "";
            if (dsInwardVTTHSupplier1.spRpt_InwardVTTHInvoice_ByID != null && dsInwardVTTHSupplier1.spRpt_InwardVTTHInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardVTTHSupplier1.spRpt_InwardVTTHInvoice_ByID.Rows[0]["TotalHaveDicounting"])
                        + Convert.ToDecimal(dsInwardVTTHSupplier1.spRpt_InwardVTTHInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardVTTHSupplier1.spRpt_InwardVTTHInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardVTTHSupplier1.spRpt_InwardVTTHInvoice_ByID.Rows[0]["DifferenceValue"]);
                try
                {
                    strCurrencyName = dsInwardVTTHSupplier1.spRpt_InwardVTTHInvoice_ByID.Rows[0]["CurrencyName"].ToString();
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

        private void XRptInwardVTTHSupplier_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
