using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp25aTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp25aTH()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            spRpt_CreateTemplate25aTHTableAdapter.Fill((this.DataSource as DataSchema.dsTemp25aTH).spRpt_CreateTemplate25aTH, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value));
            decimal total = 0;
            if (dsTemp25aTH1.spRpt_CreateTemplate25aTH != null && dsTemp25aTH1.spRpt_CreateTemplate25aTH.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemp25aTH1.spRpt_CreateTemplate25aTH.Rows.Count; i++)
                {
                    if (dsTemp25aTH1.spRpt_CreateTemplate25aTH.Rows[i]["HealthInsuranceRebate"] != DBNull.Value)
                    {
                        total += Convert.ToDecimal(dsTemp25aTH1.spRpt_CreateTemplate25aTH.Rows[i]["HealthInsuranceRebate"]);
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

        private void XRpt_Temp25aTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
