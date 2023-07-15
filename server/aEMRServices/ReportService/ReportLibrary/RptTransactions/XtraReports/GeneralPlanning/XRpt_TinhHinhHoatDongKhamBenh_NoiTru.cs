using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_TinhHinhHoatDongKhamBenh_NoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TinhHinhHoatDongKhamBenh_NoiTru()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsTKTinhHinhKCB_NoiTru1.sp_KHTH_TKTinhHinhKCB_NoiTru, sp_KHTH_TKTinhHinhKCB_NoiTruTableAdapter.Adapter.GetFillParameters(), new object[] {
               Convert.ToDateTime(parFromDate.Value.ToString()), Convert.ToDateTime(parToDate.Value.ToString())
            }, int.MaxValue);
        }

        private void XRpt_TinhHinhHoatDongKhamBenh_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
