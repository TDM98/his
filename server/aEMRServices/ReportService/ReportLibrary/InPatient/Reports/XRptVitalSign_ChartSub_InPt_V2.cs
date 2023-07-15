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
    public partial class XRptVitalSign_ChartSub_InPt_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        int countPage = 0;
        public XRptVitalSign_ChartSub_InPt_V2()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsXRptVitalSign_InPt_V21.EnforceConstraints = false;
            spXRptVitalSign_SubChart_InPt_V2TableAdapter.Fill(dsXRptVitalSign_InPt_V21.spXRptVitalSign_SubChart_InPt_V2
               , Convert.ToInt64(PtRegistrationID.Value)
               , Convert.ToInt32(PageNumber.Value)
               , Convert.ToInt64(V_RegistrationType.Value)
               , Convert.ToInt64(DeptLocID.Value));
        }

        private void XRptVitalSign_ChartSub_InPt_V2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
