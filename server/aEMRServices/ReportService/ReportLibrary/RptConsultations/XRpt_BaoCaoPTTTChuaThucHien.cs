using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BaoCaoPTTTChuaThucHien : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoPTTTChuaThucHien()
        {
            InitializeComponent();
        }

        private void XRpt_BaoCaoPTTTChuaThucHien_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBaoCaoPTTTChuaThucHien1.EnforceConstraints = false;
            spRpt_BaoCaoPTTTChuaThucHienTableAdapter.Fill(dsBaoCaoPTTTChuaThucHien1.spRpt_BaoCaoPTTTChuaThucHien, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt16(parFindPatient.Value));

            if (parFindPatient.Value.ToString() == "1")
            {
                xrLabel15.Text = "BÁO CÁO PHẨU THUẬT - THỦ THUẬT CHƯA THỰC HIỆN CỦA NỘI TRÚ";
            }
            else if (parFindPatient.Value.ToString() == "0")
            {
                xrLabel15.Text = "BÁO CÁO PHẨU THUẬT - THỦ THUẬT CHƯA THỰC HIỆN CỦA NGOẠI TRÚ";
            }
            else
            {
                xrLabel15.Text = "BÁO CÁO PHẨU THUẬT - THỦ THUẬT CHƯA THỰC HIỆN CỦA NỘI-NGOẠI TRÚ";
            }
        }
    }
}
