using System;
using aEMR.DataAccessLayer.Providers;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptTransactions.XtraReports.Accountant
{
    public partial class XRpt_BCNhapTuNCC : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCNhapTuNCC()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCNhapTuNCC1.sp_BCNhapTuNCC, sp_BCNhapTuNCCTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString())
                , Convert.ToInt64(V_MedProductType.Value)
            }, int.MaxValue);

            decimal totalSumSell = 0;
            if (dsBCNhapTuNCC1.sp_BCNhapTuNCC != null && dsBCNhapTuNCC1.sp_BCNhapTuNCC.Rows.Count > 0)
            {
                for (int i = 0; i < dsBCNhapTuNCC1.sp_BCNhapTuNCC.Rows.Count; i++)
                {
                    totalSumSell += Convert.ToDecimal(dsBCNhapTuNCC1.sp_BCNhapTuNCC.Rows[i]["ThanhTien"]);
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
        
        private void XRptInOutStocks_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
