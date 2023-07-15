using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Request
{
    public partial class XRptRequestHIStoreDetailApproved : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRequestHIStoreDetailApproved()
        {
            InitializeComponent();
        }

        private void XRptRequestHIStoreDetailApproved_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           spRequestDrugInwardHIStore_GetByIDTableAdapter.Fill(dsRequestHIStore1.spRequestDrugInwardHIStore_GetByID, Convert.ToInt64(this.RequestID.Value));
           spReqOutwardDrugHIStoreDetails_GetByIDTableAdapter.Fill(dsRequestHIStore1.spReqOutwardDrugHIStoreDetails_GetByID, Convert.ToInt64(this.RequestID.Value),0);
            
        }
    }
}
