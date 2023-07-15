using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BaoCaoCongSuatGiuongBenh : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoCongSuatGiuongBenh()
        {
            InitializeComponent();
        }

        private void XRpt_BaoCaoCongSuatGiuongBenh_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsBaoCaoCongSuatGiuongBenh1.EnforceConstraints = false;
            sp_RptBaoCaoCongSuatGiuongBenhTableAdapter.Fill(dsBaoCaoCongSuatGiuongBenh1.sp_RptBaoCaoCongSuatGiuongBenh,
                Convert.ToDateTime(FromDate.Value),
                Convert.ToDateTime(ToDate.Value),
                Convert.ToInt32(Quarter.Value),
                Convert.ToInt32(Month.Value),
                Convert.ToInt32(Year.Value),
                Convert.ToInt32(flag.Value),
                Convert.ToInt64(DeptID.Value));
        }
    }
}
