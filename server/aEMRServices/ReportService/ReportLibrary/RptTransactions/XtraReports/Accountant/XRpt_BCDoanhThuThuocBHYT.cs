using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCDoanhThuThuocBHYT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCDoanhThuThuocBHYT()
        {
            InitializeComponent();
        }

        private void XRpt_BCDoanhThuThuocBHYT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCDoanhThuThuocBHYTNgTru1.spRpt_BCDoanhThuThuocBHYTNgTru, spRpt_BCDoanhThuThuocBHYTNgTruTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value)
            }, int.MaxValue);
        }
    }
}
