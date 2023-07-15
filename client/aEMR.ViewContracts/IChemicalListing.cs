using System;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Su dung cho autocomplete box
    /// </summary>
    public interface IChemicalListing
    {
        PagedSortableCollectionView<RefGenMedProductDetails> MedProductList { get; }
        MedProductSearchCriteria SearchCriteria { get; }
        RefGenMedProductDetails SelectedMedProduct { get; }
        bool IsLoading { get; }
        void Clear();
    }
}
