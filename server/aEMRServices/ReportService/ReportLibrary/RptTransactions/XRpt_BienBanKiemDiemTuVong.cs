using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BienBanKiemDiemTuVong : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BienBanKiemDiemTuVong()
        {
            InitializeComponent();
        }

        private void XRpt_BienBanKiemDiemTuVong_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_BienBanKiemDiemTuVong1.EnforceConstraints = false;
            spGetDeathCheckRecordByDCRIDTableAdapter.Fill(dsXRpt_BienBanKiemDiemTuVong1.spGetDeathCheckRecordByDCRID,
                Convert.ToInt64(DeathCheckRecordID.Value));
        }
    }
}
