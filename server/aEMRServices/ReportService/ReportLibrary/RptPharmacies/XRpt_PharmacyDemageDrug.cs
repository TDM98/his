using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRpt_PharmacyDemageDrug : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PharmacyDemageDrug()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            spOutwardDrugInvoices_ByIDVisitorTableAdapter.Fill((this.DataSource as DataSchema.dsXuatNoiBo).spOutwardDrugInvoices_ByIDVisitor, Convert.ToInt64(this.OutiID.Value));
            spOutwardDrug_ByIDInvoiceTableAdapter.Fill((this.DataSource as DataSchema.dsXuatNoiBo).spOutwardDrug_ByIDInvoice, Convert.ToInt64(this.OutiID.Value));
            decimal total = 0;
            if (dsXuatNoiBo1.spOutwardDrug_ByIDInvoice != null && dsXuatNoiBo1.spOutwardDrug_ByIDInvoice.Rows.Count > 0)
            {
                for (int i = 0; i < dsXuatNoiBo1.spOutwardDrug_ByIDInvoice.Rows.Count; i++)
                {
                    total += Convert.ToDecimal(dsXuatNoiBo1.spOutwardDrug_ByIDInvoice.Rows[i]["OutQuantity"]) * Convert.ToDecimal(dsXuatNoiBo1.spOutwardDrug_ByIDInvoice.Rows[i]["InCost"]);
                }
                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (total < 0)
                {
                    temp = 0 - Math.Round(total, 0);
                    prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
                }
                else
                {
                    temp = Math.Round(total, 0);
                    prefix = "";
                }
                this.Parameters["ReadMoney"].Value = string.Format("{0} : ",  eHCMSResources.G2256_G1_VietBangChu)+prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }
            
        }

        private void XRpt_PharmacyDemageDrug_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            
        }
    }
}
