using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptSoKhamBenh : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoKhamBenh()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoKhamBenh1.EnforceConstraints = false;
            spRptSoKhamBenhTableAdapter.Fill(dsSoKhamBenh1.spRptSoKhamBenh    
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );
        }

        private void XRptSoKhamBenh_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
