using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRptDanhMucKyThuatMoi : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDanhMucKyThuatMoi()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            spBC_DM_KyThuatMoiTableAdapter.Fill((DataSource as DataSchema.dsDanhMucKyThuatMoi).spBC_DM_KyThuatMoi
                , Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value)
                , Convert.ToInt32(Quarter.Value), Convert.ToInt32(Month.Value), Convert.ToInt32(Year.Value)
                , Convert.ToByte(Flag.Value));
        }

        private void XRptDanhMucKyThuatMoi_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
