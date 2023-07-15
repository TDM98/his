using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardTiemNguaSupplier : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardTiemNguaSupplier()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardVaccine_ByIDTableAdapter.Fill(dsInwardVaccineSupplier1.spRpt_InwardVaccine_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardVaccineInvoice_ByIDTableAdapter.Fill(dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID, Convert.ToInt64(InvID.Value));
            spInwardVaccineMedDeptInvoice_GetListCostTableAdapter.Fill(dsInwardVaccineSupplier1.spInwardVaccineMedDeptInvoice_GetListCost, Convert.ToInt64(InvID.Value));
            decimal total = 0;
            string strCurrencyName = "";
            if (dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID != null && dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID.Rows[0]["TotalHaveDicounting"])
                        + Convert.ToDecimal(dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID.Rows[0]["DifferenceValue"]);
                try
                {
                    strCurrencyName = dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID.Rows[0]["CurrencyName"].ToString();
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

        private void XRptInwardTiemNguaSupplier_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
