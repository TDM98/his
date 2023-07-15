using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_TomTatHoSoBenhAn : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_TomTatHoSoBenhAn()
        {
            InitializeComponent();
        }
        private void FillData()
        {
            dsXRpt_TomTatHoSoBenhAn1.EnforceConstraints = false;
            spGetSummaryMedicalRecords_ByIDTableAdapter.Fill(dsXRpt_TomTatHoSoBenhAn1.spGetSummaryMedicalRecords_ByID
                , Convert.ToInt64(SummaryMedicalRecordID.Value)
                , Convert.ToInt64(V_RegistrationType.Value)
                );
        }
        private void XRpt_TomTatHoSoBenhAn_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
