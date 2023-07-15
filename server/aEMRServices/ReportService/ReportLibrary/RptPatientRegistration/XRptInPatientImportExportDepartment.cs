using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptInPatientImportExportDepartment : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInPatientImportExportDepartment()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptInPatientImportExportDepartment_BeforePrint);
        }

        public void FillData()
        {
            spRpt_InPatientImportExportDepartment_DetailTableAdapter.Fill(dsInPatientImportExportDepartment1.spRpt_InPatientImportExportDepartment_Detail
                                                                , Convert.ToDateTime(FromDate.Value)
                                                                , Convert.ToDateTime(ToDate.Value)
                                                                , Convert.ToInt16(Quarter.Value)
                                                                , Convert.ToInt16(Month.Value)
                                                                , Convert.ToInt16(Year.Value)
                                                                , Convert.ToInt16(flag.Value)
                                                                , Convert.ToInt16(Status.Value)
                                                                , Convert.ToInt32(DeptID.Value)
                                                                , Convert.ToBoolean(IsExportExcel.Value));
        }

        private void XRptInPatientImportExportDepartment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
