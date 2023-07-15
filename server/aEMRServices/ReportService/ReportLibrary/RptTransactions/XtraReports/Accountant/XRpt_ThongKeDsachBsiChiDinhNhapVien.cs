using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ThongKeDsachBsiChiDinhNhapVien : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ThongKeDsachBsiChiDinhNhapVien()
        {
            InitializeComponent();
        }

        private void XRpt_ThongKeDsachBsiChiDinhNhapVien_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            //dsThongKeDsachBsiChiDinhNhapVien1.EnforceConstraints = false;
            //sp_ThongKeDsachBsiChiDinhNhapVienTableAdapter.Fill(dsThongKeDsachBsiChiDinhNhapVien1.sp_ThongKeDsachBsiChiDinhNhapVien, Convert.ToInt64(parDeptID.Value), Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));
            ReportSqlProvider.Instance.ReaderIntoSchema(dsThongKeDsachBsiChiDinhNhapVien1.sp_ThongKeDsachBsiChiDinhNhapVien, sp_ThongKeDsachBsiChiDinhNhapVienTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(parDeptID.Value), Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
            }, int.MaxValue);
        }
    }
}
