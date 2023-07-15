using System;
using DataEntities;

namespace Ax.ViewContracts.SL
{
    public interface IPropTranfer
    {
        object gridProp { get; set; }
        object treeDept { get; set; }
        ResourcePropLocations selectedResourcePropLocations { get; set; }
    }
}
