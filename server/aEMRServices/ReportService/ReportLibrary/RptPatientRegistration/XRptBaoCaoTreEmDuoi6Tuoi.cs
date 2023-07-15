using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBaoCaoTreEmDuoi6Tuoi : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoTreEmDuoi6Tuoi()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBaoCaoTreEmDuoi6Tuoi_BeforePrint);
        }

        public void FillData()
        {

            spRptOutPtChildUnderSixOldStatisticsTableAdapter.Fill(dsBaoCaoSoLieuTreEmDuoi6Tuoi.spRptOutPtChildUnderSixOldStatistics
                                                                , Convert.ToDateTime(this.FromDate.Value)
                                                                , Convert.ToDateTime(this.ToDate.Value));         
        }

        private void XRptBaoCaoTreEmDuoi6Tuoi_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
