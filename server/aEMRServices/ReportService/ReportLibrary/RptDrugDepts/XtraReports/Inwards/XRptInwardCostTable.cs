using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardCostTable : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardCostTable()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spCostTableMedDept_ByCoIDTableAdapter.Fill(dsInwardCostTable1.spCostTableMedDept_ByCoID, Convert.ToInt64(this.InvID.Value));
            spCostTableMedDeptList_ByCoIDTableAdapter.Fill(dsInwardCostTable1.spCostTableMedDeptList_ByCoID, Convert.ToInt64(this.InvID.Value));
            
            decimal total = 0;
            string strCurrencyName = "";
            if (dsInwardCostTable1.spCostTableMedDept_ByCoID != null && dsInwardCostTable1.spCostTableMedDept_ByCoID.Rows.Count > 0)
            {
                total = Convert.ToDecimal(dsInwardCostTable1.spCostTableMedDept_ByCoID.Rows[0]["TotalValueHaveVAT"]);
                try
                {
                    strCurrencyName = dsInwardCostTable1.spCostTableMedDept_ByCoID.Rows[0]["CurrencyName"].ToString();
                }
                catch
                { }
            }
            if (strCurrencyName.ToUpper() == eHCMSResources.G1616_G1_VND.ToUpper() || strCurrencyName.ToUpper() == eHCMSResources.G1616_G1_VND.ToUpper() || strCurrencyName == "")
            {
                total = Math.Round(total, 0);
            }
            else
            {
                total = Math.Round(total, 2);
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
            this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), strCurrencyName);
        }

        private void XRptInwardCostTable_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
