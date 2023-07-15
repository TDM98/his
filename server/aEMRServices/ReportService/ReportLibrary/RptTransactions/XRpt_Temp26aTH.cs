using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp26aTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp26aTH()
        {
            InitializeComponent();
            FillDataInit();
        }
        public void FillDataInit()
        {
            spRpt_CreateTemplate26aTHTableAdapter.Fill(dsTemp26TH1.spRpt_CreateTemplate26aTH, null,null,0, 1,2010,2, 0, false);
           
        }
        public void FillData()
        {
            spRpt_CreateTemplate26aTHTableAdapter.Fill(dsTemp26TH1.spRpt_CreateTemplate26aTH, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value), Convert.ToInt64(this.DeptID.Value), Convert.ToBoolean(this.NotTreatedAsInPt.Value));
            decimal total = 0;
            if (dsTemp26TH1.spRpt_CreateTemplate26aTH != null && dsTemp26TH1.spRpt_CreateTemplate26aTH.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemp26TH1.spRpt_CreateTemplate26aTH.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsTemp26TH1.spRpt_CreateTemplate26aTH.Rows[i]["HealthInsuranceRebate"]);
                }

                total = Math.Round(total, 0, MidpointRounding.AwayFromZero);

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

        private void XRpt_Temp26aTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
