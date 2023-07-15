using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal
{
    public partial class XRptOutInternalDrugDeptToClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutInternalDrugDeptToClinicDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsOutwardInternal1.EnforceConstraints = false;
            spOutwardDrugMedDept_ByOutIDTableAdapter.Fill(dsOutwardInternal1.spOutwardDrugMedDept_ByOutID, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value), null);
            spOutwardDrugMedDeptInvoices_GetTableAdapter.Fill(dsOutwardInternal1.spOutwardDrugMedDeptInvoices_Get, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value));
            decimal total = 0;
            string strCurrencyName = "";

            if (dsOutwardInternal1.spOutwardDrugMedDept_ByOutID != null && dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows[i]["OutAmount"]);
                }
                try
                {
                    strCurrencyName = dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows[0]["CurrencyName"].ToString();
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

        private void XRptOutInternalDrugDeptToClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        void xrTable2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //if (GetCurrentColumnValue("IsDeleted") != null && GetCurrentColumnValue("IsDeleted") != DBNull.Value && GetCurrentColumnValue("IsDeleted").ToString() == "True")
            //{
            //    xrLine1.Visible = true;
            //}
            //else
            //{
            //    xrLine1.Visible = false;
            //}

            NumberToLetterConverter converter = new NumberToLetterConverter();
            bool ConvertComleted = Convert.ToBoolean(IsConvertCompleted.Value.ToString());
            if (GetCurrentColumnValue("QuantityShowAsString") != null && GetCurrentColumnValue("QuantityShowAsString") != DBNull.Value
                && GetCurrentColumnValue("QuantityShowAsString").ToString() == "1" && !ConvertComleted)
            {
                if (dsOutwardInternal1.spOutwardDrugMedDept_ByOutID != null && dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows.Count > 0)
                {
                    for (int i = 0; i < dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows.Count; i++)
                    {
                        string Qty = dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows[i]["Qty"].ToString();
                        string OutQuantity = dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows[i]["OutQuantity"].ToString();
                        dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows[i]["Qty"] = converter.Convert(Qty, '.', " ");
                        dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows[i]["OutQuantity"] = converter.Convert(OutQuantity, '.', " ");
                    }
                    IsConvertCompleted.Value = true;
                }
                xrTableCell12.TextFormatString = "";
                xrTableCell10.TextFormatString = "";
            }
        }
    }
}
