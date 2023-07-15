using System;
using aEMR.DataAccessLayer.Providers;
using AxLogging;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptDischargePapers_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDischargePapers_V2()
        {
            AxLogger.Instance.LogInfo("XRptDischargePapers_V2 HAM KHOI TAO");
            InitializeComponent();
            PrintingSystem.ShowPrintStatusDialog = false;
        }

        void XRptDischargePapers_V2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsDisChargePapers_V21.EnforceConstraints = false;
            spRptDisChargePapersTableAdapter.ClearBeforeFill = true;
            spRptDisChargePapersTableAdapter.Fill(dsDisChargePapers_V21.spRptDisChargePapers, Convert.ToInt32(PtRegistrationID.Value), Convert.ToInt32(V_RegistrationType.Value));
        }
    }
}
