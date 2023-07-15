using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal
{
    public partial class XRptOutInternalDrugDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutInternalDrugDept()
        {
            InitializeComponent();
        }

        public void FillData()
        {
            spOutwardDrugMedDept_ByOutIDTableAdapter.Fill(dsOutwardInternal1.spOutwardDrugMedDept_ByOutID, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value), null);
            spOutwardDrugMedDeptInvoices_GetTableAdapter.Fill(dsOutwardInternal1.spOutwardDrugMedDeptInvoices_Get, Convert.ToInt64(OutiID.Value), Convert.ToInt64(V_MedProductType.Value));

            decimal VAT = 0;
            if (dsOutwardInternal1.spOutwardDrugMedDeptInvoices_Get != null && dsOutwardInternal1.spOutwardDrugMedDeptInvoices_Get.Rows.Count > 0)
            {
                VAT = Convert.ToDecimal(dsOutwardInternal1.spOutwardDrugMedDeptInvoices_Get.Rows[0]["VAT"]);
            }

            decimal totalNotVAT = 0;
            if (dsOutwardInternal1.spOutwardDrugMedDept_ByOutID != null && dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows.Count > 0)
            {
                for (int i = 0; i < dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows.Count; i++)
                {
                    totalNotVAT += Convert.ToDecimal(dsOutwardInternal1.spOutwardDrugMedDept_ByOutID.Rows[i]["OutAmount"]);
                }
            }

            totalNotVAT = Math.Round((totalNotVAT), 0);

            decimal total = Math.Round((totalNotVAT * VAT), 0);

            decimal VATCash = total - totalNotVAT;


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

            Parameters["StrVAT"].Value = ((VAT - 1) * 100).ToString("#,#.##") + "% VAT";
            Parameters["VATCash"].Value = VATCash;
            Parameters["TotalPriceNotVAT"].Value = totalNotVAT;
            Parameters["TotalPrice"].Value = total;
            Parameters["ReadMoney"].Value = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }

        private void XRptOutInternalDrugDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
