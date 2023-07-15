using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPatientInfo_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientInfo_TV()
        {
            InitializeComponent();
        }

        public void SetParametersForSubReport(object sender)
        {
            if ((bool)parIsChild.Value)
            {
                ((XRSubreport)sender).ReportSource = new XRptPatientInfo_SubReport_ForChild_TV();
                ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).PatientCode.Value = PatientCode.Value.ToString();
                ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).FileCodeNumber.Value = FileCodeNumber.Value.ToString();
                ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).PatientName.Value = PatientName.Value.ToString();
                ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).DOB.Value = DOB.Value.ToString();
                ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).Age.Value = Age.Value.ToString();
                ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).Gender.Value = Gender.Value.ToString();
                ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).PatientFullAddress.Value = PatientFullAddress.Value.ToString();
                ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).parWeight.Value = parWeight.Value.ToString();

                DateTime admissionDate = Convert.ToDateTime(AdmissionDate.Value);
                if (admissionDate.Year > 1970)
                {
                    ((XRptPatientInfo_SubReport_ForChild_TV)((XRSubreport)sender).ReportSource).AdmissionDate.Value = admissionDate;
                }
            }
            else
            {
                ((XRSubreport)sender).ReportSource = new XRptPatientInfo_SubReport_TV();
                ((XRptPatientInfo_SubReport_TV)((XRSubreport)sender).ReportSource).PatientCode.Value = PatientCode.Value.ToString();
                ((XRptPatientInfo_SubReport_TV)((XRSubreport)sender).ReportSource).FileCodeNumber.Value = FileCodeNumber.Value.ToString();
                ((XRptPatientInfo_SubReport_TV)((XRSubreport)sender).ReportSource).PatientName.Value = PatientName.Value.ToString();
                ((XRptPatientInfo_SubReport_TV)((XRSubreport)sender).ReportSource).DOB.Value = DOB.Value.ToString();
                ((XRptPatientInfo_SubReport_TV)((XRSubreport)sender).ReportSource).Age.Value = Age.Value.ToString();
                ((XRptPatientInfo_SubReport_TV)((XRSubreport)sender).ReportSource).Gender.Value = Gender.Value.ToString();
                ((XRptPatientInfo_SubReport_TV)((XRSubreport)sender).ReportSource).PatientFullAddress.Value = PatientFullAddress.Value.ToString();

                DateTime admissionDate = Convert.ToDateTime(AdmissionDate.Value);
                if (admissionDate.Year > 1970)
                {
                    ((XRptPatientInfo_SubReport_TV)((XRSubreport)sender).ReportSource).AdmissionDate.Value = admissionDate;
                }
            }            
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            SetParametersForSubReport(sender);
        }
    }
}
