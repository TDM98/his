using System;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptSoXQuang : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoXQuang()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoXQuang1.EnforceConstraints = false;
            spRptSoXQuangTableAdapter.Fill(dsSoXQuang1.spRptSoXQuang    
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );
        }

        private void XRptSoXQuang_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
