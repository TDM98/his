using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
     using DevExpress.Xpf.Printing;
    public class RequestDrugModel:ReportModelBase
    {
        public RequestDrugModel()
            : base("eHCMS.ReportLib.RptPharmacies.XptRequestDrug")
        {

        }
    }
    public class RequestDrugPharmacyModel : ReportModelBase
    {
        public RequestDrugPharmacyModel()
            : base("eHCMS.ReportLib.RptPharmacies.XptRequestDrugPharmacy")
        {

        }
    }
   
}
