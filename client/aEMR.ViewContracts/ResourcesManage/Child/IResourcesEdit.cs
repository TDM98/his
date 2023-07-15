using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace Ax.ViewContracts.SL
{
    public interface IResourcesEdit
    {
        Resources curResource { get; set; }
        ObservableCollection<Lookup> refResourceUnit { get; set; }
        ObservableCollection<ResourceGroup> refResourceGroup { get; set; }
        ObservableCollection<ResourceType> refResourceType { get; set; }
        ObservableCollection<Supplier> refSuplier { get; set; }
    }
}
