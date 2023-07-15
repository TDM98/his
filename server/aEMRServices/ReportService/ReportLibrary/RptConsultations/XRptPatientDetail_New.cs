using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientDetail_New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientDetail_New()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID
                , spGetPatientDetail_ByPtRegistrationIDTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(parPtRegistrationID.Value)
            }, int.MaxValue);
        }

        private void XRptPatientDetail_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
