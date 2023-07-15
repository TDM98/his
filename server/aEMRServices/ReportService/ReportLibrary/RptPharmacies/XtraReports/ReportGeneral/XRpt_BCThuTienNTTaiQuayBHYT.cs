using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_BCThuTienNTTaiQuayBHYT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCThuTienNTTaiQuayBHYT()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsBC_ThuTien_NhaThuoc_TaiQuayBHYT1.EnforceConstraints = false;
            baoCao_ThuTien_NhaThuoc_TaiQuayBHYTTableAdapter.Fill(dsBC_ThuTien_NhaThuoc_TaiQuayBHYT1.BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT,
               Convert.ToInt64(2),
               Convert.ToDateTime(FromDate.Value),
               Convert.ToDateTime(ToDate.Value),
               Convert.ToInt32(Quarter.Value),
               Convert.ToInt32(Month.Value),
               Convert.ToInt32(Year.Value),
               Convert.ToByte(Flag.Value));

            decimal totalSumSell = 0;
            if (dsBC_ThuTien_NhaThuoc_TaiQuayBHYT1.BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT != null && dsBC_ThuTien_NhaThuoc_TaiQuayBHYT1.BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT.Rows.Count > 0)
            {
                for (int i = 0; i < dsBC_ThuTien_NhaThuoc_TaiQuayBHYT1.BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT.Rows.Count; i++)
                {
                    totalSumSell += Convert.ToDecimal(dsBC_ThuTien_NhaThuoc_TaiQuayBHYT1.BaoCao_ThuTien_NhaThuoc_TaiQuayBHYT.Rows[i]["OutAmount"]);
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

        private void XRpt_BCThuTienNTTaiQuayBHYT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
