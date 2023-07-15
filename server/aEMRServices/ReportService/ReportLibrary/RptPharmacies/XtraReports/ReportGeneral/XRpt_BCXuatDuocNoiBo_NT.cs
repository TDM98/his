using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_BCXuatDuocNoiBo_NT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCXuatDuocNoiBo_NT()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsBCXuatDuocNoiBo_NT1.EnforceConstraints = false;
            sp_BCXuatDuocNoiBo_NTTableAdapter.Fill(dsBCXuatDuocNoiBo_NT1.sp_BCXuatDuocNoiBo_NT,
               Convert.ToDateTime(FromDate.Value),
               Convert.ToDateTime(ToDate.Value),
               Convert.ToInt64(StoreID.Value)
            );

            decimal totalSumSell = 0;
            if (dsBCXuatDuocNoiBo_NT1.sp_BCXuatDuocNoiBo_NT != null && dsBCXuatDuocNoiBo_NT1.sp_BCXuatDuocNoiBo_NT.Rows.Count > 0)
            {
                for (int i = 0; i < dsBCXuatDuocNoiBo_NT1.sp_BCXuatDuocNoiBo_NT.Rows.Count; i++)
                {
                    totalSumSell += Convert.ToDecimal(dsBCXuatDuocNoiBo_NT1.sp_BCXuatDuocNoiBo_NT.Rows[i]["Amount"]);
                }
                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temptotalSumSell = 0;
                string prefixtotalSumSell = "";
                if (totalSumSell < 0)
                {
                    temptotalSumSell = 0 - totalSumSell;
                    prefixtotalSumSell = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temptotalSumSell = totalSumSell;
                    prefixtotalSumSell = "";
                }
                Parameters["ReadMoney"].Value = "( " + prefixtotalSumSell + converter.Convert(Math.Round(temptotalSumSell, 0).ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + " " + eHCMSResources.Z0872_G1_Dong.ToLower() + " )";
            }
        }

        private void XRpt_BCXuatDuocNoiBo_NT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
