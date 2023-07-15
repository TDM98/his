using System;
using aEMR.DataAccessLayer.Providers;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp79aTH_BV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp79aTH_BV()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsTemp79aTH_BV1.spRpt_CreateTemplate79a_TH_BV, spRpt_CreateTemplate79a_TH_BVTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt32(Quarter.Value)
                , Convert.ToInt32(Month.Value), Convert.ToInt32(Year.Value), Convert.ToByte(flag.Value)
            }, int.MaxValue);

            decimal total = 0;
            if (dsTemp79aTH_BV1.spRpt_CreateTemplate79a_TH_BV != null && dsTemp79aTH_BV1.spRpt_CreateTemplate79a_TH_BV.Rows.Count > 0)
            {
                for (int i = 0; i < dsTemp79aTH_BV1.spRpt_CreateTemplate79a_TH_BV.Rows.Count; i++)
                {
                    if (dsTemp79aTH_BV1.spRpt_CreateTemplate79a_TH_BV.Rows[i]["t_bhtt"] != DBNull.Value)
                    {
                        // qty = Convert.ToDecimal(dsTemplate381.spRpt_PrintBillForPatientInsurance.Rows[i]["Qty"]);
                        total += Convert.ToDecimal(dsTemp79aTH_BV1.spRpt_CreateTemplate79a_TH_BV.Rows[i]["t_bhtt"]);
                    }
                }
                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (total < 0)
                {
                    temp = 0 - total;
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = total;
                    prefix = "";
                }
                Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }

        private void XRpt_Temp79aTH_BV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
