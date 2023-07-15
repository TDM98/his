using System;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IStoragesHistory
    {
        object GridPropHis { get; set; }
        ResourcePropLocations selectedResourcePropLocations { get; set; }
    }
}
