

using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IPharmacyOutwardDrugReportSearch
    {
        SearchOutwardReport SearchCriteria { get; set; }
        PagedSortableCollectionView<PharmacyOutwardDrugReport> PharmacyOutwardDrugReportList { get; set; }
        string pageTitle { get; set; }
    }
}
