using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRpt_AgeOfTheArtery : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_AgeOfTheArtery()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRpt_AgeOfTheArtery_BeforePrint);
            FillData();
        }

        public void FillData()
        {
            dsXRpt_AgeOfTheArtery1.EnforceConstraints = false;
            spXRpt_AgeOfTheArteryTableAdapter.Fill(dsXRpt_AgeOfTheArtery1.spXRpt_AgeOfTheArtery, Convert.ToInt64(AgeOfTheArteryID.Value));
        }

        private void XRpt_AgeOfTheArtery_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
