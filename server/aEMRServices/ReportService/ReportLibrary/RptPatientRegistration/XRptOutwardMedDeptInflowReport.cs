using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutwardMedDeptInflowReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutwardMedDeptInflowReport()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptOutwardMedDeptInflowReport_BeforePrint);

        }

        public void FillData()
        {
            spRpt_OutwardMedDeptInflowReportTableAdapter.Fill(dsOutwardMedDeptInflow1.spRpt_OutwardMedDeptInflowReport
                                                            , Convert.ToDateTime(this.FromDate.Value)
                                                            , Convert.ToDateTime(this.ToDate.Value)
                                                            , Convert.ToInt16(this.Quarter.Value)
                                                            , Convert.ToInt16(this.Month.Value)
                                                            , Convert.ToInt16(this.Year.Value)
                                                            , Convert.ToInt16(this.flag.Value)
                                                            , Convert.ToInt32(this.StaffID.Value));

            decimal TotalPaymentAmount = 0;

            if (dsOutwardMedDeptInflow1.spRpt_OutwardMedDeptInflowReport != null && dsOutwardMedDeptInflow1.spRpt_OutwardMedDeptInflowReport.Rows.Count > 0)
            {

                for (int i = 0; i < dsOutwardMedDeptInflow1.spRpt_OutwardMedDeptInflowReport.Rows.Count; i++)
                {
                    TotalPaymentAmount += Convert.ToDecimal(dsOutwardMedDeptInflow1.spRpt_OutwardMedDeptInflowReport.Rows[i]["PayAmount"]);
                }


                System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp1 = 0;
                string prefix1 = "";
                if (TotalPaymentAmount < 0)
                {
                    temp1 = 0 - TotalPaymentAmount;
                    prefix1 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am);
                }
                else
                {
                    temp1 = TotalPaymentAmount;
                    prefix1 = "";
                }
                this.Parameters["ReadMoney"].Value = prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }

        }



        private void XRptOutwardMedDeptInflowReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
