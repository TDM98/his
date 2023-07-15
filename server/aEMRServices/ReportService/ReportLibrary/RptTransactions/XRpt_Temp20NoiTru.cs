using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp20NoiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp20NoiTru()
        {
            InitializeComponent();
            //FillData();
        }

        private void FillData()
        {
            spRpt_CreateTemp20NoiTruTableAdapter.Fill(dsTemp20NoiTru1.spRpt_CreateTemp20NoiTru
                                                    , Convert.ToDateTime(this.FromDate.Value)
                                                    , Convert.ToDateTime(this.ToDate.Value)
                                                    , Convert.ToInt32(this.Quarter.Value)
                                                    , Convert.ToInt32(this.Month.Value)
                                                    , Convert.ToInt32(this.Year.Value)
                                                    , Convert.ToByte(this.Flag.Value));
        }

        private void XRpt_Temp20NoiTru_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
