using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;
using System.Windows;

namespace aEMR.ViewContracts
{
    public interface IConsultationRoom
    {
        
    }

    public interface IConsultationSummaryView
    {
        Window GetViewWindow();
    }
}
