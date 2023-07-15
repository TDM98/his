using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XptRequestDrugPharmacy : DevExpress.XtraReports.UI.XtraReport
    {
        public XptRequestDrugPharmacy()
        {
            InitializeComponent();
        }
       
        public void FillData()
        {
           spRequestDrugInward_GetByIDTableAdapter.Fill((this.DataSource as DataSchema.dsRequestDrugPharmacy).spRequestDrugInward_GetByID, Convert.ToInt64(this.RequestID.Value));
           spRequestDrugInwardDetail_GetByIDTableAdapter.Fill((this.DataSource as DataSchema.dsRequestDrugPharmacy).spRequestDrugInwardDetail_GetByID, Convert.ToInt64(this.RequestID.Value));
        }

        private void XptRequestDrugPharmacy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
