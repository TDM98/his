using System;
using eHCMSLanguage;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal
{
    public partial class XRptOutVTTHDrugDeptToClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutVTTHDrugDeptToClinicDept()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            dsOutwardVTTH.EnforceConstraints = false;
            spOutwardVTTHMedDept_ByOutIDTableAdapter.Fill(dsOutwardVTTH.spOutwardVTTHMedDept_ByOutID, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value), null);
            spOutwardVTTHMedDeptInvoices_GetTableAdapter.Fill(dsOutwardVTTH.spOutwardVTTHMedDeptInvoices_Get, Convert.ToInt64(OutiID.Value));

            decimal total = 0;
            string strCurrencyName = "";
            if (dsOutwardVTTH.spOutwardVTTHMedDept_ByOutID != null && dsOutwardVTTH.spOutwardVTTHMedDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsOutwardVTTH.spOutwardVTTHMedDept_ByOutID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsOutwardVTTH.spOutwardVTTHMedDept_ByOutID.Rows[i]["OutAmount"]);
                }
                try
                {
                    strCurrencyName = dsOutwardVTTH.spOutwardVTTHMedDept_ByOutID.Rows[0]["CurrencyName"].ToString();
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

        private void XRptOutVTTHDrugDeptToClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
