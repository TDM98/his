using System;
using DataEntities;

namespace Ax.ViewContracts.SL
{
    public interface IResourceInfoTrans
    {
        //object ActiveContent { get; set; }
        ResourcePropLocations selectedResourcePropLocations { get; set; }
        bool IsChildWindowForChonDiBaoTri { get; set; }
    }
}
