using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardChemicalMedDeptFromClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardChemicalMedDeptFromClinicDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardChemical_ByIDTableAdapter.Fill(dsInwardChemicalSupplier1.spRpt_InwardChemical_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardChemicalInvoice_ByIDTableAdapter.Fill(dsInwardChemicalSupplier1.spRpt_InwardChemicalInvoice_ByID, Convert.ToInt64(InvID.Value));

            decimal total = 0;
            if (dsInwardChemicalSupplier1.spRpt_InwardChemical_ByID != null && dsInwardChemicalSupplier1.spRpt_InwardChemical_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardChemicalSupplier1.spRpt_InwardChemical_ByID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsInwardChemicalSupplier1.spRpt_InwardChemical_ByID.Rows[i]["TotalPriceNotVAT"]);
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

        private void XRptInwardChemicalMedDeptFromClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
