using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IResTypeListSelector
    {
        ObservableCollection<ResourceTypeLists> RscrTypeListsOld { get; set; }
        ObservableCollection<RefMedicalServiceType> refResourceTypes { get; set; }
    }
}
