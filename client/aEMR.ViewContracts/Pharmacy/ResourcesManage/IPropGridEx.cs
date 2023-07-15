using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPropGridEx
    {
        ObservableCollection<ResourcePropLocations> lstNewResourcePropLocations {get;set;}
    }
}
