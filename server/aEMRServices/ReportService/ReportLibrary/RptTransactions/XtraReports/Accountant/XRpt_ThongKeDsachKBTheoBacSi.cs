using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ThongKeDsachKBTheoBacSi : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ThongKeDsachKBTheoBacSi()
        {
            InitializeComponent();
        }

        private void XRpt_ThongKeDsachKBTheoBacSi_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            //dsThongKeDsachKBTheoBacSi1.EnforceConstraints = false;
            //spRpt_ThongKeDsachBacSiKhamBenhTableAdapter.Fill(dsThongKeDsachKBTheoBacSi1.spRpt_ThongKeDsachBacSiKhamBenh, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), 1);

            ReportSqlProvider.Instance.ReaderIntoSchema(dsThongKeDsachKBTheoBacSi1.spRpt_ThongKeDsachBacSiKhamBenh, spRpt_ThongKeDsachBacSiKhamBenhTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), 1
            }, int.MaxValue);
        }
    }
}
