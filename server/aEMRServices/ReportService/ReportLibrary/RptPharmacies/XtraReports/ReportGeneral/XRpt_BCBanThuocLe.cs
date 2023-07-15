using System;
using aEMR.DataAccessLayer.Providers;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_BCBanThuocLe : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCBanThuocLe()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCBanThuocLe1.sp_BCBanThuocLe, sp_BCBanThuocLeTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString()), Convert.ToInt64(StoreID.Value)
            }, int.MaxValue);

            decimal totalSumSell = 0;
            if (dsBCBanThuocLe1.sp_BCBanThuocLe != null && dsBCBanThuocLe1.sp_BCBanThuocLe.Rows.Count > 0)
            {
                for (int i = 0; i < dsBCBanThuocLe1.sp_BCBanThuocLe.Rows.Count; i++)
                {
                    totalSumSell += Convert.ToDecimal(dsBCBanThuocLe1.sp_BCBanThuocLe.Rows[i]["OutAmount"]);
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

        private void XRpt_BCBanThuocLe_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
