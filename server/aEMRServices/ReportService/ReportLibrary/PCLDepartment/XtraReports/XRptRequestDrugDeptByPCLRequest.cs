using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.PCLDepartment.XtraReports
{
    public partial class XRptRequestDrugDeptByPCLRequest : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRequestDrugDeptByPCLRequest()
        {
            InitializeComponent();
        }

        private void XRptRequestDrugDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRequestDrugDeptByPCLRequest1.EnforceConstraints = true;
            FillData();
           //spRequestDrugInwardClinicDept_GetByIDTableAdapter.Fill(dsRequestDrugDept1.spRequestDrugInwardClinicDept_GetByID, Convert.ToInt64(this.RequestID.Value));
           //spReqOutwardDrugClinicDeptPatient_SumGetByIDTableAdapter.Fill(dsRequestDrugDept1.spReqOutwardDrugClinicDeptPatient_SumGetByID, Convert.ToInt64(this.RequestID.Value));
        }
        private void FillData()
        {
            spRequestDrugInwardClinicDeptByPCLRequest_GetPatientInfoTableAdapter.Fill(dsRequestDrugDeptByPCLRequest1.spRequestDrugInwardClinicDeptByPCLRequest_GetPatientInfo
                ,Convert.ToInt64(ReqDrugInClinicDeptID.Value),Convert.ToInt64(V_RegistrationType.Value));

        }
    }
}
