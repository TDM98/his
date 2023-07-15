using System;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
/*
* 20181103 #001 TNHX: [BM0005214] Update report PhieuNhanThuoc base RefApplicationConfig.MixedHIPharmacyStores (don't print report if Drug has OutHIAllowedPrice == 0)
*/
namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class PhieuNhanThuocBH : XtraReport
    {
        public PhieuNhanThuocBH()
        {
            InitializeComponent();
        }

        public void FillData(System.Drawing.Printing.PrintEventArgs e)
        {
            dsPhieuNhanThuoc1.EnforceConstraints = false;
            spRpt_PhieuNhanThuoc_InfoTableAdapter.Fill((DataSource as DataSchema.dsPhieuNhanThuoc).spRpt_PhieuNhanThuoc_Info, Convert.ToInt64(OutiID.Value));
            spRpt_PhieuNhanThuoc_DetailsTableAdapter.Fill((DataSource as DataSchema.dsPhieuNhanThuoc).spRpt_PhieuNhanThuoc_Details, Convert.ToInt64(OutiID.Value));

            decimal totalInvoicePrice = 0;
            decimal totalHIPayment = 0;
            decimal totalPatientPayment = 0;

            if (dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details == null || dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details.Rows.Count == 0)
            {
                e.Cancel = true;
                return;
            }
            if (dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details != null && dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details.Rows.Count >= 1
                && Convert.ToDecimal(dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details.Rows[0]["OutHIAllowedPrice"]) == 0)
            {
                e.Cancel = true;
            }
            else
            {
                if (dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details != null && dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details.Rows.Count > 0)
                {
                    foreach (DataSchema.dsPhieuNhanThuoc.spRpt_PhieuNhanThuoc_DetailsRow item in dsPhieuNhanThuoc1.spRpt_PhieuNhanThuoc_Details.Rows)
                    {
                        totalInvoicePrice += Convert.ToDecimal(item.Amount);
                        totalHIPayment += Convert.ToDecimal(item.OutHIRebate);
                    }

                    totalInvoicePrice = Math.Round(totalInvoicePrice, MidpointRounding.AwayFromZero);
                    totalHIPayment = Math.Round(totalHIPayment, MidpointRounding.AwayFromZero);
                    totalPatientPayment = totalInvoicePrice - totalHIPayment;

                    Parameters["TotalInvoicePrice"].Value = totalInvoicePrice;
                    Parameters["TotalHIPayment"].Value = totalHIPayment;
                    Parameters["TotalPatientPayment"].Value = totalPatientPayment;

                    NumberToLetterConverter converter = new NumberToLetterConverter();
                    decimal temp = 0;
                    string prefix = "";
                    if (totalPatientPayment < 0)
                    {
                        //KMx: Ở trên làm tròn rồi (22/08/2014 12:03).
                        //temp = 0 - Math.Round(totalPatientPayment, 0);
                        temp = 0 - totalPatientPayment;
                        prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
                    }
                    else
                    {
                        //KMx: Ở trên làm tròn rồi (22/08/2014 12:03).
                        //temp = Math.Round(totalPatientPayment, 0);
                        temp = totalPatientPayment;
                        prefix = "";
                    }
                    Parameters["ReadMoney"].Value = string.Format("{0} : ", eHCMSResources.R0823_G1_Vietthanhchu) + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}", eHCMSResources.Z0872_G1_Dong.ToLower());
                }
            }
        }

        private void PhieuNhanThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData(e);
        }
    }
}
