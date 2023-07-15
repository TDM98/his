using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptSoNoiSoi : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoNoiSoi()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoNoiSoi1.EnforceConstraints = false;
            spRptSoNoiSoiTableAdapter.Fill(dsSoNoiSoi1.spRptSoNoiSoi
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
