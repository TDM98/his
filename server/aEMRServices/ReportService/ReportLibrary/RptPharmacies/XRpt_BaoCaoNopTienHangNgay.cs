using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRpt_BaoCaoNopTienHangNgay : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoNopTienHangNgay()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            baoCao_NopTienHangNgayTableAdapter.Fill((this.DataSource as DataSchema.dsBaoCaoNopTien).BaoCao_NopTienHangNgay, this.PharmacyOutRepID.Value.ToString());
            decimal total = 0;
            if (dsBaoCaoNopTien1.BaoCao_NopTienHangNgay != null && dsBaoCaoNopTien1.BaoCao_NopTienHangNgay.Rows.Count > 0)
            {
                for (int i = 0; i < dsBaoCaoNopTien1.BaoCao_NopTienHangNgay.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsBaoCaoNopTien1.BaoCao_NopTienHangNgay.Rows[i]["ThucThu"]) + Convert.ToDecimal(dsBaoCaoNopTien1.BaoCao_NopTienHangNgay.Rows[i]["OutAmountCoPay"]);
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
                this.Parameters["StrMoney"].Value = string.Format("{0} :",  eHCMSResources.G2256_G1_VietBangChu) +prefix+ converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }
        }

        private void XRpt_BaoCaoNopTienHangNgay_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
