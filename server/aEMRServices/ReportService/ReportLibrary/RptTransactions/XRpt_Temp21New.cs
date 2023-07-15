using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_Temp21New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Temp21New()
        {
            InitializeComponent();
            //FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsCreateTemp21_New1.spRpt_CreateTemp21_New, spRpt_CreateTemp21_NewTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt32(Quarter.Value)
                , Convert.ToInt32(Month.Value), Convert.ToInt32(Year.Value), Convert.ToByte(Flag.Value)
                , Convert.ToInt32(TypeKCBBD.Value)
            }, int.MaxValue);
        }

        private void XRpt_Temp21New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
