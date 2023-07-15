using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal
{
    public partial class XRptOutChemicalDrugDeptToClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutChemicalDrugDeptToClinicDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsOutwardChemical.EnforceConstraints = false;
            spOutwardChemicalMedDept_ByOutIDTableAdapter.Fill(dsOutwardChemical.spOutwardChemicalMedDept_ByOutID, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value), null);
            spOutwardChemicalMedDeptInvoices_GetTableAdapter.Fill(dsOutwardChemical.spOutwardChemicalMedDeptInvoices_Get, Convert.ToInt64(OutiID.Value));

            decimal total = 0;
            string strCurrencyName = "";
            if (dsOutwardChemical.spOutwardChemicalMedDept_ByOutID != null && dsOutwardChemical.spOutwardChemicalMedDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsOutwardChemical.spOutwardChemicalMedDept_ByOutID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsOutwardChemical.spOutwardChemicalMedDept_ByOutID.Rows[i]["OutAmount"]);
                }
                try
                {
                    strCurrencyName = dsOutwardChemical.spOutwardChemicalMedDept_ByOutID.Rows[0]["CurrencyName"].ToString();
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
                prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = total;
                prefix = "";
            }
            xrLabel43.Text = "Tổng số tiền (Bằng chữ): " + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), strCurrencyName);
            xrLabel44.Text = "Tổng số tiền (Bằng chữ): " + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), strCurrencyName);
            if (Convert.ToBoolean(parIsLiquidation.Value))
            {
                SubBand1.Visible = true;
                SubBand2.Visible = false;
            }
        }

        private void XRptOutChemicalDrugDeptToClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
