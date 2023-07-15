using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_TinhHinhHoatDongKhamBenh_TongHop : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TinhHinhHoatDongKhamBenh_TongHop()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsTKTinhHinhKCB1.sp_KHTH_TKTinhHinhKCB, sp_KHTH_TKTinhHinhKCBTableAdapter.Adapter.GetFillParameters(), new object[] {
               Convert.ToDateTime(parFromDate.Value.ToString()), Convert.ToDateTime(parToDate.Value.ToString())
            }, int.MaxValue);
        }

        private void XRpt_TinhHinhHoatDongKhamBenh_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
