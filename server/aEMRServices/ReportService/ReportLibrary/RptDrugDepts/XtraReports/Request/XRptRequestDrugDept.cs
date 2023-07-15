using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Request
{
    public partial class XRptRequestDrugDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRequestDrugDept()
        {
            InitializeComponent();
        }

        private void XRptRequestDrugDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRequestDrugDept1.EnforceConstraints = false;
            spRequestDrugInwardClinicDept_GetByIDTableAdapter.Fill(dsRequestDrugDept1.spRequestDrugInwardClinicDept_GetByID, Convert.ToInt64(this.RequestID.Value));
            spReqOutwardDrugClinicDeptPatient_SumGetByIDTableAdapter.Fill(dsRequestDrugDept1.spReqOutwardDrugClinicDeptPatient_SumGetByID, Convert.ToInt64(this.RequestID.Value));
        }
    }
}
