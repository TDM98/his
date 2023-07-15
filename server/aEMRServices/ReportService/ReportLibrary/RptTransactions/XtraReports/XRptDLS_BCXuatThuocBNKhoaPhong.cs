using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRptDLS_BCXuatThuocBNKhoaPhong : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDLS_BCXuatThuocBNKhoaPhong()
        {
            InitializeComponent();
        }

        private void XRptDLS_BCXuatThuocBNKhoaPhong_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCXuatThuocBNKhoaPhong_v21.spRpt_BCXuatThuocBNKhoaPhong
                , spRpt_BCXuatThuocBNKhoaPhongTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToBoolean(MoreThreeDay.Value),
                Convert.ToInt32(DeptID.Value), Convert.ToInt32(DrugClassID.Value), Convert.ToInt32(PatientType.Value)
            }, int.MaxValue);
        }
    }
}
