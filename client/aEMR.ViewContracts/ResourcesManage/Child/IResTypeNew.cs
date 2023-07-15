using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace Ax.ViewContracts.SL
{
    public interface IResTypeNew
    {
        ObservableCollection<ResourceGroup> refResourceGroup { get; set; }
    }
}
