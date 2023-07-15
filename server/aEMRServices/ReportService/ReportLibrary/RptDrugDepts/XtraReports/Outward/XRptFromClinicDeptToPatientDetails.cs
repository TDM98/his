using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward
{
    public partial class XRptFromClinicDeptToPatientDetails : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptFromClinicDeptToPatientDetails()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spOutwardDrugClinicDeptToPatient_ByOutIDTableAdapter.Fill(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDeptToPatient_ByOutID, Convert.ToInt64(OutiID.Value));
            spOutwardDrugClinicDeptInvoices_GetTableAdapter.Fill(dsXuatTuKhoNoiTru1.spOutwardDrugClinicDeptInvoices_Get, Convert.ToInt64(OutiID.Value), 0);

            decimal totalInvoicePrice = 0;
            decimal totalHIPayment = 0;
            decimal totalPatientPayment = 0;

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

                Parameters["TotalPatientPayment"].Value = totalPatientPayment;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (totalPatientPayment < 0)
                {
                    temp = 0 - totalPatientPayment;
                    prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = totalPatientPayment;
                    prefix = "";
                }
                Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }

        private void XRXRptFromClinicDeptToPatientDetails_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
