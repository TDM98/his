using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Request
{
    public partial class XRptRequestDrugDeptDetailsGroupByPatient : XtraReport
    {
        public XRptRequestDrugDeptDetailsGroupByPatient()
        {
            InitializeComponent();
        }

        private void XRptRequestDrugDeptDetails_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsXRptRequestDrugDeptDetailsGroupByPatient1.EnforceConstraints = false;
            spXRptRequestDrugDeptDetailsGroupByPatientTableAdapter.Fill(dsXRptRequestDrugDeptDetailsGroupByPatient1.spXRptRequestDrugDeptDetailsGroupByPatient, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value), null);
            spXRptRequestDrugDeptHeaderByPatientTableAdapter.Fill(dsXRptRequestDrugDeptDetailsGroupByPatient1.spXRptRequestDrugDeptHeaderByPatient, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value));
        }
    }
}
