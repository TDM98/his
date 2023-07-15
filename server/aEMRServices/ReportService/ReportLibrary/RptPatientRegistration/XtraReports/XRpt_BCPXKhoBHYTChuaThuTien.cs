using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration.XtraReports
{
    public partial class XRpt_BCPXKhoBHYTChuaThuTien : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCPXKhoBHYTChuaThuTien()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsBCPXKhoBHYTChuaThuTien1.EnforceConstraints = false;
            spRpt_BCPXKhoBHYTChuaThuTienTableAdapter.Fill(dsBCPXKhoBHYTChuaThuTien1.spRpt_BCPXKhoBHYTChuaThuTien,
               Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(parStaffID.Value));

            decimal totalSumSell = 0;
            if (dsBCPXKhoBHYTChuaThuTien1.spRpt_BCPXKhoBHYTChuaThuTien != null && dsBCPXKhoBHYTChuaThuTien1.spRpt_BCPXKhoBHYTChuaThuTien.Rows.Count > 0)
            {
                for (int i = 0; i < dsBCPXKhoBHYTChuaThuTien1.spRpt_BCPXKhoBHYTChuaThuTien.Rows.Count; i++)
                {
                    totalSumSell += Convert.ToDecimal(dsBCPXKhoBHYTChuaThuTien1.spRpt_BCPXKhoBHYTChuaThuTien.Rows[i]["Tien"]);
                }
                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temptotalSumSell = 0;
                string prefixtotalSumSell = "";
                if (totalSumSell < 0)
                {
                    temptotalSumSell = 0 - totalSumSell;
                    prefixtotalSumSell = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temptotalSumSell = totalSumSell;
                    prefixtotalSumSell = "";
                }
                Parameters["ReadMoney"].Value = "( " + prefixtotalSumSell + converter.Convert(Math.Round(temptotalSumSell, 0).ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + " " + eHCMSResources.Z0872_G1_Dong.ToLower() + " )";
            }
        }

        private void XRpt_BCPXKhoBHYTChuaThuTien_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
