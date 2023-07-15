using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class RptHuongDanDungThuoc3x6 : XtraReport
    {
        //[DefaultValue(false)]
        //[SRCategory(ReportStringId.CatBehavior)]
        //public virtual bool AllowMarkupText { get; set; }

        public RptHuongDanDungThuoc3x6()
        {
            InitializeComponent();
        }
        
        public void FillData(System.Drawing.Printing.PrintEventArgs e)
        {
            dsHuongDanDungThuoc1.EnforceConstraints = false;
            spGetHuongDanDungThuocTableAdapter.Fill(dsHuongDanDungThuoc1.spGetHuongDanDungThuoc, Convert.ToInt64(this.PrescriptID.Value), Convert.ToBoolean(this.BeOfHIMedicineList.Value));

            //if (dsHuongDanDungThuoc1.spGetHuongDanDungThuoc != null && dsHuongDanDungThuoc1.spGetHuongDanDungThuoc.Rows.Count > 0)
            //{
            //    float Y = 0;
            //    foreach (DataRow row in dsHuongDanDungThuoc1.spGetHuongDanDungThuoc.Rows)
            //    {
            //        ReportHeader.Controls.Add(CreateLabel(row["GenericName"].ToString() + " " + row["Content"].ToString() + ", SL: " + row["Qty"].ToString() + " " + row["UnitName"].ToString(), 39.3f, Y));
            //        Y += 39.3f;
            //        ReportHeader.Controls.Add(CreateLabel(row["Dose"].ToString(), 39.3f, Y));
            //        Y += 39.3f;
                    
            //        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            //        string FullName = row["FullName"].ToString();
                    
            //        ReportHeader.Controls.Add(CreateLabel(textInfo.ToTitleCase(FullName.ToLower()) + ", " + row["AgeOnly"].ToString(), 39.4f, Y));
            //        Y += 39.4f;
            //    }
            //}
        }

        private void RptHuongDanDungThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData(e);
        }

        private static XRLabel CreateLabel(string text, float HeightF, float Y = 0)
        {
            XRLabel labeld = new XRLabel();
            labeld.LocationF = new PointF(5.58f, Y);
            labeld.WidthF = 230f;
            labeld.HeightF = HeightF;
            labeld.Text = text;

            labeld.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            if (labeld.Text.Length > 93) labeld.Font = new Font("Arial", 3);
            else if(labeld.Text.Length > 60) labeld.Font = new Font("Arial", 4);
            else if (labeld.Text.Length > 50) labeld.Font = new Font("Arial", 5);
            else if (labeld.Text.Length > 42) labeld.Font = new Font("Arial", 6);
            else if (labeld.Text.Length > 37) labeld.Font = new Font("Arial", 7);
            else if (labeld.Text.Length > 33) labeld.Font = new Font("Arial", 8);
            else if (labeld.Text.Length > 30) labeld.Font = new Font("Arial", 9);
            else if (labeld.Text.Length > 29) labeld.Font = new Font("Arial", 10);
            else labeld.Font = new Font("Arial", 11);

            return labeld;
        }
    }
}
