using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation
{
    public partial class XRptEstimateDrugDeptFromRequest : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEstimateDrugDeptFromRequest()
        {
            InitializeComponent();
            PrintingSystem.PageSettings.Landscape = true;
        }

        public void FillData()
        {
            dsEstimationDrugDept_V21.EnforceConstraints = false;
            spDrugDeptEstimationForPO_ByIDTableAdapter1.Fill(dsEstimationDrugDept_V21.spDrugDeptEstimationForPO_ByID
                                                                        , Convert.ToInt64(DrugDeptEstimatePoID.Value)
                                                                        , Convert.ToInt64(EstimationCodeBegin.Value)
                                                                        , Convert.ToInt64(EstimationCodeEnd.Value)
                                                                        , Convert.ToInt64(V_MedProductType.Value));
            spDrugDeptEstimationForPoDetails_ByIDParent_V2TableAdapter.Fill(dsEstimationDrugDept_V21.spDrugDeptEstimationForPoDetails_ByIDParent_V2
                                                                        , Convert.ToInt64(DrugDeptEstimatePoID.Value)
                                                                        , Convert.ToInt64(EstimationCodeBegin.Value)
                                                                        , Convert.ToInt64(EstimationCodeEnd.Value)
                                                                        , Convert.ToInt64(V_MedProductType.Value)
                                                                        , Convert.ToBoolean(EstimationFromRequest.Value));

            decimal TotalAmount = 0;

            if (dsEstimationDrugDept_V21.spDrugDeptEstimationForPoDetails_ByIDParent_V2 != null && dsEstimationDrugDept_V21.spDrugDeptEstimationForPoDetails_ByIDParent_V2.Rows.Count > 0)
            {

                for (int i = 0; i < dsEstimationDrugDept_V21.spDrugDeptEstimationForPoDetails_ByIDParent_V2.Rows.Count; i++)
                {
                    TotalAmount += Convert.ToDecimal(dsEstimationDrugDept_V21.spDrugDeptEstimationForPoDetails_ByIDParent_V2.Rows[i]["AdjustedQty"]) * Convert.ToDecimal(dsEstimationDrugDept_V21.spDrugDeptEstimationForPoDetails_ByIDParent_V2.Rows[i]["UnitPrice"]);
                }

                TotalAmount = Math.Round(TotalAmount, MidpointRounding.AwayFromZero);

                Parameters["TotalAmount"].Value = TotalAmount;


                System.Globalization.CultureInfo cutureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

                NumberToLetterConverter converter = new NumberToLetterConverter();
                decimal temp1 = 0;
                string prefix1 = "";
                if (TotalAmount < 0)
                {
                    temp1 = 0 - TotalAmount;
                    prefix1 = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am);
                }
                else
                {
                    temp1 = TotalAmount;
                    prefix1 = "";
                }
                Parameters["ReadMoneyTotalAmount"].Value = prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }

        private void XRptEstimateDrugDeptFromRequest_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            PrintingSystem.PageSettings.Landscape = true;
            FillData();
        }
    }
}
