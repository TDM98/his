using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardThanhTrungSupplier : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardThanhTrungSupplier()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardThanhTrung_ByIDTableAdapter.Fill(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrung_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardThanhTrungInvoice_ByIDTableAdapter.Fill(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID, Convert.ToInt64(InvID.Value));
            spInwardThanhTrungMedDeptInvoice_GetListCostTableAdapter.Fill(dsInwardThanhTrungSupplier1.spInwardThanhTrungMedDeptInvoice_GetListCost, Convert.ToInt64(InvID.Value));
            decimal total = 0;
            string strCurrencyName = "";
            if (dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID != null && dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["TotalHaveDicounting"])
                        + Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["CustomTax"])
                        + Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["VATValueInvoice"])
                        + Convert.ToDecimal(dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["DifferenceValue"]);
                try
                {
                    strCurrencyName = dsInwardThanhTrungSupplier1.spRpt_InwardThanhTrungInvoice_ByID.Rows[0]["CurrencyName"].ToString();
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

        private void XRptInwardThanhTrungSupplier_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
