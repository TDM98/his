using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientApptPCLRequests : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientApptPCLRequests()
        {
            InitializeComponent();
            
        }

        private void XRptPatientApptPCLRequests_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        
        private void FillData()
        {
            this.dsPatientApptPCLRequests1.EnforceConstraints = false;
            this.spRptPatientApptPCLRequestsTableAdapter.Fill(this.dsPatientApptPCLRequests1.spRptPatientApptPCLRequests, Convert.ToInt32(this.parAppointmentID.Value), Convert.ToInt32(this.parPatientPCLReqID.Value));
        }
    }
}
