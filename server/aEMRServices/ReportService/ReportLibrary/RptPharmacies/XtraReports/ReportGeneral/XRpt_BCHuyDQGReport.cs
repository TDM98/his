using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_BCHuyDQGReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCHuyDQGReport()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsBCHuyDQGReport1.EnforceConstraints = false;
            spRptDeleteDQGReportTableAdapter.Fill(dsBCHuyDQGReport1.spRptDeleteDQGReport,
                Convert.ToDateTime(FromDate.Value),
                Convert.ToDateTime(ToDate.Value),
                Convert.ToInt16(Quarter.Value),
                Convert.ToInt16(Month.Value),
                Convert.ToInt16(Year.Value),
                Convert.ToInt16(Flag.Value)
            );
        }

        private void XRpt_BCXuatDuocNoiBo_NT_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
