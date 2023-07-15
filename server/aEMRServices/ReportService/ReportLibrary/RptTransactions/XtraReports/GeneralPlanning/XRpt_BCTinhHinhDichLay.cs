using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCTinhHinhDichLay : XtraReport
    {
        public XRpt_BCTinhHinhDichLay()
        {
            InitializeComponent();
        }

        private void XRpt_BCTinhHinhDichLay_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            if (parFindPatient.Value.ToString() == "1")
            {
                xrLabel15.Text = "BÁO CÁO NỘI TRÚ";
            }
            else if (parFindPatient.Value.ToString() == "0")
            {
                xrLabel15.Text = "BÁO CÁO NGOẠI TRÚ";
            }
            else
            {
                xrLabel15.Text = "BÁO CÁO NỘI-NGOẠI TRÚ";
            }
        }

        private void XrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_BCTinhHinhDichLay_I)((XRSubreport)sender).ReportSource).Parameters["parFromDate"].Value = Convert.ToDateTime(parFromDate.Value.ToString());
            ((XRpt_BCTinhHinhDichLay_I)((XRSubreport)sender).ReportSource).Parameters["parToDate"].Value = Convert.ToDateTime(parToDate.Value.ToString());
            ((XRpt_BCTinhHinhDichLay_I)((XRSubreport)sender).ReportSource).Parameters["parFindPatient"].Value = parFindPatient.Value;
        }

        private void XrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRpt_BCTinhHinhDichLay_II)((XRSubreport)sender).ReportSource).Parameters["parFromDate"].Value = Convert.ToDateTime(parFromDate.Value.ToString());
            ((XRpt_BCTinhHinhDichLay_II)((XRSubreport)sender).ReportSource).Parameters["parToDate"].Value = Convert.ToDateTime(parToDate.Value.ToString());
            ((XRpt_BCTinhHinhDichLay_II)((XRSubreport)sender).ReportSource).Parameters["parFindPatient"].Value = parFindPatient.Value;
        }
    }
}
