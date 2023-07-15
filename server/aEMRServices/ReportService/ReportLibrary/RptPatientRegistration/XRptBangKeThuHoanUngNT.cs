using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBangKeThuHoanUngNT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBangKeThuHoanUngNT()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBangKeThuHoanUngNT_BeforePrint);
        }

        public void FillData()
        {
            spRptInPtPaidCashAdvanceStatisticsTableAdapter.Fill(dsBangKeThuHoanUngNT.spRptInPtPaidCashAdvanceStatistics
                                                                , Convert.ToDateTime(this.FromDate.Value)
                                                                , Convert.ToDateTime(this.ToDate.Value)
                                                                , Convert.ToInt64(this.StaffID.Value));
            decimal TotalPaymentAmount = 0;
            decimal TotalRefundAmount = 0;
            decimal TotalCancelAmount = 0;
            decimal TotalReturnAmount = 0;
            decimal SoTienPhaiNop = 0;
            if (dsBangKeThuHoanUngNT.spRptInPtPaidCashAdvanceStatistics != null && dsBangKeThuHoanUngNT.spRptInPtPaidCashAdvanceStatistics.Rows.Count > 0)
            {

                for (int i = 0; i < dsBangKeThuHoanUngNT.spRptInPtPaidCashAdvanceStatistics.Rows.Count; i++)
                {
                    TotalPaymentAmount += Convert.ToDecimal(dsBangKeThuHoanUngNT.spRptInPtPaidCashAdvanceStatistics.Rows[i]["PaymentAmount"]);
                    TotalRefundAmount += Convert.ToDecimal(dsBangKeThuHoanUngNT.spRptInPtPaidCashAdvanceStatistics.Rows[i]["RefundAmount"]);
                    TotalCancelAmount += Convert.ToDecimal(dsBangKeThuHoanUngNT.spRptInPtPaidCashAdvanceStatistics.Rows[i]["CancelAmount"]);
                    TotalReturnAmount += Convert.ToDecimal(dsBangKeThuHoanUngNT.spRptInPtPaidCashAdvanceStatistics.Rows[i]["ReturnAmount"]);
                }
                SoTienPhaiNop = TotalPaymentAmount - TotalRefundAmount - TotalCancelAmount - TotalReturnAmount;
                this.Parameters["SoTienPhaiNop"].Value = SoTienPhaiNop;
                this.Parameters["ReadMoneyTotalAmount"].Value = Globals.ReadMoneyToString(SoTienPhaiNop);
            }
        }
        private void XRptBangKeThuHoanUngNT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
