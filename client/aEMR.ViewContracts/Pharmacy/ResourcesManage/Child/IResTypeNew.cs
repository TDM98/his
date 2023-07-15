using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IResTypeNew
    {
        ObservableCollection<ResourceGroup> refResourceGroup { get; set; }
    }
}
