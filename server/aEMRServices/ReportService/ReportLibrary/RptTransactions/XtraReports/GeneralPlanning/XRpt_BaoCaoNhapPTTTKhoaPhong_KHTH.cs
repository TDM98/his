using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BaoCaoNhapPTTTKhoaPhong_KHTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoNhapPTTTKhoaPhong_KHTH()
        {
            InitializeComponent();
        }

        private void XRpt_BaoCaoNhapPTTTKhoaPhong_KHTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBaoCaoNhapPTTTKhoaPhong_KHTH1.EnforceConstraints = false;
            spRpt_BaoCaoNhapPTTTKhoaPhong_KHTHTableAdapter.Fill(dsBaoCaoNhapPTTTKhoaPhong_KHTH1.spRpt_BaoCaoNhapPTTTKhoaPhong_KHTH
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                , Convert.ToInt16(parFindPatient.Value)
                , Convert.ToInt32(DeptLocationID.Value)
                , Convert.ToInt32(DeptID.Value)

                );

            if (parFindPatient.Value.ToString() == "1")
            {
                xrLabel15.Text = "DANH SÁCH BỆNH NHÂN THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NỘI TRÚ";
            }
            else if (parFindPatient.Value.ToString() == "0")
            {
                xrLabel15.Text = "DANH SÁCH BỆNH NHÂN THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NGOẠI TRÚ";
            }
            else
            {
                xrLabel15.Text = "DANH SÁCH BỆNH NHÂN THỰC HIỆN PHẨU THUẬT - THỦ THUẬT CỦA NỘI-NGOẠI TRÚ";
            }
        }
    }
}
