using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;
namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Returns
{
    public partial class XptReturnMedDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XptReturnMedDept()
        {
            InitializeComponent();
        }
       
        public void FillData()
        {
            spOutwardDrugMedDept_ByOutIDTableAdapter.Fill(dsReturnDrugDept1.spOutwardDrugMedDept_ByOutID, Convert.ToInt64(this.OutiID.Value));
            spRpt_ReturnMedDeptInvoiceTableAdapter.Fill(dsReturnDrugDept1.spRpt_ReturnMedDeptInvoice, Convert.ToInt64(this.OutiID.Value));
            decimal total = 0;
            if (dsReturnDrugDept1.spOutwardDrugMedDept_ByOutID != null && dsReturnDrugDept1.spOutwardDrugMedDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsReturnDrugDept1.spOutwardDrugMedDept_ByOutID.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsReturnDrugDept1.spOutwardDrugMedDept_ByOutID.Rows[i]["OutPrice"]) * Convert.ToDecimal(dsReturnDrugDept1.spOutwardDrugMedDept_ByOutID.Rows[i]["OutQuantity"]);
                }
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
                temp = Math.Round(temp,0);
                this.Parameters["ReadMoney"].Value = string.Format("{0} : ",  eHCMSResources.G2256_G1_VietBangChu) + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }

        }

        private void XptReturnMedDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
