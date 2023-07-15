using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptRegistrationDetailList : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRegistrationDetailList()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            int? total=0;
            this.dsRegistrationDetailList1.EnforceConstraints = false;
            this.spSearchRegistrationsForDiagTableAdapter.Fill(
                this.dsRegistrationDetailList1.spSearchRegistrationsForDiag
                , ""
                , ""
                , ""
                , Convert.ToInt64(this.parStaffID.Value)
                , Convert.ToDateTime(this.parFromDate.Value)
                , Convert.ToDateTime(this.parToDate.Value)
                , null
                , Convert.ToInt32(this.parDeptID.Value)
                , Convert.ToInt32(this.parDeptLocationID.Value)
                , true
                , int.MaxValue
                , 0
                , ""
                , true
                , ref total);
        }

        private void XRptRegistrationDetailList_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
