using System;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
/*
* 20181103 #001 TNHX: [BM0005214] Update report PhieuSoanThuoc base RefApplicationConfig.MixedHIPharmacyStores (don't print report if Drug has OutHIAllowedPrice == 0)
*/
namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class PhieuSoanThuocBH : XtraReport
    {
        public PhieuSoanThuocBH()
        {
            InitializeComponent();
        }

        public void FillData(System.Drawing.Printing.PrintEventArgs e)
        {
            dsPhieuSoanThuocPaging1.EnforceConstraints = false;
            spRpt_PhieuSoanThuoc_InfoTableAdapter1.Fill((DataSource as DataSchema.dsPhieuSoanThuocPaging).spRpt_PhieuSoanThuoc_Info, Convert.ToInt64(PrescriptID.Value));
            spRpt_PhieuSoanThuoc_DetailsTableAdapter1.Fill((DataSource as DataSchema.dsPhieuSoanThuocPaging).spRpt_PhieuSoanThuoc_Details, Convert.ToInt64(PrescriptID.Value));
        }

        private void PhieuSoanThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData(e);
        }
    }
}
