using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBangKeThuPhiXNTheoNgay : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBangKeThuPhiXNTheoNgay()
        {
            InitializeComponent();
        }

        private void XRptBangKeThuPhiXNTheoNgay_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRpt_BangKeThuPhiXetNghiem_TheoNgayTableAdapter.Fill(dsBangKeThuPhiXNTheoNgay1.spRpt_BangKeThuPhiXetNghiem_TheoNgay
                , Convert.ToDateTime(this.FromDate.Value)
                , Convert.ToDateTime(this.ToDate.Value)
                , Convert.ToInt16(this.Quarter.Value)
                , Convert.ToInt16(this.Month.Value)
                , Convert.ToInt16(this.Year.Value)
                , Convert.ToInt16(this.flag.Value)
                , Convert.ToInt64(this.paraStaffID.Value));

            decimal ThucThu = 0;
           
            if (dsBangKeThuPhiXNTheoNgay1.spRpt_BangKeThuPhiXetNghiem_TheoNgay != null && dsBangKeThuPhiXNTheoNgay1.spRpt_BangKeThuPhiXetNghiem_TheoNgay.Rows.Count > 0)
            {
                for (int i = 0; i < dsBangKeThuPhiXNTheoNgay1.spRpt_BangKeThuPhiXetNghiem_TheoNgay.Rows.Count; i++)
                {
                    ThucThu += Convert.ToDecimal(dsBangKeThuPhiXNTheoNgay1.spRpt_BangKeThuPhiXetNghiem_TheoNgay.Rows[i]["ThucThu"]);
                }

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
                this.Parameters["ReadMoney"].Value = "(" + eHCMSResources.Z1383_G1_VietBangChuTienNop + ": " + prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper()) + ")";
            }
        }

    }
}
