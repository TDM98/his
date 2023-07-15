using System;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IResourceInfoTrans
    {
        //object ActiveContent { get; set; }
        ResourcePropLocations selectedResourcePropLocations { get; set; }
        bool IsChildWindowForChonDiBaoTri { get; set; }
    }
}
