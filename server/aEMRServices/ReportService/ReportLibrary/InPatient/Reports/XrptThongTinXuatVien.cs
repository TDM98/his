using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.InPatient.Reports
{
    public partial class XrptThongTinXuatVien : DevExpress.XtraReports.UI.XtraReport
    {
        public XrptThongTinXuatVien()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_GetInPatientAdmDisDetailsByIDTableAdapter.Fill((this.DataSource as DataSchema.dsThongTinXuatVien).spRpt_GetInPatientAdmDisDetailsByID, Convert.ToInt64(this.InPatientAdmDisDetailID.Value));
        }
        private void XrptThongTinXuatVien_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
