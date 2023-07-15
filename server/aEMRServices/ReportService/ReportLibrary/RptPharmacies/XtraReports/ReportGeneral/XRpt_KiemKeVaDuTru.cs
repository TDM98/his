using System;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_KiemKeVaDuTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_KiemKeVaDuTru()
        {
            InitializeComponent();
            FillData();
        }

        public void FillData()
        {
            dsKiemKeVaDuTru1.EnforceConstraints = false;
            baoCao_KiemKeVaDuTru_BaseDepotTableAdapter.Fill((DataSource as DataSchema.ReportGeneral.dsKiemKeVaDuTru).BaoCao_KiemKeVaDuTru_BaseDepot,
                Convert.ToInt32(Month.Value),
                Convert.ToInt32(Year.Value));
        }

        private void XRpt_KiemKeVaDuTru_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
