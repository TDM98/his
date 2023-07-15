using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions.XtraReports
{
    public partial class XRpt_LichDangKyKhamNgoaiGio : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_LichDangKyKhamNgoaiGio()
        {
            InitializeComponent();
        }

        private void XRpt_LichDangKyKhamNgoaiGio_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_LichDangKyKhamNgoaiGio1.EnforceConstraints = false;
            spXRpt_LichDangKyKhamNgoaiGioTableAdapter.Fill(dsXRpt_LichDangKyKhamNgoaiGio1.spXRpt_LichDangKyKhamNgoaiGio
                , Convert.ToInt64(OvertimeWorkingWeekID.Value));
        }

        private void SubMonday_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["OvertimeWorkingWeekID"].Value = Convert.ToInt32(OvertimeWorkingWeekID.Value);
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["WorkDate"].Value = Convert.ToDateTime(GetCurrentColumnValue("Monday"));
        }

        private void SubTuesday_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["OvertimeWorkingWeekID"].Value = Convert.ToInt32(OvertimeWorkingWeekID.Value);
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["WorkDate"].Value = Convert.ToDateTime(GetCurrentColumnValue("TuesDay"));
        }

        private void SubWednesday_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["OvertimeWorkingWeekID"].Value = Convert.ToInt32(OvertimeWorkingWeekID.Value);
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["WorkDate"].Value = Convert.ToDateTime(GetCurrentColumnValue("Wednesday"));
        }

        private void SubThursday_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["OvertimeWorkingWeekID"].Value = Convert.ToInt32(OvertimeWorkingWeekID.Value);
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["WorkDate"].Value = Convert.ToDateTime(GetCurrentColumnValue("Thursday"));
        }

        private void SubFriday_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["OvertimeWorkingWeekID"].Value = Convert.ToInt32(OvertimeWorkingWeekID.Value);
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["WorkDate"].Value = Convert.ToDateTime(GetCurrentColumnValue("Friday"));
        }

        private void SubSaturday_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["OvertimeWorkingWeekID"].Value = Convert.ToInt32(OvertimeWorkingWeekID.Value);
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["WorkDate"].Value = Convert.ToDateTime(GetCurrentColumnValue("Saturday"));
        }

        private void SubSunday_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["OvertimeWorkingWeekID"].Value = Convert.ToInt32(OvertimeWorkingWeekID.Value);
            ((XRpt_LichDangKyKhamNgoaiGioSub)((XRSubreport)sender).ReportSource).Parameters["WorkDate"].Value = Convert.ToDateTime(GetCurrentColumnValue("Sunday"));
        }
    }
}
