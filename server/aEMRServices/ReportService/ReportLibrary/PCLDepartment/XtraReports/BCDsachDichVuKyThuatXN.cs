using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment.XtraReports
{
    public partial class BCDsachDichVuKyThuatXN : DevExpress.XtraReports.UI.XtraReport
    {
        public BCDsachDichVuKyThuatXN()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsBCDsachDichVuKyThuatXN1.EnforceConstraints = false;
            sp_Rpt_DsachDichVuKyThuatXNTableAdapter.Fill(dsBCDsachDichVuKyThuatXN1.sp_Rpt_DsachDichVuKyThuatXN, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value), Convert.ToInt32(parPCLStatus.Value), Convert.ToInt64(parSectionID.Value), Convert.ToInt64(parPCLExamTypeID.Value));
        }

        private void BCDsachDichVuKyThuatXN_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
