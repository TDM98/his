using System;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_SellDrugGeneral : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_SellDrugGeneral()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            baoCao_BanThuocTongHopTableAdapter.Fill((DataSource as DataSchema.ReportGeneral.dsSellDrugGeneral).BaoCao_BanThuocTongHop,
                Convert.ToDateTime(FromDate.Value),
                Convert.ToDateTime(ToDate.Value),
                Convert.ToInt32(Quarter.Value),
                Convert.ToInt32(Month.Value),
                Convert.ToInt32(Year.Value),
                Convert.ToByte(Flag.Value),
                Convert.ToInt64(StoreID.Value),
                Convert.ToInt64(V_PaymentMode.Value));                       
        }

        private void XRpt_SellDrugGeneral_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
