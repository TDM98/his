using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IPrescriptionSearch
    {
        PrescriptionSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<Prescription> PrescriptionList { get; set; }
        byte FormType { get; set; }
    }
}
