using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
using DevExpress.XtraCharts;
using DevExpress.Utils;
using System.Windows.Forms;

namespace eHCMS.ReportLib.InPatient.Reports
{
    public partial class XRptVitalSign_InPt_Paging : DevExpress.XtraReports.UI.XtraReport
    {
        int countPage = 0;
        public XRptVitalSign_InPt_Paging()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRptVitalSign_InPt_V21.EnforceConstraints = false;
            spXRptVitalSign_InPt_PagingTableAdapter.Fill(dsXRptVitalSign_InPt_V21.spXRptVitalSign_InPt_Paging
                , Convert.ToInt64(PtRegistrationID.Value)
                , Convert.ToInt64(V_RegistrationType.Value));
        }

        private void XRptVitalSign_InPt_Paging_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptVitalSign_InPt_V2)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = Convert.ToString(parDepartmentOfHealth.Value);
            ((XRptVitalSign_InPt_V2)((XRSubreport)sender).ReportSource).parHospitalName.Value = Convert.ToString(parHospitalName.Value);
            ((XRptVitalSign_InPt_V2)((XRSubreport)sender).ReportSource).V_RegistrationType.Value = Convert.ToInt64(V_RegistrationType.Value);
            ((XRptVitalSign_InPt_V2)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt64(GetCurrentColumnValue("PtRegistrationID"));
            ((XRptVitalSign_InPt_V2)((XRSubreport)sender).ReportSource).DeptLocID.Value = Convert.ToInt64(GetCurrentColumnValue("DeptLocID"));
        }

        private void xrPageInfo2_PrintOnPage(object sender, PrintOnPageEventArgs e)
        {
            if (e.PageCount > 0)
            {
                // Check if the control is printed on the first page.
                if (e.PageIndex == 0)
                {
                    // Cancels the control's printing.
                    e.Cancel = true;
                }
            }
        }
    }
}
