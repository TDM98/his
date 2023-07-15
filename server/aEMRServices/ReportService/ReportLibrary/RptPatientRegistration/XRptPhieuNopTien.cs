using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptPhieuNopTien : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuNopTien()
        {
            InitializeComponent();
        }
        public void FillData() 
        {
            dsPhieuNopTien1.EnforceConstraints = false;
            spRpt_PhieuNopTienTableAdapter.Fill(dsPhieuNopTien1.spRpt_PhieuNopTien
                , Convert.ToDateTime(this.FromDate.Value)
                , Convert.ToDateTime(this.ToDate.Value),
                Convert.ToInt16(this.Quarter.Value),
                Convert.ToInt16(this.Month.Value),
                Convert.ToInt16(this.Year.Value),
                Convert.ToInt16(this.flag.Value),
                Convert.ToInt16(this.StaffID.Value));

            decimal ThucThu = 0;
            if (dsPhieuNopTien1.spRpt_PhieuNopTien!= null && dsPhieuNopTien1.spRpt_PhieuNopTien.Rows.Count > 0)
            {
                ThucThu = Convert.ToDecimal(dsPhieuNopTien1.spRpt_PhieuNopTien.Rows[0]["TotalPatientPayment"]);
                
                System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp1 = 0;
                string prefix1 = "";
                if (ThucThu < 0)
                {
                    temp1 = 0 - ThucThu;
                    prefix1 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp1 = ThucThu;
                    prefix1 = "";
                }
                this.Parameters["ReadMoney"].Value = "(" + prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper()) + ")";
            }
        }

        private void XRptPhieuNopTien_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
