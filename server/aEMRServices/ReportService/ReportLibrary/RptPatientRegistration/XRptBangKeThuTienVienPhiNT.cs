using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBangKeThuTienVienPhiNT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBangKeThuTienVienPhiNT()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBangKeThuTienVienPhiNT_BeforePrint);
        }

        public void FillData()
        {
            spRptInPtPaymentStatisticsTableAdapter.Fill(dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics
                                                                , Convert.ToDateTime(this.FromDate.Value)
                                                                , Convert.ToDateTime(this.ToDate.Value)
                                                                , Convert.ToInt64(this.StaffID.Value));
            decimal TotalAmount = 0;
            decimal TotalCoPayment = 0;
            decimal TotalHIPayment = 0;
            decimal TotalPatientPayment = 0;
            decimal SoTienPhaiNop = 0;
            if (dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics != null && dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows.Count > 0)
            {
                for (int i = 0; i < dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows.Count; i++)
                {
                    if (dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows[i]["Amount"] != DBNull.Value)
                    {
                        TotalAmount += Convert.ToDecimal(dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows[i]["Amount"]);
                    }
                    if (dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows[i]["TotalCoPayment"] != DBNull.Value)
                    {
                        TotalCoPayment += Convert.ToDecimal(dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows[i]["TotalCoPayment"]);
                    }
                    if (dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows[i]["TotalHIPayment"] != DBNull.Value)
                    {
                        TotalHIPayment += Convert.ToDecimal(dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows[i]["TotalHIPayment"]);
                    }
                    if (dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows[i]["TotalPatientPayment"] != DBNull.Value)
                    {
                        TotalPatientPayment += Convert.ToDecimal(dsBangKeThuTienVienPhiNT.spRptInPtPaymentStatistics.Rows[i]["TotalPatientPayment"]);
                    }
                }
                SoTienPhaiNop = TotalAmount;
                this.Parameters["SoTienPhaiNop"].Value = SoTienPhaiNop;
                this.Parameters["ReadMoneyTotalAmount"].Value = Globals.ReadMoneyToString(SoTienPhaiNop);
            }
        }
        private void XRptBangKeThuTienVienPhiNT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
