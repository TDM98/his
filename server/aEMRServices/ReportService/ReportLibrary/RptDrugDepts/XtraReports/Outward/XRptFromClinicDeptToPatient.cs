using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward
{
    public partial class XRptFromClinicDeptToPatient : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptFromClinicDeptToPatient()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spOutwardDrugClinicDept_ByOutIDTableAdapter.Fill(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID, Convert.ToInt64(this.OutiID.Value), Convert.ToInt64(this.V_MedProductType.Value));
            spOutwardDrugClinicDeptInvoices_GetTableAdapter.Fill(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDeptInvoices_Get, Convert.ToInt64(this.OutiID.Value), Convert.ToInt64(this.V_MedProductType.Value));

            decimal totalInvoicePrice = 0;
            decimal totalHIPayment = 0;
            decimal totalPatientPayment = 0;

            //if (dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID != null && dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows.Count; i++)
            //    {
            //        totalPatientPayment += Convert.ToDecimal(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows[i]["OutAmount"]) - Convert.ToDecimal(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows[i]["OutHIRebate"]);
            //    }
            //}

            //totalPatientPayment = Math.Round(totalPatientPayment, 0);

            if (dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID != null && dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows.Count; i++)
                {
                    totalInvoicePrice += Convert.ToDecimal(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows[i]["OutAmount"]);
                    totalHIPayment += Convert.ToDecimal(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDept_ByOutID.Rows[i]["OutHIRebate"]);
                }

                totalInvoicePrice = Math.Round(totalInvoicePrice, MidpointRounding.AwayFromZero);
                totalHIPayment = Math.Round(totalHIPayment, MidpointRounding.AwayFromZero);
                totalPatientPayment = totalInvoicePrice - totalHIPayment;

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
