using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BangKeBacSiThucHienPT_TT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BangKeBacSiThucHienPT_TT()
        {
            InitializeComponent();
        }

        private void XRpt_BangKeBacSiThucHienPT_TT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBangKeBacSiThucHienPTTT1.spRpt_BangKeBacSiThucHienPTTT, spRpt_BangKeBacSiThucHienPTTTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt16(parFindPatient.Value)
            }, int.MaxValue);

            if (parFindPatient.Value.ToString() == "1")
            {
                xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NỘI TRÚ";
            }
            else if (parFindPatient.Value.ToString() == "0")
            {
                xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NGOẠI TRÚ";
            }
            else
            {
                xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NỘI-NGOẠI TRÚ";
            }
        }
    }
}
