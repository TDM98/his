using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Request
{
    public partial class XRptRequestHIStoreApproved : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRequestHIStoreApproved()
        {
            InitializeComponent();
        }

        private void XRptRequestHIStoreApproved_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           spRequestDrugInwardHIStore_GetByIDTableAdapter.Fill(dsRequestHIStore1.spRequestDrugInwardHIStore_GetByID, Convert.ToInt64(this.RequestID.Value));
           spReqOutwardDrugHIStoreDetails_SumGetByIDTableAdapter.Fill(dsRequestHIStore1.spReqOutwardDrugHIStoreDetails_SumGetByID, Convert.ToInt64(this.RequestID.Value));
        }
    }
}
