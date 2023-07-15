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
    public partial class XRptVitalSign_InPt_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        int countPage = 0;
        public XRptVitalSign_InPt_V2()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRptVitalSign_InPt_V21.EnforceConstraints = false;
            spXRptVitalSign_InPt_V2TableAdapter.Fill(dsXRptVitalSign_InPt_V21.spXRptVitalSign_InPt_V2
                , Convert.ToInt64(PtRegistrationID.Value)
                , Convert.ToInt64(V_RegistrationType.Value)
                , Convert.ToInt64(DeptLocID.Value));
        }

        private void XRptVitalSign_InPt_V2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (GetCurrentColumnValue("PtRegistrationCode") == null)
            {
                e.Cancel = true;
            }
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptVitalSign_ChartSub_InPt_V2)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt64(PtRegistrationID.Value);
            ((XRptVitalSign_ChartSub_InPt_V2)((XRSubreport)sender).ReportSource).PageNumber.Value = Convert.ToInt32(GetCurrentColumnValue("PageNumber"));
            ((XRptVitalSign_ChartSub_InPt_V2)((XRSubreport)sender).ReportSource).V_RegistrationType.Value = Convert.ToInt64(V_RegistrationType.Value);
            ((XRptVitalSign_ChartSub_InPt_V2)((XRSubreport)sender).ReportSource).DeptLocID.Value = Convert.ToInt64(DeptLocID.Value);
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

        private void GroupHeader1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //if (Detail.GetCurrentColumnValue("GroupChart") == null 
            //    || string.IsNullOrEmpty(DetailReport.GetCurrentColumnValue("Group1").ToString()) 
            //|| DetailReport.GetCurrentColumnValue("Group1").ToString() != "b")
            //if (GetCurrentColumnValue("PageNumber") != null
            //    && GetCurrentColumnValue("PageNumber") != DBNull.Value)
            //{
            //    spXRptVitalSign_SubChart_InPtTableAdapter.Fill(dsXRptVitalSign_InPt_V21.spXRptVitalSign_SubChart_InPt
            //    , Convert.ToInt64(PtRegistrationID.Value)
            //    , Convert.ToInt32(GetCurrentColumnValue("PageNumber")));
            //    string temp = dsXRptVitalSign_InPt_V21.spXRptVitalSign_InPt_V2.Rows[0]["PageNumber"].ToString();
            //}
        }
    }
}
