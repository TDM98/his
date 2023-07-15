using System;
using AxLogging;

namespace eHCMS.ReportLib.InPatient.Reports
{
    public partial class XrptDischargePaper_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XrptDischargePaper_TV()
        {
            AxLogger.Instance.LogInfo("XRptOutPatientReceipt HAM KHOI TAO");
            InitializeComponent();
            PrintingSystem.ShowPrintStatusDialog = false;
        }

        void XrptDischargePaper_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsDischargePaper1.EnforceConstraints = false;
            spRpt_GetInPatientAdmDisDetailsByIDTableAdapter.ClearBeforeFill = true;
            spRpt_GetInPatientAdmDisDetailsByIDTableAdapter.Fill(dsDischargePaper1.spRpt_GetInPatientAdmDisDetailsByID, Convert.ToInt64(InPatientAdmDisDetailID.Value.ToString()));

            //switch (parHospitalCode.Value.ToString())
            //{
            //    case "95078":

            //        xrLabel22.Text = "SỞ Y TẾ TỈNH BẠC LIÊU";
            //        xrLabel41.Text = "BVĐK THANH VŨ MEDIC BẠC LIÊU";
            //        xrLabel23.Text = "02DN Đường Tránh QL1A,K.1,P.7,TP.Bạc Liêu";
            //        break;

            //    case "95076":
            //        xrLabel22.Text = "SỞ Y TẾ TỈNH BẠC LIÊU";
            //        xrLabel41.Text = "BVĐK THANH VŨ MEDIC";
            //        xrLabel23.Text = "183 Bà Triệu, Phường 3, TP.Bạc Liêu";
            //        break;

            //    default:
            //        xrLabel22.Text = "SỞ Y TẾ TỈNH CÀ MAU";
            //        xrLabel41.Text = "PKĐK THANH VŨ MEDIC CÀ MAU";
            //        xrLabel23.Text = "187 Nguyễn Tất Thành, P.8, TP.Cà Mau";
            //        break;
            //}
        }
    }
}
