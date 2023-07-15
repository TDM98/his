using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_BCThuTienNTTaiQuayBHYT_Detail : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCThuTienNTTaiQuayBHYT_Detail()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsBaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet1.EnforceConstraints = false;
            sp_BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTietTableAdapter.Fill(dsBaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet1.sp_BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet,
               Convert.ToInt64(2),
               Convert.ToDateTime(FromDate.Value),
               Convert.ToDateTime(ToDate.Value));

            decimal totalSumSell = 0;
            if (dsBaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet1.sp_BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet != null && dsBaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet1.sp_BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet.Rows.Count > 0)
            {
                for (int i = 0; i < dsBaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet1.sp_BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet.Rows.Count; i++)
                {
                    totalSumSell += Convert.ToDecimal(dsBaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet1.sp_BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT_ChiTiet.Rows[i]["Tien"]);
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

        private void XRpt_BCThuTienNTTaiQuayBHYT_Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
