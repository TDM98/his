using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment.XtraReports
{
    public partial class XRptBaoCaoDSBenhNhanCapNhatKetQua : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoDSBenhNhanCapNhatKetQua()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsXRptBaoCaoDSBenhNhanCapNhatKetQua1.EnforceConstraints = false;
            spXRptBaoCaoDSBenhNhanCapNhatKetQuaTableAdapter.Fill(dsXRptBaoCaoDSBenhNhanCapNhatKetQua1.spXRptBaoCaoDSBenhNhanCapNhatKetQua, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value));
        }

        private void DsachBNChiDinhThucHienXN_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
