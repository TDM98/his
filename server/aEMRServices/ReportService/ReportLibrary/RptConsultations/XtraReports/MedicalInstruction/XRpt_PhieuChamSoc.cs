using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRpt_PhieuChamSoc : DevExpress.XtraReports.UI.XtraReport
    {
        public virtual event PrintOnPageEventHandler PrintOnPage;
        public XRpt_PhieuChamSoc()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRptPhieuChamSoc1.EnforceConstraints = false;
            spRptPhieuChamSocTableAdapter.Fill(dsRptPhieuChamSoc1.spRptPhieuChamSoc, PtRegistrationID.Value as long?, (long)V_RegistrationType.Value, (bool)KhoaHienTai.Value);
        }

        private void xrLabel11_PrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (e.PageCount > 0)
            {
                if(e.PageIndex % 2 == 0)
                {
                    xrLabel11.Text = "Y tá (điều dưỡng) ghi       Phiếu số: " + ((e.PageIndex + 2)/2).ToString();
                }
                else
                {
                    xrLabel11.Text = "Y tá (điều dưỡng) ghi       Phiếu số: " + ((e.PageIndex + 1) / 2).ToString();
                }
                // Check if the control is printed on the first page.
                //if (e.PageIndex == 0)
                //{
                //    // Cancels the control's printing.
                //    e.Cancel = true;    
                //}
            }
        }

        private void xrPageInfo2_PrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (e.PageCount > 0)
            {
                // Check if the control is printed on the first page.
                if (e.PageIndex == 0)
                {
                    // Cancels the control's printing.
                    e.Cancel = true;    
                }
            }
        }
    }
}
