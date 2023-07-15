using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IPclExamTypeListing
    {
        PagedSortableCollectionView<PCLExamType> PclExamTypes { get; }
        PCLExamTypeSearchCriteria SearchCriteria { get; }
        PCLExamType SelectedPCLExamType { get; set; }
        void StartSearching();
        void ClearItems();
    }
}
