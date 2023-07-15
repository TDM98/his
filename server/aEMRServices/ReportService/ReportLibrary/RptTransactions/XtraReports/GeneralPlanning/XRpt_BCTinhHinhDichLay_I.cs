using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCTinhHinhDichLay_I : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCTinhHinhDichLay_I()
        {
            InitializeComponent();
        }

        private void XRpt_BCTinhHinhDichLay_I_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBCTinhHinhDichLay1.EnforceConstraints = false;
            sp_BCTinhHinhDichLay_ITableAdapter.Fill(dsBCTinhHinhDichLay1.sp_BCTinhHinhDichLay_I, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), 1);
        }
    }
}
