using System;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation
{
    public partial class XRptEstimateDrugDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEstimateDrugDept()
        {
            InitializeComponent();
            PrintingSystem.PageSettings.Landscape = true;
        }

        public void FillData()
        {
            dsEstimationDrugDept1.EnforceConstraints = false;
            spDrugDeptEstimationForPO_ByIDTableAdapter.Fill(dsEstimationDrugDept1.spDrugDeptEstimationForPO_ByID
                                                                        , Convert.ToInt64(DrugDeptEstimatePoID.Value)
                                                                        , Convert.ToInt64(EstimationCodeBegin.Value)
                                                                        , Convert.ToInt64(EstimationCodeEnd.Value)
                                                                        , Convert.ToInt64(V_MedProductType.Value));
            spDrugDeptEstimationForPoDetails_ByIDParentTableAdapter.Fill(dsEstimationDrugDept1.spDrugDeptEstimationForPoDetails_ByIDParent
                                                                        , Convert.ToInt64(DrugDeptEstimatePoID.Value)
                                                                        , Convert.ToInt64(EstimationCodeBegin.Value)
                                                                        , Convert.ToInt64(EstimationCodeEnd.Value)
                                                                        , Convert.ToInt64(V_MedProductType.Value));

            decimal TotalAmount = 0;

            if (dsEstimationDrugDept1.spDrugDeptEstimationForPoDetails_ByIDParent != null && dsEstimationDrugDept1.spDrugDeptEstimationForPoDetails_ByIDParent.Rows.Count > 0)
            {

                for (int i = 0; i < dsEstimationDrugDept1.spDrugDeptEstimationForPoDetails_ByIDParent.Rows.Count; i++)
                {
                    TotalAmount += Convert.ToDecimal(dsEstimationDrugDept1.spDrugDeptEstimationForPoDetails_ByIDParent.Rows[i]["AdjustedQty"]) * Convert.ToDecimal(dsEstimationDrugDept1.spDrugDeptEstimationForPoDetails_ByIDParent.Rows[i]["UnitPrice"]);
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
                    prefix1 = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am);
                }
                else
                {
                    temp1 = TotalAmount;
                    prefix1 = "";
                }
                Parameters["ReadMoneyTotalAmount"].Value = prefix1 + converter.Convert(temp1.ToString(), cutureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
            }
        }

        private void XRptEstimateDrugDept_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            PrintingSystem.PageSettings.Landscape = true;
            FillData();
        }
    }
}
