using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Drawing.Printing;
using System.Data;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptReportCancelInvoice : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptReportCancelInvoice()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            dsPaymentReceiptByStaffDetails1.EnforceConstraints = false;
            spReportPaymentReceiptByStaff_HeaderTableAdapter.Fill(
                  dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaff_Header,
                  Convert.ToInt32(RepPaymentRecvID.Value));
            spReportPaymentReceiptByStaffDetails_ByIDTableAdapter.Fill(
                dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaffDetails_ByID,
                Convert.ToInt32(RepPaymentRecvID.Value));
            string strservices = "";
            decimal totalAmount = 0;
            if (dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaffDetails_ByID != null && dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaffDetails_ByID.Rows.Count > 0)
            {
                foreach (DataRow row in dsPaymentReceiptByStaffDetails1.spReportPaymentReceiptByStaffDetails_ByID.Rows)
                {
                    if (!string.IsNullOrEmpty(strservices))
                    {
                        strservices += " ; ";
                    }
                    strservices += row["ReceiptNumber"].ToString();
                    totalAmount += (decimal)row["PayAmount"];
                }

                Parameters["param_strservices"].Value = strservices;
            }

            totalAmount = Math.Round(totalAmount, 0);

            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (totalAmount < 0)
            {
                temp = 0 - totalAmount;
                prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = totalAmount;
                prefix = "";
            }

            this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

        private void XRptReportCancelInvoice_BeforePrint(object sender, PrintEventArgs e)
        {
            FillData();
        }


    }
}
