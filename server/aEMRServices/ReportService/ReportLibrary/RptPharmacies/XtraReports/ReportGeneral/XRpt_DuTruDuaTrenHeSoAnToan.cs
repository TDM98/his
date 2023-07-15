
namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_DuTruDuaTrenHeSoAnToan : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_DuTruDuaTrenHeSoAnToan()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            baoCao_DuTruDuaVaoHeSoAnToanTableAdapter.Fill((this.DataSource as DataSchema.ReportGeneral.dsDuTruDuaTrenHeSoAnToan).BaoCao_DuTruDuaVaoHeSoAnToan);
        }

        private void XRpt_DuTruDuaTrenHeSoAnToan_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
