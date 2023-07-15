using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardBloodSupplier : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardBloodSupplier()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardBlood_ByIDTableAdapter.Fill(dsInwardBloodSupplier1.spRpt_InwardBlood_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardBloodInvoice_ByIDTableAdapter.Fill(dsInwardBloodSupplier1.spRpt_InwardBloodInvoice_ByID, Convert.ToInt64(InvID.Value));
            //spInwardBloodMedDeptInvoice_GetListCostTableAdapter.Fill(dsInwardBloodSupplier1.spInwardBloodMedDeptInvoice_GetListCost, Convert.ToInt64(InvID.Value));
            decimal total = 0;
            string strCurrencyName = "";
            if (dsInwardBloodSupplier1.spRpt_InwardBloodInvoice_ByID != null && dsInwardBloodSupplier1.spRpt_InwardBloodInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardBloodSupplier1.spRpt_InwardBloodInvoice_ByID.Rows[0]["TotalHaveDicounting"])
                        + Convert.ToDecimal(dsInwardBloodSupplier1.spRpt_InwardBloodInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardBloodSupplier1.spRpt_InwardBloodInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardBloodSupplier1.spRpt_InwardBloodInvoice_ByID.Rows[0]["DifferenceValue"]);
                try
                {
                    strCurrencyName = dsInwardBloodSupplier1.spRpt_InwardBloodInvoice_ByID.Rows[0]["CurrencyName"].ToString();
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

        private void XRptInwardBloodSupplier_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
