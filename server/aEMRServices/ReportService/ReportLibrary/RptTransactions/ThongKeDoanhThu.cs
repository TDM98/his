using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class ThongKeDoanhThu : DevExpress.XtraReports.UI.XtraReport
    {
        public ThongKeDoanhThu()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            spRpt_ThongKeDoanhThuTableAdapter.Fill((this.DataSource as DataSchema.DsThongKeDoanhThu).spRpt_ThongKeDoanhThu,Convert.ToDateTime(this.FromDate.Value),Convert.ToDateTime(this.ToDate.Value),Convert.ToInt32(this.Quarter.Value),Convert.ToInt32(this.Month.Value),Convert.ToInt32(this.Year.Value),Convert.ToByte(this.Flag.Value));
        }
        private void ThongKeDoanhThu_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
