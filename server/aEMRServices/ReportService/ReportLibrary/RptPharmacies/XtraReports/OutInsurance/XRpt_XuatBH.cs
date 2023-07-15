using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.OutInsurance
{
    public partial class XRpt_XuatBH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_XuatBH()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            baoCao_XuatChoBHTableAdapter.Fill((this.DataSource as DataSchema.OutInsurance.dsXuatChoBH).BaoCao_XuatChoBH,
                Convert.ToInt64(this.StoreID.Value), 
                Convert.ToDateTime(this.FromDate.Value),
                Convert.ToDateTime(this.ToDate.Value),
                Convert.ToInt32(this.Quarter.Value),
                Convert.ToInt32(this.Month.Value),
                Convert.ToInt32(this.Year.Value),
                Convert.ToByte(this.Flag.Value) );
        }

        private void XRpt_XuatBH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
