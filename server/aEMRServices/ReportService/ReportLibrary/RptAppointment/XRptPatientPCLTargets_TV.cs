using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.IO;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientPCLTargets_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLTargets_TV()
        {
            InitializeComponent();            
        }

        private void XRptPatientPCLTargets_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        
        private void FillData()
        {
            this.dsPCLTarget1.EnforceConstraints = false;
            this.spRpt_PCLExamTypeServiceTargetTableAdapter.Fill(dsPCLTarget1.spRpt_PCLExamTypeServiceTarget, Convert.ToInt32(par_PCLExamTypeID.Value), Convert.ToDateTime(par_FromDate.Value), Convert.ToDateTime(par_ToDate.Value),Convert.ToBoolean(par_IsAppointment.Value));
        }
    }
}
