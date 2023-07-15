using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptHIAppointment : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptHIAppointment()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spRptHIAppointmentTableAdapter.Fill(dsHIAppointment1.spRptHIAppointment, Convert.ToInt64(this.param_AppointmentID.Value));
        }

        private void XRptHIAppointment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
