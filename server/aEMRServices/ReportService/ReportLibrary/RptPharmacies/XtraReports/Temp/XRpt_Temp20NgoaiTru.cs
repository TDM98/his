using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptPharmacies.Temp
{
    public partial class XRpt_Temp20NgoaiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp20NgoaiTru()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsTemp20NgoaiTru1.spRpt_CreateTemp20NgoaiTru, spRpt_CreateTemp20NgoaiTruTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.Flag.Value)
            }, int.MaxValue);
            //spRpt_CreateTemp20NgoaiTruTableAdapter.ClearBeforeFill = true;
            //spRpt_CreateTemp20NgoaiTruTableAdapter.Fill(dsTemp20NgoaiTru1.spRpt_CreateTemp20NgoaiTru,Convert.ToDateTime(this.FromDate.Value),Convert.ToDateTime(this.ToDate.Value),Convert.ToInt32(this.Quarter.Value),Convert.ToInt32(this.Month.Value),Convert.ToInt32(this.Year.Value),Convert.ToByte(this.Flag.Value));
        }
        private void XRpt_Temp20NgoaiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}