using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp79aTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp79aTH()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            spRpt_CreateTemplate79aTHTableAdapter.Fill((this.DataSource as DataSchema.dsTemp79aTH).spRpt_CreateTemplate79aTH, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value));
            decimal total = 0;
            if (dsTemp79aTH1.spRpt_CreateTemplate79aTH != null && dsTemp79aTH1.spRpt_CreateTemplate79aTH.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemp79aTH1.spRpt_CreateTemplate79aTH.Rows.Count; i++)
                {
                    if (dsTemp79aTH1.spRpt_CreateTemplate79aTH.Rows[i]["HealthInsuranceRebate"] != DBNull.Value)
                    {
                        total += Convert.ToDecimal(dsTemp79aTH1.spRpt_CreateTemplate79aTH.Rows[i]["HealthInsuranceRebate"]);
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
                this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }

        private void XRpt_Temp79aTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
