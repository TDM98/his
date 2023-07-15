using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientPCLTargets : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLTargets()
        {
            InitializeComponent();
            
        }

        private void XRptPatientPCLTargets_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        
        private void FillData()
        {
            this.dsPCLTarget1.EnforceConstraints = false;
            this.spRpt_PCLExamTypeServiceTargetTableAdapter.Fill(this.dsPCLTarget1.spRpt_PCLExamTypeServiceTarget, Convert.ToInt32(this.par_PCLExamTypeID.Value), Convert.ToDateTime(this.par_FromDate.Value), Convert.ToDateTime(this.par_ToDate.Value),Convert.ToBoolean(this.par_IsAppointment.Value));
        }
    }
}
