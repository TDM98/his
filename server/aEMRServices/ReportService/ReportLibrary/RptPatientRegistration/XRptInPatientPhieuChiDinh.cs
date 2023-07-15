using System;
using System.Data;
using AxLogging;
using eHCMS.Services.Core;
using eHCMSLanguage;
/*
 * 20200106 #001 TNHX: Init
 */
namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptInPatientPhieuChiDinh : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInPatientPhieuChiDinh()
        {
            AxLogger.Instance.LogInfo("XRptInPatientReceipt HAM KHOI TAO");
            InitializeComponent();
            PrintingSystem.ShowPrintStatusDialog = false;
        }

        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInPatientPhieuChiDinh1.EnforceConstraints = false;
            sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentIDTableAdapter.ClearBeforeFill = true;
            sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentIDTableAdapter.Fill(dsInPatientPhieuChiDinh1.sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentID, Convert.ToInt64(param_DeptLocID.Value), param_ItemType.Value.ToString(), param_ListID.Value.ToString());
            if (dsInPatientPhieuChiDinh1.sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentID.Rows.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            decimal totalBN = 0;
            string quyenLoiAndDoiTuong = "";
            if (dsInPatientPhieuChiDinh1.sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentID != null && dsInPatientPhieuChiDinh1.sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentID.Rows.Count > 0)
            {
                foreach (DataRow row in dsInPatientPhieuChiDinh1.sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentID.Rows)
                {
                    totalBN += (decimal)row["PatientAmount"];
                }
                string HICode = dsInPatientPhieuChiDinh1.sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentID.Rows[0]["HICardNo"].ToString();
                if (!string.IsNullOrEmpty(HICode))
                {
                    quyenLoiAndDoiTuong = HICode.Substring(2, 1) + " - " + (Convert.ToDecimal(dsInPatientPhieuChiDinh1.sp_Rpt_spReportInPatientPhieuChiDinh_ByPaymentID.Rows[0]["HIBenefit"].ToString()) * 100).ToString("N0") + "%";
                    Parameters["parHIDate"].Value = " - ";
                }
            }
            Parameters["parBHYTString"].Value = quyenLoiAndDoiTuong;
        }
    }
}
