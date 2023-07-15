using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientApptPCLRequestsCombo : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientApptPCLRequestsCombo()
        {
            InitializeComponent();
        }

        private void XRptPatientApptPCLRequestsCombo_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        
        private void FillData()
        {
            this.dsPatientApptPCLRequestCombo1.EnforceConstraints = false;
            this.spRptPatientApptPCLRequestsCombineTableAdapter.Fill(this.dsPatientApptPCLRequestCombo1.spRptPatientApptPCLRequestsCombine, Convert.ToInt32(this.parAppointmentID.Value), this.parPtPCLReqID_List.Value.ToString());
            this.spRptPatientApptPCLRequestsHeaderTableAdapter1.Fill(this.dsPatientApptPCLRequestCombo1.spRptPatientApptPCLRequestsHeader, Convert.ToInt32(this.parAppointmentID.Value), this.parPtPCLReqID_List.Value.ToString());
        }
    }
}
