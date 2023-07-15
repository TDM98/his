using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_PhatHanhTheKCB : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PhatHanhTheKCB()
        {
            InitializeComponent();
        }

        private void XRpt_PhatHanhTheKCB_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_PhatHanhTheKCB1.EnforceConstraints = false;  
            spXRpt_PhatHanhTheKCBTableAdapter.Fill(dsXRpt_PhatHanhTheKCB1.spXRpt_PhatHanhTheKCB
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value));
        }
    }
}
