using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.ViewContracts
{
    public interface IShellViewModel
    {
        bool IsBusy { get; set; }
        string BusyContent { get; set; }
        string UserName { get; set; }
        string SiteName { get; set; }

        bool IsVisible { get; set; }
        bool isLogin { get; set; }        
        void ShowBusy(string strBusyText);
        void HideBusy();
    }
}
