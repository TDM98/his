using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptRegistrationList : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRegistrationList()
        {
            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            int? total=0;
            this.dsRegistrationList1.EnforceConstraints = false;
            this.spSearchRegistrationsTableAdapter.Fill(
                this.dsRegistrationList1.spSearchRegistrations
                , ""
                , ""
                , ""
                , Convert.ToInt64(this.parStaffID.Value)
                , Convert.ToDateTime(this.parFromDate.Value)
                , Convert.ToDateTime(this.parToDate.Value)
                , null
                , Convert.ToInt32(this.parDeptID.Value)
                , Convert.ToInt32(this.parDeptLocationID.Value)                
                , Convert.ToInt16(this.parPatientFindBy.Value)
                , Convert.ToBoolean(this.parKhamBenh.Value)
                , Convert.ToBoolean(this.parIsHoanTat.Value)
                , true
                , Convert.ToBoolean(this.parIsCancel.Value)
                , Convert.ToInt16(this.parTypeSearch.Value)
                , 0, 0, "", false, ref total);
        }

        private void XRptRegistrationList_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
