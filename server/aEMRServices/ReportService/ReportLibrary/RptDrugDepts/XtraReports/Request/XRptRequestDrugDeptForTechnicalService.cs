using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Request
{
    public partial class XRptRequestDrugDeptForTechnicalService : XtraReport
    {
        public XRptRequestDrugDeptForTechnicalService()
        {
            InitializeComponent();
        }

        private void XRptRequestDrugDeptForTechnicalService_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRequestDrugInwardClinicDeptForTechnicalService_GetByIDTableAdapter.Fill(dsRequestDrugDeptForTechnicalService1.spRequestDrugInwardClinicDeptForTechnicalService_GetByID, Convert.ToInt64(this.RequestID.Value));
            spReqOutwardDrugClinicDeptPatientForTechnicalService_SumGetByIDTableAdapter.Fill(dsRequestDrugDeptForTechnicalService1.spReqOutwardDrugClinicDeptPatientForTechnicalService_SumGetByID, Convert.ToInt64(this.RequestID.Value));
        }
    }
}
