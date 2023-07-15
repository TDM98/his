using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.OutInternal
{
    public partial class XRpt_XuatNoiBoTheoNguoiMua : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_XuatNoiBoTheoNguoiMua()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            baoCao_XuatNoiBoTenNguoiMuaTableAdapter.Fill((this.DataSource as DataSchema.OutInternal.dsXuatNoiBoTheoNguoiMua).BaoCao_XuatNoiBoTenNguoiMua,
                Convert.ToInt64(this.StoreID.Value),
                Convert.ToDateTime(this.FromDate.Value),
                Convert.ToDateTime(this.ToDate.Value),
                Convert.ToInt32(this.Quarter.Value),
                Convert.ToInt32(this.Month.Value),
                Convert.ToInt32(this.Year.Value),
                Convert.ToByte(this.Flag.Value),
                Convert.ToByte(this.OutTo.Value),
                Convert.ToInt64(this.TypID.Value));
        }

        private void XRpt_XuatNoiBoTheoNguoiMua_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
