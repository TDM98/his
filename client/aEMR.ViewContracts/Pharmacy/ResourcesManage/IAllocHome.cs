using System;

namespace aEMR.ViewContracts
{
    public interface IAllocHome
    {
        object leftContent { get; set; }

        object infoContent { get; set; }

        object gridContent { get; set; }

        long ResourceCategoryEnum { get; set; }
    }
}
