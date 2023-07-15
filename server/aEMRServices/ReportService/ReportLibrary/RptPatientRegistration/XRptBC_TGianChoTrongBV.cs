using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptBC_TGianChoTrongBV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBC_TGianChoTrongBV()
        {
            InitializeComponent();
        }

        private void XRptBC_TGianChoTrongBV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsBC_TGianChoTrongBV.EnforceConstraints = false;
            xRptBC_TGianChoTrongBVTableAdapter.Fill(dsBC_TGianChoTrongBV.XRptBC_TGianChoTrongBV
                , Convert.ToDateTime(parFromDate.Value)
                , Convert.ToDateTime(parToDate.Value)
                );
        }
    }
}
