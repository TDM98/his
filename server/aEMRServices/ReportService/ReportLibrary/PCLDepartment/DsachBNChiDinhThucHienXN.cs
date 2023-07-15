using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class DsachBNChiDinhThucHienXN : DevExpress.XtraReports.UI.XtraReport
    {
        public DsachBNChiDinhThucHienXN()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsDsachBNChiDinhThucHienXN1.EnforceConstraints = false;
            sp_Rpt_DsachBNChiDinhThucHienXNTableAdapter.Fill(dsDsachBNChiDinhThucHienXN1.sp_Rpt_DsachBNChiDinhThucHienXN, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value), Convert.ToInt64(parSectionID.Value));
        }

        private void DsachBNChiDinhThucHienXN_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
