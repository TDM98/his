using aEMR.DataAccessLayer.Providers;
using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_TongHopDoanhThuTheoKhoa : XtraReport
    {
        public XRpt_TongHopDoanhThuTheoKhoa()
        {
            InitializeComponent();
        }

        private void XRpt_TongHopDoanhThuTheoKhoa_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsTongHopDoanhThuTheoKhoa1.spRpt_KT_TongHopDoanhThuTheoKhoa, spRpt_KT_TongHopDoanhThuTheoKhoaTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt16(parDeptID.Value)
                , Convert.ToBoolean(parHasDischarge.Value)
            }, int.MaxValue);

            if (parIsDetail.Value.ToString() == "True")
            {
                Detail.Visible = true;
            }
        }
    }
}
