using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptPatientPCLLaboratoryResults_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLLaboratoryResults_TV()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPatientPCLLaboratoryResults1.EnforceConstraints = false;
            spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_InfoTableAdapter.Fill(dsPatientPCLLaboratoryResults1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info, Convert.ToInt32(parPatientPCLReqID.Value), Convert.ToInt32(parPatientFindBy.Value));
            spPCLLaboratoryResults_With_ResultOldTableAdapter.Fill(dsPatientPCLLaboratoryResults1.spPCLLaboratoryResults_With_ResultOld,Convert.ToInt32(parPatientID.Value), Convert.ToInt32(parPatientPCLReqID.Value), Convert.ToInt32(parV_PCLRequestType.Value));
        }

        private void XRptPatientPCLLaboratoryResults_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
