using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class PhieuSoanThuocPaging : XtraReport
    {
        public PhieuSoanThuocPaging()
        {
            InitializeComponent();
        }

        private void PhieuSoanThuocPaging_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsPhieuSoanThuocPaging1.EnforceConstraints = false;
            spPhieuSoanThuocPagingTableAdapter.Fill((DataSource as DataSchema.dsPhieuSoanThuocPaging).spPhieuSoanThuocPaging, Convert.ToInt64(PtRegistrationID.Value));
        }

        private void PhieuSoanThuocBH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((PhieuSoanThuocBH)((XRSubreport)sender).ReportSource).PrescriptID.Value = GetCurrentColumnValue("PrescriptID");
            ((PhieuSoanThuocBH)((XRSubreport)sender).ReportSource).StaffName.Value = StaffName.Value.ToString();
            ((PhieuSoanThuocBH)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
        }
    }
}
