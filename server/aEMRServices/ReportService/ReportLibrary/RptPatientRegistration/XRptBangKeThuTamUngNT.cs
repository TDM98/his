using System;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBangKeThuTamUngNT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBangKeThuTamUngNT()
        {
            InitializeComponent();
            BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBangKeThuTamUngNT_BeforePrint);
        }

        public void FillData()
        {
            spRptInPtCashAdvanceStatisticsTableAdapter.Fill(dsBangKeThuTamUngNT.spRptInPtCashAdvanceStatistics
                , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(StaffID.Value), Convert.ToInt64(V_PaymentMode.Value));
            decimal TotalPaymentAmount = 0;
            decimal TotalCancelAmount = 0;
            decimal TotalRefundlAmount = 0;
            decimal TotalReturnlAmount = 0;
            decimal SoTienPhaiNop = 0;
            if (dsBangKeThuTamUngNT.spRptInPtCashAdvanceStatistics != null && dsBangKeThuTamUngNT.spRptInPtCashAdvanceStatistics.Rows.Count > 0)
            {

                for (int i = 0; i < dsBangKeThuTamUngNT.spRptInPtCashAdvanceStatistics.Rows.Count; i++)
                {
                    TotalPaymentAmount += Convert.ToDecimal(dsBangKeThuTamUngNT.spRptInPtCashAdvanceStatistics.Rows[i]["PaymentAmount"]);
                    TotalCancelAmount += Convert.ToDecimal(dsBangKeThuTamUngNT.spRptInPtCashAdvanceStatistics.Rows[i]["CancelAmount"]);
                    TotalRefundlAmount += Convert.ToDecimal(dsBangKeThuTamUngNT.spRptInPtCashAdvanceStatistics.Rows[i]["RefundAmount"]);
                    TotalReturnlAmount += Convert.ToDecimal(dsBangKeThuTamUngNT.spRptInPtCashAdvanceStatistics.Rows[i]["ReturnAmount"]);
                }
                SoTienPhaiNop = TotalPaymentAmount - TotalCancelAmount - TotalRefundlAmount - TotalReturnlAmount;
                Parameters["SoTienPhaiNop"].Value = SoTienPhaiNop;
                Parameters["ReadMoneyTotalAmount"].Value = Globals.ReadMoneyToString(SoTienPhaiNop);
            }
        }
        private void XRptBangKeThuTamUngNT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
