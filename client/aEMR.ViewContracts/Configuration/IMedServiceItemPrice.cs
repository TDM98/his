using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.ViewContracts.Configuration
{
    public interface IMedServiceItemPrice
    {
        object IDeptMedServiceItems_Current { get; set; }
        
        object ISearchCriteria { get; set; }
        void LoadForm();

        bool IStatusFromDate { get; set; }
        bool IStatusToDate { get; set; }
        bool IStatusCheckFindDate { get; set; }
    }
}
