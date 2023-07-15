using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptSoDoChucNangHoHap : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoDoChucNangHoHap()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoDoChucNangHoHap1.EnforceConstraints = false;
            spRptSoDoChucNangHoHapTableAdapter.Fill(dsSoDoChucNangHoHap1.spRptSoDoChucNangHoHap
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );
        }

        private void XRptSoNoiSoi_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
