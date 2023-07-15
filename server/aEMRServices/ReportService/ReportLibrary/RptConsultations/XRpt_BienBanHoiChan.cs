using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRpt_BienBanHoiChan : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BienBanHoiChan()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_BienBanHoiChan1.EnforceConstraints = false;
            spXRpt_BienBanHoiChanTableAdapter.Fill(dsXRpt_BienBanHoiChan1.spXRpt_BienBanHoiChan, Convert.ToInt64(DiagConsultationSummaryID.Value), Convert.ToInt64(V_RegistrationType.Value));
        }

        private void XRpt_BienBanHoiChan_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
