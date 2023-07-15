using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace Ax.ViewContracts.SL
{
    public interface IPropGridEx
    {
        ObservableCollection<ResourcePropLocations> lstNewResourcePropLocations {get;set;}
    }
}
