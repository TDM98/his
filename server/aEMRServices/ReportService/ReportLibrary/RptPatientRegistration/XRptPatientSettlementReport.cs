using System;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPatientSettlementReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientSettlementReport()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_PatientSettlement_DetailTableAdapter.Fill(dsPatientSettlementReport1.spRpt_PatientSettlement_Detail
                                                            , Convert.ToDateTime(FromDate.Value)
                                                            , Convert.ToDateTime(ToDate.Value)
                                                            , Convert.ToInt16(Quarter.Value)
                                                            , Convert.ToInt16(Month.Value)
                                                            , Convert.ToInt16(Year.Value)
                                                            , Convert.ToInt16(flag.Value)
                                                            , Convert.ToInt32(StaffID.Value)
                                                            , Convert.ToInt32(V_PaymentMode.Value));
        }

        private void XRptPatientSettlementReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
