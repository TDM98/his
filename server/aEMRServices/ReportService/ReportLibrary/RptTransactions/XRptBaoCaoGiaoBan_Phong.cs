using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBaoCaoGiaoBan_Phong : XtraReport
    {
        public XRptBaoCaoGiaoBan_Phong()
        {
            InitializeComponent();
        }

        private void XRptBaoCaoGiaoBan_Phong_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBaoCaoGiaoBan1.EnforceConstraints = false;
            spDSCaKhamTheoPhongTableAdapter.Fill(dsBaoCaoGiaoBan1.spDSCaKhamTheoPhong, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));
        }

        private void XrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptBaoCaoGiaoBan_Bsy)((XRSubreport)sender).ReportSource).parFromDate.Value = Convert.ToDateTime(parFromDate.Value);
            ((XRptBaoCaoGiaoBan_Bsy)((XRSubreport)sender).ReportSource).parToDate.Value = Convert.ToDateTime(parToDate.Value);
        }
    }
}
