using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptDrugMedDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugMedDept()
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

            if (this.parFromDate.Value != null && Convert.ToDateTime(this.parFromDate.Value) > DateTime.MinValue)
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
            //xrTableCell13
            if (Convert.ToInt32(this.parV_MedProductType.Value) == 11001)
            {
                xrTableCell13.Text = "Tên thuốc";
            }
            else
                if (Convert.ToInt32(this.parV_MedProductType.Value) == 11002)
                {
                    xrTableCell13.Text = "Tên y cụ";
                }
                else
                    if (Convert.ToInt32(this.parV_MedProductType.Value) == 11003)
                    {
                        xrTableCell13.Text = "Tên hoá chất";
                    }
                    else
                    {
                        xrTableCell13.Text = "Tên";
                    }
            this.dsRptDrugMedDept1.EnforceConstraints = false;
            this.spSoKiemNhapThuoc_KhoaDuocTableAdapter.Fill(this.dsRptDrugMedDept1.spSoKiemNhapThuoc_KhoaDuoc,FromDate,ToDate,Quy,Thang,Nam,Convert.ToInt32(this.parViewBy.Value),Convert.ToInt32(this.parV_MedProductType.Value));
        }

        private void XRptDrugMedDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
