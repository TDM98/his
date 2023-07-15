using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal
{
    public partial class XRptOutThanhTrungDrugDeptToClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutThanhTrungDrugDeptToClinicDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsOutwardThanhTrung.EnforceConstraints = false;
            spOutwardThanhTrungMedDept_ByOutIDTableAdapter.Fill(dsOutwardThanhTrung.spOutwardThanhTrungMedDept_ByOutID, Convert.ToInt64(OutiID.Value), Convert.ToInt64(this.V_MedProductType.Value), null);
            spOutwardThanhTrungMedDeptInvoices_GetTableAdapter.Fill(dsOutwardThanhTrung.spOutwardThanhTrungMedDeptInvoices_Get, Convert.ToInt64(OutiID.Value));

            decimal total = 0;
            string strCurrencyName = "";
            if (dsOutwardThanhTrung.spOutwardThanhTrungMedDept_ByOutID != null && dsOutwardThanhTrung.spOutwardThanhTrungMedDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsOutwardThanhTrung.spOutwardThanhTrungMedDept_ByOutID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsOutwardThanhTrung.spOutwardThanhTrungMedDept_ByOutID.Rows[i]["OutAmount"]);
                }
                try
                {
                    strCurrencyName = dsOutwardThanhTrung.spOutwardThanhTrungMedDept_ByOutID.Rows[0]["CurrencyName"].ToString();
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

        private void XRptOutThanhTrungDrugDeptToClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
