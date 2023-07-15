using System;

namespace aEMR.ViewContracts
{
    public interface IAbout
    {
        
    }

    public interface IBusyIndicatorPopupView
    {        
        string strBusyMsg { get; set; }
        bool IsPopupBusy { get; set; }
    }
}
