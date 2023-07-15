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
    public class Hospital_Event
    {
    }

    public class dgListDblClickSelectHospital_Event
    {
        public object Hospital_Current { get; set; }
    }

    public class dgHospitalListClickSelectionChanged_Event
    {
        public object Hospital_Current { get; set; }
    }

    public class Hospital_Event_Save
    {
        public object Result { get; set; }
    }
}
