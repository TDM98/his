using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRpt_NutritionalRating : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_NutritionalRating()
        {
            InitializeComponent();
        }

        private void XRpt_NutritionalRating_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_NutritionalRating1.EnforceConstraints = false;
            spGetNutritionalRatingByIDTableAdapter.Fill(dsXRpt_NutritionalRating1.spGetNutritionalRatingByID,  Convert.ToInt64(NutritionalRatingID.Value));
        }
    }
}
