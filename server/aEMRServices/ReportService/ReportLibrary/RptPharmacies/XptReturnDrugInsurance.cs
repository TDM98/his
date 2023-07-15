using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XptReturnDrugInsurance : DevExpress.XtraReports.UI.XtraReport
    {
        public XptReturnDrugInsurance()
        {
            InitializeComponent();
            
        }
       
        public void FillData()
        {
            spOutwardDrugInvoice_ReturnIDTableAdapter.Fill((this.DataSource as DataSchema.ReturnDrug).spOutwardDrugInvoice_ReturnID, Convert.ToInt64(this.OutiID.Value));
            spRpt_OutwardDrug_ReturnIDTableAdapter.Fill((this.DataSource as DataSchema.ReturnDrug).spRpt_OutwardDrug_ReturnID, Convert.ToInt64(this.OutiID.Value));

            decimal totalReturnInvoice = 0;
            decimal totalReturnHI = 0;
            decimal totalReturnPatient = 0;

            if (returnDrug1.spRpt_OutwardDrug_ReturnID != null && returnDrug1.spRpt_OutwardDrug_ReturnID.Rows.Count > 0)
            {
                //for (int i = 0; i < returnDrug1.spRpt_OutwardDrug_ReturnID.Rows.Count; i++)
                //{
                //    //total += Convert.ToDecimal(returnDrug1.spRpt_OutwardDrug_ReturnID.Rows[i]["OutPrice"]) * Convert.ToDecimal(returnDrug1.spRpt_OutwardDrug_ReturnID.Rows[i]["OutQuantity"]);
                //    totalReturnPatient += Convert.ToDecimal(returnDrug1.spRpt_OutwardDrug_ReturnID.Rows[i]["OutAmount"]) - Convert.ToDecimal(returnDrug1.spRpt_OutwardDrug_ReturnID.Rows[i]["OutHIRebate"]);
                //}

                for (int i = 0; i < returnDrug1.spRpt_OutwardDrug_ReturnID.Rows.Count; i++)
                {
                    totalReturnInvoice += Convert.ToDecimal(returnDrug1.spRpt_OutwardDrug_ReturnID.Rows[i]["OutAmount"]);
                    totalReturnHI += Convert.ToDecimal(returnDrug1.spRpt_OutwardDrug_ReturnID.Rows[i]["OutHIRebate"]);
                }
                totalReturnInvoice = Math.Round(totalReturnInvoice, MidpointRounding.AwayFromZero);
                totalReturnHI = Math.Round(totalReturnHI, MidpointRounding.AwayFromZero);
                totalReturnPatient = totalReturnInvoice - totalReturnHI;

                this.Parameters["TotalReturnHI"].Value = totalReturnHI;
                this.Parameters["TotalReturnPT"].Value = totalReturnPatient;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (totalReturnPatient < 0)
                {
                    temp = 0 - totalReturnPatient;
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = totalReturnPatient;
                    prefix = "";
                }

                this.Parameters["ReadMoney"].Value = string.Format("{0} : ",  eHCMSResources.R0823_G1_Vietthanhchu) + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }

        }

        private void XptReturnDrugInsurance_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
