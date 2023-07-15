using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Su dung cho autocomplete box
    /// </summary>
    public interface IMedProductRemaingListing
    {
        ObservableCollection<RefGenMedProductDetails> MedProductList { get; set; }
        bool IsLoading { get; }
        void StartLoadingByIdList(Dictionary<long,List<long>> drugIdList);
        string Title { get; set; }
    }
}
