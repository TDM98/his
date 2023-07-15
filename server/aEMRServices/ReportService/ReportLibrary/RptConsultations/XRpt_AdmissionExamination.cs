using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRpt_AdmissionExamination : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_AdmissionExamination()
        {
            InitializeComponent();
        }

        private void XRpt_AdmissionExamination_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_AdmissionExamination1.EnforceConstraints = false;
            spXRpt_AdmissionExaminationTableAdapter.Fill(dsXRpt_AdmissionExamination1.spXRpt_AdmissionExamination, Convert.ToInt64(PtRegistrationID.Value), 24003);
        }
    }
}
