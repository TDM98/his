using System;
using System.Linq;
using aEMR.DataAccessLayer.Providers;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.MinistryOfHealth
{
    public partial class XRpt_TKTheoDoiNXTThuocKhac_NT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TKTheoDoiNXTThuocKhac_NT()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsTKTheoDoiNXTThuocKhac_NT1.sp_TKTheoDoiNXTThuocKhac_NT, sp_TKTheoDoiNXTThuocKhac_NTTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt32(StoreID.Value), Convert.ToInt32(DrugID.Value), Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
            }, int.MaxValue);

            if (dsTKTheoDoiNXTThuocKhac_NT1.sp_TKTheoDoiNXTThuocKhac_NT.FirstOrDefault() != null && !string.IsNullOrEmpty(dsTKTheoDoiNXTThuocKhac_NT1.sp_TKTheoDoiNXTThuocKhac_NT.FirstOrDefault()["NCC"].ToString()))
            {
                xrLabel3.Text = "Nhà sản xuất: " + dsTKTheoDoiNXTThuocKhac_NT1.sp_TKTheoDoiNXTThuocKhac_NT.FirstOrDefault()["NCC"].ToString();
            }
            else
            {
                xrLabel3.Text = "Nhà sản xuất: ";
            }
        }

        private void XRpt_TKTheoDoiNXTThuocKhac_NT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
