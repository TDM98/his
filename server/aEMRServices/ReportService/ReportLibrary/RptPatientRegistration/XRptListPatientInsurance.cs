using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptListPatientInsurance : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptListPatientInsurance()
        {
            InitializeComponent();
        }

        private void XRptListPatientInsurance_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            baoCao_DanhSachDangKyBaoHiemTableAdapter.Fill(dsListPatientInsurance1.BaoCao_DanhSachDangKyBaoHiem, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.flag.Value),Convert.ToInt16(this.FindPatient.Value),Convert.ToInt32(this.StaffID.Value));
        }

    }
}
