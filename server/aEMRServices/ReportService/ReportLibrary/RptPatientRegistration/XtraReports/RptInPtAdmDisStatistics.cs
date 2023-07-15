using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class RptInPtAdmDisStatistics : DevExpress.XtraReports.UI.XtraReport
    {
        public RptInPtAdmDisStatistics()
        {
            InitializeComponent();
        }
        private void RptInPtAdmDisStatistics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spGetInPtAdmDisStatisticsTableAdapter.Fill(dsInPtAdmDisStatistics1.spGetInPtAdmDisStatistics, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.DeptID.Value));
        }
    }
}
