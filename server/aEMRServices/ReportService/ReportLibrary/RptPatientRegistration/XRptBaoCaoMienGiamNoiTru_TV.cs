using System;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBaoCaoMienGiamNoiTru_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoMienGiamNoiTru_TV()
        {
            InitializeComponent();
            BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBaoCaoMienGiamNoiTru_TV_BeforePrint);
        }

        public void FillData()
        {
            spRptInPtDiscountStatisticsTableAdapter.Fill(dsBaoCaoMienGiamNoiTru1.spRptInPtDiscountStatistics
                                                                , Convert.ToDateTime(FromDate.Value)
                                                                , Convert.ToDateTime(ToDate.Value)
                                                                , Convert.ToInt64(StaffID.Value)
                                                                , Convert.ToInt64(V_RegistrationType.Value));
            decimal TotalAmount = 0;
            decimal TotalDiscountAmount = 0;
            decimal SoTienPhaiNop = 0;

            if (dsBaoCaoMienGiamNoiTru1.spRptInPtDiscountStatistics != null && dsBaoCaoMienGiamNoiTru1.spRptInPtDiscountStatistics.Rows.Count > 0)
            {

                for (int i = 0; i < dsBaoCaoMienGiamNoiTru1.spRptInPtDiscountStatistics.Rows.Count; i++)
                {
                    TotalAmount += Convert.ToDecimal(dsBaoCaoMienGiamNoiTru1.spRptInPtDiscountStatistics.Rows[i]["TotalAmount"]);
                    TotalDiscountAmount += Convert.ToDecimal(dsBaoCaoMienGiamNoiTru1.spRptInPtDiscountStatistics.Rows[i]["DiscountAmount"]);
                }
                SoTienPhaiNop = TotalAmount - TotalDiscountAmount;

                Parameters["TotalAmount"].Value = TotalAmount;
                Parameters["TotalDiscountAmount"].Value = TotalDiscountAmount;
                Parameters["SoTienPhaiNop"].Value = SoTienPhaiNop;
                Parameters["ReadMoneyTotalAmount"].Value = Globals.ReadMoneyToString(SoTienPhaiNop);
            }
        }

        private void XRptBaoCaoMienGiamNoiTru_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
