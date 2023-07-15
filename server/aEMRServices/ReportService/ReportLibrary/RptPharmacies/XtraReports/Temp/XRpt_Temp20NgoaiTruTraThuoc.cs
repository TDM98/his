using System;

namespace eHCMS.ReportLib.RptPharmacies.Temp
{
    public partial class XRpt_Temp20NgoaiTruTraThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp20NgoaiTruTraThuoc()
        {
            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            dsTemp20NgoaiTruTraThuoc1.EnforceConstraints = false;
            spRpt_CreateTemp20NgoaiTruTraThuocTableAdapter.Fill(dsTemp20NgoaiTruTraThuoc1.spRpt_CreateTemp20NgoaiTruTraThuoc,Convert.ToDateTime(this.FromDate.Value),Convert.ToDateTime(this.ToDate.Value),Convert.ToInt32(this.Quarter.Value),Convert.ToInt32(this.Month.Value),Convert.ToInt32(this.Year.Value),Convert.ToByte(this.Flag.Value));
        }

        private void XRpt_Temp20NgoaiTruTraThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
