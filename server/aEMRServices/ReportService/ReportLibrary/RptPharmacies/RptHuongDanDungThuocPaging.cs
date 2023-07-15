using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class RptHuongDanDungThuocPaging : XtraReport
    {
        public RptHuongDanDungThuocPaging()
        {
            InitializeComponent();
        }

        private void RptHuongDanDungThuocPaging_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsPhieuSoanThuocPaging1.EnforceConstraints = false;
            spPhieuSoanThuocPagingTableAdapter.Fill((DataSource as DataSchema.dsPhieuSoanThuocPaging).spPhieuSoanThuocPaging, Convert.ToInt64(PtRegistrationID.Value));
        }

        private void RptHuongDanDungThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((RptHuongDanDungThuoc3x6)((XRSubreport)sender).ReportSource).PrescriptID.Value = GetCurrentColumnValue("PrescriptID");
            ((RptHuongDanDungThuoc3x6)((XRSubreport)sender).ReportSource).BeOfHIMedicineList.Value = BeOfHIMedicineList.Value;
        }
    }
}
