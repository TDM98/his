using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_LichDangKyKhamNgoaiGioSub : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_LichDangKyKhamNgoaiGioSub()
        {
            InitializeComponent();
        }

        private void XRpt_LichDangKyKhamNgoaiGioSub_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_LichDangKyKhamNgoaiGioSub1.EnforceConstraints = false;
            spXRpt_LichDangKyKhamNgoaiGioSubTableAdapter.Fill(dsXRpt_LichDangKyKhamNgoaiGioSub1.spXRpt_LichDangKyKhamNgoaiGioSub
                , Convert.ToInt64(OvertimeWorkingWeekID.Value)
                , Convert.ToDateTime(WorkDate.Value));
        }
    }
}
