using System;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral
{
    public partial class XRpt_ReturnDrugGeneral : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ReturnDrugGeneral()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            dsReturnDrugGeneral1.EnforceConstraints = false;
            baoCao_TraThuocTongHopTableAdapter.Fill((DataSource as DataSchema.ReportGeneral.dsReturnDrugGeneral).BaoCao_TraThuocTongHop,
                Convert.ToDateTime(FromDate.Value),
                Convert.ToDateTime(ToDate.Value),
                Convert.ToInt32(Quarter.Value),
                Convert.ToInt32(Month.Value),
                Convert.ToInt32(Year.Value),
                Convert.ToByte(Flag.Value));
        }

        private void XRpt_ReturnDrugGeneral_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
