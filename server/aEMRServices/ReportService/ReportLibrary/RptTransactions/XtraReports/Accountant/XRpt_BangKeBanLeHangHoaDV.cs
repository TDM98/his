using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BangKeBanLeHangHoaDV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BangKeBanLeHangHoaDV()
        {
            InitializeComponent();
        }

        private void XRpt_BangKeBanLeHangHoaDV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBangKeBanLeHangHoaDV1.spRpt_BangKeBanLeHangHoaDV, spRpt_BangKeBanLeHangHoaDVTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt16(parFindPatient.Value)
            }, int.MaxValue);

            //if (parFindPatient.Value.ToString() == "1")
            //{
            //    xrLabel15.Text = "THỐNG KÊ DANH SÁCH KHÁM BỆNH NỘI TRÚ THEO BÁC SĨ";
            //} else if (parFindPatient.Value.ToString() == "0")
            //{
            //    xrLabel15.Text = "THỐNG KÊ DANH SÁCH KHÁM BỆNH NGOẠI TRÚ THEO BÁC SĨ";
            //}
            //else
            //{
            //    xrLabel15.Text = "THỐNG KÊ DANH SÁCH KHÁM BỆNH NỘI-NGOẠI TRÚ THEO BÁC SĨ";
            //}
        }
    }
}
