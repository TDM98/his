using System;

namespace aEMR.ViewContracts
{
    public interface IAppUpdatedNotification
    {
        string Header { get; set; }
        string Content { get; set; }
    }
}
