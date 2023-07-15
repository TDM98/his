using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptTheoDoiThuocCoGioiHanSL : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptTheoDoiThuocCoGioiHanSL()
        {
            InitializeComponent();
        }
        public void FillData()
        {
             spRpt_TheoDoiThuocCoGioiHanSLTableAdapter.Fill((this.DataSource as DataSchema.dsTheoDoiThuocCoGioiHanSL).spRpt_TheoDoiThuocCoGioiHanSL,
                Convert.ToInt64(this.StoreID.Value), 
                Convert.ToDateTime(this.FromDate.Value),
                Convert.ToDateTime(this.ToDate.Value),
                Convert.ToInt32(this.Quarter.Value),
                Convert.ToInt32(this.Month.Value),
                Convert.ToInt32(this.Year.Value),
                Convert.ToByte(this.Flag.Value));
        }

        private void XRptTheoDoiThuocCoGioiHanSL_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
