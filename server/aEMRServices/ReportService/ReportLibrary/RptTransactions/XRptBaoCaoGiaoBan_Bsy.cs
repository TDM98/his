using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBaoCaoGiaoBan_Bsy : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoGiaoBan_Bsy()
        {
            InitializeComponent();
        }

        private void XRptBaoCaoGiaoBan_Bsy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBaoCaoGiaoBan1.EnforceConstraints = false;
            spBaoCaoGiaoBan_BsyKhamTableAdapter.Fill(dsBaoCaoGiaoBan1.spBaoCaoGiaoBan_BsyKham, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));
        }
    }
}
