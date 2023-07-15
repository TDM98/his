using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRpt_BaoCaoNopTienChiTiet : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoNopTienChiTiet()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            baoCao_NopTienHangNgay_ChiTietTableAdapter.Fill((DataSource as DataSchema.dsBaoCaoNopTien).BaoCao_NopTienHangNgay_ChiTiet,PharmacyOutRepID.Value.ToString());
            decimal total = 0;
            if (dsBaoCaoNopTien1.BaoCao_NopTienHangNgay_ChiTiet != null && dsBaoCaoNopTien1.BaoCao_NopTienHangNgay_ChiTiet.Rows.Count > 0)
            {
                for (int i = 0; i < dsBaoCaoNopTien1.BaoCao_NopTienHangNgay_ChiTiet.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsBaoCaoNopTien1.BaoCao_NopTienHangNgay_ChiTiet.Rows[i]["ThucNopThuQuy"]);
                }
                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (total < 0)
                {
                    temp = 0 - Math.Round(total, 0);
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = Math.Round(total, 0);
                    prefix = "";
                }
                Parameters["StrMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }

        private void XRpt_BaoCaoNopTienChiTiet_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
