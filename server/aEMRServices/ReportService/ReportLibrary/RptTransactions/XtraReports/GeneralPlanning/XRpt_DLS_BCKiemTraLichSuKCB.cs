using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_DLS_BCKiemTraLichSuKCB : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_DLS_BCKiemTraLichSuKCB()
        {
            InitializeComponent();
        }

        private void XRpt_DLS_BCKiemTraLichSuKCB_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsDLS_BCKiemTraLichSuKCB1.spRptCheckDoctorDiagAndPrescription, spRptCheckDoctorDiagAndPrescriptionTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
            }, int.MaxValue);
        }
    }
}
