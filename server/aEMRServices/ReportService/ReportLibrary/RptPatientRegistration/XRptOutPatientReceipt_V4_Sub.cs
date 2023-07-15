using System;
using System.Data;
using System.Linq;
using AxLogging;
using eHCMS.Services.Core;
using eHCMSLanguage;
using DevExpress.XtraReports.UI;

/*
 * 20181026 TNHX #001: [0004198] Update report to show data from parameter which sent from client
 * 20181026 TNHX #002: [BM0002176] Change report from "Hoa don" to "Bien lai thu tien". Disable function "In Hoa Don"
 */
namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutPatientReceipt_V4_Sub : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceipt_V4_Sub()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceipt HAM KHOI TAO");
            InitializeComponent();
            PrintingSystem.ShowPrintStatusDialog = false;
        }

        private void XRptOutPatientReceipt_V4_Sub_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        public void FillData()
        {
            outPatientCashReceipt_Sub1.EnforceConstraints = false;
            spRpt_GetDetailtOrderOutPatientCashReceipt_ByOutPtCashAdvanceIDTableAdapter.ClearBeforeFill = true;
            spRpt_GetDetailtOrderOutPatientCashReceipt_ByOutPtCashAdvanceIDTableAdapter.Fill(
                outPatientCashReceipt_Sub1.spRpt_GetDetailtOrderOutPatientCashReceipt_ByOutPtCashAdvanceID, Convert.ToInt64(param_PaymentID.Value), Convert.ToInt64(pOutPtCashAdvanceID.Value));
        }
    }
}
