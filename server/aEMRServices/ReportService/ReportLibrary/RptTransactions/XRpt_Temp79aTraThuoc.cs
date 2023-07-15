using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp79aTraThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp79aTraThuoc()
        {
            InitializeComponent();
            FillDataInit();
        }

        public void FillDataInit()
        {
            spRpt_CreateTemplate79aTraThuocTableAdapter.Fill(dsTemp79aTraThuoc1.spRpt_CreateTemplate79aTraThuoc, null, null, 0, 1, 2010, 1);
        }
        public void FillData()
        {
            spRpt_CreateTemplate79aTraThuocTableAdapter.Fill((this.DataSource as DataSchema.dsTemp79aTraThuoc).spRpt_CreateTemplate79aTraThuoc, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value));
            decimal total = 0;
            if (dsTemp79aTraThuoc1.spRpt_CreateTemplate79aTraThuoc != null && dsTemp79aTraThuoc1.spRpt_CreateTemplate79aTraThuoc.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemp79aTraThuoc1.spRpt_CreateTemplate79aTraThuoc.Rows.Count; i++)
                {
                    if (dsTemp79aTraThuoc1.spRpt_CreateTemplate79aTraThuoc.Rows[i]["HealthInsuranceRebate"] != DBNull.Value)
                    {
                        // qty = Convert.ToDecimal(dsTemplate381.spRpt_PrintBillForPatientInsurance.Rows[i]["Qty"]);
                        total += Convert.ToDecimal(dsTemp79aTraThuoc1.spRpt_CreateTemplate79aTraThuoc.Rows[i]["HealthInsuranceRebate"]);
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

        private void XRpt_Temp79aTraThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
