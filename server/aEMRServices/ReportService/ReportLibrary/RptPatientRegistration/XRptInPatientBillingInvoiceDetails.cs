using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.ReportLib.RptPatientRegistration.DataSchema;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptInPatientBillingInvoiceDetails : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInPatientBillingInvoiceDetails()
        {
            InitializeComponent();

            this.PrintingSystem.ShowPrintStatusDialog = false;
        }

        public void FillData()
        {
            lblBHYT_CardNo.Visible = false;
            lblBHYT.Visible =  false;

            spGetBillingInvoiceDetailList_RptTableAdapter.ClearBeforeFill = true;
            spGetBillingInvoiceInfo_RptTableAdapter.ClearBeforeFill = true;
            
            int? ID = this.Parameters["param_InPatientBillingInvID"].Value as int?;
            if (ID.HasValue && ID.Value > 0)
            {
                spGetBillingInvoiceDetailList_RptTableAdapter.Fill(dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceDetailList_Rpt, ID);
                sp_GetLiabilityUpToBillingInvoiceTableAdapter.Fill(dsInPatientBillingInvoiceDetailList1.sp_GetLiabilityUpToBillingInvoice, ID);
                spGetBillingInvoiceInfo_RptTableAdapter.Fill(dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceInfo_Rpt, ID);
                
                if (dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceInfo_Rpt != null
                    && dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceInfo_Rpt.Rows.Count > 0)
                {
                    //
                    long? classID = dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceInfo_Rpt.Rows[0]["PatientClassID"] as long?;

                    if(classID.HasValue)
                    {
                        if(classID.Value == 2)//Benh nhan Bao hiem
                        {
                            double? benefit = dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceInfo_Rpt.Rows[0]["PtInsuranceBenefit"] as double?;
                            string strBenefit = "0%";
                            if(benefit.HasValue && benefit.Value > 0)
                            {
                                strBenefit = string.Format("{0}%", benefit.Value * 100);
                            }
                            string hiCardInfo = string.Format("{0} ({1})", dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceInfo_Rpt.Rows[0]["HICardNo"]
                                                                         , strBenefit);
                            this.Parameters["param_HICardInfo"].Value = hiCardInfo;
                            lblBHYT_CardNo.Visible = true;
                            lblBHYT.Visible = true;
                        }
                    }

                    if(dsInPatientBillingInvoiceDetailList1.sp_GetLiabilityUpToBillingInvoice != null
                        && dsInPatientBillingInvoiceDetailList1.sp_GetLiabilityUpToBillingInvoice.Count > 0)
                    {
                        decimal totalPatientPayment = (dsInPatientBillingInvoiceDetailList1.sp_GetLiabilityUpToBillingInvoice.Rows[0]["TotalPatientPayment"] as decimal?).GetValueOrDefault(0);
                        decimal totalPatientPaid = (dsInPatientBillingInvoiceDetailList1.sp_GetLiabilityUpToBillingInvoice.Rows[0]["TotalPatientPaid"] as decimal?).GetValueOrDefault(0);
                        _liability = totalPatientPayment - totalPatientPaid;
                        if(_liability < 0)
                        {
                            lblLiability.Text = string.Format("{0}:",  eHCMSResources.R0055_G1_BNTraDu);
                            this.Parameters["param_Liability"].Value = - _liability;
                        }
                        else
                        {
                            lblLiability.Text = string.Format("{0}:",  eHCMSResources.R0481_G1_Notruoc);
                            this.Parameters["param_Liability"].Value = _liability;
                        }
                    }
                }
                decimal totalOfCurrentBill = 0;
                if(dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceDetailList_Rpt != null)
                {
                    foreach (DataRow row in dsInPatientBillingInvoiceDetailList1.spGetBillingInvoiceDetailList_Rpt.Rows)
                    {
                        totalOfCurrentBill += (row["TotalPatientPayment"] as decimal?).GetValueOrDefault(0);
                    }
                }

                decimal total = totalOfCurrentBill + _liability;
                if (total > 0)
                {
                    lblTotalPaid.Text = string.Format("{0}:",  eHCMSResources.R0052_G1_BNPhaiTra);
                    this.Parameters["param_ShouldPay"].Value = total;
                }
                else
                {
                    lblTotalPaid.Text = string.Format("{0}:",  eHCMSResources.R0052_G1_BNPhaiTra);
                    this.Parameters["param_ShouldPay"].Value = 0;
                }
            }
        }

        private void XRptInPatientBillingInvoiceDetails_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            
        }
        private decimal _liability;

    }
}
