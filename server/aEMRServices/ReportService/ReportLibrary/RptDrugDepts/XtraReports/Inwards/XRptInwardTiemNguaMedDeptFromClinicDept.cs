using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardTiemNguaMedDeptFromClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardTiemNguaMedDeptFromClinicDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardVaccine_ByIDTableAdapter.Fill(dsInwardVaccineSupplier1.spRpt_InwardVaccine_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardVaccineInvoice_ByIDTableAdapter.Fill(dsInwardVaccineSupplier1.spRpt_InwardVaccineInvoice_ByID, Convert.ToInt64(InvID.Value));

            decimal total = 0;
            if (dsInwardVaccineSupplier1.spRpt_InwardVaccine_ByID != null && dsInwardVaccineSupplier1.spRpt_InwardVaccine_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardVaccineSupplier1.spRpt_InwardVaccine_ByID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsInwardVaccineSupplier1.spRpt_InwardVaccine_ByID.Rows[i]["TotalPriceNotVAT"]);
                }
            }

            total = Math.Round(total, 0);

            NumberToLetterConverter converter = new NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (total < 0)
            {
                temp = 0 - total;
                prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                temp = total;
                prefix = "";
            }
            Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

        private void XRptInwardTiemNguaMedDeptFromClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
