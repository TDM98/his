using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.Order
{
    public partial class XRpt_PurchaseOrderPharmacy : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PurchaseOrderPharmacy()
        {
            InitializeComponent();
        }
     
        public void FillData()
        {
            spPharmacyPurchaseOrderDetails_ByIDParentTableAdapter.Fill((this.DataSource as DataSchema.Order.dsOrderPharmacy).spPharmacyPurchaseOrderDetails_ByIDParent,
                Convert.ToInt64(this.ID.Value), 0);
            spPharmacyPurchaseOrders_ByIDTableAdapter.Fill((this.DataSource as DataSchema.Order.dsOrderPharmacy).spPharmacyPurchaseOrders_ByID,
               Convert.ToInt64(this.ID.Value));

            decimal totalNotVAT = 0;
            decimal VAT = 0;
            decimal VATValue = 0;
            decimal total = 0;
            if (dsOrderPharmacy1.spPharmacyPurchaseOrders_ByID != null && dsOrderPharmacy1.spPharmacyPurchaseOrders_ByID.Rows.Count > 0)
            {
                VAT = Convert.ToDecimal(dsOrderPharmacy1.spPharmacyPurchaseOrders_ByID.Rows[0]["VAT"]);
            }
            if (dsOrderPharmacy1.spPharmacyPurchaseOrderDetails_ByIDParent != null && dsOrderPharmacy1.spPharmacyPurchaseOrderDetails_ByIDParent.Rows.Count > 0)
            {
                for (int i = 0; i < dsOrderPharmacy1.spPharmacyPurchaseOrderDetails_ByIDParent.Rows.Count; i++)
                {
                    totalNotVAT += Convert.ToDecimal(dsOrderPharmacy1.spPharmacyPurchaseOrderDetails_ByIDParent.Rows[i]["UnitPrice"]) * Convert.ToDecimal(dsOrderPharmacy1.spPharmacyPurchaseOrderDetails_ByIDParent.Rows[i]["PoUnitQty"]);
                }
                if (VAT > 0)
                {
                    VATValue = (VAT - 1) * totalNotVAT;
                    this.Parameters["VATValue"].Value = VATValue;
                }
                total = totalNotVAT + VATValue;
                this.Parameters["TotalValue"].Value = total;

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
                this.Parameters["ReadMoney"].Value = string.Format("{0} : ",  eHCMSResources.G2256_G1_VietBangChu) + prefix + converter.Convert(Math.Round(temp, 0).ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }
        }
        private void XRpt_PurchaseOrderPharmacy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}