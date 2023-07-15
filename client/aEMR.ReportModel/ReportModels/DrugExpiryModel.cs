using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
     using DevExpress.Xpf.Printing;
    public class DrugExpiryModel:ReportModelBase
    {
        public DrugExpiryModel()
            : base("eHCMS.ReportLib.RptPharmacies.XptDrugExpiryDate")
        {

        }
    }
   
}
