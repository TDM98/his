using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp20_VTYTTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp20_VTYTTH()
        {
            InitializeComponent();
            //FillData();
        }

        private void FillData()
        {
            spRpt_CreateTemp20VTYTTH_ExcelTableAdapter.Fill(dsTemp20_VTYTTH1.spRpt_CreateTemp20VTYTTH_Excel
                                                        , Convert.ToDateTime(this.FromDate.Value)
                                                        , Convert.ToDateTime(this.ToDate.Value)
                                                        , Convert.ToInt32(this.Quarter.Value)
                                                        , Convert.ToInt32(this.Month.Value)
                                                        , Convert.ToInt32(this.Year.Value)
                                                        , Convert.ToByte(this.Flag.Value));
        }

        private void XRpt_Temp20_VTYTTH_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
