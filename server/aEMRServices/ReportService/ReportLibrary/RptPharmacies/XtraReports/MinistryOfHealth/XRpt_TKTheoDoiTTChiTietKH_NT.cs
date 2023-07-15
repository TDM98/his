using System;
using aEMR.DataAccessLayer.Providers;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.MinistryOfHealth
{
    public partial class XRpt_TKTheoDoiTTChiTietKH_NT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TKTheoDoiTTChiTietKH_NT()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsTKTheoDoiTTChiTietKH_NT1.sp_TKTheoDoiTTChiTietKH_NT, sp_TKTheoDoiTTChiTietKH_NTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt32(StoreID.Value), Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(DrugID.Value)
            }, int.MaxValue);
        }

        private void XRpt_TKTheoDoiTTChiTietKH_NT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
