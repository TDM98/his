using aEMR.DataAccessLayer.Providers;
using System;
/*
 * 20221022 #001 DatTB: Báo cáo BN trễ hẹn điều trị ngoại trú: Thêm cột "Ngày tái khám", Thêm bộ lọc "Nhóm bệnh" ĐTNT, Căn giữa trường hiển thị ngày xem báo cáo
 */
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRpt_BCBenhTreHenDieuTriNgoaiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCBenhTreHenDieuTriNgoaiTru()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCBenhTreHenDieuTriNgoaiTru1.spRpt_BCBenhTreHenDieuTriNgoaiTru, spRpt_BCBenhTreHenDieuTriNgoaiTruTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(FromDate.Value.ToString()),
                Convert.ToDateTime(ToDate.Value.ToString()),
                //▼==== #001
                Convert.ToInt64(OutpatientTreatmentTypeID.Value)
                //▲==== #001
        }, int.MaxValue);
        }

        private void XRptBCDoDHST_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
