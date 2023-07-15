using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_TongHopDoanhThu : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TongHopDoanhThu()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            Parameters["TruongNhaThuoc"].Value = Parameters["TruongNhaThuoc"].Value.ToString().ToUpper();
            baoCao_TongHopDoanhThuTableAdapter.Fill((DataSource as DataSchema.ReportGeneral.dsTongHopDoanhThu).BaoCao_TongHopDoanhThu, Convert.ToInt32(Month.Value), Convert.ToInt32(Year.Value));
            decimal totalSumSell = 0;
            decimal totalBenefit = 0;
            decimal Cost = 0;
            if (dsTongHopDoanhThu1.BaoCao_TongHopDoanhThu != null && dsTongHopDoanhThu1.BaoCao_TongHopDoanhThu.Rows.Count > 0)
            {
                for (int i = 0; i < dsTongHopDoanhThu1.BaoCao_TongHopDoanhThu.Rows.Count; i++)
                {
                    totalSumSell += Convert.ToDecimal(dsTongHopDoanhThu1.BaoCao_TongHopDoanhThu.Rows[i]["SumSell"]);
                    totalBenefit += Convert.ToDecimal(dsTongHopDoanhThu1.BaoCao_TongHopDoanhThu.Rows[i]["Benefit"]);
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
                this.Parameters["ReadMoney"].Value = "( " + prefixtotalSumSell + converter.Convert(Math.Round(temptotalSumSell, 0).ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + " " + eHCMSResources.Z0872_G1_Dong.ToLower() + " )";

                NumberToLetterConverter converter2 = new NumberToLetterConverter();
                decimal temptotalBenefit = 0;
                string prefixtotalBenefit = "";
                if (totalBenefit < 0)
                {
                    temptotalBenefit = 0 - totalBenefit;
                    prefixtotalBenefit = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temptotalBenefit = totalBenefit;
                    prefixtotalBenefit = "";
                }
                Parameters["ReadMoney2"].Value = "( " + prefixtotalBenefit + converter2.Convert(Math.Round(temptotalBenefit, 0).ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + " " + eHCMSResources.Z0872_G1_Dong.ToLower() + " )";
                Cost = totalSumSell - totalBenefit;
                if (Cost > 0)
                {
                    Parameters["TyLeLai"].Value = totalBenefit / Cost;
                }
            }
        }

        private void XRpt_TongHopDoanhThu_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
