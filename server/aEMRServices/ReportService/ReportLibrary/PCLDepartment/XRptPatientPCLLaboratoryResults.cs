using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;

namespace eHCMS.ReportLib.PCLDepartment
{
    public partial class XRptPatientPCLLaboratoryResults : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLLaboratoryResults()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsPatientPCLLaboratoryResults1.EnforceConstraints = false;
            spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_InfoTableAdapter.Fill(this.dsPatientPCLLaboratoryResults1.spRptPatientPCLLaboratoryResults_ByPatientPCLReqID_Info, Convert.ToInt32(this.parPatientPCLReqID.Value), Convert.ToInt32(this.parPatientFindBy.Value));
            spPCLLaboratoryResults_With_ResultOldTableAdapter.Fill(dsPatientPCLLaboratoryResults1.spPCLLaboratoryResults_With_ResultOld,Convert.ToInt32(this.parPatientID.Value), Convert.ToInt32(this.parPatientPCLReqID.Value), Convert.ToInt32(this.parV_PCLRequestType.Value));
        }

        private void XRptPatientPCLLaboratoryResults_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void XRptPatientPCLLaboratoryResults_AfterPrint(object sender, EventArgs e)
        {
          //  PrintingSystem.ExecCommand(DevExpress.XtraPrinting.PrintingSystemCommand.ZoomToTwoPages);
           // PrintingSystem.PageSettings.Landscape = true;
        }

    }
}
