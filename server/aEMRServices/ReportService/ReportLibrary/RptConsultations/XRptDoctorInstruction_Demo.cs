using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptDoctorInstruction_Demo: DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDoctorInstruction_Demo()
        {
            InitializeComponent();
        }

        private void XRptDoctorInstruction_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }

    }
}
