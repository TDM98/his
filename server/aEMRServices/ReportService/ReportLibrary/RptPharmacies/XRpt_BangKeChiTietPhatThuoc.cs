using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRpt_BangKeChiTietPhatThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BangKeChiTietPhatThuoc()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            baoCao_BangKeChiTietPhatThuocTableAdapter.Fill((this.DataSource as DataSchema.dsBangKeChiTietPhatThuoc).BaoCao_BangKeChiTietPhatThuoc,this.PharmacyOutRepID.Value.ToString(),Convert.ToBoolean(this.IsInsurance.Value));
        }

        private void XRpt_BangKeChiTietPhatThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
