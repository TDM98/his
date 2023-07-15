using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class XRptEstimatePharmacy : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEstimatePharmacy()
        {
            InitializeComponent();
        }
        public void FillData()
        {
            dsEstimatePharmacy1.EnforceConstraints = false;
            spPharmacyEstimationForPO_ByIDTableAdapter.Fill((this.DataSource as DataSchema.dsEstimatePharmacy).spPharmacyEstimationForPO_ByID,Convert.ToInt64(this.PharmacyEstimatePoID.Value));
            spPharmacyEstimationForPoDetails_ByIDParentTableAdapter.Fill((this.DataSource as DataSchema.dsEstimatePharmacy).spPharmacyEstimationForPoDetails_ByIDParent
                                                                        , Convert.ToInt64(this.PharmacyEstimatePoID.Value)
                                                                        , Convert.ToInt64(this.SupplierID.Value)
                                                                        , Convert.ToInt64(this.PCOID.Value));

            decimal TotalAmount = 0;

            if (dsEstimatePharmacy1.spPharmacyEstimationForPoDetails_ByIDParent != null && dsEstimatePharmacy1.spPharmacyEstimationForPoDetails_ByIDParent.Rows.Count > 0)
            {

                for (int i = 0; i < dsEstimatePharmacy1.spPharmacyEstimationForPoDetails_ByIDParent.Rows.Count; i++)
                {
                    TotalAmount += Convert.ToDecimal(dsEstimatePharmacy1.spPharmacyEstimationForPoDetails_ByIDParent.Rows[i]["AdjustedQty"]) * Convert.ToDecimal(dsEstimatePharmacy1.spPharmacyEstimationForPoDetails_ByIDParent.Rows[i]["UnitPrice"]);
                }

                TotalAmount = Math.Round(TotalAmount, MidpointRounding.AwayFromZero);

                this.Parameters["TotalAmount"].Value = TotalAmount;


                System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp1 = 0;
                string prefix1 = "";
                if (TotalAmount < 0)
                {
                    temp1 = 0 - TotalAmount;
                    prefix1 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am);
                }
                else
                {
                    temp1 = TotalAmount;
                    prefix1 = "";
                }
                this.Parameters["ReadMoneyTotalAmount"].Value = prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }

        }

        private void XRptEstimatePharmacy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
