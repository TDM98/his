using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardMedDeptFromClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardMedDeptFromClinicDept()
        {
            InitializeComponent();
            //xem lai dung chung nhu the nao?
            // Thread.CurrentThread.CurrentCulture = new CultureInfo("vi-VN",false);
           // CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        public void FillData()
        {
            this.dsInwardMedDeptSupplier1.EnforceConstraints = false;
            spRpt_InwardDrugMedDept_ByIDTableAdapter.Fill(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID, Convert.ToInt64(InvID.Value));
            spRpt_InwardDrugMedDeptInvoice_ByIDTableAdapter.Fill(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDeptInvoice_ByID, Convert.ToInt64(InvID.Value));

            decimal total = 0;
            if (dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID != null && dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsInwardMedDeptSupplier1.spRpt_InwardDrugMedDept_ByID.Rows[i]["TotalPriceNotVAT"]);
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

        private void XRptInwardMedDeptFromClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
