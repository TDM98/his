using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptTransferFormType2_1 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptTransferFormType2_1()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            dsTransferFormType2_11.EnforceConstraints = false;
            spTransferFormType2_1RptTableAdapter.Fill((DataSource as DataSchema.GeneralPlanning.dsTransferFormType2_1).spTransferFormType2_1Rpt
                , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
                , Convert.ToInt32(Quarter.Value), Convert.ToInt32(Month.Value)
                , Convert.ToInt32(Year.Value), Convert.ToByte(Flag.Value));
        }

        private void XRptTransferFormType2_1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
