using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptBienBanHoiChan: DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBienBanHoiChan()
        {
            InitializeComponent();
        }

        private void XRptBienBanHoiChan_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            spRptInstruction_ParentTableAdapter.Fill(dsDoctorInstruction1.spRptInstruction_Parent, Convert.ToInt32(DiagConsultationSummaryID));
        }
    }
}
