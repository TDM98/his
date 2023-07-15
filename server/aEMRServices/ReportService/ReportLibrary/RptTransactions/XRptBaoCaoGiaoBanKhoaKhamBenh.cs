using DevExpress.XtraReports.UI;
using System;
using System.Data;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBaoCaoGiaoBanKhoaKhamBenh : XtraReport
    {
        public XRptBaoCaoGiaoBanKhoaKhamBenh()
        {
            InitializeComponent();
        }

        private void XRptBaoCaoGiaoBanKhoaKhamBenh_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBCGiaoBanNgoaiTru1.EnforceConstraints = false;
            spBCGiaoBanKhoaKhamBenhTableAdapter1.Fill(dsBCGiaoBanNgoaiTru1.spBCGiaoBanKhoaKhamBenh,
                                                        Convert.ToDateTime(parFromDate.Value),
                                                        Convert.ToDateTime(parToDate.Value),
                                                        Convert.ToInt32(Quarter.Value),
                                                        Convert.ToInt32(Month.Value),
                                                        Convert.ToInt32(Year.Value),
                                                        Convert.ToInt32(flag.Value));
        }
    }
}
