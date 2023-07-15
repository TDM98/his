using System;
using DataEntities;

namespace Ax.ViewContracts.SL
{
    public interface IStoragesHistory
    {
        object GridPropHis { get; set; }
        ResourcePropLocations selectedResourcePropLocations { get; set; }
    }
}
