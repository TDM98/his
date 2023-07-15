using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptStoreDept.XtraReports.Request
{
    public partial class XRptFoodRequestDrugDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptFoodRequestDrugDept()
        {
            InitializeComponent();
        }

        private void XRptFoodRequestDrugDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsRequestFood1.EnforceConstraints = false;
            spReqFoodClinicDeptDetail_GetByIDTableAdapter.Fill(dsRequestFood1.spReqFoodClinicDeptDetail_GetByID, Convert.ToInt64(RequestID.Value));
            spRequestFoodClinicDept_GetByIDTableAdapter.Fill(dsRequestFood1.spRequestFoodClinicDept_GetByID, Convert.ToInt64(RequestID.Value));
        }
    }
}
