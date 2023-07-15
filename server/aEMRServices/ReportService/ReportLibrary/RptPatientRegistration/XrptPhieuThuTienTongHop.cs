using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XrptPhieuThuTienTongHop : DevExpress.XtraReports.UI.XtraReport
    {
        public XrptPhieuThuTienTongHop()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsPhieuThuTienTongHop1.EnforceConstraints = false;
            spRpt_PhieuThuTienTongHopTableAdapter.Fill(dsPhieuThuTienTongHop1.spRpt_PhieuThuTienTongHop
                , Convert.ToInt32(this.par_RepPaymentRecvID.Value));
            spRpt_GetReceiptNumberFromReportPaymentTableAdapter.Fill(dsPhieuThuTienTongHop1.spRpt_GetReceiptNumberFromReportPayment
                , Convert.ToInt32(this.par_RepPaymentRecvID.Value));

            decimal ThucThu = 0;
            if (dsPhieuThuTienTongHop1.spRpt_PhieuThuTienTongHop != null && dsPhieuThuTienTongHop1.spRpt_PhieuThuTienTongHop.Rows.Count > 0)
            {
                ThucThu = Convert.ToDecimal(dsPhieuThuTienTongHop1.spRpt_PhieuThuTienTongHop.Rows[0]["TotalPatientPayment"]);

                System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp1 = 0;
                string prefix1 = "";
                if (ThucThu < 0)
                {
                    temp1 = 0 - ThucThu;
                    prefix1 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp1 = ThucThu;
                    prefix1 = "";
                }
                this.Parameters["ReadMoney"].Value = "(" + prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper()) + ")";
            }
        }

        private void XrptPhieuThuTienTongHop_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
