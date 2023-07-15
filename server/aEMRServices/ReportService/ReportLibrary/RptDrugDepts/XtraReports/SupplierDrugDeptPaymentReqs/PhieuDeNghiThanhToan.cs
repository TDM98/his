using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.SupplierDrugDeptPaymentReqs
{
    public partial class PhieuDeNghiThanhToan : DevExpress.XtraReports.UI.XtraReport
    {
        public PhieuDeNghiThanhToan()
        {
            InitializeComponent();
        }
       
        private void FillData()
        {
           spSupplierDrugDeptPaymentReqDetails_IDTableAdapter.Fill((this.DataSource as DataSchema.SupplierDrugDeptPaymentReqs.SupplierDrugDeptPaymentReqs).spSupplierDrugDeptPaymentReqDetails_ID,Convert.ToInt64(this.ID.Value));
           spSupplierDrugDeptPaymentReqs_IDTableAdapter.Fill((this.DataSource as DataSchema.SupplierDrugDeptPaymentReqs.SupplierDrugDeptPaymentReqs).spSupplierDrugDeptPaymentReqs_ID, Convert.ToInt64(this.ID.Value));
           decimal total = 0;
           if (supplierDrugDeptPaymentReqs1.spSupplierDrugDeptPaymentReqDetails_ID != null && supplierDrugDeptPaymentReqs1.spSupplierDrugDeptPaymentReqDetails_ID.Rows.Count > 0)
           {
               for (int i = 0; i < supplierDrugDeptPaymentReqs1.spSupplierDrugDeptPaymentReqDetails_ID.Rows.Count; i++)
               {
                   total += Convert.ToDecimal(supplierDrugDeptPaymentReqs1.spSupplierDrugDeptPaymentReqDetails_ID.Rows[i]["TotalPrice"]);
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
               this.Parameters["ReadMoney"].Value = "( " + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + " " + eHCMSResources.Z0872_G1_Dong.ToLower() + " )";
           }
        }
        private void SupplierDrugDeptPaymentReqs_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
