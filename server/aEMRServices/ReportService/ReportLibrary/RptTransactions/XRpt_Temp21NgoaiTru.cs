using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp21NgoaiiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp21NgoaiiTru()
        {
            InitializeComponent();
            FillData();
        }
        private void FillData()
        {
            dsTemp21NgoaiTru1.EnforceConstraints = false;
            spRpt_CreateTemp21NgoaiTruTableAdapter.Fill((this.DataSource as DataSchema.dsTemp21NgoaiTru).spRpt_CreateTemp21NgoaiTru, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.Flag.Value));
        }

        private void XRpt_Temp21NgoaiiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
