using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Circulars22
{
    public partial class XRpt_HospitalUseOfDrugs : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_HospitalUseOfDrugs()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsHospitalUseOfDrugs1.spRpt_HospitalUseOfDrugs, spRpt_HospitalUseOfDrugsTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString()), Convert.ToInt64(V_MedProductType.Value)
            }, int.MaxValue);
        }
        
        private void XRpt_HospitalUseOfDrugs_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (V_MedProductType.Value.ToString() == "11003")
            {
                xrLabel15.Text = "BÁO CÁO SỬ DỤNG HÓA CHẤT";
                xrLabel1.Text = "MS: 08D/BV-01";
                xrTableCell63.Text = "Lâm Sàng";
                xrTableCell65.Text = "Cận Lâm Sàng";
            } else if (V_MedProductType.Value.ToString() == "11001" || V_MedProductType.Value.ToString() == "11002")
            {
                xrLabel15.Text = "BÁO CÁO SỬ DỤNG THUỐC";
            } else
            {
                xrLabel15.Text = "BÁO CÁO SỬ DỤNG VẬT TƯ Y TẾ TIÊU HAO";
                xrLabel1.Text = "MS: 09D/BV-01";
            }
            FillData();
        }
    }
}
