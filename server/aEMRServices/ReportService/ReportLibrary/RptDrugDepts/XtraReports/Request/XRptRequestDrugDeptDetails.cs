using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Request
{
    public partial class XRptRequestDrugDeptDetails : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRequestDrugDeptDetails()
        {
            InitializeComponent();
        }

        private void XRptRequestDrugDeptDetails_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           spRequestDrugInwardClinicDept_GetByIDTableAdapter.Fill(dsRequestDrugDept1.spRequestDrugInwardClinicDept_GetByID, Convert.ToInt64(this.RequestID.Value));
           spReqOutwardDrugClinicDeptPatient_GetByIDTableAdapter.Fill(dsRequestDrugDept1.spReqOutwardDrugClinicDeptPatient_GetByID, Convert.ToInt64(this.RequestID.Value));
        }
    }
}
