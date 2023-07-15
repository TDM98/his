using System;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptSoSieuAm : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoSieuAm()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoSieuAm1.EnforceConstraints = false;
            spRptSoSieuAmTableAdapter.Fill(dsSoSieuAm1.spRptSoSieuAm    
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );
        }

        private void XRptSoSieuAm_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
