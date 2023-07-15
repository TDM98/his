using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BaoCaoNhapPTTTKhoaPhong : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoNhapPTTTKhoaPhong()
        {
            InitializeComponent();
        }

        private void XRpt_BaoCaoNhapPTTTKhoaPhong_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBaoCaoNhapPTTTKhoaPhong1.EnforceConstraints = false;
            spRpt_BaoCaoNhapPTTTKhoaPhongTableAdapter.Fill(dsBaoCaoNhapPTTTKhoaPhong1.spRpt_BaoCaoNhapPTTTKhoaPhong    
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                , Convert.ToInt16(parFindPatient.Value)
                , Convert.ToInt32(DeptLocationID.Value)
                , Convert.ToInt32(DeptID.Value)
                
                );

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
