using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_GiayDeNghiMoTheKCB : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_GiayDeNghiMoTheKCB()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_GiayDeNghiMoTheKCB1.EnforceConstraints = false;
            spXRpt_GiayDeNghiMoTheKCBTableAdapter.Fill(dsXRpt_GiayDeNghiMoTheKCB1.spXRpt_GiayDeNghiMoTheKCB, Convert.ToInt64(CardID.Value));
        }

        private void XRpt_XacNhanDieuTri_NgoaiTru_NoiTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
