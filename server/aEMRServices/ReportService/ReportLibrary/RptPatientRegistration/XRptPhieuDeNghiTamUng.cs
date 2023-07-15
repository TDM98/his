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
    public sealed partial class XRptPhieuDeNghiTamUng : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPhieuDeNghiTamUng()
        {
            InitializeComponent();

            BeforePrint += XRptPhieuDeNghiTamUng_BeforePrint;
            PrintingSystem.ShowPrintStatusDialog = false;
        }


        void XRptPhieuDeNghiTamUng_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            spRptPatientCashAdvReminder_ByIDTableAdapter.ClearBeforeFill = true;
            spRptPatientCashAdvReminder_ByIDTableAdapter.Fill(dsPhieuDeNghiTamUng1.spRptPatientCashAdvReminder_ByID, Convert.ToInt64(param_RptPtCashAdvRemID.Value));
            if (dsPhieuDeNghiTamUng1.spRptPatientCashAdvReminder_ByID == null || dsPhieuDeNghiTamUng1.spRptPatientCashAdvReminder_ByID.Rows.Count <= 0)
                return;
            var total = dsPhieuDeNghiTamUng1.spRptPatientCashAdvReminder_ByID.Rows[0]["RemAmount"] as decimal?;
            if (total.HasValue)
            {
                var cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                var converter = new NumberToLetterConverter();
                Parameters["param_AmountInWords"].Value = converter.Convert(total.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }
    }
}
