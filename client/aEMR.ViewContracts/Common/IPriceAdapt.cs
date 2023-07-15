using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DataEntities;

namespace aEMR.ViewContracts.Configuration
{
    public interface IPriceAdapt
    {
        string ServiceName { get; set; }
        decimal OldPrice { get; set; }
        decimal NewPrice { get; set; }
        string Comments { get; set; }
        bool IsOk { get; set; }
        void SetValue(string serviceName, decimal oldPrice);
    }
}
