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
    public sealed partial class XRptPatientCashAdvance : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientCashAdvance()
        {
            InitializeComponent();

            BeforePrint += XRptPatientCashAdvance_BeforePrint;
            PrintingSystem.ShowPrintStatusDialog = false;
        }


        void XRptPatientCashAdvance_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            if (dsPatientCashAdvance1.spPatientCashAdvance_GetByID == null || dsPatientCashAdvance1.spPatientCashAdvance_GetByID.Rows.Count <= 0)
            return;

            string LoaiTamUng = dsPatientCashAdvance1.spPatientCashAdvance_GetByID.Rows[0]["PaymentReason"] as string;

            if (LoaiTamUng == "Thu Tiền Tạm Ứng")
            {
                xrLabel14.Visible = true;
                
            }


        }

        public void FillData()
        {
            spPatientCashAdvance_GetByIDTableAdapter.ClearBeforeFill = true;
            var paymentID = Parameters["param_PaymentID"].Value as int?;
            var FindPatient = Parameters["FindPatient"].Value as int?;
            if (!paymentID.HasValue || paymentID.Value <= 0)
                return;
            spPatientCashAdvance_GetByIDTableAdapter.Fill(dsPatientCashAdvance1.spPatientCashAdvance_GetByID, paymentID, FindPatient);
            if (dsPatientCashAdvance1.spPatientCashAdvance_GetByID == null || dsPatientCashAdvance1.spPatientCashAdvance_GetByID.Rows.Count <= 0)
                return;
            var total = dsPatientCashAdvance1.spPatientCashAdvance_GetByID.Rows[0]["PaymentAmount"] as decimal?;
            if (total.HasValue)
            {
                var cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                var converter = new NumberToLetterConverter();

                Parameters["param_AmountInWords"].Value = converter.Convert(total.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }

           


        }
    }
}
