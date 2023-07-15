using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal
{
    public partial class XRptOutBloodDrugDeptToClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutBloodDrugDeptToClinicDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsOutwardBlood.EnforceConstraints = false;
            spOutwardBloodMedDept_ByOutIDTableAdapter.Fill(dsOutwardBlood.spOutwardBloodMedDept_ByOutID, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value), null);
            spOutwardBloodMedDeptInvoices_GetTableAdapter.Fill(dsOutwardBlood.spOutwardBloodMedDeptInvoices_Get, Convert.ToInt64(OutiID.Value));

            decimal total = 0;
            string strCurrencyName = "";
            if (dsOutwardBlood.spOutwardBloodMedDept_ByOutID != null && dsOutwardBlood.spOutwardBloodMedDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsOutwardBlood.spOutwardBloodMedDept_ByOutID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsOutwardBlood.spOutwardBloodMedDept_ByOutID.Rows[i]["OutAmount"]);
                }
                try
                {
                    strCurrencyName = dsOutwardBlood.spOutwardBloodMedDept_ByOutID.Rows[0]["CurrencyName"].ToString();
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

        private void XRptOutBloodDrugDeptToClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
