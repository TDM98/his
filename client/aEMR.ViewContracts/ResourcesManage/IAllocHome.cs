using System;

namespace Ax.ViewContracts.SL
{
    public interface IAllocHome
    {
        object leftContent { get; set; }

        object infoContent { get; set; }

        object gridContent { get; set; }

        long ResourceCategoryEnum { get; set; }
    }
}
