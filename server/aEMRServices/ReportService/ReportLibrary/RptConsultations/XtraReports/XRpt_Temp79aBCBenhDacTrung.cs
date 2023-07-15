using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRpt_Temp79aBCBenhDacTrung : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp79aBCBenhDacTrung()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            dsTemp79aBCBenhDacTrung1.EnforceConstraints = false;
            spRpt_BC_BNKhamBenhDacTrung_ViewTableAdapter.Fill(dsTemp79aBCBenhDacTrung1.spRpt_BC_BNKhamBenhDacTrung_View, 
                Convert.ToDateTime(FromDate.Value), 
                Convert.ToDateTime(ToDate.Value)
            );
        }

        private void XRpt_Temp79aBCBenhDacTrung_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
