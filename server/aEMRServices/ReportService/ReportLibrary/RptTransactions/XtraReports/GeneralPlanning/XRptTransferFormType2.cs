using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptTransferFormType2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptTransferFormType2()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            spTransferFormType2RptTableAdapter.Fill((DataSource as DataSchema.GeneralPlanning.dsTransferFormType2).spTransferFormType2Rpt
                , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
                , Convert.ToInt32(Quarter.Value), Convert.ToInt32(Month.Value), Convert.ToInt32(Year.Value)
                , Convert.ToByte(Flag.Value));
        }

        private void XRptTransferFormType2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
