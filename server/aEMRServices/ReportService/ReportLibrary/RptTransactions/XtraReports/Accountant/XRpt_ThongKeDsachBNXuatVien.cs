using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ThongKeDsachBNXuatVien : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ThongKeDsachBNXuatVien()
        {
            InitializeComponent();
        }

        private void XRpt_ThongKeDsachBNXuatVien_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            //dsThongKeDsachBNXuatVien1.EnforceConstraints = false;
            //sp_ThongKeDsachBNXuatVienTableAdapter.Fill(dsThongKeDsachBNXuatVien1.sp_ThongKeDsachBNXuatVien, Convert.ToInt64(parDeptID.Value), Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));
            ReportSqlProvider.Instance.ReaderIntoSchema(dsThongKeDsachBNXuatVien1.sp_ThongKeDsachBNXuatVien, sp_ThongKeDsachBNXuatVienTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(parDeptID.Value), Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
            }, int.MaxValue);
        }
    }
}
