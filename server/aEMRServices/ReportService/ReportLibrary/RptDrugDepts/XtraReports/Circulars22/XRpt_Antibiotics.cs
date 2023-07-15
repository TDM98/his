using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Circulars22
{
    public partial class XRpt_Antibiotics : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_Antibiotics()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsAntibiotics1.spRpt_Antibiotics, spRpt_AntibioticsTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString()), Convert.ToInt64(V_MedProductType.Value)
            }, int.MaxValue);
        }
        
        private void XRpt_Antibiotics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
