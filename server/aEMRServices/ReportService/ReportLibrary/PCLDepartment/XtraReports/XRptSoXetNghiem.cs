using System;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptSoXetNghiem : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoXetNghiem()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoXetNghiem1.EnforceConstraints = false;
            spRptSoXetNghiemTableAdapter.Fill(dsSoXetNghiem1.spRptSoXetNghiem    
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );
        }
        private void XRptSoXetNghiem_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
