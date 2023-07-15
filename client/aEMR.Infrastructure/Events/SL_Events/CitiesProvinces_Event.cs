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

namespace aEMR.Infrastructure.Events
{
    public class CitiesProvinces_Event
    {
    }

    public class dgListDblClickSelectCitiesProvinces_Event
    {
        public object CitiesProvinces_Current { get; set; }
    }

    public class dgCitiesProvincesListClickSelectionChanged_Event
    {
        public object CitiesProvinces_Current { get; set; }
    }

    public class CitiesProvinces_Event_Save
    {
        public long FormType { get; set; }
        public object Result { get; set; }
    }
}
