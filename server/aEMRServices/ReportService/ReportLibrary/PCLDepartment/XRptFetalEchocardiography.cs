using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptFetalEchocardiography : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptFetalEchocardiography()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsFetalEchocardiography1.EnforceConstraints = false;
            spRptFetalEchocardiographyTableAdapter.Fill(this.dsFetalEchocardiography1.spRptFetalEchocardiography, Convert.ToInt32(this.PatientPCLReqID.Value));
        }
        private void XRptFetalEchocardiography_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
