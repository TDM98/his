using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBaoCaoMienGiamNgoaiTru_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoMienGiamNgoaiTru_TV()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBaoCaoMienGiamNgoaiTru_TV_BeforePrint);
        }

        public void FillData()
        {
            
             

            spRptOutPtDiscountStatisticsTableAdapter.Fill(dsBaoCaoMienGiamNgoaiTru.spRptOutPtDiscountStatistics
                                                                , Convert.ToDateTime(FromDate.Value)
                                                                , Convert.ToDateTime(ToDate.Value)
                                                                , Convert.ToInt64(StaffID.Value));
            decimal TotalAmount = 0;
            decimal TotalDiscountAmount = 0;
            decimal SoTienPhaiNop = 0;

            if (dsBaoCaoMienGiamNgoaiTru.spRptOutPtDiscountStatistics != null && dsBaoCaoMienGiamNgoaiTru.spRptOutPtDiscountStatistics.Rows.Count > 0)
            {

                for (int i = 0; i < dsBaoCaoMienGiamNgoaiTru.spRptOutPtDiscountStatistics.Rows.Count; i++)
                {
                    TotalAmount += Convert.ToDecimal(dsBaoCaoMienGiamNgoaiTru.spRptOutPtDiscountStatistics.Rows[i]["TotalAmount"]);
                    TotalDiscountAmount += Convert.ToDecimal(dsBaoCaoMienGiamNgoaiTru.spRptOutPtDiscountStatistics.Rows[i]["DiscountAmount"]);
                }
                SoTienPhaiNop = TotalAmount - TotalDiscountAmount;

                Parameters["TotalAmount"].Value = TotalAmount;
                Parameters["TotalDiscountAmount"].Value = TotalDiscountAmount;
                Parameters["SoTienPhaiNop"].Value = SoTienPhaiNop;
                Parameters["ReadMoneyTotalAmount"].Value = Globals.ReadMoneyToString(SoTienPhaiNop);
            }
        }

        private void XRptBaoCaoMienGiamNgoaiTru_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
