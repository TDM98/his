using System;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Su dung cho autocomplete box
    /// </summary>
    public interface IHospitalAutoCompleteEdit
    {
        string DisplayText { get; set; }
        PagedSortableCollectionView<Hospital> HospitalList { get; }
        string SearchCriteria { get; }
        Hospital SelectedHospital { get; set; }
        bool IsChildWindown { get; }
        bool DisplayHiCode { get; set; }
        bool IsUpdate { get; set; }
        bool IsPaperReferal { get; set; }        
        string Title { get; set; }
    }
}
