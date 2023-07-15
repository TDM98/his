using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.Outward
{
    public partial class XRptDanhSachXuatNhaThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDanhSachXuatNhaThuoc()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            Nullable<DateTime> FromDate = null;
            Nullable<DateTime> ToDate = null;
            int Quy = 0;
            int Thang = 0;
            int Nam = 0;

            if(this.parFromDate.Value!=null)
            {
                FromDate = Convert.ToDateTime(this.parFromDate.Value);
            }

            if (this.parToDate.Value != null)
            {
                ToDate = Convert.ToDateTime(this.parToDate.Value);
            }


            if (this.parQuy.Value != null)
            {
                Quy = Convert.ToInt32(this.parQuy.Value);
            }

            if (this.parThang.Value != null)
            {
                Thang = Convert.ToInt32(this.parThang.Value);
            }

            if (this.parNam.Value != null)
            {
                Nam = Convert.ToInt32(this.parNam.Value);
            }
            
            this.dsDanhSachXuatNhaThuoc1.EnforceConstraints = false;
            this.spRpt_DanhSachXuatNhaThuocTableAdapter.Fill(this.dsDanhSachXuatNhaThuoc1.spRpt_DanhSachXuatNhaThuoc, FromDate, ToDate, Quy, Thang, Nam, Convert.ToInt32(this.parViewBy.Value),Convert.ToInt64(this.parStoreID.Value));
        }

        private void XRptDanhSachXuatNhaThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
