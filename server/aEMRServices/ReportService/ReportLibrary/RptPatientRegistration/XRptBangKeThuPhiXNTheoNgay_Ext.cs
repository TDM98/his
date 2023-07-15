using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBangKeThuPhiXNTheoNgay_Ext : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBangKeThuPhiXNTheoNgay_Ext()
        {
            InitializeComponent();
        }

        private void XRptBangKeThuPhiXNTheoNgay_Ext_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRpt_BangKeThuPhiXetNghiem_TheoNgay_TxdTableAdapter.Fill(dsBangKeThuPhiXN_Ext1.spRpt_BangKeThuPhiXetNghiem_TheoNgay_Txd
                , Convert.ToInt64(this.paraStaffID.Value)
                , Convert.ToInt64(this.paraRepPaymtRecptByStaffID.Value));

            decimal ThucThu = 0;

            if (dsBangKeThuPhiXN_Ext1.spRpt_BangKeThuPhiXetNghiem_TheoNgay_Txd != null && dsBangKeThuPhiXN_Ext1.spRpt_BangKeThuPhiXetNghiem_TheoNgay_Txd.Rows.Count > 0)
            {
                for (int i = 0; i < dsBangKeThuPhiXN_Ext1.spRpt_BangKeThuPhiXetNghiem_TheoNgay_Txd.Rows.Count; i++)
                {
                    ThucThu += Convert.ToDecimal(dsBangKeThuPhiXN_Ext1.spRpt_BangKeThuPhiXetNghiem_TheoNgay_Txd.Rows[i]["ThucThu"]);
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
