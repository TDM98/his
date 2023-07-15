using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards
{
    public partial class XRptInwardFromMedDeptForClinicDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptInwardFromMedDeptForClinicDept()
        {
            InitializeComponent();
            FillDataInit();
        }
        public void FillDataInit()
        {
            spRpt_InwardDrugClinicDeptDetails_ByIDTableAdapter.Fill(dsInwardDrugClinicDept1.spRpt_InwardDrugClinicDeptDetails_ByID, 0, Convert.ToInt64(V_MedProductType.Value));
            spRpt_InwardDrugClinicDeptInvoice_ByIDTableAdapter.Fill(dsInwardDrugClinicDept1.spRpt_InwardDrugClinicDeptInvoice_ByID, 0, Convert.ToInt64(V_MedProductType.Value));
        }
        public void FillData()
        {
            spRpt_InwardDrugClinicDeptDetails_ByIDTableAdapter.Fill(dsInwardDrugClinicDept1.spRpt_InwardDrugClinicDeptDetails_ByID, Convert.ToInt64(this.InvID.Value), Convert.ToInt64(V_MedProductType.Value));
            spRpt_InwardDrugClinicDeptInvoice_ByIDTableAdapter.Fill(dsInwardDrugClinicDept1.spRpt_InwardDrugClinicDeptInvoice_ByID, Convert.ToInt64(this.InvID.Value), Convert.ToInt64(V_MedProductType.Value));
            decimal total = 0;

            if (dsInwardDrugClinicDept1.spRpt_InwardDrugClinicDeptDetails_ByID != null && dsInwardDrugClinicDept1.spRpt_InwardDrugClinicDeptDetails_ByID.Rows.Count > 0)
            {
                for (int i = 0; i < dsInwardDrugClinicDept1.spRpt_InwardDrugClinicDeptDetails_ByID.Rows.Count; i ++)
                {
                    total += Convert.ToDecimal(dsInwardDrugClinicDept1.spRpt_InwardDrugClinicDeptDetails_ByID.Rows[i]["TotalPriceNotVAT"]) ;
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
            this.Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
        }

        private void XRptInwardFromMedDeptForClinicDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}