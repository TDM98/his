using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.InPatient.Reports
{
    public partial class XrptDischargePaper : DevExpress.XtraReports.UI.XtraReport
    {
        public XrptDischargePaper()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_GetInPatientAdmDisDetailsByIDTableAdapter.Fill(dsDischargePaper1.spRpt_GetInPatientAdmDisDetailsByID, Convert.ToInt64(this.InPatientAdmDisDetailID.Value));
        }

        private void XrptDischargePaper_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
