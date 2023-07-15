using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.OutwardFromPrescription
{
    public partial class XRptOutwardFromPrescription : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutwardFromPrescription()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spRpt_GetOutwardDrugInvoice_InPtTableAdapter.Fill(dsOutwardFromMedDeptToPatient1.spRpt_GetOutwardDrugInvoice_InPt, Convert.ToInt64(this.OutiID.Value));
            spRpt_GetOutwardDrug_ByIDInvoice_InPtTableAdapter.Fill(dsOutwardFromMedDeptToPatient1.spRpt_GetOutwardDrug_ByIDInvoice_InPt, Convert.ToInt64(this.OutiID.Value));

            decimal totalInvoicePrice = 0;
            decimal totalHIPayment = 0;
            decimal totalPatientPayment = 0;

            if (dsOutwardFromMedDeptToPatient1.spRpt_GetOutwardDrug_ByIDInvoice_InPt != null && dsOutwardFromMedDeptToPatient1.spRpt_GetOutwardDrug_ByIDInvoice_InPt.Rows.Count > 0)
            {
                for (int i = 0; i < dsOutwardFromMedDeptToPatient1.spRpt_GetOutwardDrug_ByIDInvoice_InPt.Rows.Count; i++)
                {
                    totalInvoicePrice += Convert.ToDecimal(dsOutwardFromMedDeptToPatient1.spRpt_GetOutwardDrug_ByIDInvoice_InPt.Rows[i]["OutAmount"]);
                    totalHIPayment += Convert.ToDecimal(dsOutwardFromMedDeptToPatient1.spRpt_GetOutwardDrug_ByIDInvoice_InPt.Rows[i]["OutHIRebate"]);
                }

                totalInvoicePrice = Math.Round(totalInvoicePrice, MidpointRounding.AwayFromZero);
                totalHIPayment = Math.Round(totalHIPayment, MidpointRounding.AwayFromZero);
                totalPatientPayment = totalInvoicePrice - totalHIPayment;

                this.Parameters["TotalInvoicePrice"].Value = totalInvoicePrice;
                this.Parameters["TotalHIPayment"].Value = totalHIPayment;
                this.Parameters["TotalPatientPayment"].Value = totalPatientPayment;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (totalPatientPayment < 0)
                {
                    temp = 0 - totalPatientPayment;
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = totalPatientPayment;
                    prefix = "";
                }
                this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());

            }

        }

        private void XRptFromClinicDeptToPatient_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
