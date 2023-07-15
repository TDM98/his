using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BC_QLKiemDuyetHoSo_KHTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BC_QLKiemDuyetHoSo_KHTH()
        {
            InitializeComponent();
        }

        private void XRpt_BC_QLKiemDuyetHoSo_KHTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBC_QLKiemDuyetHoSo_KHTH1.spRptCheckMedicalFiles_InPt, spRptCheckMedicalFiles_InPtTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
            }, int.MaxValue);
        }
    }
}
