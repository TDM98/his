using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    using DevExpress.Xpf.Printing;
    public class PharmacyReturnDrugReportModel : ReportModelBase
    {
        public PharmacyReturnDrugReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XptReturnDrug")
        {

        }

    }
    public class PharmacyReturnDrugInsuranceReportModel : ReportModelBase
    {
        public PharmacyReturnDrugInsuranceReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XptReturnDrugInsurance")
        {

        }

    }
}
