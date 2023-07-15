using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp21NoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp21NoiTru()
        {
            InitializeComponent();
            //FillData();
        }

        private void FillData()
        {
            dsTemp21NoiTru1.EnforceConstraints = false;
            spRpt_CreateTemp21NoiTruTableAdapter.Fill((this.DataSource as DataSchema.dsTemp21NoiTru).spRpt_CreateTemp21NoiTru, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.Flag.Value));
        }

        private void XRpt_Temp21NoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
