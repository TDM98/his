using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardVTYTTHMedDeptFromClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardVTYTTHMedDeptFromClinicDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spRpt_InwardDrugVTYTTH_ByIDTableAdapter.Fill(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTH_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardDrugVTYTTHInvoice_ByIDTableAdapter.Fill(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTHInvoice_ByID, Convert.ToInt64(InvID.Value));

            decimal total = 0;
            if (dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTH_ByID != null && dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTH_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTH_ByID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsInwardVTYTTHSupplier1.spRpt_InwardDrugVTYTTH_ByID.Rows[i]["TotalPriceNotVAT"]);
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

        private void XRptInwardVTYTTHMedDeptFromClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
