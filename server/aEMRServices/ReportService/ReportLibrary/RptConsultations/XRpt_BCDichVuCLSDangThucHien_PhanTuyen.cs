using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCDichVuCLSDangThucHien_PhanTuyen : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCDichVuCLSDangThucHien_PhanTuyen()
        {
            InitializeComponent();
        }

        private void XRpt_BaoCaoNhapPTTTKhoaPhong_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBCDichVuCLSDangThucHien_PhanTuyen1.EnforceConstraints = false;
            sp_BCDichVuCLSDangThucHien_PhanTuyenTableAdapter.Fill(dsBCDichVuCLSDangThucHien_PhanTuyen1.sp_BCDichVuCLSDangThucHien_PhanTuyen    
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                , Convert.ToInt32(parFindPatient.Value)
                , Convert.ToInt64(DeptLocationID.Value)
                , Convert.ToInt64(DeptID.Value)

                );

            if (parFindPatient.Value.ToString() == "1")
            {
                xrLabel15.Text = "Báo cáo DS dịch vụ kỹ thuật đã thực hiện nội trú";
            }
            else if (parFindPatient.Value.ToString() == "0")
            {
                xrLabel15.Text = "Báo cáo DS dịch vụ kỹ thuật đã thực hiện ngoại trú";
            }
            else
            {
                xrLabel15.Text = "Báo cáo DS dịch vụ kỹ thuật đã thực hiện nội-ngoại trú";
            }
        }
    }
}
