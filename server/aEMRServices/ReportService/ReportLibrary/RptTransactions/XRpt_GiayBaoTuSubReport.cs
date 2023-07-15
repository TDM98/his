using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_GiayBaoTuSubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_GiayBaoTuSubReport()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsXRpt_GiayBaoTu1.EnforceConstraints = false;
            spGetDeceasedInfo_ByPtRegIDTableAdapter.Fill(dsXRpt_GiayBaoTu1.spGetDeceasedInfo_ByPtRegID, Convert.ToInt64(PtRegistrationID.Value));
        }

        private void XRpt_GiayBaoTuSubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
