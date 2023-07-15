using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ThongKeDsachBNXuatVien_KHTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ThongKeDsachBNXuatVien_KHTH()
        {
            InitializeComponent();
        }

        private void XRpt_ThongKeDsachBNXuatVien_KHTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsThongKeDsachBNXuatVien_KHTH1.sp_ThongKeDsachBNXuatVien_KHTH, sp_ThongKeDsachBNXuatVien_KHTHTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(parDeptID.Value), Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
            }, int.MaxValue);
        }
    }
}
