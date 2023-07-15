using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptStoreDept.XtraReports.Outward
{
    public partial class XRptPhieuTruyenMau : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuTruyenMau()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsXRptPhieuTruyenMau1.EnforceConstraints = false;
            sp_XRptPhieuTruyenMauTableAdapter.Fill(dsXRptPhieuTruyenMau1.sp_XRptPhieuTruyenMau, Convert.ToInt64(OutiID.Value));
        }

        private void XRptPhieuTruyenMau_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
