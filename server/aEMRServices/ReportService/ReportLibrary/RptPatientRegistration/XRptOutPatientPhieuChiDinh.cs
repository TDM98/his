using System;
using System.Data;
using AxLogging;
using eHCMS.Services.Core;
using eHCMSLanguage;
/*
 * 20181030 #001 TNHX: [BM0002176] Add params HospitalName, DepartmentOfHealth. Update report base on new flow (XacNhanQLBH)
 */
namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutPatientPhieuChiDinh : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientPhieuChiDinh()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceipt HAM KHOI TAO");
            InitializeComponent();
            PrintingSystem.ShowPrintStatusDialog = false;
        }

        void XRptPatientPayment_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            outPatientPhieuChiDinh.EnforceConstraints = false;
            sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDTableAdapter.ClearBeforeFill = true;
            sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDTableAdapter.Fill(outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID, Convert.ToInt64(param_DeptLocID.Value), param_ItemType.Value.ToString(), param_ListID.Value.ToString());
            if (outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID.Rows.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            decimal totalBN = 0;
            string quyenLoiAndDoiTuong = "";
            string HospitalCode = "";
            int LoaiKham = 0;
            if (outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID != null && outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID.Rows.Count > 0)
            {
                foreach (DataRow row in outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID.Rows)
                {
                    totalBN += (decimal)row["PatientAmount"];
                    if (row["ServiceSeqNum"].ToString() != "")
                    {
                        xrLabel17.Visible = true;
                        xrLabel13.Visible = true;
                        xrTableRow6.Visible = true;
                    }
                }
                string HICode = outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID.Rows[0]["HICardNo"].ToString();
                if (!string.IsNullOrEmpty(HICode))
                {
                    quyenLoiAndDoiTuong = HICode.Substring(2, 1) + " - " + (Convert.ToDecimal(outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID.Rows[0]["HIBenefit"].ToString()) * 100).ToString("N0") + "%";
                    Parameters["parHIDate"].Value = " - ";
                }
                HospitalCode = outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID.Rows[0]["HospitalCode"].ToString();
                LoaiKham = Convert.ToInt32(outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentID.Rows[0]["LoaiKham"]);
            }
            Parameters["parBHYTString"].Value = quyenLoiAndDoiTuong;

            if (HospitalCode == "95078")
            {
                xrRichText1.Visible = false;
                xrRichText2.Visible = true;
                xrRichText3.Visible = false;
            }
            else if (HospitalCode == "95076")
            {
                xrRichText1.Visible = true;
                xrRichText2.Visible = false;
                xrRichText3.Visible = false;
            }
            else
            {
                xrRichText1.Visible = false;
                xrRichText2.Visible = false;
                xrRichText3.Visible = true;
            }
            switch (LoaiKham)
            {
                case 1: //KhamNoi
                    xrRTKhamNoi.Visible = true;
                    xrRTKhamNoi.Text = "- Huyết áp: \n" +
                                       "- Triệu chứng/ khám chuyên khoa/ Yêu cầu BS: \n\n" +
                                       "- CN:                , CC: \n" +
                                       "- Mạch:            , Nhiệt độ:";
                    xrRTKhamNhi.Visible = false;
                    break;
                case 2: //KhamNhi
                    xrRTKhamNoi.Visible = false;
                    xrRTKhamNhi.Visible = true;
                    xrRTKhamNhi.Text = "- Triệu chứng/Yêu cầu BS:  \n\n" +
                                        "- CN:                , CC: \n" +
                                        "- Mạch:            , Nhiệt độ: \n" +
                                        "- Tháng tuổi:";
                    break;
                default:
                    xrRTKhamNoi.Visible = false;
                    xrRTKhamNhi.Visible = false;
                    break;
            }
        }
    }
}
