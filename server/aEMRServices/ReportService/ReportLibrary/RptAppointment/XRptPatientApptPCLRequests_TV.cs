using System;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientApptPCLRequests_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientApptPCLRequests_TV()
        {
            InitializeComponent();
        }

        private void XRptPatientApptPCLRequests_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsPatientApptPCLRequests1.EnforceConstraints = false;
            spRptPatientApptPCLRequestsTableAdapter.Fill(dsPatientApptPCLRequests1.spRptPatientApptPCLRequests
                , Convert.ToInt32(parAppointmentID.Value), Convert.ToInt32(parPatientPCLReqID.Value));
        }
    }
}
