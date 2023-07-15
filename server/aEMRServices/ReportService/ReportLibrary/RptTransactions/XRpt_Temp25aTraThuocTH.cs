using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp25aTraThuocTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp25aTraThuocTH()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            spRpt_CreateTemplate25aTraThuocTHTableAdapter.Fill((this.DataSource as DataSchema.dsTemp25aTraThuocTH).spRpt_CreateTemplate25aTraThuocTH, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value));
            decimal total = 0;
            if (dsTemp25aTraThuocTH1.spRpt_CreateTemplate25aTraThuocTH != null && dsTemp25aTraThuocTH1.spRpt_CreateTemplate25aTraThuocTH.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemp25aTraThuocTH1.spRpt_CreateTemplate25aTraThuocTH.Rows.Count; i++)
                {
                    if (dsTemp25aTraThuocTH1.spRpt_CreateTemplate25aTraThuocTH.Rows[i]["HealthInsuranceRebate"] != DBNull.Value)
                    {
                        total += Convert.ToDecimal(dsTemp25aTraThuocTH1.spRpt_CreateTemplate25aTraThuocTH.Rows[i]["HealthInsuranceRebate"]);
                    }
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
                this.Parameters["ReadMoney"].Value = "Trừ " + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }

        private void XRpt_Temp25aTraThuocTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
