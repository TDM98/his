using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward
{
    public partial class XRptDanhSachXuatKhoPhong : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDanhSachXuatKhoPhong()
        {
            InitializeComponent();
        }

        private void XRptDanhSachXuatKhoPhong_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            Nullable<DateTime> FromDate = null;
            Nullable<DateTime> ToDate = null;
            int Quy = 0;
            int Thang = 0;
            int Nam = 0;

            if(this.parFromDate.Value!=null && Convert.ToDateTime(this.parFromDate.Value) > DateTime.MinValue)
            {
                FromDate = Convert.ToDateTime(this.parFromDate.Value);
            }

            if (this.parToDate.Value != null && Convert.ToDateTime(this.parToDate.Value) > DateTime.MinValue)
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
            
            this.dsRpt_DanhSachXuatKhoPhong1.EnforceConstraints = false;
            this.spRpt_DanhSachXuatKhoaNoiTruTableAdapter.Fill(this.dsRpt_DanhSachXuatKhoPhong1.spRpt_DanhSachXuatKhoaNoiTru, Convert.ToInt32(this.parV_MedProductType.Value), FromDate, ToDate, Quy, Thang, Nam, Convert.ToInt32(this.parViewBy.Value),Convert.ToInt64(this.parStoreID.Value));
        }

    }
}
