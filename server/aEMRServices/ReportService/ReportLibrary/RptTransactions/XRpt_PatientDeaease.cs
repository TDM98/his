using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_PatientDeaease : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PatientDeaease()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            //dsThongKeDsachKBTheoBacSi1.EnforceConstraints = false;
            //spRpt_ThongKeDsachBacSiKhamBenhTableAdapter.Fill(dsThongKeDsachKBTheoBacSi1.spRpt_ThongKeDsachBacSiKhamBenh, Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), 1);

            ReportSqlProvider.Instance.ReaderIntoSchema(dsPatientDecease1.spRptPatientDecease, spRptPatientDeceaseTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
            }, int.MaxValue);
        }

        private void XRpt_PatientDeaease_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
