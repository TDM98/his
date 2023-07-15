using System;
namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientPCLRequestDetailsByPatientPCLReqID : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLRequestDetailsByPatientPCLReqID()
        {
            InitializeComponent();
        }
        private void XRptPatientPCLRequestDetailsByPatientPCLReqID_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            this.dsPatientPCLRequestDetailsByPatientPCLReqID1.EnforceConstraints = false;
            this.spRptPatientPCLRequestDetailsByPatientPCLReqIDTableAdapter.Fill(this.dsPatientPCLRequestDetailsByPatientPCLReqID1.spRptPatientPCLRequestDetailsByPatientPCLReqID, Convert.ToInt32(this.parPatientPCLReqID.Value), Convert.ToInt32(this.paramV_RegistrationType.Value));
        }
    }
}