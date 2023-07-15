using System;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPropTranfer
    {
        object gridProp { get; set; }
        object treeDept { get; set; }
        ResourcePropLocations selectedResourcePropLocations { get; set; }
    }
}
