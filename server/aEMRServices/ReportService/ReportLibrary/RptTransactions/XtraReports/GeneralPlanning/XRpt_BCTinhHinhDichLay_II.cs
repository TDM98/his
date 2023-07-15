using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCTinhHinhDichLay_II : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCTinhHinhDichLay_II()
        {
            InitializeComponent();
        }

        private void XRpt_BCTinhHinhDichLay_II_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBCTinhHinhDichLay1.EnforceConstraints = false;
            sp_BCTinhHinhDichLay_IITableAdapter.Fill(dsBCTinhHinhDichLay1.sp_BCTinhHinhDichLay_II, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value));
        }
    }
}
