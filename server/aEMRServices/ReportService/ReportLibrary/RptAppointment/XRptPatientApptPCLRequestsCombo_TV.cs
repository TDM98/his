using System;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientApptPCLRequestsCombo_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientApptPCLRequestsCombo_TV()
        {
            InitializeComponent();
        }

        private void XRptPatientApptPCLRequestsCombo_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsPatientApptPCLRequestCombo1.EnforceConstraints = false;
            spRptPatientApptPCLRequestsCombineTableAdapter.Fill(dsPatientApptPCLRequestCombo1.spRptPatientApptPCLRequestsCombine
                , Convert.ToInt32(parAppointmentID.Value), parPtPCLReqID_List.Value.ToString());
            spRptPatientApptPCLRequestsHeaderTableAdapter1.Fill(dsPatientApptPCLRequestCombo1.spRptPatientApptPCLRequestsHeader
                , Convert.ToInt32(parAppointmentID.Value), parPtPCLReqID_List.Value.ToString());
        }
    }
}
