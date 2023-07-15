using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptSoThuThuat : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoThuThuat()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoThuThuat1.EnforceConstraints = false;
            spRptSoThuThuatTableAdapter.Fill(dsSoThuThuat1.spRptSoThuThuat    
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );
        }

        private void XRptSoThuThuat_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
