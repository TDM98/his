using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IOutwardFromPrescriptionSearch
    {
        PrescriptionSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<Prescription> PrescriptionList { get; set; }
    }
}
