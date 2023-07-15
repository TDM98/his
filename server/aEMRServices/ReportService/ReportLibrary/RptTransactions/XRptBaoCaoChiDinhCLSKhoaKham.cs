using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBaoCaoChiDinhCLSKhoaKham : XtraReport
    {
        public XRptBaoCaoChiDinhCLSKhoaKham()
        {
            InitializeComponent();
        }

        private void XRptBaoCaoChiDinhCLSKhoaKham_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBCGiaoBanNgoaiTru1.EnforceConstraints = false;
            spBCChiDinhCLSKhoaKhamTableAdapter1.Fill(dsBCGiaoBanNgoaiTru1.spBCChiDinhCLSKhoaKham, 
                                                        Convert.ToDateTime(parFromDate.Value), 
                                                        Convert.ToDateTime(parToDate.Value),
                                                        Convert.ToInt32(Quarter.Value),
                                                        Convert.ToInt32(Month.Value),
                                                        Convert.ToInt32(Year.Value),
                                                        Convert.ToInt32(flag.Value));
        }
    }
}
