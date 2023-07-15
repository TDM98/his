using System;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Su dung cho autocomplete box
    /// </summary>
    public interface Iicd10Listing
    {
        PagedSortableCollectionView<DiseasesReference> AllItems { get; }
        
        DiseasesReference SelectedItem { get; }
        string SearchString { get; set; }
        bool IsLoading { get; }
        void Clear();
        void SetText(string str);
    }
}
