using System;

namespace aEMR.ViewContracts
{
    public interface ITranfHome
    {
        object leftContent { get; set; }

        object infoContent { get; set; }

        object gridContent { get; set; }

        bool IsChildWindowForChonDiBaoTri { get; set; }
    }
}
