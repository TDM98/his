using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBaoCaoThuVienPhiBHYT : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoThuVienPhiBHYT()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBaoCaoThuVienPhiBHYT_BeforePhint);
        }

        public void FillData()
        {
            dsBaoCaoThuVienPhiBHYT.EnforceConstraints = false;
            spReportPaymentReceiptByStaffDetailsTableAdapter.Fill(dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails
                                                                , Convert.ToInt32(RepPaymentRecvID.Value)
                                                                , Convert.ToInt32(StaffID.Value)
                                                                , Convert.ToDateTime(FromDate.Value)
                                                                , Convert.ToDateTime(ToDate.Value)
                                                                , Convert.ToInt32(Case.Value)
                                                                , Convert.ToInt32(V_PaymentMode.Value)); //--26/01/2021 DatTB
        }

        private void XRptBaoCaoThuVienPhiBHYT_BeforePhint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            decimal TotalAmount = 0;

            if (dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails != null && dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows.Count > 0)
            {

                for (int i = 0; i < dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows.Count; i++)
                {
                    TotalAmount += ((Convert.ToDecimal(dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows[i]["PayAmount"]))
                                     - (Convert.ToDecimal(dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows[i]["CancelAmount"]))
                                     - (Convert.ToDecimal(dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows[i]["RefundAmount"])));
                }
                Parameters["TotalAmount"].Value = TotalAmount;
                Parameters["parReadMoneyTotalAmount"].Value = Globals.ReadMoneyToString(TotalAmount);
            }
        }
    }
}
