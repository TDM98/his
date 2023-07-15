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
    public class Job_Event
    {
    }

    public class dgListDblClickSelectJob_Event
    {
        public object Job_Current { get; set; }
    }

    public class dgJobListClickSelectionChanged_Event
    {
        public object Job_Current { get; set; }
    }

    public class Job_Event_Save
    {
        public object Result { get; set; }
    }
}
