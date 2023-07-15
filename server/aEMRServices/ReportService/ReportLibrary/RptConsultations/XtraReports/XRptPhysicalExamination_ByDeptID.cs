using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptPhysicalExamination_ByDeptID : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhysicalExamination_ByDeptID()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsPhysicalExamination_ByDeptID1.spRptPhysicalExamination_ByDeptID, spRptPhysicalExamination_ByDeptIDTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(DeptID.Value),
                Convert.ToDateTime(FromDate.Value.ToString()),
                Convert.ToDateTime(ToDate.Value.ToString()),
                Convert.ToBoolean(IsWeb.Value)
            }, int.MaxValue);
        }

        private void XRptBCDoDHST_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
