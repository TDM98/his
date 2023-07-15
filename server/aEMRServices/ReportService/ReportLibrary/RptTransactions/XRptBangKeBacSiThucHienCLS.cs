using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBangKeBacSiThucHienCLS : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBangKeBacSiThucHienCLS()
        {
            InitializeComponent();
        }

        private void XRptBangKeBacSiThucHienCLS_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            //dsBangKeBacSiThucHienCLS1.EnforceConstraints = false;
            //spRpt_BangKeBacSiThucHienCLSTableAdapter.Fill(dsBangKeBacSiThucHienCLS1.spRpt_BangKeBacSiThucHienCLS, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value));

            ReportSqlProvider.Instance.ReaderIntoSchema(dsBangKeBacSiThucHienCLS1.spRpt_BangKeBacSiThucHienCLS, spRpt_BangKeBacSiThucHienCLSTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value),Convert.ToBoolean(Settlement.Value)
            }, int.MaxValue);

            if (parFindPatient.Value.ToString() == "0")
            {
                xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN CLS NGOẠI TRÚ";
            }
            else if (parFindPatient.Value.ToString() == "1")
            {
                xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN CLS NỘI TRÚ";
            }
            else
            {
                xrLabel15.Text = "BẢNG KÊ BÁC SĨ THỰC HIỆN CLS NỘI-NGOẠI TRÚ";
            }
        }
    }
}
