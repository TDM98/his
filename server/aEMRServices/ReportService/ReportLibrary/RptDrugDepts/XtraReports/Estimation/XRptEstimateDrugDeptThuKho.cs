using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation
{
    public partial class XRptEstimateDrugDeptThuKho : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEstimateDrugDeptThuKho()
        {
            InitializeComponent();
        }

        public void FillData()
        {
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
        }

        private void XRptEstimateDrugDeptThuKho_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
