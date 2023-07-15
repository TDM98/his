using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DataEntities;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public sealed partial class XRptRepayCashAdvance : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptRepayCashAdvance()
        {
            InitializeComponent();

            BeforePrint += XRptRepayCashAdvance_BeforePrint;
            PrintingSystem.ShowPrintStatusDialog = false;
        }


        void XRptRepayCashAdvance_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            sp_Rpt_RepayCashAdvanceTableAdapter.ClearBeforeFill = true;

            sp_Rpt_RepayCashAdvanceTableAdapter.Fill(dsRepayCashAdvance1.sp_Rpt_RepayCashAdvance, Convert.ToInt32(this.param_PaymentID.Value));

            if (dsRepayCashAdvance1.sp_Rpt_RepayCashAdvance == null || dsRepayCashAdvance1.sp_Rpt_RepayCashAdvance.Rows.Count <= 0)
            {
                return;
            }

            var total = dsRepayCashAdvance1.sp_Rpt_RepayCashAdvance.Rows[0]["PayAmount"] as decimal?;
            if (total.HasValue)
            {
                var cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                var converter = new NumberToLetterConverter();

                Parameters["param_AmountInWords"].Value = converter.Convert(total.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }
    }
}
