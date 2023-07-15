using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientApptServiceDetails_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientApptServiceDetails_TV()
        {
            InitializeComponent();
        }

        private void XRptPatientApptServiceDetails_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            this.dsRptPatientApptServiceDetails1.EnforceConstraints = false;
            this.spRptPatientApptServiceDetails_HeaderTableAdapter1.Fill(this.dsRptPatientApptServiceDetails1.spRptPatientApptServiceDetails_Header, Convert.ToInt32(this.parAppointmentID.Value));
            this.spRptPatientApptServiceDetails_DetailsTableAdapter.Fill(this.dsRptPatientApptServiceDetails1.spRptPatientApptServiceDetails_Details, Convert.ToInt32(this.parAppointmentID.Value));
        }

    }
}
