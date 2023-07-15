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
    public partial class XRptVitalSign_InPt : DevExpress.XtraReports.UI.XtraReport
    {
        int countPage = 0;
        public XRptVitalSign_InPt()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRptVitalSign_InPt1.EnforceConstraints = false;
            spXRptVitalSign_InPtTableAdapter.Fill(dsXRptVitalSign_InPt1.spXRptVitalSign_InPt
                , Convert.ToInt64(PtRegistrationID.Value)
                , Convert.ToInt64(V_RegistrationType.Value));
        }

        private void XRptVitalSign_InPt_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptVitalSign_ChartSub_InPt)((XRSubreport)sender).ReportSource).PtRegistrationID.Value = Convert.ToInt64(PtRegistrationID.Value);
            ((XRptVitalSign_ChartSub_InPt)((XRSubreport)sender).ReportSource).PageNumber.Value = Convert.ToInt32(GetCurrentColumnValue("PageNumber"));
            ((XRptVitalSign_ChartSub_InPt)((XRSubreport)sender).ReportSource).V_RegistrationType.Value = Convert.ToInt64(V_RegistrationType.Value);
        }
        
        private void xrChart1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //if (Detail.GetCurrentColumnValue("GroupChart") == null 
            //    || string.IsNullOrEmpty(DetailReport.GetCurrentColumnValue("Group1").ToString()) 
            //|| DetailReport.GetCurrentColumnValue("Group1").ToString() != "b")
            if (GetCurrentColumnValue("PageNumber") != null
                && GetCurrentColumnValue("PageNumber") != DBNull.Value)
            {
                spXRptVitalSign_SubChart_InPtTableAdapter.Fill(dsXRptVitalSign_InPt1.spXRptVitalSign_SubChart_InPt
                , Convert.ToInt64(PtRegistrationID.Value)
                , Convert.ToInt32(GetCurrentColumnValue("PageNumber")) - 1
                , Convert.ToInt64(V_RegistrationType.Value));

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
            //    spXRptVitalSign_SubChart_InPtTableAdapter.Fill(dsXRptVitalSign_InPt1.spXRptVitalSign_SubChart_InPt
            //    , Convert.ToInt64(PtRegistrationID.Value)
            //    , Convert.ToInt32(GetCurrentColumnValue("PageNumber")));
            //    string temp = dsXRptVitalSign_InPt1.spXRptVitalSign_InPt.Rows[0]["PageNumber"].ToString();
            //}
        }
    }
}
