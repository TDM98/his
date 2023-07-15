using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp25a : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp25a()
        {
            InitializeComponent();
            FillDataInit();
        }

        public void FillDataInit()
        {
            spRpt_CreateTemplate25aTableAdapter.Fill(dsTemplate25a1.spRpt_CreateTemplate25a, null, null, 0, 1, 2010, 1);
        }
        public void FillData()
        {
            spRpt_CreateTemplate25aTableAdapter.Fill((this.DataSource as DataSchema.dsTemplate25a).spRpt_CreateTemplate25a, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value));
            decimal total = 0;
            if (dsTemplate25a1.spRpt_CreateTemplate25a != null && dsTemplate25a1.spRpt_CreateTemplate25a.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemplate25a1.spRpt_CreateTemplate25a.Rows.Count; i++)
                {
                    if (dsTemplate25a1.spRpt_CreateTemplate25a.Rows[i]["HealthInsuranceRebate"] != DBNull.Value)
                    {
                        // qty = Convert.ToDecimal(dsTemplate381.spRpt_PrintBillForPatientInsurance.Rows[i]["Qty"]);
                        total += Convert.ToDecimal(dsTemplate25a1.spRpt_CreateTemplate25a.Rows[i]["HealthInsuranceRebate"]);
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

        private void XRpt_Temp25a_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
