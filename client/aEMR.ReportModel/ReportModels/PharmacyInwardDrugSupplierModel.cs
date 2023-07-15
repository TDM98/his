using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    using DevExpress.Xpf.Printing;
    public class PharmacyInwardDrugSupplierModel : ReportModelBase
    {
        public PharmacyInwardDrugSupplierModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRptInwardDrugSupplier")
        {

        }
    }
    public class PharmacyPurchaseOrderReportModel : ReportModelBase
    {
        public PharmacyPurchaseOrderReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.Order.XRpt_PurchaseOrderPharmacy")
        {

        }
    }
    public class PharmacySupplierTemplateReportModel : ReportModelBase
    {
        public PharmacySupplierTemplateReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.XRpt_SupplierTemplate")
        {

        }
    }
}
