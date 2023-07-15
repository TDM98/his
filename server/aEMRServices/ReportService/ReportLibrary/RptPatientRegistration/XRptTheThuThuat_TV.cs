using System;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public sealed partial class XRptTheThuThuat_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptTheThuThuat_TV()
        {
            InitializeComponent();
        }

        void XRptTheThuThuat_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            dsTheThuThuat1.EnforceConstraints = false;
            spGetSmallProcedureTableAdapter.ClearBeforeFill = true;
            spGetSmallProcedureTableAdapter.Fill(dsTheThuThuat1.spGetSmallProcedure, Convert.ToInt64(parPtRegistrationDetailID.Value), Convert.ToInt64(V_RegistrationType.Value), 0);
        }
    }
}