using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XrptBangKeThuTienNgoaiTruTheoBienLai : DevExpress.XtraReports.UI.XtraReport
    {
        public XrptBangKeThuTienNgoaiTruTheoBienLai()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_BangKeThuTienNgoaiTruTheoBienLaiTableAdapter.Fill(dsBangKeThuTienNgoaiTruTheoBienLai1.spRpt_BangKeThuTienNgoaiTruTheoBienLai, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt64(parStaffID.Value));

            decimal totalPayAmount = 0;
            decimal totalRefundAmount = 0;
            decimal totalCancelAmount = 0;
            decimal total = 0;
            if (dsBangKeThuTienNgoaiTruTheoBienLai1.spRpt_BangKeThuTienNgoaiTruTheoBienLai != null && dsBangKeThuTienNgoaiTruTheoBienLai1.spRpt_BangKeThuTienNgoaiTruTheoBienLai.Rows.Count > 0)
            {
                for (int i = 0; i < dsBangKeThuTienNgoaiTruTheoBienLai1.spRpt_BangKeThuTienNgoaiTruTheoBienLai.Rows.Count; i++)
                {
                    totalPayAmount += Convert.ToDecimal(dsBangKeThuTienNgoaiTruTheoBienLai1.spRpt_BangKeThuTienNgoaiTruTheoBienLai.Rows[i]["PayAmount"]);
                    totalRefundAmount += Convert.ToDecimal(dsBangKeThuTienNgoaiTruTheoBienLai1.spRpt_BangKeThuTienNgoaiTruTheoBienLai.Rows[i]["RefundAmount"]);
                    totalCancelAmount += Convert.ToDecimal(dsBangKeThuTienNgoaiTruTheoBienLai1.spRpt_BangKeThuTienNgoaiTruTheoBienLai.Rows[i]["CancelAmount"]);
                }
            }

            total = totalPayAmount + totalRefundAmount + totalCancelAmount;

            total = Math.Round(total, 0);

            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (total < 0)
            {
                temp = 0 - total;
                prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = total;
                prefix = "";
            }
            Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

        private void XrptPhieuThuTienTongHop_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
