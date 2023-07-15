using System;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptSoDienTim : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoDienTim()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsSoDienTim1.EnforceConstraints = false;
            spRptSoDienTimTableAdapter.Fill(dsSoDienTim1.spRptSoDienTim    
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );
        }


        private void XRptSoDienTim_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
