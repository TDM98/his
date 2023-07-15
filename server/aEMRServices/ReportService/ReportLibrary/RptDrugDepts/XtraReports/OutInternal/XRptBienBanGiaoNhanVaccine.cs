using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal
{
    public partial class XRptBienBanGiaoNhanVaccine : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBienBanGiaoNhanVaccine()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsBienBanGiaoNhanVaccine1.EnforceConstraints = false;
            spBienBanGiaoNhanVaccineTableAdapter.Fill(dsBienBanGiaoNhanVaccine1.spBienBanGiaoNhanVaccine, Convert.ToInt64(outiID.Value), Convert.ToInt64(V_MedProductType.Value));
        }

        private void XRptBienBanGiaoNhanVaccine_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
