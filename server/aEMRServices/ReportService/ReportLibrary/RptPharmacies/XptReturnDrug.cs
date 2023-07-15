using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XptReturnDrug : DevExpress.XtraReports.UI.XtraReport
    {
        public XptReturnDrug()
        {
            InitializeComponent();
        }
       
        public void FillData()
        {
            spOutwardDrugInvoice_ReturnIDTableAdapter.Fill((this.DataSource as DataSchema.ReturnDrug).spOutwardDrugInvoice_ReturnID, Convert.ToInt64(this.OutiID.Value));
            spRpt_OutwardDrug_ReturnIDTableAdapter.Fill((this.DataSource as DataSchema.ReturnDrug).spRpt_OutwardDrug_ReturnID, Convert.ToInt64(this.OutiID.Value));
            decimal total = 0;
            if (returnDrug1.spRpt_OutwardDrug_ReturnID != null && returnDrug1.spRpt_OutwardDrug_ReturnID.Rows.Count > 0)
            {
                for (int i = 0; i < returnDrug1.spRpt_OutwardDrug_ReturnID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(returnDrug1.spRpt_OutwardDrug_ReturnID.Rows[i]["OutPrice"]) * Convert.ToDecimal(returnDrug1.spRpt_OutwardDrug_ReturnID.Rows[i]["OutQuantity"]);
                }
                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (total < 0)
                {
                    temp = 0 - Math.Round(total, 0);
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = Math.Round(total, 0);
                    prefix = "";
                }

                this.Parameters["ReadMoney"].Value = string.Format("{0} : ",  eHCMSResources.G2256_G1_VietBangChu) + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }

        }

        private void XptReturnDrug_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
